import { Box, Button, Divider, Typography, Grid2 } from "@mui/material";
import { Link, NavLink } from "react-router";
import { useGates } from "../../lib/hooks/useGates";
import { useClients } from "../../lib/hooks/useClients";

export default function AdminComponent() {
  const pageNumber = 1;
  const pageSize = 100;

  const { gates, deleteGate } = useGates(pageNumber, pageSize, {});
  const { deleteClients, updateClients } = useClients();

  const updateClientsHandle = () => {
    if (window.confirm(`Подумай дважды!!!`)) {
      updateClients.mutate(null!, {
        onSuccess: () => {
          console.log(`Clients were updated successfully`);
        },
        onError: (error) => {
          console.error("Failed to update clients:", error);
        },
      });
    }
  };

  const deleteGatesHandle = () => {
    if (window.confirm(`Подумай дважды!!!`)) {
      const gatesIds = gates?.map((x) => x.id);
      gatesIds?.forEach((gateId) => {
        deleteGate.mutate(gateId!, {
          onSuccess: () => {
            console.log(`Gate with ID ${gateId} deleted successfully`);
          },
          onError: (error) => {
            console.error("Failed to delete gate:", error);
          },
        });
      });
    }
  };

  const deleteClientsHandle = () => {
    if (window.confirm(`Подумай дважды!!!`)) {
      deleteClients.mutate(null!, {
        onSuccess: () => {
          console.log(`Clients were deleted successfully`);
        },
        onError: (error) => {
          console.error("Failed to delete clients:", error);
        },
      });
    }
  };

  return (
    <Box p={3} sx={{ backgroundColor: "#f9f9f9", borderRadius: 2 }}>
      {/* Создание маршрутизатора */}
      <Grid2 container spacing={2} alignItems="center">
        <Grid2>
          <Button
            variant="contained"
            color="info"
            component={Link}
            to={`/createGate`}
            sx={{ boxShadow: 3 }}
          >
            Создать маршрутизатор
          </Button>
        </Grid2>
        <Grid2>
          <Typography variant="body1" color="text.secondary">
            Создание нового маршрутизатора, ПО автоматически обновляет
            информацию о них.
          </Typography>
        </Grid2>
      </Grid2>
      <Divider sx={{ my: 2 }} />

      {/* Удаление маршрутизаторов */}
      <Grid2 container spacing={2} alignItems="center">
        <Grid2>
          <Button
            variant="contained"
            color="error"
            onClick={deleteGatesHandle}
            sx={{ boxShadow: 3 }}
          >
            Удалить все маршрутизаторы
          </Button>
        </Grid2>
        <Grid2>
          <Typography variant="body1" color="text.secondary">
            Внимание! Удаление всех маршрутизаторов (нужно создавать заново).
          </Typography>
        </Grid2>
      </Grid2>
      <Divider sx={{ my: 2 }} />

      {/* Информация о маршрутизаторах */}
      <Typography variant="body2" color="text.primary" sx={{ mb: 2 }}>
        Маршрутизаторы - это виртуальные сущности, создаваемые исключительно для
        опроса шлюзов.
      </Typography>
      <Divider sx={{ my: 2 }} />

      {/* Обновление клиентов */}
      <Grid2 container spacing={2} alignItems="center">
        <Grid2>
          <Button
            variant="contained"
            color="warning"
            onClick={updateClientsHandle}
            sx={{ boxShadow: 3 }}
          >
            Обновить клиентов
          </Button>
        </Grid2>
        <Grid2>
          <Typography variant="body1" color="text.secondary">
            Обновить информацию о клиентах, тарифных планах, VLAN'ах и пулах
            адресов.
          </Typography>
        </Grid2>
      </Grid2>
      <Divider sx={{ my: 2 }} />

      {/* Удаление клиентов */}
      <Grid2 container spacing={2} alignItems="center">
        <Grid2>
          <Button
            variant="contained"
            color="error"
            onClick={deleteClientsHandle}
            sx={{ boxShadow: 3 }}
          >
            Удалить всех клиентов
          </Button>
        </Grid2>
        <Grid2>
          <Typography variant="body1" color="text.secondary">
            Внимание! Удаление всех клиентов (нужно опрашивать заново).
          </Typography>
        </Grid2>
      </Grid2>
      <Divider sx={{ my: 2 }} />

      {/* Информация о клиентах */}
      <Typography variant="body2" color="text.primary" sx={{ mb: 2 }}>
        Клиенты - это сущности, информация которых собирается из биллинга.
      </Typography>
      <Divider sx={{ my: 2 }} />

      {/* Обновление шлюзов */}
      <Grid2 container spacing={2} alignItems="center">
        <Grid2>
          <Button
            component={NavLink}
            to="/networkDevices"
            variant="contained"
            color="info"
            sx={{ boxShadow: 3 }}
          >
            Шлюзы
          </Button>
        </Grid2>
        <Grid2>
          <Button variant="contained" color="warning" sx={{ boxShadow: 3 }}>
            Обновить все шлюзы
          </Button>
        </Grid2>
        <Grid2>
          <Typography variant="body1" color="text.secondary">
            Внимание! Удаление всех шлюзов (нужно опрашивать заново).
          </Typography>
        </Grid2>
      </Grid2>
      <Divider sx={{ my: 2 }} />

      {/* Удаление шлюзов */}
      <Grid2 container spacing={2} alignItems="center">
        <Grid2>
          <Button variant="contained" color="error" sx={{ boxShadow: 3 }}>
            Удалить всех шлюзов
          </Button>
        </Grid2>
        <Grid2>
          <Typography variant="body1" color="text.secondary">
            Внимание! Удаление всех шлюзов (нужно опрашивать заново).
          </Typography>
        </Grid2>
      </Grid2>
      <Divider sx={{ my: 2 }} />

      {/* Информация о шлюзах */}
      <Typography variant="body2" color="text.primary" sx={{ mb: 2 }}>
        Шлюзы - это сущности, информация которых собирается из маршрутизаторов
        ЦОДа.
      </Typography>
    </Box>
  );
}
