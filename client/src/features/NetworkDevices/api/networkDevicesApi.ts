import httpClient from "@/lib/api/httpClient";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged, PagedResponse } from "@/lib/api/common/paged";
import { OrderByParams } from "@/lib/api/common/orderByParams";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { PaginationParams } from "@/lib/types/Common/PaginationParams";
import { NetworkDevice } from "@/lib/types/NetworkDevices/NetworkDevice";
import { CompoundObject } from "@/lib/types/NetworkDevices/CompoundObject";
import { apiPath } from "@/config/api.config";

export const DEFAULT_NETWORK_DEVICE_FILTERS: FilterDescriptor[] = [];

export type NetworkDevicesListRequest = {
  pagination: PaginationParams;
  filters?: FilterDescriptor[];
  orderBy: OrderByParams;
};

export const networkDevicesApi = {
  list(
    request: NetworkDevicesListRequest
  ): Promise<PagedResponse<NetworkDevice>> {
    return fetchPaged(
      apiPath("NetworkDevices", "GetNetworkDevicesByFilter"),
      buildRequestParams(
        request.pagination,
        request.orderBy,
        request.filters ?? []
      )
    );
  },

  async getById(id: string): Promise<NetworkDevice> {
    const response = await httpClient.get<NetworkDevice>(
      apiPath("NetworkDevices", "GetNetworkDeviceById", id)
    );
    return response.data;
  },

  async getByVlan(vlanId: number): Promise<CompoundObject> {
    const response = await httpClient.get<CompoundObject>(
      apiPath("NetworkDevices", "GetNetworkDevicesByVlanTag", vlanId)
    );
    return response.data;
  },

  async removeAll(): Promise<void> {
    await httpClient.delete(apiPath("NetworkDevices", "DeleteNetworkDevices"));
  },
};
