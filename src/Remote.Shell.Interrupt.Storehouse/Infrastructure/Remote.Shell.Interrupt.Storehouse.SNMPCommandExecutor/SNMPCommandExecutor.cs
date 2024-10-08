namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    public async Task<SNMPResponse> GetCommand(string host, string community, string oid, CancellationToken cancellationToken, bool toHex = false)
    {
        var result = new SNMPResponse();
        var version = VersionCode.V2;
        var target = new IPEndPoint(IPAddress.Parse(host), 161);
        var communityString = new OctetString(community);
        var currentOid = new ObjectIdentifier(oid);

        try
        {
            var response = await Messenger.GetAsync(version,
                                                    target,
                                                    communityString,
                                                    [new(currentOid)],
                                                    cancellationToken);
            if (response.Count > 0)
            {
                var item = response.FirstOrDefault();
                if (item is not null)
                {
                    result.OID = item.Id.ToString();
                    result.Data = toHex ? ConvertSnmpDataToHex(item.Data) : item.Data.ToString();
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

    public async Task<List<SNMPResponse>> WalkCommand(string host, string community, string oid, CancellationToken cancellationToken, bool toHex = false, int repetitions = 20)
    {
        var result = new List<SNMPResponse>();
        var version = VersionCode.V2;
        var target = new IPEndPoint(IPAddress.Parse(host), 161);
        var communityString = new OctetString(community);
        var currentOid = new ObjectIdentifier(oid);
        var userRegistry = new Lextm.SharpSnmpLib.Security.UserRegistry();
        var maxRepetitions = repetitions;

        try
        {
            while (true)
            {
                var message = new GetBulkRequestMessage(0,
                                                        version,
                                                        communityString,
                                                        0,
                                                        maxRepetitions,
                                                        [new Variable(currentOid)]);

                var response = await message.GetResponseAsync(target, registry: userRegistry, cancellationToken);

                // Проверка на ошибки в ответе
                if (response.Pdu().ErrorStatus.ToInt32() != 0)
                {
                    throw new Exception($"Error in response: {response.Pdu().ErrorStatus} (OID: {currentOid})");
                }

                var responseVariables = response.Pdu().Variables;
                if (responseVariables.Count == 0)
                {
                    break; // Нет больше данных
                }

                bool foundBaseOid = false; // Флаг для проверки наличия базового OID

                foreach (var item in response.Pdu().Variables)
                {
                    var itemOid = item.Id.ToString();

                    if (itemOid.StartsWith(oid))
                    {
                        result.Add(new SNMPResponse()
                        {
                            OID = item.Id.ToString(),
                            Data = toHex ? ConvertSnmpDataToHex(item.Data) : item.Data.ToString()
                        });

                        foundBaseOid = true; // Базовый OID найден
                    }
                    else
                    {
                        foundBaseOid = false;
                        break;
                    }
                }

                // Если базовый OID не найден, выходим из цикла
                if (!foundBaseOid)
                {
                    break; // Нет больше данных для получения
                }

                // Обновляем текущий OID для следующего запроса
                // Используем последний полученный OID для следующего запроса
                currentOid = responseVariables.Last().Id; // Для следующего запроса
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

    public static string ConvertSnmpDataToHex(ISnmpData snmpData)
    {
        // Проверяем, является ли snmpData экземпляром OctetString
        if (snmpData is OctetString octetString)
        {
            // Получаем массив байтов из OctetString
            byte[] bytes = octetString.GetRaw();

            // Создаем строку в формате Hex
            return string.Join(" ", bytes.Select(b => b.ToString("X2")));
        }
        else
        {
            throw new ArgumentException("Provided ISnmpData is not an OctetString.");
        }
    }
}