import { NetworkDevice } from "../../../lib/types/NetworkDevices/NetworkDevice";
import NetworkWorkspace from "./NetworkWorkspace";

export default function NetworkDeviceDetailsCard({
  networkDevice,
}: {
  networkDevice: NetworkDevice;
}) {
  return <NetworkWorkspace devices={[networkDevice]} />;
}
