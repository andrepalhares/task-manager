import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { Input } from "./Input";

test("Input matches snapshot", () => {
  const { asFragment } = render(
    <Input aria-label="Name" defaultValue="Ada" disabled />,
  );

  expect(asFragment()).toMatchSnapshot();
});
