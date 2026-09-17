import {
  AccountTreeOutlined,
  AdminPanelSettingsOutlined,
  ArrowOutward,
  CreditCardOutlined,
  DarkModeOutlined,
  DnsOutlined,
  GroupOutlined,
  HubOutlined,
  LightModeOutlined,
  Logout,
  Menu,
  Terminal,
  Close,
  Search,
} from "@mui/icons-material";
import {
  Avatar,
  Box,
  Button,
  CircularProgress,
  Divider,
  Drawer,
  IconButton,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import { NavLink, useLocation, useNavigate } from "react-router";
import { useState } from "react";
import { useAuth } from "../../lib/auth/useAuth";
import { routes } from "../router/paths";
import { useThemeMode } from "../ThemeModeContext";

export const sidebarWidth = 104;

export default function NavBar() {
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const { user, isAuthenticated, isAdmin, logout } = useAuth();
  const { mode, toggleMode } = useThemeMode();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [isSigningOut, setIsSigningOut] = useState(false);
  const items = [
    { to: routes.main, label: "Network search", short: "Search", icon: Search },
    {
      to: routes.networkDevices,
      label: "Devices",
      short: "Devices",
      icon: DnsOutlined,
    },
    {
      to: routes.clients,
      label: "Clients",
      short: "Clients",
      icon: GroupOutlined,
    },
    { to: routes.sprVlans, label: "VLANs", short: "VLANs", icon: HubOutlined },
    {
      to: routes.tariffPlans,
      label: "Tariff plans",
      short: "Tariffs",
      icon: CreditCardOutlined,
    },
    {
      to: routes.gates,
      label: "Gates",
      short: "Gates",
      icon: ArrowOutward,
      admin: true,
    },
    {
      to: routes.adminWorkflows,
      label: "Workflows",
      short: "Workflows",
      icon: AccountTreeOutlined,
      admin: true,
    },
    {
      to: routes.adminUsers,
      label: "Users",
      short: "Users",
      icon: GroupOutlined,
      admin: true,
    },
    {
      to: routes.admin,
      label: "Administration",
      short: "Admin",
      icon: AdminPanelSettingsOutlined,
      admin: true,
    },
  ].filter((item) => isAuthenticated && (!item.admin || isAdmin));
  const active = (to: string) =>
    pathname === to ||
    (to !== routes.admin && pathname.startsWith(`${to}/`)) ||
    (to === routes.gates && pathname === routes.createGate);
  const current = items.find((item) => active(item.to));
  const handleLogout = async () => {
    setIsSigningOut(true);
    try {
      await logout();
      navigate(routes.login, { replace: true });
    } finally {
      setIsSigningOut(false);
    }
  };
  const navigation = (expanded: boolean) => (
    <Stack sx={{ height: "100%", p: 1.25, gap: 1 }}>
      <Button
        component={NavLink}
        to={routes.home}
        aria-label="Remote Shell home"
        sx={{ gap: 1, minHeight: 56, mb: 1 }}
      >
        <Box
          sx={{
            display: "grid",
            placeItems: "center",
            bgcolor: "primary.main",
            color: "primary.contrastText",
            width: 36,
            height: 36,
            borderRadius: 2,
          }}
        >
          <Terminal />
        </Box>
        {expanded && <Typography fontWeight={700}>Remote Shell</Typography>}
      </Button>
      <Stack component="nav" aria-label="Main navigation" gap={0.5}>
        {items.map(({ to, label, short, icon: Icon, admin }, index) => (
          <Box key={to}>
            {admin && !items[index - 1]?.admin && <Divider sx={{ my: 1.5 }} />}
            <Button
              component={NavLink}
              to={to}
              end={to === routes.admin}
              aria-label={label}
              aria-current={active(to) ? "page" : undefined}
              onClick={() => setMobileOpen(false)}
              sx={{
                width: "100%",
                minWidth: 0,
                minHeight: 54,
                px: 1,
                gap: 0.5,
                flexDirection: expanded ? "row" : "column",
                justifyContent: expanded ? "flex-start" : "center",
                fontSize: expanded ? 13 : 11,
                color: active(to) ? "primary.main" : "text.secondary",
                bgcolor: active(to) ? "primary.light" : "transparent",
                "&:hover": { bgcolor: "action.hover" },
              }}
            >
              <Icon fontSize="small" />
              {expanded ? label : short}
            </Button>
          </Box>
        ))}
      </Stack>
      <Box sx={{ mt: "auto", pt: 2, textAlign: "center" }}>
        <Typography variant="caption" color="text.secondary">
          {isAdmin ? "Admin" : "Workspace"}
        </Typography>
      </Box>
    </Stack>
  );
  return (
    <>
      <Box
        component="aside"
        sx={{
          display: { xs: "none", md: "block" },
          position: "fixed",
          inset: "0 auto 0 0",
          width: sidebarWidth,
          bgcolor: "background.paper",
          borderRight: 1,
          borderColor: "divider",
          overflowY: "auto",
        }}
      >
        {navigation(false)}
      </Box>
      <Drawer
        open={mobileOpen}
        onClose={() => setMobileOpen(false)}
        PaperProps={{ sx: { width: 264 } }}
      >
        <IconButton
          aria-label="Close menu"
          onClick={() => setMobileOpen(false)}
          sx={{ position: "absolute", right: 4, top: 4 }}
        >
          <Close fontSize="small" />
        </IconButton>
        {navigation(true)}
      </Drawer>
      <Box
        component="header"
        sx={{
          ml: { md: `${sidebarWidth}px` },
          px: { xs: 2, md: 3 },
          minHeight: 64,
          display: "flex",
          alignItems: "center",
          gap: 1.5,
          borderBottom: 1,
          borderColor: "divider",
          bgcolor: "background.paper",
        }}
      >
        <IconButton
          aria-label="Open menu"
          onClick={() => setMobileOpen(true)}
          sx={{ display: { md: "none" } }}
        >
          <Menu />
        </IconButton>
        <Typography variant="body2" color="text.secondary" sx={{ minWidth: 0 }}>
          <Box component="span" sx={{ display: { xs: "none", sm: "inline" } }}>
            Engineering workspace /{" "}
          </Box>
          <Box component="span" sx={{ color: "text.primary", fontWeight: 600 }}>
            {current?.label ?? "Overview"}
          </Box>
        </Typography>
        <Box sx={{ flex: 1 }} />
        <Tooltip
          title={mode === "dark" ? "Switch to light mode" : "Switch to dark mode"}
        >
          <IconButton
            aria-label={
              mode === "dark" ? "Switch to light mode" : "Switch to dark mode"
            }
            onClick={toggleMode}
          >
            {mode === "dark" ? (
              <LightModeOutlined fontSize="small" />
            ) : (
              <DarkModeOutlined fontSize="small" />
            )}
          </IconButton>
        </Tooltip>
        {isAuthenticated ? (
          <Stack direction="row" alignItems="center" gap={1}>
            <Avatar
              sx={{
                width: 30,
                height: 30,
                fontSize: 13,
                bgcolor: "primary.light",
                color: "primary.main",
              }}
            >
              {user?.email?.[0]?.toUpperCase() ?? "U"}
            </Avatar>
            <Typography
              variant="caption"
              noWrap
              sx={{ display: { xs: "none", sm: "block" }, maxWidth: 210 }}
            >
              {user?.email}
            </Typography>
            <Tooltip title="Sign out">
              <IconButton
                aria-label="Sign out"
                disabled={isSigningOut}
                onClick={() => void handleLogout()}
              >
                {isSigningOut ? (
                  <CircularProgress size={18} />
                ) : (
                  <Logout fontSize="small" />
                )}
              </IconButton>
            </Tooltip>
          </Stack>
        ) : (
          <Button component={NavLink} to={routes.login}>
            Sign in
          </Button>
        )}
      </Box>
    </>
  );
}
