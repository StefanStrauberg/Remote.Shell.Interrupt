namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal class SNMPCommandExecutor : ISNMPCommandExecutor
{
    public event ISNMPCommandExecutor.CommandExecutorHandler? Notify;

    async Task<string> ISNMPCommandExecutor.WalkCommand(SNMPParams snmpParams,
                                                        string oid,
                                                        CancellationToken cancellationToken)
    {
        var community = new OctetString(snmpParams.Community);
        var param = new AgentParameters(community)
        {
            Version = SnmpVersion.Ver2
        };
        var agent = new IpAddress(snmpParams.Host);
        var target = new UdpTarget((IPAddress)agent,
                                   snmpParams.Port,
                                   2000,
                                   1);

        var rootOid = new Oid(oid);
        var lastOid = (Oid)rootOid.Clone();
        var pdu = new Pdu(PduType.GetBulk)
        {
            NonRepeaters = 0,
            MaxRepetitions = 5
        };

        var sb = new StringBuilder();

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
                    sb.Append($"Error in SNMP reply. Error {result.Pdu.ErrorStatus} index {result.Pdu.ErrorIndex}");
                    Notify?.Invoke(sb.ToString());
                    lastOid = null;
                    break;
                }
                else
                {
                    foreach (Vb v in result.Pdu.VbList)
                    {
                        if (rootOid.IsRootOf(v.Oid))
                        {
                            sb.Append($"{v.Oid.ToString()} ({SnmpConstants.GetTypeName(v.Value.Type)}): {v.Value.ToString()}");
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
            {
                sb.Append("No response received from SNMP agent.");
                Notify?.Invoke(sb.ToString());
            }
        }
        target.Close();

        return await Task.FromResult(sb.ToString());
    }
}