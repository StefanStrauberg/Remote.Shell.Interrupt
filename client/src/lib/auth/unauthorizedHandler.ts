type UnauthorizedHandler = () => void | Promise<void>;

let unauthorizedHandler: UnauthorizedHandler | undefined;

/** Connects infrastructure-level 401 responses to the application shell. */
export function registerUnauthorizedHandler(
  handler: UnauthorizedHandler
): void {
  unauthorizedHandler = handler;
}

/** Keeps the HTTP transport independent from React Router and page paths. */
export async function notifyUnauthorized(): Promise<void> {
  await unauthorizedHandler?.();
}
