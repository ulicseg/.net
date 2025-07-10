import { AlertCircle, Calendar, CheckCircle, Clock, LogOut, Plus, XCircle } from 'lucide-react';
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { reservaService } from '../../services/reservaService';
import type { Reserva } from '../../types';

export default function Dashboard() {
  const { user, logout } = useAuth();
  const [reservas, setReservas] = useState<Reserva[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadReservas();
  }, []);

  const loadReservas = async () => {
    try {
      setIsLoading(true);
      const response = await reservaService.getReservas({ page: 1, pageSize: 100 });
      setReservas(response.reservas);
    } catch (err) {
      setError('Error al cargar las reservas');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
  };

  // Calcular estadísticas
  const totalReservas = reservas.length;
  const activas = reservas.filter(r => r.estado === 1).length; // EstadoReserva.Activa
  const completadas = reservas.filter(r => r.estado === 2).length; // EstadoReserva.Completada  
  const canceladas = reservas.filter(r => r.estado === 3).length; // EstadoReserva.Cancelada

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header con navegación simple */}
      <div className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <h1 className="text-xl font-bold text-blue-600">Sistema de Reservas</h1>
            <div className="flex items-center space-x-4">
              <span className="text-gray-700">Hola, {user?.nombre}</span>
              <button 
                onClick={handleLogout}
                className="flex items-center text-red-600 hover:text-red-800 transition-colors"
              >
                <LogOut className="h-4 w-4 mr-1" />
                Cerrar Sesión
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Contenido principal */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        
        {/* Título de bienvenida */}
        <div className="mb-8">
          <h2 className="text-3xl font-bold text-gray-900">
            ¡Bienvenido, {user?.nombre}! 👋
          </h2>
          <p className="text-gray-600 mt-2">
            Administra tus reservas de manera sencilla y eficiente
          </p>
        </div>

        {/* Tarjetas de estadísticas */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <Calendar className="h-10 w-10 text-blue-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">{isLoading ? '...' : totalReservas}</p>
                <p className="text-sm text-gray-500">Total Reservas</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <Clock className="h-10 w-10 text-yellow-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">{isLoading ? '...' : activas}</p>
                <p className="text-sm text-gray-500">Activas</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <CheckCircle className="h-10 w-10 text-green-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">{isLoading ? '...' : completadas}</p>
                <p className="text-sm text-gray-500">Completadas</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <XCircle className="h-10 w-10 text-red-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">{isLoading ? '...' : canceladas}</p>
                <p className="text-sm text-gray-500">Canceladas</p>
              </div>
            </div>
          </div>
        </div>

        {/* Sección principal */}
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200">
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-medium text-gray-900">Mis Reservas</h3>
              <Link
                to="/reservas/nueva"
                className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
              >
                <Plus className="h-4 w-4 mr-2" />
                Nueva Reserva
              </Link>
            </div>
          </div>

          {/* Contenido de reservas */}
          <div className="p-6">
            {/* Error Alert */}
            {error && (
              <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg flex items-start gap-3">
                <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
                <p className="text-red-600">{error}</p>
              </div>
            )}

            {/* Loading */}
            {isLoading ? (
              <div className="text-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
                <p className="text-gray-500">Cargando reservas...</p>
              </div>
            ) : reservas.length === 0 ? (
              /* Sin reservas */
              <div className="text-center py-12">
                <Calendar className="h-20 w-20 text-gray-300 mx-auto mb-4" />
                <h4 className="text-xl font-medium text-gray-900 mb-2">
                  No tienes reservas aún
                </h4>
                <p className="text-gray-500 mb-6">
                  Comienza creando tu primera reserva
                </p>
                <Link
                  to="/reservas/nueva"
                  className="inline-flex items-center px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
                >
                  <Plus className="h-5 w-5 mr-2" />
                  Crear Primera Reserva
                </Link>
              </div>
            ) : (
              /* Lista de reservas */
              <div className="space-y-4">
                {reservas.map((reserva) => (
                  <div key={reserva.id} className="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition-colors">
                    <div className="flex justify-between items-start">
                      <div className="flex-1">
                        <h5 className="text-lg font-medium text-gray-900 mb-1">
                          {reserva.titulo}
                        </h5>
                        <p className="text-sm text-gray-600 mb-2">
                          {reserva.tipoServicioTexto}
                        </p>
                        <div className="flex items-center text-sm text-gray-500 space-x-4">
                          <span className="flex items-center">
                            <Calendar className="h-4 w-4 mr-1" />
                            {new Date(reserva.fechaReserva).toLocaleDateString('es-AR')}
                          </span>
                          <span className="flex items-center">
                            <Clock className="h-4 w-4 mr-1" />
                            {new Date(reserva.fechaReserva).toLocaleTimeString('es-AR', { 
                              hour: '2-digit', 
                              minute: '2-digit' 
                            })}
                          </span>
                        </div>
                        {reserva.descripcion && (
                          <p className="text-sm text-gray-600 mt-2">
                            {reserva.descripcion}
                          </p>
                        )}
                      </div>
                      <div className="ml-4 flex flex-col items-end space-y-2">
                        <span className={`px-2 py-1 text-xs rounded-full ${
                          reserva.estado === 1 ? 'bg-yellow-100 text-yellow-800' :
                          reserva.estado === 2 ? 'bg-green-100 text-green-800' :
                          'bg-red-100 text-red-800'
                        }`}>
                          {reserva.estadoTexto}
                        </span>
                        <Link
                          to={`/reservas/${reserva.id}`}
                          className="text-blue-600 hover:text-blue-800 text-sm"
                        >
                          Ver detalles
                        </Link>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Enlaces rápidos */}
        <div className="mt-8 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="bg-white rounded-lg shadow p-6">
            <h4 className="text-lg font-medium text-gray-900 mb-4">Acciones Rápidas</h4>
            <div className="space-y-3">
              <Link
                to="/reservas/nueva"
                className="block w-full text-left px-4 py-2 bg-gray-50 rounded hover:bg-gray-100 transition-colors"
              >
                📅 Crear Nueva Reserva
              </Link>
              <Link
                to="/dashboard"
                className="block w-full text-left px-4 py-2 bg-gray-50 rounded hover:bg-gray-100 transition-colors"
              >
                📊 Ver Estadísticas
              </Link>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <h4 className="text-lg font-medium text-gray-900 mb-4">Información</h4>
            <div className="space-y-2 text-sm text-gray-600">
              <p>• Gestiona todas tus reservas desde aquí</p>
              <p>• Recibe notificaciones automáticas</p>
              <p>• Genera códigos QR para tus reservas</p>
              <p>• Consulta el historial completo</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
