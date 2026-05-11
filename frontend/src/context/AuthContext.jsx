import React, { createContext, useState, useEffect } from 'react';
import { getMe, login as loginApi, register as registerApi } from '../api/authApi';
import api from '../api/axiosConfig';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAuth = async () => {
      const token = localStorage.getItem('devtask_token');
      if (token) {
        try {
          const userData = await getMe();
          setUser(userData);
        } catch (error) {
          console.error("Auth check failed", error);
          localStorage.removeItem('devtask_token');
        }
      }
      setLoading(false);
    };

    checkAuth();
  }, []);

  const login = async (credentials) => {
    const data = await loginApi(credentials);
    localStorage.setItem('devtask_token', data.token);
    setUser(data);
  };

  const register = async (userData) => {
    const data = await registerApi(userData);
    return data;
  };

  const logout = () => {
    localStorage.removeItem('devtask_token');
    setUser(null);
    window.location.href = '/login';
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};
