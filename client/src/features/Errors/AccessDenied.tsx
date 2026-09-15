import { GppBad } from "@mui/icons-material";
import { Button } from "@mui/material";
import { Link } from "react-router";
import { useLocation } from "react-router";
import { routes } from "../../app/router/paths";
import StatusPage from "../../app/shared/components/StatusPage";

export default function AccessDenied() {
  const location = useLocation();
  const attemptedPath = (location.state as { from?: string } | null)?.from;

  return (
    <StatusPage
      icon={<GppBad />}
      tone="error"
      title="Access denied"
      description={
        <>
          Your account cannot access this page
          {attemptedPath ? ` (${attemptedPath})` : ""}. Contact an administrator
          if you believe this is a mistake.
        </>
      }
      actions={
        <>
          <Button variant="contained" component={Link} to={routes.main}>
            Back to dashboard
          </Button>
          <Button variant="outlined" component={Link} to={routes.home}>
            Home
          </Button>
        </>
      }
    />
  );
}
