import { useQuery } from "@tanstack/react-query";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { SprVlan } from "../types/SprVlan";
import { SPRVlanFilter } from "../types/SPRVlanFilter";

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

      const response = await agent.get<SprVlan[]>("/SPRVlans/GetSPRVlans", {
        params: { pageNumber, pageSize, Filters: filterString },
      });
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled: location.pathname === "/sprVlans",
  });

  // Extract organizations and pagination from the gatesResponse
  const sprVlans = sprVlansResponse?.data;
  const pagination = sprVlansResponse?.pagination;

  return {
    sprVlans,
    pagination,
    isPending,
  };
};
