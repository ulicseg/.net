import { apiClient } from './apiClient';
import type { 
  Reserva, 
  CreateReservaRequest, 
  UpdateReservaRequest,
  ReservaListResponse,
  ApiResponse,
  PaginationParams
} from '../types';

/**
 * Servicio para operaciones CRUD de reservas
 * ¿Por qué separar? Para organizar todas las operaciones de reservas y facilitar el mantenimiento
 */
export class ReservaService {

  /**
   * Obtener lista paginada de reservas del usuario
   */
  async getReservas(params: PaginationParams): Promise<ReservaListResponse> {
    try {
      const queryParams = new URLSearchParams({
        page: params.page.toString(),
        limit: params.pageSize.toString()
      });

      const response = await apiClient.get<ApiResponse<ReservaListResponse>>(
        `/reservas?${queryParams}`
      );

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error('No se pudieron obtener las reservas');
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al obtener reservas');
    }
  }

  /**
   * Obtener una reserva específica por ID
   */
  async getReservaById(id: number): Promise<Reserva> {
    try {
      const response = await apiClient.get<ApiResponse<Reserva>>(`/reservas/${id}`);

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error('Reserva no encontrada');
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al obtener la reserva');
    }
  }

  /**
   * Crear nueva reserva
   */
  async createReserva(reservaData: CreateReservaRequest): Promise<Reserva> {
    try {
      const response = await apiClient.post<ApiResponse<Reserva>>('/reservas', reservaData);

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error('No se pudo crear la reserva');
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al crear la reserva');
    }
  }

  /**
   * Actualizar reserva existente
   */
  async updateReserva(id: number, reservaData: UpdateReservaRequest): Promise<Reserva> {
    try {
      const response = await apiClient.put<ApiResponse<Reserva>>(`/reservas/${id}`, reservaData);

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error('No se pudo actualizar la reserva');
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al actualizar la reserva');
    }
  }

  /**
   * Eliminar reserva
   */
  async deleteReserva(id: number): Promise<void> {
    try {
      const response = await apiClient.delete<ApiResponse<any>>(`/reservas/${id}`);

      if (!response.success) {
        throw new Error('No se pudo eliminar la reserva');
      }
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Error al eliminar la reserva');
    }
  }
}

// Exportar instancia única
export const reservaService = new ReservaService();
