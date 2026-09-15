export const ADMIN_ROLE = "Admin";
export const USER_ROLE = "User";

/** Authenticated user profile stored in the global auth state. */
export type AuthUser = {
  id: string;
  email: string;
  roles: string[];
};

/**
 * Returns true when the user satisfies the required roles.
 * An empty/undefined requirement list means "any authenticated user".
 */
export function hasAnyRole(user: AuthUser | null, roles?: string[]): boolean {
  if (!roles || roles.length === 0) return true;
  if (!user) return false;
  return user.roles.some((role) => roles.includes(role));
}
