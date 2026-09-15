import {
  AccountTreeOutlined,
  AdminPanelSettingsOutlined,
  ArrowOutward,
  CreditCardOutlined,
  DnsOutlined,
  GroupOutlined,
  HubOutlined,
  Logout,
  Menu,
  Search,
  Terminal,
  Close,
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
  Typography,
} from "@mui/material";
import { NavLink, useLocation, useNavigate } from "react-router";
import { useState } from "react";
import { useAuth } from "../../lib/auth/useAuth";
import { routes } from "../router/paths";

export const sidebarWidth = 248;

export default function NavBar() {
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const { user, isAuthenticated, isAdmin, logout } = useAuth();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [isSigningOut, setIsSigningOut] = useState(false);
  const items = [
    { to: routes.main, label: "Network search", icon: Search },
    { to: routes.networkDevices, label: "Devices", icon: DnsOutlined },
    { to: routes.clients, label: "Clients", icon: GroupOutlined },
    { to: routes.sprVlans, label: "VLANs", icon: HubOutlined },
    { to: routes.tariffPlans, label: "Tariff plans", icon: CreditCardOutlined },
    { to: routes.gates, label: "Gates", icon: ArrowOutward, admin: true },
    {
      to: routes.adminWorkflows,
      label: "Workflows",
      icon: AccountTreeOutlined,
      admin: true,
    },
    { to: routes.adminUsers, label: "Users", icon: GroupOutlined, admin: true },
    {
      to: routes.admin,
      label: "Administration",
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
  const navigation = (
    <Box
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        p: 2.5,
        color: "#b6c4c8",
      }}
    >
      <Stack
        component={NavLink}
        to={routes.home}
        direction="row"
        gap={1.5}
        alignItems="center"
        sx={{ textDecoration: "none", color: "white", py: 1, mb: 5 }}
      >
        <Box
          sx={{
            display: "grid",
            placeItems: "center",
            width: 38,
            height: 38,
            bgcolor: "#45d6b0",
            color: "#102c2c",
            borderRadius: 2,
          }}
        >
          <Terminal />
        </Box>
        <Box>
          <Typography fontWeight={750} letterSpacing="-.03em">
            Remote Shell
          </Typography>
          <Typography fontSize={10} letterSpacing=".19em" color="#8da6a8">
            INTERRUPT
          </Typography>
        </Box>
      </Stack>
      <Typography
        variant="overline"
        sx={{
          color: "#81999d",
          fontSize: 10,
          letterSpacing: ".16em",
          mb: 1.5,
          px: 1.5,
        }}
      >
        Workspace
      </Typography>
      <Stack component="nav" aria-label="Main navigation" gap={0.6}>
        {items.map(({ to, label, icon: Icon }) => (
          <Button
            key={to}
            component={NavLink}
            to={to}
            onClick={() => setMobileOpen(false)}
            aria-current={active(to) ? "page" : undefined}
            startIcon={<Icon sx={{ fontSize: "20px !important" }} />}
            sx={{
              justifyContent: "flex-start",
              px: 1.5,
              minHeight: 44,
              fontWeight: active(to) ? 650 : 450,
              color: active(to) ? "#76edcb" : "#b6c4c8",
              bgcolor: active(to) ? "#213e3e" : "transparent",
              "&:hover": { bgcolor: "#253a3e", color: "white" },
            }}
          >
            {label}
          </Button>
        ))}
      </Stack>
      <Box sx={{ mt: "auto", pt: 4 }}>
        <Box
          sx={{ border: "1px solid #30464a", borderRadius: 2.5, p: 2, mb: 2.5 }}
        >
          <HubOutlined sx={{ color: "#6dd8bb", mb: 1 }} />
          <Typography color="#e2ebed" variant="body2" fontWeight={600}>
            Your network. Connected.
          </Typography>
          <Typography
            fontSize={12}
            sx={{ mt: 0.75, lineHeight: 1.7, color: "#98acb0" }}
          >
            Infrastructure, customers and automation in one workspace.
          </Typography>
        </Box>
        <Divider sx={{ borderColor: "#30464a", mb: 2 }} />
        {isAuthenticated ? (
          <Stack direction="row" gap={1} alignItems="center">
            <Avatar
              sx={{
                width: 34,
                height: 34,
                bgcolor: "#304b50",
                color: "#b8eadd",
                fontSize: 14,
              }}
            >
              {user?.email?.[0]?.toUpperCase() ?? "U"}
            </Avatar>
            <Box sx={{ minWidth: 0, flex: 1 }}>
              <Typography noWrap fontSize={12} color="#e2ebed">
                {user?.email}
              </Typography>
              <Typography fontSize={11} color="#98acb0">
                {isAdmin ? "Administrator" : "User"}
              </Typography>
            </Box>
            <IconButton
              aria-label="Sign out"
              disabled={isSigningOut}
              onClick={() => void handleLogout()}
              sx={{ color: "#b6c4c8" }}
            >
              {isSigningOut ? (
                <CircularProgress size={18} />
              ) : (
                <Logout fontSize="small" />
              )}
            </IconButton>
          </Stack>
        ) : (
          <Button component={NavLink} to={routes.login}>
            Sign in
          </Button>
        )}
      </Box>
    </Box>
  );
  return (
    <>
      <Box
        component="aside"
        sx={{
          display: { xs: "none", lg: "block" },
          position: "fixed",
          inset: "0 auto 0 0",
          width: sidebarWidth,
          bgcolor: "#14292e",
          overflowY: "auto",
        }}
      >
        {navigation}
      </Box>
      <Drawer
        open={mobileOpen}
        onClose={() => setMobileOpen(false)}
        PaperProps={{ sx: { width: sidebarWidth + 24, bgcolor: "#14292e" } }}
      >
        <IconButton
          aria-label="Close menu"
          onClick={() => setMobileOpen(false)}
          sx={{ position: "absolute", right: 6, top: 6, color: "#b6c4c8" }}
        >
          <Close fontSize="small" />
        </IconButton>
        {navigation}
      </Drawer>
      <Box
        component="header"
        sx={{
          ml: { lg: `${sidebarWidth}px` },
          px: { xs: 2, md: 4 },
          height: 76,
          display: "flex",
          alignItems: "center",
          gap: 2,
          borderBottom: "1px solid",
          borderColor: "divider",
          bgcolor: "background.paper",
        }}
      >
        <IconButton
          aria-label="Open menu"
          onClick={() => setMobileOpen(true)}
          sx={{ display: { lg: "none" } }}
        >
          <Menu />
        </IconButton>
        <Typography variant="body2" color="text.secondary">
          Workspace{" "}
          <Box component="span" sx={{ mx: 1.5, color: "#b4bfc2" }}>
            /
          </Box>
          <Box component="span" sx={{ color: "text.primary", fontWeight: 550 }}>
            {current?.label ?? "Overview"}
          </Box>
        </Typography>
        <Box sx={{ flex: 1 }} />
        <Typography
          sx={{
            display: { xs: "none", sm: "block" },
            fontSize: 11,
            letterSpacing: ".12em",
            color: "text.secondary",
          }}
        >
          NETWORK OPERATIONS
        </Typography>
      </Box>
    </>
  );
}
