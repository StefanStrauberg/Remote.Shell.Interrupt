// @vitest-environment jsdom
import { act, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { toast } from "react-toastify";
import AdminPage from "../src/features/Admin/AdminPage";
import { clientsApi } from "../src/features/Clients/api/clientsApi";
import { gatesApi } from "../src/features/Gates/api/gatesApi";
import { networkDevicesApi } from "../src/features/NetworkDevices/api/networkDevicesApi";
import { usersApi } from "../src/features/Users/api/usersApi";
import { clientKeys } from "../src/features/Clients/api/clientsQueries";
import { tfPlanKeys } from "../src/features/TfPlans/api/tfPlansQueries";
import { sprVlanKeys } from "../src/features/SPRVlans/api/sprVlansQueries";
import { DEFAULT_PAGINATION } from "../src/lib/constants/pagination";
import { deferred, renderApp } from "./renderApp";

beforeEach(() => {
  const page = {
    data: [],
    pagination: { ...DEFAULT_PAGINATION, TotalCount: 2 },
  };
  vi.spyOn(gatesApi, "list").mockResolvedValue(page);
  vi.spyOn(clientsApi, "list").mockResolvedValue(page);
  vi.spyOn(networkDevicesApi, "list").mockResolvedValue(page);
  vi.spyOn(usersApi, "list").mockResolvedValue(page);
  vi.spyOn(toast, "success").mockImplementation(() => "success");
  vi.spyOn(toast, "error").mockImplementation(() => "error");
});

async function openConfirmation(name: RegExp) {
  await waitFor(() =>
    expect(screen.getByRole("button", { name })).toBeEnabled()
  );
  await userEvent.click(screen.getByRole("button", { name }));
  return within(await screen.findByRole("dialog"));
}

describe("administrative operations", () => {
  it.each([
    ["clients", /Delete all clients/i],
    ["devices", /Delete all network devices/i],
    ["gates", /Delete all gates/i],
  ] as const)(
    "never deletes %s after dismissing confirmation",
    async (_kind, name) => {
      const clients = vi.spyOn(clientsApi, "removeAll").mockResolvedValue();
      const devices = vi
        .spyOn(networkDevicesApi, "removeAll")
        .mockResolvedValue();
      const gates = vi.spyOn(gatesApi, "removeAll").mockResolvedValue(2);
      renderApp(<AdminPage />);
      const dialog = await openConfirmation(name);
      await userEvent.click(dialog.getByRole("button", { name: "Cancel" }));
      await waitFor(() =>
        expect(screen.queryByRole("dialog")).not.toBeInTheDocument()
      );
      expect(clients).not.toHaveBeenCalled();
      expect(devices).not.toHaveBeenCalled();
      expect(gates).not.toHaveBeenCalled();
    }
  );

  it("runs the selected deletion once and blocks concurrent operations", async () => {
    const pending = deferred<void>();
    const remove = vi
      .spyOn(networkDevicesApi, "removeAll")
      .mockReturnValue(pending.promise);
    const clients = vi.spyOn(clientsApi, "removeAll").mockResolvedValue();
    renderApp(<AdminPage />);
    const dialog = await openConfirmation(/Delete all network devices/i);
    expect(remove).not.toHaveBeenCalled();
    await userEvent.click(dialog.getByRole("button", { name: "Confirm" }));
    await waitFor(() => expect(remove).toHaveBeenCalledTimes(1));
    await waitFor(() =>
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument()
    );
    expect(
      screen.getByRole("button", { name: "Delete all clients" })
    ).toBeDisabled();
    expect(
      screen.getByRole("button", { name: "Sync clients, plans, VLANs" })
    ).toBeDisabled();
    expect(clients).not.toHaveBeenCalled();
    await act(async () => pending.resolve());
    await waitFor(() =>
      expect(toast.success).toHaveBeenCalledWith(
        "All network devices were deleted successfully."
      )
    );
    expect(
      screen.getByRole("button", { name: "Delete all clients" })
    ).toBeEnabled();
  });

  it("invalidates clients, tariffs and VLANs after synchronization", async () => {
    const sync = vi.spyOn(clientsApi, "synchronize").mockResolvedValue();
    const { queryClient } = renderApp(<AdminPage />);
    const keys = [clientKeys.all, tfPlanKeys.all, sprVlanKeys.all].map(
      (key) => [...key, "cached"]
    );
    keys.forEach((key) => queryClient.setQueryData(key, []));
    const dialog = await openConfirmation(/Sync clients, plans, VLANs/);
    expect(sync).not.toHaveBeenCalled();
    await userEvent.click(dialog.getByRole("button", { name: "Confirm" }));
    await waitFor(() =>
      expect(toast.success).toHaveBeenCalledWith(
        "Clients were updated successfully."
      )
    );
    expect(sync).toHaveBeenCalledTimes(1);
    keys.forEach((key) =>
      expect(queryClient.getQueryState(key)?.isInvalidated).toBe(true)
    );
  });

  it("reports a failed deletion and makes the controls available for retry", async () => {
    vi.spyOn(clientsApi, "removeAll").mockRejectedValue(
      new Error("Database unavailable")
    );
    renderApp(<AdminPage />);
    const dialog = await openConfirmation(/Delete all clients/i);
    await userEvent.click(dialog.getByRole("button", { name: "Confirm" }));
    await waitFor(() =>
      expect(toast.error).toHaveBeenCalledWith(
        "Failed to delete clients: Database unavailable"
      )
    );
    await waitFor(() =>
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument()
    );
    expect(
      screen.getByRole("button", { name: "Delete all clients" })
    ).toBeEnabled();
    expect(toast.success).not.toHaveBeenCalled();
  });
});
