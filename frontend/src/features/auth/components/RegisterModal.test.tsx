import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { RegisterModal } from "./RegisterModal";

test("RegisterModal matches snapshot", () => {
  const { asFragment } = render(
    <RegisterModal open={true} onClose={() => undefined} />,
  );

  expect(asFragment()).toMatchSnapshot();
});
