import React from 'react';
import { Navbar } from './Navbar';
import { useNavigate } from 'react-router-dom';

/**
 * Layout principal de la aplicación
 * ¿Por qué Layout? Para mantener estructura consistente en todas las páginas autenticadas
 */
interface LayoutProps {
  children: React.ReactNode;
  showNavbar?: boolean;
}

export const Layout: React.FC<LayoutProps> = ({ 
  children, 
  showNavbar = true 
}) => {
  const navigate = useNavigate();

  const handleNavigation = (path: string) => {
    navigate(path);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {showNavbar && <Navbar onNavigate={handleNavigation} />}
      
      <main className={showNavbar ? 'pt-0' : ''}>
        {children}
      </main>
    </div>
  );
};
