// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import SPRVlansDashboard from "../src/features/SPRVlans/SPRVlansDashboard";
import { sprVlansApi } from "../src/features/SPRVlans/api/sprVlansApi";
import { SprVlan } from "../src/lib/types/SPRVlans/SprVlan";
import { renderApp } from "./renderApp";

const pagination = {
  TotalCount: 2,
  PageSize: 15,
  CurrentPage: 1,
  TotalPages: 2,
  HasNext: true,
  HasPrevious: false,
};

const linked: SprVlan = {
  id: "vlan-1",
  idVlan: 101,
  idClient: 42,
  useClient: true,
  useCOD: false,
};
const unlinked: SprVlan = {
  id: "vlan-2",
  idVlan: 202,
  idClient: 0,
  useClient: false,
  useCOD: true,
};

describe("SPRVlans dashboard", () => {
  it("defaults to the UseClient=true filter on first load", async () => {
    const list = vi
      .spyOn(sprVlansApi, "list")
      .mockResolvedValue({ data: [linked], pagination });
    renderApp(<SPRVlansDashboard />);
    await screen.findByText("101");
    expect(list).toHaveBeenCalledWith(
      expect.objectContaining({
        filters: [
          { PropertyPath: "UseClient", Operator: "Equals", Value: "true" },
        ],
      })
    );
  });

  it("shows an empty state when nothing matches", async () => {
    vi.spyOn(sprVlansApi, "list").mockResolvedValue({
      data: [],
      pagination: { ...pagination, TotalCount: 0, TotalPages: 0 },
    });
    renderApp(<SPRVlansDashboard />);
    expect(await screen.findByText("No VLANs found")).toBeVisible();
  });

  it("shows a load error", async () => {
    vi.spyOn(sprVlansApi, "list").mockRejectedValue(new Error("Timed out"));
    renderApp(<SPRVlansDashboard />);
    expect(await screen.findByRole("alert")).toHaveTextContent("Timed out");
  });

  it("links a VLAN row to its client when one is assigned, and not otherwise", async () => {
    vi.spyOn(sprVlansApi, "list").mockResolvedValue({
      data: [linked, unlinked],
      pagination,
    });
    renderApp(<SPRVlansDashboard />);
    await screen.findByText("101");
    expect(screen.getByRole("link", { name: "View Client" })).toHaveAttribute(
      "href",
      "/clients/42"
    );
    expect(screen.getByText("No client")).toBeVisible();
  });

  it("sorts by Client ID and resets to page one", async () => {
    const list = vi
      .spyOn(sprVlansApi, "list")
      .mockResolvedValue({ data: [linked], pagination });
    renderApp(<SPRVlansDashboard />);
    await screen.findByText("101");

    await userEvent.click(screen.getByRole("button", { name: /Go to page 2/ }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({ pagination: { pageNumber: 2, pageSize: 15 } })
      )
    );

    await userEvent.click(screen.getByRole("button", { name: /Sort by/ }));
    await userEvent.click(screen.getByRole("menuitem", { name: "Client ID" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          pagination: { pageNumber: 1, pageSize: 15 },
          orderBy: { property: "idClient", descending: false },
        })
      )
    );

    await userEvent.click(screen.getByRole("button", { name: /Sort by/ }));
    await userEvent.click(screen.getByRole("menuitem", { name: "Client ID" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          orderBy: { property: "idClient", descending: true },
        })
      )
    );
  });

  it("applies VLAN ID, Client ID and checkbox filters together", async () => {
    const list = vi
      .spyOn(sprVlansApi, "list")
      .mockResolvedValue({ data: [linked], pagination });
    renderApp(<SPRVlansDashboard />);
    await screen.findByText("101");

    await userEvent.type(screen.getByLabelText("VLAN ID"), "101");
    await userEvent.type(screen.getByLabelText("Client ID"), "42");
    await userEvent.click(screen.getByLabelText("Used by COD"));
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));

    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: [
            { PropertyPath: "UseClient", Operator: "Equals", Value: "true" },
            { PropertyPath: "UseCOD", Operator: "Equals", Value: "true" },
            { PropertyPath: "IdVlan", Operator: "Equals", Value: "101" },
            { PropertyPath: "IdClient", Operator: "Equals", Value: "42" },
          ],
        })
      )
    );

    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: [
            { PropertyPath: "UseClient", Operator: "Equals", Value: "true" },
          ],
        })
      )
    );
  });
});
