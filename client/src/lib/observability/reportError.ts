import { API_BASE_URL, apiPath } from "@/config/api.config";

export type ErrorContext = Readonly<Record<string, unknown>>;

const REPORT_URL = `${API_BASE_URL ?? ""}${apiPath("Diagnostics", "ReportClientError")}`;

/**
 * Single integration point for reporting unhandled application errors. Console output remains
 * the local fallback, and the error is also best-effort forwarded to the backend's own
 * structured logs (see DiagnosticsController) - this self-hosted app's actual production
 * monitoring, not a third-party SaaS this deployment has no account with.
 *
 * Uses a bare fetch rather than the shared httpClient: this runs from error boundaries after
 * something has already gone wrong, so it must never throw back into the caller or depend on
 * app state (axios interceptors, Zustand stores) that may itself be implicated in the crash.
 */
export function reportError(error: unknown, context?: ErrorContext): void {
  console.error("Unhandled application error", error, context);

  const message = error instanceof Error ? error.message : String(error);
  const stack = error instanceof Error ? (error.stack ?? null) : null;

  void fetch(REPORT_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      message,
      stack,
      url: typeof location === "undefined" ? null : location.href,
      userAgent: typeof navigator === "undefined" ? null : navigator.userAgent,
      context: context ? JSON.stringify(context) : null,
    }),
  }).catch(() => {
    // Best-effort only - the console.error above already covers this locally,
    // and retrying would risk looping if the network itself is the problem.
  });
}
