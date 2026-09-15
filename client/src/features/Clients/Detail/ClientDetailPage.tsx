import { ArrowBack, Person } from "@mui/icons-material";
import { Box, Button, Card, CardContent, CardHeader } from "@mui/material";
import { Link, useParams } from "react-router";
import { useClientQuery } from "../api/clientsQueries";
import { routes } from "../../../app/router/paths";
import PageHeader from "../../../app/shared/components/PageHeader";
import {
  PageError,
  PageLoading,
} from "../../../app/shared/components/PageFeedback";
import EmptyPage from "../../../app/shared/components/EmptyPage";
import ClientDetailsSections from "./ClientDetailsSections";

export default function ClientDetailPage() {
  const { id } = useParams();
  const clientQuery = useClientQuery(id);
  const clientById = clientQuery.data;

  if (clientQuery.isLoading) return <PageLoading message="Loading client…" />;

  if (clientQuery.isError)
    return (
      <PageError title="Unable to load client" error={clientQuery.error} />
    );

  if (!clientById) return <EmptyPage input="Client not found" />;

  return (
    <Box>
      <PageHeader
        title="Client details"
        description="Customer profile, contacts, services, and history"
        icon={Person}
        action={
          <Button
            variant="outlined"
            startIcon={<ArrowBack />}
            component={Link}
            to={routes.clients}
          >
            Back to clients
          </Button>
        }
      />
      <Card>
        <CardHeader title={clientById.name} />
        <CardContent sx={{ p: 0 }}>
          <ClientDetailsSections client={clientById} />
        </CardContent>
      </Card>
    </Box>
  );
}
