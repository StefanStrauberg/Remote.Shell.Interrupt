import { Group } from "@mui/icons-material";
import {
  AppBar,
  Box,
  Button,
  Container,
  MenuItem,
  Toolbar,
  Typography,
} from "@mui/material";

export default function NavBar() {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position="static"
        sx={{
          backgroundImage:
            "linear-gradient(35deg, #182a73 0%, #218aae 69%, #20a7ac)",
        }}
      >
        <Container maxWidth="xl">
          <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
            <Box>
              <MenuItem sx={{ display: "flex", gap: 2 }}>
                <Group fontSize="medium" />
                <Typography variant="h5" fontWeight="bold">
                  Remote Shell Interrupt
                </Typography>
              </MenuItem>
            </Box>
            <Box sx={{ display: "flex" }}>
              <MenuItem
                sx={{
                  fontSize: "1.2rem",
                }}
              >
                NetworkDevices
              </MenuItem>
              <MenuItem
                sx={{
                  fontSize: "1.2rem",
                }}
              >
                Organizations
              </MenuItem>
              <MenuItem
                sx={{
                  fontSize: "1.2rem",
                }}
              >
                Gates
              </MenuItem>
            </Box>
            <Button size="large" variant="contained" color="warning">
              Button
            </Button>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
