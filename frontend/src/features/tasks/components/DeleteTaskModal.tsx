import { useState } from "react";
import { toast } from "sonner";
import { extractApiErrorMessage } from "../../../shared/api/axiosClient";
import { Button } from "../../../shared/components/Button";
import { Modal } from "../../../shared/components/Modal";
import { tasksApi } from "../tasks.api";

type DeleteTaskModalProps = {
  open: boolean;
  taskId: string | null;
  taskTitle: string | null;
  onClose: () => void;
  onDeleted: () => void;
};

export function DeleteTaskModal({
  open,
  taskId,
  taskTitle,
  onClose,
  onDeleted,
}: DeleteTaskModalProps) {
  const [submitting, setSubmitting] = useState(false);

  async function onConfirm() {
    if (!taskId) return;
    setSubmitting(true);
    try {
      await tasksApi.remove(taskId);
      toast.success("Task deleted.");
      onDeleted();
      onClose();
    } catch (err) {
      toast.error(extractApiErrorMessage(err, "Could not delete the task."));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      title="Delete task"
      closeDisabled={submitting}
      footer={
        <>
          <Button variant="secondary" onClick={onClose} disabled={submitting}>
            Cancel
          </Button>
          <Button variant="danger" onClick={onConfirm} disabled={submitting}>
            {submitting ? "Deleting…" : "Delete"}
          </Button>
        </>
      }
    >
      <p className="text-sm text-pale-sky-800">
        Are you sure you want to delete{" "}
        <span className="font-medium">"{taskTitle ?? "this task"}"</span>? This
        action cannot be undone.
      </p>
    </Modal>
  );
}
