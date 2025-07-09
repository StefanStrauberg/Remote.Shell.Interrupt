import { useQuery } from "@tanstack/react-query";
import { NetworkDevice } from "../types/NetworkDevices/NetworkDevice";
import { PaginationMetadata } from "../types/Common/PaginationMetadata";
import { useLocation } from "react-router";
import { NetworkDeviceFilter } from "../types/NetworkDevices/NetworkDeviceFilter";
import agent from "../api/agent";

export const useNetworkDevices = (
  pageNumber: number = 1,
  pageSize: number = 10,
  filters: NetworkDeviceFilter = {}
): {
  networkDevices: NetworkDevice[];
  isLoadingNetworkDevices: boolean;
  pagination: PaginationMetadata;
} => {
  const location = useLocation();

  const { data: networkDevicesResponse, isPending: isLoadingNetworkDevices } =
    useQuery({
      queryKey: ["gates", pageNumber, pageSize, filters],
      queryFn: async () => {
        // Преобразуем фильтры в строку Sieve-формата: e.g. "Name@=John,Working==true"
        const filterString = Object.entries(filters)
          .map(([key, { op, value }]) => `${key}${op}${value}`)
          .join(",");

        console.log(filterString);

        const response = await agent.get<NetworkDevice[]>(
          "/api/NetworkDevices/GetNetworkDevices",
          {
            params: { pageNumber, pageSize, Filters: filterString },
          }
        );
        return {
          data: response.data,
          pagination: JSON.parse(response.headers["x-pagination"]),
        };
      },
      enabled: location.pathname === "/networkDevices",
    });

  return {
    networkDevices: networkDevicesResponse?.data ?? [],
    pagination: networkDevicesResponse?.pagination ?? {
      TotalPages: 0,
      CurrentPage: 0,
    },
    isLoadingNetworkDevices,
  };
};
