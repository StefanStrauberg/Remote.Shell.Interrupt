namespace Remmote.Shell.Interrupt.Storehouse.Infrastructure;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    async Task<List<SNMPResponse>> ISNMPCommandExecutor.WalkCommand(string host,
                                                                    string community,
                                                                    string oid,
                                                                    CancellationToken cancellationToken)
    {
        var result = new List<SNMPResponse>();

        var target = new UdpTarget(IPAddress.Parse(host), 161, 2000, 1);
        var communityString = new OctetString(community);
        var agentParams = new AgentParameters(communityString)
        {
            Version = SnmpVersion.Ver2
        };
        Oid currentOid = new(oid);
        bool moreData = true;

        while (moreData)
        {
            try
            {
                var pdu = new Pdu(PduType.GetBulk)
                {
                    NonRepeaters = 0,
                    MaxRepetitions = 10
                };
                pdu.VbList.Add(currentOid);
                // Используем TaskCompletionSource для преобразования асинхронного колбэка в Task
                var tcs = new TaskCompletionSource<SnmpPacket>();
                SnmpAsyncResponse callback = (result, packet) =>
                    {
                        if (result == AsyncRequestResult.NoError)
                        {
                            tcs.SetResult(packet);
                        }
                        else
                        {
                            tcs.SetException(new SNMPBadRequestException("Request failed."));
                        }
                    };

                target.RequestAsync(pdu, agentParams, callback);
                using (cancellationToken.Register(() => tcs.SetCanceled()))
                {

                    var responsePacket = await tcs.Task;

                    if (responsePacket != null && responsePacket.Pdu.VbList.Count > 0)
                    {
                        moreData = false;

                        foreach (Vb item in responsePacket.Pdu.VbList)
                        {
                            if (item.Oid.ToString().StartsWith(oid))
                            {
                                result.Add(new SNMPResponse()
                                {
                                    OID = item.Oid.ToString(),
                                    Data = item.Value.ToString() ?? string.Empty
                                });

                                moreData = true;
                            }
                        }
                        currentOid = new Oid(responsePacket.Pdu.VbList.Last().Oid.ToString()); // Update OID for next request
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new SNMPBadRequestException("The SNMP Walk operation was canceled.");
            }
            catch (Exception ex)
            {
                throw new SNMPBadRequestException($"Error during SNMP Walk: {ex.Message}", ex);
            }
        }
        return result;
    }

    async Task<SNMPResponse> ISNMPCommandExecutor.GetCommand(string host,
                                                     string community,
                                                     string oid,
                                                     CancellationToken cancellationToken)
    {
        SNMPResponse result = new();
        var target = new UdpTarget(IPAddress.Parse(host), 161, 2000, 1);
        var communityString = new OctetString(community);

        var agentParams = new AgentParameters(communityString)
        {
            Version = SnmpVersion.Ver2
        };

        try
        {

            var pdu = new Pdu(PduType.Get);
            pdu.VbList.Add(new Oid(oid));

            var tcs = new TaskCompletionSource<SnmpPacket>();
            SnmpAsyncResponse callback = (responseResult, packet) =>
            {
                if (responseResult == AsyncRequestResult.NoError)
                {
                    tcs.SetResult(packet);
                }
                else
                {
                    tcs.SetException(new SNMPBadRequestException($"Request failed with result: {responseResult}"));
                }
            };

            target.RequestAsync(pdu, agentParams, callback);

            using (cancellationToken.Register(() => tcs.SetCanceled()))
            {
                var responsePacket = await tcs.Task;

                if (responsePacket != null && responsePacket.Pdu.VbList.Count > 0)
                {
                    var item = responsePacket.Pdu.VbList.FirstOrDefault();
                    if (item != null)
                    {
                        result.OID = item.Oid.ToString();
                        result.Data = item.Value.ToString() ?? string.Empty;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw new SNMPBadRequestException("The SNMP Get operation was canceled.");
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException($"Error during SNMP Get: {ex.Message}", ex);
        }

        return result;
    }
}