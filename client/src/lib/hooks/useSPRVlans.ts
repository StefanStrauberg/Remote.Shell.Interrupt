import { useQuery } from "@tanstack/react-query";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { SprVlan } from "../types/SPRVlans/SprVlan";
import { SPRVlanFilter } from "../types/SPRVlans/SPRVlanFilter";

export const useSPRVlans = (
  pageNumber: number = 1,
  pageSize: number = 10,
  filters: SPRVlanFilter = {}
) => {
  const location = useLocation();

  const { data: sprVlansResponse, isPending } = useQuery({
    queryKey: ["sprVlans", pageNumber, pageSize, filters],
    queryFn: async () => {
      // Преобразуем фильтры в строку Sieve-формата: e.g. "Name@=John,Working==true"
      const filterString = Object.entries(filters)
        .map(([key, { op, value }]) => `${key}${op}${value}`)
        .join(",");

      console.log(filterString);

      const response = await agent.get<SprVlan[]>("/api/SPRVlans/GetSPRVlans", {
        params: { pageNumber, pageSize, Filters: filterString },
      });
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: location.pathname === "/sprVlans",
  });

  return {
    sprVlans: sprVlansResponse?.data ?? [],
    pagination: sprVlansResponse?.pagination ?? {
      totalPages: 0,
      currentPage: 0,
    },
    isPending,
  };
};
