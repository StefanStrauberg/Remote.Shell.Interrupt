import { FilterList } from "@mui/icons-material";
import {
  Box,
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardHeader,
  Divider,
  TextField,
} from "@mui/material";

export default function MainPageListFilter() {
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
            variant="outlined"
            fullWidth
          />
          <TextField label="IP адрес" variant="outlined" fullWidth />
          <TextField label="Id влана" variant="outlined" fullWidth />
          <Divider />
          <ButtonGroup>
            <Button variant="contained" color="info">
              Сбросить
            </Button>
            <Button variant="contained" color="success">
              Применить
            </Button>
          </ButtonGroup>
        </Box>
      </CardContent>
    </Card>
  );
}
