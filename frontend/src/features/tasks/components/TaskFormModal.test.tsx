import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { TaskFormModal } from "./TaskFormModal";

test("TaskFormModal matches snapshot in create mode", () => {
  const { asFragment } = render(
    <TaskFormModal
      mode="create"
      open={true}
      onClose={() => undefined}
      onSaved={() => undefined}
    />,
  );

  expect(asFragment()).toMatchSnapshot();
});
