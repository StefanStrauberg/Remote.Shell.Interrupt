import { Box, Container, LinearProgress } from "@mui/material";
import NavBar from "./NavBar";
import "./styles.css";
import { Outlet, useLocation } from "react-router";
import { useIsBusy } from "../../lib/stores/uiStore";
import { routes } from "../router/paths";

/** Routes rendered without the application chrome (landing + auth screens). */
const bareRoutes = new Set([routes.home, routes.login, routes.register]);

function App() {
  const location = useLocation();
  const isBusy = useIsBusy();
  const isBareRoute = bareRoutes.has(location.pathname);

  return (
    <Box sx={{ bgcolor: "background.default", minHeight: "100vh" }}>
      {isBusy && (
        <LinearProgress
          color="secondary"
          sx={{ position: "fixed", top: 0, left: 0, right: 0, zIndex: 1301 }}
        />
      )}
      {isBareRoute ? (
        <Outlet />
      ) : (
        <>
          <NavBar />
          <Container maxWidth="xl" sx={{ py: { xs: 2.5, md: 4 } }}>
            <Outlet />
          </Container>
        </>
      )}
    </Box>
  );
}

export default App;
