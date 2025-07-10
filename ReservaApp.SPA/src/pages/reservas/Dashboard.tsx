import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { 
  Plus, 
  Calendar, 
  Clock, 
  Filter,
  Search,
  ChevronLeft,
  ChevronRight,
  Eye,
  Edit,
  Trash2,
  MapPin
} from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { reservaService } from '../../services/reservaService';
import { Layout } from '../../components/layout/Layout';
import type { Reserva, EstadoReserva } from '../../types';

export default function Dashboard() {
  const { user } = useAuth();
  const [reservas, setReservas] = useState<Reserva[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [filterEstado, setFilterEstado] = useState<EstadoReserva | 'all'>('all');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 6;

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

  const handleDelete = async (id: number) => {
    if (!window.confirm('¿Estás seguro de que quieres eliminar esta reserva?')) {
      return;
    }

    try {
      await reservaService.deleteReserva(id);
      setReservas(prev => prev.filter(r => r.id !== id));
    } catch (err) {
      console.error('Error al eliminar reserva:', err);
      alert('Error al eliminar la reserva');
    }
  };

  // Filtrar reservas
  const filteredReservas = reservas.filter(reserva => {
    const matchesSearch = reserva.titulo.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         (reserva.descripcion?.toLowerCase().includes(searchTerm.toLowerCase()) ?? false);
    const matchesEstado = filterEstado === 'all' || reserva.estado === filterEstado;
    return matchesSearch && matchesEstado;
  });

  // Paginación
  const totalPages = Math.ceil(filteredReservas.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const paginatedReservas = filteredReservas.slice(startIndex, startIndex + itemsPerPage);

  const getEstadoColor = (estado: EstadoReserva) => {
    switch (estado) {
      case 'Confirmada':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'Pendiente':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Cancelada':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getEstadoText = (estado: EstadoReserva) => {
    return estado;
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            ¡Hola, {user?.nombre}! 👋
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
                <p className="text-2xl font-bold text-gray-900">
                  {reservas.filter(r => r.estado === 'Confirmada').length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <MapPin className="h-8 w-8 text-orange-600" />
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Pendientes</p>
                <p className="text-2xl font-bold text-gray-900">
                  {reservas.filter(r => r.estado === 'Pendiente').length}
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Actions & Filters */}
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 mb-6">
          <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center">
            <div className="flex flex-col sm:flex-row gap-4 flex-1">
              {/* Search */}
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar reservas..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 w-full sm:w-64"
                />
              </div>

              {/* Filter by Estado */}
              <div className="relative">
                <Filter className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                <select
                  value={filterEstado}
                  onChange={(e) => setFilterEstado(e.target.value as EstadoReserva | 'all')}
                  className="pl-10 pr-8 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 appearance-none bg-white"
                >
                  <option value="all">Todos los estados</option>
                  <option value="Pendiente">Pendiente</option>
                  <option value="Confirmada">Confirmada</option>
                  <option value="Cancelada">Cancelada</option>
                </select>
              </div>
            </div>

            {/* New Reservation Button */}
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
        {isLoading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : error ? (
          <div className="text-center py-12">
            <div className="bg-red-50 border border-red-200 rounded-lg p-6 max-w-md mx-auto">
              <p className="text-red-600">{error}</p>
              <button
                onClick={loadReservas}
                className="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
              >
                Reintentar
              </button>
            </div>
          </div>
        ) : filteredReservas.length === 0 ? (
          <div className="text-center py-12">
            <Calendar className="h-16 w-16 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">
              {searchTerm || filterEstado !== 'all' ? 'No se encontraron reservas' : 'No tienes reservas aún'}
            </h3>
            <p className="text-gray-600 mb-4">
              {searchTerm || filterEstado !== 'all' 
                ? 'Intenta cambiar los filtros de búsqueda' 
                : 'Crea tu primera reserva para comenzar'
              }
            </p>
            {(!searchTerm && filterEstado === 'all') && (
              <Link
                to="/reservas/nueva"
                className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
              >
                <Plus className="h-5 w-5 mr-2" />
                Crear Primera Reserva
              </Link>
            )}
          </div>
        ) : (
          <>
            {/* Reservas Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
              {paginatedReservas.map((reserva) => (
                <div key={reserva.id} className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
                  <div className="p-6">
                    <div className="flex justify-between items-start mb-4">
                      <h3 className="text-lg font-semibold text-gray-900 truncate">
                        {reserva.titulo}
                      </h3>
                      <span className={`px-2 py-1 rounded-full text-xs font-medium border ${getEstadoColor(reserva.estado)}`}>
                        {getEstadoText(reserva.estado)}
                      </span>
                    </div>

                    <div className="space-y-2 mb-4">
                      <div className="flex items-center text-sm text-gray-600">
                        <Calendar className="h-4 w-4 mr-2" />
                        {new Date(reserva.fechaReserva).toLocaleDateString('es-ES')}
                      </div>
                      <div className="flex items-center text-sm text-gray-600">
                        <Clock className="h-4 w-4 mr-2" />
                        {new Date(reserva.fechaReserva).toLocaleTimeString('es-ES', { 
                          hour: '2-digit', 
                          minute: '2-digit' 
                        })}
                      </div>
                    </div>

                    <p className="text-gray-600 text-sm mb-4 line-clamp-2">
                      {reserva.descripcion}
                    </p>

                    <div className="flex justify-between items-center">
                      <div className="flex space-x-2">
                        <Link
                          to={`/reservas/${reserva.id}`}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Ver detalle"
                        >
                          <Eye className="h-4 w-4" />
                        </Link>
                        <Link
                          to={`/reservas/${reserva.id}/editar`}
                          className="p-2 text-green-600 hover:bg-green-50 rounded-lg transition-colors"
                          title="Editar"
                        >
                          <Edit className="h-4 w-4" />
                        </Link>
                        <button
                          onClick={() => handleDelete(reserva.id)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                          title="Eliminar"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      </div>
                      
                      <span className="text-xs text-gray-500">
                        ID: {reserva.id.toString().slice(0, 8)}...
                      </span>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="flex justify-center items-center space-x-4">
                <button
                  onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                  disabled={currentPage === 1}
                  className="flex items-center px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <ChevronLeft className="h-4 w-4 mr-1" />
                  Anterior
                </button>

                <span className="text-sm text-gray-700">
                  Página {currentPage} de {totalPages}
                </span>

                <button
                  onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                  disabled={currentPage === totalPages}
                  className="flex items-center px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Siguiente
                  <ChevronRight className="h-4 w-4 ml-1" />
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </Layout>
  );
}
