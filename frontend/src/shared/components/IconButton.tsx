import type { ButtonHTMLAttributes, ReactNode } from "react";

type Tone = "neutral" | "danger";

const TONE_CLASSES: Record<Tone, string> = {
  neutral:
    "text-pale-sky-700 hover:bg-pale-sky-100 focus-visible:ring-pale-sky-300",
  danger:
    "text-soft-blossom-700 hover:bg-soft-blossom-100 focus-visible:ring-soft-blossom-300",
};

type IconButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  tone?: Tone;
  label: string; // accessible label, never visually rendered
  children: ReactNode;
};

export function IconButton({
  tone = "neutral",
  label,
  className = "",
  children,
  ...rest
}: IconButtonProps) {
  return (
    <button
      {...rest}
      type="button"
      aria-label={label}
      title={label}
      className={[
        "inline-flex h-9 w-9 items-center justify-center rounded-md",
        "transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2",
        "disabled:opacity-40 disabled:cursor-not-allowed",
        TONE_CLASSES[tone],
        className,
      ].join(" ")}
    >
      {children}
    </button>
  );
}
