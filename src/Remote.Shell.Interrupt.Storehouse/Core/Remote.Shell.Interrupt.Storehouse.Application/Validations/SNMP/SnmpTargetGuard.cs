using System.Net;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.SNMP;

/// <summary>
/// Rejects IPv4 addresses that are never a real network device's management IP, shared by
/// every validator that accepts a raw SNMP target (<see cref="SNMPGetCommandValidator"/>,
/// <see cref="SNMPWalkCommandValidator"/>, and the workflow-execution validator).
/// </summary>
/// <remarks>
/// This is a denylist of special-purpose ranges, not an allowlist of specific subnets: routers
/// polled by this application legitimately live anywhere in private space (10/8, 172.16/12,
/// 192.168/16 - including the SNMP simulator's 192.168.101.0/24, see docker-compose.yml) or on
/// a public IP, so those stay allowed. What's blocked is the SSRF-shaped case - a host string
/// that resolves to the local machine or the local network stack itself rather than to a
/// distinct device the API reaches over the wire:
/// <list type="bullet">
/// <item>127.0.0.0/8 - loopback: the API host itself.</item>
/// <item>169.254.0.0/16 - link-local, which includes 169.254.169.254, the cloud metadata
/// endpoint on AWS/Azure/GCP; a host running there with an SNMP-only "Admin" credential
/// leaking would otherwise let SNMPExecutor/ExecuteWorkflow blind-probe it.</item>
/// <item>0.0.0.0/8 - "this network" (RFC 791), never a routable destination.</item>
/// <item>224.0.0.0/4 - multicast, not a single device.</item>
/// <item>240.0.0.0/4 - reserved for future use, including 255.255.255.255 (limited broadcast).</item>
/// </list>
/// The Host regex on every validator using this already restricts input to a syntactically
/// valid IPv4 literal (no hostnames), so there is no DNS-rebinding angle to also guard against
/// here - the string IS the address SNMP will actually connect to.
/// </remarks>
public static class SnmpTargetGuard
{
  public static bool IsAllowedTarget(string host)
  {
    if (!IPAddress.TryParse(host, out var address))
      return false;

    var octets = address.GetAddressBytes();

    if (octets.Length != 4)
      return false;

    return octets[0] switch
    {
      0 => false,                              // 0.0.0.0/8 - "this network"
      127 => false,                            // 127.0.0.0/8 - loopback
      169 when octets[1] == 254 => false,      // 169.254.0.0/16 - link-local / cloud metadata
      >= 224 and <= 239 => false,              // 224.0.0.0/4 - multicast
      >= 240 => false,                         // 240.0.0.0/4 - reserved + broadcast
      _ => true
    };
  }
}
