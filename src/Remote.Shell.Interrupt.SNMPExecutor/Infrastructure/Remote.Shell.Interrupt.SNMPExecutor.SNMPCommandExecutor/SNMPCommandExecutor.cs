using System.Text;
using System.Text.RegularExpressions;

namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

internal partial class SNMPCommandExecutor : ISNMPCommandExecutor
{
    async Task<IEnumerable<Info>> ISNMPCommandExecutor.WalkCommand(string host,
                                                                   string community,
                                                                   string oid,
                                                                   CancellationToken cancellationToken)
    {
        List<Info> result = [];

        try
        {
            ObjectIdentifier targetOid = new(oid);
            IList<Variable> list = [];

            var response = await Messenger.WalkAsync(version: VersionCode.V2, // SNMP version
                                                     endpoint: new IPEndPoint(IPAddress.Parse(host), 161), // SNMP agent endpoint
                                                     community: new OctetString(community), // SNMP community string
                                                     table: targetOid, // Starting OID for the walk operation
                                                     list: list, // empty list since the variables are handled in the handler
                                                     mode: WalkMode.WithinSubtree,
                                                     token: cancellationToken); // Walk mode (e.g., WithinSubtree)

            foreach (var item in list)
            {
                // The media-dependent `physical' address. 
                if (oid == "1.3.6.1.2.1.4.22.1.2")
                {
                    result.Add(new Info()
                    {
                        OID = item.Id.ToString(),
                        Data = ConvertToMacAddress(item.Data.ToBytes()),
                    });
                }
                else
                {
                    result.Add(new Info()
                    {
                        OID = item.Id.ToString(),
                        Data = item.Data.ToString(),
                    });
                }
            }
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException(ex.Message);
        }

        return result;
    }

    static string ConvertToMacAddress(byte[] bytes)
    {
        //if (bytes.Length != 6) throw new ArgumentException("Invalid MAC address length");

        return string.Join(":", bytes.Skip(2)
                                     .ToArray()
                                     .Select(b => b.ToString("X2")));
    }

    async Task<Info> ISNMPCommandExecutor.GetCommand(string host,
                                                     string community,
                                                     string oid,
                                                     CancellationToken cancellationToken)
    {
        Info result = new();

        try
        {
            IList<Variable> variables = [new Variable(new ObjectIdentifier(oid))];

            var response = await Messenger.GetAsync(version: VersionCode.V2, // SNMP version
                                                    endpoint: new IPEndPoint(IPAddress.Parse(host), 161), // SNMP agent endpoint
                                                    community: new OctetString(community), // SNMP community string
                                                    variables: variables,
                                                    token: cancellationToken);

            if (response is not null)
            {
                var item = response.FirstOrDefault();
                if (item is not null)
                {
                    result.OID = item.Id.ToString();
                    result.Data = item.Data.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            throw new SNMPBadRequestException(ex.Message);
        }

        return result;
    }
}