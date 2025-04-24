import { Box, Divider, IconButton, Typography } from "@mui/material";
import { Port } from "../../../lib/types/NetworkDevices/Port";
import {
  CheckBox,
  Fingerprint,
  Home,
  Info,
  Speed,
  ExpandMore,
  ExpandLess,
} from "@mui/icons-material";
import RouterSubPort from "./RouterSubPort";
import { useState } from "react";

type Props = {
  port: Port;
};

export default function RouterPort({ port }: Props) {
  const [isExpanded, setIsExpanded] = useState(false); // Состояние для контроля раскрытия

  const toggleExpand = () => setIsExpanded(!isExpanded); // Переключение состояния

  const getStatusColor = (status: string) =>
    status === "up" ? "black" : "gray";
  const getIconColor = (status: string, color: string) =>
    status === "up" ? color : "gray";

  return (
    <>
      <Box
        display="flex"
        alignItems="center"
        justifyContent="space-between"
        my={2}
        px={2}
      >
        <Box display="flex" alignItems="center">
          <Fingerprint
            sx={{ mr: 1, color: getIconColor(port.interfaceStatus, "black") }}
          />
          <Typography sx={{ color: getStatusColor(port.interfaceStatus) }}>
            Имя: {port.interfaceName}
          </Typography>

          <CheckBox
            sx={{ mx: 2, color: getIconColor(port.interfaceStatus, "green") }}
          />
          <Typography sx={{ color: getStatusColor(port.interfaceStatus) }}>
            Статус: {port.interfaceStatus}
          </Typography>

          <Speed
            sx={{ mx: 2, color: getIconColor(port.interfaceStatus, "red") }}
          />
          <Typography sx={{ color: getStatusColor(port.interfaceStatus) }}>
            Скорость: {port.interfaceSpeed / 1_000_000_000} Гб
          </Typography>

          <Home
            sx={{ mx: 2, color: getIconColor(port.interfaceStatus, "black") }}
          />
          <Typography sx={{ color: getStatusColor(port.interfaceStatus) }}>
            MAC: {port.macAddress}
          </Typography>
        </Box>

        {port.aggregatedPorts.length > 0 && (
          <IconButton onClick={toggleExpand}>
            {isExpanded ? <ExpandLess /> : <ExpandMore />}
          </IconButton>
        )}
      </Box>

      <Box display="flex" alignItems="center" my={2} px={2}>
        <Info
          sx={{ mr: 1, color: getIconColor(port.interfaceStatus, "#1976d2") }}
        />
        <Typography sx={{ mr: 1, color: getStatusColor(port.interfaceStatus) }}>
          Вланы:
        </Typography>
        <Typography sx={{ color: getStatusColor(port.interfaceStatus) }}>
          {port.vlaNs.length > 0
            ? port.vlaNs
                .map((vlan) => `${vlan.vlanTag} (${vlan.vlanName})`)
                .join(", ")
            : "Нет вланов"}
        </Typography>
      </Box>

      {isExpanded && port.aggregatedPorts.length > 0 && (
        <Box sx={{ pl: 4 }}>
          {port.aggregatedPorts.map((aggregatedPort) => (
            <RouterSubPort key={aggregatedPort.id} port={aggregatedPort} />
          ))}
        </Box>
      )}

      <Divider />
    </>
  );
}
