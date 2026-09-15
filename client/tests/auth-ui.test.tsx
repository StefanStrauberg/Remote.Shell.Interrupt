// @vitest-environment jsdom
import { beforeEach, describe, expect, it, vi } from "vitest";
import { act, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ReactElement } from "react";
import { Route, Routes } from "react-router";
import ProtectedRoute from "../src/app/router/ProtectedRoute";
import NavBar from "../src/app/layout/NavBar";
import LoginPage from "../src/features/Auth/Login/LoginPage";
import { useAuthStore } from "../src/lib/auth/authStore";
import { authApi } from "../src/lib/auth/authApi";
import { ApiError } from "../src/lib/api/ApiError";
import { deferred, renderApp } from "./renderApp";

const profile = {
  id: "test-user",
  email: "operator@example.com",
  roles: ["User"],
};
beforeEach(() => {
  localStorage.clear();
  useAuthStore.setState({ user: null, status: "unauthenticated" });
});

function renderGuard(element: ReactElement, entry = "/") {
  return renderApp(
    <Routes>
      <Route path={entry.split(/[?#]/)[0]} element={element} />
      <Route path="/login" element={<h1>Login destination</h1>} />
      <Route path="/access-denied" element={<h1>Access denied</h1>} />
    </Routes>,
    entry
  );
}

describe("route access in the rendered application", () => {
  it("waits for session restoration without revealing protected content", () => {
    useAuthStore.setState({ status: "idle" });
    renderGuard(
      <ProtectedRoute>
        <h1>Private content</h1>
      </ProtectedRoute>
    );
    expect(screen.getByRole("progressbar")).toBeInTheDocument();
    expect(screen.queryByText("Private content")).not.toBeInTheDocument();
  });
  it("redirects anonymous visitors and preserves query and fragment", async () => {
    renderGuard(
      <ProtectedRoute>
        <h1>Private content</h1>
      </ProtectedRoute>,
      "/clients?page=2#contact"
    );
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        '/login|{"from":"/clients?page=2#contact"}'
      )
    );
    expect(screen.queryByText("Private content")).not.toBeInTheDocument();
  });
  it("denies authenticated users without the required role", async () => {
    useAuthStore.getState().setSession(profile);
    renderGuard(
      <ProtectedRoute roles={["Admin"]}>
        <h1>Private content</h1>
      </ProtectedRoute>,
      "/admin"
    );
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/access-denied"
      )
    );
    expect(screen.queryByText("Private content")).not.toBeInTheDocument();
  });
  it("allows a matching role among multiple roles", () => {
    useAuthStore
      .getState()
      .setSession({ ...profile, roles: ["User", "Admin"] });
    renderGuard(
      <ProtectedRoute roles={["Admin"]}>
        <h1>Private content</h1>
      </ProtectedRoute>
    );
    expect(
      screen.getByRole("heading", { name: "Private content" })
    ).toBeVisible();
  });
});

describe("workspace navigation", () => {
  it("hides administration links from ordinary users and marks nested routes active", () => {
    useAuthStore.getState().setSession(profile);
    renderApp(<NavBar />, "/networkDevices/device-1");
    const nav = within(
      screen.getByRole("navigation", { name: "Main navigation" })
    );
    expect(nav.getByRole("link", { name: "Devices" })).toHaveAttribute(
      "aria-current",
      "page"
    );
    expect(nav.getByRole("link", { name: "Clients" })).toHaveAttribute(
      "href",
      "/clients"
    );
    expect(
      nav.queryByRole("link", { name: "Administration" })
    ).not.toBeInTheDocument();
    expect(nav.queryByRole("link", { name: "Gates" })).not.toBeInTheDocument();
    expect(
      nav.queryByRole("link", { name: "Workflows" })
    ).not.toBeInTheDocument();
  });
  it("exposes administrative destinations to administrators", () => {
    useAuthStore.getState().setSession({ ...profile, roles: ["Admin"] });
    renderApp(<NavBar />, "/admin/workflows/one");
    const nav = within(
      screen.getByRole("navigation", { name: "Main navigation" })
    );
    expect(nav.getByRole("link", { name: "Workflows" })).toHaveAttribute(
      "aria-current",
      "page"
    );
    expect(
      nav.getByRole("link", { name: "Administration" })
    ).not.toHaveAttribute("aria-current");
    expect(nav.getByRole("link", { name: "Users" })).toHaveAttribute(
      "href",
      "/admin/users"
    );
    expect(nav.getByRole("link", { name: "Gates" })).toBeInTheDocument();
  });
  it("clears session and cached business data even if server logout fails", async () => {
    useAuthStore.getState().setSession(profile);
    vi.spyOn(authApi, "cookieLogout").mockRejectedValue(new Error("Offline"));
    const { queryClient } = renderApp(<NavBar />);
    queryClient.setQueryData(["clients"], [{ id: "private-record" }]);
    await userEvent.click(screen.getByRole("button", { name: "Sign out" }));
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/login"
      )
    );
    expect(useAuthStore.getState().status).toBe("unauthenticated");
    expect(localStorage.getItem("rsi.auth.user")).toBeNull();
    expect(queryClient.getQueryData(["clients"])).toBeUndefined();
  });
});

function loginPage(
  entry: string | { pathname: string; state: unknown } = "/login"
) {
  return renderApp(
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="*" element={<h1>Destination</h1>} />
    </Routes>,
    entry
  );
}
async function fillLogin() {
  const user = userEvent.setup();
  await user.type(screen.getByLabelText(/^Email/), profile.email);
  await user.type(screen.getByLabelText(/^Password/), "Test-password1!");
  await user.tab();
  return user;
}

describe("sign-in flow", () => {
  it("validates email before sending credentials", async () => {
    const login = vi.spyOn(authApi, "cookieLogin");
    loginPage();
    const user = userEvent.setup();
    await user.type(screen.getByLabelText(/^Email/), "invalid");
    await user.tab();
    expect(
      await screen.findByText("Enter a valid email address")
    ).toBeVisible();
    expect(screen.getByRole("button", { name: "Sign in" })).toBeDisabled();
    expect(login).not.toHaveBeenCalled();
  });
  it("toggles password visibility without changing its value", async () => {
    loginPage();
    await userEvent.type(screen.getByLabelText(/^Password/), "secret");
    await userEvent.click(
      screen.getByRole("button", { name: "Show password" })
    );
    expect(screen.getByLabelText(/^Password/)).toHaveAttribute("type", "text");
    expect(screen.getByLabelText(/^Password/)).toHaveValue("secret");
    await userEvent.click(
      screen.getByRole("button", { name: "Hide password" })
    );
    expect(screen.getByLabelText(/^Password/)).toHaveAttribute(
      "type",
      "password"
    );
  });
  it("submits cookie credentials once, disables pending submission and returns to the requested route", async () => {
    const pending = deferred<Awaited<ReturnType<typeof authApi.cookieLogin>>>();
    const login = vi
      .spyOn(authApi, "cookieLogin")
      .mockReturnValue(pending.promise);
    loginPage({
      pathname: "/login",
      state: { from: "/clients?page=2#contact" },
    });
    const user = await fillLogin();
    await user.click(screen.getByRole("button", { name: "Sign in" }));
    expect(
      await screen.findByRole("button", { name: /Signing in/ })
    ).toBeDisabled();
    expect(login).toHaveBeenCalledExactlyOnceWith(
      profile.email,
      "Test-password1!",
      true
    );
    await act(async () =>
      pending.resolve({
        userId: profile.id,
        email: profile.email,
        roles: profile.roles,
      })
    );
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/clients?page=2#contact"
      )
    );
    expect(useAuthStore.getState().user).toEqual(profile);
  });
  it("shows a failed login and allows a successful retry", async () => {
    vi.spyOn(authApi, "cookieLogin")
      .mockRejectedValueOnce(
        new ApiError({ message: "Unauthorized", status: 401 })
      )
      .mockResolvedValue({
        userId: profile.id,
        email: profile.email,
        roles: profile.roles,
      });
    loginPage();
    const user = await fillLogin();
    await user.click(screen.getByRole("button", { name: "Sign in" }));
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Invalid email or password."
    );
    expect(useAuthStore.getState().status).toBe("unauthenticated");
    await user.click(screen.getByRole("button", { name: "Sign in" }));
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/mainPage"
      )
    );
  });
  it("rejects a malformed server response without establishing a session", async () => {
    vi.spyOn(authApi, "cookieLogin").mockResolvedValue({
      userId: "",
      email: profile.email,
      roles: [],
    });
    loginPage();
    const user = await fillLogin();
    await user.click(screen.getByRole("button", { name: "Sign in" }));
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "invalid login response"
    );
    expect(useAuthStore.getState().user).toBeNull();
  });
  it("redirects an already signed-in visitor without calling the login API", async () => {
    useAuthStore.getState().setSession(profile);
    const login = vi.spyOn(authApi, "cookieLogin");
    loginPage();
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/mainPage"
      )
    );
    expect(login).not.toHaveBeenCalled();
  });
});
