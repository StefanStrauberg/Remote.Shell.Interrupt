import { Box, Button, Pagination, Paper, Typography } from "@mui/material";
import GateCard from "./GateCard";
import { useGates } from "../../lib/hooks/useGates";
import { Link } from "react-router";
import { useState } from "react";

export default function GateListPage() {
  // Manage local state for pagination
  const [pageNumber, setPageNumber] = useState(1); // TablePagination uses zero-based index
  const pageSize = 10; // Default page size

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
          gap: 4,
          alignItems: "center",
          alignContent: "center",
          justifyContent: "center",
          height: "90vh",
          backgroundImage: "linear-gradient(135deg, #1d3557, #457b9d, #1d3557)",
          boxShadow: "0 8px 20px rgba(0, 0, 0, 0.2)", // Add depth with shadow
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 3,
            animation: "fadeIn 3s ease", // Smooth fade-in animation
            "@keyframes fadeIn": {
              from: { opacity: 0 },
              to: { opacity: 1 },
            },
          }}
        >
          <Typography
            variant="h3"
            fontWeight="bold"
            sx={{
              fontFamily: "'Poppins', sans-serif",
              textShadow: "2px 2px 6px rgba(0, 0, 0, 0.3)", // Subtle text shadow
            }}
          >
            Маршрутизаторы не найдены
          </Typography>
        </Box>
        <Button
          component={Link}
          to="/createGate"
          size="large"
          variant="contained"
          sx={{
            backgroundColor: "#e5e7eb",
            color: "#1d3557",
            height: 60,
            width: "fit-content",
            padding: "0 2rem",
            borderRadius: "8px",
            fontSize: "1.5rem",
            fontWeight: "bold",
            textTransform: "none",
            boxShadow: "0 4px 12px rgba(0, 0, 0, 0.2)", // Add shadow to button
            transition: "all 0.3s ease",
            "&:hover": {
              backgroundColor: "#ffd166", // Slightly lighter hover effect
              transform: "scale(1.05)", // Subtle scaling on hover
            },
          }}
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
          display: "grid",
          gridTemplateColumns: "repeat(2, 1fr)", // Two columns
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
        variant="outlined"
        color="primary"
        sx={{ alignSelf: "center", mt: 2 }}
      />
    </Box>
  );
}
