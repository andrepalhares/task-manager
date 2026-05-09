import { Plus } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import { toast } from "sonner";
import { DeleteTaskModal } from "../features/tasks/components/DeleteTaskModal";
import { PaginationControls } from "../features/tasks/components/PaginationControls";
import { TaskCard } from "../features/tasks/components/TaskCard";
import { TaskFormModal } from "../features/tasks/components/TaskFormModal";
import { tasksApi } from "../features/tasks/tasks.api";
import type { PaginatedTasks, TaskDto } from "../features/tasks/types";
import { extractApiErrorMessage } from "../shared/api/axiosClient";
import { Button } from "../shared/components/Button";

type ModalState =
  | { kind: "none" }
  | { kind: "create" }
  | { kind: "view"; taskId: string }
  | { kind: "edit"; taskId: string }
  | { kind: "delete"; task: TaskDto };

export function TasksPage() {
  const [page, setPage] = useState(1);
  const [data, setData] = useState<PaginatedTasks | null>(null);
  const [loading, setLoading] = useState(false);
  const [modal, setModal] = useState<ModalState>({ kind: "none" });

  const fetchTasks = useCallback(async (targetPage: number) => {
    setLoading(true);
    try {
      const result = await tasksApi.list(targetPage);
      setData(result);
    } catch (err) {
      toast.error(extractApiErrorMessage(err, "Could not load your tasks."));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchTasks(page);
  }, [page, fetchTasks]);

  const closeModal = () => setModal({ kind: "none" });
  const refetch = () => fetchTasks(page);

  return (
    <main className="mx-auto max-w-5xl px-4 py-8">
      <div className="mb-6 flex items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-pale-sky-900">Tasks</h1>
          <p className="mt-1 text-sm text-pale-sky-600">
            {data
              ? `${data.totalCount} task${data.totalCount === 1 ? "" : "s"}`
              : "..."}
          </p>
        </div>
        <Button onClick={() => setModal({ kind: "create" })}>
          <Plus className="h-4 w-4" />
          New task
        </Button>
      </div>

      {loading && !data && (
        <p className="py-12 text-center text-sm text-pale-sky-600">
          Loading tasks…
        </p>
      )}

      {data && data.items.length === 0 && (
        <div className="rounded-lg border border-dashed border-pale-sky-200 bg-white py-12 text-center">
          <p className="text-sm text-pale-sky-700">You don't have any task.</p>
        </div>
      )}

      {data && data.items.length > 0 && (
        <>
          <div className="space-y-3">
            {data.items.map((task) => (
              <TaskCard
                key={task.id}
                task={task}
                onView={(id) => setModal({ kind: "view", taskId: id })}
                onEdit={(id) => setModal({ kind: "edit", taskId: id })}
                onDelete={(t) => setModal({ kind: "delete", task: t })}
              />
            ))}
          </div>

          <PaginationControls
            page={data.page}
            pageSize={data.pageSize}
            totalCount={data.totalCount}
            onPageChange={setPage}
          />
        </>
      )}

      <TaskFormModal
        mode="create"
        open={modal.kind === "create"}
        onClose={closeModal}
        onSaved={() => {
          if (page !== 1) setPage(1);
          else refetch();
        }}
      />

      <TaskFormModal
        mode="view"
        taskId={modal.kind === "view" ? modal.taskId : undefined}
        open={modal.kind === "view"}
        onClose={closeModal}
        onSaved={refetch}
      />

      <TaskFormModal
        mode="edit"
        taskId={modal.kind === "edit" ? modal.taskId : undefined}
        open={modal.kind === "edit"}
        onClose={closeModal}
        onSaved={refetch}
      />

      <DeleteTaskModal
        open={modal.kind === "delete"}
        taskId={modal.kind === "delete" ? modal.task.id : null}
        taskTitle={modal.kind === "delete" ? modal.task.title : null}
        onClose={closeModal}
        onDeleted={() => {
          if (data && data.items.length === 1 && page > 1) setPage(page - 1);
          else refetch();
        }}
      />
    </main>
  );
}
