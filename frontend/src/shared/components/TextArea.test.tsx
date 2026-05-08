import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { TextArea } from "./TextArea";

test("TextArea matches snapshot", () => {
  const { asFragment } = render(
    <TextArea aria-label="Notes" defaultValue="Hello" disabled />,
  );

  expect(asFragment()).toMatchSnapshot();
});
