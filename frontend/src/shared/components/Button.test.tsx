import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { Button } from "./Button";

test("Button matches snapshot", () => {
  const { asFragment } = render(<Button>Save</Button>);

  expect(asFragment()).toMatchSnapshot();
});
