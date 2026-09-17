// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { createMemoryRouter, MemoryRouter, RouterProvider } from "react-router";
import { ThemeProvider } from "@mui/material";
import { appTheme } from "../src/app/theme";
import AccessDenied from "../src/features/Errors/AccessDenied";
import NotFound from "../src/features/Errors/NotFound";
import ServerError from "../src/features/Errors/ServerError";
import ApplicationErrorBoundary from "../src/app/ApplicationErrorBoundary";
import RouteErrorPage from "../src/app/router/RouteErrorPage";
import { ApiErrorResponse } from "../src/lib/types/Common/ApiErrorResponse";

function withProviders(
  ui: React.ReactElement,
  entry: string | { pathname: string; state: unknown } = "/"
) {
  return render(
    <ThemeProvider theme={appTheme}>
      <MemoryRouter initialEntries={[entry]}>{ui}</MemoryRouter>
    </ThemeProvider>
  );
}

describe("AccessDenied", () => {
  it("mentions the attempted path when one is provided", () => {
    withProviders(<AccessDenied />, {
      pathname: "/access-denied",
      state: { from: "/gates" },
    });
    expect(screen.getByText(/Access denied/)).toBeVisible();
    expect(screen.getByText(/\/gates/)).toBeVisible();
    expect(
      screen.getByRole("link", { name: "Back to dashboard" })
    ).toHaveAttribute("href", "/mainPage");
    expect(screen.getByRole("link", { name: "Home" })).toHaveAttribute(
      "href",
      "/"
    );
  });

  it("omits the parenthetical when no attempted path is known", () => {
    withProviders(<AccessDenied />);
    expect(
      screen.getByText(
        "Your account cannot access this page. Contact an administrator if you believe this is a mistake."
      )
    ).toBeVisible();
  });
});

describe("NotFound", () => {
  it("offers a link back to network devices", () => {
    withProviders(<NotFound />);
    expect(screen.getByText("Page not found")).toBeVisible();
    expect(
      screen.getByRole("link", { name: "View network devices" })
    ).toHaveAttribute("href", "/networkDevices");
  });
});

describe("ServerError", () => {
  it("falls back to a generic message with no error in navigation state", () => {
    withProviders(<ServerError />);
    expect(screen.getByText("Server error")).toBeVisible();
    expect(
      screen.getByText(
        "The server could not complete the request. Try again later."
      )
    ).toBeVisible();
  });

  it("renders the server-provided title, detail, status and validation errors", () => {
    const error: ApiErrorResponse = {
      Status: 422,
      Title: "Validation failed",
      Detail: "One or more fields are invalid.",
      Errors: { Name: ["is required"], IpAddress: ["is invalid"] },
    };
    withProviders(<ServerError />, {
      pathname: "/server-error",
      state: { error },
    });
    expect(screen.getByText("Validation failed")).toBeVisible();
    expect(screen.getByText("One or more fields are invalid.")).toBeVisible();
    expect(screen.getByText("Status: 422")).toBeVisible();
    expect(
      screen.getByText("Name: is required; IpAddress: is invalid")
    ).toBeVisible();
  });
});

describe("ApplicationErrorBoundary", () => {
  function Bomb(): never {
    throw new Error("render exploded");
  }

  it("renders a fallback instead of crashing, and reloads on demand", () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: true }));
    vi.spyOn(console, "error").mockImplementation(() => {});
    const reload = vi.fn();
    Object.defineProperty(window, "location", {
      value: { ...window.location, reload },
      writable: true,
    });

    render(
      <ThemeProvider theme={appTheme}>
        <ApplicationErrorBoundary>
          <Bomb />
        </ApplicationErrorBoundary>
      </ThemeProvider>
    );

    expect(screen.getByText("Something went wrong")).toBeVisible();
    screen.getByRole("button", { name: "Reload application" }).click();
    expect(reload).toHaveBeenCalledTimes(1);
  });

  it("keeps rendering children normally when nothing throws", () => {
    render(
      <ThemeProvider theme={appTheme}>
        <ApplicationErrorBoundary>
          <div>All good</div>
        </ApplicationErrorBoundary>
      </ThemeProvider>
    );
    expect(screen.getByText("All good")).toBeVisible();
  });
});

describe("RouteErrorPage", () => {
  function renderRouted(entry: string, willThrow: boolean) {
    const router = createMemoryRouter(
      [
        {
          path: "/ok",
          element: willThrow ? <ThrowingRoute /> : <div>fine</div>,
          errorElement: <RouteErrorPage />,
        },
      ],
      { initialEntries: [entry] }
    );
    return render(
      <ThemeProvider theme={appTheme}>
        <RouterProvider router={router} />
      </ThemeProvider>
    );
  }

  function ThrowingRoute(): never {
    throw new Error("loader boom");
  }

  it("shows a generic message for a thrown application error", () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: true }));
    vi.spyOn(console, "error").mockImplementation(() => {});
    renderRouted("/ok", true);
    expect(screen.getByText("Something went wrong")).toBeVisible();
    expect(screen.getByRole("link", { name: "Home" })).toHaveAttribute(
      "href",
      "/"
    );
  });

  it("shows the HTTP status for a routing error, such as an unmatched path", () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: true }));
    vi.spyOn(console, "error").mockImplementation(() => {});
    renderRouted("/does-not-exist", false);
    expect(screen.getByText("Request failed (404)")).toBeVisible();
  });
});
