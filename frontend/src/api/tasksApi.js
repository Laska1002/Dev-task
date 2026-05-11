import api from './axiosConfig';

export const getTasks = async ({ page = 1, size = 10, projectId = '', status = '', priority = '', assignedTo = '' }) => {
  const params = new URLSearchParams({ page, size });
  if (projectId) params.append('projectId', projectId);
  if (status) params.append('status', status);
  if (priority) params.append('priority', priority);
  if (assignedTo) params.append('assignedTo', assignedTo);
  const response = await api.get(`/tasks?${params.toString()}`);
  return response.data;
};

export const getTask = async (id) => {
  const response = await api.get(`/tasks/${id}`);
  return response.data;
};

export const createTask = async (data) => {
  const response = await api.post('/tasks', data);
  return response.data;
};

export const updateTask = async (id, data) => {
  const response = await api.put(`/tasks/${id}`, data);
  return response.data;
};

export const patchTaskStatus = async (id, status) => {
  const response = await api.patch(`/tasks/${id}/status`, { status });
  return response.data;
};

export const deleteTask = async (id) => {
  await api.delete(`/tasks/${id}`);
};

export const getTaskComments = async (id) => {
  const response = await api.get(`/tasks/${id}/comments`);
  return response.data;
};

export const addTaskComment = async (id, data) => {
  const response = await api.post(`/tasks/${id}/comments`, data);
  return response.data;
};
