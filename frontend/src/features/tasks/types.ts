export type TaskStatus = "Pending" | "InProgress" | "Completed";

export const TASK_STATUSES: TaskStatus[] = [
  "Pending",
  "InProgress",
  "Completed",
];

export type TaskDto = {
  id: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string | null; // ISO datetime
  userId: string;
};

export type PaginatedTasks = {
  items: TaskDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type CreateTaskRequest = {
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string | null; // ISO datetime
};

export type UpdateTaskRequest = CreateTaskRequest;
