import { Box, Paper, Typography } from "@mui/material";
import { Port } from "../../../lib/types/NetworkDevices/Port";
import { CheckBox, Fingerprint, Home, Speed } from "@mui/icons-material";

type Props = {
  port: Port;
};

export default function RouterSubPort({ port }: Props) {
  // Определяем стили для "up" и "down" статусов
  const getStatusColor = (status: string) =>
    status === "up" ? "black" : "gray";
  const getIconColor = (status: string, color: string) =>
    status === "up" ? color : "gray";

  return (
    <>
      <Paper elevation={3} sx={{ my: 1, p: 2, border: "1px solid #ddd" }}>
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
      </Paper>
    </>
  );
}
