import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test, vi } from "vitest";
import { LandingPage } from "./LandingPage";

const { useAuthMock } = vi.hoisted(() => ({
  useAuthMock: vi.fn(),
}));

vi.mock("../features/auth/AuthContext", () => ({
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

test("LandingPage matches snapshot for anonymous users", () => {
  useAuthMock.mockReturnValue({ isAuthenticated: false });

  const { asFragment } = render(<LandingPage />, {
    wrapper: ({ children }) => <MemoryRouter>{children}</MemoryRouter>,
  });

  expect(asFragment()).toMatchSnapshot();
});

test("LandingPage matches snapshot for authenticated users", () => {
  useAuthMock.mockReturnValue({ isAuthenticated: true });

  const { asFragment } = render(<LandingPage />, {
    wrapper: ({ children }) => <MemoryRouter>{children}</MemoryRouter>,
  });

  expect(asFragment()).toMatchSnapshot();
});
