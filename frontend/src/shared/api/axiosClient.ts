import axios, { AxiosError } from "axios";
import { toast } from "sonner";

export const TOKEN_STORAGE_KEY = "taskmanager.token";

export const apiClient = axios.create({
  baseURL: "/api",
  headers: { "Content-Type": "application/json" },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

let onUnauthorized: (() => void) | null = null;

export function registerUnauthorizedHandler(handler: () => void) {
  onUnauthorized = handler;
}

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401 && onUnauthorized) {
      const url = error.config?.url ?? "";
      const isLoginAttempt = url.includes("/auth/login");
      if (!isLoginAttempt) {
        toast.error("Your session has expired. Please log in again.");
        onUnauthorized();
      }
    }
    return Promise.reject(error);
  },
);

export function extractApiErrorMessage(
  error: unknown,
  fallback: string,
): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as
      | { title?: string; detail?: string; errors?: Record<string, string[]> }
      | undefined;

    if (data?.errors) {
      const firstField = Object.values(data.errors)[0];
      if (firstField && firstField.length > 0) return firstField[0];
    }
    if (data?.detail) return data.detail;
    if (data?.title) return data.title;
  }
  return fallback;
}
