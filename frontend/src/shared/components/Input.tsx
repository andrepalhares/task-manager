import { forwardRef, type InputHTMLAttributes } from "react";

export const Input = forwardRef<
  HTMLInputElement,
  InputHTMLAttributes<HTMLInputElement>
>(function Input({ className = "", ...rest }, ref) {
  return (
    <input
      ref={ref}
      {...rest}
      className={[
        "w-full rounded-md border border-pale-sky-200 bg-white px-3 py-2 text-sm",
        "text-pale-sky-900 placeholder:text-pale-sky-400",
        "focus:outline-none focus:border-pale-sky-500 focus:ring-2 focus:ring-pale-sky-200",
        "disabled:bg-pale-sky-50 disabled:text-pale-sky-700 disabled:cursor-not-allowed",
        className,
      ].join(" ")}
    />
  );
});
