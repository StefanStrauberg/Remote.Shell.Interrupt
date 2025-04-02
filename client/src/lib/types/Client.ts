import { Cod } from "./Cod";
import { SprVlan } from "./SprVlan";
import { tfPlan } from "./TfPlan";

export type Client = {
  id: string;
  contactC?: string; // Nullable field
  telephoneC?: string; // Nullable field
  emailC?: string; // Nullable field
  history?: string; // Nullable field
  id_COD: number;
  cod?: Cod; // Required COD object
  id_TfPlan?: number; // Nullable field
  tfPlan?: tfPlan; // Nullable TfPlan object
  sprVlans: SprVlan[];
  idClient: number;
  name: string;
  contactT?: string; // Nullable field
  telephoneT?: string; // Nullable field
  emailT?: string; // Nullable field
  working: boolean;
  antiDDOS: boolean;
};
