import { useState } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../features/auth/AuthContext";
import { RegisterModal } from "../features/auth/components/RegisterModal";
import { Button } from "../shared/components/Button";

export function LandingPage() {
  const { isAuthenticated } = useAuth();
  const [registerOpen, setRegisterOpen] = useState(false);

  if (isAuthenticated) return <Navigate to="/tasks" replace />;

  return (
    <main className="mx-auto flex max-w-3xl flex-col items-center px-4 py-16 text-center">
      <h1 className="text-4xl font-bold tracking-tight text-pale-sky-900 sm:text-5xl">
        Task Manager
      </h1>
      <p className="mt-4 max-w-xl text-base text-pale-sky-700 sm:text-lg">
        A simple application to track your tasks.
      </p>

      <div className="mt-8">
        <Button onClick={() => setRegisterOpen(true)}>Create account</Button>
      </div>

      <p className="mt-3 text-sm text-pale-sky-600">
        Already have an account? Use the{" "}
        <span className="font-medium">Login</span> button at the top right.
      </p>

      <RegisterModal
        open={registerOpen}
        onClose={() => setRegisterOpen(false)}
      />
    </main>
  );
}
