import { z } from "zod";

export const gateSchema = z.object({
  name: z.string().nonempty("Name"),
  ipAddress: z
    .string()
    .nonempty("IP Address is required")
    .regex(
      /^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$/,
      "Invalid IP address format"
    ),
  community: z.string().nonempty("Community"),
  typeOfNetworkDevice: z.string().nonempty("Type Of NetworkDevice"),
});

export type GateSchema = z.infer<typeof gateSchema>;
