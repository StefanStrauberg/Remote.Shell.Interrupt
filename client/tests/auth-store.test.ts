import { beforeEach, describe, expect, it, vi } from "vitest";

/**
 * Minimal Storage stand-in: the default Vitest environment here is plain
 * Node, which has no `localStorage` global, and authStore.ts only ever
 * calls getItem/setItem/removeItem - so a real jsdom isn't needed to
 * exercise it.
 */
class MemoryStorage implements Pick<
  Storage,
  "getItem" | "setItem" | "removeItem"
> {
  private store = new Map<string, string>();
  getItem(key: string): string | null {
    return this.store.has(key) ? this.store.get(key)! : null;
  }
  setItem(key: string, value: string): void {
    this.store.set(key, value);
  }
  removeItem(key: string): void {
    this.store.delete(key);
  }
  keys(): string[] {
    return [...this.store.keys()];
  }
}

const memoryStorage = new MemoryStorage();
vi.stubGlobal("localStorage", memoryStorage);
// A real EventTarget stands in for `window` well enough to exercise
// addEventListener("storage", ...) - this project's Vitest setup is plain
// Node (no jsdom), so `window` doesn't exist here otherwise.
vi.stubGlobal("window", new EventTarget());

const { useAuthStore } = await import("../src/lib/auth/authStore");

/**
 * Simulates the "storage" event another same-origin tab would fire after
 * writing to localStorage - the browser never fires this event in the tab
 * that made the write itself, only in every other open tab.
 */
function fireStorageEvent(key: string | null, newValue: string | null): void {
  const event = new Event("storage") as Event & {
    key: string | null;
    newValue: string | null;
  };
  event.key = key;
  event.newValue = newValue;
  window.dispatchEvent(event);
}

const user = { id: "user-1", email: "a@test.com", roles: ["User"] };

describe("authStore session persistence", () => {
  beforeEach(() => {
    memoryStorage.keys().forEach((key) => memoryStorage.removeItem(key));
    useAuthStore.setState({ user: null, status: "idle" });
  });

  it("never writes a bearer token to storage — only the non-secret profile", () => {
    useAuthStore.getState().setSession(user);

    expect(memoryStorage.keys()).toEqual(["rsi.auth.user"]);
    expect(memoryStorage.getItem("rsi.auth.token")).toBeNull();
    expect(JSON.parse(memoryStorage.getItem("rsi.auth.user")!)).toEqual(user);
  });

  it("exposes no token field on the store at all", () => {
    useAuthStore.getState().setSession(user);

    expect("token" in useAuthStore.getState()).toBe(false);
  });

  it("restoreSession re-authenticates from the cached profile alone", () => {
    memoryStorage.setItem("rsi.auth.user", JSON.stringify(user));

    useAuthStore.getState().restoreSession();

    expect(useAuthStore.getState().status).toBe("authenticated");
    expect(useAuthStore.getState().user).toEqual(user);
  });

  it("restoreSession is unauthenticated with nothing cached", () => {
    useAuthStore.getState().restoreSession();

    expect(useAuthStore.getState().status).toBe("unauthenticated");
    expect(useAuthStore.getState().user).toBeNull();
  });

  it("clearSession removes the cached profile", () => {
    useAuthStore.getState().setSession(user);

    useAuthStore.getState().clearSession();

    expect(memoryStorage.keys()).toEqual([]);
    expect(useAuthStore.getState().status).toBe("unauthenticated");
  });
});

describe("authStore cross-tab sync", () => {
  beforeEach(() => {
    memoryStorage.keys().forEach((key) => memoryStorage.removeItem(key));
    useAuthStore.setState({ user: null, status: "idle" });
  });

  it("signs this tab out when another tab clears the cached profile", () => {
    useAuthStore.getState().setSession(user);

    fireStorageEvent("rsi.auth.user", null);

    expect(useAuthStore.getState().status).toBe("unauthenticated");
    expect(useAuthStore.getState().user).toBeNull();
  });

  it("signs this tab out when another tab calls localStorage.clear()", () => {
    // The spec fires key: null, newValue: null for a plain .clear() call
    // rather than naming the key that held the session.
    useAuthStore.getState().setSession(user);

    fireStorageEvent(null, null);

    expect(useAuthStore.getState().status).toBe("unauthenticated");
  });

  it("adopts the session when another tab logs in", () => {
    memoryStorage.setItem("rsi.auth.user", JSON.stringify(user));

    fireStorageEvent("rsi.auth.user", JSON.stringify(user));

    expect(useAuthStore.getState().status).toBe("authenticated");
    expect(useAuthStore.getState().user).toEqual(user);
  });

  it("ignores storage events for unrelated keys", () => {
    useAuthStore.getState().setSession(user);

    fireStorageEvent("some-unrelated-key", null);

    expect(useAuthStore.getState().status).toBe("authenticated");
    expect(useAuthStore.getState().user).toEqual(user);
  });
});
