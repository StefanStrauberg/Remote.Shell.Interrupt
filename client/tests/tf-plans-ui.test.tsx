// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { screen } from "@testing-library/react";
import TfPlansDashboard from "../src/features/TfPlans/TfPlansDashboard";
import { tfPlansApi } from "../src/features/TfPlans/api/tfPlansApi";
import { TfPlan } from "../src/lib/types/TfPlans/TfPlan";
import { renderApp } from "./renderApp";

const pagination = {
  TotalCount: 1,
  PageSize: 12,
  CurrentPage: 1,
  TotalPages: 1,
  HasNext: false,
  HasPrevious: false,
};

const plan: TfPlan = {
  id: "plan-1",
  idTfPlan: 7,
  nameTfPlan: "Business 100",
  descTfPlan: "100 Mbps symmetric",
};

describe("tariff plans dashboard", () => {
  it("shows an empty state when there are no plans", async () => {
    vi.spyOn(tfPlansApi, "list").mockResolvedValue({
      data: [],
      pagination: { ...pagination, TotalCount: 0 },
    });
    renderApp(<TfPlansDashboard />);
    expect(await screen.findByText("No tariff plans found")).toBeVisible();
  });

  it("renders plan cards with their description", async () => {
    vi.spyOn(tfPlansApi, "list").mockResolvedValue({
      data: [plan],
      pagination,
    });
    renderApp(<TfPlansDashboard />);
    expect(await screen.findByText("Business 100")).toBeVisible();
    expect(screen.getByText(/100 Mbps symmetric/)).toBeVisible();
    expect(screen.getByText("ID: 7")).toBeVisible();
  });

  it("falls back to a placeholder when a plan has no description", async () => {
    vi.spyOn(tfPlansApi, "list").mockResolvedValue({
      data: [{ ...plan, descTfPlan: undefined }],
      pagination,
    });
    renderApp(<TfPlansDashboard />);
    expect(await screen.findByText(/No description available/)).toBeVisible();
  });

  it("shows a load error", async () => {
    vi.spyOn(tfPlansApi, "list").mockRejectedValue(
      new Error("Service unavailable")
    );
    renderApp(<TfPlansDashboard />);
    const alert = await screen.findByRole("alert");
    expect(alert).toHaveTextContent("Unable to load tariff plans");
    expect(alert).toHaveTextContent("Service unavailable");
  });
});
