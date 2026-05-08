import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test, vi } from "vitest";
import { Navbar } from "./Navbar";

const { useAuthMock } = vi.hoisted(() => ({
  useAuthMock: vi.fn(),
}));

vi.mock("../../features/auth/AuthContext", () => ({
  useAuth: () => useAuthMock(),
}));

test("Navbar matches snapshot for anonymous users", () => {
  useAuthMock.mockReturnValue({
    user: null,
    isAuthenticated: false,
    logout: vi.fn(),
    setToken: vi.fn(),
  });

  const { asFragment } = render(<Navbar />, {
    wrapper: ({ children }) => <MemoryRouter>{children}</MemoryRouter>,
  });

  expect(asFragment()).toMatchSnapshot();
});
