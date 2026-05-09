import { X } from "lucide-react";
import { useEffect, type ReactNode } from "react";
import { IconButton } from "./IconButton";

type ModalProps = {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  footer?: ReactNode;
  closeDisabled?: boolean;
};

export function Modal({
  open,
  onClose,
  title,
  children,
  footer,
  closeDisabled,
}: ModalProps) {
  useEffect(() => {
    if (!open) return;

    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape" && !closeDisabled) onClose();
    };
    document.addEventListener("keydown", onKey);

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    return () => {
      document.removeEventListener("keydown", onKey);
      document.body.style.overflow = previousOverflow;
    };
  }, [open, onClose, closeDisabled]);

  if (!open) return null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-pale-sky-950/40 p-4"
      onClick={() => !closeDisabled && onClose()}
      role="dialog"
      aria-modal="true"
      aria-label={title}
    >
      <div
        className="w-full max-w-lg rounded-lg bg-white shadow-xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between border-b border-pale-sky-100 px-5 py-3">
          <h2 className="text-base font-semibold text-pale-sky-900">{title}</h2>
          <IconButton label="Close" onClick={onClose} disabled={closeDisabled}>
            <X className="h-4 w-4" />
          </IconButton>
        </div>
        <div className="px-5 py-4">{children}</div>
        {footer && (
          <div className="flex justify-end gap-2 border-t border-pale-sky-100 bg-pale-sky-50 px-5 py-3 rounded-b-lg">
            {footer}
          </div>
        )}
      </div>
    </div>
  );
}
