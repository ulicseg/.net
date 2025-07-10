import { useState, useEffect } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { Layout } from '../../components/layout/Layout';

export default function DashboardProgressive() {
  const { user } = useAuth();
  const [step, setStep] = useState(1);

  // Paso 1: Solo mostrar usuario
  if (step === 1) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-gray-900">
              ¡Hola, {user?.nombre}! 👋
            </h1>
            <p className="text-gray-600 mt-2">
              Gestiona tus reservas de forma fácil y rápida
            </p>
          </div>
          
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-green-600 font-semibold">✅ Paso 1: Header funcionando</p>
            <button 
              onClick={() => setStep(2)}
              className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Continuar a Paso 2
            </button>
          </div>
        </div>
      </Layout>
    );
  }

  // Paso 2: Agregar cards de estadísticas
  if (step === 2) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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
              <div className="text-2xl font-bold text-gray-900">0</div>
              <p className="text-sm font-medium text-gray-600">Total Reservas</p>
            </div>
            <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
              <div className="text-2xl font-bold text-gray-900">0</div>
              <p className="text-sm font-medium text-gray-600">Confirmadas</p>
            </div>
            <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
              <div className="text-2xl font-bold text-gray-900">0</div>
              <p className="text-sm font-medium text-gray-600">Pendientes</p>
            </div>
          </div>
          
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-green-600 font-semibold">✅ Paso 2: Stats cards funcionando</p>
            <button 
              onClick={() => setStep(3)}
              className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Continuar a Paso 3
            </button>
            <button 
              onClick={() => setStep(1)}
              className="mt-4 ml-4 px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
            >
              Volver a Paso 1
            </button>
          </div>
        </div>
      </Layout>
    );
  }

  // Paso 3: Estado inicial con carga
  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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
            <div className="text-2xl font-bold text-gray-900">0</div>
            <p className="text-sm font-medium text-gray-600">Total Reservas</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
            <div className="text-2xl font-bold text-gray-900">0</div>
            <p className="text-sm font-medium text-gray-600">Confirmadas</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
            <div className="text-2xl font-bold text-gray-900">0</div>
            <p className="text-sm font-medium text-gray-600">Pendientes</p>
          </div>
        </div>

        {/* Loading State */}
        <div className="flex justify-center items-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow mt-6">
          <p className="text-green-600 font-semibold">✅ Paso 3: Loading state funcionando</p>
          <p className="text-gray-600 mt-2">
            Si llegaste hasta aquí, el problema debe estar en la llamada a la API o en el renderizado de reservas.
          </p>
          <button 
            onClick={() => setStep(2)}
            className="mt-4 px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
          >
            Volver a Paso 2
          </button>
        </div>
      </div>
    </Layout>
  );
}
