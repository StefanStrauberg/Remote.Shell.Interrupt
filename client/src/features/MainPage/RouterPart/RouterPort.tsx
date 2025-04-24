import { Box, Divider, Typography } from "@mui/material";
import { Port } from "../../../lib/types/NetworkDevices/Port";
import { CheckBox, Fingerprint, Home, Info, Speed } from "@mui/icons-material";

type Props = {
  port: Port;
};

export default function RouterPort({ port }: Props) {
  // Определяем стили для "up" и "down" статусов
  const getStatusColor = (status: string) =>
    status === "up" ? "black" : "gray";
  const getIconColor = (status: string, color: string) =>
    status === "up" ? color : "gray";

  return (
    <>
      <Box display="flex" alignItems="center" my={2} px={2}>
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
          Скорость: {port.interfaceSpeed}
        </Typography>

        <Home
          sx={{ mx: 2, color: getIconColor(port.interfaceStatus, "black") }}
        />
        <Typography sx={{ color: getStatusColor(port.interfaceStatus) }}>
          MAC: {port.macAddress}
        </Typography>
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

      <Divider />
    </>
  );
}
