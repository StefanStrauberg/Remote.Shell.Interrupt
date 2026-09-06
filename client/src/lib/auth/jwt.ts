/**
 * Shape of the JWT payload issued by the backend
 * (sub = user id, email, role = "role" claim, exp = unix seconds).
 */
export type JwtPayload = {
  sub?: string;
  email?: string;
  role?: string | string[];
  exp?: number;
};

/** Decodes a JWT payload segment without external dependencies. */
export function decodeJwtPayload(token: string): JwtPayload | null {
  try {
    const base64 = token
      .split(".")[1]
      .replace(/-/g, "+")
      .replace(/_/g, "/");

    const binary = atob(base64);
    const bytes = Uint8Array.from(binary, (char) => char.charCodeAt(0));

    return JSON.parse(new TextDecoder().decode(bytes)) as JwtPayload;
  } catch {
    return null;
  }
}

/** True when the token carries an exp claim that already passed (with leeway). */
export function isTokenExpired(token: string, leewaySeconds = 30): boolean {
  const payload = decodeJwtPayload(token);

  if (!payload?.exp) return false;

  return payload.exp * 1000 <= Date.now() + leewaySeconds * 1000;
}

/** Normalizes the "role" claim (string or array) into a role list. */
export function rolesFromJwtPayload(payload: JwtPayload): string[] {
  if (Array.isArray(payload.role)) return payload.role;
  if (typeof payload.role === "string") return [payload.role];
  return [];
}
