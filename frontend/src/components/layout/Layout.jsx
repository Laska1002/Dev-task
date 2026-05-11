import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import Sidebar from './Sidebar';
import Header from './Header';
import Spinner from '../ui/Spinner';

const Layout = () => {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="h-screen w-screen flex items-center justify-center bg-gray-50">
        <Spinner />
      </div>
    );
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="flex h-screen overflow-hidden bg-gray-100">
      <Sidebar />
      <div className="flex flex-col flex-1 w-0 overflow-hidden">
        <Header />
        <main className="flex-1 relative overflow-y-auto focus:outline-none p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default Layout;
