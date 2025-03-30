import { Box, Container, CssBaseline } from "@mui/material";
import NavBar from "./NavBar";
import "./styles.css";
import { Outlet } from "react-router";
import HomePage from "../../features/home/HomePage";

function App() {
  return (
    <Box sx={{ bgcolor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
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
