import { describe, expect, it } from "vitest";
import {
  decodeJwtPayload,
  isTokenExpired,
  rolesFromJwtPayload,
} from "../src/lib/auth/jwt";
import { formatApiDate, isGuid, isValidVlanId } from "../src/lib/utils";

function tokenWithPayload(payload: object): string {
  const encoded = btoa(JSON.stringify(payload))
    .replace(/=/g, "")
    .replace(/\+/g, "-")
    .replace(/\//g, "_");
  return `header.${encoded}.signature`;
}

describe("JWT session helpers", () => {
  it("decodes URL-safe payloads and normalizes roles", () => {
    const token = tokenWithPayload({
      sub: "user-id",
      role: ["Admin", "User"],
      exp: Math.floor(Date.now() / 1000) + 120,
    });
    const payload = decodeJwtPayload(token);

    expect(payload?.sub).toBe("user-id");
    expect(rolesFromJwtPayload(payload!)).toEqual(["Admin", "User"]);
    expect(isTokenExpired(token)).toBe(false);
  });

  it("rejects malformed, non-expiring, and expired tokens", () => {
    expect(isTokenExpired("not-a-jwt")).toBe(true);
    expect(isTokenExpired(tokenWithPayload({ sub: "user-id" }))).toBe(true);
    expect(
      isTokenExpired(
        tokenWithPayload({ exp: Math.floor(Date.now() / 1000) - 60 })
      )
    ).toBe(true);
  });
});

describe("input and API formatting helpers", () => {
  it("accepts only valid VLAN tags", () => {
    expect(isValidVlanId(1)).toBe(true);
    expect(isValidVlanId(4094)).toBe(true);
    expect(isValidVlanId(0)).toBe(false);
    expect(isValidVlanId(4095)).toBe(false);
    expect(isValidVlanId(1.5)).toBe(false);
  });

  it("recognizes the GUID shape accepted by ASP.NET routes", () => {
    expect(isGuid("4f9f2cab-842a-4cba-a72f-7daab5e19ef1")).toBe(true);
    expect(isGuid("------------------------------------")).toBe(false);
    expect(isGuid("4f9f2cab-842a-4cba-a72f")).toBe(false);
  });

  it("hides invalid and .NET minimum dates", () => {
    expect(formatApiDate(undefined)).toBeNull();
    expect(formatApiDate("not-a-date")).toBeNull();
    expect(formatApiDate("0001-01-01T00:00:00Z")).toBeNull();
    expect(formatApiDate("2026-09-15T00:00:00Z")).not.toBeNull();
  });
});
