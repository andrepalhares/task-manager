import { render } from "@testing-library/react";
import { expect, test } from "vitest";
import { PaginationControls } from "./PaginationControls";

test("PaginationControls matches snapshot", () => {
  const { asFragment } = render(
    <PaginationControls
      page={2}
      pageSize={10}
      totalCount={25}
      onPageChange={() => undefined}
    />,
  );

  expect(asFragment()).toMatchSnapshot();
});
