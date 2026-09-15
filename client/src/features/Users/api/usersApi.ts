import httpClient from "@/lib/api/httpClient";
import { buildRequestParams } from "@/lib/api/common/buildRequestParams";
import { fetchPaged, PagedResponse } from "@/lib/api/common/paged";
import { OrderByParams } from "@/lib/api/common/orderByParams";
import { FilterDescriptor } from "@/lib/types/Common/FilterDescriptor";
import { PaginationParams } from "@/lib/types/Common/PaginationParams";
import { User } from "@/lib/types/Users/User";
import { apiPath } from "@/config/api.config";

export const DEFAULT_USER_FILTERS: FilterDescriptor[] = [];

export type UsersListRequest = {
  pagination: PaginationParams;
  filters?: FilterDescriptor[];
  orderBy: OrderByParams;
};

export const usersApi = {
  list(request: UsersListRequest): Promise<PagedResponse<User>> {
    return fetchPaged(
      apiPath("Users", "GetUsersByFilter"),
      buildRequestParams(
        request.pagination,
        request.orderBy,
        request.filters ?? []
      )
    );
  },

  async updateRole(userId: string, role: string): Promise<void> {
    await httpClient.put(apiPath("Users", "UpdateUserRole"), { userId, role });
  },

  async setActive(userId: string, isActive: boolean): Promise<void> {
    await httpClient.put(apiPath("Users", "SetUserActive"), {
      userId,
      isActive,
    });
  },

  async updateProfile(
    userId: string,
    email: string,
    fullName: string | null
  ): Promise<void> {
    await httpClient.put(apiPath("Users", "UpdateUserProfile"), {
      userId,
      email,
      fullName,
    });
  },

  async remove(userId: string): Promise<void> {
    await httpClient.delete(apiPath("Users", "DeleteUser", userId));
  },
};
