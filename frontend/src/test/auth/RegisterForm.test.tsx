import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { RegisterForm } from "../../../features/auth/components/RegisterForm";

vi.mock("../../../features/auth/auth.api", () => ({
  authApi: { login: vi.fn(), register: vi.fn() },
}));

describe("RegisterForm", () => {
  it("renders name, email, password fields and submit button", () => {
    render(<RegisterForm onSuccess={() => {}} />);

    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /create account/i }),
    ).toBeInTheDocument();
  });
});
