import { SearchOff } from "@mui/icons-material";
import { Button } from "@mui/material";
import { Link } from "react-router";
import { routes } from "../../app/router/paths";
import StatusPage from "../../app/shared/components/StatusPage";

export default function NotFound() {
  return (
    <StatusPage
      icon={<SearchOff />}
      title="Page not found"
      description="The requested page may have moved, or the address may be incorrect."
      actions={
        <Button variant="contained" component={Link} to={routes.networkDevices}>
          View network devices
        </Button>
      }
    />
  );
}
