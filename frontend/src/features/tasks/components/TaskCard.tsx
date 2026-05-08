import { Eye, Pencil, Trash2 } from "lucide-react";
import { IconButton } from "../../../shared/components/IconButton";
import { formatDateTime } from "../../../shared/utils/formatDateTime";
import type { TaskDto } from "../types";
import { StatusChip } from "./StatusChip";

type TaskCardProps = {
  task: TaskDto;
  onView: (id: string) => void;
  onEdit: (id: string) => void;
  onDelete: (task: TaskDto) => void;
};

export function TaskCard({ task, onView, onEdit, onDelete }: TaskCardProps) {
  return (
    <article className="rounded-lg border border-pale-sky-100 bg-white p-4 shadow-sm transition-shadow hover:shadow-md">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <h3 className="truncate text-base font-semibold text-pale-sky-900">
            {task.title}
          </h3>
          <div className="mt-2 flex flex-wrap items-center gap-3 text-sm text-pale-sky-700">
            <StatusChip status={task.status} />
            <span>
              <span className="text-pale-sky-500">Due:</span>{" "}
              {formatDateTime(task.dueDate)}
            </span>
          </div>
        </div>

        <div className="flex shrink-0 items-center gap-1">
          <IconButton label="View task" onClick={() => onView(task.id)}>
            <Eye className="h-4 w-4" />
          </IconButton>
          <IconButton label="Edit task" onClick={() => onEdit(task.id)}>
            <Pencil className="h-4 w-4" />
          </IconButton>
          <IconButton
            label="Delete task"
            tone="danger"
            onClick={() => onDelete(task)}
          >
            <Trash2 className="h-4 w-4" />
          </IconButton>
        </div>
      </div>
    </article>
  );
}
