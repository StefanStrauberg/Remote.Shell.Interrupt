import { useSyncExternalStore } from "react";

/**
 * Global UI store tracking in-flight HTTP requests.
 *
 * Implemented as a plain external store consumable through
 * {@link useSyncExternalStore}, so React components re-render when the
 * request counter changes without any additional state library.
 *
 * A counter (rather than a boolean) keeps the indicator active while
 * concurrent requests are still in flight.
 */
const listeners = new Set<() => void>();
let pendingRequests = 0;

function emit() {
  listeners.forEach((listener) => listener());
}

export const uiStore = {
  startRequest(): void {
    pendingRequests += 1;
    emit();
  },

  endRequest(): void {
    pendingRequests = Math.max(0, pendingRequests - 1);
    emit();
  },

  subscribe(listener: () => void): () => void {
    listeners.add(listener);
    return () => {
      listeners.delete(listener);
    };
  },

  getSnapshot(): number {
    return pendingRequests;
  },
};

/** Returns true while at least one HTTP request is in flight. */
export function useIsBusy(): boolean {
  return (
    useSyncExternalStore(uiStore.subscribe, uiStore.getSnapshot, () => 0) > 0
  );
}
