// @vitest-environment jsdom
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { toast } from "react-toastify";
import NetworkWorkspace from "../src/features/NetworkDevices/Detail/NetworkWorkspace";
import CopyValue from "../src/app/shared/components/CopyValue";
import MainPageDashboard from "../src/features/MainPage/MainPageDashboard";
import { networkDevicesApi } from "../src/features/NetworkDevices/api/networkDevicesApi";
import { NetworkDevice } from "../src/lib/types/NetworkDevices/NetworkDevice";
import { Port } from "../src/lib/types/NetworkDevices/Port";
import { renderApp } from "./renderApp";

const port: Port = {
  id: "port1",
  interfaceNumber: 1,
  interfaceName: "ge-0/0/1",
  interfaceType: "ethernet",
  interfaceStatus: "up",
  interfaceSpeed: 1e9,
  isAggregated: false,
  description: "Customer access",
  macAddress: "02:00:00:00:00:01",
  macTable: ["02:00:00:00:01:20"],
  vlans: [{ vlanTag: 120, vlanName: "Customer" }],
  aggregatedPorts: [],
  arpTableOfPort: { "02:00:00:00:01:20": ["192.0.2.45"] },
  networkTableOfPort: { "192.0.2.0": "255.255.255.0" },
};
const devices: NetworkDevice[] = [
  {
    id: "router1",
    networkDeviceName: "Core router",
    host: "192.0.2.10",
    typeOfNetworkDevice: "Cisco",
    generalInformation: "Distribution",
    portsOfNetworkDevice: [
      port,
      {
        ...port,
        id: "port2",
        interfaceName: "ae0",
        interfaceStatus: "down",
        isAggregated: true,
        macTable: [],
        aggregatedPorts: [
          { ...port, id: "member1", interfaceName: "ge-0/0/2" },
        ],
      },
    ],
  },
];

describe("engineering interface workspace", () => {
  it("selects an interface and preserves all diagnostic sections", async () => {
    renderApp(<NetworkWorkspace devices={devices} />);
    const detail = within(
      screen.getByRole("complementary", { name: "Interface details" })
    );
    expect(detail.getByText("Customer access")).toBeVisible();
    expect(
      detail.getByRole("link", { name: "120 (Customer)" })
    ).toHaveAttribute("href", "/mainPage?vlan=120");
    await userEvent.click(screen.getByRole("tab", { name: "MAC (1)" }));
    expect(detail.getByText("02:00:00:00:01:20")).toBeVisible();
    await userEvent.click(screen.getByRole("tab", { name: "IP data" }));
    expect(detail.getByText("192.0.2.45")).toBeVisible();
    expect(detail.getByText("255.255.255.0")).toBeVisible();
    await userEvent.click(
      screen.getByRole("button", { name: "Inspect ae0 on Core router" })
    );
    expect(screen.getByRole("tab", { name: "Properties" })).toHaveAttribute(
      "aria-selected",
      "true"
    );
    await userEvent.click(screen.getByRole("tab", { name: "Members (1)" }));
    expect(screen.getByText("ge-0/0/2")).toBeVisible();
  });

  it("filters loaded interfaces without extra API requests and recovers from empty results", async () => {
    const fetch = vi.spyOn(networkDevicesApi, "getById");
    renderApp(<NetworkWorkspace devices={devices} />);
    await userEvent.type(
      screen.getByRole("textbox", { name: "Filter loaded interfaces" }),
      "missing"
    );
    expect(
      screen.getByText("No interfaces match these filters.")
    ).toBeVisible();
    expect(screen.queryByRole("complementary")).not.toBeInTheDocument();
    await userEvent.click(
      screen.getByRole("button", { name: "Clear interface filters" })
    );
    expect(
      screen.getByRole("button", { name: "Inspect ae0 on Core router" })
    ).toBeVisible();
    expect(fetch).not.toHaveBeenCalled();
  });

  it("filters by status and closes and reopens the selected interface", async () => {
    renderApp(<NetworkWorkspace devices={devices} />);
    await userEvent.click(screen.getByRole("combobox", { name: "Status" }));
    await userEvent.click(screen.getByRole("option", { name: "Down" }));
    expect(
      screen.queryByRole("button", { name: "Inspect ge-0/0/1 on Core router" })
    ).not.toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "ae0" })).toBeVisible();
    await userEvent.click(
      screen.getByRole("button", { name: "Close interface details" })
    );
    expect(screen.queryByRole("complementary")).not.toBeInTheDocument();
    await userEvent.click(screen.getByRole("button", { name: "Show details" }));
    expect(screen.getByRole("heading", { name: "ae0" })).toBeVisible();
  });

  it("keeps devices with no interfaces accessible", () => {
    renderApp(
      <NetworkWorkspace
        devices={[{ ...devices[0], portsOfNetworkDevice: [] }]}
      />
    );
    expect(screen.getByText("No ports available")).toBeVisible();
    expect(screen.getByRole("link", { name: "Core router" })).toHaveAttribute(
      "href",
      "/networkDevices/router1"
    );
  });

  it("distinguishes an unknown status from Down", () => {
    renderApp(
      <NetworkWorkspace
        devices={[
          {
            ...devices[0],
            portsOfNetworkDevice: [{ ...port, interfaceStatus: "unknown" }],
          },
        ]}
      />
    );
    expect(screen.getAllByText("UNKNOWN")).toHaveLength(2);
    expect(screen.queryByText("DOWN")).not.toBeInTheDocument();
  });

  it("restores VLAN search from its URL and updates the URL when reset", async () => {
    const fetch = vi
      .spyOn(networkDevicesApi, "getByVlan")
      .mockResolvedValue({ clients: [], networkDevices: devices });
    renderApp(<MainPageDashboard />, "/mainPage?vlan=120");
    expect(screen.getByRole("spinbutton", { name: "VLAN ID" })).toHaveValue(
      120
    );
    await screen.findByRole("table", { name: "Network interfaces" });
    expect(fetch).toHaveBeenCalledWith(120);
    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    expect(screen.getByLabelText("Current location")).toHaveTextContent(
      "/mainPage|null"
    );
    expect(screen.getByText("Start a VLAN search")).toBeVisible();
  });

  it("ignores an invalid VLAN in the URL", () => {
    const fetch = vi.spyOn(networkDevicesApi, "getByVlan");
    renderApp(<MainPageDashboard />, "/mainPage?vlan=4095");
    expect(screen.getByText("Start a VLAN search")).toBeVisible();
    expect(fetch).not.toHaveBeenCalled();
  });
});

describe("copy diagnostic values", () => {
  it("copies the exact value and reports completion", async () => {
    const user = userEvent.setup();
    const write = vi
      .spyOn(navigator.clipboard, "writeText")
      .mockResolvedValue();
    renderApp(<CopyValue value="192.0.2.10" label="device IP" />);
    await user.click(screen.getByRole("button", { name: "Copy device IP" }));
    expect(write).toHaveBeenCalledExactlyOnceWith("192.0.2.10");
  });

  it("reports denied clipboard access without crashing", async () => {
    userEvent.setup();
    vi.spyOn(navigator.clipboard, "writeText").mockRejectedValue(
      new Error("Denied")
    );
    const error = vi.spyOn(toast, "error").mockImplementation(() => "error");
    renderApp(<CopyValue value="192.0.2.10" />);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() =>
      expect(error).toHaveBeenCalledWith(
        "Could not copy. Select the value and copy it manually."
      )
    );
  });
});
