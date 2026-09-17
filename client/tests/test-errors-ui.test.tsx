// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import TestErrors from "../src/features/Errors/TestErrors";
import httpClient from "../src/lib/api/httpClient";
import { ApiError } from "../src/lib/api/ApiError";
import { renderApp } from "./renderApp";

describe("TestErrors debug page", () => {
  it("issues a GET for the not-found trigger", async () => {
    const get = vi.spyOn(httpClient, "get").mockResolvedValue({ data: null });
    renderApp(<TestErrors />);
    await userEvent.click(screen.getByRole("button", { name: "Not found" }));
    await waitFor(() =>
      expect(get).toHaveBeenCalledWith("/api/v1/Buggy/GetNotFound")
    );
  });

  it("issues a POST for the validation-error trigger", async () => {
    const post = vi.spyOn(httpClient, "post").mockResolvedValue({ data: null });
    renderApp(<TestErrors />);
    await userEvent.click(
      screen.getByRole("button", { name: "Validation error" })
    );
    await waitFor(() =>
      expect(post).toHaveBeenCalledWith("/api/v1/Gates/CreateGate", {})
    );
  });

  it("lists validation error messages returned by the API", async () => {
    vi.spyOn(httpClient, "post").mockRejectedValue(
      new ApiError({
        message: "Validation failed",
        status: 400,
        validationErrors: ["Name is required", "IpAddress is invalid"],
      })
    );
    renderApp(<TestErrors />);
    await userEvent.click(
      screen.getByRole("button", { name: "Validation error" })
    );
    expect(await screen.findByText("Name is required")).toBeVisible();
    expect(screen.getByText("IpAddress is invalid")).toBeVisible();
  });

  it("shows nothing extra for a non-validation failure", async () => {
    vi.spyOn(httpClient, "get").mockRejectedValue(new Error("boom"));
    renderApp(<TestErrors />);
    await userEvent.click(screen.getByRole("button", { name: "Server error" }));
    await waitFor(() =>
      expect(httpClient.get).toHaveBeenCalledWith(
        "/api/v1/Buggy/GetServerError"
      )
    );
    expect(screen.queryByRole("alert")).not.toBeInTheDocument();
  });
});
