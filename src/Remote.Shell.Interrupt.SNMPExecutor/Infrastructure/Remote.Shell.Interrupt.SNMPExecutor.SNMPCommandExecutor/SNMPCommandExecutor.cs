namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal class SNMPCommandExecutor : ISNMPCommandExecutor
{
    async Task<JObject> ISNMPCommandExecutor.WalkCommand(string host,
                                                         string community,
                                                         string oid,
                                                         CancellationToken cancellationToken)
    {
        JObject response;
        var snmpHelper = new SnmpHelper(host, 161, community);
        try
        {
            IList<Variable> result = await snmpHelper.WalkRequestAsync(oid);
            response = ConvertVariablesToJsonObject(result);
        }
        catch (EonaCat.Snmp.Exceptions.TimeoutException ex)
        {
            throw new DataBadRequestException($"SNMP request timed out: {ex.Message}");
        }
        catch (EonaCat.Snmp.Exceptions.SnmpException ex)
        {
            throw new DataBadRequestException($"SNMP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InternalServerException($"An error occurred: {ex.Message}");
        }

        return response;
    }

    async Task<JObject> ISNMPCommandExecutor.GetCommand(string host,
                                                        string community,
                                                        string oid,
                                                        CancellationToken cancellationToken)
    {
        JObject response;
        var snmpHelper = new SnmpHelper(host, 161, community);
        try
        {
            IList<Variable> result = await snmpHelper.GetRequestAsync(oid);
            response = ConvertVariablesToJsonObject(result);
        }
        catch (EonaCat.Snmp.Exceptions.TimeoutException ex)
        {
            throw new DataBadRequestException($"SNMP request timed out: {ex.Message}");
        }
        catch (EonaCat.Snmp.Exceptions.SnmpException ex)
        {
            throw new DataBadRequestException($"SNMP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InternalServerException($"An error occurred: {ex.Message}");
        }

        return response;
    }

    private static JObject ConvertVariablesToJsonObject(IList<Variable> variables)
    {
        JObject result = [];

        foreach (var variable in variables)
        {
            JObject variableObject = new()
            {
                [nameof(variable.Id)] = variable.Id.ToString(),
                [nameof(variable.Data.TypeCode)] = variable.Data.TypeCode.ToString(),
                [nameof(variable.Data)] = variable.Data.ToString(),
            };
            result.Add(variableObject);
        }
        return result;
    }
}