import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getTasks, deleteTask } from '../../api/tasksApi';
import { usePagination } from '../../hooks/usePagination';
import Spinner from '../../components/ui/Spinner';
import ErrorMessage from '../../components/ui/ErrorMessage';
import Pagination from '../../components/ui/Pagination';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import { formatDate } from '../../utils/formatters';
import DueDateBadge from '../../components/ui/DueDateBadge';
import { TASK_STATUSES, TASK_PRIORITIES } from '../../utils/constants';

const TasksPage = () => {
  const { page, size, handlePageChange } = usePagination();
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');

  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ['tasks', { page, size, status: statusFilter, priority: priorityFilter }],
    queryFn: () => getTasks({ page, size, status: statusFilter, priority: priorityFilter }),
    keepPreviousData: true
  });

  const handleDelete = async (id) => {
    if (window.confirm('¿Está seguro de que desea eliminar esta tarea?')) {
      try {
        await deleteTask(id);
        refetch();
      } catch (err) {
        alert('Error al eliminar la tarea');
      }
    }
  };

  if (isLoading) return <Spinner />;
  if (isError) return <ErrorMessage message={error.message} />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-semibold text-gray-900">Tareas</h1>
        <Link to="/tasks/new">
          <Button>Nueva Tarea</Button>
        </Link>
      </div>

      <div className="bg-white p-4 shadow sm:rounded-lg flex flex-wrap items-center gap-4">
        <div className="flex items-center space-x-2">
          <span className="text-sm font-medium text-gray-700">Estado:</span>
          <select 
            className="border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value); handlePageChange(1); }}
          >
            <option value="">Todos los Estados</option>
            {TASK_STATUSES.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
          </select>
        </div>
        <div className="flex items-center space-x-2">
          <span className="text-sm font-medium text-gray-700">Prioridad:</span>
          <select 
            className="border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            value={priorityFilter}
            onChange={(e) => { setPriorityFilter(e.target.value); handlePageChange(1); }}
          >
            <option value="">Todas las Prioridades</option>
            {TASK_PRIORITIES.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
          </select>
        </div>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Código</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Título / Proyecto</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estado</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Prioridad</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha Límite</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items?.length === 0 && (
              <tr><td colSpan="6" className="px-6 py-4 text-center text-sm text-gray-500">No se encontraron tareas.</td></tr>
            )}
            {data?.items?.map((task) => (
              <tr key={task.id}>
                {/* R3: Código inamovible prominente */}
                <td className="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-700">
                  <span className="bg-gray-100 border border-gray-300 text-gray-800 px-2 py-1 rounded">{task.taskCode}</span>
                </td>
                <td className="px-6 py-4 text-sm font-medium text-gray-900">
                  <Link to={`/tasks/${task.id}`} className="hover:text-indigo-600 block">{task.title}</Link>
                  <span className="text-xs text-gray-500 font-normal">{task.projectName}</span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <Badge type="status" value={task.status} />
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <Badge type="priority" value={task.priority} />
                </td>
                <td className="px-6 py-4 whitespace-nowrap"><DueDateBadge dueDate={task.dueDate} /></td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <Link to={`/tasks/${task.id}`} className="text-indigo-600 hover:text-indigo-900 mr-4">Ver</Link>
                  <Link to={`/tasks/${task.id}/edit`} className="text-blue-600 hover:text-blue-900 mr-4">Editar</Link>
                  <button onClick={() => handleDelete(task.id)} className="text-red-600 hover:text-red-900">Eliminar</button>
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

export default TasksPage;
