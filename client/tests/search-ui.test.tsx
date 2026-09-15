// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { act, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import MainPageDashboard from "../src/features/MainPage/MainPageDashboard";
import MainPageListFilter from "../src/features/MainPage/MainPageListFilter";
import { networkDevicesApi } from "../src/features/NetworkDevices/api/networkDevicesApi";
import { CompoundObject } from "../src/lib/types/NetworkDevices/CompoundObject";
import { deferred, renderApp } from "./renderApp";

const empty: CompoundObject = { clients: [], networkDevices: [] };
const result: CompoundObject = {
  clients: [],
  networkDevices: [
    {
      id: "router-1",
      host: "192.0.2.1",
      networkDeviceName: "Core router",
      typeOfNetworkDevice: "Cisco",
      generalInformation: "Main building",
      portsOfNetworkDevice: [],
    },
  ],
};

describe("VLAN search form", () => {
  it.each(["0", "4095", "1.5", "-1"])(
    "rejects invalid VLAN %s without invoking search",
    async (value) => {
      const search = vi.fn();
      const apply = vi.fn();
      renderApp(
        <MainPageListFilter onApplyFilters={apply} onSearch={search} />
      );
      await userEvent.type(
        screen.getByRole("spinbutton", { name: "VLAN ID" }),
        value
      );
      expect(
        screen.getByText("VLAN ID must be a whole number from 1 to 4094")
      ).toBeVisible();
      expect(screen.getByRole("button", { name: "Search" })).toBeDisabled();
      await userEvent.keyboard("{Enter}");
      expect(search).not.toHaveBeenCalled();
      expect(apply).not.toHaveBeenCalled();
    }
  );
  it.each([1, 4094])("submits boundary VLAN %s by keyboard", async (value) => {
    const apply = vi.fn();
    const search = vi.fn();
    renderApp(<MainPageListFilter onApplyFilters={apply} onSearch={search} />);
    await userEvent.type(
      screen.getByRole("spinbutton", { name: "VLAN ID" }),
      `${value}{Enter}`
    );
    expect(apply).toHaveBeenCalledExactlyOnceWith({
      IdVlan: { op: "==", value },
    });
    expect(search).toHaveBeenCalledTimes(1);
  });
  it("resets the value, validation and applied filters", async () => {
    const apply = vi.fn();
    const search = vi.fn();
    renderApp(<MainPageListFilter onApplyFilters={apply} onSearch={search} />);
    await userEvent.type(screen.getByRole("spinbutton"), "4095");
    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    expect(screen.getByRole("spinbutton")).toHaveValue(null);
    expect(
      screen.queryByText("VLAN ID must be a whole number from 1 to 4094")
    ).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Search" })).toBeDisabled();
    expect(apply).toHaveBeenLastCalledWith({});
    expect(search).not.toHaveBeenCalled();
  });
});

describe("network search integration", () => {
  it("does not fetch on initial render or while typing", async () => {
    const fetch = vi
      .spyOn(networkDevicesApi, "getByVlan")
      .mockResolvedValue(empty);
    renderApp(<MainPageDashboard />);
    expect(screen.getByText("Start a VLAN search")).toBeVisible();
    await userEvent.type(screen.getByRole("spinbutton"), "100");
    expect(fetch).not.toHaveBeenCalled();
  });
  it("shows loading and then the actual device returned by the API", async () => {
    const pending = deferred<CompoundObject>();
    const fetch = vi
      .spyOn(networkDevicesApi, "getByVlan")
      .mockReturnValue(pending.promise);
    renderApp(<MainPageDashboard />);
    await userEvent.type(screen.getByRole("spinbutton"), "100{Enter}");
    expect(await screen.findByText("Searching for devices…")).toBeVisible();
    expect(screen.getByRole("spinbutton")).toHaveValue(100);
    expect(fetch).toHaveBeenCalledExactlyOnceWith(100);
    await act(async () => pending.resolve(result));
    expect(await screen.findByText("Core router")).toBeVisible();
    expect(screen.getByText("Network Devices (1)")).toBeVisible();
    expect(
      screen.queryByText("Searching for devices…")
    ).not.toBeInTheDocument();
  });
  it("distinguishes an empty successful result from the initial state", async () => {
    vi.spyOn(networkDevicesApi, "getByVlan").mockResolvedValue(empty);
    renderApp(<MainPageDashboard />);
    await userEvent.clear(screen.getByRole("spinbutton"));
    await userEvent.type(screen.getByRole("spinbutton"), "200{Enter}");
    expect(await screen.findByText("No devices found")).toBeVisible();
    expect(screen.queryByText("Start a VLAN search")).not.toBeInTheDocument();
  });
  it("lets the user recover from an API error with a new search", async () => {
    const fetch = vi
      .spyOn(networkDevicesApi, "getByVlan")
      .mockRejectedValueOnce(new Error("Connection unavailable"))
      .mockResolvedValue(result);
    renderApp(<MainPageDashboard />);
    await userEvent.type(screen.getByRole("spinbutton"), "100{Enter}");
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Connection unavailable"
    );
    await userEvent.clear(screen.getByRole("spinbutton"));
    await userEvent.type(screen.getByRole("spinbutton"), "200{Enter}");
    expect(await screen.findByText("Core router")).toBeVisible();
    expect(fetch).toHaveBeenLastCalledWith(200);
    expect(screen.queryByRole("alert")).not.toBeInTheDocument();
  });
  it("clears displayed results on reset", async () => {
    vi.spyOn(networkDevicesApi, "getByVlan").mockResolvedValue(result);
    renderApp(<MainPageDashboard />);
    await userEvent.type(screen.getByRole("spinbutton"), "100{Enter}");
    await screen.findByText("Core router");
    expect(screen.getByRole("spinbutton")).toHaveValue(100);
    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    await waitFor(() =>
      expect(screen.getByText("Start a VLAN search")).toBeVisible()
    );
    expect(screen.queryByText("Core router")).not.toBeInTheDocument();
  });
});
