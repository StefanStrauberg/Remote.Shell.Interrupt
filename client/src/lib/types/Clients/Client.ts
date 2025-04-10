import { Cod } from "../CODs/Cod";
import { SprVlan } from "../SPRVlans/SprVlan";
import { TfPlan } from "../TfPlans/TfPlan";

export type Client = {
  id: string;
  dat1?: Date;
  dat2?: Date;
  prim1?: string;
  prim2?: string;
  nik?: string;
  nrDogovor: string;
  contactC?: string; // Nullable field
  telephoneC?: string; // Nullable field
  emailC?: string; // Nullable field
  history?: string; // Nullable field
  id_COD: number;
  cod?: Cod; // Required COD object
  id_TfPlan?: number; // Nullable field
  tfPlan?: TfPlan; // Nullable TfPlan object
  sprVlans: SprVlan[];
  idClient: number;
  name: string;
  contactT?: string; // Nullable field
  telephoneT?: string; // Nullable field
  emailT?: string; // Nullable field
  working: boolean;
  antiDDOS: boolean;
};
