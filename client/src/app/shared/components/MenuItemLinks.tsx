import { NavLink, useLocation } from "react-router";
import { MenuItem, Typography, Badge, Tooltip, Box } from "@mui/material";
import { ReactNode } from "react";

interface MenuItemLinksProps {
  to: string;
  children: ReactNode;
  badgeCount?: number;
  tooltip?: string;
  exact?: boolean;
}

export default function MenuItemLinks({
  to,
  children,
  badgeCount = 0,
  tooltip,
  exact = false,
}: MenuItemLinksProps) {
  const location = useLocation();

  // Check if the menu item is active
  const isActive = exact
    ? location.pathname === to
    : location.pathname.startsWith(to);

  return (
    <Tooltip
      title={tooltip}
      arrow
      placement="right"
      disableHoverListener={!tooltip}
    >
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
          overflow: "hidden",
          "&:hover": {
            backgroundColor: "rgba(255, 255, 255, 0.1)",
            transform: "translateY(-2px)",
            "&::before": {
              width: "70%",
            },
          },
          "&.active": {
            backgroundColor: "rgba(255, 255, 255, 0.15)",
            fontWeight: "bold",
            "&::before": {
              width: "80%",
            },
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
            borderRadius: "1px",
          },
          "&:focus-visible": {
            outline: "2px solid #f1faee",
            outlineOffset: "2px",
          },
          // Ripple effect
          "&::after": {
            content: '""',
            position: "absolute",
            top: "50%",
            left: "50%",
            width: "0",
            height: "0",
            borderRadius: "50%",
            backgroundColor: "rgba(255, 255, 255, 0.2)",
            transform: "translate(-50%, -50%)",
            transition: "width 0.3s ease, height 0.3s ease",
          },
          "&:active::after": {
            width: "200px",
            height: "200px",
          },
        }}
        className={isActive ? "active" : ""}
      >
        <Typography
          variant="body1"
          sx={{
            fontWeight: "inherit",
            display: "flex",
            alignItems: "center",
            gap: 1,
            position: "relative",
            zIndex: 1, // Ensure text is above the ripple effect
          }}
        >
          {children}
          {badgeCount > 0 && (
            <Badge
              badgeContent={badgeCount}
              color="error"
              max={99}
              sx={{
                "& .MuiBadge-badge": {
                  fontSize: "0.7rem",
                  height: "18px",
                  minWidth: "18px",
                  borderRadius: "9px",
                  transform: "scale(0.9) translate(50%, -50%)",
                },
              }}
            />
          )}
        </Typography>

        {/* Active indicator dot for mobile */}
        {isActive && (
          <Box
            sx={{
              display: { xs: "block", md: "none" },
              position: "absolute",
              top: "50%",
              right: "8px",
              transform: "translateY(-50%)",
              width: "8px",
              height: "8px",
              borderRadius: "50%",
              backgroundColor: "#f1faee",
            }}
          />
        )}
      </MenuItem>
    </Tooltip>
  );
}
