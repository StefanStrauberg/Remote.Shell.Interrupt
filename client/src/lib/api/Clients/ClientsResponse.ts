import { ClientShort } from "../../types/Clients/ClientShort";
import { PaginationMetadata } from "../../types/Common/PaginationMetadata";

export type ClientsResponse = {
  data: ClientShort[];
  pagination: PaginationMetadata;
};
