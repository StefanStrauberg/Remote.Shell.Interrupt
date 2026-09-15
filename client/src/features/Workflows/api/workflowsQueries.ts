import { skipToken, useQuery } from "@tanstack/react-query";
import { workflowsApi } from "./workflowsApi";
export const workflowKeys = {
  all: ["workflows"] as const,
  list: (page: number, search: string) =>
    ["workflows", "list", page, search] as const,
  detail: (id: string) => ["workflows", "detail", id] as const,
};
export function useWorkflows(page: number, search: string) {
  return useQuery({
    queryKey: workflowKeys.list(page, search),
    queryFn: () => workflowsApi.list(page, search),
  });
}
export function useWorkflow(id?: string) {
  return useQuery({
    queryKey: workflowKeys.detail(id ?? ""),
    queryFn: id ? () => workflowsApi.get(id) : skipToken,
  });
}
