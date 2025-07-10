import { useAuth } from '../../contexts/AuthContext';
import { Layout } from '../../components/layout/Layout';

export default function DashboardSimple() {
  const { user } = useAuth();

  return (
    <Layout>
      <div className="p-4">
        <h1 className="text-2xl font-bold text-gray-900 mb-4">Dashboard Simple</h1>
        <div className="bg-white rounded-lg shadow p-6">
          <p className="text-lg">¡Bienvenido, {user?.nombre || 'Usuario'}!</p>
          <p className="text-gray-600 mt-2">Email: {user?.email}</p>
          <p className="text-gray-600">Esta es una versión simple del dashboard para verificar que todo funciona.</p>
        </div>
      </div>
    </Layout>
  );
}
