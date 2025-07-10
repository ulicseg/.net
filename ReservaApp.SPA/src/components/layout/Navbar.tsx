import React from 'react';
import { LogOut, User, Calendar, QrCode } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';

/**
 * Barra de navegación principal
 * ¿Por qué componentizar? Para reutilizar en todas las páginas y centralizar la navegación
 */
interface NavbarProps {
  onNavigate?: (path: string) => void;
}

export const Navbar: React.FC<NavbarProps> = ({ onNavigate }) => {
  const { user, logout } = useAuth();

  const handleNavigation = (path: string) => {
    if (onNavigate) {
      onNavigate(path);
    }
  };

  return (
    <nav className="bg-white shadow-sm border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo y título */}
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <h1 className="text-xl font-bold text-primary-600">
                Sistema de Reservas
              </h1>
            </div>
          </div>

          {/* Navegación central */}
          <div className="hidden md:block">
            <div className="ml-10 flex items-baseline space-x-4">
              <button
                onClick={() => handleNavigation('/dashboard')}
                className="text-gray-600 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200 flex items-center gap-2"
              >
                <Calendar className="w-4 h-4" />
                Mis Reservas
              </button>
              <button
                onClick={() => handleNavigation('/qr-scanner')}
                className="text-gray-600 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200 flex items-center gap-2"
              >
                <QrCode className="w-4 h-4" />
                Escanear QR
              </button>
            </div>
          </div>

          {/* Usuario y logout */}
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2">
              <User className="w-5 h-5 text-gray-400" />
              <span className="text-sm text-gray-700">
                {user?.nombreCompleto || 'Usuario'}
              </span>
            </div>
            <button
              onClick={logout}
              className="text-gray-400 hover:text-red-600 transition-colors duration-200"
              title="Cerrar sesión"
            >
              <LogOut className="w-5 h-5" />
            </button>
          </div>
        </div>
      </div>

      {/* Navegación móvil */}
      <div className="md:hidden">
        <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3 border-t border-gray-200">
          <button
            onClick={() => handleNavigation('/dashboard')}
            className="text-gray-600 hover:text-primary-600 block px-3 py-2 rounded-md text-base font-medium w-full text-left transition-colors duration-200"
          >
            <Calendar className="w-4 h-4 inline mr-2" />
            Mis Reservas
          </button>
          <button
            onClick={() => handleNavigation('/qr-scanner')}
            className="text-gray-600 hover:text-primary-600 block px-3 py-2 rounded-md text-base font-medium w-full text-left transition-colors duration-200"
          >
            <QrCode className="w-4 h-4 inline mr-2" />
            Escanear QR
          </button>
        </div>
      </div>
    </nav>
  );
};
