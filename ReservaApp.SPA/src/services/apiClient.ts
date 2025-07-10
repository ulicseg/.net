import axios from 'axios';
import type { AxiosInstance, AxiosResponse } from 'axios';
import type { User } from '../types';

/**
 * Configuración base de la API
 * ¿Por qué centralizar? Para manejar tokens, interceptors y configuración base en un solo lugar
 */
class ApiClient {
  private client: AxiosInstance;
  private baseURL: string;

  constructor() {
    this.baseURL = import.meta.env.VITE_API_URL || 'http://localhost:5284/api';
    
    this.client = axios.create({
      baseURL: this.baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  /**
   * Configurar interceptors para manejo automático de tokens y errores
   * ¿Por qué interceptors? Para agregar automáticamente el token JWT y manejar errores globalmente
   */
  private setupInterceptors() {
    // Request interceptor - agregar token JWT automáticamente
    this.client.interceptors.request.use(
      (config) => {
        const token = this.getToken();
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor - manejar errores globalmente
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Token expirado o inválido
          this.removeToken();
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  /**
   * Métodos para manejo de tokens
   * ¿Por qué localStorage? Simple y efectivo para SPAs, aunque podríamos usar cookies seguras en producción
   */
  private getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  public setToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  public removeToken(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
  }

  /**
   * Métodos HTTP base
   */
  public async get<T>(url: string): Promise<T> {
    const response: AxiosResponse<T> = await this.client.get(url);
    return response.data;
  }

  public async post<T>(url: string, data?: any): Promise<T> {
    const response: AxiosResponse<T> = await this.client.post(url, data);
    return response.data;
  }

  public async put<T>(url: string, data?: any): Promise<T> {
    const response: AxiosResponse<T> = await this.client.put(url, data);
    return response.data;
  }

  public async delete<T>(url: string): Promise<T> {
    const response: AxiosResponse<T> = await this.client.delete(url);
    return response.data;
  }

  /**
   * Verificar si el usuario está autenticado
   */
  public isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      // Verificar si el token no ha expirado
      const payload = JSON.parse(atob(token.split('.')[1]));
      const now = Date.now() / 1000;
      return payload.exp > now;
    } catch {
      return false;
    }
  }

  /**
   * Obtener usuario actual del localStorage
   */
  public getCurrentUser(): User | null {
    const userStr = localStorage.getItem('currentUser');
    if (!userStr) return null;
    
    try {
      return JSON.parse(userStr);
    } catch {
      return null;
    }
  }

  /**
   * Guardar usuario actual en localStorage
   */
  public setCurrentUser(user: User): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
  }
}

// Exportar instancia única (singleton)
export const apiClient = new ApiClient();
