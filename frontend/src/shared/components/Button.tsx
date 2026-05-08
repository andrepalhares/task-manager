import type { ButtonHTMLAttributes, ReactNode } from "react";

type Variant = "primary" | "secondary" | "danger";

const VARIANT_CLASSES: Record<Variant, string> = {
  primary:
    "bg-pale-sky-600 text-white hover:bg-pale-sky-700 focus-visible:ring-pale-sky-500",
  secondary:
    "bg-white text-pale-sky-800 border border-pale-sky-200 hover:bg-pale-sky-50 focus-visible:ring-pale-sky-300",
  danger:
    "bg-soft-blossom-600 text-white hover:bg-soft-blossom-700 focus-visible:ring-soft-blossom-500",
};

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: Variant;
  children: ReactNode;
};

export function Button({
  variant = "primary",
  className = "",
  children,
  ...rest
}: ButtonProps) {
  return (
    <button
      {...rest}
      className={[
        "inline-flex items-center justify-center gap-2 rounded-md px-4 py-2 text-sm font-medium",
        "transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2",
        "disabled:opacity-50 disabled:cursor-not-allowed",
        VARIANT_CLASSES[variant],
        className,
      ].join(" ")}
    >
      {children}
    </button>
  );
}
