// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { act, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Route, Routes } from "react-router";
import GateForm from "../src/features/Gates/CreateUpdate/GateForm";
import { gatesApi } from "../src/features/Gates/api/gatesApi";
import { Gate } from "../src/lib/types/Gates/Gate";
import { deferred, renderApp } from "./renderApp";

const gate: Gate = {
  id: "gate-1",
  name: "Core gateway",
  ipAddress: "192.0.2.1",
  community: "test-community",
  typeOfNetworkDevice: "Cisco",
};
function renderGate(edit = false) {
  return renderApp(
    <Routes>
      <Route path="/createGate" element={<GateForm />} />
      <Route path="/gates/:id" element={<GateForm />} />
      <Route path="*" element={<h1>Saved destination</h1>} />
    </Routes>,
    edit ? "/gates/gate-1" : "/createGate"
  );
}
async function fillGate() {
  const user = userEvent.setup();
  await user.type(screen.getByLabelText(/^Name/), gate.name);
  await user.type(screen.getByLabelText(/^IP Address/), gate.ipAddress);
  await user.type(screen.getByLabelText(/^Community/), gate.community);
  await user.click(screen.getByRole("combobox"));
  await user.click(screen.getByRole("option", { name: "Cisco" }));
  await user.tab();
  return user;
}

describe("gate create and edit forms", () => {
  it("does not fetch a gate or enable Save in create mode", () => {
    const get = vi.spyOn(gatesApi, "getById");
    renderGate();
    expect(screen.getByRole("heading", { name: "Create gate" })).toBeVisible();
    expect(screen.getByRole("button", { name: "Save" })).toBeDisabled();
    expect(get).not.toHaveBeenCalled();
  });
  it("rejects an invalid IP address", async () => {
    const create = vi.spyOn(gatesApi, "create");
    renderGate();
    await userEvent.type(screen.getByLabelText(/^IP Address/), "300.1.1.1");
    await userEvent.tab();
    expect(await screen.findByText("Invalid IP address format")).toBeVisible();
    expect(screen.getByRole("button", { name: "Save" })).toBeDisabled();
    expect(create).not.toHaveBeenCalled();
  });
  it("creates a gate once and prevents duplicate submission while saving", async () => {
    const pending = deferred<void>();
    const create = vi
      .spyOn(gatesApi, "create")
      .mockReturnValue(pending.promise);
    renderGate();
    const user = await fillGate();
    await user.click(screen.getByRole("button", { name: "Save" }));
    expect(
      await screen.findByRole("button", { name: "Saving..." })
    ).toBeDisabled();
    expect(create).toHaveBeenCalledTimes(1);
    expect(create.mock.calls[0][0]).toEqual({
      name: gate.name,
      ipAddress: gate.ipAddress,
      community: gate.community,
      typeOfNetworkDevice: gate.typeOfNetworkDevice,
    });
    await act(async () => pending.resolve());
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/admin"
      )
    );
  });
  it("preserves input when creation fails so the user can retry", async () => {
    vi.spyOn(gatesApi, "create").mockRejectedValue(
      new Error("A gate with this address already exists")
    );
    renderGate();
    const user = await fillGate();
    await user.click(screen.getByRole("button", { name: "Save" }));
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "already exists"
    );
    expect(screen.getByLabelText(/^Name/)).toHaveValue(gate.name);
    expect(screen.getByRole("button", { name: "Save" })).toBeEnabled();
  });
  it("loads existing values and updates only after an edit", async () => {
    vi.spyOn(gatesApi, "getById").mockResolvedValue(gate);
    const update = vi.spyOn(gatesApi, "update").mockResolvedValue();
    const { queryClient } = renderGate(true);
    queryClient.setQueryData(["gates", "list", "fixture"], [gate]);
    const name = await screen.findByLabelText(/^Name/);
    expect(name).toHaveValue(gate.name);
    expect(screen.getByRole("button", { name: "Save" })).toBeDisabled();
    await userEvent.clear(name);
    await userEvent.type(name, "Updated gateway");
    await userEvent.tab();
    await userEvent.click(screen.getByRole("button", { name: "Save" }));
    await waitFor(() =>
      expect(update.mock.calls[0]?.[0]).toEqual({
        ...gate,
        name: "Updated gateway",
      })
    );
    await waitFor(() =>
      expect(screen.getByLabelText("Current location")).toHaveTextContent(
        "/gates|null"
      )
    );
    expect(
      queryClient.getQueryState(["gates", "list", "fixture"])?.isInvalidated
    ).toBe(true);
  });
  it("shows a read error and a way back to the list", async () => {
    vi.spyOn(gatesApi, "getById").mockRejectedValue(
      new Error("Gate not found")
    );
    renderGate(true);
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Gate not found"
    );
    expect(screen.getByRole("link", { name: "Back to gates" })).toHaveAttribute(
      "href",
      "/gates"
    );
    expect(
      screen.queryByRole("button", { name: "Save" })
    ).not.toBeInTheDocument();
  });
  it("cancels without creating or updating a record", async () => {
    const create = vi.spyOn(gatesApi, "create");
    const update = vi.spyOn(gatesApi, "update");
    renderGate();
    await userEvent.click(screen.getByRole("link", { name: "Cancel" }));
    expect(screen.getByLabelText("Current location")).toHaveTextContent(
      "/admin"
    );
    expect(create).not.toHaveBeenCalled();
    expect(update).not.toHaveBeenCalled();
  });
});
