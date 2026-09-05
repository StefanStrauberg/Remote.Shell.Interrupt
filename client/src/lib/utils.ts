import { DateArg, format, setDefaultOptions } from "date-fns";
import { ru } from "date-fns/locale";

setDefaultOptions({ locale: ru });

/** Formats a date as "dd MMM yyyy" using the Russian locale. */
export function formatDate(date: DateArg<Date>) {
  return format(date, "dd MMM yyyy");
}
