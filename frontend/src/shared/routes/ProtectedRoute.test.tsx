import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test, vi } from "vitest";
import { ProtectedRoute } from "./ProtectedRoute";

const { useAuthMock } = vi.hoisted(() => ({
  useAuthMock: vi.fn(),
}));

vi.mock("../../features/auth/AuthContext", () => ({
  useAuth: () => useAuthMock(),
}));

vi.mock("react-router-dom", async () => {
  const actual =
    await vi.importActual<typeof import("react-router-dom")>(
      "react-router-dom",
    );

  return {
    ...actual,
    Navigate: ({ to, replace }: { to: string; replace?: boolean }) => (
      <span
        data-testid="navigate"
        data-to={to}
        data-replace={String(replace)}
      />
    ),
  };
});

test("ProtectedRoute matches snapshot for anonymous users", () => {
  useAuthMock.mockReturnValue({ isAuthenticated: false });

  const { asFragment } = render(
    <MemoryRouter>
      <ProtectedRoute>
        <div>Secret area</div>
      </ProtectedRoute>
    </MemoryRouter>,
  );

  expect(asFragment()).toMatchSnapshot();
});

test("ProtectedRoute matches snapshot for authenticated users", () => {
  useAuthMock.mockReturnValue({ isAuthenticated: true });

  const { asFragment } = render(
    <MemoryRouter>
      <ProtectedRoute>
        <div>Secret area</div>
      </ProtectedRoute>
    </MemoryRouter>,
  );

  expect(asFragment()).toMatchSnapshot();
});
