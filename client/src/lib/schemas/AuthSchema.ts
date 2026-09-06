import { z } from "zod";

export const authRoles = ["Admin", "User"] as const;
export type AuthRole = (typeof authRoles)[number];

export const loginSchema = z.object({
  email: z
    .string()
    .nonempty("Email is required")
    .email("Enter a valid email address"),
  password: z.string().nonempty("Password is required")
});

export type LoginValues = z.infer<typeof loginSchema>;

export const registerSchema = z
  .object({
    email: z
      .string()
      .nonempty("Email is required")
      .email("Enter a valid email address"),
    // Mirrors the backend Identity password policy.
    password: z
      .string()
      .nonempty("Password is required")
      .min(10, "Password must be at least 10 characters long")
      .regex(/[A-Z]/, "Password must contain an uppercase letter")
      .regex(/[a-z]/, "Password must contain a lowercase letter")
      .regex(/[0-9]/, "Password must contain a digit")
      .regex(/[^A-Za-z0-9]/, "Password must contain a special character"),
    confirmPassword: z.string().nonempty("Confirm the password"),
    role: z
      .string()
      .nonempty("Role is required")
      .refine(
        (role): role is AuthRole => (authRoles as readonly string[]).includes(role),
        "Role must be Admin or User"
      )
  })
  .refine((values) => values.password === values.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"]
  });

export type RegisterValues = z.infer<typeof registerSchema>;
