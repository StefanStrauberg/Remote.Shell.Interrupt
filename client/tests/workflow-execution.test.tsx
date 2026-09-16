// @vitest-environment jsdom
import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { ExecutionPanel } from "../src/features/Workflows/designer/components/ExecutionPanel";
import { DesignerProvider } from "../src/features/Workflows/designer/state/DesignerProvider";
import { useDesigner } from "../src/features/Workflows/designer/state/designerContext";
import { createWorkflow } from "../src/features/Workflows/domain/workflow/defaults";
import { ExecutionResult } from "../src/features/Workflows/domain/workflow/model";
import { workflowsApi } from "../src/features/Workflows/api/workflowsApi";
import { networkDeviceKeys } from "../src/features/NetworkDevices/api/networkDevicesQueries";
import { deferred, renderApp } from "./renderApp";

const success: ExecutionResult = {
  success: true,
  error: null,
  steps: [
    {
      nodeName: "Read interfaces",
      nodeType: "SnmpWalk",
      outputs: { count: 2 },
      decision: null,
      candidateEdges: [],
      why: "Read complete",
      nextNodeName: "Save device",
      logs: ["Two interfaces received"],
    },
  ],
  finalVariables: { "interfaces.count": 2 },
};

function DesignerProbe() {
  const d = useDesigner();
  return (
    <>
      <button onClick={() => d.updateWorkflow({ name: "Edited" })}>
        Edit graph
      </button>
      <output aria-label="Designer busy">{String(d.busy)}</output>
    </>
  );
}

function setup(persisted = true) {
  const graph = createWorkflow();
  const app = renderApp(
    <DesignerProvider initial={graph}>
      <DesignerProbe />
      <ExecutionPanel persisted={persisted} />
    </DesignerProvider>
  );
  fireEvent.change(screen.getByLabelText("Device IP address"), {
    target: { value: " 192.0.2.10 " },
  });
  fireEvent.change(screen.getByLabelText("SNMP community"), {
    target: { value: "test-community" },
  });
  return { ...app, graph };
}

describe("workflow execution", () => {
  it("requires saving a new workflow before execution", () => {
    const execute = vi
      .spyOn(workflowsApi, "execute")
      .mockResolvedValue(success);
    setup(false);
    expect(
      screen.getByText("Save this workflow before running it.")
    ).toBeVisible();
    expect(screen.getByRole("button", { name: "Run workflow" })).toBeDisabled();
    expect(execute).not.toHaveBeenCalled();
  });

  it("blocks execution when the saved graph has unsaved changes", async () => {
    setup();
    await userEvent.click(screen.getByRole("button", { name: "Edit graph" }));
    expect(screen.getByRole("button", { name: "Run workflow" })).toBeDisabled();
    expect(
      screen.getByText("Save this workflow before running it.")
    ).toBeVisible();
  });

  it.each(["[]", "null", '"text"', "{"])(
    "rejects non-object or malformed input %s before calling the API",
    async (input) => {
      const execute = vi
        .spyOn(workflowsApi, "execute")
        .mockResolvedValue(success);
      setup();
      fireEvent.change(screen.getByLabelText("Input variables (JSON)"), {
        target: { value: input },
      });
      await userEvent.click(
        screen.getByRole("button", { name: "Run workflow" })
      );
      expect(await screen.findByRole("alert")).toBeVisible();
      expect(execute).not.toHaveBeenCalled();
      expect(screen.getByLabelText("Designer busy")).toHaveTextContent("false");
    }
  );

  it("sends parsed input, locks the form, renders the trace and refreshes device data", async () => {
    const pending = deferred<ExecutionResult>();
    const execute = vi
      .spyOn(workflowsApi, "execute")
      .mockReturnValue(pending.promise);
    const { queryClient, graph } = setup();
    queryClient.setQueryData([...networkDeviceKeys.all, "cached"], []);
    await userEvent.click(screen.getByRole("button", { name: "Run workflow" }));
    expect(execute).toHaveBeenCalledExactlyOnceWith(
      graph.id,
      {
        host: "192.0.2.10",
        community: "test-community",
        input: { "input.vendor": "Huawei" },
      },
      expect.any(AbortSignal)
    );
    expect(screen.getByLabelText("Device IP address")).toBeDisabled();
    expect(screen.getByLabelText("SNMP community")).toHaveAttribute(
      "type",
      "password"
    );
    expect(screen.getByLabelText("Designer busy")).toHaveTextContent("true");
    await act(async () => pending.resolve(success));
    expect(await screen.findByText(/Workflow completed/)).toHaveTextContent(
      "1 recorded steps"
    );
    await userEvent.click(
      screen.getByRole("button", { name: /Read interfaces/ })
    );
    expect(screen.getByText("Two interfaces received")).toBeVisible();
    await userEvent.click(
      screen.getByRole("button", { name: "Final variables" })
    );
    expect(screen.getByText(/"interfaces.count": 2/)).toBeVisible();
    expect(screen.getByLabelText("Designer busy")).toHaveTextContent("false");
    expect(
      queryClient.getQueryState([...networkDeviceKeys.all, "cached"])
        ?.isInvalidated
    ).toBe(true);
  });

  it("shows a workflow failure even when the HTTP request succeeds", async () => {
    vi.spyOn(workflowsApi, "execute").mockResolvedValue({
      ...success,
      success: false,
      error: "No matching edge",
    });
    setup();
    await userEvent.click(screen.getByRole("button", { name: "Run workflow" }));
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "No matching edge"
    );
    expect(screen.queryByText(/Workflow completed/)).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Run workflow" })).toBeEnabled();
  });

  it("recovers from a transport failure and clears the error on retry", async () => {
    const execute = vi
      .spyOn(workflowsApi, "execute")
      .mockRejectedValueOnce(new Error("Server unavailable"))
      .mockResolvedValue(success);
    setup();
    await userEvent.click(screen.getByRole("button", { name: "Run workflow" }));
    expect(await screen.findByText("Server unavailable")).toBeVisible();
    await userEvent.click(screen.getByRole("button", { name: "Run workflow" }));
    expect(await screen.findByText(/Workflow completed/)).toBeVisible();
    expect(screen.queryByText("Server unavailable")).not.toBeInTheDocument();
    expect(execute).toHaveBeenCalledTimes(2);
  });

  it("cancels the request, explains persisted side effects and unlocks the designer", async () => {
    const pending = deferred<ExecutionResult>();
    const execute = vi
      .spyOn(workflowsApi, "execute")
      .mockReturnValue(pending.promise);
    setup();
    await userEvent.click(screen.getByRole("button", { name: "Run workflow" }));
    const signal = execute.mock.calls[0][2]!;
    await userEvent.click(
      screen.getByRole("button", { name: "Cancel request" })
    );
    expect(signal.aborted).toBe(true);
    await act(async () => pending.reject(new Error("Aborted")));
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Request cancelled. A device already saved by the workflow remains in the database."
    );
    expect(screen.getByLabelText("Designer busy")).toHaveTextContent("false");
    expect(screen.getByRole("button", { name: "Run workflow" })).toBeEnabled();
  });

  it("aborts an in-flight request on unmount", async () => {
    const pending = deferred<ExecutionResult>();
    const execute = vi
      .spyOn(workflowsApi, "execute")
      .mockReturnValue(pending.promise);
    const { unmount } = setup();
    await userEvent.click(screen.getByRole("button", { name: "Run workflow" }));
    const signal = execute.mock.calls[0][2]!;
    unmount();
    expect(signal.aborted).toBe(true);
    await act(async () => pending.reject(new Error("Aborted")));
    await waitFor(() => expect(execute).toHaveBeenCalledTimes(1));
  });
});
