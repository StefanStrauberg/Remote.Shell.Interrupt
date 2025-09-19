import {
  TextField,
  TextFieldProps,
  InputAdornment,
  IconButton,
  Tooltip,
  Box,
} from "@mui/material";
import {
  FieldValues,
  useController,
  UseControllerProps,
} from "react-hook-form";
import {
  Visibility,
  VisibilityOff,
  HelpOutline,
  Clear,
} from "@mui/icons-material";
import { useState, ReactNode } from "react";

type Props<T extends FieldValues> = UseControllerProps<T> & {
  helperText?: string;
  showClearButton?: boolean;
  onClear?: () => void;
  startAdornment?: ReactNode;
  endAdornment?: ReactNode;
  showPasswordToggle?: boolean;
  tooltip?: string;
} & Omit<
    TextFieldProps,
    "defaultValue" | "onChange" | "onBlur" | "value" | "ref"
  >;

export default function TextInput<T extends FieldValues>(props: Props<T>) {
  const {
    // Controller props
    name,
    control,
    rules,
    shouldUnregister,
    // Custom props
    helperText,
    showClearButton = false,
    onClear,
    startAdornment,
    endAdornment,
    showPasswordToggle = false,
    tooltip,
    // TextField props
    type: initialType = "text",
    ...textFieldProps
  } = props;

  const { field, fieldState } = useController({
    name,
    control,
    rules,
    shouldUnregister,
  });

  const [showPassword, setShowPassword] = useState(false);
  const [isFocused, setIsFocused] = useState(false);

  // Determine input type
  const type = showPasswordToggle
    ? showPassword
      ? "text"
      : "password"
    : initialType;

  // Handle clear button click
  const handleClear = () => {
    field.onChange("");
    onClear?.();
  };

  // Handle password visibility toggle
  const handleTogglePassword = () => {
    setShowPassword(!showPassword);
  };

  // Combine helper texts
  const combinedHelperText = fieldState.error?.message || helperText;

  // Build end adornment
  const buildEndAdornment = () => {
    const adornments: ReactNode[] = [];

    if (endAdornment) {
      adornments.push(endAdornment);
    }

    if (showPasswordToggle) {
      adornments.push(
        <InputAdornment position="end" key="password-toggle">
          <IconButton
            aria-label={showPassword ? "Hide password" : "Show password"}
            onClick={handleTogglePassword}
            edge="end"
            size="small"
          >
            {showPassword ? <VisibilityOff /> : <Visibility />}
          </IconButton>
        </InputAdornment>
      );
    }

    if (showClearButton && field.value && isFocused) {
      adornments.push(
        <InputAdornment position="end" key="clear-button">
          <IconButton
            aria-label="Clear input"
            onClick={handleClear}
            edge="end"
            size="small"
          >
            <Clear />
          </IconButton>
        </InputAdornment>
      );
    }

    if (tooltip) {
      adornments.push(
        <InputAdornment position="end" key="tooltip">
          <Tooltip title={tooltip} arrow>
            <HelpOutline
              color="action"
              sx={{
                fontSize: "1rem",
                opacity: 0.6,
                cursor: "help",
              }}
            />
          </Tooltip>
        </InputAdornment>
      );
    }

    return adornments.length > 0 ? adornments : null;
  };

  return (
    <Box position="relative">
      <TextField
        {...textFieldProps}
        {...field}
        value={field.value || ""}
        fullWidth
        variant="outlined"
        type={type}
        error={!!fieldState.error}
        helperText={combinedHelperText}
        onFocus={(e) => {
          setIsFocused(true);
          textFieldProps.onFocus?.(e);
        }}
        onBlur={(e) => {
          setIsFocused(false);
          field.onBlur();
          textFieldProps.onBlur?.(e);
        }}
        onChange={(e) => {
          field.onChange(e.target.value);
          textFieldProps.onChange?.(e);
        }}
        InputProps={{
          ...textFieldProps.InputProps,
          startAdornment: startAdornment ? (
            <InputAdornment position="start">{startAdornment}</InputAdornment>
          ) : undefined,
          endAdornment: buildEndAdornment(),
        }}
        sx={{
          "& .MuiOutlinedInput-root": {
            ...(textFieldProps.sx as any)?.["& .MuiOutlinedInput-root"],
          },
          ...textFieldProps.sx,
        }}
      />
    </Box>
  );
}
