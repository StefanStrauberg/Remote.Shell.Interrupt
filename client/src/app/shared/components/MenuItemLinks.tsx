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
        color: "whitesmoke",
        "&.active": {
          color: "yellow",
        },
      }}
    >
      {children}
    </MenuItem>
  );
}
