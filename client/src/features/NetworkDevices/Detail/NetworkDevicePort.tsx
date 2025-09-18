import {
  Box,
  Divider,
  IconButton,
  Typography,
  Collapse,
  Chip,
} from "@mui/material";
import { Port } from "../../../lib/types/NetworkDevices/Port";
import {
  CheckBox,
  Fingerprint,
  Home,
  Info,
  Speed,
  ExpandMore,
  ExpandLess,
  Lan,
} from "@mui/icons-material";
import { useState } from "react";
import NetworkDeviceSubPort from "./NetworkDeviceSubPort";
import DeveloperBoardIcon from "@mui/icons-material/DeveloperBoard";
import NetworkDeviceMACTable from "./NetworkDeviceMACTable";

type Props = {
  port: Port;
};

export default function NetworkDevicePort({ port }: Props) {
  const [isExpanded, setIsExpanded] = useState(false);
  const [showMacTable, setShowMacTable] = useState(false);

  const toggleExpand = () => setIsExpanded(!isExpanded);
  const toggleMacTable = () => setShowMacTable(!showMacTable);

  const getStatusIconColor = (status: string) =>
    status === "up" ? "success.main" : "disabled";

  const getSpeedText = (speed: number) => {
    if (speed >= 1_000_000_000) return `${speed / 1_000_000_000} Gbps`;
    if (speed >= 1_000_000) return `${speed / 1_000_000} Mbps`;
    return `${speed / 1_000} Kbps`;
  };

  return (
    <Box sx={{ p: 2 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Box display="flex" alignItems="center" flexWrap="wrap" gap={2}>
          {/* Interface Name */}
          <Box display="flex" alignItems="center">
            <Fingerprint
              sx={{ mr: 1, color: getStatusIconColor(port.interfaceStatus) }}
            />
            <Typography variant="body1" fontWeight="medium">
              {port.interfaceName}
            </Typography>
          </Box>

          {/* Status */}
          <Box display="flex" alignItems="center">
            <CheckBox
              sx={{ color: getStatusIconColor(port.interfaceStatus), mr: 0.5 }}
            />
            <Chip
              label={port.interfaceStatus.toUpperCase()}
              size="small"
              color={port.interfaceStatus === "up" ? "success" : "error"}
              variant="outlined"
            />
          </Box>

          {/* Speed */}
          <Box display="flex" alignItems="center">
            <Speed
              sx={{ color: getStatusIconColor(port.interfaceStatus), mr: 0.5 }}
            />
            <Typography variant="body2">
              {getSpeedText(port.interfaceSpeed)}
            </Typography>
          </Box>

          {/* MAC Address */}
          <Box display="flex" alignItems="center">
            <Home
              sx={{ color: getStatusIconColor(port.interfaceStatus), mr: 0.5 }}
            />
            <Typography variant="body2" fontFamily="monospace">
              {port.macAddress}
            </Typography>
          </Box>
        </Box>
      </Box>

      {/* VLANs */}
      <Box display="flex" alignItems="center" mt={2} flexWrap="wrap" gap={1}>
        <Info sx={{ mr: 1, color: "primary.main" }} />
        <Typography variant="body2" fontWeight="medium" sx={{ mr: 1 }}>
          VLANs:
        </Typography>
        {port.vlaNs.length > 0 ? (
          port.vlaNs.map((vlan) => (
            <Chip
              key={vlan.vlanTag}
              icon={<Lan />}
              label={`${vlan.vlanTag} (${vlan.vlanName})`}
              size="small"
              variant="outlined"
              color="primary"
            />
          ))
        ) : (
          <Typography variant="body2" color="text.secondary">
            No VLANs
          </Typography>
        )}
      </Box>

      {/* MAC Table */}
      <Box display="flex" alignItems="flex-start" mt={2}>
        <DeveloperBoardIcon sx={{ mr: 1, color: "secondary.light", mt: 0.5 }} />
        <Box flexGrow={1}>
          <Box display="flex" alignItems="center">
            <Typography variant="body2" fontWeight="medium" sx={{ mr: 1 }}>
              MAC Table: {port.macTable.length} entries
            </Typography>
            {port.macTable.length > 0 && (
              <IconButton onClick={toggleMacTable} size="small">
                {showMacTable ? <ExpandLess /> : <ExpandMore />}
              </IconButton>
            )}
          </Box>

          <Collapse in={showMacTable && port.macTable.length > 0}>
            <Box sx={{ pl: 0, mt: 1 }}>
              <NetworkDeviceMACTable macTable={port.macTable} />
            </Box>
          </Collapse>
        </Box>
      </Box>

      {/* Aggregated Ports */}
      <Box display="flex" alignItems="flex-start" mt={2}>
        <Lan sx={{ mr: 1, color: "secondary.dark", mt: 0.5 }} />
        <Box flexGrow={1}>
          <Box display="flex" alignItems="center">
            <Typography variant="body2" fontWeight="medium" sx={{ mr: 1 }}>
              Aggregated Ports: {port.aggregatedPorts.length} entries
            </Typography>
            {port.aggregatedPorts.length > 0 && (
              <IconButton onClick={toggleExpand} size="small">
                {isExpanded ? <ExpandLess /> : <ExpandMore />}
              </IconButton>
            )}
          </Box>

          <Collapse in={isExpanded}>
            <Box sx={{ pl: 0, mt: 1 }}>
              <Typography variant="subtitle2" fontWeight="bold" gutterBottom>
                Aggregated Ports:
              </Typography>
              {port.aggregatedPorts.map((aggregatedPort) => (
                <NetworkDeviceSubPort
                  key={aggregatedPort.id}
                  port={aggregatedPort}
                />
              ))}
            </Box>
          </Collapse>
        </Box>
      </Box>

      <Divider sx={{ mt: 2 }} />
    </Box>
  );
}
