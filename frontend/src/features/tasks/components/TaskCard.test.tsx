import { render } from "@testing-library/react";
import { expect, test, vi } from "vitest";
import type { TaskDto } from "../types";
import { TaskCard } from "./TaskCard";

vi.mock("../../../shared/utils/formatDateTime", () => ({
  formatDateTime: vi.fn(() => "Apr 5, 2026, 9:30 AM"),
}));

test("TaskCard matches snapshot", () => {
  const task: TaskDto = {
    id: "task-1",
    title: "Write tests",
    description: "Cover the UI",
    status: "InProgress",
    dueDate: "2026-04-05T09:30:00.000Z",
    userId: "user-1",
  };

  const { asFragment } = render(
    <TaskCard
      task={task}
      onView={() => undefined}
      onEdit={() => undefined}
      onDelete={() => undefined}
    />,
  );

  expect(asFragment()).toMatchSnapshot();
});
