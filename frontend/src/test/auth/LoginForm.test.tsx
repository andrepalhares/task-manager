import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import { LoginForm } from "../../../features/auth/components/LoginForm";

vi.mock("../../../features/auth/auth.api", () => ({
  authApi: { login: vi.fn(), register: vi.fn() },
}));

vi.mock("../../../features/auth/AuthContext", () => ({
  useAuth: () => ({
    user: null,
    isAuthenticated: false,
    setToken: vi.fn(),
    logout: vi.fn(),
  }),
}));

describe("LoginForm", () => {
  it("renders email and password fields plus submit button", () => {
    render(
      <MemoryRouter>
        <LoginForm onSuccess={() => {}} />
      </MemoryRouter>,
    );

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /log in/i })).toBeInTheDocument();
  });
});
