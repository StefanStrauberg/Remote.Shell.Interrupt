import { Component, ErrorInfo, ReactNode } from "react";
import { ErrorOutline, Refresh } from "@mui/icons-material";
import { Button } from "@mui/material";
import { reportError } from "@/lib/observability/reportError";
import StatusPage from "./shared/components/StatusPage";

type Props = { children: ReactNode };
type State = { hasError: boolean };

export default class ApplicationErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false };

  static getDerivedStateFromError(): State {
    return { hasError: true };
  }

  componentDidCatch(error: Error, info: ErrorInfo): void {
    reportError(error, {
      boundary: "application",
      componentStack: info.componentStack,
    });
  }

  render() {
    if (!this.state.hasError) return this.props.children;

    return (
      <StatusPage
        fullScreen
        tone="error"
        icon={<ErrorOutline />}
        title="Something went wrong"
        description="The application could not render this page. Reload it to try again."
        actions={
          <Button
            variant="contained"
            startIcon={<Refresh />}
            onClick={() => window.location.reload()}
          >
            Reload application
          </Button>
        }
      />
    );
  }
}
