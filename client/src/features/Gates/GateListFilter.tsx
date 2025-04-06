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
  InputLabel,
  ListItemText,
  MenuItem,
  OutlinedInput,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import { useState } from "react";
import { GateFilter } from "../../lib/types/GateFilter";

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
  onApplyFilters: (filters: GateFilter) => void;
};

export default function GateListFilter({ onApplyFilters }: Props) {
  const [name, setName] = useState<string>("");
  const [ipAddress, setIpAddress] = useState<string>("");

  const [personName, setPersonName] = useState<string[]>([]);

  const handleApplyClick = () => {
    const filters: GateFilter = {};
    if (name) filters.Name = { op: "~=", value: name };
    if (ipAddress) filters.IpAddress = { op: "~=", value: ipAddress };
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
            label="Название маршрутизатора"
            value={name}
            onChange={(e) => setName(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <TextField
            label="IP адрес"
            value={ipAddress}
            onChange={(e) => setIpAddress(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <FormControl fullWidth>
            <InputLabel id="gateType">Тип маршрутизатора</InputLabel>
            <Select
              labelId="gateType"
              id="gateType-name"
              multiple
              value={personName}
              onChange={handleChange}
              input={<OutlinedInput label="Тип маршрутизатора" />}
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
