import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test, vi } from "vitest";
import { LoginForm } from "./LoginForm";

const { authApiMock, useAuthMock, toastMock, navigateMock } = vi.hoisted(
  () => ({
    authApiMock: {
      login: vi.fn(),
      register: vi.fn(),
    },
    useAuthMock: vi.fn(),
    toastMock: {
      success: vi.fn(),
      error: vi.fn(),
    },
    navigateMock: vi.fn(),
  }),
);

vi.mock("../auth.api", () => ({
  authApi: authApiMock,
}));

vi.mock("../AuthContext", () => ({
  useAuth: () => useAuthMock(),
}));

vi.mock("sonner", () => ({
  toast: toastMock,
}));

vi.mock("react-router-dom", async () => {
  const actual =
    await vi.importActual<typeof import("react-router-dom")>(
      "react-router-dom",
    );

  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

test("LoginForm matches snapshot", () => {
  useAuthMock.mockReturnValue({ setToken: vi.fn() });

  const { asFragment } = render(
    <MemoryRouter>
      <LoginForm onSuccess={() => undefined} />
    </MemoryRouter>,
  );

  expect(asFragment()).toMatchSnapshot();
});
