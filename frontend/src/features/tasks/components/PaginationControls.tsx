import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "../../../shared/components/Button";

type PaginationControlsProps = {
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (next: number) => void;
};

export function PaginationControls({
  page,
  pageSize,
  totalCount,
  onPageChange,
}: PaginationControlsProps) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const hasPrev = page > 1;
  const hasNext = page < totalPages;

  return (
    <div className="flex items-center justify-center gap-3 pt-4">
      <Button
        variant="secondary"
        onClick={() => onPageChange(page - 1)}
        disabled={!hasPrev}
      >
        <ChevronLeft className="h-4 w-4" />
        Previous
      </Button>
      <span className="text-sm text-pale-sky-700">
        Page <span className="font-semibold">{page}</span> of{" "}
        <span className="font-semibold">{totalPages}</span>
      </span>
      <Button
        variant="secondary"
        onClick={() => onPageChange(page + 1)}
        disabled={!hasNext}
      >
        Next
        <ChevronRight className="h-4 w-4" />
      </Button>
    </div>
  );
}
