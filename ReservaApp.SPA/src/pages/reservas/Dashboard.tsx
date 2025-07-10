import { Link } from 'react-router-dom';
import { Plus, Calendar, Clock, CheckCircle, XCircle, LogOut } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';

export default function Dashboard() {
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
  };

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
                <p className="text-2xl font-bold text-gray-900">0</p>
                <p className="text-sm text-gray-500">Total Reservas</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <Clock className="h-10 w-10 text-yellow-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">0</p>
                <p className="text-sm text-gray-500">Pendientes</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <CheckCircle className="h-10 w-10 text-green-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">0</p>
                <p className="text-sm text-gray-500">Confirmadas</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <XCircle className="h-10 w-10 text-red-500" />
              <div className="ml-4">
                <p className="text-2xl font-bold text-gray-900">0</p>
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
