import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { 
  Calendar, 
  User, 
  FileText, 
  CheckCircle,
  AlertCircle,
  QrCode,
  Smartphone
} from 'lucide-react';
import { qrService } from '../../services/qrService';
import type { QRAccessResponse } from '../../types';

export default function QRAccess() {
  const { hash } = useParams<{ hash: string }>();
  const [reservaData, setReservaData] = useState<QRAccessResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (hash) {
      accessQR();
    }
  }, [hash]);

  const accessQR = async () => {
    if (!hash) return;
    
    try {
      setIsLoading(true);
      const response = await qrService.accessViaQR(hash);
      setReservaData(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al acceder al código QR');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
        <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <h2 className="text-lg font-semibold text-gray-900 mb-2">Verificando código QR...</h2>
          <p className="text-gray-600">Por favor espera mientras validamos tu acceso</p>
        </div>
      </div>
    );
  }

  if (error || !reservaData) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-red-50 to-red-100 flex items-center justify-center p-4">
        <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full text-center">
          <AlertCircle className="h-16 w-16 text-red-500 mx-auto mb-4" />
          <h2 className="text-xl font-bold text-gray-900 mb-2">Acceso Denegado</h2>
          <p className="text-gray-600 mb-6">
            {error || 'El código QR es inválido, ha expirado o no existe.'}
          </p>
          <div className="space-y-3">
            <p className="text-sm text-gray-500">
              Los códigos QR expiran después de 10 minutos por seguridad.
            </p>
            <button
              onClick={() => window.history.back()}
              className="w-full px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
            >
              Volver
            </button>
          </div>
        </div>
      </div>
    );
  }

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

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-50 to-emerald-100">
      {/* Header */}
      <div className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-4xl mx-auto px-4 py-6">
          <div className="flex items-center gap-3">
            <div className="p-3 bg-green-100 rounded-full">
              <QrCode className="h-6 w-6 text-green-600" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-900">Acceso por Código QR</h1>
              <p className="text-gray-600">Información de la reserva</p>
            </div>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-4xl mx-auto px-4 py-8">
        {/* Success Message */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <div className="flex items-center gap-3 mb-4">
            <CheckCircle className="h-8 w-8 text-green-500" />
            <div>
              <h2 className="text-xl font-semibold text-gray-900">¡Acceso Exitoso!</h2>
              <p className="text-gray-600">{reservaData.mensajeAcceso}</p>
            </div>
          </div>
          
          <div className="bg-green-50 border border-green-200 rounded-lg p-4">
            <div className="flex items-center gap-2">
              <Smartphone className="h-5 w-5 text-green-600" />
              <span className="text-sm font-medium text-green-800">
                Acceso registrado el {new Date(reservaData.fechaAcceso).toLocaleString('es-ES')}
              </span>
            </div>
          </div>
        </div>

        {/* Reservation Details */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Info */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <div className="flex justify-between items-start mb-6">
                <h3 className="text-xl font-semibold text-gray-900">
                  {reservaData.tipoServicio}
                </h3>
                <span className={`px-3 py-1 rounded-full text-sm font-medium border ${getEstadoColor(reservaData.estado)}`}>
                  {reservaData.estado}
                </span>
              </div>

              <div className="space-y-6">
                {/* Date and Time */}
                <div className="flex items-start gap-3">
                  <Calendar className="h-5 w-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Fecha y Hora</p>
                    <p className="text-gray-900 font-medium">
                      {new Date(reservaData.fechaReserva).toLocaleDateString('es-ES', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                      })}
                    </p>
                    <p className="text-gray-600">
                      {new Date(reservaData.fechaReserva).toLocaleTimeString('es-ES', {
                        hour: '2-digit',
                        minute: '2-digit'
                      })}
                    </p>
                  </div>
                </div>

                {/* Description */}
                {reservaData.descripcion && (
                  <div className="flex items-start gap-3">
                    <FileText className="h-5 w-5 text-gray-400 mt-0.5" />
                    <div>
                      <p className="text-sm font-medium text-gray-700">Descripción</p>
                      <p className="text-gray-900 whitespace-pre-wrap">{reservaData.descripcion}</p>
                    </div>
                  </div>
                )}

                {/* Client */}
                <div className="flex items-start gap-3">
                  <User className="h-5 w-5 text-gray-400 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">Cliente</p>
                    <p className="text-gray-900 font-medium">{reservaData.clienteNombre}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            {/* QR Info */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h4 className="text-lg font-semibold text-gray-900 mb-4">Información del QR</h4>
              <div className="space-y-3">
                <div>
                  <p className="text-sm font-medium text-gray-700">ID de Reserva</p>
                  <p className="text-gray-900">#{reservaData.reservaId}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-gray-700">Fecha de Acceso</p>
                  <p className="text-gray-900">
                    {new Date(reservaData.fechaAcceso).toLocaleString('es-ES')}
                  </p>
                </div>
              </div>
            </div>

            {/* Security Notice */}
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <h5 className="text-sm font-medium text-blue-900 mb-2">🔒 Seguridad</h5>
              <p className="text-sm text-blue-800">
                Este acceso ha sido registrado por motivos de seguridad. 
                Los códigos QR expiran automáticamente después de 10 minutos.
              </p>
            </div>

            {/* Actions */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h4 className="text-lg font-semibold text-gray-900 mb-4">Acciones</h4>
              <div className="space-y-3">
                <button
                  onClick={() => window.print()}
                  className="w-full px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Imprimir Información
                </button>
                
                <button
                  onClick={() => {
                    const data = `
Reserva #${reservaData.reservaId}
Servicio: ${reservaData.tipoServicio}
Cliente: ${reservaData.clienteNombre}
Fecha: ${new Date(reservaData.fechaReserva).toLocaleString('es-ES')}
Estado: ${reservaData.estado}
${reservaData.descripcion ? `Descripción: ${reservaData.descripcion}` : ''}
                    `.trim();
                    
                    navigator.clipboard.writeText(data).then(() => {
                      alert('Información copiada al portapapeles');
                    });
                  }}
                  className="w-full px-4 py-2 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors"
                >
                  Copiar Información
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
