namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Helper;

public sealed class Response(string id,
                string typeCode,
                string data)
{
    public string OID { get; set; } = id;
    public string TypeCode { get; set; } = typeCode;
    public string Data { get; set; } = data;
}