import {
  Storage,
  ArrowForward,
  Security,
  Speed,
  Dashboard,
} from "@mui/icons-material";
import {
  Box,
  Button,
  Paper,
  Typography,
  Container,
  Grid2,
  Fade,
  Zoom,
} from "@mui/material";
import { Link } from "react-router";
import { useState, useEffect } from "react";

export default function HomePage() {
  const [showContent, setShowContent] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setShowContent(true);
    }, 300);

    return () => clearTimeout(timer);
  }, []);

  const features = [
    {
      icon: <Security sx={{ fontSize: 40, color: "#ffc966ff" }} />,
      title: "Secure Access",
      description: "Optimized only for getting information using ICMP",
    },
    {
      icon: <Speed sx={{ fontSize: 40, color: "#06d6a0" }} />,
      title: "High Performance",
      description: "Optimized for fast information search and filtering",
    },
    {
      icon: <Dashboard sx={{ fontSize: 40, color: "#118ab2" }} />,
      title: "Dashboard",
      description: "Optimized only for network infrastructure monitoring",
    },
  ];

  return (
    <Paper
      sx={{
        color: "white",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        backgroundImage:
          "linear-gradient(135deg, #1d3557 0%, #457b9d 50%, #1d3557 100%)",
        backgroundSize: "400% 400%",
        animation: "gradientShift 15s ease infinite",
        boxShadow: "none",
        borderRadius: 0,
        overflow: "hidden",
        position: "relative",
        "&::before": {
          content: '""',
          position: "absolute",
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          background:
            "radial-gradient(circle at 30% 20%, rgba(255, 255, 255, 0.1) 0%, transparent 50%)",
          pointerEvents: "none",
        },
        "@keyframes gradientShift": {
          "0%": { backgroundPosition: "0% 50%" },
          "50%": { backgroundPosition: "100% 50%" },
          "100%": { backgroundPosition: "0% 50%" },
        },
      }}
    >
      {/* Animated background elements */}
      <Box
        sx={{
          position: "absolute",
          top: "20%",
          left: "10%",
          width: 100,
          height: 100,
          borderRadius: "50%",
          background: "rgba(255, 255, 255, 0.05)",
          animation: "float 6s ease-in-out infinite",
        }}
      />
      <Box
        sx={{
          position: "absolute",
          bottom: "30%",
          right: "15%",
          width: 80,
          height: 80,
          borderRadius: "50%",
          background: "rgba(255, 255, 255, 0.03)",
          animation: "float 8s ease-in-out infinite",
        }}
      />

      <Container maxWidth="lg">
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            textAlign: "center",
            gap: { xs: 3, md: 4 },
            py: { xs: 4, md: 6 },
          }}
        >
          {/* Logo Section */}
          <Fade in={showContent} timeout={1000}>
            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: 2,
                mb: 2,
              }}
            >
              <Box
                sx={{
                  position: "relative",
                  animation: "pulse 2s ease-in-out infinite",
                  "@keyframes pulse": {
                    "0%": { transform: "scale(1)" },
                    "50%": { transform: "scale(1.05)" },
                    "100%": { transform: "scale(1)" },
                  },
                }}
              >
                <Storage
                  sx={{
                    height: { xs: 80, md: 120 },
                    width: { xs: 80, md: 120 },
                    color: "#ffd166",
                    filter: "drop-shadow(0 4px 8px rgba(0, 0, 0, 0.3))",
                  }}
                />
              </Box>

              <Typography
                variant="h2"
                fontWeight="bold"
                sx={{
                  fontFamily: "'Poppins', sans-serif",
                  textShadow: "2px 2px 8px rgba(0, 0, 0, 0.4)",
                  fontSize: { xs: "2rem", md: "3rem" },
                  background:
                    "linear-gradient(45deg, #f1faee 0%, #a8dadc 100%)",
                  backgroundClip: "text",
                  WebkitBackgroundClip: "text",
                  WebkitTextFillColor: "transparent",
                }}
              >
                Remote Shell Interrupt
              </Typography>

              <Typography
                variant="h6"
                sx={{
                  color: "#e5e7eb",
                  fontWeight: 300,
                  textShadow: "1px 1px 4px rgba(0, 0, 0, 0.3)",
                  maxWidth: "600px",
                  lineHeight: 1.6,
                }}
              >
                Advanced monitoring platform for network infrastructure
              </Typography>
            </Box>
          </Fade>

          {/* Features Grid */}
          <Zoom
            in={showContent}
            timeout={1500}
            style={{ transitionDelay: showContent ? "500ms" : "0ms" }}
          >
            <Grid2 container spacing={3} sx={{ mb: 4 }}>
              {features.map((feature, index) => (
                <Grid2 size={{ xs: 12, sm: 6, md: 4 }} key={index}>
                  <Box
                    sx={{
                      textAlign: "center",
                      p: 3,
                      borderRadius: 2,
                      background: "rgba(255, 255, 255, 0.05)",
                      backdropFilter: "blur(10px)",
                      border: "1px solid rgba(255, 255, 255, 0.1)",
                      transition: "all 0.3s ease",
                      "&:hover": {
                        transform: "translateY(-4px)",
                        background: "rgba(255, 255, 255, 0.1)",
                        boxShadow: "0 8px 32px rgba(0, 0, 0, 0.2)",
                      },
                    }}
                  >
                    <Box sx={{ mb: 2 }}>{feature.icon}</Box>
                    <Typography
                      variant="h6"
                      fontWeight="bold"
                      sx={{ color: "#f1faee", mb: 1 }}
                    >
                      {feature.title}
                    </Typography>
                    <Typography
                      variant="body2"
                      sx={{ color: "#e5e7eb", opacity: 0.9 }}
                    >
                      {feature.description}
                    </Typography>
                  </Box>
                </Grid2>
              ))}
            </Grid2>
          </Zoom>

          {/* Action Button */}
          <Fade
            in={showContent}
            timeout={2000}
            style={{ transitionDelay: showContent ? "1000ms" : "0ms" }}
          >
            <Button
              component={Link}
              to="/mainPage"
              size="large"
              variant="contained"
              endIcon={<ArrowForward />}
              sx={{
                backgroundColor: "#ffd166",
                color: "#1d3557",
                height: { xs: 50, md: 60 },
                px: { xs: 3, md: 4 },
                borderRadius: "12px",
                fontSize: { xs: "1.1rem", md: "1.25rem" },
                fontWeight: "bold",
                textTransform: "none",
                boxShadow: "0 8px 24px rgba(255, 209, 102, 0.3)",
                transition: "all 0.3s ease",
                "&:hover": {
                  backgroundColor: "#ffc44d",
                  transform: "translateY(-2px) scale(1.02)",
                  boxShadow: "0 12px 32px rgba(255, 209, 102, 0.4)",
                },
                "&:active": {
                  transform: "translateY(0) scale(1)",
                },
              }}
            >
              Enter Dashboard
            </Button>
          </Fade>
        </Box>
      </Container>

      {/* Footer */}
      <Box
        sx={{
          position: "absolute",
          bottom: 20,
          textAlign: "center",
          width: "100%",
        }}
      >
        <Typography
          variant="caption"
          sx={{
            color: "rgba(255, 255, 255, 0.6)",
            fontSize: "0.8rem",
          }}
        >
          © 2024 Remote Shell Interrupt.
        </Typography>
      </Box>
    </Paper>
  );
}
