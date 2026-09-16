import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Divider,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useDesigner } from "../state/designerContext";
import {
  nodeTypes,
  parseObject,
  WorkflowNode,
} from "../../domain/workflow/model";
import { defaultConfigForType } from "../../domain/workflow/defaults";

const hints: Record<string, string> = {
  Start: "The entry point of the workflow.",
  End: "Finishes the run and returns the context variables.",
  Join: "Continues the selected branch. This node does not wait for parallel branches.",
  Decision:
    'Reads variable; mode is "value" or "firmware-major". Edges match the result, then DEFAULT or an unconditional edge.',
  SetVariable: "Sets a context key (name) to a JSON value.",
  SnmpGet:
    "Reads an OID from the run's target device and writes it to output. toHex is optional.",
  SnmpWalk:
    "Returns an array of {oid, data}. Set output, toHex and repetitions as needed.",
  Script:
    "Runs function execute(input, context) on the server. Return a value or {success, decision, outputs, error}.",
  SaveNetworkDevice:
    "Saves a device to the database. Reads input.vendor, device.name, device.generalInformation and network.ports; returns device.id.",
};
export function Inspector() {
  const d = useDesigner();
  return (
    <Box
      sx={{
        p: 2,
        minWidth: 0,
        overflow: "auto",
        maxHeight: { lg: 680 },
        borderLeft: { lg: 1 },
        borderColor: "divider",
      }}
    >
      <Typography variant="h6" mb={2}>
        Properties
      </Typography>
      <TextField
        select
        fullWidth
        size="small"
        label="Find node"
        value={d.selectedNodeId ?? ""}
        onChange={(e) => d.selectNode(e.target.value || null)}
        sx={{ mb: 2 }}
      >
        <MenuItem value="">Workflow properties</MenuItem>
        {d.workflow.nodes.map((node) => (
          <MenuItem key={node.id} value={node.id}>
            {node.name} ({node.type})
          </MenuItem>
        ))}
      </TextField>
      {d.selectedNode ? (
        <NodeEditor key={d.selectedNode.id} node={d.selectedNode} />
      ) : d.selectedEdge ? (
        <EdgeEditor key={d.selectedEdge.id} />
      ) : (
        <Stack spacing={2}>
          <TextField
            label="Workflow name"
            value={d.workflow.name}
            disabled={d.readOnly}
            onChange={(e) => d.updateWorkflow({ name: e.target.value })}
          />
          <TextField
            label="Version"
            type="number"
            value={d.workflow.version}
            disabled={d.readOnly}
            onChange={(e) =>
              d.updateWorkflow({ version: Number(e.target.value) })
            }
          />
          <Alert severity="info">
            Select a node or an edge label to edit it. Drag from the right port
            to a left port to connect nodes.
          </Alert>
          <Typography variant="body2" color="text.secondary">
            Drag the background to pan. Ctrl + wheel zooms. Fit shows the entire
            graph.
          </Typography>
        </Stack>
      )}
    </Box>
  );
}
function NodeEditor({ node }: { node: WorkflowNode }) {
  const d = useDesigner();
  const [configText, setConfigText] = useState(
    JSON.stringify(node.config, null, 2)
  );
  const [error, setError] = useState("");
  const [editingConfig, setEditingConfig] = useState(false);
  const config = (key: string, value: unknown) =>
    d.updateNode(node.id, { config: { ...node.config, [key]: value } });
  return (
    <Stack spacing={2}>
      <TextField
        label="Name"
        value={node.name}
        disabled={d.readOnly}
        onChange={(e) => d.updateNode(node.id, { name: e.target.value })}
      />
      <TextField
        label="Type"
        select
        value={node.type}
        disabled={d.readOnly}
        onChange={(e) => {
          const type = e.target.value as WorkflowNode["type"];
          d.updateNode(node.id, { type, config: defaultConfigForType(type) });
        }}
      >
        {nodeTypes.map((type) => (
          <MenuItem key={type} value={type}>
            {type}
          </MenuItem>
        ))}
      </TextField>
      <TextField
        label="Key"
        value={node.key ?? ""}
        disabled={d.readOnly}
        onChange={(e) => d.updateNode(node.id, { key: e.target.value || null })}
      />
      <Typography variant="body2" color="text.secondary">
        {hints[node.type]}
      </Typography>
      {node.type === "Script" && (
        <>
          <Alert severity="warning">
            This code runs on the server, with access to this workflow&apos;s
            context data (SNMP results, variables from earlier nodes), the next
            time the workflow executes. Only paste JavaScript you trust.
          </Alert>
          <TextField
            label="Input context path"
            value={String(node.config.input ?? "")}
            disabled={d.readOnly}
            onChange={(e) => config("input", e.target.value)}
            helperText="Leave empty to pass the whole context."
          />
          <TextField
            label="Output context path"
            value={String(node.config.output ?? "")}
            disabled={d.readOnly}
            onChange={(e) => config("output", e.target.value)}
          />
          <TextField
            label="Timeout (ms)"
            type="number"
            value={Number(node.config.timeoutMs ?? 1500)}
            disabled={d.readOnly}
            onChange={(e) => config("timeoutMs", Number(e.target.value))}
          />
          <TextField
            label="JavaScript"
            multiline
            minRows={12}
            maxRows={24}
            value={String(node.config.scriptSource ?? "")}
            disabled={d.readOnly}
            onChange={(e) => config("scriptSource", e.target.value)}
            slotProps={{
              input: { sx: { fontFamily: "monospace", fontSize: 12 } },
              htmlInput: { spellCheck: false },
            }}
          />
        </>
      )}
      {node.type !== "Script" && (
        <>
          <Box
            component="pre"
            sx={{
              m: 0,
              p: 1.5,
              bgcolor: "background.default",
              overflow: "auto",
              fontSize: 12,
            }}
          >
            {JSON.stringify(node.config, null, 2)}
          </Box>
          <Button
            disabled={d.readOnly}
            onClick={() => {
              setConfigText(JSON.stringify(node.config, null, 2));
              setError("");
              setEditingConfig(true);
            }}
          >
            Edit configuration
          </Button>
          <Dialog
            open={editingConfig}
            onClose={() => setEditingConfig(false)}
            fullWidth
            maxWidth="sm"
          >
            <DialogTitle>Node configuration</DialogTitle>
            <DialogContent>
              <TextField
                autoFocus
                fullWidth
                label="Config JSON"
                multiline
                minRows={10}
                value={configText}
                onChange={(e) => setConfigText(e.target.value)}
                error={!!error}
                helperText={error}
                sx={{ mt: 1 }}
                slotProps={{
                  input: { sx: { fontFamily: "monospace", fontSize: 13 } },
                }}
              />
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setEditingConfig(false)}>Cancel</Button>
              <Button
                variant="contained"
                onClick={() => {
                  try {
                    d.updateNode(node.id, { config: parseObject(configText) });
                    setEditingConfig(false);
                  } catch (e) {
                    setError(e instanceof Error ? e.message : "Invalid JSON");
                  }
                }}
              >
                Apply
              </Button>
            </DialogActions>
          </Dialog>
        </>
      )}{" "}
      <Divider />
      <Stack direction="row" gap={1} flexWrap="wrap">
        <Button disabled={d.readOnly} onClick={() => d.duplicateNode(node.id)}>
          Duplicate
        </Button>
        <Button disabled={d.readOnly} onClick={() => d.setStartNode(node.id)}>
          Make Start
        </Button>
        <Button
          disabled={d.readOnly}
          onClick={() => d.insertNodeBefore(node.id)}
        >
          Insert before
        </Button>
        <Button
          disabled={d.readOnly}
          onClick={() => d.insertNodeAfter(node.id)}
        >
          Insert after
        </Button>
        <Button
          color="error"
          disabled={d.readOnly}
          onClick={() => d.deleteNode(node.id)}
        >
          Delete node
        </Button>
        <Button
          color="error"
          disabled={d.readOnly}
          onClick={() => d.deleteNode(node.id, true)}
        >
          Delete & reconnect
        </Button>
      </Stack>
    </Stack>
  );
}
function EdgeEditor() {
  const d = useDesigner();
  const edge = d.selectedEdge!;
  return (
    <Stack spacing={2}>
      <Typography variant="body2">
        {d.workflow.nodes.find((n) => n.id === edge.fromNodeId)?.name} →{" "}
        {d.workflow.nodes.find((n) => n.id === edge.toNodeId)?.name}
      </Typography>
      <TextField
        label="Condition"
        value={edge.condition ?? ""}
        disabled={d.readOnly}
        onChange={(e) =>
          d.updateEdge(edge.id, { condition: e.target.value || null })
        }
        helperText="Exact decision, DEFAULT, or empty for fallback."
      />
      <TextField
        label="Priority"
        type="number"
        value={edge.priority}
        disabled={d.readOnly}
        onChange={(e) =>
          d.updateEdge(edge.id, { priority: Number(e.target.value) })
        }
        helperText="Lower numbers are checked first."
      />
      <Button
        disabled={d.readOnly}
        onClick={() => d.insertNodeIntoEdge(edge.id)}
      >
        Insert node into edge
      </Button>
      <Button
        color="error"
        disabled={d.readOnly}
        onClick={() => d.deleteEdge(edge.id)}
      >
        Delete edge
      </Button>
    </Stack>
  );
}
