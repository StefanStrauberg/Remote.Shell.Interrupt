import { alpha, createTheme, type PaletteMode, type Theme } from "@mui/material/styles";

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

const lightSurfaces = {
  default: "#F4F6FA",
  paper: "#FFFFFF",
  paperAlt: "#F5F7FB",
  border: "#DFE5EE",
  borderStrong: "#CDD6E4",
  textPrimary: "#243246",
  textSecondary: "#617089",
};

const darkSurfaces = {
  default: "#0E1420",
  paper: "#141B2B",
  paperAlt: "#1A2336",
  border: "#28324A",
  borderStrong: "#374361",
  textPrimary: "#E9EDF6",
  textSecondary: "#94A1BD",
};

export function buildTheme(mode: PaletteMode): Theme {
  const isDark = mode === "dark";
  const s = isDark ? darkSurfaces : lightSurfaces;
  const primaryMain = isDark ? "#7C9BFF" : designTokens.brand.blue;

  return createTheme({
    palette: {
      mode,
      primary: {
        main: primaryMain,
        dark: isDark ? "#4F6FD6" : designTokens.brand.navy,
        light: isDark ? alpha(primaryMain, 0.16) : "#EDF2FF",
        contrastText: isDark ? "#0E1420" : "#FFFFFF",
      },
      secondary: {
        main: isDark ? "#8FA6D6" : designTokens.brand.cyan,
        dark: isDark ? "#5E76A8" : "#3B5280",
        light: isDark ? alpha("#8FA6D6", 0.16) : "#EEF2FA",
      },
      success: { main: isDark ? "#4ADE94" : "#2F855A" },
      warning: { main: isDark ? "#F0B855" : "#C47B16" },
      error: { main: isDark ? "#F27272" : "#C74343" },
      info: { main: isDark ? "#5FA8DE" : "#3978A8" },
      background: {
        default: s.default,
        paper: s.paper,
      },
      text: {
        primary: s.textPrimary,
        secondary: s.textSecondary,
      },
      divider: s.border,
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
          html: { minHeight: "100%", backgroundColor: s.default },
          body: {
            minHeight: "100%",
            margin: 0,
            backgroundColor: s.default,
            WebkitFontSmoothing: "antialiased",
            MozOsxFontSmoothing: "grayscale",
            colorScheme: mode,
          },
          "#root": { minHeight: "100vh" },
          "*": { scrollbarColor: `${s.borderStrong} transparent` },
          "*::-webkit-scrollbar": { width: 10, height: 10 },
          "*::-webkit-scrollbar-thumb": {
            backgroundColor: s.borderStrong,
            borderRadius: 8,
            border: `2px solid ${s.default}`,
          },
          "*::-webkit-scrollbar-track": { backgroundColor: "transparent" },
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
            borderColor: s.border,
            boxShadow: isDark ? "none" : designTokens.shadows.surface,
          },
        },
      },
      MuiCard: {
        defaultProps: { variant: "outlined" },
        styleOverrides: {
          root: {
            borderColor: s.border,
            borderRadius: 8,
            boxShadow: isDark ? "none" : designTokens.shadows.surface,
            transition:
              "box-shadow 180ms ease, transform 180ms ease, border-color 180ms ease",
          },
        },
      },
      MuiCardHeader: {
        styleOverrides: {
          root: { padding: "18px 20px", backgroundColor: s.paper },
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
            backgroundColor: s.paper,
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: primaryMain,
            },
            "&.Mui-focused": {
              boxShadow: `0 0 0 3px ${alpha(primaryMain, isDark ? 0.24 : 0.12)}`,
            },
          },
          notchedOutline: { borderColor: s.borderStrong },
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
          root: { borderRadius: 12, border: `1px solid ${s.border}` },
        },
      },
      MuiTableRow: {
        styleOverrides: {
          root: {
            "&:hover": {
              backgroundColor: isDark
                ? alpha(primaryMain, 0.06)
                : alpha(primaryMain, 0.035),
            },
          },
        },
      },
      MuiTableCell: {
        styleOverrides: {
          root: { borderColor: s.border },
          head: {
            backgroundColor: s.paperAlt,
            color: s.textSecondary,
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
}

export const appTheme = buildTheme("light");
export const appThemeDark = buildTheme("dark");
