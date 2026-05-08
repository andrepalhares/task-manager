/**
 * Formats an ISO datetime string for display.
 * Returns an em dash for null/undefined input so the UI never shows "null".
 */
export function formatDateTime(iso: string | null | undefined): string {
  if (!iso) return "—";

  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) return "—";

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
}

/**
 * Converts an ISO datetime string into the format expected by
 * `<input type="datetime-local">` (YYYY-MM-DDTHH:mm), in the user's
 * local timezone. Returns an empty string for null input.
 */
export function toDateTimeLocalInputValue(
  iso: string | null | undefined,
): string {
  if (!iso) return "";

  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) return "";

  // Adjust to local timezone, then strip seconds/milliseconds + the trailing Z
  const tzOffsetMs = date.getTimezoneOffset() * 60_000;
  const local = new Date(date.getTime() - tzOffsetMs);
  return local.toISOString().slice(0, 16);
}

/**
 * Converts the value of a `<input type="datetime-local">` into an ISO string
 * suitable for the API. Returns null if the input is empty.
 */
export function fromDateTimeLocalInputValue(value: string): string | null {
  if (!value) return null;
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return null;
  return date.toISOString();
}
