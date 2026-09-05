import {
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
  Chip,
  Box,
  useTheme,
} from "@mui/material";
import { SelectChangeEvent } from "@mui/material/Select";
import {
  FieldValues,
  useController,
  UseControllerProps,
} from "react-hook-form";
import { ReactNode, FocusEvent } from "react";

interface SelectItem {
  text: string;
  value: string;
  disabled?: boolean;
  icon?: ReactNode;
}

// Create proper types for the Select props we want to include
type SelectPropsWithoutConflicts = Omit<
  React.ComponentProps<typeof Select>,
  "name" | "onChange" | "onBlur" | "value" | "ref" | "defaultValue"
>;

// Replace the existing Props type with:
type Props<T extends FieldValues> = {
  items: SelectItem[];
  label: string;
  multiple?: boolean;
  showChips?: boolean;
  placeholder?: string;
  size?: "small" | "medium";
  onChange?: (event: SelectChangeEvent<unknown>, child: ReactNode) => void;
  onBlur?: (event: FocusEvent<HTMLInputElement>) => void;
} & UseControllerProps<T> &
  Omit<
    SelectPropsWithoutConflicts,
    "name" | "onChange" | "onBlur" | "value" | "ref"
  >;

export default function SelectInput<T extends FieldValues>(props: Props<T>) {
  const {
    name,
    control,
    rules,
    shouldUnregister,
    items,
    label,
    multiple = false,
    showChips = false,
    placeholder,
    size = "medium",
    onChange, // Properly typed now
    onBlur, // Properly typed now
    ...selectProps
  } = props;

  const theme = useTheme();
  const { field, fieldState } = useController({
    name,
    control,
    rules,
    shouldUnregister,
  });

  const renderValue = (selected: unknown) => {
    if (multiple && Array.isArray(selected)) {
      if (selected.length === 0) {
        return placeholder || `Select ${label}`;
      }

      if (showChips) {
        return (
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
            {selected.map((value) => {
              const item = items.find((item) => item.value === value);
              return (
                <Chip
                  key={value}
                  label={item?.text || value}
                  size="small"
                  sx={{
                    backgroundColor: theme.palette.primary.light,
                    color: theme.palette.primary.contrastText,
                  }}
                />
              );
            })}
          </Box>
        );
      }

      return selected
        .map((value) => {
          const item = items.find((item) => item.value === value);
          return item?.text || value;
        })
        .join(", ");
    }

    if (!selected || selected === "") {
      return placeholder || `Select ${label}`;
    }

    const item = items.find((item) => item.value === selected);
    return item?.text || String(selected);
  };

  const handleChange = (
    event: SelectChangeEvent<unknown>,
    child: ReactNode
  ) => {
    field.onChange(event.target.value);
    onChange?.(event, child);
  };

  const handleBlur = () => {
    field.onBlur();
    // MUI Select's onBlur doesn't receive parameters in the same way as TextField
    if (onBlur) {
      // You might need to adapt this based on how you want to handle onBlur
      const syntheticEvent = {
        target: { value: field.value },
        type: "blur",
      } as unknown as FocusEvent<HTMLInputElement>;
      onBlur(syntheticEvent);
    }
  };

  // Safe value checking for multiple selects
  const hasValue = multiple
    ? Array.isArray(field.value) && field.value.length > 0
    : !!field.value && field.value !== "";

  return (
    <FormControl
      fullWidth
      error={!!fieldState.error}
      size={size}
      disabled={selectProps.disabled}
    >
      <InputLabel id={`${field.name}-label`} shrink={hasValue}>
        {label}
      </InputLabel>
      <Select
        {...selectProps}
        name={field.name}
        value={field.value ?? (multiple ? [] : "")}
        labelId={`${field.name}-label`}
        multiple={multiple}
        renderValue={renderValue}
        displayEmpty
        onBlur={handleBlur}
        onChange={handleChange}
        inputRef={field.ref}
        MenuProps={{
          PaperProps: {
            sx: {
              maxHeight: 300,
              "& .MuiMenuItem-root": {
                display: "flex",
                alignItems: "center",
                gap: 1,
              },
            },
          },
        }}
        sx={{
          textAlign: "left",
          "& .MuiSelect-select": {
            display: "flex",
            alignItems: "center",
            gap: 1,
          },
          ...selectProps.sx,
        }}
      >
        {placeholder && !multiple && (
          <MenuItem value="">
            <em>{placeholder}</em>
          </MenuItem>
        )}

        {items.map((item) => (
          <MenuItem
            key={item.value}
            value={item.value}
            disabled={item.disabled}
            sx={{
              opacity: item.disabled ? 0.5 : 1,
              display: "flex",
              alignItems: "center",
              gap: 1,
            }}
          >
            {item.icon && (
              <Box sx={{ display: "flex", alignItems: "center" }}>
                {item.icon}
              </Box>
            )}
            {item.text}
          </MenuItem>
        ))}
      </Select>
      {fieldState.error && (
        <FormHelperText error>{fieldState.error.message}</FormHelperText>
      )}
    </FormControl>
  );
}
