import { z } from "zod";

export const editUserProfileSchema = z.object({
  email: z
    .string()
    .trim()
    .nonempty("Email is required")
    .email("Enter a valid email address"),
  fullName: z.string().trim().optional(),
});

export type EditUserProfileValues = z.infer<typeof editUserProfileSchema>;
