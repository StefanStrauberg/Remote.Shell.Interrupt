import {
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from "@mui/material";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type { PointerEvent as ReactPointerEvent } from "react";
import { NODE_HEIGHT, NODE_WIDTH } from "../../domain/workflow/defaults";
import { nodeTypes, type WorkflowNodeType } from "../../domain/workflow/model";
import { useDesigner } from "../state/designerContext";
import { EdgeLayer } from "./EdgeLayer";
import { NodeCard } from "./NodeCard";

interface Viewport {
  x: number;
  y: number;
  scale: number;
}

interface DragState {
  nodeId: string;
  startClientX: number;
  startClientY: number;
  startNodeX: number;
  startNodeY: number;
  historyPushed: boolean;
}

interface PanState {
  startClientX: number;
  startClientY: number;
  startX: number;
  startY: number;
}

export function WorkflowCanvas() {
  const {
    workflow,
    selectedNodeId,
    selectedEdgeId,
    readOnly,
    addNode,
    addEdge,
    moveNode,
    pushHistory,
    deleteNode,
    duplicateNode,
    setStartNode,
    insertNodeBefore,
    insertNodeAfter,
    selectNode,
    selectEdge,
    clearSelection,
  } = useDesigner();

  const wrapRef = useRef<HTMLDivElement>(null);
  const [viewport, setViewport] = useState<Viewport>({
    x: 70,
    y: 60,
    scale: 0.9,
  });
  const viewportRef = useRef(viewport);
  useEffect(() => {
    viewportRef.current = viewport;
  }, [viewport]);
  const dragRef = useRef<DragState | null>(null);
  const panRef = useRef<PanState | null>(null);
  const [connecting, setConnecting] = useState<{
    fromNodeId: string;
    x: number;
    y: number;
  } | null>(null);
  const connectingRef = useRef(connecting);
  useEffect(() => {
    connectingRef.current = connecting;
  }, [connecting]);
  const [pendingEdge, setPendingEdge] = useState<{
    fromNodeId: string;
    toNodeId: string;
  } | null>(null);
  const [condition, setCondition] = useState("");
  const [priority, setPriority] = useState(1);
  const [contextMenu, setContextMenu] = useState<{
    nodeId: string;
    x: number;
    y: number;
  } | null>(null);

  const nodeMap = useMemo(
    () => new Map(workflow.nodes.map((node) => [node.id, node])),
    [workflow.nodes]
  );

  const screenToWorld = useCallback((clientX: number, clientY: number) => {
    const rect = wrapRef.current!.getBoundingClientRect();
    const view = viewportRef.current;
    return {
      x: (clientX - rect.left - view.x) / view.scale,
      y: (clientY - rect.top - view.y) / view.scale,
    };
  }, []);

  const fit = useCallback(() => {
    const wrap = wrapRef.current;
    if (!wrap || !workflow.nodes.length) return;
    const rect = wrap.getBoundingClientRect();
    const minX = Math.min(...workflow.nodes.map((node) => node.positionX)) - 80;
    const minY = Math.min(...workflow.nodes.map((node) => node.positionY)) - 80;
    const maxX =
      Math.max(...workflow.nodes.map((node) => node.positionX)) +
      NODE_WIDTH +
      80;
    const maxY =
      Math.max(...workflow.nodes.map((node) => node.positionY)) +
      NODE_HEIGHT +
      80;
    const width = maxX - minX;
    const height = maxY - minY;
    const scale = Math.max(
      0.05,
      Math.min(1.25, Math.min(rect.width / width, rect.height / height))
    );
    setViewport({
      scale,
      x: (rect.width - width * scale) / 2 - minX * scale,
      y: (rect.height - height * scale) / 2 - minY * scale,
    });
  }, [workflow.nodes]);

  const fitted = useRef(false);
  useEffect(() => {
    if (fitted.current) return;
    const frame = requestAnimationFrame(() => {
      fit();
      fitted.current = true;
    });
    return () => cancelAnimationFrame(frame);
  }, [fit]);

  useEffect(() => {
    const onPointerMove = (event: PointerEvent) => {
      if (dragRef.current) {
        const drag = dragRef.current;
        const scale = viewportRef.current.scale;
        const dx = (event.clientX - drag.startClientX) / scale;
        const dy = (event.clientY - drag.startClientY) / scale;
        if (!drag.historyPushed && (Math.abs(dx) > 3 || Math.abs(dy) > 3)) {
          pushHistory();
          drag.historyPushed = true;
        }
        if (drag.historyPushed)
          moveNode(drag.nodeId, drag.startNodeX + dx, drag.startNodeY + dy);
        return;
      }

      if (panRef.current) {
        const pan = panRef.current;
        setViewport((current) => ({
          ...current,
          x: pan.startX + event.clientX - pan.startClientX,
          y: pan.startY + event.clientY - pan.startClientY,
        }));
        return;
      }

      if (connectingRef.current) {
        const point = screenToWorld(event.clientX, event.clientY);
        setConnecting((current) =>
          current ? { ...current, x: point.x, y: point.y } : null
        );
      }
    };

    const onPointerUp = () => {
      dragRef.current = null;
      panRef.current = null;
      document.body.classList.remove("dragging-canvas");
      if (connectingRef.current) setConnecting(null);
    };

    window.addEventListener("pointermove", onPointerMove);
    window.addEventListener("pointerup", onPointerUp);
    return () => {
      window.removeEventListener("pointermove", onPointerMove);
      window.removeEventListener("pointerup", onPointerUp);
      document.body.classList.remove("dragging-canvas");
    };
  }, [moveNode, pushHistory, screenToWorld]);

  useEffect(() => {
    const closeOnEscape = (event: KeyboardEvent) => {
      if (event.key === "Escape") setContextMenu(null);
    };
    const closeOnPointerDown = () => setContextMenu(null);
    window.addEventListener("keydown", closeOnEscape);
    window.addEventListener("pointerdown", closeOnPointerDown);
    return () => {
      window.removeEventListener("keydown", closeOnEscape);
      window.removeEventListener("pointerdown", closeOnPointerDown);
    };
  }, []);

  const zoomAt = useCallback(
    (factor: number, clientX?: number, clientY?: number) => {
      const wrap = wrapRef.current;
      if (!wrap) return;
      const rect = wrap.getBoundingClientRect();
      const cx = clientX ?? rect.left + rect.width / 2;
      const cy = clientY ?? rect.top + rect.height / 2;
      const world = screenToWorld(cx, cy);

      setViewport((current) => {
        const scale = Math.max(0.05, Math.min(1.8, current.scale * factor));
        return {
          scale,
          x: cx - rect.left - world.x * scale,
          y: cy - rect.top - world.y * scale,
        };
      });
    },
    [screenToWorld]
  );

  // React attaches its root wheel listener as passive, so a JSX onWheel can't
  // preventDefault (logs a console error and lets the page scroll underneath
  // the canvas). A native, non-passive listener is required to block it.
  useEffect(() => {
    const wrap = wrapRef.current;
    if (!wrap) return;
    const onWheel = (event: WheelEvent) => {
      event.preventDefault();
      if (event.ctrlKey || event.metaKey) {
        zoomAt(event.deltaY < 0 ? 1.1 : 0.9, event.clientX, event.clientY);
        return;
      }
      setViewport((current) => ({
        ...current,
        x: current.x - event.deltaX,
        y: current.y - event.deltaY,
      }));
    };
    wrap.addEventListener("wheel", onWheel, { passive: false });
    return () => wrap.removeEventListener("wheel", onWheel);
  }, [zoomAt]);

  const finishConnection = (toNodeId: string) => {
    if (readOnly) return;
    const current = connectingRef.current;
    if (!current || current.fromNodeId === toNodeId) return;
    const from = nodeMap.get(current.fromNodeId);
    setPendingEdge({ fromNodeId: current.fromNodeId, toNodeId });
    setCondition(from?.type === "Decision" ? "DEFAULT" : "");
    setPriority(1);
    setConnecting(null);
  };

  return (
    <main
      ref={wrapRef}
      className="canvas-wrap"
      onContextMenu={(event) => {
        if (!(event.target as HTMLElement).closest(".workflow-node")) {
          event.preventDefault();
          setContextMenu(null);
        }
      }}
      onPointerDown={(event) => {
        if (event.button !== 0 || event.target !== event.currentTarget) return;
        setContextMenu(null);
        clearSelection();
        panRef.current = {
          startClientX: event.clientX,
          startClientY: event.clientY,
          startX: viewport.x,
          startY: viewport.y,
        };
        document.body.classList.add("dragging-canvas");
      }}
      onDragOver={(event) => {
        if (event.dataTransfer.types.includes("application/workflow-node-type"))
          event.preventDefault();
      }}
      onDrop={(event) => {
        const type = event.dataTransfer.getData(
          "application/workflow-node-type"
        ) as WorkflowNodeType;
        if (readOnly || !nodeTypes.includes(type)) return;
        event.preventDefault();
        const point = screenToWorld(event.clientX, event.clientY);
        addNode(type, point.x - NODE_WIDTH / 2, point.y - NODE_HEIGHT / 2);
      }}
    >
      <div
        className="world"
        style={{
          transform: `translate(${viewport.x}px, ${viewport.y}px) scale(${viewport.scale})`,
        }}
        onPointerDown={(event) => {
          if (event.target === event.currentTarget) {
            event.stopPropagation();
            setContextMenu(null);
            clearSelection();
            panRef.current = {
              startClientX: event.clientX,
              startClientY: event.clientY,
              startX: viewport.x,
              startY: viewport.y,
            };
            document.body.classList.add("dragging-canvas");
          }
        }}
      >
        <EdgeLayer
          workflow={workflow}
          selectedEdgeId={selectedEdgeId}
          chosenEdgeIds={[]}
          preview={connecting}
          onSelectEdge={selectEdge}
        />
        {workflow.nodes.map((node) => (
          <NodeCard
            key={node.id}
            node={node}
            selected={selectedNodeId === node.id}
            current={false}
            done={false}
            connectingFrom={connecting?.fromNodeId ?? null}
            onSelect={() => selectNode(node.id)}
            onDragStart={(event: ReactPointerEvent<HTMLDivElement>) => {
              if (
                readOnly ||
                event.button !== 0 ||
                (event.target as HTMLElement).closest(".node-port")
              )
                return;
              event.stopPropagation();
              dragRef.current = {
                nodeId: node.id,
                startClientX: event.clientX,
                startClientY: event.clientY,
                startNodeX: node.positionX,
                startNodeY: node.positionY,
                historyPushed: false,
              };
            }}
            onOutputStart={(event) => {
              event.preventDefault();
              event.stopPropagation();
              if (readOnly) return;
              const point = screenToWorld(event.clientX, event.clientY);
              setConnecting({ fromNodeId: node.id, x: point.x, y: point.y });
            }}
            onInputFinish={(event) => {
              event.preventDefault();
              event.stopPropagation();
              finishConnection(node.id);
            }}
            onContextMenu={(event) => {
              event.preventDefault();
              event.stopPropagation();
              selectNode(node.id);
              if (readOnly) return;
              setContextMenu({
                nodeId: node.id,
                x: Math.min(event.clientX, window.innerWidth - 210),
                y: Math.min(event.clientY, window.innerHeight - 290),
              });
            }}
          />
        ))}
      </div>
      {contextMenu && nodeMap.has(contextMenu.nodeId) && (
        <div
          className="node-context-menu"
          role="menu"
          aria-label="Node actions"
          style={{
            left: Math.max(8, contextMenu.x),
            top: Math.max(8, contextMenu.y),
          }}
          onPointerDown={(event) => event.stopPropagation()}
        >
          <strong>{nodeMap.get(contextMenu.nodeId)?.name}</strong>
          <button
            role="menuitem"
            onClick={() => {
              duplicateNode(contextMenu.nodeId);
              setContextMenu(null);
            }}
          >
            Duplicate
          </button>
          <button
            role="menuitem"
            onClick={() => {
              setStartNode(contextMenu.nodeId);
              setContextMenu(null);
            }}
          >
            Make Start
          </button>
          <button
            role="menuitem"
            onClick={() => {
              insertNodeBefore(contextMenu.nodeId);
              setContextMenu(null);
            }}
          >
            Insert before
          </button>
          <button
            role="menuitem"
            onClick={() => {
              insertNodeAfter(contextMenu.nodeId);
              setContextMenu(null);
            }}
          >
            Insert after
          </button>
          <div className="context-menu-separator" />
          <button
            className="danger"
            role="menuitem"
            onClick={() => {
              if (window.confirm("Delete this node and its edges?")) {
                deleteNode(contextMenu.nodeId);
              }
              setContextMenu(null);
            }}
          >
            Delete
          </button>
          <button
            className="danger"
            role="menuitem"
            onClick={() => {
              if (window.confirm("Delete this node and reconnect its edges?")) {
                deleteNode(contextMenu.nodeId, true);
              }
              setContextMenu(null);
            }}
          >
            Delete and reconnect
          </button>
        </div>
      )}
      <div className="canvas-controls">
        <button onClick={() => zoomAt(1 / 1.15)}>−</button>
        <span>{Math.round(viewport.scale * 100)}%</span>
        <button onClick={() => zoomAt(1.15)}>+</button>
        <button onClick={fit}>Fit</button>
        <button
          disabled={!selectedNodeId}
          onClick={() => {
            const node = nodeMap.get(selectedNodeId ?? "");
            const rect = wrapRef.current?.getBoundingClientRect();
            if (node && rect)
              setViewport({
                scale: 1,
                x: rect.width / 2 - node.positionX - NODE_WIDTH / 2,
                y: rect.height / 2 - node.positionY - NODE_HEIGHT / 2,
              });
          }}
        >
          Focus selected
        </button>
      </div>
      <Dialog
        open={!!pendingEdge}
        onClose={() => setPendingEdge(null)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Connect nodes</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            fullWidth
            label="Condition"
            value={condition}
            onChange={(event) => setCondition(event.target.value)}
            placeholder="Huawei / DEFAULT / empty"
            sx={{ mt: 1, mb: 2 }}
          />
          <TextField
            fullWidth
            label="Priority"
            type="number"
            value={priority}
            onChange={(event) => setPriority(Number(event.target.value))}
            error={!Number.isInteger(priority)}
            helperText="Lower numbers are checked first."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPendingEdge(null)}>Cancel</Button>
          <Button
            variant="contained"
            disabled={readOnly || !Number.isInteger(priority)}
            onClick={() => {
              if (pendingEdge)
                addEdge(
                  pendingEdge.fromNodeId,
                  pendingEdge.toNodeId,
                  condition.trim() || null,
                  priority
                );
              setPendingEdge(null);
            }}
          >
            Connect
          </Button>
        </DialogActions>
      </Dialog>{" "}
    </main>
  );
}
