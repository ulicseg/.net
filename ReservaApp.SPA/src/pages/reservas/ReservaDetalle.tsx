import {
  AlertCircle,
  ArrowLeft,
  Calendar,
  Check,
  Clock,
  Edit,
  FileText,
  QrCode,
  Trash2,
  User
} from 'lucide-react';
import QRCodeLib from 'qrcode';
import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Layout } from '../../components/layout/Layout';
import { qrService } from '../../services/qrService';
import { reservaService } from '../../services/reservaService';
import type { Reserva } from '../../types';

export default function ReservaDetalle() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [reserva, setReserva] = useState<Reserva | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [isGeneratingQR, setIsGeneratingQR] = useState(false);
  const [qrUrl, setQrUrl] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      loadReserva();
    }
  }, [id]);

  const loadReserva = async () => {
    if (!id) return;
    
    try {
      setIsLoading(true);
      const response = await reservaService.getReservaById(parseInt(id));
      setReserva(response);
    } catch (err) {
      setError('Error al cargar la reserva');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!reserva || !window.confirm('¿Estás seguro de que quieres eliminar esta reserva?')) {
      return;
    }

    try {
      await reservaService.deleteReserva(reserva.id);
      navigate('/dashboard');
    } catch (err) {
      console.error('Error al eliminar reserva:', err);
      alert('Error al eliminar la reserva');
    }
  };

  const handleGenerateQR = async () => {
    if (!reserva) return;

    try {
      setIsGeneratingQR(true);
      // Primero obtener la URL del QR desde el backend
      const response = await qrService.generateQR(reserva.id);
      
      // Luego generar la imagen QR usando la URL obtenida
      const qrDataURL = await QRCodeLib.toDataURL(response.qrUrl, {
        width: 256,
        margin: 2,
        color: {
          dark: '#000000',
          light: '#FFFFFF'
        }
      });
      
      setQrUrl(qrDataURL);
    } catch (err) {
      console.error('Error al generar QR:', err);
      alert('Error al generar código QR');
    } finally {
      setIsGeneratingQR(false);
    }
  };

  const getEstadoColor = (estado: string) => {
    switch (estado.toLowerCase()) {
      case 'confirmada':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'pendiente':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'cancelada':
        return 'bg-red-100 text-red-800 border-red-200';
      case 'completada':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getEstadoIcon = (estado: string) => {
    switch (estado.toLowerCase()) {
      case 'confirmada':
      case 'completada':
        return <Check className="h-4 w-4" />;
      default:
        return <AlertCircle className="h-4 w-4" />;
    }
  };

  if (isLoading) {
    return (
      <Layout>
        <div className="flex justify-center items-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  if (error || !reserva) {
    return (
      <Layout>
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="text-center py-12">
            <div className="bg-red-50 border border-red-200 rounded-lg p-6 max-w-md mx-auto">
              <AlertCircle className="h-12 w-12 text-red-500 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-red-900 mb-2">Error</h3>
              <p className="text-red-600 mb-4">{error || 'Reserva no encontrada'}</p>
              <button
                onClick={() => navigate('/dashboard')}
                className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
              >
                Volver al Dashboard
              </button>
            </div>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center gap-4 mb-4">
            <button
              onClick={() => navigate('/dashboard')}
              className="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
            >
              <ArrowLeft className="h-5 w-5" />
            </button>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">{reserva.titulo}</h1>
              <p className="text-gray-600">Detalles de la reserva #{reserva.id}</p>
            </div>
          </div>

          {/* Actions */}
          <div className="flex flex-wrap gap-3">
            <Link
              to={`/reservas/${reserva.id}/editar`}
              className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <Edit className="h-4 w-4 mr-2" />
              Editar
            </Link>
            
            <button
              onClick={handleGenerateQR}
              disabled={isGeneratingQR}
              className="inline-flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 transition-colors"
            >
              <QrCode className="h-4 w-4 mr-2" />
              {isGeneratingQR ? 'Generando...' : 'Generar QR'}
            </button>
            
            <button
              onClick={handleDelete}
              className="inline-flex items-center px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
            >
              <Trash2 className="h-4 w-4 mr-2" />
              Eliminar
            </button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2 space-y-6">
            {/* Status Card */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Estado de la Reserva</h2>
              <div className="flex items-center gap-3">
                <span className={`inline-flex items-center gap-2 px-3 py-2 rounded-full text-sm font-medium border ${getEstadoColor(reserva.estadoTexto || String(reserva.estado))}`}>
                  {getEstadoIcon(reserva.estadoTexto || String(reserva.estado))}
                  {reserva.estadoTexto || reserva.estado}
                </span>
              </div>
            </div>

            {/* Details Card */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-6">Detalles</h2>
              
              <div className="space-y-4">
                <div className="flex items-start gap-3">
                  <Calendar className="h-5 w-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Fecha y Hora</p>
                    <p className="text-gray-900">
                      {new Date(reserva.fechaReserva).toLocaleDateString('es-ES', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                      })}
                    </p>
                    <p className="text-gray-600">
                      {new Date(reserva.fechaReserva).toLocaleTimeString('es-ES', {
                        hour: '2-digit',
                        minute: '2-digit'
                      })}
                    </p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <FileText className="h-5 w-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Tipo de Servicio</p>
                    <p className="text-gray-900">{reserva.tipoServicioTexto || reserva.tipoServicio}</p>
                  </div>
                </div>

                {reserva.descripcion && (
                  <div className="flex items-start gap-3">
                    <FileText className="h-5 w-5 text-gray-400 mt-0.5" />
                    <div>
                      <p className="text-sm font-medium text-gray-700">Descripción</p>
                      <p className="text-gray-900 whitespace-pre-wrap">{reserva.descripcion}</p>
                    </div>
                  </div>
                )}

                <div className="flex items-start gap-3">
                  <Clock className="h-5 w-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Fecha de Creación</p>
                    <p className="text-gray-900">
                      {new Date(reserva.fechaCreacion).toLocaleDateString('es-ES')}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            {/* QR Code Card */}
            {qrUrl && (
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Código QR</h2>
                <div className="flex flex-col items-center gap-4">
                  <img src={qrUrl} alt="Código QR" className="w-48 h-48 border rounded-lg" />
                  <p className="text-sm text-gray-600 text-center">
                    Escanea este código QR para acceder rápidamente a los detalles de la reserva
                  </p>
                  <a
                    href={qrUrl}
                    download={`reserva-${reserva.id}-qr.png`}
                    className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                  >
                    Descargar QR
                  </a>
                </div>
              </div>
            )}
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            {/* User Info */}
            {reserva.usuario && (
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Usuario</h3>
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                    <User className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-900">{reserva.usuario.nombreCompleto}</p>
                    <p className="text-sm text-gray-600">{reserva.usuario.email}</p>
                  </div>
                </div>
              </div>
            )}

            {/* Quick Actions */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Acciones Rápidas</h3>
              <div className="space-y-3">
                <Link
                  to="/reservas/nueva"
                  className="block w-full px-4 py-2 text-center bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Nueva Reserva
                </Link>
                <Link
                  to="/dashboard"
                  className="block w-full px-4 py-2 text-center bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Ver Todas las Reservas
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
}
