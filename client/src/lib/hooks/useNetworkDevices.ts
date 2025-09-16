import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { NetworkDevice } from "../types/NetworkDevices/NetworkDevice";
import { useLocation } from "react-router";
import agent from "../api/agent";
import { PaginationParams } from "../types/Common/PaginationParams";
import { FilterDescriptor } from "../types/Common/FilterDescriptor";
import { buildRequestParams } from "../api/common/buildRequestParams";
import { DEFAULT_PAGINATION } from "../types/Common/PaginationMetadata";
import { OrderByParams } from "../api/common/orderByParams";

export const useNetworkDevices = (
  pagination: PaginationParams,
  filters: FilterDescriptor[] = [],
  orderBy: OrderByParams
) => {
  const location = useLocation();
  const { pageNumber, pageSize } = pagination;
  const queryClient = useQueryClient();
  const queryKey = [
    "networkDevices",
    pageNumber,
    pageSize,
    JSON.stringify(filters),
  ];

  const { data: networkDevicesResponse, isPending } = useQuery({
    queryKey,
    queryFn: async () => {
      const params = buildRequestParams(pagination, orderBy, filters);

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

  const deleteNetworkDevices = useMutation({
    mutationFn: async () => {
      await agent.delete("/api/NetworkDevices/DeleteNetworkDevices");
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["clients", "tfPlans", "sprVlans"],
      });
    },
  });

  return {
    networkDevices: networkDevicesResponse?.data ?? [],
    isPending,
    pagination: networkDevicesResponse?.pagination ?? DEFAULT_PAGINATION,
    deleteNetworkDevices,
  };
};
