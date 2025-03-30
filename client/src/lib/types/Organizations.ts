import { Cod } from "./Cod";
import { SprVlan } from "./SprVlan";
import { TfPlan } from "./TfPlan";

export type Organization = {
  idClient: number;
  name: string;
  contactC: string;
  telephoneC: string;
  contactT: string;
  telephoneT: string;
  emailC: string;
  working: boolean;
  emailT: string;
  history: string;
  antiDDOS: boolean;
  id_COD: number;
  cod: Cod;
  id_TPlan: number;
  tfPlan: TfPlan;
  sprVlans: SprVlan[];
};
