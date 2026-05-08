import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { extractApiErrorMessage } from "../../../shared/api/axiosClient";
import { Button } from "../../../shared/components/Button";
import { Input } from "../../../shared/components/Input";
import { Label } from "../../../shared/components/Label";
import { authApi } from "../auth.api";
import { useAuth } from "../AuthContext";

type LoginFormProps = { onSuccess: () => void };

export function LoginForm({ onSuccess }: LoginFormProps) {
  const navigate = useNavigate();
  const { setToken } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      const { accessToken } = await authApi.login({ email, password });
      setToken(accessToken);
      onSuccess();
      navigate("/tasks", { replace: true });
      toast.success("Welcome back!");
    } catch (err) {
      setError(extractApiErrorMessage(err, "Login failed."));
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
        <Label htmlFor="login-email">Email</Label>
        <Input
          id="login-email"
          type="email"
          autoComplete="email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          disabled={submitting}
        />
      </div>

      <div>
        <Label htmlFor="login-password">Password</Label>
        <Input
          id="login-password"
          type="password"
          autoComplete="current-password"
          required
          minLength={1}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={submitting}
        />
      </div>

      <Button type="submit" disabled={submitting} className="w-full">
        {submitting ? "Logging in…" : "Log in"}
      </Button>
    </form>
  );
}
