import { apiClient } from './apiClient';
import { TipoServicioValues } from '../types';
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

      // La API devuelve ApiResponseDto<PagedResultDto<ReservaListDto>>
      const response = await apiClient.get<ApiResponse<{
        data: Reserva[];
        totalCount: number;
        pageNumber: number;
        pageSize: number;
        totalPages: number;
      }>>(
        `/reservas?${queryParams}`
      );

      if (response.success && response.data) {
        // Mapear la estructura de la API a la estructura esperada por el frontend
        return {
          reservas: response.data.data,
          totalCount: response.data.totalCount,
          currentPage: response.data.pageNumber,
          pageSize: response.data.pageSize,
          totalPages: response.data.totalPages
        };
      }

      throw new Error('No se pudieron obtener las reservas');
    } catch (error: any) {
      console.error('Error en getReservas:', error);
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
      console.log('Enviando datos para crear reserva:', reservaData);
      
      // Convertir el tipoServicio string a número que espera la API
      const requestData = {
        ...reservaData,
        tipoServicio: TipoServicioValues[reservaData.tipoServicio]
      };
      
      console.log('Datos convertidos para API:', requestData);
      const response = await apiClient.post<ApiResponse<Reserva>>('/reservas', requestData);
      console.log('Respuesta de crear reserva:', response);

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error(response.message || 'No se pudo crear la reserva');
    } catch (error: any) {
      console.error('Error al crear reserva:', error);
      console.error('Respuesta del error:', error.response?.data);
      throw new Error(error.response?.data?.message || error.message || 'Error al crear la reserva');
    }
  }

  /**
   * Actualizar reserva existente
   */
  async updateReserva(id: number, reservaData: UpdateReservaRequest): Promise<Reserva> {
    try {
      console.log('Enviando datos para actualizar reserva:', reservaData);
      
      // Convertir el tipoServicio string a número que espera la API
      const requestData = {
        ...reservaData,
        tipoServicio: TipoServicioValues[reservaData.tipoServicio]
      };
      
      console.log('Datos convertidos para API:', requestData);
      const response = await apiClient.put<ApiResponse<Reserva>>(`/reservas/${id}`, requestData);

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error('No se pudo actualizar la reserva');
    } catch (error: any) {
      console.error('Error al actualizar reserva:', error);
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
