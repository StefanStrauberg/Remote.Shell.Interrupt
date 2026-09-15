import { useEffect, useRef, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Box,
  Button,
  CircularProgress,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { networkDeviceKeys } from "@/features/NetworkDevices/api/networkDevicesQueries";
import { workflowsApi } from "../../api/workflowsApi";
import { ExecutionResult, parseObject } from "../../domain/workflow/model";
import { useDesigner } from "../state/designerContext";
export function ExecutionPanel({ persisted }: { persisted: boolean }) {
  const d = useDesigner();
  const cache = useQueryClient();
  const [host, setHost] = useState("");
  const [community, setCommunity] = useState("");
  const [input, setInput] = useState('{"input.vendor":"Huawei"}');
  const [result, setResult] = useState<ExecutionResult | null>(null);
  const [error, setError] = useState("");
  const [running, setRunning] = useState(false);
  const request = useRef<AbortController | null>(null);
  useEffect(() => () => request.current?.abort(), []);
  const run = async () => {
    let variables: Record<string, unknown>;
    try {
      variables = parseObject(input);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Invalid input");
      return;
    }
    const controller = new AbortController();
    request.current = controller;
    setRunning(true);
    d.setBusy(true);
    setError("");
    setResult(null);
    try {
      setResult(
        await workflowsApi.execute(
          d.workflow.id,
          { host: host.trim(), community, input: variables },
          controller.signal
        )
      );
    } catch (e) {
      setError(
        controller.signal.aborted
          ? "Request cancelled. A device already saved by the workflow remains in the database."
          : e instanceof Error
            ? e.message
            : "Execution failed."
      );
    } finally {
      setRunning(false);
      d.setBusy(false);
      request.current = null;
      void cache.invalidateQueries({ queryKey: networkDeviceKeys.all });
    }
  };
  return (
    <Stack spacing={2}>
      <Typography variant="h6">Run on device</Typography>
      {(!persisted || d.dirty) && (
        <Alert severity="info">Save this workflow before running it.</Alert>
      )}
      {d.workflow.nodes.some((n) => n.type === "SaveNetworkDevice") && (
        <Alert severity="warning">
          This workflow can save a network device to the database.
        </Alert>
      )}
      <Stack direction={{ xs: "column", md: "row" }} gap={2}>
        <TextField
          label="Device IP address"
          value={host}
          disabled={d.busy}
          onChange={(e) => setHost(e.target.value)}
          placeholder="192.168.101.8"
        />
        <TextField
          label="SNMP community"
          type="password"
          value={community}
          disabled={d.busy}
          onChange={(e) => setCommunity(e.target.value)}
          autoComplete="off"
        />
        <Button
          variant="contained"
          disabled={
            !persisted || d.dirty || d.busy || !host.trim() || !community
          }
          onClick={run}
        >
          {running ? <CircularProgress size={20} /> : "Run workflow"}
        </Button>
        {running && (
          <Button color="warning" onClick={() => request.current?.abort()}>
            Cancel request
          </Button>
        )}
      </Stack>
      <TextField
        label="Input variables (JSON)"
        multiline
        minRows={3}
        value={input}
        disabled={d.busy}
        onChange={(e) => setInput(e.target.value)}
        helperText='Use context keys such as "input.vendor".'
        slotProps={{ input: { sx: { fontFamily: "monospace" } } }}
      />
      {running && (
        <Alert severity="info">
          Executing the saved graph on the server. The trace will appear when
          the request finishes.
        </Alert>
      )}
      {error && <Alert severity="error">{error}</Alert>}
      {result && (
        <>
          <Alert severity={result.success ? "success" : "error"}>
            {result.success
              ? "Workflow completed."
              : result.error || "Workflow failed."}{" "}
            {result.steps.length} recorded steps.
          </Alert>
          {result.steps.map((step, index) => (
            <Accordion key={index} disableGutters>
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography variant="body2">
                  {index + 1}. {step.nodeName} · {step.nodeType}
                  {step.decision ? ` → ${step.decision}` : ""}
                </Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Typography variant="body2" mb={1}>
                  {step.why}
                </Typography>
                {step.nextNodeName && (
                  <Typography variant="body2">
                    Next: {step.nextNodeName}
                  </Typography>
                )}
                {step.logs.length > 0 && (
                  <Box
                    component="pre"
                    sx={{ whiteSpace: "pre-wrap", overflowWrap: "anywhere" }}
                  >
                    {step.logs.join("\n")}
                  </Box>
                )}
                <Typography variant="subtitle2">Outputs</Typography>
                <Box
                  component="pre"
                  sx={{ overflow: "auto", maxHeight: 320, fontSize: 12 }}
                >
                  {JSON.stringify(step.outputs, null, 2)}
                </Box>
                {step.candidateEdges.length > 0 && (
                  <>
                    <Typography variant="subtitle2">Candidate edges</Typography>
                    <Box
                      component="pre"
                      sx={{ overflow: "auto", fontSize: 12 }}
                    >
                      {JSON.stringify(step.candidateEdges, null, 2)}
                    </Box>
                  </>
                )}
              </AccordionDetails>
            </Accordion>
          ))}
          <Accordion>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              Final variables
            </AccordionSummary>
            <AccordionDetails>
              <Box
                component="pre"
                sx={{ overflow: "auto", maxHeight: 400, fontSize: 12 }}
              >
                {JSON.stringify(result.finalVariables, null, 2)}
              </Box>
            </AccordionDetails>
          </Accordion>
        </>
      )}
    </Stack>
  );
}
