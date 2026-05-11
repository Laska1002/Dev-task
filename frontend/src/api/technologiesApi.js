import api from './axiosConfig';

export const getTechnologies = async (projectTypeId = null) => {
  const url = projectTypeId ? `/technologies?projectTypeId=${projectTypeId}` : '/technologies';
  const response = await api.get(url);
  return response.data;
};
