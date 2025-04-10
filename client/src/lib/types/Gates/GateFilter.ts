export type GateFilter = {
  Name?: { op: string; value: string };
  IpAddress?: { op: string; value: string };
  typeOfNetworkDevice?: { op: string; value: string };
};
