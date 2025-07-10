import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import PrivateRoute from './components/auth/PrivateRoute';

// Pages
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import Dashboard from './pages/reservas/Dashboard';
import ReservaDetalle from './pages/reservas/ReservaDetalle';
import ReservaForm from './pages/reservas/ReservaForm';
import QRAccess from './pages/qr/QRAccess';

import './App.css'

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="min-h-screen bg-gray-50">
          <Routes>
            {/* Public Routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/qr/:hash" element={<QRAccess />} />
            
            {/* Private Routes */}
            <Route path="/dashboard" element={
              <PrivateRoute>
                <Dashboard />
              </PrivateRoute>
            } />
            
            <Route path="/reservas/nueva" element={
              <PrivateRoute>
                <ReservaForm />
              </PrivateRoute>
            } />
            
            <Route path="/reservas/:id" element={
              <PrivateRoute>
                <ReservaDetalle />
              </PrivateRoute>
            } />
            
            <Route path="/reservas/:id/editar" element={
              <PrivateRoute>
                <ReservaForm />
              </PrivateRoute>
            } />
            
            {/* Redirects */}
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App
