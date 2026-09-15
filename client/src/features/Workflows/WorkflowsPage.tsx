import { useState } from "react";
import { Link } from "react-router";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  Grid2,
  Pagination,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import AddIcon from "@mui/icons-material/Add";
import PageHeader from "@/app/shared/components/PageHeader";
import { routes } from "@/app/router/paths";
import { useWorkflows } from "./api/workflowsQueries";
export default function WorkflowsPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const query = useWorkflows(page, search);
  return (
    <Box>
      <Button component={Link} to={routes.admin} sx={{ mb: 2 }}>
        ← Administration
      </Button>
      <PageHeader
        title="Workflows"
        description="Design device discovery, transform SNMP data, and run workflows."
        icon={AccountTreeIcon}
        action={
          <Button
            component={Link}
            to={routes.createWorkflow}
            variant="contained"
            startIcon={<AddIcon />}
          >
            New workflow
          </Button>
        }
      />
      <TextField
        label="Search workflows"
        value={search}
        onChange={(e) => {
          setSearch(e.target.value);
          setPage(1);
        }}
        sx={{ mb: 3, width: { xs: "100%", sm: 360 } }}
      />
      {query.isPending && <CircularProgress aria-label="Loading workflows" />}
      {query.isError && (
        <Alert
          severity="error"
          action={<Button onClick={() => query.refetch()}>Retry</Button>}
        >
          {query.error.message}
        </Alert>
      )}
      {query.data && (
        <>
          {!query.data.data.length && (
            <Alert severity="info">
              No workflows found. Create a workflow or change your search.
            </Alert>
          )}
          <Grid2 container spacing={2}>
            {query.data.data.map((workflow) => (
              <Grid2 key={workflow.id} size={{ xs: 12, md: 6, lg: 4 }}>
                <Card variant="outlined" sx={{ height: "100%" }}>
                  <CardContent>
                    <Stack
                      direction="row"
                      justifyContent="space-between"
                      alignItems="center"
                      mb={2}
                    >
                      <AccountTreeIcon color="primary" />
                      <Chip
                        size="small"
                        label={workflow.status}
                        color={
                          workflow.status === "Published"
                            ? "success"
                            : workflow.status === "Draft"
                              ? "info"
                              : "default"
                        }
                      />
                    </Stack>
                    <Typography variant="h6" sx={{ overflowWrap: "anywhere" }}>
                      {workflow.name}
                    </Typography>
                    <Typography
                      color="text.secondary"
                      variant="body2"
                      mt={1}
                      mb={2}
                    >
                      Version {workflow.version} · {workflow.nodeCount} nodes ·{" "}
                      {workflow.edgeCount} edges
                    </Typography>
                    <Button component={Link} to={routes.workflow(workflow.id)}>
                      Open workflow →
                    </Button>
                  </CardContent>
                </Card>
              </Grid2>
            ))}
          </Grid2>
          {query.data.pagination.TotalPages > 1 && (
            <Pagination
              sx={{ mt: 3 }}
              count={query.data.pagination.TotalPages}
              page={page}
              onChange={(_, value) => setPage(value)}
            />
          )}
        </>
      )}
    </Box>
  );
}
