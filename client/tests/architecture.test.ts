import { describe, expect, it } from "vitest";
import { routes } from "../src/app/router/paths";
import { apiPath } from "../src/config/api.config";
import { ApiError, toApiError } from "../src/lib/api/ApiError";

describe("architecture contracts", () => {
  it("encodes dynamic API and UI path segments", () => {
    expect(apiPath("Gates", "rack/one")).toBe("/api/v1/Gates/rack%2Fone");
    expect(routes.client("customer/42")).toBe("/clients/customer%2F42");
  });

  it("normalizes validation failures into a stable application error", () => {
    const error = toApiError({
      isAxiosError: true,
      response: {
        status: 422,
        data: {
          Errors: {
            Email: ["Email is required."],
            Password: ["Password is too short."],
          },
        },
      },
    });

    expect(error).toBeInstanceOf(ApiError);
    expect(error.status).toBe(422);
    expect(error.validationErrors).toEqual([
      "Email is required.",
      "Password is too short.",
    ]);
    expect(error.message).toBe("Email is required.; Password is too short.");
  });

  it("distinguishes network errors from HTTP responses", () => {
    const error = toApiError({ isAxiosError: true });

    expect(error.isNetworkError).toBe(true);
    expect(error.status).toBeUndefined();
  });
});
