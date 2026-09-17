// @vitest-environment jsdom
import { act, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { Route, Routes } from "react-router";
import ClientDetailPage from "../src/features/Clients/Detail/ClientDetailPage";
import NetworkDeviceDetailPage from "../src/features/NetworkDevices/Detail/NetworkDeviceDetailPage";
import NetworkDevicePort from "../src/features/NetworkDevices/Detail/NetworkDevicePort";
import { clientsApi } from "../src/features/Clients/api/clientsApi";
import { networkDevicesApi } from "../src/features/NetworkDevices/api/networkDevicesApi";
import { Client } from "../src/lib/types/Clients/Client";
import { Port } from "../src/lib/types/NetworkDevices/Port";
import { routes } from "../src/app/router/paths";
import { deferred, renderApp } from "./renderApp";

const client: Client = {
  id: "customer-1",
  idClient: 1042,
  name: "Example customer",
  nrDogovor: "C-1042",
  working: true,
  antiDDOS: false,
  id_COD: 1,
  sprVlans: [],
  contactT: "Network operations",
  emailT: "noc@example.com",
  history: "Service enabled",
};
const port: Port = {
  id: "port-1",
  interfaceNumber: 1,
  interfaceName: "ae0",
  interfaceType: "ethernet",
  interfaceStatus: "up",
  interfaceSpeed: 10_000_000_000,
  isAggregated: true,
  macAddress: "02:00:00:00:00:01",
  description: "Uplink",
  vlans: [],
  aggregatedPorts: [],
  macTable: [],
};

describe("client detail page", () => {
  it("loads the route's client and renders contacts and history with missing optional data", async () => {
    const pending = deferred<Client>();
    const get = vi
      .spyOn(clientsApi, "getById")
      .mockReturnValue(pending.promise);
    renderApp(
      <Routes>
        <Route path="/clients/:id" element={<ClientDetailPage />} />
      </Routes>,
      "/clients/customer-1"
    );
    expect(screen.getByText("Loading client…")).toBeVisible();
    expect(get).toHaveBeenCalledExactlyOnceWith("customer-1");
    await act(async () => pending.resolve(client));
    expect(await screen.findByText("Example customer")).toBeVisible();
    expect(screen.getByText(/Дата начала: Нет информации/)).toBeVisible();
    await userEvent.click(screen.getByRole("tab", { name: "Contacts" }));
    expect(screen.getByText(/noc@example.com/)).toBeVisible();
    await userEvent.click(screen.getByRole("tab", { name: "Notes & history" }));
    expect(screen.getByText("Service enabled")).toBeVisible();
    await userEvent.click(screen.getByRole("tab", { name: "Network & plan" }));
    expect(screen.getByText(/ID Влана: Нет информации/)).toBeVisible();
    expect(
      screen.getByRole("link", { name: "Back to clients" })
    ).toHaveAttribute("href", routes.clients);
  });

  it("shows an API failure instead of a misleading empty client", async () => {
    vi.spyOn(clientsApi, "getById").mockRejectedValue(
      new Error("Customer unavailable")
    );
    renderApp(
      <Routes>
        <Route path="/clients/:id" element={<ClientDetailPage />} />
      </Routes>,
      "/clients/customer-1"
    );
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Customer unavailable"
    );
    expect(screen.queryByText("Client not found")).not.toBeInTheDocument();
  });

  it("does not fetch without an identifier", () => {
    const get = vi.spyOn(clientsApi, "getById").mockResolvedValue(client);
    renderApp(<ClientDetailPage />);
    expect(screen.getByText("Client not found")).toBeVisible();
    expect(get).not.toHaveBeenCalled();
  });
});

describe("network device detail page", () => {
  it("loads the selected device and its ports and links back to the directory", async () => {
    const device = {
      id: "router-1",
      host: "192.0.2.10",
      networkDeviceName: "Core router",
      typeOfNetworkDevice: "Cisco",
      generalInformation: "Core",
      portsOfNetworkDevice: [port],
    };
    const pending = deferred<typeof device>();
    const get = vi
      .spyOn(networkDevicesApi, "getById")
      .mockReturnValue(pending.promise);
    renderApp(
      <Routes>
        <Route path="/devices/:id" element={<NetworkDeviceDetailPage />} />
      </Routes>,
      "/devices/router-1"
    );
    expect(screen.getByText("Loading device details…")).toBeVisible();
    await act(async () => pending.resolve(device));
    expect(
      await screen.findByRole("link", { name: "Core router" })
    ).toBeVisible();
    expect(
      screen.getByRole("button", { name: "Inspect ae0 on Core router" })
    ).toBeVisible();
    expect(get).toHaveBeenCalledExactlyOnceWith("router-1");
    expect(
      screen.getByRole("link", { name: /Back to devices/i })
    ).toHaveAttribute("href", routes.networkDevices);
  });

  it("retains directory navigation after an API failure", async () => {
    vi.spyOn(networkDevicesApi, "getById").mockRejectedValue(
      new Error("Device unavailable")
    );
    renderApp(
      <Routes>
        <Route path="/devices/:id" element={<NetworkDeviceDetailPage />} />
      </Routes>,
      "/devices/router-1"
    );
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Device unavailable"
    );
    expect(
      screen.getByRole("link", { name: /Back to devices/i })
    ).toHaveAttribute("href", routes.networkDevices);
  });
});

describe("interface diagnostics", () => {
  it.each([
    [10_000_000_000, "10 Gbps"],
    [100_000_000, "100 Mbps"],
    [128_000, "128 Kbps"],
  ])("formats speed %s using the corresponding unit", (speed, label) => {
    renderApp(
      <NetworkDevicePort port={{ ...port, interfaceSpeed: Number(speed) }} />
    );
    expect(screen.getByText(String(label))).toBeVisible();
  });

  it("shows empty VLAN and MAC data without nonfunctional expansion controls", () => {
    renderApp(
      <NetworkDevicePort port={{ ...port, interfaceStatus: "down" }} />
    );
    expect(screen.getByText("DOWN")).toBeVisible();
    expect(screen.getByText("No VLANs")).toBeVisible();
    expect(screen.getByText("MAC Table: 0 entries")).toBeVisible();
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });

  it("expands MAC addresses and aggregated members independently", async () => {
    renderApp(
      <NetworkDevicePort
        port={{
          ...port,
          macTable: ["02:00:00:00:01:20"],
          aggregatedPorts: [
            { ...port, id: "member-1", interfaceName: "ge-0/0/1" },
          ],
        }}
      />
    );
    const [macToggle, membersToggle] = screen.getAllByRole("button");
    expect(screen.getByText("02:00:00:00:01:20")).not.toBeVisible();
    expect(screen.getByText("ge-0/0/1")).not.toBeVisible();
    await userEvent.click(macToggle);
    expect(screen.getByText("02:00:00:00:01:20")).toBeVisible();
    expect(screen.getByText("ge-0/0/1")).not.toBeVisible();
    await userEvent.click(membersToggle);
    expect(screen.getByText("ge-0/0/1")).toBeVisible();
    await userEvent.click(macToggle);
    await waitFor(() =>
      expect(screen.getByText("02:00:00:00:01:20")).not.toBeVisible()
    );
    expect(screen.getByText("ge-0/0/1")).toBeVisible();
  });
});
