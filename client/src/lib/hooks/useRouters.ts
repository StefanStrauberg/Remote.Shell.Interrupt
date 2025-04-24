import { useQuery, useQueryClient } from "@tanstack/react-query";
import { CompoundObject } from "../types/NetworkDevices/CompoundObject";
import agent from "../api/agent";
import { RouterFilter } from "../types/NetworkDevices/RouterFilter";

export const useRouters = (filters: RouterFilter = {}, isEnabled: boolean) => {
  const queryClient = useQueryClient(); // для сброса данных при фильтрах

  const fetchData = async () => {
    const response = await agent.get<CompoundObject>(
      `/api/NetworkDevices/GetNetworkDevicesByVlanTag/${filters.IdVlan?.value}`
    );
    return response.data;
  };

  const { data: compoundObject, isLoading: isCompoundObject } = useQuery({
    queryKey: ["mainPage", filters], // зависит от фильтров
    queryFn: fetchData,
    enabled: isEnabled, // активируем запрос вручную
  });

  const resetCache = () =>
    queryClient.invalidateQueries({
      queryKey: ["mainPage"], // Добавляем сюда queryKey
      exact: true, // Уточняем, что необходимо сбросить данные строго по этому ключу
    });

  return {
    compoundObject,
    isCompoundObject,
    resetCache,
  };
};
