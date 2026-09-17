// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import UsersDashboard from "../src/features/Users/List/UsersDashboard";
import { usersApi } from "../src/features/Users/api/usersApi";
import { useAuthStore } from "../src/lib/auth/authStore";
import { User } from "../src/lib/types/Users/User";
import { renderApp } from "./renderApp";

const pagination = {
  TotalCount: 2,
  PageSize: 15,
  CurrentPage: 1,
  TotalPages: 1,
  HasNext: false,
  HasPrevious: false,
};

const me: User = {
  id: "me-id",
  email: "me@example.com",
  fullName: "Current User",
  roles: ["Admin"],
  isActive: true,
  createdAtUtc: "2026-01-01T00:00:00Z",
};
const other: User = {
  id: "other-id",
  email: "other@example.com",
  fullName: "Other User",
  roles: ["User"],
  isActive: true,
  createdAtUtc: "2026-01-02T00:00:00Z",
};

function signIn() {
  useAuthStore.getState().setSession({
    id: me.id,
    email: me.email,
    roles: me.roles,
  });
}

describe("users dashboard", () => {
  it("marks the signed-in account and offers a way to create a user", async () => {
    signIn();
    vi.spyOn(usersApi, "list").mockResolvedValue({
      data: [me, other],
      pagination,
    });
    renderApp(<UsersDashboard />);
    await screen.findByText(me.email);
    expect(screen.getByText("You")).toBeVisible();
    expect(screen.getByRole("link", { name: "Create user" })).toHaveAttribute(
      "href",
      "/register"
    );
  });

  it("shows an empty state when no accounts match", async () => {
    signIn();
    vi.spyOn(usersApi, "list").mockResolvedValue({
      data: [],
      pagination: { ...pagination, TotalCount: 0 },
    });
    renderApp(<UsersDashboard />);
    expect(await screen.findByText("No accounts found")).toBeVisible();
  });

  it("shows a load error", async () => {
    signIn();
    vi.spyOn(usersApi, "list").mockRejectedValue(new Error("Forbidden"));
    renderApp(<UsersDashboard />);
    expect(await screen.findByRole("alert")).toHaveTextContent("Forbidden");
  });

  it("filters accounts by email and active status", async () => {
    signIn();
    const list = vi
      .spyOn(usersApi, "list")
      .mockResolvedValue({ data: [me, other], pagination });
    renderApp(<UsersDashboard />);
    await screen.findByText(me.email);

    await userEvent.type(screen.getByLabelText("Email contains"), "other");
    // The filter's "Status" select is the last combobox in the DOM — each
    // row's own role select (rendered earlier, one per listed user) is also
    // a combobox, so a plain getByRole would be ambiguous.
    const comboboxes = screen.getAllByRole("combobox");
    await userEvent.click(comboboxes[comboboxes.length - 1]);
    await userEvent.click(screen.getByRole("option", { name: "Active" }));
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));

    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: [
            { PropertyPath: "Email", Operator: "Contains", Value: "other" },
            { PropertyPath: "IsActive", Operator: "Equals", Value: "true" },
          ],
        })
      )
    );

    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({ filters: [] })
      )
    );
    expect(screen.getByLabelText("Email contains")).toHaveValue("");
  });
});
