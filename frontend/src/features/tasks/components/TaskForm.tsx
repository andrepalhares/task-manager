import type { ChangeEvent } from "react";
import { Input } from "../../../shared/components/Input";
import { Label } from "../../../shared/components/Label";
import { TextArea } from "../../../shared/components/TextArea";
import { toDateTimeLocalInputValue } from "../../../shared/utils/formatDateTime";
import { TASK_STATUSES, type TaskStatus } from "../types";
import { getStatusLabel } from "../utils/statusMapping";

export type TaskFormState = {
  title: string;
  description: string;
  status: TaskStatus;
  dueDate: string; // datetime-local value (or "")
};

export const EMPTY_TASK_FORM: TaskFormState = {
  title: "",
  description: "",
  status: "Pending",
  dueDate: "",
};

export function buildFormStateFromTask(task: {
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string | null;
}): TaskFormState {
  return {
    title: task.title,
    description: task.description ?? "",
    status: task.status,
    dueDate: toDateTimeLocalInputValue(task.dueDate),
  };
}

type TaskFormProps = {
  state: TaskFormState;
  onChange: (next: TaskFormState) => void;
  disabled?: boolean;
};

export function TaskForm({ state, onChange, disabled = false }: TaskFormProps) {
  function update<K extends keyof TaskFormState>(
    key: K,
    value: TaskFormState[K],
  ) {
    onChange({ ...state, [key]: value });
  }

  return (
    <div className="space-y-4">
      <div>
        <Label htmlFor="task-title">Title</Label>
        <Input
          id="task-title"
          type="text"
          required
          maxLength={200}
          value={state.title}
          disabled={disabled}
          onChange={(e: ChangeEvent<HTMLInputElement>) =>
            update("title", e.target.value)
          }
        />
      </div>

      <div>
        <Label htmlFor="task-description">Description</Label>
        <TextArea
          id="task-description"
          maxLength={2000}
          value={state.description}
          disabled={disabled}
          onChange={(e: ChangeEvent<HTMLTextAreaElement>) =>
            update("description", e.target.value)
          }
        />
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <Label htmlFor="task-status">Status</Label>
          <select
            id="task-status"
            value={state.status}
            disabled={disabled}
            onChange={(e) => update("status", e.target.value as TaskStatus)}
            className={[
              "w-full rounded-md border border-pale-sky-200 bg-white px-3 py-2 text-sm",
              "text-pale-sky-900",
              "focus:outline-none focus:border-pale-sky-500 focus:ring-2 focus:ring-pale-sky-200",
              "disabled:bg-pale-sky-50 disabled:text-pale-sky-700 disabled:cursor-not-allowed",
            ].join(" ")}
          >
            {TASK_STATUSES.map((s) => (
              <option key={s} value={s}>
                {getStatusLabel(s)}
              </option>
            ))}
          </select>
        </div>

        <div>
          <Label htmlFor="task-due">Due date</Label>
          <Input
            id="task-due"
            type="datetime-local"
            value={state.dueDate}
            disabled={disabled}
            onChange={(e) => update("dueDate", e.target.value)}
          />
        </div>
      </div>
    </div>
  );
}
