import { NavLink } from "react-router";
import { MenuItem, Typography } from "@mui/material";
import { ReactNode } from "react";

interface MenuItemLinksProps {
  to: string;
  children: ReactNode;
}

export default function MenuItemLinks({ to, children }: MenuItemLinksProps) {
  return (
    <MenuItem
      component={NavLink}
      to={to}
      sx={{
        color: "#f1faee",
        textDecoration: "none",
        borderRadius: "8px",
        margin: "0 0.25rem",
        transition: "all 0.3s ease",
        position: "relative",
        "&:hover": {
          backgroundColor: "rgba(255, 255, 255, 0.1)",
          transform: "translateY(-2px)",
        },
        "&.active": {
          backgroundColor: "rgba(255, 255, 255, 0.15)",
          fontWeight: "bold",
        },
        "&::before": {
          content: '""',
          position: "absolute",
          bottom: 0,
          left: "50%",
          transform: "translateX(-50%)",
          width: 0,
          height: "2px",
          backgroundColor: "#f1faee",
          transition: "width 0.3s ease",
        },
        "&:hover::before": {
          width: "70%",
        },
        "&.active::before": {
          width: "80%",
        },
      }}
    >
      <Typography
        variant="body1"
        sx={{
          fontWeight: "inherit",
          display: "flex",
          alignItems: "center",
        }}
      >
        {children}
      </Typography>
    </MenuItem>
  );
}
