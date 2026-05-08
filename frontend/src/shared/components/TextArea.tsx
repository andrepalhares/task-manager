import { forwardRef, type TextareaHTMLAttributes } from "react";

export const TextArea = forwardRef<
  HTMLTextAreaElement,
  TextareaHTMLAttributes<HTMLTextAreaElement>
>(function TextArea({ className = "", ...rest }, ref) {
  return (
    <textarea
      ref={ref}
      {...rest}
      className={[
        "w-full rounded-md border border-pale-sky-200 bg-white px-3 py-2 text-sm",
        "text-pale-sky-900 placeholder:text-pale-sky-400",
        "focus:outline-none focus:border-pale-sky-500 focus:ring-2 focus:ring-pale-sky-200",
        "disabled:bg-pale-sky-50 disabled:text-pale-sky-700 disabled:cursor-not-allowed",
        "min-h-[100px] resize-y",
        className,
      ].join(" ")}
    />
  );
});
