import {
  Button,
  Card,
  CardHeader,
  Collapse,
  IconButton,
  Stack,
} from "@mui/material";
import { Link } from "react-router";
import { routes } from "@/app/router/paths";
import { Client } from "../../../lib/types/Clients/Client";
import { useState } from "react";
import { ExpandLess, ExpandMore } from "@mui/icons-material";
import ClientDetailsSections from "../../Clients/Detail/ClientDetailsSections";

type Props = {
  client: Client;
};

export default function ClientBox({ client }: Props) {
  const [isExpanded, setExpanded] = useState(false);

  const toggleExpand = () => {
    setExpanded(!isExpanded);
  };

  return (
    <Card sx={{ mb: 2, overflow: "hidden" }}>
      <CardHeader
        title={client.name}
        subheader={`${client.telephoneT || "No phone"} · ${client.emailT || "No email"}`}
        action={
          <Stack direction="row" alignItems="center">
            <Button component={Link} to={routes.client(client.id)} size="small">
              Open client
            </Button>
            <IconButton
              onClick={toggleExpand}
              aria-label={isExpanded ? "Collapse client" : "Expand client"}
            >
              {isExpanded ? <ExpandLess /> : <ExpandMore />}
            </IconButton>
          </Stack>
        }
      />

      <Collapse in={isExpanded}>
        <ClientDetailsSections client={client} />
      </Collapse>
    </Card>
  );
}
