import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { StatusChip } from "./StatusChip";

test.each([["Pending"], ["InProgress"], ["Completed"]] as const)(
  "StatusChip matches snapshot for %s",
  (status) => {
    const { asFragment } = render(<StatusChip status={status} />);

    expect(asFragment()).toMatchSnapshot();
  },
);
