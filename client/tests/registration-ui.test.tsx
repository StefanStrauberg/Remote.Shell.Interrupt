// @vitest-environment jsdom
import { act, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { toast } from "react-toastify";
import RegisterPage from "../src/features/Auth/Register/RegisterPage";
import { authApi, AuthRegisterResponse } from "../src/lib/auth/authApi";
import { useAuthStore } from "../src/lib/auth/authStore";
import { routes } from "../src/app/router/paths";
import { deferred, renderApp } from "./renderApp";

const admin = { id: "admin-1", email: "admin@example.com", roles: ["Admin"] };
beforeEach(() => {
  localStorage.clear();
  useAuthStore.setState({ user: admin, status: "authenticated" });
  vi.spyOn(toast, "success").mockImplementation(() => "success");
});
afterEach(() => {
  useAuthStore.setState({ user: null, status: "unauthenticated" });
  localStorage.clear();
});

async function fillForm(confirm = "ValidPass123!") {
  await userEvent.type(
    screen.getByRole("textbox", { name: /Email/ }),
    "new@example.com"
  );
  await userEvent.type(
    screen.getByLabelText(/^Password\s*\*?$/),
    "ValidPass123!"
  );
  await userEvent.type(screen.getByLabelText(/^Confirm password/), confirm);
  await userEvent.tab();
}

describe("administrator account provisioning", () => {
  it.each([false, true])(
    "redirects unauthorized visitors (authenticated=%s)",
    async (authenticated) => {
      const register = vi
        .spyOn(authApi, "register")
        .mockResolvedValue({ success: true, userId: "new-user" });
      useAuthStore.setState({
        user: authenticated ? { ...admin, roles: ["User"] } : null,
        status: authenticated ? "authenticated" : "unauthenticated",
      });
      renderApp(<RegisterPage />);
      await waitFor(() =>
        expect(screen.getByLabelText("Current location")).toHaveTextContent(
          authenticated ? routes.accessDenied : routes.login
        )
      );
      expect(
        screen.queryByRole("button", { name: "Create account" })
      ).not.toBeInTheDocument();
      expect(register).not.toHaveBeenCalled();
    }
  );

  it("rejects a mismatched password confirmation", async () => {
    const register = vi
      .spyOn(authApi, "register")
      .mockResolvedValue({ success: true, userId: "new-user" });
    renderApp(<RegisterPage />);
    await fillForm("DifferentPass123!");
    expect(await screen.findByText("Passwords do not match")).toBeVisible();
    expect(
      screen.getByRole("button", { name: "Create account" })
    ).toBeDisabled();
    expect(register).not.toHaveBeenCalled();
  });

  it("creates an account with the selected role without replacing the administrator session", async () => {
    const pending = deferred<AuthRegisterResponse>();
    const register = vi
      .spyOn(authApi, "register")
      .mockReturnValue(pending.promise);
    const { queryClient } = renderApp(<RegisterPage />);
    queryClient.setQueryData(["users", "cached"], []);
    await fillForm();
    await userEvent.click(screen.getByRole("combobox"));
    await userEvent.click(
      screen.getByRole("option", { name: "Administrator" })
    );
    await userEvent.click(
      screen.getByRole("button", { name: "Create account" })
    );
    await waitFor(() =>
      expect(register).toHaveBeenCalledExactlyOnceWith(
        "new@example.com",
        "ValidPass123!",
        "Admin"
      )
    );
    expect(screen.getByRole("button", { name: "Creating..." })).toBeDisabled();
    await act(async () =>
      pending.resolve({ success: true, userId: "new-user" })
    );
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        routes.adminUsers
      )
    );
    expect(useAuthStore.getState().user).toEqual(admin);
    expect(queryClient.getQueryState(["users", "cached"])?.isInvalidated).toBe(
      true
    );
  });

  it("displays a rejected registration without navigating and lets the administrator retry", async () => {
    const register = vi
      .spyOn(authApi, "register")
      .mockResolvedValueOnce({
        success: false,
        userId: null,
        error: "Email already registered",
      })
      .mockResolvedValue({ success: true, userId: "new-user" });
    renderApp(<RegisterPage />);
    await fillForm();
    await userEvent.click(
      screen.getByRole("button", { name: "Create account" })
    );
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Email already registered"
    );
    expect(screen.getByLabelText("Current location")).not.toHaveTextContent(
      routes.adminUsers
    );
    await userEvent.click(
      screen.getByRole("button", { name: "Create account" })
    );
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        routes.adminUsers
      )
    );
    expect(register).toHaveBeenCalledTimes(2);
  });
});
