import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { Label } from "./Label";

test("Label matches snapshot", () => {
  const { asFragment } = render(<Label htmlFor="name">Name</Label>);

  expect(asFragment()).toMatchSnapshot();
});
