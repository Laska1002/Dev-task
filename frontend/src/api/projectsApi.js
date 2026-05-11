import api from './axiosConfig';

export const getProjects = async ({ page = 1, size = 10, status = '', typeId = '' }) => {
  const params = new URLSearchParams({ page, size });
  if (status) params.append('status', status);
  if (typeId) params.append('typeId', typeId);
  const response = await api.get(`/projects?${params.toString()}`);
  return response.data;
};

export const getProject = async (id) => {
  const response = await api.get(`/projects/${id}`);
  return response.data;
};

export const createProject = async (data) => {
  const response = await api.post('/projects', data);
  return response.data;
};

export const updateProject = async (id, data) => {
  const response = await api.put(`/projects/${id}`, data);
  return response.data;
};

export const deleteProject = async (id) => {
  await api.delete(`/projects/${id}`);
};

export const getProjectTypes = async () => {
  const response = await api.get('/project-types');
  return response.data;
};

export const assignDevelopers = async (id, data) => {
  const response = await api.post(`/projects/${id}/developers`, data);
  return response.data;
};

export const removeDeveloperFromProject = async (projectId, developerId) => {
  await api.delete(`/projects/${projectId}/developers/${developerId}`);
};

export const getProjectDevelopers = async (id) => {
  const response = await api.get(`/projects/${id}/developers`);
  return response.data;
};
