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
        color: "#f1faee",
        textTransform: "capitalize",
        textDecoration: "none",
        padding: "0.5rem 1rem",
        borderRadius: "8px", // Rounded corners
        transition: "background-color 0.3s ease, color 0.3s ease", // Smooth transition effect
        "&:hover": {
          backgroundColor: "rgba(255, 255, 255, 0.1)", // Hover background
          color: "#a8dadc",
        },
        "&.active": {
          backgroundColor: "#ffca3a", // Bold yellow background for active link
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
