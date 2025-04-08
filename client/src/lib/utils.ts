import { DateArg, format, setDefaultOptions } from "date-fns";
import { ru } from "date-fns/locale";

export function formatDate(date: DateArg<Date>) {
  setDefaultOptions({ locale: ru });
  return format(date, "dd MMM yyyy");
}
