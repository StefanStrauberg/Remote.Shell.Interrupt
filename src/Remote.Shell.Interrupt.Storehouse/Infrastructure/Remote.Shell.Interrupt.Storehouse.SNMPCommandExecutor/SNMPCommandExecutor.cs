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
        var walkMode = WalkMode.Default;

        try
        {
            var response = new List<Variable>();
            await Messenger.WalkAsync(version,
                                      target,
                                      communityString,
                                      currentOid,
                                      response,
                                      walkMode,
                                      cancellationToken);

            foreach (var item in response)
            {
                result.Add(new SNMPResponse()
                {
                    OID = item.Id.ToString(),
                    Data = item.Data.ToString(),
                });
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