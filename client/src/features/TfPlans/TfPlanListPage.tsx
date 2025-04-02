import { useState } from "react";
import { useTfPlans } from "../../lib/hooks/useTfPlans";
import { Box, Button, Pagination, Paper, Typography } from "@mui/material";
import { Link } from "react-router";
import TfPlanCard from "./TfPlanCard";

export default function TfPlanListPage() {
  // Manage local state for pagination
  const [pageNumber, setPageNumber] = useState(1); // TablePagination uses zero-based index
  const pageSize = 10; // Default page size

  // Hook for fetching data
  const { tfPlans, pagination, isPending } = useTfPlans(pageNumber, pageSize);

  // Loading state
  if (!tfPlans || isPending) return <Typography>Loading ...</Typography>;

  // No tfPlans state
  if (tfPlans.length === 0)
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
          Update TfPlans
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
        <Button variant="contained" color="error">
          Update all TfPlans
        </Button>
      </Box>

      {/* Render tfPlans */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(2, 1fr)", // Two columns
          gap: 3,
        }}
      >
        {tfPlans.map((tfPlan) => (
          <TfPlanCard key={tfPlan.id} tfPlan={tfPlan} />
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
