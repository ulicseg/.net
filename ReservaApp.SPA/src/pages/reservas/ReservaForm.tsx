import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  ArrowLeft, 
  Calendar, 
  FileText, 
  Save, 
  AlertCircle
} from 'lucide-react';
import { Layout } from '../../components/layout/Layout';
import { reservaService } from '../../services/reservaService';
import { TipoServicioOptions } from '../../types';
import type { TipoServicio, CreateReservaRequest, UpdateReservaRequest } from '../../types';

interface FormData {
  titulo: string;
  descripcion: string;
  fechaReserva: string;
  tipoServicio: TipoServicio;
}

export default function ReservaForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditing = !!id;
  
  const [formData, setFormData] = useState<FormData>({
    titulo: '',
    descripcion: '',
    fechaReserva: '',
    tipoServicio: 'ConsultaMedica'
  });
  
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [validationErrors, setValidationErrors] = useState<{[key: string]: string}>({});

  useEffect(() => {
    if (isEditing && id) {
      loadReserva();
    }
  }, [isEditing, id]);

  const loadReserva = async () => {
    if (!id) return;
    
    try {
      setIsLoading(true);
      const reserva = await reservaService.getReservaById(parseInt(id));
      
      // Formatear fecha para input datetime-local
      const fechaFormatted = new Date(reserva.fechaReserva).toISOString().slice(0, 16);
      
      setFormData({
        titulo: reserva.titulo,
        descripcion: reserva.descripcion || '',
        fechaReserva: fechaFormatted,
        tipoServicio: reserva.tipoServicio
      });
    } catch (err) {
      setError('Error al cargar la reserva');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Limpiar error de validación para este campo
    if (validationErrors[name]) {
      setValidationErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = (): boolean => {
    const errors: {[key: string]: string} = {};
    
    if (!formData.titulo.trim()) {
      errors.titulo = 'El título es obligatorio';
    } else if (formData.titulo.length > 100) {
      errors.titulo = 'El título no puede exceder 100 caracteres';
    }
    
    if (!formData.fechaReserva) {
      errors.fechaReserva = 'La fecha de reserva es obligatoria';
    } else {
      const fechaReserva = new Date(formData.fechaReserva);
      const ahora = new Date();
      if (fechaReserva < ahora) {
        errors.fechaReserva = 'La fecha de reserva debe ser futura';
      }
    }
    
    if (formData.descripcion.length > 500) {
      errors.descripcion = 'La descripción no puede exceder 500 caracteres';
    }
    
    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }
    
    setIsSaving(true);
    setError('');
    
    try {
      const requestData = {
        titulo: formData.titulo.trim(),
        descripcion: formData.descripcion.trim() || undefined,
        fechaReserva: formData.fechaReserva,
        tipoServicio: formData.tipoServicio
      };
      
      if (isEditing && id) {
        await reservaService.updateReserva(parseInt(id), requestData as UpdateReservaRequest);
      } else {
        await reservaService.createReserva(requestData as CreateReservaRequest);
      }
      
      navigate('/dashboard');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al guardar la reserva');
    } finally {
      setIsSaving(false);
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

  return (
    <Layout>
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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
              <h1 className="text-3xl font-bold text-gray-900">
                {isEditing ? 'Editar Reserva' : 'Nueva Reserva'}
              </h1>
              <p className="text-gray-600">
                {isEditing ? 'Modifica los detalles de tu reserva' : 'Crea una nueva reserva'}
              </p>
            </div>
          </div>
        </div>

        {/* Error Alert */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg flex items-start gap-3">
            <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
            <p className="text-red-600">{error}</p>
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 space-y-6">
          {/* Título */}
          <div>
            <label htmlFor="titulo" className="block text-sm font-medium text-gray-700 mb-2">
              Título *
            </label>
            <div className="relative">
              <FileText className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input
                id="titulo"
                name="titulo"
                type="text"
                required
                value={formData.titulo}
                onChange={handleInputChange}
                className={`w-full pl-10 pr-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                  validationErrors.titulo ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="Ej: Consulta médica general"
                maxLength={100}
              />
            </div>
            {validationErrors.titulo && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.titulo}</p>
            )}
            <p className="mt-1 text-xs text-gray-500">{formData.titulo.length}/100 caracteres</p>
          </div>

          {/* Tipo de Servicio */}
          <div>
            <label htmlFor="tipoServicio" className="block text-sm font-medium text-gray-700 mb-2">
              Tipo de Servicio *
            </label>
            <select
              id="tipoServicio"
              name="tipoServicio"
              required
              value={formData.tipoServicio}
              onChange={handleInputChange}
              className="w-full px-3 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            >
              {Object.entries(TipoServicioOptions).map(([key, label]) => (
                <option key={key} value={key}>
                  {label}
                </option>
              ))}
            </select>
          </div>

          {/* Fecha y Hora */}
          <div>
            <label htmlFor="fechaReserva" className="block text-sm font-medium text-gray-700 mb-2">
              Fecha y Hora *
            </label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input
                id="fechaReserva"
                name="fechaReserva"
                type="datetime-local"
                required
                value={formData.fechaReserva}
                onChange={handleInputChange}
                min={new Date().toISOString().slice(0, 16)}
                className={`w-full pl-10 pr-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                  validationErrors.fechaReserva ? 'border-red-300' : 'border-gray-300'
                }`}
              />
            </div>
            {validationErrors.fechaReserva && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.fechaReserva}</p>
            )}
          </div>

          {/* Descripción */}
          <div>
            <label htmlFor="descripcion" className="block text-sm font-medium text-gray-700 mb-2">
              Descripción
            </label>
            <textarea
              id="descripcion"
              name="descripcion"
              rows={4}
              value={formData.descripcion}
              onChange={handleInputChange}
              className={`w-full px-3 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors resize-none ${
                validationErrors.descripcion ? 'border-red-300' : 'border-gray-300'
              }`}
              placeholder="Describe los detalles adicionales de tu reserva..."
              maxLength={500}
            />
            {validationErrors.descripcion && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.descripcion}</p>
            )}
            <p className="mt-1 text-xs text-gray-500">{formData.descripcion.length}/500 caracteres</p>
          </div>

          {/* Actions */}
          <div className="flex flex-col sm:flex-row gap-3 pt-6 border-t border-gray-200">
            <button
              type="submit"
              disabled={isSaving}
              className="flex-1 inline-flex items-center justify-center px-6 py-3 bg-blue-600 text-white rounded-lg font-medium hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {isSaving ? (
                <>
                  <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-2"></div>
                  {isEditing ? 'Guardando...' : 'Creando...'}
                </>
              ) : (
                <>
                  <Save className="h-5 w-5 mr-2" />
                  {isEditing ? 'Guardar Cambios' : 'Crear Reserva'}
                </>
              )}
            </button>
            
            <button
              type="button"
              onClick={() => navigate('/dashboard')}
              className="flex-1 sm:flex-initial px-6 py-3 bg-gray-100 text-gray-700 rounded-lg font-medium hover:bg-gray-200 focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 transition-colors"
            >
              Cancelar
            </button>
          </div>
        </form>

        {/* Tips */}
        <div className="mt-8 bg-blue-50 border border-blue-200 rounded-lg p-4">
          <h3 className="text-sm font-medium text-blue-900 mb-2">💡 Consejos</h3>
          <ul className="text-sm text-blue-800 space-y-1">
            <li>• Asegúrate de que la fecha y hora sean correctas</li>
            <li>• Proporciona una descripción clara para facilitar la identificación</li>
            <li>• Puedes editar la reserva más tarde si es necesario</li>
          </ul>
        </div>
      </div>
    </Layout>
  );
}
