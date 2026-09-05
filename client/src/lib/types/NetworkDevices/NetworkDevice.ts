import { Port } from "./Port";

export type NetworkDevice = {
  id: string;
  host: string;
  typeOfNetworkDevice: string;
  networkDeviceName: string;
  generalInformation: string;
  portsOfNetworkDevice: Port[];
};
