// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import GatesDashboard from "../src/features/Gates/List/GatesDashboard";
import GateCard from "../src/features/Gates/List/GateCard";
import { gatesApi } from "../src/features/Gates/api/gatesApi";
import { Gate } from "../src/lib/types/Gates/Gate";
import { deferred, renderApp } from "./renderApp";

const pagination = {
  TotalCount: 1,
  PageSize: 12,
  CurrentPage: 1,
  TotalPages: 1,
  HasNext: false,
  HasPrevious: false,
};

const gate: Gate = {
  id: "gate-1",
  name: "Core gateway",
  ipAddress: "192.0.2.1",
  community: "public",
  typeOfNetworkDevice: "Cisco",
};

describe("gates dashboard", () => {
  it("shows an empty state and lets the filter panel apply and reset filters", async () => {
    const list = vi.spyOn(gatesApi, "list").mockResolvedValue({
      data: [],
      pagination: { ...pagination, TotalCount: 0 },
    });
    renderApp(<GatesDashboard />);
    expect(await screen.findByText("No gates found")).toBeVisible();

    await userEvent.type(screen.getByLabelText("Gate Name"), "core");
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: [
            { PropertyPath: "Name", Operator: "Contains", Value: "core" },
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
    expect(screen.getByLabelText("Gate Name")).toHaveValue("");
  });

  it("renders gate cards once data loads", async () => {
    vi.spyOn(gatesApi, "list").mockResolvedValue({ data: [gate], pagination });
    renderApp(<GatesDashboard />);
    expect(await screen.findByText("Core gateway")).toBeVisible();
    expect(screen.getByText("1 gates found")).toBeVisible();
  });

  it("shows a load error", async () => {
    vi.spyOn(gatesApi, "list").mockRejectedValue(new Error("Network down"));
    renderApp(<GatesDashboard />);
    expect(await screen.findByRole("alert")).toHaveTextContent("Network down");
  });

  it("filters by IP address and device type together", async () => {
    const list = vi
      .spyOn(gatesApi, "list")
      .mockResolvedValue({ data: [gate], pagination });
    renderApp(<GatesDashboard />);
    await screen.findByText("Core gateway");

    await userEvent.type(screen.getByLabelText("IP Address"), "192.0.2.1");
    await userEvent.click(screen.getByRole("combobox"));
    await userEvent.click(screen.getByRole("option", { name: "Cisco" }));
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));

    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: [
            {
              PropertyPath: "IpAddress",
              Operator: "Equals",
              Value: "192.0.2.1",
            },
            {
              PropertyPath: "TypeOfNetworkDevice",
              Operator: "Equals",
              Value: "Cisco",
            },
          ],
        })
      )
    );
  });
});

describe("gate card", () => {
  function renderCard() {
    return renderApp(<GateCard gate={gate} />);
  }

  it("links to the edit page for this gate", () => {
    renderCard();
    expect(screen.getByRole("link", { name: "Edit" })).toHaveAttribute(
      "href",
      "/gates/gate-1"
    );
  });

  it("does not delete when the confirmation dialog is cancelled", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(false);
    const remove = vi.spyOn(gatesApi, "remove");
    renderCard();
    await userEvent.click(screen.getByRole("button", { name: "Delete" }));
    expect(window.confirm).toHaveBeenCalledWith(
      expect.stringContaining("Core gateway")
    );
    expect(remove).not.toHaveBeenCalled();
  });

  it("deletes the gate after confirmation and shows a pending state", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(true);
    const pending = deferred<void>();
    const remove = vi
      .spyOn(gatesApi, "remove")
      .mockReturnValue(pending.promise);
    renderCard();
    await userEvent.click(screen.getByRole("button", { name: "Delete" }));
    expect(remove).toHaveBeenCalledTimes(1);
    expect(remove.mock.calls[0][0]).toBe("gate-1");
    expect(
      await screen.findByRole("button", { name: "Deleting..." })
    ).toBeDisabled();
  });
});
