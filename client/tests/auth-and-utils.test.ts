import { describe, expect, it } from "vitest";
import { formatApiDate, isGuid, isValidVlanId } from "../src/lib/utils";

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
