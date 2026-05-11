import React from 'react';
import { useAuth } from '../../hooks/useAuth';
import Button from '../ui/Button';

const Header = () => {
  const { user, logout } = useAuth();

  return (
    <header className="bg-white shadow">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-gray-900">Dashboard</h1>
        <div className="flex items-center space-x-4">
          <span className="text-sm text-gray-700">
            Bienvenido, <strong>{user?.username}</strong>
            {user?.role === 'admin' && <span className="ml-2 px-2 py-1 bg-indigo-100 text-indigo-800 text-xs rounded-full">Admin</span>}
          </span>
          <Button variant="secondary" onClick={logout} className="text-sm px-3 py-1">
            Cerrar Sesión
          </Button>
        </div>
      </div>
    </header>
  );
};

export default Header;
