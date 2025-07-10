import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { qrService } from '../../services/qrService';
import type { QRAccessResponse } from '../../types';

/**
 * Componente para acceder a detalles de reserva via código QR
 * ¿Por qué página separada? Es funcionalidad exclusiva accesible solo por QR
 */
export default function QRAccess() {
  const { hash } = useParams<{ hash: string }>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [reservaData, setReservaData] = useState<QRAccessResponse | null>(null);

  useEffect(() => {
    if (!hash) {
      setError('Código QR inválido');
      setLoading(false);
      return;
    }

    const loadReservaData = async () => {
      try {
        setLoading(true);
        setError('');
        
        const data = await qrService.accessViaQR(hash);
        setReservaData(data);
      } catch (err: any) {
        setError(err.message || 'No se pudo acceder a la información de la reserva');
      } finally {
        setLoading(false);
      }
    };

    loadReservaData();
  }, [hash]);

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Cargando información de la reserva...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="max-w-md mx-auto text-center">
          <div className="bg-white p-8 rounded-lg shadow-md">
            <div className="text-red-500 text-6xl mb-4">⚠️</div>
            <h1 className="text-2xl font-bold text-gray-900 mb-4">
              Código QR Inválido
            </h1>
            <p className="text-gray-600 mb-6">{error}</p>
            <button
              onClick={() => window.location.href = '/'}
              className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Volver al Inicio
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (!reservaData) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600">No se encontró información de la reserva</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-2xl mx-auto px-4">
        {/* Header */}
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            📱 Acceso por Código QR
          </h1>
          <p className="text-gray-600">
            Información exclusiva accesible solo por código QR
          </p>
        </div>

        {/* Información de la reserva */}
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          <div className="bg-blue-600 text-white p-4">
            <h2 className="text-xl font-bold">
              Reserva #{reservaData.reservaId}
            </h2>
            <p className="text-blue-100">
              Estado: {reservaData.estado}
            </p>
          </div>

          <div className="p-6">
            <div className="grid md:grid-cols-2 gap-6">
              {/* Detalles principales */}
              <div>
                <h3 className="font-semibold text-gray-900 mb-3">
                  Detalles de la Reserva
                </h3>
                <div className="space-y-2">
                  <div>
                    <span className="text-gray-600">Tipo de Servicio:</span>
                    <span className="ml-2 font-medium">{reservaData.tipoServicio}</span>
                  </div>
                  <div>
                    <span className="text-gray-600">Fecha:</span>
                    <span className="ml-2 font-medium">
                      {new Date(reservaData.fechaReserva).toLocaleDateString('es-ES', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                      })}
                    </span>
                  </div>
                  <div>
                    <span className="text-gray-600">Fecha de Acceso:</span>
                    <span className="ml-2 font-medium">
                      {new Date(reservaData.fechaAcceso).toLocaleDateString()}
                    </span>
                  </div>
                </div>
              </div>

              {/* Información del usuario */}
              <div>
                <h3 className="font-semibold text-gray-900 mb-3">
                  Información del Cliente
                </h3>
                <div className="space-y-2">
                  <div>
                    <span className="text-gray-600">Cliente:</span>
                    <span className="ml-2 font-medium">{reservaData.clienteNombre}</span>
                  </div>
                  <div>
                    <span className="text-gray-600">ID Reserva:</span>
                    <span className="ml-2 font-medium">{reservaData.reservaId}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Descripción */}
            {reservaData.descripcion && (
              <div className="mt-6">
                <h3 className="font-semibold text-gray-900 mb-3">
                  Descripción
                </h3>
                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-gray-700">{reservaData.descripcion}</p>
                </div>
              </div>
            )}

            {/* Mensaje de acceso */}
            {reservaData.mensajeAcceso && (
              <div className="mt-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
                <h3 className="font-semibold text-blue-800 mb-2">
                  📋 Mensaje del Sistema
                </h3>
                <p className="text-blue-700">{reservaData.mensajeAcceso}</p>
              </div>
            )}

            {/* Información adicional solo disponible por QR */}
            <div className="mt-6 p-4 bg-green-50 border border-green-200 rounded-lg">
              <h3 className="font-semibold text-green-800 mb-2 flex items-center">
                ✅ Información Exclusiva por QR
              </h3>
              <div className="text-sm text-green-700 space-y-1">
                <p>• Acceso verificado mediante código QR</p>
                <p>• Información adicional de seguridad disponible</p>
                <p>• Acceso temporal válido por tiempo limitado</p>
                <p>• Código generado por el propietario de la reserva</p>
              </div>
            </div>

            {/* Información de acceso */}
            <div className="mt-6 text-center">
              <p className="text-sm text-gray-500">
                Accedido el {new Date().toLocaleString('es-ES')}
              </p>
              <p className="text-xs text-gray-400 mt-1">
                Este acceso ha sido registrado por motivos de seguridad
              </p>
            </div>
          </div>
        </div>

        {/* Acciones */}
        <div className="mt-8 text-center space-x-4">
          <button
            onClick={() => window.print()}
            className="px-6 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
          >
            Imprimir
          </button>
          <button
            onClick={() => window.location.href = '/'}
            className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Volver al Inicio
          </button>
        </div>
      </div>
    </div>
  );
}
