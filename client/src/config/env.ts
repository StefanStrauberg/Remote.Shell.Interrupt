function normalizeApiBaseUrl(value: string | undefined): string | undefined {
  const normalized = value?.trim().replace(/\/+$/, "");
  if (!normalized) return undefined;

  if (normalized.startsWith("/")) return normalized;

  let url: URL;
  try {
    url = new URL(normalized);
  } catch {
    throw new Error(
      "VITE_API_URL must be an absolute HTTP(S) URL or a root-relative path."
    );
  }

  if (url.protocol !== "http:" && url.protocol !== "https:") {
    throw new Error("VITE_API_URL must use the HTTP or HTTPS protocol.");
  }

  return normalized;
}

/** Validated runtime configuration. Undefined API URL means same-origin API. */
export const env = Object.freeze({
  apiBaseUrl: normalizeApiBaseUrl(import.meta.env.VITE_API_URL),
});
