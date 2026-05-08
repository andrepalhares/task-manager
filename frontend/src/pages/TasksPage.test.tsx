import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { expect, test, vi } from "vitest";
import { TasksPage } from "./TasksPage";

vi.mock("../features/tasks/tasks.api", () => ({
  tasksApi: {
    list: vi.fn(() => new Promise(() => undefined)),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    remove: vi.fn(),
  },
}));

test("TasksPage matches snapshot while loading", async () => {
  const { asFragment } = render(
    <MemoryRouter>
      <TasksPage />
    </MemoryRouter>,
  );

  await waitFor(() => {
    expect(screen.getByText(/Loading tasks/i)).toBeInTheDocument();
  });

  expect(asFragment()).toMatchSnapshot();
});
