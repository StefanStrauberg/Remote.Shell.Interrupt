// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import NetworkDeviceDashboard from "../src/features/NetworkDevices/List/NetworkDeviceDashboard";
import { networkDevicesApi } from "../src/features/NetworkDevices/api/networkDevicesApi";
import { NetworkDevice } from "../src/lib/types/NetworkDevices/NetworkDevice";
import { PagedResponse } from "../src/lib/api/common/paged";
import { deferred, renderApp } from "./renderApp";

const device: NetworkDevice = {
  id: "router-1",
  host: "192.0.2.1",
  networkDeviceName: "Core switch",
  typeOfNetworkDevice: "Cisco",
  generalInformation: "",
  portsOfNetworkDevice: [],
};
const page: PagedResponse<NetworkDevice> = {
  data: [device],
  pagination: {
    TotalCount: 13,
    PageSize: 12,
    CurrentPage: 1,
    TotalPages: 2,
    HasNext: true,
    HasPrevious: false,
  },
};

describe("network device inventory", () => {
  it("shows pending state before the request completes", () => {
    vi.spyOn(networkDevicesApi, "list").mockReturnValue(
      deferred<PagedResponse<NetworkDevice>>().promise
    );
    renderApp(<NetworkDeviceDashboard />);
    expect(screen.getByText("Loading network devices…")).toBeVisible();
    expect(
      screen.queryByText(device.networkDeviceName)
    ).not.toBeInTheDocument();
  });
  it("shows API failure without presenting it as an empty inventory", async () => {
    vi.spyOn(networkDevicesApi, "list").mockRejectedValue(
      new Error("Inventory unavailable")
    );
    renderApp(<NetworkDeviceDashboard />);
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Inventory unavailable"
    );
    expect(
      screen.queryByText("Network devices not found")
    ).not.toBeInTheDocument();
  });
  it("shows an empty inventory without pagination", async () => {
    vi.spyOn(networkDevicesApi, "list").mockResolvedValue({
      data: [],
      pagination: { ...page.pagination, TotalPages: 0, TotalCount: 0 },
    });
    renderApp(<NetworkDeviceDashboard />);
    expect(await screen.findByText("Network devices not found")).toBeVisible();
    expect(
      screen.queryByRole("navigation", { name: "pagination navigation" })
    ).not.toBeInTheDocument();
  });
  it("renders server records with detail links and the total count", async () => {
    vi.spyOn(networkDevicesApi, "list").mockResolvedValue(page);
    renderApp(<NetworkDeviceDashboard />);
    expect(await screen.findByText("Core switch")).toBeVisible();
    expect(screen.getByText("13 network devices found")).toBeVisible();
    expect(screen.getByRole("link", { name: "View" })).toHaveAttribute(
      "href",
      "/networkDevices/router-1"
    );
    expect(screen.getByText("No information")).toBeVisible();
  });
  it("requests the selected page, then resets pagination when filters change", async () => {
    const list = vi.spyOn(networkDevicesApi, "list").mockResolvedValue(page);
    renderApp(<NetworkDeviceDashboard />);
    await screen.findByText("Core switch");
    await userEvent.click(screen.getByRole("button", { name: "Go to page 2" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({ pagination: { pageNumber: 2, pageSize: 12 } })
      )
    );
    await userEvent.type(
      await screen.findByLabelText("Device Name"),
      "  Core  "
    );
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith({
        pagination: { pageNumber: 1, pageSize: 12 },
        orderBy: { property: "host", descending: false },
        filters: [
          {
            PropertyPath: "NetworkDeviceName",
            Operator: "Contains",
            Value: "Core",
          },
        ],
      })
    );
    await screen.findByText("Core switch");
    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: [],
          pagination: { pageNumber: 1, pageSize: 12 },
        })
      )
    );
  });
});
