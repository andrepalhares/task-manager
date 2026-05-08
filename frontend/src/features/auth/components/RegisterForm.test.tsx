import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { RegisterForm } from "./RegisterForm";

test("RegisterForm matches snapshot", () => {
  const { asFragment } = render(<RegisterForm onSuccess={() => undefined} />);

  expect(asFragment()).toMatchSnapshot();
});
