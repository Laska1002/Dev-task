import api from './axiosConfig';

export const getProjectTypes = async () => {
  const response = await api.get('/project-types');
  return response.data;
};
