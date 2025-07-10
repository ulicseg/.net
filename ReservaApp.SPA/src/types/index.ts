// Tipos base para la API
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

// Auth types
export interface User {
  id: string;
  email: string;
  nombre: string;
  apellido: string;
  nombreCompleto: string;
  fechaRegistro: string;
  roles: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterRequest {
  nombre: string;
  apellido: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  success: boolean;
  token?: string;
  expiration?: string;
  user?: User;
  message: string;
  errors?: string[];
}

// Reserva types
export interface Reserva {
  id: number;
  titulo: string;
  descripcion?: string;
  fechaReserva: string;
  fechaCreacion: string;
  tipoServicio: TipoServicio;
  tipoServicioTexto: string;
  estado: EstadoReserva;
  estadoTexto: string;
  usuarioId: string;
  usuario?: User;
}

export interface CreateReservaRequest {
  titulo: string;
  descripcion?: string;
  fechaReserva: string;
  tipoServicio: TipoServicio;
}

export interface UpdateReservaRequest {
  titulo: string;
  descripcion?: string;
  fechaReserva: string;
  tipoServicio: TipoServicio;
}

export interface ReservaListResponse {
  reservas: Reserva[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Enums matching backend (usar números como en el backend)
export enum TipoServicio {
  ConsultaMedica = 1,
  TerapiaFisica = 2,
  ConsultaNutricional = 3,
  ExamenLaboratorio = 4,
  ConsultaPsicologica = 5,
  CirugiaMenor = 6
}

export enum EstadoReserva {
  Activa = 1,
  Completada = 2,
  Cancelada = 3
}

// API DTO types (to match backend DTOs)
export interface ReservaListDto {
  id: number;
  titulo: string;
  fechaReserva: string;
  fechaCreacion: string;
  tipoServicio: string;
  estado: string;
  usuarioNombre: string;
}

export interface PagedResultDto<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Helper objects for display and validation
export const TipoServicioOptions = {
  [TipoServicio.ConsultaMedica]: 'Consulta Médica',
  [TipoServicio.TerapiaFisica]: 'Terapia Física',
  [TipoServicio.ConsultaNutricional]: 'Consulta Nutricional',
  [TipoServicio.ExamenLaboratorio]: 'Examen de Laboratorio',
  [TipoServicio.ConsultaPsicologica]: 'Consulta Psicológica',
  [TipoServicio.CirugiaMenor]: 'Cirugía Menor'
} as const;

export const EstadoReservaOptions = {
  [EstadoReserva.Activa]: 'Activa',
  [EstadoReserva.Completada]: 'Completada',
  [EstadoReserva.Cancelada]: 'Cancelada'
} as const;

// QR types
export interface QRAccessResponse {
  reservaId: number;
  tipoServicio: string;
  fechaReserva: string;
  estado: string;
  descripcion?: string;
  clienteNombre: string;
  fechaAcceso: string;
  mensajeAcceso: string;
}

export interface QRGenerateResponse {
  hash: string;
  qrUrl: string;
  fechaExpiracion: string;
  reservaId: number;
  minutosValidez: number;
}

// UI State types
export interface LoadingState {
  isLoading: boolean;
  error?: string;
}

export interface PaginationParams {
  page: number;
  pageSize: number;
}

// Form validation types
export interface FormErrors {
  [key: string]: string;
}

export interface ValidationResult {
  isValid: boolean;
  errors: FormErrors;
}
