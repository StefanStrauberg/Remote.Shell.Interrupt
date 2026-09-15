import { useCallback, useEffect, useRef, useState } from "react";
import {
  useBeforeUnload,
  useBlocker,
  useLocation,
  useNavigate,
  useParams,
} from "react-router";
import { useQueryClient } from "@tanstack/react-query";
import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Paper,
  Stack,
  Tab,
  Tabs,
  Typography,
} from "@mui/material";
import { alpha } from "@mui/material/styles";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import SaveIcon from "@mui/icons-material/Save";
import { toast } from "react-toastify";
import PageHeader from "@/app/shared/components/PageHeader";
import { routes } from "@/app/router/paths";
import { useWorkflow, workflowKeys } from "./api/workflowsQueries";
import { workflowsApi } from "./api/workflowsApi";
import { createWorkflow } from "./domain/workflow/defaults";
import { importAsDraft, validateWorkflow } from "./domain/workflow/graph";
import { nodeTypes, WorkflowDefinition } from "./domain/workflow/model";
import { DesignerProvider } from "./designer/state/DesignerProvider";
import { useDesigner } from "./designer/state/designerContext";
import { WorkflowCanvas } from "./designer/components/WorkflowCanvas";
import { Inspector } from "./designer/components/Inspector";
import { ExecutionPanel } from "./designer/components/ExecutionPanel";
import "./designer.css";

export default function WorkflowEditorPage() {
  const { id } = useParams();
  const location = useLocation();
  const query = useWorkflow(id);
  if (id && query.isPending)
    return <CircularProgress aria-label="Loading workflow" />;
  if (id && query.isError)
    return (
      <Alert
        severity="error"
        action={<Button onClick={() => query.refetch()}>Retry</Button>}
      >
        {query.error.message}
      </Alert>
    );
  if (id && !query.data) return null;
  return (
    <LoadedEditor
      key={id ?? location.key}
      initial={query.data}
      draft={(location.state as { draft?: WorkflowDefinition } | null)?.draft}
    />
  );
}
function LoadedEditor({
  initial,
  draft,
}: {
  initial?: WorkflowDefinition;
  draft?: WorkflowDefinition;
}) {
  const [graph] = useState(() => initial ?? draft ?? createWorkflow());
  return (
    <DesignerProvider initial={graph}>
      <Editor persisted={!!initial} />
    </DesignerProvider>
  );
}
function Editor({ persisted }: { persisted: boolean }) {
  const d = useDesigner();
  const navigate = useNavigate();
  const cache = useQueryClient();
  const [tab, setTab] = useState(0);
  const [error, setError] = useState("");
  const [issues, setIssues] = useState<string[] | null>(null);
  const [confirmation, setConfirmation] = useState<
    "publish" | "archive" | "remove" | null
  >(null);
  const bypassBlock = useRef(false);
  const fileInput = useRef<HTMLInputElement>(null);
  const blocker = useBlocker(() => !bypassBlock.current && (d.dirty || d.busy));
  useBeforeUnload(
    useCallback(
      (event) => {
        if (d.dirty || d.busy) {
          event.preventDefault();
          event.returnValue = "";
        }
      },
      [d.dirty, d.busy]
    )
  );
  useEffect(() => {
    const handleKey = (event: KeyboardEvent) => {
      if (
        !(event.ctrlKey || event.metaKey) ||
        (event.target as HTMLElement).closest(
          "input, textarea, [contenteditable=true]"
        )
      )
        return;
      if (event.key.toLowerCase() === "z") {
        event.preventDefault();
        if (event.shiftKey) d.redo();
        else d.undo();
      }
      if (event.key.toLowerCase() === "y") {
        event.preventDefault();
        d.redo();
      }
    };
    window.addEventListener("keydown", handleKey);
    return () => window.removeEventListener("keydown", handleKey);
  }, [d]);
  const save = async () => {
    const errors = validateWorkflow(d.workflow);
    setIssues(errors);
    if (errors.length) return;
    d.setBusy(true);
    setError("");
    try {
      if (persisted) {
        await workflowsApi.update(d.workflow);
        d.replaceSaved(d.workflow);
        cache.setQueryData(workflowKeys.detail(d.workflow.id), d.workflow);
      } else {
        const saved = await workflowsApi.create(d.workflow);
        bypassBlock.current = true;
        if (saved) {
          cache.setQueryData(workflowKeys.detail(saved.id), saved);
          navigate(routes.workflow(saved.id), { replace: true });
        } else {
          toast.info(
            "Workflow created. Open it from the list to continue editing."
          );
          navigate(routes.adminWorkflows, { replace: true });
        }
      }
      void cache.invalidateQueries({ queryKey: workflowKeys.all });
      toast.success("Workflow saved.");
    } catch (e) {
      setError(e instanceof Error ? e.message : "Could not save workflow.");
    } finally {
      d.setBusy(false);
    }
  };
  const changeLifecycle = async () => {
    if (!confirmation) return;
    const action = confirmation;
    setConfirmation(null);
    setError("");
    d.setBusy(true);
    try {
      await workflowsApi[action](d.workflow.id);
      if (action === "remove") {
        bypassBlock.current = true;
        navigate(routes.adminWorkflows, { replace: true });
      } else {
        const graph: WorkflowDefinition = {
          ...d.workflow,
          status: action === "publish" ? "Published" : "Archived",
        };
        d.replaceSaved(graph);
        cache.setQueryData(workflowKeys.detail(graph.id), graph);
      }
      void cache.invalidateQueries({ queryKey: workflowKeys.all });
    } catch (e) {
      setError(e instanceof Error ? e.message : "Operation failed.");
    } finally {
      d.setBusy(false);
    }
  };
  const copy = () => {
    const graph = importAsDraft(d.workflow);
    graph.name += " copy";
    graph.version += 1;
    navigate(routes.createWorkflow, { state: { draft: graph } });
  };
  const exportGraph = () => {
    const url = URL.createObjectURL(
      new Blob([JSON.stringify(d.workflow, null, 2)], {
        type: "application/json",
      })
    );
    const link = document.createElement("a");
    link.href = url;
    link.download = `${d.workflow.name.replace(/[^\p{L}\p{N}_-]/gu, "_")}.json`;
    link.click();
    URL.revokeObjectURL(url);
  };
  return (
    <Box>
      <Button onClick={() => navigate(routes.adminWorkflows)} sx={{ mb: 2 }}>
        ← Workflows
      </Button>
      <PageHeader
        title={d.workflow.name || "Untitled workflow"}
        description="Build and run a device workflow."
        icon={AccountTreeIcon}
        action={
          <Stack direction="row" alignItems="center" gap={1}>
            <Chip
              label={persisted ? d.workflow.status : "New draft"}
              color={d.workflow.status === "Published" ? "success" : "default"}
            />
            {d.dirty && <Chip label="Unsaved" color="warning" size="small" />}
            <Button
              variant="contained"
              startIcon={<SaveIcon />}
              disabled={d.readOnly || (persisted && !d.dirty)}
              onClick={save}
            >
              Save
            </Button>
          </Stack>
        }
      />
      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError("")}>
          {error}
        </Alert>
      )}
      {d.workflow.status !== "Draft" && (
        <Alert severity="info" sx={{ mb: 2 }}>
          {d.workflow.status} graphs are read-only. Create a draft copy to make
          changes.
        </Alert>
      )}
      <Paper variant="outlined" sx={{ mb: 2, p: 1 }}>
        <Stack direction="row" gap={0.5} flexWrap="wrap">
          <Button disabled={!d.canUndo} onClick={d.undo}>
            Undo
          </Button>
          <Button disabled={!d.canRedo} onClick={d.redo}>
            Redo
          </Button>
          <Button
            disabled={d.busy}
            onClick={() => setIssues(validateWorkflow(d.workflow))}
          >
            Validate
          </Button>
          <Button disabled={d.readOnly} onClick={d.toggleSnap}>
            Snap: {d.snapEnabled ? "on" : "off"}
          </Button>
          <Button disabled={d.readOnly} onClick={d.autoLayout}>
            Auto layout
          </Button>
          <Button onClick={d.clearSelection}>Workflow properties</Button>
          <Button disabled={d.busy} onClick={exportGraph}>
            Export JSON
          </Button>
          <Button disabled={d.busy} onClick={() => fileInput.current?.click()}>
            Import as draft
          </Button>
          <Button disabled={d.busy} onClick={copy}>
            Create draft copy
          </Button>
          {persisted && (
            <>
              <Button
                disabled={d.busy || d.dirty || d.workflow.status !== "Draft"}
                onClick={() => setConfirmation("publish")}
              >
                Publish
              </Button>
              <Button
                disabled={d.busy || d.dirty || d.workflow.status === "Archived"}
                onClick={() => setConfirmation("archive")}
              >
                Archive
              </Button>
              <Button
                color="error"
                disabled={d.busy}
                onClick={() => setConfirmation("remove")}
              >
                Delete
              </Button>
            </>
          )}
        </Stack>
        <input
          ref={fileInput}
          type="file"
          accept=".json,application/json"
          hidden
          onChange={async (event) => {
            const file = event.target.files?.[0];
            event.target.value = "";
            if (!file) return;
            try {
              const draft = importAsDraft(JSON.parse(await file.text()));
              navigate(routes.createWorkflow, { state: { draft } });
            } catch (e) {
              setError(
                e instanceof Error ? e.message : "Invalid workflow file."
              );
            }
          }}
        />
      </Paper>
      {issues && (
        <Alert
          severity={issues.length ? "error" : "success"}
          sx={{ mb: 2 }}
          onClose={() => setIssues(null)}
        >
          {issues.length
            ? issues.map((issue, i) => <div key={i}>{issue}</div>)
            : "Graph validation passed."}
        </Alert>
      )}
      <Paper variant="outlined" sx={{ overflow: "hidden" }}>
        <Tabs value={tab} onChange={(_, value: number) => setTab(value)}>
          <Tab label="Designer" />
          <Tab label="Execution" />
          <Tab label="JSON" />
        </Tabs>
        <Divider />
        <Box
          hidden={tab !== 0}
          className="workflow-designer"
          sx={(theme) => ({
            "--wf-paper": theme.palette.background.paper,
            "--wf-bg": theme.palette.background.default,
            "--wf-line": theme.palette.divider,
            "--wf-text": theme.palette.text.primary,
            "--wf-muted": theme.palette.text.secondary,
            "--wf-accent": theme.palette.primary.main,
            "--wf-accent-soft": theme.palette.primary.light,
            "--wf-green": theme.palette.success.main,
            "--wf-green-soft": alpha(theme.palette.success.main, 0.08),
            "--wf-warning-soft": alpha(theme.palette.warning.main, 0.08),
            "--wf-danger": theme.palette.error.main,
          })}
        >
          <Stack
            direction="row"
            flexWrap="wrap"
            gap={1}
            p={1.5}
            borderBottom={1}
            borderColor="divider"
          >
            {nodeTypes.map((type) => (
              <Button
                key={type}
                size="small"
                variant="outlined"
                disabled={d.readOnly}
                draggable={!d.readOnly}
                onDragStart={(event) =>
                  event.dataTransfer.setData(
                    "application/workflow-node-type",
                    type
                  )
                }
                onClick={() => {
                  const selected =
                    d.selectedNode ??
                    d.workflow.nodes[d.workflow.nodes.length - 1];
                  d.addNode(
                    type,
                    (selected?.positionX ?? 0) + 280,
                    selected?.positionY ?? 120
                  );
                }}
              >
                + {type}
              </Button>
            ))}
          </Stack>
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: { xs: "1fr", lg: "minmax(0, 1fr) 340px" },
            }}
          >
            <WorkflowCanvas />
            <Inspector />
          </Box>
        </Box>
        <Box hidden={tab !== 1} p={2}>
          <ExecutionPanel
            key={JSON.stringify(d.workflow)}
            persisted={persisted}
          />
        </Box>
        {tab === 2 && (
          <Box
            component="pre"
            sx={{ p: 2, m: 0, overflow: "auto", maxHeight: 680, fontSize: 12 }}
          >
            {JSON.stringify(d.workflow, null, 2)}
          </Box>
        )}
      </Paper>
      <Dialog
        open={confirmation !== null}
        onClose={() => setConfirmation(null)}
      >
        <DialogTitle>
          {confirmation === "publish"
            ? "Publish workflow?"
            : confirmation === "archive"
              ? "Archive workflow?"
              : "Delete workflow?"}
        </DialogTitle>
        <DialogContent>
          <Typography>
            {confirmation === "remove"
              ? "This permanently deletes the workflow and its graph."
              : "The saved graph will become read-only. You can create a new draft copy to edit it later."}
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmation(null)}>Cancel</Button>
          <Button
            variant="contained"
            color={confirmation === "remove" ? "error" : "primary"}
            onClick={changeLifecycle}
          >
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog open={blocker.state === "blocked"}>
        <DialogTitle>Leave workflow?</DialogTitle>
        <DialogContent>
          {d.busy
            ? "A request is still running. Leaving closes this editor."
            : "Unsaved changes will be discarded."}
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => blocker.state === "blocked" && blocker.reset()}
          >
            Stay
          </Button>
          <Button
            color="warning"
            onClick={() => blocker.state === "blocked" && blocker.proceed()}
          >
            Leave
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
