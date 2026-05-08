import type { TaskStatus } from "../types";

const STATUS_LABELS: Record<TaskStatus, string> = {
  Pending: "Pending",
  InProgress: "In Progress",
  Completed: "Completed",
};

export function getStatusLabel(status: TaskStatus): string {
  return STATUS_LABELS[status];
}
