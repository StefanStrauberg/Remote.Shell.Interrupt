import { Box, Container, LinearProgress } from "@mui/material";
import NavBar, { sidebarWidth } from "./NavBar";
import "./styles.css";
import { Outlet, useLocation } from "react-router";
import { useIsBusy } from "../../lib/stores/uiStore";
import { routes } from "../router/paths";

const bareRoutes = new Set([routes.home, routes.login, routes.register]);

export default function App() {
  const location = useLocation();
  const isBusy = useIsBusy();
  return (
    <Box sx={{ minHeight: "100vh" }}>
      <a className="skip-link" href="#main-content">
        Skip to content
      </a>
      {isBusy && (
        <LinearProgress
          color="secondary"
          sx={{ position: "fixed", top: 0, left: 0, right: 0, zIndex: 1301 }}
        />
      )}
      {bareRoutes.has(location.pathname) ? (
        <Box id="main-content" tabIndex={-1}>
          <Outlet />
        </Box>
      ) : (
        <>
          <NavBar />
          <Box
            component="main"
            id="main-content"
            tabIndex={-1}
            sx={{ ml: { md: `${sidebarWidth}px` }, minWidth: 0 }}
          >
            <Container
              maxWidth={false}
              sx={{ py: { xs: 2, md: 3 }, px: { xs: 2, md: 3 } }}
            >
              <Outlet />
            </Container>
          </Box>
        </>
      )}
    </Box>
  );
}
