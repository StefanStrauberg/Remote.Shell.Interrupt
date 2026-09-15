import httpClient from "@/lib/api/httpClient";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged, PagedResponse } from "@/lib/api/common/paged";
import { OrderByParams } from "@/lib/api/common/orderByParams";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { PaginationParams } from "@/lib/types/Common/PaginationParams";
import { Gate, GateInput } from "@/lib/types/Gates/Gate";
import { apiPath } from "@/config/api.config";

export const DEFAULT_GATE_FILTERS: FilterDescriptor[] = [];

export type GatesListRequest = {
  pagination: PaginationParams;
  filters?: FilterDescriptor[];
  orderBy: OrderByParams;
};

const gatesEndpoint = (action: string, id?: string) =>
  id ? apiPath("Gates", action, id) : apiPath("Gates", action);

export const gatesApi = {
  list(request: GatesListRequest): Promise<PagedResponse<Gate>> {
    return fetchPaged(
      gatesEndpoint("GetGatesByFilter"),
      buildRequestParams(
        request.pagination,
        request.orderBy,
        request.filters ?? []
      )
    );
  },

  async getById(id: string): Promise<Gate> {
    const response = await httpClient.get<Gate>(
      gatesEndpoint("GetGateById", id)
    );
    return response.data;
  },

  async create(gate: GateInput): Promise<void> {
    await httpClient.post(gatesEndpoint("CreateGate"), gate);
  },

  async update(gate: Gate): Promise<void> {
    await httpClient.put(gatesEndpoint("UpdateGate"), gate);
  },

  async remove(id: string): Promise<void> {
    await httpClient.delete(gatesEndpoint("DeleteGateById", id));
  },

  async removeAll(): Promise<number> {
    // There is no bulk backend endpoint. Snapshot every ID before deletion so
    // page contents cannot shift while records are being removed.
    const ids = new Set<string>();
    let pageNumber = 1;
    let totalPages: number;

    do {
      const page = await gatesApi.list({
        pagination: { pageNumber, pageSize: 50 },
        filters: [],
        orderBy: { property: "Id", descending: false },
      });
      page.data.forEach((gate) => ids.add(gate.id));
      totalPages = page.pagination.TotalPages;
      pageNumber += 1;
    } while (pageNumber <= totalPages);

    for (const id of ids) await gatesApi.remove(id);
    return ids.size;
  },
};
