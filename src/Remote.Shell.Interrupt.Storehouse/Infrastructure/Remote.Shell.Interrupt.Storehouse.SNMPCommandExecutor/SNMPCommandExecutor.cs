using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    // //Выполняет SNMP Walk запрос
    // async Task<List<SNMPResponse>> ISNMPCommandExecutor.WalkCommand(string host,
    //                                                                 string community,
    //                                                                 string oid,
    //                                                                 CancellationToken cancellationToken)
    // {
    //     var result = new List<SNMPResponse>();

    //     // Создание целевого UDP соединения
    //     var target = new UdpTarget(IPAddress.Parse(host), 161, 20000, 1);
    //     var communityString = new OctetString(community);
    //     var agentParams = new AgentParameters(communityString)
    //     {
    //         Version = SnmpVersion.Ver2
    //     };
    //     Oid currentOid = new(oid);
    //     bool moreData = true;

    //     try
    //     {
    //         // Цикл для выполнения SNMP Walk, пока есть данные
    //         while (moreData)
    //         {
    //             var pdu = new Pdu(PduType.GetBulk)
    //             {
    //                 NonRepeaters = 0,
    //                 MaxRepetitions = 10
    //             };
    //             pdu.VbList.Add(currentOid);

    //             // Использование TaskCompletionSource для асинхронного ожидания ответа
    //             var tcs = new TaskCompletionSource<SnmpPacket>();
    //             SnmpAsyncResponse callback = (result, packet) =>
    //             {
    //                 if (result == AsyncRequestResult.NoError)
    //                 {
    //                     tcs.SetResult(packet);
    //                 }
    //                 else
    //                 {
    //                     tcs.SetException(new SNMPBadRequestException("Request failed."));
    //                 }
    //             };

    //             // Отправка запроса
    //             target.RequestAsync(pdu, agentParams, callback);

    //             using (cancellationToken.Register(() => tcs.SetCanceled()))
    //             {
    //                 var responsePacket = await tcs.Task;

    //                 if (responsePacket != null && responsePacket.Pdu.VbList.Count > 0)
    //                 {
    //                     moreData = false; // Прекратить цикл, если есть данные

    //                     foreach (Vb item in responsePacket.Pdu.VbList)
    //                     {
    //                         if (item.Oid.ToString().StartsWith(oid))
    //                         {
    //                             result.Add(new SNMPResponse()
    //                             {
    //                                 OID = item.Oid.ToString(),
    //                                 Data = item.Value.ToString() ?? string.Empty
    //                             });

    //                             moreData = true; // Продолжить цикл, если есть больше данных
    //                         }
    //                     }
    //                     // Обновить OID для следующего запроса
    //                     currentOid = new Oid(responsePacket.Pdu.VbList.Last().Oid.ToString()); // Update OID for next request
    //                 }
    //             }
    //         }
    //     }
    //     catch (OperationCanceledException)
    //     {
    //         throw new SNMPBadRequestException("The SNMP Walk operation was canceled.");
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new SNMPBadRequestException($"Error during SNMP Walk: {ex.Message}", ex);
    //     }
    //     finally
    //     {
    //         // Закрытие соединения
    //         target.Close();
    //     }
    //     return result;
    // }

    // // Выполняет SNMP Get запрос
    // async Task<SNMPResponse> ISNMPCommandExecutor.GetCommand(string host,
    //                                                  string community,
    //                                                  string oid,
    //                                                  CancellationToken cancellationToken)
    // {
    //     SNMPResponse result = new();
    //     var target = new UdpTarget(IPAddress.Parse(host), 161, 20000, 1);
    //     var communityString = new OctetString(community);

    //     var agentParams = new AgentParameters(communityString)
    //     {
    //         Version = SnmpVersion.Ver2
    //     };

    //     try
    //     {

    //         var pdu = new Pdu(PduType.Get);
    //         pdu.VbList.Add(new Oid(oid));

    //         // Использование TaskCompletionSource для асинхронного ожидания ответа
    //         var tcs = new TaskCompletionSource<SnmpPacket>();
    //         SnmpAsyncResponse callback = (result, packet) =>
    //         {
    //             if (result == AsyncRequestResult.NoError)
    //             {
    //                 tcs.SetResult(packet);
    //             }
    //             else
    //             {
    //                 tcs.SetException(new SNMPBadRequestException("Request failed."));
    //             }
    //         };

    //         // Отправка запроса
    //         target.RequestAsync(pdu, agentParams, callback);

    //         using (cancellationToken.Register(() => tcs.SetCanceled()))
    //         {
    //             var responsePacket = await tcs.Task;

    //             if (responsePacket != null && responsePacket.Pdu.VbList.Count > 0)
    //             {
    //                 var item = responsePacket.Pdu.VbList.FirstOrDefault();
    //                 if (item != null)
    //                 {
    //                     result.OID = item.Oid.ToString();
    //                     result.Data = item.Value.ToString() ?? string.Empty;
    //                 }
    //             }
    //         }
    //     }
    //     catch (OperationCanceledException)
    //     {
    //         throw new SNMPBadRequestException("The SNMP Get operation was canceled.");
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new SNMPBadRequestException($"Error during SNMP Get: {ex.Message}", ex);
    //     }
    //     finally
    //     {
    //         // Закрытие соединения
    //         target.Close();
    //     }
    //     return result;
    // }
    public async Task<SNMPResponse> GetCommand(string host, string community, string oid, CancellationToken cancellationToken)
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
                    result.Data = item.Data.ToString() ?? string.Empty;
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

    public async Task<List<SNMPResponse>> WalkCommand(string host, string community, string oid, CancellationToken cancellationToken)
    {
        var result = new List<SNMPResponse>();
        var version = VersionCode.V2;
        var target = new IPEndPoint(IPAddress.Parse(host), 161);
        var communityString = new OctetString(community);
        var currentOid = new ObjectIdentifier(oid);
        var userRegistry = new Lextm.SharpSnmpLib.Security.UserRegistry();

        try
        {
            while (true)
            {
                var message = new GetBulkRequestMessage(0,
                                                        version,
                                                        communityString,
                                                        0,
                                                        10,
                                                        [new Variable(currentOid)]);

                var response = await message.GetResponseAsync(target, registry: userRegistry, cancellationToken);

                // Проверка на ошибки в ответе
                if (response.Pdu().ErrorStatus.ToInt32() != 0)
                {
                    throw new Exception($"Error in response: {response.Pdu().ErrorStatus}");
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
                        if (item.Data.TypeCode == SnmpType.OctetString)
                        {
                            result.Add(new SNMPResponse()
                            {
                                OID = item.Id.ToString(),
                                Data = ConvertSnmpDataToHex(item.Data),
                            });
                        }
                        else
                        {
                            result.Add(new SNMPResponse()
                            {
                                OID = item.Id.ToString(),
                                Data = item.Data.ToString(),
                            });
                        }

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

            // Если длина массива байтов равна 6, считаем это MAC-адресом
            if (bytes.Length == 6)
            {
                // Создаем строку в формате Hex
                return string.Join(" ", bytes.Select(b => b.ToString("X2")));
            }

            // Если длина не 6, создаем строку
            return snmpData.ToString();
        }
        else
        {
            throw new ArgumentException("Provided ISnmpData is not an OctetString.");
        }
    }
}