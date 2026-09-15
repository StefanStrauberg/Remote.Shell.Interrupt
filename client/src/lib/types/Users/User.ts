import { ADMIN_ROLE, USER_ROLE } from "../Auth/AuthUser";

export const ASSIGNABLE_ROLES = [ADMIN_ROLE, USER_ROLE] as const;

export type User = {
  id: string;
  email: string;
  fullName?: string;
  roles: string[];
  isActive: boolean;
  createdAtUtc: string;
};
