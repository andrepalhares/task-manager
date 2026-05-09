import { useState, type FormEvent } from "react";
import { toast } from "sonner";
import { extractApiErrorMessage } from "../../../shared/api/axiosClient";
import { Button } from "../../../shared/components/Button";
import { Input } from "../../../shared/components/Input";
import { Label } from "../../../shared/components/Label";
import { authApi } from "../auth.api";

type RegisterFormProps = { onSuccess: () => void };

export function RegisterForm({ onSuccess }: RegisterFormProps) {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await authApi.register({ name, email, password });
      onSuccess();
      toast.success("Account created successfully. Please log in now.");
    } catch (err) {
      setError(
        extractApiErrorMessage(err, "Registration failed. Please try again."),
      );
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      {error && (
        <div className="rounded-md border border-soft-blossom-200 bg-soft-blossom-50 px-3 py-2 text-sm text-soft-blossom-800">
          {error}
        </div>
      )}

      <div>
        <Label htmlFor="register-name">Name</Label>
        <Input
          id="register-name"
          type="text"
          autoComplete="name"
          required
          minLength={3}
          maxLength={100}
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={submitting}
        />
      </div>

      <div>
        <Label htmlFor="register-email">Email</Label>
        <Input
          id="register-email"
          type="email"
          autoComplete="email"
          required
          maxLength={100}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          disabled={submitting}
        />
      </div>

      <div>
        <Label htmlFor="register-password">Password</Label>
        <Input
          id="register-password"
          type="password"
          autoComplete="new-password"
          required
          minLength={8}
          maxLength={100}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={submitting}
        />
      </div>

      <Button type="submit" disabled={submitting} className="w-full">
        {submitting ? "Creating account…" : "Create account"}
      </Button>
    </form>
  );
}
