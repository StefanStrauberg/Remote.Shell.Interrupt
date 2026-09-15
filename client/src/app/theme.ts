import { alpha, createTheme } from "@mui/material/styles";

export const designTokens = Object.freeze({
  brand: {
    navy: "#17324D",
    blue: "#2F6B9A",
    cyan: "#3A91A8",
    accent: "#F4C95D",
    onDark: "#F7FAFC",
  },
  gradients: {
    brand: "linear-gradient(135deg, #17324D 0%, #2F6B9A 58%, #24506F 100%)",
    appBar: "linear-gradient(110deg, #17324D 0%, #24506F 55%, #2F6B9A 100%)",
  },
  shadows: {
    surface: "0 8px 28px rgba(23, 50, 77, 0.08)",
    floating: "0 18px 50px rgba(23, 50, 77, 0.16)",
  },
});

export const appTheme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: designTokens.brand.blue,
      dark: designTokens.brand.navy,
      light: "#DCEAF3",
      contrastText: "#FFFFFF",
    },
    secondary: {
      main: designTokens.brand.cyan,
      dark: "#256D80",
      light: "#D9EEF2",
    },
    success: { main: "#2F855A" },
    warning: { main: "#C47B16" },
    error: { main: "#C74343" },
    info: { main: "#3978A8" },
    background: {
      default: "#F3F6F9",
      paper: "#FFFFFF",
    },
    text: {
      primary: "#17283A",
      secondary: "#607286",
    },
    divider: "#DCE4EC",
  },
  shape: { borderRadius: 12 },
  spacing: 8,
  typography: {
    fontFamily: '"Roboto", "Segoe UI", sans-serif',
    h1: { fontSize: "2.5rem", fontWeight: 700, letterSpacing: "-0.03em" },
    h2: { fontSize: "2rem", fontWeight: 700, letterSpacing: "-0.025em" },
    h3: { fontSize: "1.75rem", fontWeight: 700, letterSpacing: "-0.02em" },
    h4: { fontSize: "1.5rem", fontWeight: 700, letterSpacing: "-0.015em" },
    h5: { fontSize: "1.25rem", fontWeight: 700 },
    h6: { fontSize: "1rem", fontWeight: 700 },
    button: { fontWeight: 600, textTransform: "none", letterSpacing: 0 },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        html: { minHeight: "100%", backgroundColor: "#F3F6F9" },
        body: {
          minHeight: "100%",
          margin: 0,
          backgroundColor: "#F3F6F9",
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
          borderColor: "#DCE4EC",
          boxShadow: designTokens.shadows.surface,
        },
      },
    },
    MuiCard: {
      defaultProps: { variant: "outlined" },
      styleOverrides: {
        root: {
          borderColor: "#DCE4EC",
          borderRadius: 14,
          boxShadow: designTokens.shadows.surface,
          transition: "box-shadow 180ms ease, transform 180ms ease",
        },
      },
    },
    MuiCardHeader: {
      styleOverrides: {
        root: { padding: "18px 20px", backgroundColor: "#F8FAFC" },
        title: { fontWeight: 700 },
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
        notchedOutline: { borderColor: "#C9D5E1" },
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
        root: { borderRadius: 12, border: "1px solid #DCE4EC" },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          backgroundColor: "#F4F7FA",
          color: "#30475C",
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
