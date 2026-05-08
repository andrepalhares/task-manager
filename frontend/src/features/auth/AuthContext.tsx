import { jwtDecode } from "jwt-decode";
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { useNavigate } from "react-router-dom";
import {
  TOKEN_STORAGE_KEY,
  registerUnauthorizedHandler,
} from "../../shared/api/axiosClient";
import type { AuthUser, DecodedJwt } from "./types";

type AuthContextValue = {
  user: AuthUser | null;
  isAuthenticated: boolean;
  setToken: (token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function decodeUserFromToken(token: string): AuthUser | null {
  try {
    const claims = jwtDecode<DecodedJwt>(token);
    if (!claims.sub || !claims.email || !claims.name) return null;
    // Defensive: reject already-expired tokens at boot
    if (claims.exp * 1000 < Date.now()) return null;
    return { id: claims.sub, email: claims.email, name: claims.name };
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem(TOKEN_STORAGE_KEY);
    return stored ? decodeUserFromToken(stored) : null;
  });

  const setToken = useCallback((token: string) => {
    localStorage.setItem(TOKEN_STORAGE_KEY, token);
    setUser(decodeUserFromToken(token));
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    setUser(null);
    navigate("/", { replace: true });
  }, [navigate]);

  // Hand the logout function to the axios interceptor so it can fire on 401.
  useEffect(() => {
    registerUnauthorizedHandler(() => {
      localStorage.removeItem(TOKEN_STORAGE_KEY);
      setUser(null);
      navigate("/", { replace: true });
    });
  }, [navigate]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      setToken,
      logout,
    }),
    [user, setToken, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside <AuthProvider>");
  return ctx;
}
