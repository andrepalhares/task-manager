import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { StatusChip } from "../../../features/tasks/components/StatusChip";

describe("StatusChip", () => {
  it.each([
    ["Pending", /pending/i],
    ["InProgress", /in progress/i],
    ["Completed", /completed/i],
  ] as const)("renders the human label for %s", (status, label) => {
    render(<StatusChip status={status} />);

    expect(screen.getByText(label)).toBeInTheDocument();
  });
});
