import type { LabelHTMLAttributes, ReactNode } from "react";

type LabelProps = LabelHTMLAttributes<HTMLLabelElement> & {
  children: ReactNode;
};

export function Label({ className = "", children, ...rest }: LabelProps) {
  return (
    <label
      {...rest}
      className={`block text-sm font-medium text-pale-sky-900 mb-1 ${className}`}
    >
      {children}
    </label>
  );
}
