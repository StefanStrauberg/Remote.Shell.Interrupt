using Newtonsoft.Json.Linq;

namespace Remote.Shell.Interrupt.SNMPExecutor.API.SNMP;

public record SNMPGetWalkResponse(JObject KeyValuePairs);
