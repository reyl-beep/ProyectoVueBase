import type { DashboardGlobal, DashboardUsuario, LoginRequest, RegisterRequest, Resultado, TokenResponse } from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5180';

let authToken: string | null = null;

const buildHeaders = (hasBody: boolean) => {
  const headers: Record<string, string> = {
    Accept: 'application/json',
  };

  if (hasBody) {
    headers['Content-Type'] = 'application/json';
  }

  if (authToken) {
    headers.Authorization = `Bearer ${authToken}`;
  }

  return headers;
};

const request = async <T = unknown>(path: string, options: RequestInit = {}): Promise<Resultado<T>> => {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      ...buildHeaders(Boolean(options.body)),
      ...options.headers,
    },
  });

  if (!response.ok) {
    return {
      value: false,
      message: `Error de red: ${response.statusText}`,
      data: null,
    } as Resultado<T>;
  }

  const result = (await response.json()) as Resultado<T>;
  return result;
};

export const api = {
  setToken(token: string | null) {
    authToken = token;
  },
  async register(payload: RegisterRequest) {
    return await request<TokenResponse>('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify(payload),
    });
  },
  async login(payload: LoginRequest) {
    return await request<TokenResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(payload),
    });
  },
  async getDashboard() {
    return await request<DashboardUsuario>('/api/dashboard/me');
  },
  async getAdminDashboard() {
    return await request<DashboardGlobal>('/api/dashboard/admin');
  },
};
