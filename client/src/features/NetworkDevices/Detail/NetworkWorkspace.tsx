import { Fragment, useId, useState } from "react";
import { Link } from "react-router";
import {
  Box,
  Button,
  Chip,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tabs,
  TextField,
  Typography,
} from "@mui/material";
import { Close, OpenInNew, ViewSidebarOutlined } from "@mui/icons-material";
import { NetworkDevice } from "@/lib/types/NetworkDevices/NetworkDevice";
import { Port } from "@/lib/types/NetworkDevices/Port";
import { routes } from "@/app/router/paths";
import CopyValue from "@/app/shared/components/CopyValue";
import NetworkDevicePort from "./NetworkDevicePort";

const mono = {
  fontFamily: '"Consolas", monospace',
  fontVariantNumeric: "tabular-nums",
};
const speed = (value: number) =>
  value >= 1e9
    ? `${value / 1e9} Gbps`
    : value >= 1e6
      ? `${value / 1e6} Mbps`
      : `${value / 1e3} Kbps`;
const statusColor = (value: string) =>
  value.toLowerCase() === "up"
    ? "success"
    : value.toLowerCase() === "down"
      ? "error"
      : "default";

function Properties({ values }: { values: [string, string][] }) {
  return (
    <Box component="dl" sx={{ m: 0 }}>
      {values.map(([label, value]) => (
        <Box
          key={label}
          sx={{
            display: "grid",
            gridTemplateColumns: "minmax(80px, 1fr) minmax(0, 1.5fr)",
            gap: 2,
            py: 1.2,
            borderBottom: 1,
            borderColor: "divider",
          }}
        >
          <Typography component="dt" variant="body2" color="text.secondary">
            {label}
          </Typography>
          <Typography
            component="dd"
            variant="body2"
            sx={{ m: 0, overflowWrap: "anywhere", textAlign: "right" }}
          >
            {value || "Not available"}
          </Typography>
        </Box>
      ))}
    </Box>
  );
}

function PortInspector({
  device,
  port,
  close,
}: {
  device: NetworkDevice;
  port: Port;
  close: () => void;
}) {
  const [tab, setTab] = useState(0);
  const id = useId();
  const labels = [
    "Properties",
    `MAC (${port.macTable.length})`,
    `Members (${port.aggregatedPorts.length})`,
    "IP data",
  ];
  return (
    <Paper
      component="aside"
      aria-label="Interface details"
      variant="outlined"
      sx={{
        minWidth: 0,
        overflow: "hidden",
        position: { lg: "sticky" },
        top: 16,
      }}
    >
      <Stack
        direction="row"
        alignItems="center"
        sx={{ px: 2, py: 1, borderBottom: 1, borderColor: "divider" }}
      >
        <Typography variant="overline" color="text.secondary">
          Interface details
        </Typography>
        <IconButton
          aria-label="Close interface details"
          onClick={close}
          size="small"
          sx={{ ml: "auto" }}
        >
          <Close fontSize="small" />
        </IconButton>
      </Stack>
      <Box sx={{ p: 2 }}>
        <Stack direction="row" alignItems="center" gap={1}>
          <Typography variant="h6" sx={mono}>
            {port.interfaceName}
          </Typography>
          <Chip
            size="small"
            label={port.interfaceStatus?.toUpperCase() || "UNKNOWN"}
            color={statusColor(port.interfaceStatus)}
          />
        </Stack>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          {device.networkDeviceName}
        </Typography>
        <Stack direction="row" alignItems="center">
          <Typography variant="caption" sx={mono}>
            {device.host}
          </Typography>
          <CopyValue value={device.host} label="device IP" />
        </Stack>
      </Box>
      <Tabs
        value={tab}
        onChange={(_, value: number) => setTab(value)}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Interface detail sections"
        sx={{
          borderBottom: 1,
          borderColor: "divider",
          minHeight: 42,
          "& .MuiTab-root": {
            minWidth: 0,
            minHeight: 42,
            px: 1.5,
            fontSize: 12,
          },
        }}
      >
        {labels.map((label, index) => (
          <Tab
            key={label}
            label={label}
            id={`${id}-tab-${index}`}
            aria-controls={`${id}-panel-${index}`}
          />
        ))}
      </Tabs>
      <Box
        role="tabpanel"
        id={`${id}-panel-${tab}`}
        aria-labelledby={`${id}-tab-${tab}`}
        sx={{ p: 2 }}
      >
        {tab === 0 && (
          <>
            <Properties
              values={[
                ["Interface number", String(port.interfaceNumber)],
                ["Type", port.interfaceType],
                ["Speed", speed(port.interfaceSpeed)],
                ["Description", port.description],
                ["MAC address", port.macAddress],
                ["Aggregation", port.isAggregated ? "Yes" : "No"],
                ["Parent ID", port.parentId ?? ""],
                ["Device information", device.generalInformation],
              ]}
            />
            <Stack direction="row" gap={0.5} flexWrap="wrap" sx={{ mt: 2 }}>
              {port.vlans.length ? (
                port.vlans.map((v) => (
                  <Chip
                    key={v.vlanTag}
                    size="small"
                    label={`${v.vlanTag} (${v.vlanName})`}
                    component={Link}
                    clickable
                    to={`${routes.main}?vlan=${v.vlanTag}`}
                  />
                ))
              ) : (
                <Typography variant="body2" color="text.secondary">
                  No VLANs
                </Typography>
              )}
            </Stack>
          </>
        )}
        {tab === 1 &&
          (port.macTable.length ? (
            port.macTable.map((mac, index) => (
              <Stack
                key={`${mac}-${index}`}
                direction="row"
                alignItems="center"
                justifyContent="space-between"
              >
                <Typography variant="body2" sx={mono}>
                  {mac}
                </Typography>
                <CopyValue value={mac} />
              </Stack>
            ))
          ) : (
            <Typography variant="body2" color="text.secondary">
              No learned MAC addresses in this snapshot.
            </Typography>
          ))}
        {tab === 2 &&
          (port.aggregatedPorts.length ? (
            port.aggregatedPorts.map((member) => (
              <NetworkDevicePort key={member.id} port={member} />
            ))
          ) : (
            <Typography variant="body2" color="text.secondary">
              No aggregated members.
            </Typography>
          ))}
        {tab === 3 && (
          <>
            <Typography variant="subtitle2">ARP addresses</Typography>
            <Properties
              values={Object.entries(port.arpTableOfPort ?? {}).map(
                ([mac, ips]) => [mac, ips.join(", ")]
              )}
            />
            {!Object.keys(port.arpTableOfPort ?? {}).length && (
              <Typography variant="body2" color="text.secondary" sx={{ my: 1 }}>
                No ARP data available.
              </Typography>
            )}
            <Typography variant="subtitle2" sx={{ mt: 2 }}>
              Terminated networks
            </Typography>
            <Properties
              values={Object.entries(port.networkTableOfPort ?? {})}
            />
            {!Object.keys(port.networkTableOfPort ?? {}).length && (
              <Typography variant="body2" color="text.secondary" sx={{ my: 1 }}>
                No network data available.
              </Typography>
            )}
          </>
        )}
        <Button
          component={Link}
          to={routes.networkDevice(device.id)}
          startIcon={<OpenInNew />}
          variant="outlined"
          fullWidth
          sx={{ mt: 2 }}
        >
          Open device
        </Button>
      </Box>
    </Paper>
  );
}

export default function NetworkWorkspace({
  devices,
}: {
  devices: NetworkDevice[];
}) {
  const [selection, setSelection] = useState<{
    deviceId: string;
    portId: string;
  } | null>(null);
  const [closed, setClosed] = useState(false);
  const [query, setQuery] = useState("");
  const [status, setStatus] = useState("all");
  const [compact, setCompact] = useState(true);
  const allPorts = devices.flatMap((device) =>
    device.portsOfNetworkDevice.map((port) => ({ device, port }))
  );
  const normalized = query.trim().toLowerCase();
  const visible = allPorts.filter(
    ({ device, port }) =>
      (status === "all" || port.interfaceStatus.toLowerCase() === status) &&
      [
        device.networkDeviceName,
        device.host,
        port.interfaceName,
        port.description,
        port.macAddress,
        ...port.macTable,
        ...port.vlans.map((v) => `${v.vlanTag} ${v.vlanName}`),
      ]
        .join(" ")
        .toLowerCase()
        .includes(normalized)
  );
  const selected =
    visible.find(
      ({ device, port }) =>
        device.id === selection?.deviceId && port.id === selection?.portId
    ) ?? visible[0];
  const inspecting = !closed && !!selected;
  return (
    <Box>
      <Stack
        direction="row"
        gap={1}
        alignItems="center"
        flexWrap="wrap"
        sx={{ mb: 2 }}
      >
        <TextField
          size="small"
          label="Filter loaded interfaces"
          placeholder="Name, IP, VLAN or MAC"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          sx={{ minWidth: 200, flex: 1 }}
        />
        <TextField
          select
          size="small"
          label="Status"
          value={status}
          onChange={(e) => setStatus(e.target.value)}
          sx={{ width: 120 }}
        >
          <MenuItem value="all">All</MenuItem>
          <MenuItem value="up">Up</MenuItem>
          <MenuItem value="down">Down</MenuItem>
        </TextField>
        <Button
          variant="outlined"
          aria-pressed={compact}
          onClick={() => setCompact(!compact)}
        >
          Compact rows
        </Button>
        {closed && selected && (
          <Button
            startIcon={<ViewSidebarOutlined />}
            onClick={() => setClosed(false)}
          >
            Show details
          </Button>
        )}
      </Stack>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: {
            xs: "minmax(0, 1fr)",
            lg: inspecting ? "minmax(0, 1fr) 340px" : "minmax(0, 1fr)",
          },
          gap: 2,
          alignItems: "start",
        }}
      >
        <Paper variant="outlined" sx={{ minWidth: 0, overflow: "hidden" }}>
          <Stack
            direction="row"
            justifyContent="space-between"
            flexWrap="wrap"
            gap={1}
            sx={{ px: 2, py: 1.5 }}
          >
            <Typography variant="subtitle2">
              Interfaces · {visible.length} / {allPorts.length}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              Recorded SNMP data
            </Typography>
          </Stack>
          <TableContainer sx={{ border: 0, borderRadius: 0 }}>
            <Table
              size={compact ? "small" : "medium"}
              aria-label="Network interfaces"
            >
              <TableHead>
                <TableRow>
                  <TableCell>Interface</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Speed</TableCell>
                  <TableCell>VLANs</TableCell>
                  <TableCell align="right">MACs</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {devices.map((device) => {
                  const ports = visible
                    .filter((item) => item.device.id === device.id)
                    .map((item) => item.port);
                  if (
                    !ports.length &&
                    (device.portsOfNetworkDevice.length ||
                      normalized ||
                      status !== "all")
                  )
                    return null;
                  return (
                    <Fragment key={device.id}>
                      <TableRow>
                        <TableCell
                          colSpan={5}
                          sx={{ bgcolor: "background.default", py: 1 }}
                        >
                          <Stack
                            direction="row"
                            gap={1.5}
                            alignItems="center"
                            flexWrap="wrap"
                          >
                            <Button
                              component={Link}
                              to={routes.networkDevice(device.id)}
                              size="small"
                              sx={{ px: 0 }}
                            >
                              {device.networkDeviceName}
                            </Button>
                            <Typography variant="caption" sx={mono}>
                              {device.host}
                            </Typography>
                            <Typography
                              variant="caption"
                              color="text.secondary"
                            >
                              {device.typeOfNetworkDevice}
                            </Typography>
                          </Stack>
                          {device.generalInformation && (
                            <Typography
                              variant="caption"
                              color="text.secondary"
                            >
                              {device.generalInformation}
                            </Typography>
                          )}
                        </TableCell>
                      </TableRow>
                      {ports.map((port) => (
                        <TableRow
                          key={port.id}
                          hover
                          selected={
                            inspecting &&
                            selected?.device.id === device.id &&
                            selected.port.id === port.id
                          }
                        >
                          <TableCell>
                            <Button
                              size="small"
                              sx={{
                                ...mono,
                                px: 0,
                                justifyContent: "flex-start",
                              }}
                              aria-label={`Inspect ${port.interfaceName} on ${device.networkDeviceName}`}
                              onClick={() => {
                                setSelection({
                                  deviceId: device.id,
                                  portId: port.id,
                                });
                                setClosed(false);
                              }}
                            >
                              {port.interfaceName}
                            </Button>
                            {port.description && (
                              <Typography
                                variant="caption"
                                color="text.secondary"
                                display="block"
                                sx={{ maxWidth: 230 }}
                              >
                                {port.description}
                              </Typography>
                            )}
                          </TableCell>
                          <TableCell>
                            <Chip
                              size="small"
                              variant="outlined"
                              color={statusColor(port.interfaceStatus)}
                              label={
                                port.interfaceStatus?.toUpperCase() || "UNKNOWN"
                              }
                            />
                          </TableCell>
                          <TableCell sx={{ whiteSpace: "nowrap" }}>
                            {speed(port.interfaceSpeed)}
                          </TableCell>
                          <TableCell sx={{ ...mono, minWidth: 100 }}>
                            {port.vlans.map((v) => v.vlanTag).join(", ") || "—"}
                          </TableCell>
                          <TableCell align="right" sx={mono}>
                            {port.macTable.length}
                          </TableCell>
                        </TableRow>
                      ))}
                      {!ports.length && (
                        <TableRow>
                          <TableCell colSpan={5}>No ports available</TableCell>
                        </TableRow>
                      )}
                    </Fragment>
                  );
                })}
                {!visible.length && allPorts.length > 0 && (
                  <TableRow>
                    <TableCell colSpan={5}>
                      <Typography sx={{ py: 3 }} color="text.secondary">
                        No interfaces match these filters.
                      </Typography>
                      <Button
                        onClick={() => {
                          setQuery("");
                          setStatus("all");
                        }}
                      >
                        Clear interface filters
                      </Button>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
        {inspecting && (
          <PortInspector
            key={`${selected.device.id}/${selected.port.id}`}
            device={selected.device}
            port={selected.port}
            close={() => setClosed(true)}
          />
        )}
      </Box>
    </Box>
  );
}
