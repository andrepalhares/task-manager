import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import {
  EMPTY_TASK_FORM,
  TaskForm,
} from "../../../features/tasks/components/TaskForm";

describe("TaskForm", () => {
  it("renders title, description, status and due date inputs", () => {
    render(<TaskForm state={EMPTY_TASK_FORM} onChange={() => {}} />);

    expect(screen.getByLabelText(/title/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/description/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/status/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/due date/i)).toBeInTheDocument();
  });

  it("populates fields from provided state", () => {
    render(
      <TaskForm
        state={{
          title: "Buy groceries",
          description: "Milk and bread",
          status: "InProgress",
          dueDate: "",
        }}
        onChange={() => {}}
      />,
    );

    expect(screen.getByLabelText(/title/i)).toHaveValue("Buy groceries");
    expect(screen.getByLabelText(/description/i)).toHaveValue("Milk and bread");
    expect(screen.getByLabelText(/status/i)).toHaveValue("InProgress");
  });
});
