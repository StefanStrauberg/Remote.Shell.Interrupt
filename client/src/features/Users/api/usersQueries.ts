import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { usersApi, UsersListRequest } from "./usersApi";

export const userKeys = {
  all: ["users"] as const,
  lists: () => [...userKeys.all, "list"] as const,
  list: (request: UsersListRequest) => [...userKeys.lists(), request] as const,
};

export function useUsersQuery(request: UsersListRequest) {
  return useQuery({
    queryKey: userKeys.list(request),
    queryFn: () => usersApi.list(request),
  });
}

function useInvalidateUsers() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: userKeys.all });
}

export function useUpdateUserRoleMutation() {
  const invalidate = useInvalidateUsers();
  return useMutation<void, Error, { userId: string; role: string }>({
    mutationFn: ({ userId, role }) => usersApi.updateRole(userId, role),
    onSuccess: invalidate,
  });
}

export function useSetUserActiveMutation() {
  const invalidate = useInvalidateUsers();
  return useMutation<void, Error, { userId: string; isActive: boolean }>({
    mutationFn: ({ userId, isActive }) => usersApi.setActive(userId, isActive),
    onSuccess: invalidate,
  });
}

export function useDeleteUserMutation() {
  const invalidate = useInvalidateUsers();
  return useMutation<void, Error, string>({
    mutationFn: usersApi.remove,
    onSuccess: invalidate,
  });
}
