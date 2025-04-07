import { Storage } from "@mui/icons-material";
import {
  AppBar,
  Box,
  Container,
  MenuItem,
  Toolbar,
  Typography,
} from "@mui/material";
import { NavLink } from "react-router";
import MenuItemLinks from "../shared/components/MenuItemLinks";

export default function NavBar() {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position="static"
        sx={{
          backgroundImage:
            "linear-gradient(35deg, #182a73 0%, #457b9d 69%, #1d3557)",
          position: "relative",
          padding: "0.5rem 0",
        }}
      >
        <Container maxWidth="xl">
          <Toolbar
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            {/* Logo Section */}
            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <MenuItem
                component={NavLink}
                to="/"
                sx={{
                  display: "flex",
                  alignItems: "center",
                  textDecoration: "none",
                  "&:hover": {
                    backgroundColor: "rgba(255, 255, 255, 0.1)",
                    borderRadius: "8px",
                  },
                  padding: "0.5rem",
                }}
              >
                <Storage fontSize="large" sx={{ color: "#f1faee" }} />
                <Typography
                  variant="h5"
                  fontWeight="bold"
                  sx={{
                    color: "#f1faee",
                    fontFamily: "'Poppins', sans-serif",
                  }}
                >
                  Remote Shell Interrupt
                </Typography>
              </MenuItem>
            </Box>
            {/* Navigation Links */}
            <Box sx={{ display: "flex", gap: 2 }}>
              <MenuItemLinks to="/networkdevices">Главная</MenuItemLinks>
              <MenuItemLinks to="/gates">Маршр-ры</MenuItemLinks>
              <MenuItemLinks to="/clients">Клиенты</MenuItemLinks>
              <MenuItemLinks to="/tfPlans">Планы</MenuItemLinks>
              <MenuItemLinks to="/sprVlans">Вланы</MenuItemLinks>
            </Box>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
