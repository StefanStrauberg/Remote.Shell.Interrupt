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
  vlans: Vlan[];
  aggregatedPorts: Port[];
  macTable: string[];
  /** MAC address -> set of IP addresses resolved via ARP on this port. */
  arpTableOfPort?: Record<string, string[]>;
  /** Network address -> netmask of terminated networks on this port. */
  networkTableOfPort?: Record<string, string>;
  parentId?: string;
};
