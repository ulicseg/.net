import { apiClient } from './apiClient';
import type { QRAccessResponse, QRGenerateResponse } from '../types';

/**
 * Servicio para operaciones con códigos QR
 * ¿Por qué separar? Los QR tienen lógica específica diferente a las reservas normales
 */
export class QRService {

  /**
   * Acceder a información de reserva via código QR
   * ¿Por qué público? Este endpoint no requiere autenticación para permitir acceso directo
   */
  async accessViaQR(hash: string): Promise<QRAccessResponse> {
    try {
      // Nota: Este endpoint no requiere autenticación, así que usamos fetch directo
      const baseURL = import.meta.env.VITE_API_URL || 'http://localhost:5284/api';
      const response = await fetch(`${baseURL}/qr/access/${hash}`);
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'QR inválido o expirado');
      }

      const data = await response.json();
      return data;
    } catch (error: any) {
      throw new Error(error.message || 'Error al acceder via QR');
    }
  }

  /**
   * Generar código QR para una reserva
   * ¿Por qué requiere auth? Solo el dueño de la reserva puede generar QRs
   */
  async generateQR(reservaId: number): Promise<QRGenerateResponse> {
    try {
      const response = await apiClient.post<QRGenerateResponse>(`/qr/generate/${reservaId}`);
      return response;
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al generar código QR');
    }
  }

  /**
   * Verificar si un hash QR es válido (sin consumirlo)
   */
  async validateQRHash(hash: string): Promise<boolean> {
    try {
      await this.accessViaQR(hash);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Extraer hash de una URL QR
   */
  extractHashFromURL(url: string): string | null {
    try {
      const urlObj = new URL(url);
      const pathSegments = urlObj.pathname.split('/');
      
      // Buscar tanto 'access' como 'view' para compatibilidad
      let hashIndex = pathSegments.findIndex(segment => segment === 'view');
      if (hashIndex === -1) {
        hashIndex = pathSegments.findIndex(segment => segment === 'access');
      }
      
      if (hashIndex !== -1 && hashIndex < pathSegments.length - 1) {
        return pathSegments[hashIndex + 1];
      }
      
      return null;
    } catch {
      return null;
    }
  }
}

// Exportar instancia única
export const qrService = new QRService();
