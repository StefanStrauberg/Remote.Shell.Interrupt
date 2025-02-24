import { Vlan } from "./Vlan";

export type Port = {
  id: string;
  interfaceNumber: number;
  interfaceName: string;
  interfaceType: string;
  interfaceStatus: string;
  interfaceSpeed: number;
  isAggregated: boolean;
  macAddress: string;
  description: string;
  aggregatedPorts: unknown[];
  macTable: unknown[];
  arpTableOfPort: unknown;
  networkTableOfPort: unknown;
  vlaNs: Vlan[];
};
