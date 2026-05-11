import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getDevelopers, deleteDeveloper } from '../../api/developersApi';
import { usePagination } from '../../hooks/usePagination';
import Spinner from '../../components/ui/Spinner';
import ErrorMessage from '../../components/ui/ErrorMessage';
import Pagination from '../../components/ui/Pagination';
import Button from '../../components/ui/Button';

const DevelopersPage = () => {
  const { page, size, handlePageChange } = usePagination();

  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ['developers', { page, size }],
    queryFn: () => getDevelopers({ page, size }),
    keepPreviousData: true
  });

  const handleDelete = async (id) => {
    if (window.confirm('¿Está seguro de que desea desactivar este desarrollador?')) {
      try {
        await deleteDeveloper(id);
        refetch();
      } catch (err) {
        alert('Error al desactivar desarrollador');
      }
    }
  };

  if (isLoading) return <Spinner />;
  if (isError) return <ErrorMessage message={error.message} />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-semibold text-gray-900">Desarrolladores</h1>
        <Link to="/developers/new">
          <Button>Agregar Desarrollador</Button>
        </Link>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cédula</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Habilidades</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nivel</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items?.length === 0 && (
              <tr><td colSpan="6" className="px-6 py-4 text-center text-sm text-gray-500">No se encontraron desarrolladores.</td></tr>
            )}
            {data?.items?.map((dev) => (
              <tr key={dev.id}>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{dev.fullName}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{dev.cedula}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{dev.projectTypeName}</td>
                <td className="px-6 py-4 text-sm text-gray-500 max-w-xs truncate" title={dev.technologies?.join(', ')}>
                  {dev.technologies?.join(', ') || 'N/A'}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 capitalize">{dev.seniorityLevel}</td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <Link to={`/developers/${dev.id}/edit`} className="text-indigo-600 hover:text-indigo-900 mr-4">Editar</Link>
                  <button onClick={() => handleDelete(dev.id)} className="text-red-600 hover:text-red-900">Eliminar</button>
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

export default DevelopersPage;
