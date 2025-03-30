import { Box, Button, Paper, Typography } from "@mui/material";
import GateCard from "./GateCard";
import { useGates } from "../../lib/hooks/useGates";
import { Link } from "react-router";

export default function GateListPage() {
  const { gates, isPending } = useGates();

  if (!gates || isPending) return <Typography>Loading ...</Typography>;

  if (gates.length === 0)
    return (
      <Paper
        sx={{
          color: "white",
          display: "flex",
          flexDirection: "column",
          gap: 3,
          alignItems: "center",
          alignContent: "center",
          justifyContent: "center",
          height: "90vh",
          backgroundImage:
            "linear-gradient(35deg, #182a73 0%, #218aae 69%, #20a7ac)",
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            alignContent: "center",
            color: "white",
          }}
        >
          <Typography variant="h3">There is no one gate</Typography>
        </Box>
        <Button
          component={Link}
          to="/createGate"
          variant="contained"
          sx={{ height: 60, borderRadius: 2, fontSize: "1.5rem" }}
        >
          Create new one
        </Button>
      </Paper>
    );

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Box alignSelf="end" mr={3}>
        <Button
          variant="contained"
          color="success"
          component={Link}
          to="/createGate"
        >
          Create new gate
        </Button>
      </Box>
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          gap: 3,
        }}
      >
        {gates.map((gate) => (
          <GateCard key={gate.id} gate={gate} />
        ))}
      </Box>
    </Box>
  );
}
