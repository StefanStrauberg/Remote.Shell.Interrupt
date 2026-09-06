import { Storage, Menu as MenuIcon, Login as LoginIcon, Logout as LogoutIcon } from "@mui/icons-material";
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
  Button,
  CircularProgress
} from "@mui/material";
import { NavLink, useLocation, useNavigate } from "react-router";
import { useState } from "react";
import MenuItemLinks from "../shared/components/MenuItemLinks";
import { useAuth } from "../../lib/auth/useAuth";

type NavigationItem = {
  to: string;
  label: string;
  badge: number;
  adminOnly: boolean;
};

export default function NavBar() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user, isAuthenticated, isAdmin, logout } = useAuth();
  const [mobileMenuAnchor, setMobileMenuAnchor] = useState<null | HTMLElement>(
    null
  );
  const [isSigningOut, setIsSigningOut] = useState(false);

  const handleMobileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setMobileMenuAnchor(event.currentTarget);
  };

  const handleMobileMenuClose = () => {
    setMobileMenuAnchor(null);
  };

  const handleLogout = async () => {
    setIsSigningOut(true);
    await logout();
    setIsSigningOut(false);
    navigate("/login", { replace: true });
  };

  const navigationItems: NavigationItem[] = [
    { to: "/mainPage", label: "Главная", badge: 0, adminOnly: false },
    { to: "/gates", label: "Маршр-ры", badge: 0, adminOnly: true },
    { to: "/clients", label: "Клиенты", badge: 0, adminOnly: false },
    { to: "/sprVlans", label: "Вланы", badge: 0, adminOnly: false },
    { to: "/tfPlans", label: "Планы", badge: 0, adminOnly: false },
    { to: "/networkDevices", label: "Шлюзы", badge: 0, adminOnly: false },
    { to: "/admin", label: "Админка", badge: 0, adminOnly: true }
  ];

  // Admin-only destinations are hidden from users without the role.
  const visibleItems = isAuthenticated
    ? navigationItems.filter((item) => !item.adminOnly || isAdmin)
    : [];

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
          boxShadow: "0 4px 20px rgba(0, 0, 0, 0.1)"
        }}
      >
        <Container maxWidth="xl">
          <Toolbar
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              minHeight: { xs: "64px", md: "80px" }
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
                    borderRadius: "8px"
                  },
                  padding: "0.5rem",
                  transition: "all 0.3s ease"
                }}
              >
                <Storage
                  fontSize="large"
                  sx={{
                    color: "#f1faee",
                    transition: "transform 0.3s ease",
                    "&:hover": {
                      transform: "rotate(15deg)"
                    }
                  }}
                />
                <Typography
                  variant="h5"
                  fontWeight="bold"
                  sx={{
                    color: "#f1faee",
                    fontFamily: "'Poppins', sans-serif",
                    ml: 1,
                    display: { xs: "none", sm: "block" }
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
                    display: { xs: "block", sm: "none" }
                  }}
                >
                  RSI
                </Typography>
              </Box>
            </Box>

            {/* Desktop Navigation Links */}
            <Box sx={{ display: { xs: "none", md: "flex" }, gap: 1 }}>
              {visibleItems.map((item) => (
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
                          fontSize: "0.75rem"
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
                        borderRadius: "2px"
                      }}
                    />
                  )}
                </Box>
              ))}
            </Box>

            {/* Auth Controls + Mobile Menu Button */}
            <Box display="flex" alignItems="center" gap={1}>
              {isAuthenticated ? (
                <>
                  <Chip
                    label={user?.email ?? ""}
                    variant="outlined"
                    size="small"
                    sx={{
                      color: "#f1faee",
                      borderColor: "rgba(241, 250, 238, 0.5)",
                      display: { xs: "none", lg: "inline-flex" }
                    }}
                  />
                  <Chip
                    label={isAdmin ? "Admin" : "User"}
                    size="small"
                    color={isAdmin ? "warning" : "default"}
                  />
                  <IconButton
                    aria-label="sign out"
                    title="Sign out"
                    onClick={handleLogout}
                    disabled={isSigningOut}
                    sx={{
                      color: "#f1faee",
                      "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.1)" }
                    }}
                  >
                    {isSigningOut ? <CircularProgress size={20} /> : <LogoutIcon />}
                  </IconButton>
                </>
              ) : (
                <Button
                  component={NavLink}
                  to="/login"
                  variant="contained"
                  size="small"
                  startIcon={<LoginIcon />}
                  sx={{
                    backgroundColor: "#ffd166",
                    color: "#1d3557",
                    fontWeight: "bold",
                    "&:hover": { backgroundColor: "#ffc44d" }
                  }}
                >
                  Sign in
                </Button>
              )}

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
                      backgroundColor: "rgba(255, 255, 255, 0.1)"
                    }
                  }}
                >
                  <MenuIcon />
                </IconButton>
              </Box>
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
              minWidth: "200px"
            }
          }}
        >
          {visibleItems.map((item) => (
            <MenuItem
              key={item.to}
              component={NavLink}
              to={item.to}
              onClick={handleMobileMenuClose}
              selected={isActive(item.to)}
              sx={{
                "&.Mui-selected": {
                  backgroundColor: "rgba(255, 255, 255, 0.1)"
                },
                "&:hover": {
                  backgroundColor: "rgba(255, 255, 255, 0.05)"
                },
                py: 2,
                borderLeft: isActive(item.to)
                  ? "4px solid #f1faee"
                  : "4px solid transparent"
              }}
            >
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  width: "100%"
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
                        minWidth: "16px"
                      }
                    }}
                  />
                )}
              </Box>
            </MenuItem>
          ))}

          {isAuthenticated && (
            <MenuItem
              onClick={() => {
                handleMobileMenuClose();
                void handleLogout();
              }}
              sx={{ py: 2 }}
            >
              <Box display="flex" alignItems="center" gap={1}>
                <LogoutIcon fontSize="small" />
                <Typography variant="body1">Выйти</Typography>
              </Box>
            </MenuItem>
          )}
        </Menu>
      </AppBar>
    </Box>
  );
}
