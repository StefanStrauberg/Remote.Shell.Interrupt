// @vitest-environment jsdom
import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import ClientsDashboard from "../src/features/Clients/List/ClientsDashboard";
import {
  clientsApi,
  DEFAULT_CLIENT_FILTERS,
} from "../src/features/Clients/api/clientsApi";
import { FilterOperator } from "../src/lib/types/Common/FilterOperator";
import { renderApp } from "./renderApp";

const page: Awaited<ReturnType<typeof clientsApi.list>> = {
  data: [
    {
      id: "client-1",
      idClient: 1042,
      name: "Example customer",
      nrDogovor: "C-1042",
      working: true,
      antiDDOS: false,
      contactT: "NOC",
      telephoneT: "",
      emailT: "noc@example.com",
    },
  ],
  pagination: {
    TotalCount: 13,
    PageSize: 12,
    CurrentPage: 1,
    TotalPages: 2,
    HasNext: true,
    HasPrevious: false,
  },
};

describe("customer filtering integration", () => {
  it("applies normalized filters on submission and resets pagination", async () => {
    const list = vi.spyOn(clientsApi, "list").mockResolvedValue(page);
    renderApp(<ClientsDashboard />);
    await screen.findByText("Example customer");
    await userEvent.click(screen.getByRole("button", { name: "Go to page 2" }));
    await screen.findByRole("textbox", { name: "Organization Name" });
    const count = list.mock.calls.length;
    await userEvent.type(
      screen.getByRole("textbox", { name: "Organization Name" }),
      " Example "
    );
    await userEvent.type(
      screen.getByRole("textbox", { name: "Contract Number" }),
      " C-1042 "
    );
    await userEvent.click(
      screen.getByRole("checkbox", { name: "Active Clients" })
    );
    await userEvent.click(
      screen.getByRole("checkbox", { name: "AntiDDOS Enabled" })
    );
    expect(list).toHaveBeenCalledTimes(count);
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith({
        pagination: { pageNumber: 1, pageSize: 12 },
        orderBy: { property: "name", descending: false },
        filters: [
          {
            PropertyPath: "Working",
            Operator: FilterOperator.Equals,
            Value: "false",
          },
          {
            PropertyPath: "AntiDDOS",
            Operator: FilterOperator.Equals,
            Value: "true",
          },
          {
            PropertyPath: "Name",
            Operator: FilterOperator.Contains,
            Value: "Example",
          },
          {
            PropertyPath: "NrDogovor",
            Operator: FilterOperator.Contains,
            Value: "C-1042",
          },
        ],
      })
    );
    expect(
      await screen.findByRole("textbox", { name: "Organization Name" })
    ).toHaveValue("Example");
  });

  it("keeps filters usable after empty results and restores defaults on reset", async () => {
    const list = vi
      .spyOn(clientsApi, "list")
      .mockResolvedValueOnce(page)
      .mockResolvedValue({
        ...page,
        data: [],
        pagination: { ...page.pagination, TotalCount: 0, TotalPages: 0 },
      });
    renderApp(<ClientsDashboard />);
    await screen.findByText("Example customer");
    await userEvent.type(
      screen.getByRole("textbox", { name: "Organization Name" }),
      "Missing"
    );
    await userEvent.click(screen.getByRole("button", { name: "Apply" }));
    expect(await screen.findByText("No clients found")).toBeVisible();
    expect(
      screen.getByRole("textbox", { name: "Organization Name" })
    ).toHaveValue("Missing");
    await userEvent.click(screen.getByRole("button", { name: "Reset" }));
    await waitFor(() =>
      expect(list).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: DEFAULT_CLIENT_FILTERS,
          pagination: { pageNumber: 1, pageSize: 12 },
        })
      )
    );
    expect(
      await screen.findByRole("textbox", { name: "Organization Name" })
    ).toHaveValue("");
    expect(
      screen.getByRole("checkbox", { name: "Active Clients" })
    ).toBeChecked();
    expect(
      screen.getByRole("checkbox", { name: "AntiDDOS Enabled" })
    ).not.toBeChecked();
  });
});
