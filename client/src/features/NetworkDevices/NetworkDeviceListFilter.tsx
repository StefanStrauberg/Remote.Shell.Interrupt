import {
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Divider,
  FormControl,
  InputLabel,
  ListItemText,
  MenuItem,
  OutlinedInput,
  Radio,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import { NetworkDeviceFilter } from "../../lib/types/NetworkDevices/NetworkDeviceFilter";
import { FilterList } from "@mui/icons-material";
import { useState } from "react";
import { typeOfNetworkDeviceOptions } from "../../lib/types/Gates/typeOfNetworkDeviceOptions";

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
  onApplyFilters: (newFilters: NetworkDeviceFilter) => void;
};

export default function NetworkDeviceListFilter({ onApplyFilters }: Props) {
  const [host, setHost] = useState<string>("");
  const [networkDeviceName, setNetworkDeviceName] = useState<string>("");

  const [typeOfNetworkDevice, setTypeOfNetworkDevice] = useState<string>("");

  const handleApplyClick = () => {
    const filters: NetworkDeviceFilter = {};
    if (host) filters.host = { op: "~=", value: host };
    if (networkDeviceName)
      filters.networkDeviceName = { op: "~=", value: networkDeviceName };
    if (typeOfNetworkDevice)
      filters.typeOfNetworkDevice = { op: "==", value: typeOfNetworkDevice };
    onApplyFilters(filters);
  };

  const handleChange = (event: SelectChangeEvent<string>) => {
    setTypeOfNetworkDevice(event.target.value);
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
            label="Название маршрутизатора"
            value={networkDeviceName}
            onChange={(e) => setNetworkDeviceName(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <TextField
            label="IP адрес"
            value={host}
            onChange={(e) => setHost(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <FormControl fullWidth>
            <InputLabel>Тип маршрутизатора</InputLabel>
            <Select
              value={typeOfNetworkDevice}
              onChange={handleChange}
              input={<OutlinedInput label="Тип маршрутизатора" />}
              renderValue={(selected) => selected}
              MenuProps={MenuProps}
              fullWidth
            >
              {typeOfNetworkDeviceOptions.map((typeOfND) => (
                <MenuItem key={typeOfND.value} value={typeOfND.value}>
                  <Radio
                    checked={typeOfNetworkDevice === typeOfND.value} // Радиобокс
                  />
                  <ListItemText primary={typeOfND.value} />
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <Divider />
          <ButtonGroup>
            <Button
              variant="contained"
              color="info"
              onClick={() => {
                setTypeOfNetworkDevice("");
                setHost("");
                setNetworkDeviceName("");
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
