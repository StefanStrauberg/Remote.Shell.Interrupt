import { Box, Container, CssBaseline, LinearProgress } from "@mui/material";
import NavBar from "./NavBar";
import "./styles.css";
import { Outlet, useLocation } from "react-router";
import HomePage from "../../features/home/HomePage";
import { useIsBusy } from "../../lib/stores/uiStore";

function App() {
  const location = useLocation();
  const isBusy = useIsBusy();

  return (
    <Box sx={{ bgcolor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
      {isBusy && (
        <LinearProgress
          color="secondary"
          sx={{ position: "fixed", top: 0, left: 0, right: 0, zIndex: 1301 }}
        />
      )}
      {location.pathname === "/" ? (
        <HomePage />
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
