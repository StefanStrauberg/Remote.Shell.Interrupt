import { useQuery } from "@tanstack/react-query";
import { tfPlansApi, TfPlansListRequest } from "./tfPlansApi";

export const tfPlanKeys = {
  all: ["tfPlans"] as const,
  lists: () => [...tfPlanKeys.all, "list"] as const,
  list: (request: TfPlansListRequest) =>
    [...tfPlanKeys.lists(), request] as const,
};

export function useTfPlansQuery(request: TfPlansListRequest) {
  return useQuery({
    queryKey: tfPlanKeys.list(request),
    queryFn: () => tfPlansApi.list(request),
  });
}
