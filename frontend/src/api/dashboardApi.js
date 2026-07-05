import api from './axiosConfig';

/**
 * Consume el endpoint GET /api/dashboard/stats del backend.
 * Retorna las estadísticas consolidadas: proyectos, tareas y desarrolladores.
 */
export const getDashboardStats = () =>
  api.get('/dashboard/stats').then(res => res.data);
