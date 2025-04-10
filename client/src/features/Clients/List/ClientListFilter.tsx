import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Checkbox,
  Divider,
  FormControlLabel,
  Grid2,
  TextField,
} from "@mui/material";
import { useState } from "react";
import { ClientFilter } from "../../../lib/types/Clients/ClientFilter";

type Props = {
  onApplyFilters: (filters: ClientFilter) => void;
};

export default function ClientListFilter({ onApplyFilters }: Props) {
  const [name, setName] = useState<string>("");
  const [nrDogovor, setNrDogovor] = useState<string>("");
  const [working, setWorking] = useState<boolean>(true);
  const [antiDDOS, setAntiDDOS] = useState<boolean>(false);

  const handleApplyClick = () => {
    const filters: ClientFilter = {};
    if (name) filters.Name = { op: "~=", value: name };
    if (nrDogovor) filters.NrDogovor = { op: "~=", value: nrDogovor };
    if (working) filters.Working = { op: "==", value: working };
    if (antiDDOS) filters.AntiDDOS = { op: "==", value: antiDDOS };
    onApplyFilters(filters);
  };

  return (
    <Card
      sx={{
        borderRadius: 4,
        boxShadow: 3,
        bgcolor: "background.default",
        fontSize: 18,
        overflow: "hidden",
      }}
    >
      <CardHeader
        title="Фильтры"
        avatar={<FilterList color="primary" />}
        sx={{ bgcolor: "primary.light", color: "white", p: 2 }}
      />
      <CardContent>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            label="Название организации"
            value={name}
            onChange={(e) => setName(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <TextField
            label="Номер договора"
            value={nrDogovor}
            onChange={(e) => setNrDogovor(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <Divider />
          <Grid2 container spacing={1}>
            <Grid2 size={6}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={working}
                    onChange={(e) => setWorking(e.target.checked)}
                  />
                }
                label="Working"
              />
            </Grid2>
            <Grid2 size={6}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={antiDDOS}
                    onChange={(e) => setAntiDDOS(e.target.checked)}
                  />
                }
                label="AntiDDOS"
              />
            </Grid2>
          </Grid2>
          <Divider />
          <ButtonGroup>
            <Button
              variant="contained"
              color="info"
              onClick={() => {
                setName("");
                setNrDogovor("");
                setWorking(true);
                setAntiDDOS(false);
              }}
            >
              Сбросить
            </Button>
            <Button
              variant="contained"
              color="success"
              onClick={handleApplyClick}
            >
              Применить
            </Button>
          </ButtonGroup>
        </Box>
      </CardContent>
    </Card>
  );
}
