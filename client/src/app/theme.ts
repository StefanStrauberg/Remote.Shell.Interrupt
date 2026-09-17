import { alpha, createTheme } from "@mui/material/styles";

export const designTokens = Object.freeze({
  brand: {
    navy: "#1C2940",
    blue: "#355EC7",
    cyan: "#526C9C",
    accent: "#B0C6FF",
    onDark: "#F7FAFC",
  },
  gradients: {
    brand: "linear-gradient(135deg, #1C2940 0%, #355EC7 58%, #24506F 100%)",
    appBar: "linear-gradient(110deg, #1C2940 0%, #24506F 55%, #355EC7 100%)",
  },
  shadows: {
    surface: "0 2px 8px rgba(20, 41, 46, 0.025)",
    floating: "0 18px 50px rgba(23, 50, 77, 0.16)",
  },
});

export const appTheme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: designTokens.brand.blue,
      dark: designTokens.brand.navy,
      light: "#EDF2FF",
      contrastText: "#FFFFFF",
    },
    secondary: {
      main: designTokens.brand.cyan,
      dark: "#3B5280",
      light: "#EEF2FA",
    },
    success: { main: "#2F855A" },
    warning: { main: "#C47B16" },
    error: { main: "#C74343" },
    info: { main: "#3978A8" },
    background: {
      default: "#F4F6FA",
      paper: "#FFFFFF",
    },
    text: {
      primary: "#243246",
      secondary: "#617089",
    },
    divider: "#DFE5EE",
  },
  shape: { borderRadius: 8 },
  spacing: 8,
  typography: {
    fontFamily: '"Roboto", "Segoe UI", sans-serif',
    h1: { fontSize: "2.5rem", fontWeight: 700, letterSpacing: "-0.03em" },
    h2: { fontSize: "2rem", fontWeight: 700, letterSpacing: "-0.025em" },
    h3: { fontSize: "1.75rem", fontWeight: 700, letterSpacing: "-0.02em" },
    h4: { fontSize: "1.65rem", fontWeight: 700, letterSpacing: "-0.015em" },
    h5: { fontSize: "1.25rem", fontWeight: 700 },
    h6: { fontSize: "1rem", fontWeight: 700 },
    button: { fontWeight: 600, textTransform: "none", letterSpacing: 0 },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        html: { minHeight: "100%", backgroundColor: "#F4F6FA" },
        body: {
          minHeight: "100%",
          margin: 0,
          backgroundColor: "#F4F6FA",
          WebkitFontSmoothing: "antialiased",
          MozOsxFontSmoothing: "grayscale",
        },
        "#root": { minHeight: "100vh" },
      },
    },
    MuiAppBar: {
      defaultProps: { elevation: 0 },
    },
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: {
          minHeight: 36,
          borderRadius: 8,
          paddingInline: 18,
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: { backgroundImage: "none" },
        outlined: {
          borderColor: "#DFE5EE",
          boxShadow: designTokens.shadows.surface,
        },
      },
    },
    MuiCard: {
      defaultProps: { variant: "outlined" },
      styleOverrides: {
        root: {
          borderColor: "#DFE5EE",
          borderRadius: 8,
          boxShadow: designTokens.shadows.surface,
          transition: "box-shadow 180ms ease, transform 180ms ease",
        },
      },
    },
    MuiCardHeader: {
      styleOverrides: {
        root: { padding: "18px 20px", backgroundColor: "#FFFFFF" },
        title: { fontWeight: 650, fontSize: "0.95rem" },
        content: { minWidth: 0 },
      },
    },
    MuiCardContent: {
      styleOverrides: {
        root: { padding: 20, "&:last-child": { paddingBottom: 20 } },
      },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          borderRadius: 8,
          backgroundColor: "#FFFFFF",
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: designTokens.brand.blue,
          },
          "&.Mui-focused": {
            boxShadow: `0 0 0 3px ${alpha(designTokens.brand.blue, 0.12)}`,
          },
        },
        notchedOutline: { borderColor: "#CDD6E4" },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { borderRadius: 8, fontWeight: 500 },
      },
    },
    MuiAlert: {
      styleOverrides: {
        root: { borderRadius: 12, alignItems: "center" },
      },
    },
    MuiDialog: {
      styleOverrides: {
        paper: { borderRadius: 8 },
      },
    },
    MuiTableContainer: {
      styleOverrides: {
        root: { borderRadius: 12, border: "1px solid #DFE5EE" },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          backgroundColor: "#F5F7FB",
          color: "#566680",
          fontWeight: 700,
        },
      },
    },
    MuiPaginationItem: {
      styleOverrides: {
        root: { borderRadius: 8 },
      },
    },
    MuiTooltip: {
      styleOverrides: {
        tooltip: { borderRadius: 8, fontSize: "0.75rem" },
      },
    },
  },
});
