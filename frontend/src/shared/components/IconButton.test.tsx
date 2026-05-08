import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { IconButton } from "./IconButton";

test("IconButton matches snapshot", () => {
  const { asFragment } = render(<IconButton label="Delete task">×</IconButton>);

  expect(asFragment()).toMatchSnapshot();
});
