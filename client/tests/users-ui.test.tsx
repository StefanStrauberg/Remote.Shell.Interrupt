// @vitest-environment jsdom
import { act, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { Table, TableBody } from "@mui/material";
import { toast } from "react-toastify";
import UserRow from "../src/features/Users/List/UserRow";
import EditUserDialog from "../src/features/Users/List/EditUserDialog";
import { usersApi } from "../src/features/Users/api/usersApi";
import { User } from "../src/lib/types/Users/User";
import { deferred, renderApp } from "./renderApp";

const account: User = {
  id: "operator-id",
  email: "operator@example.com",
  fullName: "Network Operator",
  roles: ["User"],
  isActive: true,
  createdAtUtc: "2026-01-01T12:00:00Z",
};

beforeEach(() => {
  vi.spyOn(toast, "success").mockImplementation(() => "success");
  vi.spyOn(toast, "error").mockImplementation(() => "error");
});

function renderRow(isSelf = false) {
  return renderApp(
    <Table>
      <TableBody>
        <UserRow user={account} isSelf={isSelf} />
      </TableBody>
    </Table>
  );
}

describe("account administration", () => {
  it("prevents changing one's own role, deactivation and deletion", () => {
    const active = vi.spyOn(usersApi, "setActive").mockResolvedValue();
    renderRow(true);
    expect(screen.getByText("You")).toBeVisible();
    expect(screen.queryByRole("combobox")).not.toBeInTheDocument();
    expect(screen.getByRole("checkbox")).toBeDisabled();
    expect(screen.queryByTestId("DeleteIcon")).not.toBeInTheDocument();
    expect(active).not.toHaveBeenCalled();
  });

  it("changes another user's role and refreshes cached users", async () => {
    const update = vi.spyOn(usersApi, "updateRole").mockResolvedValue();
    const { queryClient } = renderRow();
    queryClient.setQueryData(["users", "list", "cached"], [account]);
    await userEvent.click(screen.getByRole("combobox"));
    await userEvent.click(screen.getByRole("option", { name: "Admin" }));
    await waitFor(() =>
      expect(update).toHaveBeenCalledExactlyOnceWith(account.id, "Admin")
    );
    await waitFor(() =>
      expect(
        queryClient.getQueryState(["users", "list", "cached"])?.isInvalidated
      ).toBe(true)
    );
    expect(toast.success).toHaveBeenCalledWith(
      `${account.email} is now Admin.`
    );
  });

  it("disables concurrent actions and restores active state after a failed update", async () => {
    const pending = deferred<void>();
    vi.spyOn(usersApi, "setActive").mockReturnValue(pending.promise);
    renderRow();
    await userEvent.click(screen.getByRole("checkbox"));
    expect(screen.getByRole("checkbox")).not.toBeChecked();
    expect(screen.getByRole("checkbox")).toBeDisabled();
    expect(screen.getByRole("combobox")).toHaveAttribute(
      "aria-disabled",
      "true"
    );
    await act(async () => pending.reject(new Error("Permission denied")));
    await waitFor(() => expect(screen.getByRole("checkbox")).toBeEnabled());
    expect(screen.getByRole("checkbox")).toBeChecked();
    expect(toast.error).toHaveBeenCalledWith(
      "Failed to update account: Permission denied"
    );
  });

  it("does not delete when confirmation is cancelled", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(false);
    const remove = vi.spyOn(usersApi, "remove").mockResolvedValue();
    renderRow();
    await userEvent.click(screen.getByTestId("DeleteIcon").closest("button")!);
    expect(window.confirm).toHaveBeenCalledWith(
      expect.stringContaining(account.email)
    );
    expect(remove).not.toHaveBeenCalled();
  });

  it("deletes the selected account only after confirmation", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(true);
    const remove = vi.spyOn(usersApi, "remove").mockResolvedValue();
    renderRow();
    await userEvent.click(screen.getByTestId("DeleteIcon").closest("button")!);
    await waitFor(() => expect(remove).toHaveBeenCalledTimes(1));
    expect(remove.mock.calls[0][0]).toBe(account.id);
    await waitFor(() =>
      expect(toast.success).toHaveBeenCalledWith(
        `${account.email} was deleted.`
      )
    );
  });
});

describe("account profile form", () => {
  it("validates email before sending changes", async () => {
    const update = vi.spyOn(usersApi, "updateProfile").mockResolvedValue();
    renderApp(<EditUserDialog user={account} open onClose={vi.fn()} />);
    const email = screen.getByRole("textbox", { name: /Email/ });
    await userEvent.clear(email);
    await userEvent.type(email, "invalid");
    await userEvent.tab();
    expect(
      await screen.findByText("Enter a valid email address")
    ).toBeVisible();
    expect(screen.getByRole("button", { name: "Save" })).toBeDisabled();
    expect(update).not.toHaveBeenCalled();
  });

  it("normalizes an empty full name, blocks duplicate saves and closes on success", async () => {
    const pending = deferred<void>();
    const update = vi
      .spyOn(usersApi, "updateProfile")
      .mockReturnValue(pending.promise);
    const close = vi.fn();
    renderApp(<EditUserDialog user={account} open onClose={close} />);
    await userEvent.clear(screen.getByRole("textbox", { name: "Full name" }));
    await userEvent.type(
      screen.getByRole("textbox", { name: "Full name" }),
      "   "
    );
    await userEvent.tab();
    await userEvent.click(screen.getByRole("button", { name: "Save" }));
    await waitFor(() =>
      expect(update).toHaveBeenCalledExactlyOnceWith(
        account.id,
        account.email,
        null
      )
    );
    expect(screen.getByRole("button", { name: "Saving..." })).toBeDisabled();
    expect(close).not.toHaveBeenCalled();
    await act(async () => pending.resolve());
    await waitFor(() => expect(close).toHaveBeenCalledTimes(1));
  });

  it("keeps entered values available to retry a failed save", async () => {
    vi.spyOn(usersApi, "updateProfile").mockRejectedValue(
      new Error("Email already exists")
    );
    const close = vi.fn();
    renderApp(<EditUserDialog user={account} open onClose={close} />);
    const email = screen.getByRole("textbox", { name: /Email/ });
    await userEvent.clear(email);
    await userEvent.type(email, "other@example.com");
    await userEvent.tab();
    await userEvent.click(screen.getByRole("button", { name: "Save" }));
    await waitFor(() =>
      expect(toast.error).toHaveBeenCalledWith(
        "Failed to update account: Email already exists"
      )
    );
    expect(close).not.toHaveBeenCalled();
    expect(email).toHaveValue("other@example.com");
    expect(screen.getByRole("button", { name: "Save" })).toBeEnabled();
  });
});
