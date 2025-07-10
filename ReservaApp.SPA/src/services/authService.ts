import { apiClient } from './apiClient';
import type { 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse, 
  User 
} from '../types';

/**
 * Servicio de autenticación
 * ¿Por qué separar? Para organizar las operaciones de auth y reutilizar en toda la app
 */
export class AuthService {
  
  /**
   * Iniciar sesión
   */
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/auth/login', credentials);
      
      if (response.success && response.token && response.user) {
        // Guardar token y usuario en localStorage
        apiClient.setToken(response.token);
        apiClient.setCurrentUser(response.user);
      }
      
      return response;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al iniciar sesión');
    }
  }

  /**
   * Registrar nuevo usuario
   */
  async register(userData: RegisterRequest): Promise<AuthResponse> {
    try {
      console.log('Enviando datos de registro:', userData);
      const response = await apiClient.post<AuthResponse>('/auth/register', userData);
      console.log('Respuesta del registro:', response);
      
      if (response.success && response.token && response.user) {
        // Auto-login después del registro
        apiClient.setToken(response.token);
        apiClient.setCurrentUser(response.user);
      }
      
      return response;
    } catch (error: any) {
      console.error('Error en registro detallado:', error);
      throw new Error(error.response?.data?.message || error.message || 'Error al registrar usuario');
    }
  }

  /**
   * Cerrar sesión
   */
  logout(): void {
    apiClient.removeToken();
    window.location.href = '/login';
  }

  /**
   * Obtener información del usuario actual
   */
  async getCurrentUser(): Promise<User> {
    try {
      const response = await apiClient.get<AuthResponse>('/auth/me');
      
      if (response.success && response.user) {
        apiClient.setCurrentUser(response.user);
        return response.user;
      }
      
      throw new Error('No se pudo obtener información del usuario');
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al obtener usuario actual');
    }
  }

  /**
   * Verificar si el usuario está autenticado
   */
  isAuthenticated(): boolean {
    return apiClient.isAuthenticated();
  }

  /**
   * Obtener usuario del localStorage
   */
  getStoredUser(): User | null {
    return apiClient.getCurrentUser();
  }

  /**
   * Solicitar recuperación de contraseña
   */
  async forgotPassword(data: { email: string }): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/auth/forgot-password', data);
      return response;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al solicitar recuperación de contraseña');
    }
  }

  /**
   * Restablecer contraseña con token
   */
  async resetPassword(data: { 
    email: string; 
    token: string; 
    newPassword: string; 
    confirmPassword: string 
  }): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/auth/reset-password', data);
      return response;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al restablecer la contraseña');
    }
  }
}

// Exportar instancia única
export const authService = new AuthService();
