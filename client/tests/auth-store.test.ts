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

const { useAuthStore } = await import("../src/lib/auth/authStore");

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
