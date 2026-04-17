import { createContext,useContext,
  useState,
  useCallback,
  type ReactNode,
} from 'react';
import * as authApi from '../api/authApi';
import { setAuthToken } from '../api/client';
import type { LoginRequest } from '../types';

interface AuthContextValue {
  token: string | null;
  username: string | null;
  isAuthenticated: boolean;
  login: (req: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [username, setUsername] = useState<string | null>(null);

  const login = useCallback(async (req: LoginRequest) => {
    const response = await authApi.login(req);
    setToken(response.accessToken);
    setUsername(req.username);
    setAuthToken(response.accessToken);
  }, []);

  const logout = useCallback(() => {
    setToken(null);
    setUsername(null);
    setAuthToken(null);
  }, []);

  return (
    <AuthContext.Provider
      value={{ token, username, isAuthenticated: token !== null, login, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
