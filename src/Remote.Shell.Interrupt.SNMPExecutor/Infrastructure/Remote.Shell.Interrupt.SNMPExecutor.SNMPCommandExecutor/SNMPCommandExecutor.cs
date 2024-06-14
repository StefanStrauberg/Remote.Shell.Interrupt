using System.Text.RegularExpressions;

namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal class SNMPCommandExecutor : ISNMPCommandExecutor
{
    async Task<JsonArray> ISNMPCommandExecutor.WalkCommand(string host,
                                                           string community,
                                                           string oid,
                                                           CancellationToken cancellationToken)
    {
        var snmpHelper = new SnmpHelper(host, 161, community);
        JsonArray response;
        try
        {
            IList<Variable> result = await snmpHelper.WalkRequestAsync(oid);
            response = ConvertVariablesToJsonArray(result);
        }
        catch (EonaCat.Snmp.Exceptions.TimeoutException ex)
        {
            throw new SNMPBadRequestException($"SNMP request timed out: {ex.Message}");
        }
        catch (EonaCat.Snmp.Exceptions.SnmpException ex)
        {
            throw new InternalServerException($"SNMP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InternalServerException($"An error occurred: {ex.Message}");
        }

        return response;
    }

    async Task<JsonObject> ISNMPCommandExecutor.GetCommand(string host,
                                                           string community,
                                                           string oid,
                                                           CancellationToken cancellationToken)
    {
        var snmpHelper = new SnmpHelper(host, 161, community);
        JsonObject response;
        try
        {
            IList<Variable> result = await snmpHelper.GetRequestAsync(oid);
            response = ConvertVariablesToJsonObject(result);
        }
        catch (EonaCat.Snmp.Exceptions.TimeoutException ex)
        {
            throw new SNMPBadRequestException($"SNMP request timed out: {ex.Message}");
        }
        catch (EonaCat.Snmp.Exceptions.SnmpException ex)
        {
            throw new InternalServerException($"SNMP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InternalServerException($"An error occurred: {ex.Message}");
        }

        return response;
    }

    private static JsonArray ConvertVariablesToJsonArray(IList<Variable> variables)
    {
        var jArray = new JsonArray();

        foreach (var variable in variables)
        {
            var OID = variable.Id.ToString();

            // Create a new JObject for each variable
            var jObject = new JsonObject
            {
                ["OID"] = OID,
                ["TypeCode"] = variable.Data.TypeCode.ToString(),
                ["Data"] = Encoding.UTF8.GetString(variable.Data.ToBytes())
            };

            jArray.Add(jObject);
        }
        return jArray;
    }

    private static JsonObject ConvertVariablesToJsonObject(IList<Variable> variables)
    {
        var item = variables.FirstOrDefault();
        var jObject = new JsonObject();

        if (item is not null)
        {
            var OID = item.Id.ToString();
            jObject["OID"] = OID;
            jObject["TypeCode"] = item.Data.TypeCode.ToString();
            var data = Encoding.UTF8.GetString(item.Data.ToBytes());
            data = Regex.Replace(data, @"[\r\n\t]", "");
            jObject["Data"] = data;
        }

        return jObject;
    }
}