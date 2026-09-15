import axios from "axios";

type ErrorPayload = {
  Error?: string;
  error?: string;
  Title?: string;
  title?: string;
  Detail?: string;
  detail?: string;
  Errors?: Record<string, string[]>;
  errors?: Record<string, string[]>;
};

export class ApiError extends Error {
  readonly status?: number;
  readonly validationErrors: string[];
  readonly isNetworkError: boolean;
  readonly originalError?: unknown;

  constructor(options: {
    message: string;
    status?: number;
    validationErrors?: string[];
    isNetworkError?: boolean;
    cause?: unknown;
  }) {
    super(options.message);
    this.name = "ApiError";
    this.status = options.status;
    this.validationErrors = options.validationErrors ?? [];
    this.isNetworkError = options.isNetworkError ?? false;
    this.originalError = options.cause;
  }
}

function asPayload(value: unknown): ErrorPayload {
  return typeof value === "object" && value !== null
    ? (value as ErrorPayload)
    : {};
}

export function toApiError(error: unknown): ApiError {
  if (error instanceof ApiError) return error;

  if (!axios.isAxiosError(error)) {
    return new ApiError({
      message: error instanceof Error ? error.message : "Unexpected error.",
      cause: error,
    });
  }

  const status = error.response?.status;
  const payload = asPayload(error.response?.data);
  const errors = payload.Errors ?? payload.errors;
  const validationErrors = errors ? Object.values(errors).flat() : [];
  const isNetworkError = !error.response;
  const message =
    validationErrors.join("; ") ||
    payload.Error ||
    payload.error ||
    payload.Detail ||
    payload.detail ||
    payload.Title ||
    payload.title ||
    (isNetworkError
      ? "Network error. Check your connection and try again."
      : `Request failed${status ? ` (${status})` : ""}.`);

  return new ApiError({
    message,
    status,
    validationErrors,
    isNetworkError,
    cause: error,
  });
}
