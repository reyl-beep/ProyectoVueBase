export interface Resultado<T = unknown> {
  value: boolean;
  message: string;
  data: T | null;
}

export interface TokenResponse {
  token: string;
  expiration: string;
}

export interface UsuarioAutenticado {
  usuarioId: number;
  nombre: string;
  apellidos?: string | null;
  correo: string;
  rol: string;
  esAdmin: boolean;
}

export interface CancionResumen {
  cancionId: number;
  titulo: string;
  descripcion?: string | null;
  totalReproducciones: number;
  montoGanado: number;
  fechaPublicacion: string;
  activo: boolean;
}

export interface DashboardUsuario {
  usuario: UsuarioAutenticado;
  canciones: CancionResumen[];
}

export interface DashboardGlobal {
  usuarios: DashboardUsuario[];
}

export interface LoginRequest {
  correo: string;
  password: string;
}

export interface RegisterRequest {
  nombre: string;
  apellidos?: string | null;
  correo: string;
  password: string;
}
