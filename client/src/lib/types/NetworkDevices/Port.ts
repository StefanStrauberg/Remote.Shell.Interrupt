import { Vlan } from "./Vlan";

export type Port = {
  id: string;
  interfaceNumber: number;
  interfaceName: string;
  interfaceType: string;
  interfaceStatus: string;
  interfaceSpeed: number;
  isAggregated: false;
  macAddress: string;
  description: string;
  vlaNs: Vlan[];
  aggregatedPorts: Port[];
};
