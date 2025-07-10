import { useState, useEffect } from 'react';
import QRCode from 'qrcode';
import { qrService } from '../../services/qrService';
import type { Reserva } from '../../types';

interface QRDisplayProps {
  reserva: Reserva;
  onClose?: () => void;
}

/**
 * Componente para mostrar y generar códigos QR
 * ¿Por qué separado? Los QR tienen funcionalidad específica y reutilizable
 */
export default function QRDisplay({ reserva, onClose }: QRDisplayProps) {
  const [qrImageUrl, setQrImageUrl] = useState<string>('');
  const [qrUrl, setQrUrl] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');

  const generateQR = async () => {
    setLoading(true);
    setError('');
    
    try {
      // 1. Generar QR desde la API
      const qrResponse = await qrService.generateQR(reserva.id);
      setQrUrl(qrResponse.qrUrl);

      // 2. Generar imagen QR en el frontend
      const qrImage = await QRCode.toDataURL(qrResponse.qrUrl, {
        errorCorrectionLevel: 'M',
        type: 'image/png',
        margin: 1,
        color: {
          dark: '#000000',
          light: '#FFFFFF',
        },
        width: 256,
      });
      
      setQrImageUrl(qrImage);
    } catch (err: any) {
      setError(err.message || 'Error al generar código QR');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    generateQR();
  }, [reserva.id]);

  const copyToClipboard = async () => {
    try {
      await navigator.clipboard.writeText(qrUrl);
      // TODO: Mostrar notificación de éxito
    } catch (err) {
      console.error('Error al copiar al portapapeles:', err);
    }
  };

  const downloadQR = () => {
    if (!qrImageUrl) return;
    
    const link = document.createElement('a');
    link.download = `qr-reserva-${reserva.id}.png`;
    link.href = qrImageUrl;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900">
            Código QR - Reserva #{reserva.id}
          </h2>
          {onClose && (
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600 text-2xl"
            >
              ×
            </button>
          )}
        </div>

        {/* Información de la reserva */}
        <div className="mb-4 p-3 bg-gray-50 rounded">
          <p className="text-sm text-gray-600">
            <strong>Servicio:</strong> {reserva.tipoServicio}
          </p>
          <p className="text-sm text-gray-600">
            <strong>Fecha:</strong> {new Date(reserva.fechaReserva).toLocaleDateString()}
          </p>
          <p className="text-sm text-gray-600">
            <strong>Estado:</strong> {reserva.estado}
          </p>
        </div>

        {/* Mostrar QR */}
        {loading && (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            <span className="ml-3 text-gray-600">Generando código QR...</span>
          </div>
        )}

        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-red-700">
            {error}
          </div>
        )}

        {qrImageUrl && !loading && !error && (
          <div className="text-center">
            <div className="mb-4 flex justify-center">
              <img
                src={qrImageUrl}
                alt="Código QR"
                className="border border-gray-200 rounded"
              />
            </div>

            <p className="text-sm text-gray-600 mb-4">
              Escanea este código QR para acceder a los detalles de la reserva
            </p>

            {/* URL del QR */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                URL del código QR:
              </label>
              <div className="flex">
                <input
                  type="text"
                  value={qrUrl}
                  readOnly
                  className="flex-1 p-2 border border-gray-300 rounded-l text-sm bg-gray-50"
                />
                <button
                  onClick={copyToClipboard}
                  className="px-3 py-2 bg-blue-600 text-white text-sm rounded-r hover:bg-blue-700"
                >
                  Copiar
                </button>
              </div>
            </div>

            {/* Botones de acción */}
            <div className="flex space-x-3">
              <button
                onClick={downloadQR}
                className="flex-1 py-2 px-4 bg-green-600 text-white rounded hover:bg-green-700"
              >
                Descargar QR
              </button>
              <button
                onClick={generateQR}
                className="flex-1 py-2 px-4 bg-gray-600 text-white rounded hover:bg-gray-700"
              >
                Regenerar
              </button>
            </div>

            {/* Nota de expiración */}
            <p className="text-xs text-gray-500 mt-3">
              ⏰ Este código QR expira en 10 minutos por seguridad
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
