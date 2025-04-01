import { Box, Button, Pagination, Paper, Typography } from "@mui/material";
import GateCard from "./GateCard";
import { useGates } from "../../lib/hooks/useGates";
import { Link } from "react-router";
import { useState } from "react";

export default function GateListPage() {
  // Manage local state for pagination
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10; // You can adjust the page size as needed

  // Hook for fetching data
  const { gates, pagination, isPending } = useGates(pageNumber, pageSize);

  // Loading state
  if (!gates || isPending) return <Typography>Loading ...</Typography>;

  // No gates state
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

  // Handle page change
  const handlePageChange = (
    _event: React.ChangeEvent<unknown>,
    value: number
  ) => {
    setPageNumber(value); // Update the page number
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* Button to create a new gate */}
      <Box alignSelf="end" mr={2}>
        <Button
          variant="contained"
          color="success"
          component={Link}
          to="/createGate"
        >
          Create new gate
        </Button>
      </Box>

      {/* Render gates */}
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

      {/* Pagination Component */}
      <Pagination
        count={pagination?.TotalPages || 1} // Total pages based on pagination metadata
        page={pageNumber} // Current active page
        onChange={handlePageChange} // Handle page change
        color="primary"
        sx={{ alignSelf: "center", mt: 2 }}
      />
    </Box>
  );
}
