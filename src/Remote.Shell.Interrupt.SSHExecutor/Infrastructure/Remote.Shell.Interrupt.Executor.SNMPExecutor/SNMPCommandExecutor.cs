namespace Remote.Shell.Interrupt.Executor.SNMPExecutor;

internal class SNMPCommandExecutor : ISNMPCommandExecutor
{
    public event ISNMPCommandExecutor.CommandExecutorHandler? Notify;

    async Task<string> ISNMPCommandExecutor.ExecuteCommands(SNMPParams snmpParams,
                                                            List<string> oids,
                                                            CancellationToken cancellationToken)
    {
        foreach (var oid in oids)
        {    
            var result = await Messenger.GetAsync(VersionCode.V2,
                                                new IPEndPoint(IPAddress.Parse(snmpParams.HostName),
                                                                snmpParams.Port),
                                                new OctetString("public"),
                                                [new Variable(new ObjectIdentifier(oids[0]))],
                                                cancellationToken);
        }
        return "Done";
    }
}