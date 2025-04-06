import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Checkbox,
  Divider,
  FormControl,
  FormControlLabel,
  Grid2,
  InputLabel,
  ListItemText,
  MenuItem,
  OutlinedInput,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import { useState } from "react";
import { ClientFilter } from "../../lib/types/ClientFilter";

const ITEM_HEIGHT = 48;
const ITEM_PADDING_TOP = 8;
const MenuProps = {
  PaperProps: {
    style: {
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
      width: 350,
    },
  },
};

type Props = {
  onApplyFilters: (filters: ClientFilter) => void;
};

export default function ClientListFilter({ onApplyFilters }: Props) {
  const [name, setName] = useState<string>("");
  const [nrDogovor, setNrDogovor] = useState<string>("");
  const [working, setWorking] = useState<boolean>(true);
  const [antiDDOS, setAntiDDOS] = useState<boolean>(false);

  const [personName, setPersonName] = useState<string[]>([]);

  const handleApplyClick = () => {
    const filters: ClientFilter = {};
    if (name) filters.Name = { op: "~=", value: name };
    if (nrDogovor) filters.NrDogovor = { op: "~=", value: nrDogovor };
    if (working) filters.Working = { op: "==", value: working };
    if (antiDDOS) filters.AntiDDOS = { op: "==", value: antiDDOS };
    onApplyFilters(filters);
  };

  const handleChange = (event: SelectChangeEvent<typeof personName>) => {
    const {
      target: { value },
    } = event;
    setPersonName(
      // On autofill we get a stringified value.
      typeof value === "string" ? value.split(",") : value
    );
  };

  const names = [
    "Oliver Hansen",
    "Van Henry",
    "April Tucker",
    "Ralph Hubbard",
    "Omar Alexander",
    "Carlos Abbott",
    "Miriam Wagner",
    "Bradley Wilkerson",
    "Virginia Andrews",
    "Kelly Snyder",
  ];

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
          <FormControl fullWidth>
            <InputLabel id="demo-multiple-name-label">Name</InputLabel>
            <Select
              labelId="demo-multiple-name-label"
              id="demo-multiple-name"
              multiple
              value={personName}
              onChange={handleChange}
              input={<OutlinedInput label="Name" />}
              renderValue={(selected) => selected.join(", ")}
              MenuProps={MenuProps}
              fullWidth
            >
              {names.map((name) => (
                <MenuItem key={name} value={name}>
                  <Checkbox checked={personName.includes(name)} />
                  <ListItemText primary={name} />
                </MenuItem>
              ))}
            </Select>
          </FormControl>
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
          <Button
            variant="contained"
            color="success"
            onClick={handleApplyClick}
          >
            Применить
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
