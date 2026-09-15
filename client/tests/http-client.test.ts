import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { AxiosError, CanceledError } from "axios";
import httpClient from "../src/lib/api/httpClient";
import { uiStore } from "../src/lib/stores/uiStore";
import { registerUnauthorizedHandler } from "../src/lib/auth/unauthorizedHandler";
import {
  AUTH_COOKIE_LOGIN_URL,
  AUTH_COOKIE_LOGOUT_URL,
  AUTH_REGISTER_URL,
} from "../src/config/api.config";

const originalAdapter = httpClient.defaults.adapter;
const unauthorized = vi.fn();
beforeEach(() => {
  unauthorized.mockReset();
  registerUnauthorizedHandler(unauthorized);
});
afterEach(() => {
  httpClient.defaults.adapter = originalAdapter;
  registerUnauthorizedHandler(() => {});
});

function rejectWithStatus(status: number) {
  httpClient.defaults.adapter = async (config) => {
    throw new AxiosError("HTTP error", "ERR_BAD_RESPONSE", config, undefined, {
      status,
      statusText: "Error",
      data: { Error: "Server rejected the request" },
      headers: {},
      config,
    });
  };
}

describe("HTTP transport integration without network access", () => {
  it("sends cookies and balances the busy counter for a successful request", async () => {
    httpClient.defaults.adapter = async (config) => {
      expect(config.withCredentials).toBe(true);
      expect(uiStore.getSnapshot()).toBe(1);
      return {
        status: 200,
        statusText: "OK",
        data: { ok: true },
        headers: {},
        config,
      };
    };
    expect((await httpClient.get("/test")).data).toEqual({ ok: true });
    expect(uiStore.getSnapshot()).toBe(0);
  });
  it("normalizes business errors and clears the busy indicator", async () => {
    rejectWithStatus(422);
    await expect(httpClient.get("/test")).rejects.toMatchObject({
      name: "ApiError",
      status: 422,
      message: "Server rejected the request",
    });
    expect(uiStore.getSnapshot()).toBe(0);
    expect(unauthorized).not.toHaveBeenCalled();
  });
  it("notifies the application when a business request loses authentication", async () => {
    rejectWithStatus(401);
    await expect(
      httpClient.get("/api/v1/Clients/GetClientsByFilter")
    ).rejects.toMatchObject({ status: 401 });
    expect(unauthorized).toHaveBeenCalledTimes(1);
    expect(uiStore.getSnapshot()).toBe(0);
  });
  it.each([AUTH_COOKIE_LOGIN_URL, AUTH_COOKIE_LOGOUT_URL, AUTH_REGISTER_URL])(
    "leaves authentication errors at %s to the form",
    async (url) => {
      rejectWithStatus(401);
      await expect(httpClient.post(url, {})).rejects.toMatchObject({
        status: 401,
      });
      expect(unauthorized).not.toHaveBeenCalled();
      expect(uiStore.getSnapshot()).toBe(0);
    }
  );
  it("preserves cancellation instead of converting it to a user-facing error", async () => {
    const cancellation = new CanceledError("Cancelled by user");
    httpClient.defaults.adapter = async () => {
      throw cancellation;
    };
    await expect(httpClient.get("/test")).rejects.toBe(cancellation);
    expect(uiStore.getSnapshot()).toBe(0);
    expect(unauthorized).not.toHaveBeenCalled();
  });
  it("keeps the busy indicator active until all concurrent requests finish", async () => {
    let finishFirst!: () => void;
    let finishSecond!: () => void;
    httpClient.defaults.adapter = (config) =>
      new Promise((resolve) => {
        const complete = () =>
          resolve({
            status: 200,
            statusText: "OK",
            data: {},
            headers: {},
            config,
          });
        if (config.url === "/first") finishFirst = complete;
        else finishSecond = complete;
      });
    const first = httpClient.get("/first");
    const second = httpClient.get("/second");
    await vi.waitFor(() => expect(uiStore.getSnapshot()).toBe(2));
    finishFirst();
    await first;
    expect(uiStore.getSnapshot()).toBe(1);
    finishSecond();
    await second;
    expect(uiStore.getSnapshot()).toBe(0);
  });
});
