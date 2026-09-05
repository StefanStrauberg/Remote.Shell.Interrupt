import { Client } from "../Clients/Client";
import { NetworkDevice } from "./NetworkDevice";

export type CompoundObject = {
  networkDevices: NetworkDevice[];
  clients: Client[];
};
