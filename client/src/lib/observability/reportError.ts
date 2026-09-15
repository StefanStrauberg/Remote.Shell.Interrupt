export type ErrorContext = Readonly<Record<string, unknown>>;

/**
 * Single integration point for Sentry, Application Insights, or another
 * production error reporter. Console output remains as the local fallback.
 */
export function reportError(error: unknown, context?: ErrorContext): void {
  console.error("Unhandled application error", error, context);
}
