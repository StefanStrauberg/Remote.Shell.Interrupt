import { WorkflowDefinition, WorkflowNodeType } from "./model";
export const GRID_SIZE = 24;
export const NODE_WIDTH = 220;
export const NODE_HEIGHT = 96;
export function defaultConfigForType(
  type: WorkflowNodeType
): Record<string, unknown> {
  switch (type) {
    case "SetVariable":
      return { name: "input.vendor", value: "Huawei" };
    case "Decision":
      return { variable: "input.vendor", mode: "value" };
    case "SnmpGet":
      return { oid: "1.3.6.1.2.1.1.5.0", output: "device.name", toHex: false };
    case "SnmpWalk":
      return {
        oid: "1.3.6.1.2.1.2.2.1",
        output: "raw.interfaces",
        toHex: false,
        repetitions: 20,
      };
    case "Script":
      return {
        input: "",
        output: "script.result",
        timeoutMs: 1500,
        scriptSource: "function execute(input, context) {\n  return input;\n}",
      };
    default:
      return {};
  }
}
export function createWorkflow(): WorkflowDefinition {
  const start = crypto.randomUUID();
  const end = crypto.randomUUID();
  return {
    id: crypto.randomUUID(),
    name: "New workflow",
    version: 1,
    status: "Draft",
    startNodeId: start,
    nodes: [
      {
        id: start,
        type: "Start",
        name: "Start",
        key: "start",
        config: {},
        positionX: 48,
        positionY: 120,
      },
      {
        id: end,
        type: "End",
        name: "End",
        key: "end",
        config: {},
        positionX: 384,
        positionY: 120,
      },
    ],
    edges: [
      {
        id: crypto.randomUUID(),
        fromNodeId: start,
        toNodeId: end,
        condition: null,
        priority: 1,
      },
    ],
  };
}
