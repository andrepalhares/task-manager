import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { DeleteTaskModal } from "./DeleteTaskModal";

test("DeleteTaskModal matches snapshot", () => {
  const { asFragment } = render(
    <DeleteTaskModal
      open={true}
      taskId="task-1"
      taskTitle="Write tests"
      onClose={() => undefined}
      onDeleted={() => undefined}
    />,
  );

  expect(asFragment()).toMatchSnapshot();
});
