export type ApiErrorResponse = {
  status: number;
  title: string;
  detail: string;
  errors?: { [key: string]: string[] };
};
