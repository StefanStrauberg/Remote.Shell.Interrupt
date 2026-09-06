import { QueryClient } from "@tanstack/react-query";

/**
 * Shared TanStack Query instance. Declared in its own module so non-React
 * code (the axios 401 interceptor) can clear the cache when a session ends.
 */
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      staleTime: 5 * 60 * 1000 // 5 minutes
    }
  }
});
