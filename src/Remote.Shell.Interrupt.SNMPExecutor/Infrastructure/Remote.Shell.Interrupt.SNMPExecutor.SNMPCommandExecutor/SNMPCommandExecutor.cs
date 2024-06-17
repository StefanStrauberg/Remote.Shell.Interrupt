using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Remote.Shell.Interrupt.BuildingBlocks.Exceptions;

namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    async Task<IEnumerable<Information>> ISNMPCommandExecutor.WalkCommand(string host,
                                                                          string community,
                                                                          string oid,
                                                                          CancellationToken cancellationToken)
    {
        List<Information> response = [];

        try
        {
            var variables = new List<Variable>() { new(new ObjectIdentifier(oid)) };

            var walker = Messenger.Walk(version: VersionCode.V2,
                                        endpoint: new IPEndPoint(IPAddress.Parse(host), 161),
                                        community: new OctetString(community),
                                        table: new ObjectIdentifier(oid),
                                        list: variables,
                                        timeout: 15000,
                                        mode: WalkMode.WithinSubtree);

            for (int i = 0; i <= walker; i++)
            {
                response.Add(new Information()
                {
                    OID = variables[i].Id.ToString(),
                    Data = variables[i].Data.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            throw new InternalServerException($"An error occurred: {ex.Message}");
        }

        return response;
    }

    async Task<Information> ISNMPCommandExecutor.GetCommand(string host,
                                                            string community,
                                                            string oid,
                                                            CancellationToken cancellationToken)
    {
        Information response = new();

        try
        {
            var variables = new List<Variable> { new(new ObjectIdentifier(oid)) };

            var walker = Messenger.Get(version: VersionCode.V2,
                                       endpoint: new IPEndPoint(IPAddress.Parse(host), 161),
                                       community: new OctetString(community),
                                       variables: variables,
                                       timeout: 15000);

            if (walker is not null)
            {
                for (int i = 0; i <= walker.Count; i++)
                {
                    response.OID = variables[i].Id.ToString();
                    response.Data = variables[i].Data.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            throw new InternalServerException($"An error occurred: {ex.Message}");
        }

        return response;
    }

    // static List<Information> ConvertVariablesToList(IList<Variable> variables)
    // {
    //     List<Information> array = [];

    //     foreach (var variable in variables)
    //     {
    //         Information obj = new()
    //         {
    //             OID = variable.Id.ToString(),
    //             TypeCode = variable.Data.TypeCode.ToString(),
    //             Data = GetStringFromStringBytes(variable.Data.ToString())
    //         };
    //         array.Add(obj);
    //     }
    //     return array;
    // }

    // static string GetStringFromStringBytes(string hexString)
    // {
    //     byte[] byteArray = HexStringToByteArray(hexString);
    //     string decodedString = Encoding.UTF8.GetString(byteArray);
    //     string cleanedString = RemoveSpecialSymbols(decodedString);
    //     return cleanedString;
    // }

    // static byte[] HexStringToByteArray(string hexString)
    // {
    //     int numberChars = hexString.Length;
    //     byte[] bytes = new byte[numberChars / 2];
    //     for (int i = 0; i < numberChars; i += 2)
    //     {
    //         bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
    //     }
    //     return bytes;
    // }

    // static string RemoveSpecialSymbols(string input)
    // {
    //     string cleanedString = MyRegex().Replace(input, "");

    //     return cleanedString;
    // }

    // static Information ConvertVariablesToObject(IList<Variable> variables)
    // {
    //     var item = variables.FirstOrDefault();
    //     Information obj = new();

    //     if (item is not null)
    //     {
    //         obj.OID = item.Id.ToString();
    //         obj.TypeCode = item.Data.TypeCode.ToString();
    //         obj.Data = GetStringFromStringBytes(item.Data.ToString());
    //     }

    //     return obj;
    // }

    // [GeneratedRegex(@"[\x00-\x1F\x7F]")]
    // private static partial Regex MyRegex();
}