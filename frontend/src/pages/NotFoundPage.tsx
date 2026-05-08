import { Link } from "react-router-dom";
import { Button } from "../shared/components/Button";

export function NotFoundPage() {
  return (
    <main className="mx-auto flex max-w-2xl flex-col items-center px-4 py-20 text-center">
      <h1 className="text-6xl font-bold text-pale-sky-900">404</h1>
      <p className="mt-4 text-lg text-pale-sky-700">Page not found</p>
      <div className="mt-6">
        <Link to="/">
          <Button>Back to home</Button>
        </Link>
      </div>
    </main>
  );
}
