import { describe, expect, it, vi } from "vitest";
import { loginSchema, registerSchema } from "../src/lib/schemas/AuthSchema";
import { gateSchema } from "../src/lib/schemas/GateSchema";
import { shouldRetryQuery } from "../src/lib/api/retryPolicy";
import { ApiError } from "../src/lib/api/ApiError";
import { getAuthErrorMessage } from "../src/lib/auth/authApi";
import { gatesApi } from "../src/features/Gates/api/gatesApi";
import { Gate } from "../src/lib/types/Gates/Gate";
import { PagedResponse } from "../src/lib/api/common/paged";

const registration = {
  email: "user@example.com",
  password: "Strong-pass1!",
  confirmPassword: "Strong-pass1!",
  role: "User",
};
describe("authentication validation", () => {
  it("trims email without trimming a user's password", () => {
    expect(
      loginSchema.parse({ email: " user@example.com ", password: " password " })
    ).toEqual({ email: "user@example.com", password: " password " });
  });
  it.each([
    "short1!A",
    "alllowercase1!",
    "ALLUPPERCASE1!",
    "NoDigitsHere!",
    "NoSymbols123",
  ])("rejects registration password %s", (password) => {
    expect(
      registerSchema.safeParse({
        ...registration,
        password,
        confirmPassword: password,
      }).success
    ).toBe(false);
  });
  it.each(["User", "Admin"])("accepts a valid %s registration", (role) => {
    expect(registerSchema.safeParse({ ...registration, role }).success).toBe(
      true
    );
  });
  it("attaches password mismatch to the confirmation field", () => {
    const result = registerSchema.safeParse({
      ...registration,
      confirmPassword: "Different-pass2!",
    });
    expect(result.success).toBe(false);
    if (!result.success)
      expect(result.error.flatten().fieldErrors.confirmPassword).toContain(
        "Passwords do not match"
      );
  });
  it("rejects unknown roles", () => {
    expect(
      registerSchema.safeParse({ ...registration, role: "Owner" }).success
    ).toBe(false);
  });
  it.each([
    [
      new ApiError({ message: "Unauthorized", status: 401 }),
      "Invalid email or password.",
    ],
    [
      new ApiError({ message: "Forbidden", status: 403 }),
      "Your account is not allowed to perform this action.",
    ],
    [["Missing email", "Missing password"], "Missing email; Missing password"],
    [null, "Authentication failed. Please try again."],
  ])("maps auth failures to a readable message", (error, message) => {
    expect(getAuthErrorMessage(error)).toBe(message);
  });
});

describe("gate validation", () => {
  const gate = {
    name: " core ",
    ipAddress: " 192.0.2.1 ",
    community: "public",
    typeOfNetworkDevice: "Cisco",
  };
  it("normalizes name and IP address", () => {
    expect(gateSchema.parse(gate)).toMatchObject({
      name: "core",
      ipAddress: "192.0.2.1",
    });
  });
  it.each(["256.0.0.1", "192.168.1", "host.example.com", "::1", "1.2.3.4.5"])(
    "rejects invalid IPv4 address %s",
    (ipAddress) => {
      expect(gateSchema.safeParse({ ...gate, ipAddress }).success).toBe(false);
    }
  );
});

describe("query retry policy", () => {
  it("retries network errors at most twice", () => {
    const error = new ApiError({ message: "Offline", isNetworkError: true });
    expect(shouldRetryQuery(0, error)).toBe(true);
    expect(shouldRetryQuery(1, error)).toBe(true);
    expect(shouldRetryQuery(2, error)).toBe(false);
  });
  it.each([400, 401, 403, 404, 422, 500])(
    "does not retry HTTP %s responses",
    (status) => {
      expect(
        shouldRetryQuery(0, new ApiError({ message: "HTTP failure", status }))
      ).toBe(false);
    }
  );
  it("does not retry programming errors", () => {
    expect(shouldRetryQuery(0, new TypeError("Bad data"))).toBe(false);
  });
});

function gatePage(
  ids: string[],
  currentPage: number,
  totalPages: number
): PagedResponse<Gate> {
  return {
    data: ids.map((id) => ({
      id,
      name: id,
      ipAddress: "192.0.2.1",
      community: "test",
      typeOfNetworkDevice: "Cisco",
    })),
    pagination: {
      CurrentPage: currentPage,
      TotalPages: totalPages,
      TotalCount: 3,
      PageSize: 50,
      HasNext: currentPage < totalPages,
      HasPrevious: currentPage > 1,
    },
  };
}
describe("bulk gate deletion safety", () => {
  it("snapshots every page before deleting and deduplicates IDs", async () => {
    const operations: string[] = [];
    vi.spyOn(gatesApi, "list").mockImplementation(async ({ pagination }) => {
      operations.push(`read:${pagination.pageNumber}`);
      return pagination.pageNumber === 1
        ? gatePage(["one", "two"], 1, 2)
        : gatePage(["two", "three"], 2, 2);
    });
    vi.spyOn(gatesApi, "remove").mockImplementation(async (id) => {
      operations.push(`delete:${id}`);
    });
    expect(await gatesApi.removeAll()).toBe(3);
    expect(operations).toEqual([
      "read:1",
      "read:2",
      "delete:one",
      "delete:two",
      "delete:three",
    ]);
  });
  it("deletes nothing when a later page cannot be read", async () => {
    vi.spyOn(gatesApi, "list")
      .mockResolvedValueOnce(gatePage(["one"], 1, 2))
      .mockRejectedValueOnce(new Error("Read failed"));
    const remove = vi.spyOn(gatesApi, "remove").mockResolvedValue();
    await expect(gatesApi.removeAll()).rejects.toThrow("Read failed");
    expect(remove).not.toHaveBeenCalled();
  });
  it("stops on a delete failure instead of silently reporting success", async () => {
    vi.spyOn(gatesApi, "list").mockResolvedValue(
      gatePage(["one", "two"], 1, 1)
    );
    const remove = vi
      .spyOn(gatesApi, "remove")
      .mockRejectedValue(new Error("Forbidden"));
    await expect(gatesApi.removeAll()).rejects.toThrow("Forbidden");
    expect(remove).toHaveBeenCalledExactlyOnceWith("one");
  });
});
