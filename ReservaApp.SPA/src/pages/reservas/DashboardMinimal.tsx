import { Layout } from '../../components/layout/Layout';

export default function DashboardMinimal() {
  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <h1 className="text-3xl font-bold text-gray-900">
          Dashboard Mínimo Funciona! 🎉
        </h1>
        <p className="text-gray-600 mt-4">
          Si ves esto, el problema no está en el Layout.
        </p>
      </div>
    </Layout>
  );
}
