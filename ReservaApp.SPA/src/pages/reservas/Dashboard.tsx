import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Calendar, Clock, MapPin } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { Layout } from '../../components/layout/Layout';

export default function Dashboard() {
  const { user } = useAuth();
  // const [isLoading, setIsLoading] = useState(false);
  const [reservas] = useState([]); // Empezamos con array vacío

  console.log('Dashboard render:', { user: user?.nombre, reservasCount: reservas.length });

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            ¡Hola, {user?.nombre || 'Usuario'}! 👋
          </h1>
          <p className="text-gray-600 mt-2">
            Gestiona tus reservas de forma fácil y rápida
          </p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <Calendar className="h-8 w-8 text-blue-600" />
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Total Reservas</p>
                <p className="text-2xl font-bold text-gray-900">{reservas.length}</p>
              </div>
            </div>
          </div>
          
          <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <Clock className="h-8 w-8 text-green-600" />
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Confirmadas</p>
                <p className="text-2xl font-bold text-gray-900">0</p>
              </div>
            </div>
          </div>
          
          <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <MapPin className="h-8 w-8 text-orange-600" />
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Pendientes</p>
                <p className="text-2xl font-bold text-gray-900">0</p>
              </div>
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 mb-6">
          <div className="flex justify-between items-center">
            <h2 className="text-lg font-semibold text-gray-900">Tus Reservas</h2>
            <Link
              to="/reservas/nueva"
              className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors"
            >
              <Plus className="h-5 w-5 mr-2" />
              Nueva Reserva
            </Link>
          </div>
        </div>

        {/* Content */}
        {false ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : (
          <div className="text-center py-12">
            <Calendar className="h-16 w-16 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">
              No tienes reservas aún
            </h3>
            <p className="text-gray-600 mb-4">
              Crea tu primera reserva para comenzar
            </p>
            <Link
              to="/reservas/nueva"
              className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
            >
              <Plus className="h-5 w-5 mr-2" />
              Crear Primera Reserva
            </Link>
          </div>
        )}
      </div>
    </Layout>
  );
}
