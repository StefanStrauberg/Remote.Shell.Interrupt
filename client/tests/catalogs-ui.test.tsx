// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import WorkflowsPage from "../src/features/Workflows/WorkflowsPage";
import ClientsDashboard from "../src/features/Clients/List/ClientsDashboard";
import { workflowsApi } from "../src/features/Workflows/api/workflowsApi";
import { clientsApi } from "../src/features/Clients/api/clientsApi";
import { renderApp } from "./renderApp";

const pagination = {
  TotalCount: 13,
  PageSize: 12,
  CurrentPage: 1,
  TotalPages: 2,
  HasNext: true,
  HasPrevious: false,
};
const workflowPage: Awaited<ReturnType<typeof workflowsApi.list>> = {
  data: [
    {
      id: "workflow-1",
      name: "Device discovery",
      version: 3,
      status: "Published",
      nodeCount: 5,
      edgeCount: 4,
    },
  ],
  pagination,
};
const clientPage: Awaited<ReturnType<typeof clientsApi.list>> = {
  data: [
    {
      id: "client-1",
      idClient: 42,
      name: "Example customer",
      nrDogovor: "C-42",
      contactT: "Operator",
      telephoneT: "",
      emailT: "ops@example.com",
      working: true,
      antiDDOS: false,
    },
  ],
  pagination,
};

describe("workflow catalog", () => {
  it("renders workflow lifecycle status and editor destinations", async () => {
    vi.spyOn(workflowsApi, "list").mockResolvedValue(workflowPage);
    renderApp(<WorkflowsPage />);
    expect(await screen.findByText("Device discovery")).toBeVisible();
    expect(screen.getByText("Published")).toBeVisible();
    expect(screen.getByRole("link", { name: /Open workflow/ })).toHaveAttribute(
      "href",
      "/admin/workflows/workflow-1"
    );
    expect(screen.getByRole("link", { name: "New workflow" })).toHaveAttribute(
      "href",
      "/admin/workflows/new"
    );
  });
  it("retries a failed request through the Retry button", async () => {
    const list = vi
      .spyOn(workflowsApi, "list")
      .mockRejectedValueOnce(new Error("Temporary failure"))
      .mockResolvedValue(workflowPage);
    renderApp(<WorkflowsPage />);
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Temporary failure"
    );
    await userEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(await screen.findByText("Device discovery")).toBeVisible();
    expect(list).toHaveBeenCalledTimes(2);
  });
  it("resets the selected page when searching and explains empty results", async () => {
    const list = vi.spyOn(workflowsApi, "list").mockResolvedValue(workflowPage);
    renderApp(<WorkflowsPage />);
    await screen.findByText("Device discovery");
    await userEvent.click(screen.getByRole("button", { name: "Go to page 2" }));
    await waitFor(() => expect(list).toHaveBeenLastCalledWith(2, ""));
    list.mockResolvedValue({
      data: [],
      pagination: { ...pagination, TotalPages: 0, TotalCount: 0 },
    });
    await userEvent.type(screen.getByLabelText("Search workflows"), "missing");
    await waitFor(() => expect(list).toHaveBeenLastCalledWith(1, "missing"));
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "No workflows found"
    );
  });
});

describe("customer directory", () => {
  it("switches between grid and list while preserving the customer", async () => {
    vi.spyOn(clientsApi, "list").mockResolvedValue(clientPage);
    renderApp(<ClientsDashboard />);
    await screen.findByText("Example customer");
    await userEvent.click(screen.getByRole("button", { name: "list view" }));
    expect(screen.getByRole("button", { name: "list view" })).toHaveAttribute(
      "aria-pressed",
      "true"
    );
    expect(screen.getByText("Example customer")).toBeVisible();
    await userEvent.click(screen.getByRole("button", { name: "grid view" }));
    expect(screen.getByRole("button", { name: "grid view" })).toHaveAttribute(
      "aria-pressed",
      "true"
    );
  });
  it("toggles sorting and returns to page one", async () => {
    const list = vi.spyOn(clientsApi, "list").mockResolvedValue(clientPage);
    renderApp(<ClientsDashboard />);
    await screen.findByText("Example customer");
    await userEvent.click(screen.getByRole("button", { name: "Go to page 2" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({ pagination: { pageNumber: 2, pageSize: 12 } })
      )
    );
    await userEvent.click(await screen.findByRole("button", { name: /Sort/ }));
    await userEvent.click(
      screen.getByRole("menuitem", { name: "Client Name" })
    );
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          pagination: { pageNumber: 1, pageSize: 12 },
          orderBy: { property: "name", descending: true },
        })
      )
    );
    await userEvent.click(await screen.findByRole("button", { name: /Sort/ }));
    await userEvent.click(screen.getByRole("menuitem", { name: "Client ID" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          orderBy: { property: "idClient", descending: false },
        })
      )
    );
  });
});
