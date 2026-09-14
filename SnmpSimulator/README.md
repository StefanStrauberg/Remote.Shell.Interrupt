# SNMP Dump Simulator

Small SNMP v2c UDP simulator for replaying values from a numeric `snmpwalk` dump.

## Requirements

- .NET 9 SDK
- Net-SNMP tools are optional, but useful for creating dumps and testing.

## Create a dump

Use numeric OIDs so the simulator does not need MIB files to resolve symbolic names:

```bash
snmpwalk -v2c -c public -On 192.0.2.10 > dump.txt
```

Both of these numeric forms are accepted:

```text
.1.3.6.1.2.1.1.1.0 = STRING: "Example device"
1.3.6.1.2.1.1.3.0 = Timeticks: (12345) 0:02:03.45
```

`iso.3.6...` and `ccitt...` are also accepted. Symbolic OIDs such as
`SNMPv2-MIB::sysDescr.0` are not resolved; create the dump with `-On`.

## Run

```bash
dotnet run -- \
  --file dump.txt \
  --address 127.0.0.1 \
  --port 1161 \
  --community public
```

Defaults:

- address: `127.0.0.1`
- port: `1161`
- community: `public`

## Test

```bash
snmpget -v2c -c public 127.0.0.1:1161 1.3.6.1.2.1.1.1.0
snmpwalk -v2c -c public 127.0.0.1:1161 1.3.6.1.2.1
snmpbulkwalk -v2c -c public 127.0.0.1:1161 1.3.6.1.2.1
```

## Supported operations

- SNMP v2c GET
- SNMP v2c GETNEXT
- SNMP v2c GETBULK

GETBULK repetitions are capped at 100 per request to avoid excessive memory/CPU
use from untrusted UDP packets. Responses larger than 60,000 bytes are replaced
with a standard SNMP `tooBig` response.

## Supported dump value types

- STRING
- INTEGER / INTEGER32
- Gauge32 / Unsigned32
- Counter32
- Counter64
- Timeticks
- IpAddress (IPv4 only, as required by SNMP IpAddress syntax)
- OID
- Hex-STRING
- Opaque (hex bytes)
- NULL

Unknown value types are preserved as OCTET STRING and produce a warning.

## Notes

This is a dump-driven simulator, not a full MIB engine. It cannot reliably
distinguish `noSuchObject` from `noSuchInstance` without MIB schema information,
so a missing exact GET is represented as `noSuchObject`. GETNEXT/GETBULK use
`endOfMibView` when no lexicographic successor exists.
