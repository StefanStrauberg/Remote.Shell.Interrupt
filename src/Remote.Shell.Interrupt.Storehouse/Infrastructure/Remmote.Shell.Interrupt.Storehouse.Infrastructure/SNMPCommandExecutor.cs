namespace Remmote.Shell.Interrupt.Storehouse.Infrastructure;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    async Task<List<SNMPResponse>> ISNMPCommandExecutor.WalkCommand(string host,
                                                                    string community,
                                                                    string oid,
                                                                    CancellationToken cancellationToken)
    {
        List<SNMPResponse> result = [];

        try
        {
            var target = new UdpTarget(IPAddress.Parse(host), 161, 2000, 1);
            var communityString = new OctetString(community);

            var agentParams = new AgentParameters(communityString)
            {
                Version = SnmpVersion.Ver2
            };

            var pdu = new Pdu(PduType.GetBulk)
            {
                MaxRepetitions = 10
            };

            pdu.VbList.Add(new Oid(oid));

            var response = await Task.Run(() =>
            {
                try
                {
                    return target.Request(pdu, agentParams);
                }
                catch (Exception ex)
                {
                    throw new Exception("SNMP request failed", ex);
                }
            }, cancellationToken);

            if (response != null && response.Pdu.VbList.Count > 0)
            {
                foreach (Vb item in response.Pdu.VbList)
                {
                    if (oid == "1.3.6.1.2.1.4.22.1.2") // Пример проверки OID
                    {
                        result.Add(new SNMPResponse()
                        {
                            OID = item.Oid.ToString(),
                            Data = ConvertToMacAddress(item.Value.ToString())
                        });
                    }
                    else
                    {
                        result.Add(new SNMPResponse()
                        {
                            OID = item.Oid.ToString(),
                            Data = item.Value.ToString()
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException(ex.Message);
        }

        return result;
    }

    static string ConvertToMacAddress(string value)
    {
        var bytes = value.Split(':')
                             .Select(s => Convert.ToByte(s, 16))
                             .ToArray();

        if (bytes.Length != 6) throw new ArgumentException("Invalid MAC address length");

        return string.Join(":", bytes.Select(b => b.ToString("X2")));
    }

    async Task<SNMPResponse> ISNMPCommandExecutor.GetCommand(string host,
                                                     string community,
                                                     string oid,
                                                     CancellationToken cancellationToken)
    {
        SNMPResponse result = new();

        try
        {
            var target = new UdpTarget(IPAddress.Parse(host), 161, 2000, 1);
            var communityString = new OctetString(community);

            var agentParams = new AgentParameters(communityString)
            {
                Version = SnmpVersion.Ver2
            };

            var pdu = new Pdu(PduType.Get);
            pdu.VbList.Add(new Oid(oid));

            var response = await Task.Run(() =>
            {
                try
                {
                    return target.Request(pdu, agentParams);
                }
                catch (Exception ex)
                {
                    throw new Exception("SNMP request failed", ex);
                }
            }, cancellationToken);

            if (response != null && response.Pdu.VbList.Count > 0)
            {
                var item = response.Pdu.VbList.FirstOrDefault();
                if (item != null)
                {
                    result.OID = item.Oid.ToString();
                    result.Data = item.Value.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException(ex.Message);
        }

        return result;
    }
}