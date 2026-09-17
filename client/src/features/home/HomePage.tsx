import {
  AccountTreeOutlined,
  ArrowForward,
  DnsOutlined,
  HubOutlined,
  Search,
  Terminal,
} from "@mui/icons-material";
import { Box, Button, Container, Stack, Typography } from "@mui/material";
import { Link } from "react-router";
import { routes } from "../../app/router/paths";

export default function HomePage() {
  return (
    <Box sx={{ minHeight: "100vh", bgcolor: "background.default" }}>
      <Container maxWidth="lg">
        <Stack
          component="header"
          direction="row"
          alignItems="center"
          justifyContent="space-between"
          sx={{ py: 3, borderBottom: "1px solid", borderColor: "divider" }}
        >
          <Stack direction="row" alignItems="center" gap={1.5}>
            <Terminal sx={{ color: "primary.main", fontSize: 32 }} />
            <Typography fontWeight={750}>
              Remote Shell{" "}
              <Box
                component="span"
                sx={{
                  color: "text.secondary",
                  fontWeight: 400,
                  display: { xs: "none", sm: "inline" },
                }}
              >
                {" "}
                / Interrupt
              </Box>
            </Typography>
          </Stack>
          <Button
            component={Link}
            to={routes.login}
            endIcon={<ArrowForward />}
            color="inherit"
          >
            Sign in
          </Button>
        </Stack>
        <Box
          component="main"
          sx={{
            py: { xs: 6, md: 10 },
            display: "grid",
            gridTemplateColumns: { xs: "1fr", md: "1.05fr 1fr" },
            alignItems: "center",
            gap: { xs: 5, md: 7 },
          }}
        >
          <Box>
            <Typography
              variant="overline"
              sx={{
                color: "primary.main",
                letterSpacing: ".18em",
                fontWeight: 700,
              }}
            >
              NETWORK OPERATIONS, SIMPLIFIED
            </Typography>
            <Typography
              component="h1"
              sx={{
                fontSize: { xs: 46, md: 64 },
                fontWeight: 750,
                lineHeight: 1.08,
                letterSpacing: "-.055em",
                mt: 2,
                mb: 3,
              }}
            >
              Clarity across
              <br />
              your entire
              <br />
              <Box component="span" sx={{ color: "primary.main" }}>
                network.
              </Box>
            </Typography>
            <Typography
              color="text.secondary"
              sx={{ maxWidth: 410, fontSize: 17, lineHeight: 1.8 }}
            >
              Connect the dots between your infrastructure and your customers.
              Search, explore and automate from one focused workspace.
            </Typography>
            <Button
              component={Link}
              to={routes.main}
              variant="contained"
              size="large"
              endIcon={<ArrowForward />}
              sx={{ mt: 4, py: 1.6 }}
            >
              Open workspace
            </Button>
            <Typography
              variant="caption"
              color="text.secondary"
              sx={{ display: "block", mt: 2 }}
            >
              SNMP discovery · VLAN search · Workflow automation
            </Typography>
          </Box>
          <Box
            sx={{
              bgcolor: "#14292e",
              borderRadius: 5,
              color: "#e5efed",
              p: { xs: 3, sm: 4 },
              boxShadow: "0 30px 70px #14292e20",
              position: "relative",
              overflow: "hidden",
            }}
          >
            <Stack
              direction="row"
              alignItems="center"
              justifyContent="space-between"
            >
              <Typography fontSize={12} letterSpacing=".12em" color="#a4bbb9">
                INFRASTRUCTURE MAP
              </Typography>
              <HubOutlined sx={{ color: "#6bdebb" }} />
            </Stack>
            <Box
              aria-hidden="true"
              sx={{
                height: 245,
                position: "relative",
                my: 3,
                backgroundImage:
                  "radial-gradient(#375354 1px, transparent 1px)",
                backgroundSize: "18px 18px",
              }}
            >
              <svg
                viewBox="0 0 400 245"
                width="100%"
                height="100%"
                style={{ position: "absolute", inset: 0 }}
              >
                <path
                  d="M200 55 V120 H65 V185 M200 120 V185 M200 120 H335 V185"
                  fill="none"
                  stroke="#58877e"
                  strokeWidth="1.5"
                />
                <circle cx="200" cy="120" r="4" fill="#6bdebb" />
              </svg>
              <Box
                sx={{
                  position: "absolute",
                  left: "50%",
                  top: 12,
                  transform: "translateX(-50%)",
                  display: "grid",
                  placeItems: "center",
                  width: 80,
                  height: 60,
                  bgcolor: "#27473f",
                  border: "1px solid #609a88",
                  borderRadius: 2,
                }}
              >
                <HubOutlined sx={{ color: "#83f0cd", fontSize: 30 }} />
              </Box>
              <Box
                sx={{
                  position: "absolute",
                  bottom: 9,
                  width: "100%",
                  display: "flex",
                  justifyContent: "space-around",
                }}
              >
                {["Devices", "VLANs", "Clients"].map((label) => (
                  <Box
                    key={label}
                    sx={{
                      textAlign: "center",
                      bgcolor: "#1e373c",
                      border: "1px solid #3b565a",
                      borderRadius: 2,
                      p: 1.5,
                      width: 90,
                    }}
                  >
                    <DnsOutlined sx={{ color: "#a9c6c2" }} />
                    <Typography fontSize={11} mt={0.5}>
                      {label}
                    </Typography>
                  </Box>
                ))}
              </Box>
            </Box>
            <Typography variant="h6">One connected perspective.</Typography>
            <Typography
              fontSize={13}
              sx={{ color: "#a4bbb9", mt: 1, lineHeight: 1.8 }}
            >
              Explore the relationships between devices, VLANs and customer
              records.
            </Typography>
          </Box>
        </Box>
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: { xs: "1fr", md: "repeat(3, 1fr)" },
            borderTop: "1px solid",
            borderColor: "divider",
            gap: 4,
            py: 4,
          }}
        >
          {[
            {
              icon: Search,
              title: "Find it faster",
              text: "Trace a VLAN to its connected devices and customer details.",
            },
            {
              icon: DnsOutlined,
              title: "Know your infrastructure",
              text: "Explore discovered equipment, ports and network information.",
            },
            {
              icon: AccountTreeOutlined,
              title: "Make work flow",
              text: "Build visual workflows for repeatable network operations.",
            },
          ].map(({ icon: Icon, title, text }) => (
            <Stack key={title} direction="row" gap={2}>
              <Icon sx={{ color: "primary.main", mt: 0.5 }} />
              <Box>
                <Typography fontWeight={650} mb={0.75}>
                  {title}
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  lineHeight={1.75}
                >
                  {text}
                </Typography>
              </Box>
            </Stack>
          ))}
        </Box>
        <Typography
          component="footer"
          variant="caption"
          color="text.secondary"
          sx={{ py: 3, display: "block" }}
        >
          Remote Shell Interrupt / Network infrastructure workspace
        </Typography>
      </Container>
    </Box>
  );
}
