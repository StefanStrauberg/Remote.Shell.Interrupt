import { Typography } from "@mui/material";
import { CompoundObject } from "../../lib/types/NetworkDevices/CompoundObject";
import ClientBox from "./ClientPart/ClientBox";
import RouterBox from "./RouterPart/RouterBox";

type Props = {
  data: CompoundObject | undefined;
};

export default function MainPageList({ data }: Props) {
  if (!data) {
    return <>Данных нет</>;
  }

  return (
    <>
      {data.clients.length > 0 ? (
        data.clients.map((client) => (
          <ClientBox key={client.id} client={client} />
        ))
      ) : (
        <Typography>Клиент не найден</Typography>
      )}
      {data.networkDevices.length > 0 ? (
        data.networkDevices.map((networkDevice) => (
          <RouterBox key={networkDevice.id} networkDevice={networkDevice} />
        ))
      ) : (
        <Typography>Маршрутизаторы не найдены</Typography>
      )}
    </>
  );
}
