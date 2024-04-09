namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal class SNMPCommandExecutor : ISNMPCommandExecutor
{
    public event ISNMPCommandExecutor.CommandExecutorHandler? Notify;

    async Task<IList<Response>> ISNMPCommandExecutor.WalkCommand(SNMPParams sNMPParams,
                                                                 string oid,
                                                                 CancellationToken cancellationToken)
    {
        IList<Response> response = [];
        var snmpHelper = new SnmpHelper(sNMPParams.Host,
                                        161,
                                        sNMPParams.Community);
        try
        {
            IList<Variable> result = await snmpHelper.WalkRequestAsync(oid);
            foreach (var variable in result)
                response.Add(new Response(variable.Id.ToString(),
                                          variable.Data.TypeCode.ToString(),
                                          variable.Data.ToString()));
        }
        catch (EonaCat.Snmp.Exceptions.TimeoutException ex)
        {
            Notify?.Invoke($"SNMP request timed out: {ex.Message}");
        }
        catch (EonaCat.Snmp.Exceptions.SnmpException ex)
        {
            Notify?.Invoke($"SNMP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Notify?.Invoke($"An error occurred: {ex.Message}");
        }

        return response;
    }

    async Task<IList<Response>> ISNMPCommandExecutor.GetCommand(SNMPParams sNMPParams,
                                                                string oid,
                                                                CancellationToken cancellationToken)
    {
        IList<Response> response = [];
        var snmpHelper = new SnmpHelper(sNMPParams.Host,
                                        161,
                                        sNMPParams.Community);
        try
        {
            IList<Variable> result = await snmpHelper.GetRequestAsync(oid);
            foreach (var variable in result)
                response.Add(new Response(variable.Id.ToString(),
                                          variable.Data.TypeCode.ToString(),
                                          variable.Data.ToString()));
        }
        catch (EonaCat.Snmp.Exceptions.TimeoutException ex)
        {
            Notify?.Invoke($"SNMP request timed out: {ex.Message}");
        }
        catch (EonaCat.Snmp.Exceptions.SnmpException ex)
        {
            Notify?.Invoke($"SNMP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Notify?.Invoke($"An error occurred: {ex.Message}");
        }

        return response;
    }
}