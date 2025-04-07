export type SPRVlanFilter = {
  IdVlan?: { op: string; value: number };
  IdClient?: { op: string; value: number };
  UseClient?: { op: string; value: boolean };
  UseCOD?: { op: string; value: boolean };
};
