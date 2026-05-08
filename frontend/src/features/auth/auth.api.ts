import { apiClient } from "../../shared/api/axiosClient";
import type { LoginRequest, LoginResponse, RegisterRequest } from "./types";

export const authApi = {
  async register(request: RegisterRequest): Promise<void> {
    await apiClient.post("/auth/register", request);
  },

  async login(request: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>(
      "/auth/login",
      request,
    );
    return response.data;
  },
};
