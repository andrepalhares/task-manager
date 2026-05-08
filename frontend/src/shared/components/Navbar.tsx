import { LogOut } from "lucide-react";
import { useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../features/auth/AuthContext";
import { LoginModal } from "../../features/auth/components/LoginModal";
import { Button } from "./Button";

export function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const [loginOpen, setLoginOpen] = useState(false);

  const firstName = user ? user.name.split(" ")[0] : "";

  return (
    <>
      <header className="sticky top-0 z-30 border-b border-pale-sky-100 bg-white/90 backdrop-blur">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
          <Link
            to={isAuthenticated ? "/tasks" : "/"}
            className="text-lg font-semibold text-pale-sky-800 hover:text-pale-sky-900"
          >
            Tasks
          </Link>

          <div className="flex items-center gap-3">
            {isAuthenticated ? (
              <>
                <span className="text-sm text-pale-sky-700">
                  Hi, <span className="font-medium">{firstName}</span>
                </span>
                <Button variant="secondary" onClick={logout}>
                  <LogOut className="h-4 w-4" />
                  Logout
                </Button>
              </>
            ) : (
              <Button variant="primary" onClick={() => setLoginOpen(true)}>
                Login
              </Button>
            )}
          </div>
        </div>
      </header>

      <LoginModal open={loginOpen} onClose={() => setLoginOpen(false)} />
    </>
  );
}
