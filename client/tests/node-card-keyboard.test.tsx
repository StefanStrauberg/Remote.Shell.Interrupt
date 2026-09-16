// @vitest-environment jsdom
import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { NodeCard } from "../src/features/Workflows/designer/components/NodeCard";
import { WorkflowNode } from "../src/features/Workflows/domain/workflow/model";

// Regression coverage for: node cards on the canvas were plain <div>s with
// only mouse handlers (onClick/onPointerDown/onContextMenu) - unreachable
// and inoperable via Tab/Enter/Space, and invisible to screen readers as an
// interactive element.

const node: WorkflowNode = {
  id: "node-1",
  type: "Script",
  name: "Transform payload",
  key: "transform",
  config: {},
  positionX: 0,
  positionY: 0,
};

function renderCard(overrides: Partial<Parameters<typeof NodeCard>[0]> = {}) {
  const onSelect = vi.fn();
  render(
    <NodeCard
      node={node}
      selected={false}
      current={false}
      done={false}
      connectingFrom={null}
      onSelect={onSelect}
      onDragStart={() => {}}
      onOutputStart={() => {}}
      onInputFinish={() => {}}
      onContextMenu={() => {}}
      {...overrides}
    />
  );
  return { onSelect };
}

describe("NodeCard keyboard accessibility", () => {
  it("exposes the card as a focusable, labeled button", () => {
    renderCard({ selected: true });
    const card = screen.getByRole("button", {
      name: "Script node: Transform payload",
    });
    expect(card).toHaveAttribute("tabIndex", "0");
    expect(card).toHaveAttribute("aria-pressed", "true");
  });
  it("selects the node on Enter", async () => {
    const { onSelect } = renderCard();
    const user = userEvent.setup();
    screen.getByRole("button", { name: /Transform payload/ }).focus();
    await user.keyboard("{Enter}");
    expect(onSelect).toHaveBeenCalledTimes(1);
  });
  it("selects the node on Space", async () => {
    const { onSelect } = renderCard();
    const user = userEvent.setup();
    screen.getByRole("button", { name: /Transform payload/ }).focus();
    await user.keyboard(" ");
    expect(onSelect).toHaveBeenCalledTimes(1);
  });
  it("still selects the node on click", async () => {
    const { onSelect } = renderCard();
    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /Transform payload/ }));
    expect(onSelect).toHaveBeenCalledTimes(1);
  });
});
