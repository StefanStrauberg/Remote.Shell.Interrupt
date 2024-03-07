namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal class SNMPCommandExecutor : ISNMPCommandExecutor
{
    public event ISNMPCommandExecutor.CommandExecutorHandler? Notify;

    async Task<Dictionary<string, List<string>>> ISNMPCommandExecutor.WalkCommand(SNMPParams sNMPParams,
                                                                                  string oid,
                                                                                  CancellationToken cancellationToken)
    {
        var community = new OctetString(sNMPParams.Community);
        var param = new AgentParameters(community)
        {
            Version = SnmpVersion.Ver2
        };
        var agent = new IpAddress(sNMPParams.Host);
        var target = new UdpTarget((IPAddress)agent,
                                   161,
                                   2000,
                                   1);

        var rootOid = new Oid(oid);
        var lastOid = (Oid)rootOid.Clone();
        var pdu = new Pdu(PduType.GetBulk)
        {
            NonRepeaters = 0,
            MaxRepetitions = 5
        };

        Dictionary<string, List<string>> response = [];

        while (lastOid != null)
        {
            if (pdu.RequestId != 0)
                pdu.RequestId += 1;

            pdu.VbList.Clear();
            pdu.VbList.Add(lastOid);
            SnmpV2Packet result = (SnmpV2Packet)target.Request(pdu, param);

            if (result != null)
            {
                if (result.Pdu.ErrorStatus != 0)
                {
                    Notify?.Invoke("Error in SNMP reply. Error {0} index {1}",
                                   result.Pdu.ErrorStatus,
                                   result.Pdu.ErrorIndex);
                    break;
                }
                else
                {
                    foreach (Vb v in result.Pdu.VbList)
                    {
                        if (rootOid.IsRootOf(v.Oid))
                        {
                            response.Add(v.Oid.ToString(),
                                         [SnmpConstants.GetTypeName(v.Value.Type), 
                                          v.Value.ToString()]);
                            if (v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW)
                                lastOid = null;
                            else
                                lastOid = v.Oid;
                        }
                        else
                            lastOid = null;
                    }
                }
            }
            else
                Notify?.Invoke("No response received from SNMP agent.");
        }
        
        target.Close();
        return await Task.FromResult(response);
    }

    public Task<Dictionary<string, List<string>>> GetCommand(SNMPParams sNMPParams,
                                                             string oid,
                                                             CancellationToken cancellationToken)
    {
        var community = new OctetString(sNMPParams.Community);
        var param = new AgentParameters(community)
        {
            Version = SnmpVersion.Ver2
        };
        var agent = new IpAddress(sNMPParams.Host);
        var target = new UdpTarget((IPAddress)agent,
                                   161,
                                   2000,
                                   1);

        var pdu = new Pdu(PduType.Get);

        pdu.VbList.Add(oid);

        var sb = new StringBuilder();
        Dictionary<string, List<string>> response = [];

        SnmpV2Packet result = (SnmpV2Packet)target.Request(pdu, param);

        if (result != null)
            {
                if (result.Pdu.ErrorStatus != 0)
                    Notify?.Invoke("Error in SNMP reply. Error {0} index {1}",
                                   result.Pdu.ErrorStatus,
                                   result.Pdu.ErrorIndex);
                else
                    response.Add(result.Pdu.VbList[0].Oid.ToString(),
                                 [SnmpConstants.GetTypeName(result.Pdu.VbList[0].Value.Type), 
                                  result.Pdu.VbList[0].Value.ToString()]);
            }
            else
                Notify?.Invoke("No response received from SNMP agent.");
            
            target.Close();
            return Task.FromResult(response);
    }
}