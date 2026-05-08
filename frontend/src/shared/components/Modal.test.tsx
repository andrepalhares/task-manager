import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { Modal } from "./Modal";

test("Modal matches snapshot", () => {
  const { asFragment } = render(
    <Modal open={true} onClose={() => undefined} title="Settings">
      <p>Content</p>
    </Modal>,
  );

  expect(asFragment()).toMatchSnapshot();
});
