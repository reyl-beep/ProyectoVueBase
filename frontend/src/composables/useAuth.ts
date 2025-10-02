import { computed, reactive, ref } from 'vue';
import { api } from '../services/api';
import type { DashboardUsuario, TokenResponse, UsuarioAutenticado } from '../types';

interface AuthState {
  token: string | null;
  user: UsuarioAutenticado | null;
  expiration: string | null;
}

const state = reactive<AuthState>({
  token: null,
  user: null,
  expiration: null,
});

const initialized = ref(false);

export function useAuth() {
  const isAuthenticated = computed(() => Boolean(state.token));
  const isAdmin = computed(() => state.user?.esAdmin ?? false);

  const persist = () => {
    if (state.token) {
      localStorage.setItem('rr_token', state.token);
      localStorage.setItem('rr_expiration', state.expiration ?? '');
    } else {
      localStorage.removeItem('rr_token');
      localStorage.removeItem('rr_expiration');
    }
  };

  const setSession = (token: TokenResponse, user?: UsuarioAutenticado | null) => {
    state.token = token.token;
    state.expiration = token.expiration;
    if (user) {
      state.user = user;
    }
    api.setToken(state.token);
    persist();
  };

  const clearSession = () => {
    state.token = null;
    state.user = null;
    state.expiration = null;
    api.setToken(null);
    persist();
  };

  const initializeFromStorage = async () => {
    if (initialized.value) return;
    const storedToken = localStorage.getItem('rr_token');
    if (storedToken) {
      state.token = storedToken;
      state.expiration = localStorage.getItem('rr_expiration');
      api.setToken(storedToken);
      await refreshUser();
    }
    initialized.value = true;
  };

  const refreshUser = async () => {
    if (!state.token) {
      state.user = null;
      return null;
    }

    const result = await api.getDashboard();
    if (result.value) {
      const dashboard = result.data as DashboardUsuario;
      state.user = dashboard.usuario;
      return dashboard;
    }

    clearSession();
    return null;
  };

  const logout = () => {
    clearSession();
  };

  return {
    state,
    initialized,
    isAuthenticated,
    isAdmin,
    setSession,
    clearSession,
    initializeFromStorage,
    refreshUser,
    logout,
  };
}

export function useAuthToken() {
  return state.token;
}
