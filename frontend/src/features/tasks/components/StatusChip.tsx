import type { TaskStatus } from "../types";
import { getStatusLabel } from "../utils/statusMapping";

const CHIP_CLASSES: Record<TaskStatus, string> = {
  Pending: "bg-pearl-beige-100 text-pearl-beige-800 border-pearl-beige-200",
  InProgress: "bg-pale-sky-100 text-pale-sky-800 border-pale-sky-200",
  Completed: "bg-celadon-100 text-celadon-800 border-celadon-200",
};

export function StatusChip({ status }: { status: TaskStatus }) {
  return (
    <span
      className={[
        "inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-medium",
        CHIP_CLASSES[status],
      ].join(" ")}
    >
      {getStatusLabel(status)}
    </span>
  );
}
