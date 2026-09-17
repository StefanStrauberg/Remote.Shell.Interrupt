import { ContentCopy, Check } from "@mui/icons-material";
import { IconButton, Tooltip } from "@mui/material";
import { useState } from "react";
import { toast } from "react-toastify";

export default function CopyValue({
  value,
  label = value,
}: {
  value: string;
  label?: string;
}) {
  const [copied, setCopied] = useState(false);
  return (
    <Tooltip title={copied ? "Copied" : `Copy ${label}`}>
      <IconButton
        size="small"
        aria-label={`Copy ${label}`}
        onBlur={() => setCopied(false)}
        onClick={async () => {
          try {
            await navigator.clipboard.writeText(value);
            setCopied(true);
          } catch {
            toast.error(
              "Could not copy. Select the value and copy it manually."
            );
          }
        }}
      >
        {copied ? (
          <Check fontSize="inherit" color="success" />
        ) : (
          <ContentCopy fontSize="inherit" />
        )}
      </IconButton>
    </Tooltip>
  );
}
