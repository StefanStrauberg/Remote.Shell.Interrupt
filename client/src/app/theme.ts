import { alpha, createTheme } from "@mui/material/styles";

export const designTokens = Object.freeze({
  brand: {
    navy: "#14292E",
    blue: "#147D68",
    cyan: "#238B86",
    accent: "#74E6C3",
    onDark: "#F7FAFC",
  },
  gradients: {
    brand: "linear-gradient(135deg, #14292E 0%, #147D68 58%, #24506F 100%)",
    appBar: "linear-gradient(110deg, #14292E 0%, #24506F 55%, #147D68 100%)",
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
      light: "#E0F2EC",
      contrastText: "#FFFFFF",
    },
    secondary: {
      main: designTokens.brand.cyan,
      dark: "#176963",
      light: "#E1F3EF",
    },
    success: { main: "#2F855A" },
    warning: { main: "#C47B16" },
    error: { main: "#C74343" },
    info: { main: "#3978A8" },
    background: {
      default: "#F5F7F8",
      paper: "#FFFFFF",
    },
    text: {
      primary: "#203438",
      secondary: "#657A7F",
    },
    divider: "#E2E8EA",
  },
  shape: { borderRadius: 12 },
  spacing: 8,
  typography: {
    fontFamily: '"Roboto", "Segoe UI", sans-serif',
    h1: { fontSize: "2.5rem", fontWeight: 700, letterSpacing: "-0.03em" },
    h2: { fontSize: "2rem", fontWeight: 700, letterSpacing: "-0.025em" },
    h3: { fontSize: "1.75rem", fontWeight: 700, letterSpacing: "-0.02em" },
    h4: { fontSize: "1.8rem", fontWeight: 700, letterSpacing: "-0.015em" },
    h5: { fontSize: "1.25rem", fontWeight: 700 },
    h6: { fontSize: "1rem", fontWeight: 700 },
    button: { fontWeight: 600, textTransform: "none", letterSpacing: 0 },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        html: { minHeight: "100%", backgroundColor: "#F5F7F8" },
        body: {
          minHeight: "100%",
          margin: 0,
          backgroundColor: "#F5F7F8",
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
          minHeight: 40,
          borderRadius: 10,
          paddingInline: 18,
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: { backgroundImage: "none" },
        outlined: {
          borderColor: "#E2E8EA",
          boxShadow: designTokens.shadows.surface,
        },
      },
    },
    MuiCard: {
      defaultProps: { variant: "outlined" },
      styleOverrides: {
        root: {
          borderColor: "#E2E8EA",
          borderRadius: 16,
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
          borderRadius: 10,
          backgroundColor: "#FFFFFF",
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: designTokens.brand.blue,
          },
          "&.Mui-focused": {
            boxShadow: `0 0 0 3px ${alpha(designTokens.brand.blue, 0.12)}`,
          },
        },
        notchedOutline: { borderColor: "#D3DDDF" },
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
        paper: { borderRadius: 16 },
      },
    },
    MuiTableContainer: {
      styleOverrides: {
        root: { borderRadius: 12, border: "1px solid #E2E8EA" },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          backgroundColor: "#F6F8F9",
          color: "#50686D",
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
