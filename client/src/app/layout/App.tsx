import { Box, Container, CssBaseline, LinearProgress } from "@mui/material";
import NavBar from "./NavBar";
import "./styles.css";
import { Outlet, useLocation } from "react-router";
import { useIsBusy } from "../../lib/stores/uiStore";

/** Routes rendered without the application chrome (landing + auth screens). */
const bareRoutes = ["/", "/login", "/register"];

function App() {
  const location = useLocation();
  const isBusy = useIsBusy();
  const isBareRoute = bareRoutes.includes(location.pathname);

  return (
    <Box sx={{ bgcolor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
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
          <Container maxWidth="xl" sx={{ mt: 3 }}>
            <Outlet />
          </Container>
        </>
      )}
    </Box>
  );
}

export default App;
