import { useQuery } from "@tanstack/react-query";
import { sprVlansApi, SprVlansListRequest } from "./sprVlansApi";

export const sprVlanKeys = {
  all: ["sprVlans"] as const,
  lists: () => [...sprVlanKeys.all, "list"] as const,
  list: (request: SprVlansListRequest) =>
    [...sprVlanKeys.lists(), request] as const,
};

export function useSprVlansQuery(request: SprVlansListRequest) {
  return useQuery({
    queryKey: sprVlanKeys.list(request),
    queryFn: () => sprVlansApi.list(request),
  });
}
