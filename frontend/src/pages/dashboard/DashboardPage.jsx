import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { getProjects } from '../../api/projectsApi';
import { getTasks } from '../../api/tasksApi';
import Spinner from '../../components/ui/Spinner';
import ErrorMessage from '../../components/ui/ErrorMessage';
import { Link } from 'react-router-dom';
import Badge from '../../components/ui/Badge';
import { formatDate } from '../../utils/formatters';
import DueDateBadge from '../../components/ui/DueDateBadge';

const DashboardPage = () => {
  const { data: projectsData, isLoading: loadingProjects, error: projectsError } = useQuery({
    queryKey: ['projects', { size: 5 }],
    queryFn: () => getProjects({ size: 5 })
  });

  const { data: tasksData, isLoading: loadingTasks, error: tasksError } = useQuery({
    queryKey: ['tasks', { size: 5, status: 'todo' }],
    queryFn: () => getTasks({ size: 5, status: 'todo' })
  });

  if (loadingProjects || loadingTasks) return <Spinner />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold text-gray-900">Resumen del Dashboard</h1>
      
      {(projectsError || tasksError) && (
        <ErrorMessage message="Error al cargar los datos del dashboard." />
      )}

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        {/* Recent Projects */}
        <div className="bg-white shadow overflow-hidden sm:rounded-md">
          <div className="px-4 py-5 border-b border-gray-200 sm:px-6 flex justify-between items-center">
            <h3 className="text-lg leading-6 font-medium text-gray-900">Proyectos Recientes</h3>
            <Link to="/projects" className="text-sm font-medium text-indigo-600 hover:text-indigo-500">Ver todos</Link>
          </div>
          <ul className="divide-y divide-gray-200">
            {projectsData?.items?.length === 0 ? (
              <li className="px-4 py-4 sm:px-6 text-sm text-gray-500">No se encontraron proyectos.</li>
            ) : (
              projectsData?.items?.map((project) => (
                <li key={project.id}>
                  <Link to={`/projects/${project.id}`} className="block hover:bg-gray-50">
                    <div className="px-4 py-4 sm:px-6">
                      <div className="flex items-center justify-between">
                        <p className="text-sm font-medium text-indigo-600 truncate">{project.name}</p>
                        <div className="ml-2 flex-shrink-0 flex">
                          <Badge type="status" value={project.status} />
                        </div>
                      </div>
                      <div className="mt-2 sm:flex sm:justify-between">
                        <div className="sm:flex">
                          <p className="flex items-center text-sm text-gray-500">
                            {project.projectCode}
                          </p>
                        </div>
                          <DueDateBadge dueDate={project.dueDate} />
                      </div>
                    </div>
                  </Link>
                </li>
              ))
            )}
          </ul>
        </div>

        {/* Pending Tasks */}
        <div className="bg-white shadow overflow-hidden sm:rounded-md">
          <div className="px-4 py-5 border-b border-gray-200 sm:px-6 flex justify-between items-center">
            <h3 className="text-lg leading-6 font-medium text-gray-900">Tareas Pendientes (Por Hacer)</h3>
            <Link to="/tasks" className="text-sm font-medium text-indigo-600 hover:text-indigo-500">Ver todas</Link>
          </div>
          <ul className="divide-y divide-gray-200">
            {tasksData?.items?.length === 0 ? (
              <li className="px-4 py-4 sm:px-6 text-sm text-gray-500">No hay tareas pendientes.</li>
            ) : (
              tasksData?.items?.map((task) => (
                <li key={task.id}>
                  <Link to={`/tasks/${task.id}`} className="block hover:bg-gray-50">
                    <div className="px-4 py-4 sm:px-6">
                      <div className="flex items-center justify-between">
                        <p className="text-sm font-medium text-indigo-600 truncate">{task.title}</p>
                        <div className="ml-2 flex-shrink-0 flex">
                          <Badge type="priority" value={task.priority} />
                        </div>
                      </div>
                      <div className="mt-2 sm:flex sm:justify-between">
                        <div className="sm:flex">
                          <p className="flex items-center text-sm text-gray-500">
                            {task.taskCode} - {task.projectName}
                          </p>
                        </div>
                          <DueDateBadge dueDate={task.dueDate} />
                      </div>
                    </div>
                  </Link>
                </li>
              ))
            )}
          </ul>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
