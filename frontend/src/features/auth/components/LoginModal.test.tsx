import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test, vi } from "vitest";
import { LoginModal } from "./LoginModal";

const { useAuthMock } = vi.hoisted(() => ({
  useAuthMock: vi.fn(),
}));

vi.mock("../AuthContext", () => ({
  useAuth: () => useAuthMock(),
}));

vi.mock("react-router-dom", async () => {
  const actual =
    await vi.importActual<typeof import("react-router-dom")>(
      "react-router-dom",
    );

  return {
    ...actual,
    useNavigate: () => vi.fn(),
  };
});

test("LoginModal matches snapshot", () => {
  useAuthMock.mockReturnValue({ setToken: vi.fn() });

  const { asFragment } = render(
    <MemoryRouter>
      <LoginModal open={true} onClose={() => undefined} />
    </MemoryRouter>,
  );

  expect(asFragment()).toMatchSnapshot();
});
