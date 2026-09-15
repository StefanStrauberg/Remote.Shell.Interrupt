import { DateArg, format, isValid, setDefaultOptions } from "date-fns";
import { ru } from "date-fns/locale";

setDefaultOptions({ locale: ru });

/** Formats a date as "dd MMM yyyy" using the Russian locale. */
export function formatDate(date: DateArg<Date>) {
  return format(date, "dd MMM yyyy");
}

/** Formats an API date, treating invalid and .NET minimum values as absent. */
export function formatApiDate(value?: string | null): string | null {
  if (!value) return null;

  const date = new Date(value);
  if (!isValid(date) || date.getUTCFullYear() <= 1) return null;

  return formatDate(date);
}

/** VLAN tags available to regular 802.1Q networks. */
export function isValidVlanId(value: number | null): value is number {
  return (
    Number.isInteger(value) && value !== null && value >= 1 && value <= 4094
  );
}

export function isGuid(value: unknown): value is string {
  return (
    typeof value === "string" &&
    /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(
      value
    )
  );
}
