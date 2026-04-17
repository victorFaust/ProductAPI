import { createContext, useContext, useState, useCallback, useEffect, useRef, type ReactNode } from 'react';
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
  const refreshTokenRef = useRef<string | null>(null);
  const refreshTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const scheduleRefresh = useCallback((expiresAt: string) => {
    if (refreshTimerRef.current) clearTimeout(refreshTimerRef.current);
    const msUntilExpiry = new Date(expiresAt).getTime() - Date.now();
    // Refresh 60 seconds before expiry, minimum 5 seconds
    const delay = Math.max(msUntilExpiry - 60_000, 5_000);
    refreshTimerRef.current = setTimeout(async () => {
      if (!refreshTokenRef.current) return;
      try {
        const response = await authApi.refresh(refreshTokenRef.current);
        refreshTokenRef.current = response.refreshToken;
        setToken(response.accessToken);
        setAuthToken(response.accessToken);
        scheduleRefresh(response.accessTokenExpiresAt);
      } catch {
        // Refresh failed — force logout
        setToken(null);
        setUsername(null);
        refreshTokenRef.current = null;
        setAuthToken(null);
      }
    }, delay);
  }, []);

  const login = useCallback(async (req: LoginRequest) => {
    const response = await authApi.login(req);
    refreshTokenRef.current = response.refreshToken;
    setToken(response.accessToken);
    setUsername(req.username);
    setAuthToken(response.accessToken);
    scheduleRefresh(response.accessTokenExpiresAt);
  }, [scheduleRefresh]);

  const logout = useCallback(() => {
    if (refreshTimerRef.current) clearTimeout(refreshTimerRef.current);
    if (refreshTokenRef.current) {
      authApi.revoke(refreshTokenRef.current).catch(() => {});
    }
    refreshTokenRef.current = null;
    setToken(null);
    setUsername(null);
    setAuthToken(null);
  }, []);

  // Cleanup timer on unmount
  useEffect(() => () => { if (refreshTimerRef.current) clearTimeout(refreshTimerRef.current); }, []);

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

