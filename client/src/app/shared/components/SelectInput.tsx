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
import { SelectInputProps } from "@mui/material/Select/SelectInput";
import {
  FieldValues,
  useController,
  UseControllerProps,
} from "react-hook-form";
import { ReactNode } from "react";

interface SelectItem {
  text: string;
  value: string;
  disabled?: boolean;
  icon?: ReactNode;
}

type Props<T extends FieldValues> = {
  items: SelectItem[];
  label: string;
  multiple?: boolean;
  showChips?: boolean;
  placeholder?: string;
  size?: "small" | "medium";
} & UseControllerProps<T> &
  Partial<SelectInputProps>;

export default function SelectInput<T extends FieldValues>(props: Props<T>) {
  const {
    items,
    label,
    multiple = false,
    showChips = false,
    placeholder,
    size = "medium",
    ...controllerProps
  } = props;

  const theme = useTheme();

  const { field, fieldState } = useController({ ...controllerProps });

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

    if (!selected) {
      return placeholder || `Select ${label}`;
    }

    const item = items.find((item) => item.value === selected);
    return item?.text || String(selected);
  };

  return (
    <FormControl
      fullWidth
      error={!!fieldState.error}
      size={size}
      disabled={props.disabled}
    >
      <InputLabel
        id={`${field.name}-label`}
        shrink={
          !!field.value ||
          (multiple && Array.isArray(field.value) && field.value.length > 0)
        }
      >
        {label}
      </InputLabel>

      <Select
        {...field}
        labelId={`${field.name}-label`}
        multiple={multiple}
        renderValue={renderValue}
        displayEmpty
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
        }}
        // Pass through any additional Select props
        {...props}
      >
        {placeholder && (
          <MenuItem value="" disabled>
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
