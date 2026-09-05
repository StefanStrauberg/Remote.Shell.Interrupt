import { Storage, Menu as MenuIcon } from "@mui/icons-material";
import {
  AppBar,
  Box,
  Container,
  Toolbar,
  Typography,
  IconButton,
  Menu,
  MenuItem,
  Badge,
  Chip,
} from "@mui/material";
import { NavLink, useLocation } from "react-router";
import { useState } from "react";
import MenuItemLinks from "../shared/components/MenuItemLinks";

export default function NavBar() {
  const location = useLocation();
  const [mobileMenuAnchor, setMobileMenuAnchor] = useState<null | HTMLElement>(
    null
  );

  const handleMobileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setMobileMenuAnchor(event.currentTarget);
  };

  const handleMobileMenuClose = () => {
    setMobileMenuAnchor(null);
  };

  const navigationItems = [
    { to: "/mainPage", label: "Главная", badge: 0 },
    { to: "/gates", label: "Маршр-ры", badge: 0 },
    { to: "/clients", label: "Клиенты", badge: 0 },
    { to: "/sprVlans", label: "Вланы", badge: 0 },
    { to: "/tfPlans", label: "Планы", badge: 0 },
    { to: "/networkDevices", label: "Шлюзы", badge: 0 },
    { to: "/admin", label: "Админка", badge: 0 },
  ];

  const isActive = (path: string) => location.pathname === path;

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position="static"
        sx={{
          backgroundImage:
            "linear-gradient(35deg, #182a73 0%, #457b9d 69%, #1d3557)",
          position: "relative",
          padding: "0.5rem 0",
          boxShadow: "0 4px 20px rgba(0, 0, 0, 0.1)",
        }}
      >
        <Container maxWidth="xl">
          <Toolbar
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              minHeight: { xs: "64px", md: "80px" },
            }}
          >
            {/* Logo Section */}
            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <Box
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
                  transition: "all 0.3s ease",
                }}
              >
                <Storage
                  fontSize="large"
                  sx={{
                    color: "#f1faee",
                    transition: "transform 0.3s ease",
                    "&:hover": {
                      transform: "rotate(15deg)",
                    },
                  }}
                />
                <Typography
                  variant="h5"
                  fontWeight="bold"
                  sx={{
                    color: "#f1faee",
                    fontFamily: "'Poppins', sans-serif",
                    ml: 1,
                    display: { xs: "none", sm: "block" },
                  }}
                >
                  Remote Shell Interrupt
                </Typography>
                <Typography
                  variant="h6"
                  fontWeight="bold"
                  sx={{
                    color: "#f1faee",
                    fontFamily: "'Poppins', sans-serif",
                    ml: 1,
                    display: { xs: "block", sm: "none" },
                  }}
                >
                  RSI
                </Typography>
              </Box>
            </Box>

            {/* Desktop Navigation Links */}
            <Box sx={{ display: { xs: "none", md: "flex" }, gap: 1 }}>
              {navigationItems.map((item) => (
                <Box key={item.to} position="relative">
                  <MenuItemLinks to={item.to}>
                    {item.label}
                    {item.badge > 0 && (
                      <Chip
                        label={item.badge}
                        size="small"
                        color="error"
                        sx={{
                          ml: 1,
                          height: "20px",
                          minWidth: "20px",
                          fontSize: "0.75rem",
                        }}
                      />
                    )}
                  </MenuItemLinks>
                  {isActive(item.to) && (
                    <Box
                      sx={{
                        position: "absolute",
                        bottom: -8,
                        left: "50%",
                        transform: "translateX(-50%)",
                        width: "80%",
                        height: "3px",
                        backgroundColor: "#f1faee",
                        borderRadius: "2px",
                      }}
                    />
                  )}
                </Box>
              ))}
            </Box>

            {/* Mobile Menu Button */}
            <Box sx={{ display: { xs: "block", md: "none" } }}>
              <IconButton
                size="large"
                edge="end"
                color="inherit"
                aria-label="open menu"
                onClick={handleMobileMenuOpen}
                sx={{
                  color: "#f1faee",
                  "&:hover": {
                    backgroundColor: "rgba(255, 255, 255, 0.1)",
                  },
                }}
              >
                <MenuIcon />
              </IconButton>
            </Box>
          </Toolbar>
        </Container>

        {/* Mobile Menu */}
        <Menu
          anchorEl={mobileMenuAnchor}
          open={Boolean(mobileMenuAnchor)}
          onClose={handleMobileMenuClose}
          PaperProps={{
            sx: {
              backgroundColor: "rgba(29, 53, 87, 0.95)",
              backdropFilter: "blur(10px)",
              color: "#f1faee",
              minWidth: "200px",
            },
          }}
        >
          {navigationItems.map((item) => (
            <MenuItem
              key={item.to}
              component={NavLink}
              to={item.to}
              onClick={handleMobileMenuClose}
              selected={isActive(item.to)}
              sx={{
                "&.Mui-selected": {
                  backgroundColor: "rgba(255, 255, 255, 0.1)",
                },
                "&:hover": {
                  backgroundColor: "rgba(255, 255, 255, 0.05)",
                },
                py: 2,
                borderLeft: isActive(item.to)
                  ? "4px solid #f1faee"
                  : "4px solid transparent",
              }}
            >
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  width: "100%",
                }}
              >
                <Typography variant="body1">{item.label}</Typography>
                {item.badge > 0 && (
                  <Badge
                    badgeContent={item.badge}
                    color="error"
                    sx={{
                      "& .MuiBadge-badge": {
                        fontSize: "0.6rem",
                        height: "16px",
                        minWidth: "16px",
                      },
                    }}
                  />
                )}
              </Box>
            </MenuItem>
          ))}
        </Menu>
      </AppBar>
    </Box>
  );
}
