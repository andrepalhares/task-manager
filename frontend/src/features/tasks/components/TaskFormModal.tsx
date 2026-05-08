import { useEffect, useState } from "react";
import { toast } from "sonner";
import { extractApiErrorMessage } from "../../../shared/api/axiosClient";
import { Button } from "../../../shared/components/Button";
import { Modal } from "../../../shared/components/Modal";
import { fromDateTimeLocalInputValue } from "../../../shared/utils/formatDateTime";
import { tasksApi } from "../tasks.api";
import {
  EMPTY_TASK_FORM,
  TaskForm,
  buildFormStateFromTask,
  type TaskFormState,
} from "./TaskForm";

type Mode = "create" | "view" | "edit";

type TaskFormModalProps = {
  mode: Mode;
  taskId?: string; // required for view/edit
  open: boolean;
  onClose: () => void;
  onSaved: () => void; // called after a successful create/edit
};

const TITLES: Record<Mode, string> = {
  create: "Create a new task",
  view: "Task details",
  edit: "Edit task",
};

export function TaskFormModal({
  mode,
  taskId,
  open,
  onClose,
  onSaved,
}: TaskFormModalProps) {
  const [state, setState] = useState<TaskFormState>(EMPTY_TASK_FORM);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load (or reset) form whenever the modal opens.
  useEffect(() => {
    if (!open) return;
    setError(null);

    if (mode === "create") {
      setState(EMPTY_TASK_FORM);
      return;
    }

    if (!taskId) return;

    setLoading(true);
    tasksApi
      .getById(taskId)
      .then((task) => setState(buildFormStateFromTask(task)))
      .catch((err) =>
        setError(extractApiErrorMessage(err, "Could not load this task.")),
      )
      .finally(() => setLoading(false));
  }, [open, mode, taskId]);

  async function onSubmit() {
    if (mode === "view") return;

    if (!state.title.trim()) {
      setError("Title is required.");
      return;
    }

    setError(null);
    setSubmitting(true);
    try {
      const payload = {
        title: state.title.trim(),
        description: state.description.trim() ? state.description : null,
        status: state.status,
        dueDate: fromDateTimeLocalInputValue(state.dueDate),
      };

      if (mode === "create") {
        await tasksApi.create(payload);
        toast.success("Task created.");
      } else if (mode === "edit" && taskId) {
        await tasksApi.update(taskId, payload);
        toast.success("Task updated.");
      }

      onSaved();
      onClose();
    } catch (err) {
      setError(extractApiErrorMessage(err, "Could not save the task."));
    } finally {
      setSubmitting(false);
    }
  }

  const isReadonly = mode === "view";

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={TITLES[mode]}
      closeDisabled={submitting}
      footer={
        isReadonly ? (
          <Button variant="secondary" onClick={onClose}>
            Close
          </Button>
        ) : (
          <>
            <Button variant="secondary" onClick={onClose} disabled={submitting}>
              Cancel
            </Button>
            <Button onClick={onSubmit} disabled={submitting || loading}>
              {submitting
                ? "Saving…"
                : mode === "create"
                  ? "Create task"
                  : "Save changes"}
            </Button>
          </>
        )
      }
    >
      {loading ? (
        <p className="py-8 text-center text-sm text-pale-sky-600">Loading…</p>
      ) : (
        <>
          {error && (
            <div className="mb-4 rounded-md border border-soft-blossom-200 bg-soft-blossom-50 px-3 py-2 text-sm text-soft-blossom-800">
              {error}
            </div>
          )}
          <TaskForm
            state={state}
            onChange={setState}
            disabled={isReadonly || submitting}
          />
        </>
      )}
    </Modal>
  );
}
