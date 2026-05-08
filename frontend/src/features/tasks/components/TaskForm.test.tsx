import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { EMPTY_TASK_FORM, TaskForm } from "./TaskForm";

test("TaskForm matches snapshot", () => {
  const { asFragment } = render(
    <TaskForm state={{ ...EMPTY_TASK_FORM }} onChange={() => undefined} />,
  );

  expect(asFragment()).toMatchSnapshot();
});
