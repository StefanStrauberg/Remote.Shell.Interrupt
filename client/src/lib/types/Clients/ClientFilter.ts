export type ClientFilter = {
  Name?: { op: string; value: string };
  NrDogovor?: { op: string; value: string };
  Working?: { op: string; value: boolean };
  AntiDDOS?: { op: string; value: boolean };
};
