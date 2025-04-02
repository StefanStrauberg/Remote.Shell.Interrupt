import { MenuItem } from "@mui/material";
import { ReactNode } from "react";
import { NavLink } from "react-router";

export default function MenuItemLinks({
  children,
  to,
}: {
  children: ReactNode;
  to: string;
}) {
  return (
    <MenuItem
      component={NavLink}
      to={to}
      sx={{
        backgroundColor: "#e5e7eb",
        color: "#1d3557",
        height: 40,
        borderRadius: "8px",
        textTransform: "none",
        boxShadow: "0 4px 12px rgba(0, 0, 0, 0.2)", // Add shadow to button
        transition: "all 0.3s ease",
        "&:hover": {
          backgroundColor: "#ffd166", // Slightly lighter hover effect
          transform: "scale(1.05)", // Subtle scaling on hover
        },
        "&.active": {
          backgroundColor: "#ffd166", // Bold yellow background for active link
          color: "#1d3557", // Contrasting text color
          fontWeight: "bold",
          boxShadow: "0 4px 10px rgba(0, 0, 0, 0.2)", // Subtle shadow to make it pop
          textDecoration: "none",
        },
      }}
    >
      {children}
    </MenuItem>
  );
}
