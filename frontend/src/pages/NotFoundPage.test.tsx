import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test } from "vitest";
import { NotFoundPage } from "./NotFoundPage";

test("NotFoundPage matches snapshot", () => {
  const { asFragment } = render(<NotFoundPage />, {
    wrapper: ({ children }) => <MemoryRouter>{children}</MemoryRouter>,
  });
  expect(asFragment()).toMatchSnapshot();
});
