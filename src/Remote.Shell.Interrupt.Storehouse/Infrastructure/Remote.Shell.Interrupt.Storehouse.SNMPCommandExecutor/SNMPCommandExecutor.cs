namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    // SNMP runs over UDP with no built-in connection concept, so an unresponsive device (down,
    // firewalled with no ICMP unreachable, or simply not listening on 161) never fails on its
    // own - the library just waits for a reply that's never coming. Neither call has a timeout
    // of its own (SharpSnmpLib's *Async overloads only take a CancellationToken), and the token
    // the API passes through is just the HTTP request's RequestAborted - which never fires on
    // its own unless the client disconnects. Without an explicit deadline here, a single
    // unreachable device call would hang the request (and the thread handling it) indefinitely.
    const int GetTimeoutMs = 5_000;
    const int WalkTimeoutMs = 30_000;

    // A device that never returns EndOfMibView/NoSuchObject and never leaves the requested
    // subtree (a buggy agent, or one that starts repeating OIDs) would otherwise keep
    // WalkCommand's loop going forever even with the wall-clock timeout above bounding any
    // single round-trip - this bounds the number of round-trips too. 10,000 GETBULK calls at
    // the default 20 repetitions/call is up to 200,000 OIDs, comfortably above a real router's
    // interface/ARP/MAC/VLAN table.
    const int MaxWalkIterations = 10_000;

    public async Task<SNMPResponse> GetCommand(string host, string community, string oid, CancellationToken cancellationToken, bool toHex = false)
    {
        var result = new SNMPResponse();
        var version = VersionCode.V2;

        try
        {
            if (!IPAddress.TryParse(host, out var hostAddress))
                throw new SNMPBadRequestException($"'{host}' is not a valid IP address.");

            var target = new IPEndPoint(hostAddress, 161);
            var communityString = new OctetString(community);
            var currentOid = new ObjectIdentifier(oid);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(GetTimeoutMs));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var response = await Messenger.GetAsync(version, target, communityString, [new(currentOid)], linkedCts.Token);

            if (response.Count > 0)
            {
                var item = response[0];
                result.OID = item.Id.ToString();
                result.Data = toHex ? ConvertSnmpDataToHex(item.Data) : item.Data.ToString();
            }
        }
        catch (OperationCanceledException)
        {
            throw new SNMPBadRequestException(
                $"The SNMP Get operation was canceled, or the device at '{host}' did not respond within {GetTimeoutMs}ms.");
        }
        catch (SNMPBadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException($"Error during SNMP Get: {ex.Message}", ex);
        }
        return result;
    }

    public async Task<List<SNMPResponse>> WalkCommand(string host, string community, string oid, CancellationToken cancellationToken, bool toHex = false, int repetitions = 20)
    {
        var result = new List<SNMPResponse>();
        var version = VersionCode.V2;
        var userRegistry = new Lextm.SharpSnmpLib.Security.UserRegistry();
        var maxRepetitions = repetitions;

        try
        {
            if (!IPAddress.TryParse(host, out var hostAddress))
                throw new SNMPBadRequestException($"'{host}' is not a valid IP address.");

            var target = new IPEndPoint(hostAddress, 161);
            var communityString = new OctetString(community);
            var currentOid = new ObjectIdentifier(oid);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(WalkTimeoutMs));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            var walkToken = linkedCts.Token;

            var iterations = 0;

            while (true)
            {
                if (++iterations > MaxWalkIterations)
                    throw new SNMPBadRequestException(
                        $"SNMP Walk of '{oid}' on '{host}' exceeded the maximum of {MaxWalkIterations} GETBULK round-trips without reaching the end of the subtree.");

                var message = new GetBulkRequestMessage(0, version, communityString, 0, maxRepetitions, [new Variable(currentOid)]);
                var response = await message.GetResponseAsync(target, registry: userRegistry, walkToken);

                if (response.Pdu().ErrorStatus.ToInt32() is not 0)
                    throw new Exception($"Error in response: {response.Pdu().ErrorStatus} (OID: {currentOid})");

                if (response.Pdu().Variables.Any(v => v.Data is null))
                    throw new SNMPBadRequestException("Received SNMP variable with null data.");

                var responseVariables = response.Pdu().Variables;

                if (responseVariables.Count is 0)
                    break;

                Variable? lastMatchingVariable = null;
                var reachedEndOfMib = false;

                foreach (var item in response.Pdu().Variables)
                {
                    var itemOid = item.Id.ToString();
                    var data = item.Data;

                    if (data is EndOfMibView or NoSuchObject or NoSuchInstance)
                    {
                        reachedEndOfMib = true;
                        break;
                    }

                    if (IsWithinSubtree(itemOid, oid))
                    {
                        string dataString;
                        try
                        {
                            dataString = toHex ? ConvertSnmpDataToHex(data) : data.ToString();
                        }
                        catch (ArgumentException)
                        {
                            dataString = data.ToString();
                        }

                        result.Add(new()
                        {
                            OID = item.Id.ToString(),
                            Data = dataString
                        });

                        lastMatchingVariable = item;
                    }
                    else
                    {
                        break;
                    }
                }

                if (reachedEndOfMib || lastMatchingVariable is null)
                    break;

                currentOid = lastMatchingVariable.Id;
            }
        }
        catch (OperationCanceledException)
        {
            throw new SNMPBadRequestException(
                $"The SNMP Walk operation was canceled, or the device at '{host}' did not respond within {WalkTimeoutMs}ms.");
        }
        catch (SNMPBadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException($"Error during SNMP Walk: {ex.Message}", ex);
        }
        return result;
    }

    public static string ConvertSnmpDataToHex(ISnmpData snmpData)
    {
        // Check whether snmpData is an OctetString instance.
        if (snmpData is OctetString octetString)
        {
            // Get the byte array from the OctetString.
            byte[] bytes = octetString.GetRaw();

            // Build a hex-formatted string.
            return string.Join(" ", bytes.Select(b => b.ToString("X2")));
        }
        else
        {
            throw new ArgumentException("Provided ISnmpData is not an OctetString.");
        }
    }

    /// <summary>
    /// Determines whether <paramref name="candidateOid"/> lies within the OID subtree rooted at
    /// <paramref name="rootOid"/>, comparing OID arcs instead of raw characters so that e.g.
    /// "1.3.6.1.2.1.10" does not match a walk root of "1.3.6.1.2.1.1".
    /// </summary>
    internal static bool IsWithinSubtree(string candidateOid, string rootOid)
        => candidateOid.StartsWith(rootOid, StringComparison.Ordinal)
           && (candidateOid.Length == rootOid.Length || candidateOid[rootOid.Length] == '.');
}