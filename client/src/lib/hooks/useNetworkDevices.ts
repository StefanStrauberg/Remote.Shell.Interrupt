import { useQuery } from "@tanstack/react-query";
import { NetworkDevice } from "../types/NetworkDevices/NetworkDevice";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { DEFAULT_PAGINATION } from "../types/Common/PaginationMetadata";

export const useNetworkDevices = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = []
) => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;
  const queryKey = [
    "networkDevices",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
  ];

  const { data: networkDevicesResponse, isPending } = useQuery({
    queryKey,
    queryFn: async () => {
      const params = buildRequestParams(pagination, filters);

      const response = await agent.get<NetworkDevice[]>(
        "/api/NetworkDevices/GetNetworkDevicesByFilter",
        { params }
      );
      return {
        data: response.data,
        pagination: JSON.parse(response.headers["x-pagination"]),
      };
    },
    enabled:
      location.pathname === "/networkDevices" || location.pathname === "/admin",
  });

  return {
    networkDevices: networkDevicesResponse?.data ?? [],
    isPending,
    pagination: networkDevicesResponse?.pagination ?? DEFAULT_PAGINATION,
  };
};
