import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getProjects, deleteProject } from '../../api/projectsApi';
import { usePagination } from '../../hooks/usePagination';
import Spinner from '../../components/ui/Spinner';
import ErrorMessage from '../../components/ui/ErrorMessage';
import Pagination from '../../components/ui/Pagination';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import { formatDate } from '../../utils/formatters';
import DueDateBadge from '../../components/ui/DueDateBadge';
import { PROJECT_STATUSES } from '../../utils/constants';

const ProjectsPage = () => {
  const { page, size, handlePageChange } = usePagination();
  const [statusFilter, setStatusFilter] = useState('');

  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ['projects', { page, size, status: statusFilter }],
    queryFn: () => getProjects({ page, size, status: statusFilter }),
    keepPreviousData: true
  });

  const handleDelete = async (id) => {
    if (window.confirm('¿Está seguro de que desea eliminar este proyecto?')) {
      try {
        await deleteProject(id);
        refetch();
      } catch (err) {
        alert('Error al eliminar el proyecto');
      }
    }
  };

  if (isLoading) return <Spinner />;
  if (isError) return <ErrorMessage message={error.message} />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-semibold text-gray-900">Proyectos</h1>
        <Link to="/projects/new">
          <Button>Nuevo Proyecto</Button>
        </Link>
      </div>

      <div className="bg-white p-4 shadow sm:rounded-lg flex items-center space-x-4">
        <span className="text-sm font-medium text-gray-700">Filtrar por estado:</span>
        <select 
          className="border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
          value={statusFilter}
          onChange={(e) => { setStatusFilter(e.target.value); handlePageChange(1); }}
        >
          <option value="">Todos los Estados</option>
          {PROJECT_STATUSES.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
        </select>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Código</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estado</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha Fin</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items?.length === 0 && (
              <tr><td colSpan="5" className="px-6 py-4 text-center text-sm text-gray-500">No se encontraron proyectos.</td></tr>
            )}
            {data?.items?.map((project) => (
              <tr key={project.id}>
                {/* R3: Código inamovible prominente */}
                <td className="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-700">
                  <span className="bg-gray-100 text-gray-800 px-2 py-1 rounded">{project.projectCode}</span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  <Link to={`/projects/${project.id}`} className="hover:text-indigo-600">{project.name}</Link>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <Badge type="status" value={project.status} />
                </td>
                <td className="px-6 py-4 whitespace-nowrap"><DueDateBadge dueDate={project.dueDate} /></td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <Link to={`/projects/${project.id}`} className="text-indigo-600 hover:text-indigo-900 mr-4">Ver</Link>
                  <Link to={`/projects/${project.id}/edit`} className="text-blue-600 hover:text-blue-900 mr-4">Editar</Link>
                  <button onClick={() => handleDelete(project.id)} className="text-red-600 hover:text-red-900">Eliminar</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data && <Pagination page={page} size={size} total={data.total} onPageChange={handlePageChange} />}
      </div>
    </div>
  );
};

export default ProjectsPage;
