import { apiClient } from "../../shared/api/axiosClient";
import type {
  CreateTaskRequest,
  PaginatedTasks,
  TaskDto,
  UpdateTaskRequest,
} from "./types";

export const tasksApi = {
  async list(page: number): Promise<PaginatedTasks> {
    const response = await apiClient.get<PaginatedTasks>("/tasks", {
      params: { page },
    });
    return response.data;
  },

  async getById(id: string): Promise<TaskDto> {
    const response = await apiClient.get<TaskDto>(`/tasks/${id}`);
    return response.data;
  },

  async create(request: CreateTaskRequest): Promise<TaskDto> {
    const response = await apiClient.post<TaskDto>("/tasks", request);
    return response.data;
  },

  async update(id: string, request: UpdateTaskRequest): Promise<TaskDto> {
    const response = await apiClient.put<TaskDto>(`/tasks/${id}`, request);
    return response.data;
  },

  async remove(id: string): Promise<void> {
    await apiClient.delete(`/tasks/${id}`);
  },
};
