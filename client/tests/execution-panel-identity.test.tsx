// @vitest-environment jsdom
import { afterEach, describe, expect, it } from "vitest";
import { fireEvent, render, screen, cleanup } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createMemoryRouter, RouterProvider } from "react-router";
import { ThemeProvider } from "@mui/material";
import { appTheme } from "../src/app/theme";
import WorkflowEditorPage from "../src/features/Workflows/WorkflowEditorPage";

// Regression coverage for: ExecutionPanel used to be mounted with
// key={JSON.stringify(d.workflow)}, so it was torn down and recreated on
// every single graph edit (adding a node, moving one, renaming the
// workflow...) even while its own tab stayed hidden underneath. That wiped
// out whatever the user had typed into the Run panel (and silently aborted
// an in-flight execution, since its cleanup effect runs on unmount).

afterEach(() => cleanup());

function renderEditor() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  const router = createMemoryRouter(
    [{ path: "/workflows/new", element: <WorkflowEditorPage /> }],
    { initialEntries: ["/workflows/new"] }
  );
  return render(
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={appTheme}>
        <RouterProvider router={router} />
      </ThemeProvider>
    </QueryClientProvider>
  );
}

describe("ExecutionPanel identity across graph edits", () => {
  it("focuses the canvas without changing the graph or losing access to tools", () => {
    renderEditor();
    expect(screen.getByRole("button", { name: "+ Script" })).toBeVisible();
    fireEvent.click(screen.getByRole("button", { name: "Focus canvas" }));
    expect(
      screen.queryByRole("button", { name: "+ Script" })
    ).not.toBeInTheDocument();
    expect(screen.queryByText("Unsaved")).not.toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Show panels" }));
    expect(screen.getByRole("button", { name: "+ Script" })).toBeVisible();
    fireEvent.click(screen.getByRole("button", { name: "More actions" }));
    expect(screen.getByRole("button", { name: "Export JSON" })).toBeVisible();
    expect(
      screen.getByRole("button", { name: "Import as draft" })
    ).toBeVisible();
    expect(
      screen.getByRole("button", { name: "Create draft copy" })
    ).toBeVisible();
  });

  it("keeps its local form state after an edit elsewhere in the designer", () => {
    renderEditor();

    fireEvent.click(screen.getByRole("tab", { name: "Execution" }));
    const host = screen.getByLabelText("Device IP address") as HTMLInputElement;
    fireEvent.change(host, { target: { value: "192.168.101.8" } });
    expect(host.value).toBe("192.168.101.8");

    fireEvent.click(screen.getByRole("tab", { name: "Designer" }));
    fireEvent.click(screen.getByRole("button", { name: "+ Script" }));

    fireEvent.click(screen.getByRole("tab", { name: "Execution" }));
    expect(
      (screen.getByLabelText("Device IP address") as HTMLInputElement).value
    ).toBe("192.168.101.8");
  });
});
