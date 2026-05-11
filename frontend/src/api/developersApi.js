import api from './axiosConfig';

export const getDevelopers = async ({ page = 1, size = 10, status = '', seniority = '', technologyId = '', projectTypeId = '' }) => {
  const params = new URLSearchParams({ page, size });
  if (status) params.append('status', status);
  if (seniority) params.append('seniority', seniority);
  if (technologyId) params.append('technologyId', technologyId);
  if (projectTypeId) params.append('projectTypeId', projectTypeId);
  const response = await api.get(`/developers?${params.toString()}`);
  return response.data;
};

export const getDeveloper = async (id) => {
  const response = await api.get(`/developers/${id}`);
  return response.data;
};

export const createDeveloper = async (data) => {
  const response = await api.post('/developers', data);
  return response.data;
};

export const updateDeveloper = async (id, data) => {
  const response = await api.put(`/developers/${id}`, data);
  return response.data;
};

export const deleteDeveloper = async (id) => {
  await api.delete(`/developers/${id}`);
};
