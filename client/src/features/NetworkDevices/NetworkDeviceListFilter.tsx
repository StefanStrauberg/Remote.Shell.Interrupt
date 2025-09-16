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
import { FilterList } from "@mui/icons-material";
import { useState } from "react";
import { typeOfNetworkDeviceOptions } from "../../lib/types/Gates/typeOfNetworkDeviceOptions";
import { FilterDescriptor } from "../../lib/types/Common/FilterDescriptor";
import { FilterOperator } from "../../lib/types/Common/FilterOperator";
import { DEFAULT_FILTERS_NetworkDevices } from "../../lib/api/networkDevices/DEFAULT_FILTERS_NetworkDevices";

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

type NetworkDeviceListFilterProps = {
  onApplyFilters: (filters: FilterDescriptor[]) => void;
  initialFilters?: FilterDescriptor[];
  onResetFilters?: () => void;
};

export default function NetworkDeviceListFilter({
  onApplyFilters,
  initialFilters = [],
  onResetFilters,
}: NetworkDeviceListFilterProps) {
  const getInitialString = (property: string, defaultValue: string): string => {
    const filter = initialFilters.find((f) => f.PropertyPath === property);
    return filter ? filter.Value : defaultValue;
  };

  const [NetworkDeviceName, setNetworkDeviceName] = useState<string>(
    getInitialString("NetworkDeviceName", "")
  );
  const [Host, setHost] = useState<string>(getInitialString("Host", ""));
  const [typeOfNetworkDevice, setTypeOfNetworkDevice] = useState<string>(
    getInitialString("TypeOfNetworkDevice", "")
  );

  const createFilter = (
    property: string,
    operator: FilterOperator,
    value: string | boolean
  ): FilterDescriptor => ({
    PropertyPath: property,
    Operator: operator,
    Value: String(value),
  });

  const handleApply = () => {
    const filters: FilterDescriptor[] = [
      ...(NetworkDeviceName !== ""
        ? [
            createFilter(
              "NetworkDeviceName",
              FilterOperator.Contains,
              NetworkDeviceName
            ),
          ]
        : []),
      ...(Host !== ""
        ? [createFilter("Host", FilterOperator.Equals, Host)]
        : []),
      ...(typeOfNetworkDevice !== ""
        ? [
            createFilter(
              "TypeOfNetworkDevice",
              FilterOperator.Equals,
              typeOfNetworkDevice
            ),
          ]
        : []),
    ];
    onApplyFilters(filters);
  };

  const handleReset = () => {
    setNetworkDeviceName("");
    setHost("");
    setTypeOfNetworkDevice("");

    onApplyFilters(DEFAULT_FILTERS_NetworkDevices);
    onResetFilters?.();
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
            value={NetworkDeviceName}
            onChange={(e) => setNetworkDeviceName(e.target.value)}
            variant="outlined"
            fullWidth
          />
          <TextField
            label="IP адрес"
            value={Host}
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
              renderValue={(selected) => selected || "Не выбрано"}
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
            <Button variant="contained" color="info" onClick={handleReset}>
              Сбросить
            </Button>
            <Button variant="contained" color="success" onClick={handleApply}>
              Применить
            </Button>
          </ButtonGroup>
        </Box>
      </CardContent>
    </Card>
  );
}
