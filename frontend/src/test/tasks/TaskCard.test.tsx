import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { TaskCard } from "../../../features/tasks/components/TaskCard";
import type { TaskDto } from "../../../features/tasks/types";

const task: TaskDto = {
  id: "11111111-1111-1111-1111-111111111111",
  title: "Buy groceries",
  description: "Milk and bread",
  status: "Pending",
  dueDate: null,
  userId: "22222222-2222-2222-2222-222222222222",
};

describe("TaskCard", () => {
  it("renders the task title and action buttons", () => {
    render(
      <TaskCard
        task={task}
        onView={() => {}}
        onEdit={() => {}}
        onDelete={() => {}}
      />,
    );

    expect(screen.getByText("Buy groceries")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /view task/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /edit task/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /delete task/i })).toBeInTheDocument();
  });
});
