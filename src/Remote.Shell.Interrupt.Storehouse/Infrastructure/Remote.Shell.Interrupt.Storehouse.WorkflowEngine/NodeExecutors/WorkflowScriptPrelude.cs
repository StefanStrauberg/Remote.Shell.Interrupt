namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Small JS helper library executed before every Script node's own <c>scriptSource</c>, so
/// scripts can call these instead of re-implementing them per node. They port the pure-function
/// OID/MAC/egress-port helpers that CreateNetworkDeviceCommandHandler used to call directly in
/// C# before device discovery moved into workflow graphs; those C# helpers are gone, so these
/// definitions are now the authoritative behavior.
/// </summary>
internal static class WorkflowScriptPrelude
{
  public const string Source = """
    function oidLast(oid) {
      var parts = String(oid).split('.');
      return parseInt(parts[parts.length - 1], 10);
    }

    function oidLastButOne(oid) {
      var parts = String(oid).split('.');
      return parseInt(parts[parts.length - 2], 10);
    }

    function formatMac(macWithSpaces) {
      var cleaned = String(macWithSpaces || '').split(' ').join('');
      var pairs = [];
      for (var i = 0; i < Math.floor(cleaned.length / 2); i++) {
        pairs.push(cleaned.substring(i * 2, i * 2 + 2));
      }
      return pairs.join(':');
    }

    function macFromOid(oid) {
      var parts = String(oid).split('.');
      var bytes = parts.slice(parts.length - 6);
      return bytes.map(function (b) {
        var hex = parseInt(b, 10).toString(16).toUpperCase();
        return hex.length === 1 ? '0' + hex : hex;
      }).join(':');
    }

    function parseJuniperEgress(input) {
      if (!input) return [];
      return String(input).split(/[, ]+/).filter(function (s) { return s.length > 0; })
        .map(function (s) { return parseInt(s, 10); });
    }

    function parseHuaweiEgress(input) {
      if (!input) return [];
      var hexValues = String(input).split(' ').filter(function (s) { return s.length > 0; });
      var activePorts = [];
      var portNumber = 1;
      hexValues.forEach(function (hex) {
        var byteValue = parseInt(hex, 16);
        var binary = byteValue.toString(2);
        while (binary.length < 8) binary = '0' + binary;
        for (var i = 0; i < 8; i++) {
          if (binary[i] === '1') activePorts.push(portNumber - 1);
          portNumber++;
        }
      });
      return activePorts;
    }

    function trimVlanNamePlusDigit(input) {
      if (!input) return input;
      var s = String(input);
      var index = s.lastIndexOf('+');
      if (index !== -1 && index + 1 < s.length && /[0-9]/.test(s[index + 1])) {
        return s.substring(0, index);
      }
      return s;
    }
    """;
}
