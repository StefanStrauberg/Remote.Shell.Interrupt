import httpClient from "@/lib/api/httpClient";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged, PagedResponse } from "@/lib/api/common/paged";
import { OrderByParams } from "@/lib/api/common/orderByParams";
import { Client } from "@/lib/types/Clients/Client";
import { ClientShort } from "@/lib/types/Clients/ClientShort";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { PaginationParams } from "@/lib/types/Common/PaginationParams";
import { isGuid } from "@/lib/utils";
import { apiPath } from "@/config/api.config";
import { FilterOperator } from "@/lib/types/Common/FilterOperator";

export const DEFAULT_CLIENT_FILTERS: FilterDescriptor[] = [
  { PropertyPath: "Working", Operator: FilterOperator.Equals, Value: "true" },
];

export type ClientsListRequest = {
  pagination: PaginationParams;
  filters?: FilterDescriptor[];
  orderBy: OrderByParams;
};

export const clientsApi = {
  list(request: ClientsListRequest): Promise<PagedResponse<ClientShort>> {
    return fetchPaged(
      apiPath("Clients", "GetClientsByFilter"),
      buildRequestParams(
        request.pagination,
        request.orderBy,
        request.filters ?? []
      )
    );
  },

  async getById(id: string | number): Promise<Client> {
    if (isGuid(id)) {
      const response = await httpClient.get<Client>(
        apiPath("Clients", "GetClientById", id)
      );
      return response.data;
    }

    const response = await httpClient.get<Client>(
      apiPath("Clients", "GetClientWithChildrenByFilter"),
      {
        params: {
          "Filters[0].PropertyPath": "IdClient",
          "Filters[0].Operator": "Equals",
          "Filters[0].Value": String(id),
        },
      }
    );
    return response.data;
  },

  async synchronize(): Promise<void> {
    await httpClient.put(apiPath("Clients", "UpdateClientsLocalDb"));
  },

  async removeAll(): Promise<void> {
    await httpClient.delete(apiPath("Clients", "DeleteClientsLocalDb"));
  },
};
