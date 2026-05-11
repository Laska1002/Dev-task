import React from 'react';
import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getProject } from '../../api/projectsApi';
import Spinner from '../../components/ui/Spinner';
import ErrorMessage from '../../components/ui/ErrorMessage';
import Badge from '../../components/ui/Badge';
import { formatDate } from '../../utils/formatters';

const ProjectDetailPage = () => {
  const { id } = useParams();

  const { data: project, isLoading, isError, error } = useQuery({
    queryKey: ['project', id],
    queryFn: () => getProject(id)
  });

  if (isLoading) return <Spinner />;
  if (isError) return <ErrorMessage message={error.message} />;
  if (!project) return <ErrorMessage message="Project not found" />;

  return (
    <div className="space-y-6 max-w-4xl mx-auto">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <Link to="/projects" className="text-sm font-medium text-indigo-600 hover:text-indigo-500">&larr; Back</Link>
          <h1 className="text-2xl font-bold text-gray-900">{project.name}</h1>
          <span className="bg-gray-800 text-white text-xs font-bold px-3 py-1 rounded shadow-sm">{project.projectCode}</span>
        </div>
        <Link to={`/projects/${project.id}/edit`} className="bg-white border border-gray-300 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-50 text-sm font-medium shadow-sm">
          Edit Project
        </Link>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <div className="px-4 py-5 sm:px-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900">Project Information</h3>
          <p className="mt-1 max-w-2xl text-sm text-gray-500">{project.description}</p>
        </div>
        <div className="border-t border-gray-200 px-4 py-5 sm:p-0">
          <dl className="sm:divide-y sm:divide-gray-200">
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Status</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2"><Badge type="status" value={project.status} /></dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Project Type</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{project.projectTypeName}</dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Timeline</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{formatDate(project.startDate)} — {formatDate(project.dueDate)}</dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Task Summary</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                <div className="flex space-x-4">
                  <span className="text-gray-500">Todo: <strong className="text-gray-900">{project.taskCounts?.todo || 0}</strong></span>
                  <span className="text-blue-500">In Progress: <strong className="text-gray-900">{project.taskCounts?.inProgress || 0}</strong></span>
                  <span className="text-green-500">Done: <strong className="text-gray-900">{project.taskCounts?.done || 0}</strong></span>
                </div>
              </dd>
            </div>
          </dl>
        </div>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <div className="px-4 py-5 sm:px-6 border-b border-gray-200">
          <h3 className="text-lg leading-6 font-medium text-gray-900">Assigned Developers</h3>
        </div>
        <ul className="divide-y divide-gray-200">
          {project.developers?.length === 0 ? (
            <li className="px-6 py-4 text-sm text-gray-500">No developers assigned yet.</li>
          ) : (
            project.developers?.map((dev) => (
              <li key={dev.developerId} className="px-6 py-4 flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-900">{dev.fullName}</p>
                  <p className="text-sm text-gray-500 capitalize">{dev.seniorityLevel} - {dev.specialty}</p>
                </div>
                <div className="text-sm text-gray-500 bg-gray-100 px-2 py-1 rounded">
                  {dev.roleInProject}
                </div>
              </li>
            ))
          )}
        </ul>
      </div>
    </div>
  );
};

export default ProjectDetailPage;
