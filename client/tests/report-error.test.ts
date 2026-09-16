import { afterEach, describe, expect, it, vi } from "vitest";

// Regression coverage for: reportError() only ever did console.error, so an unhandled error in
// a real user's browser (production, no dev console open) never reached anyone. It must now
// also forward to the backend's ReportClientError endpoint - and must never throw back into an
// error boundary, even if that forwarding request itself fails.

afterEach(() => {
  vi.unstubAllGlobals();
  vi.resetModules();
});

async function importReportError() {
  return (await import("../src/lib/observability/reportError")).reportError;
}

describe("reportError", () => {
  it("always logs to the console", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true });
    vi.stubGlobal("fetch", fetchMock);
    const consoleError = vi
      .spyOn(console, "error")
      .mockImplementation(() => {});
    const reportError = await importReportError();

    reportError(new Error("boom"), { boundary: "application" });

    expect(consoleError).toHaveBeenCalledWith(
      "Unhandled application error",
      expect.any(Error),
      { boundary: "application" }
    );
  });

  it("forwards the error to the backend ReportClientError endpoint", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true });
    vi.stubGlobal("fetch", fetchMock);
    vi.spyOn(console, "error").mockImplementation(() => {});
    const reportError = await importReportError();
    const error = new Error("boom");

    reportError(error, { boundary: "router", status: 500 });
    await Promise.resolve();

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toContain("/api/v1/Diagnostics/ReportClientError");
    expect(init.method).toBe("POST");
    const body = JSON.parse(init.body as string);
    expect(body.message).toBe("boom");
    expect(body.stack).toEqual(expect.stringContaining("Error: boom"));
    expect(JSON.parse(body.context)).toEqual({
      boundary: "router",
      status: 500,
    });
  });

  it("never throws when the network request itself fails", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockRejectedValue(new Error("network down"))
    );
    vi.spyOn(console, "error").mockImplementation(() => {});
    const reportError = await importReportError();

    expect(() => reportError(new Error("boom"))).not.toThrow();
    await Promise.resolve();
    await Promise.resolve();
  });

  it("reports a non-Error thrown value by its string form", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true });
    vi.stubGlobal("fetch", fetchMock);
    vi.spyOn(console, "error").mockImplementation(() => {});
    const reportError = await importReportError();

    reportError("plain string failure");
    await Promise.resolve();

    const body = JSON.parse(fetchMock.mock.calls[0][1].body as string);
    expect(body.message).toBe("plain string failure");
    expect(body.stack).toBeNull();
  });
});
