import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { identityApi } from '../api/client';
import type { UserProfile, LoginRequest, RegisterRequest } from '../types';

interface AuthContextType {
  user: UserProfile | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

function parseJwt(token: string): UserProfile {
  const payload = JSON.parse(atob(token.split('.')[1]));
  return {
    id: payload.sub || payload.nameid || '',
    email: payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '',
    name: payload.name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || '',
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [user, setUser] = useState<UserProfile | null>(() => {
    const saved = localStorage.getItem('token');
    if (saved) {
      try { return parseJwt(saved); } catch { return null; }
    }
    return null;
  });

  useEffect(() => {
    if (token) {
      localStorage.setItem('token', token);
      try { setUser(parseJwt(token)); } catch { setUser(null); }
    } else {
      localStorage.removeItem('token');
      setUser(null);
    }
  }, [token]);

  const login = async (data: LoginRequest) => {
    const res = await identityApi.post('api/auth/login', data);
    setToken(res.data.token);
  };

  const register = async (data: RegisterRequest) => {
    const res = await identityApi.post('api/auth/register', data);
    setToken(res.data.token);
  };

  const logout = () => {
    setToken(null);
  };

  return (
    <AuthContext.Provider value={{ user, token, isAuthenticated: !!token, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
