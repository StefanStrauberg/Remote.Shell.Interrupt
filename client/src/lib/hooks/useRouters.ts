import { useLocation } from "react-router";
import { CompoundObject } from "../types/NetworkDevices/CompoundObject";
import { RouterFilter } from "../types/NetworkDevices/RouterFilter";

export const useRouters = (
  filters: RouterFilter = {}
): {
  compoundObject: CompoundObject;
} => {
  const location = useLocation();

  return {
    null,
  };
};
