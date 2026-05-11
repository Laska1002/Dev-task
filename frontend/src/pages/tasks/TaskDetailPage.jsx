import React, { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getTask, getTaskComments, addTaskComment } from '../../api/tasksApi';
import Spinner from '../../components/ui/Spinner';
import ErrorMessage from '../../components/ui/ErrorMessage';
import Badge from '../../components/ui/Badge';
import Button from '../../components/ui/Button';
import { formatDateTime, formatDate } from '../../utils/formatters';

const TaskDetailPage = () => {
  const { id } = useParams();
  const [commentText, setCommentText] = useState('');

  const { data: task, isLoading, isError, error } = useQuery({
    queryKey: ['task', id],
    queryFn: () => getTask(id)
  });

  const { data: comments, refetch: refetchComments } = useQuery({
    queryKey: ['taskComments', id],
    queryFn: () => getTaskComments(id)
  });

  const handleAddComment = async (e) => {
    e.preventDefault();
    if (!commentText.trim()) return;
    try {
      await addTaskComment(id, { comment: commentText });
      setCommentText('');
      refetchComments();
    } catch (err) {
      alert('Failed to add comment');
    }
  };

  if (isLoading) return <Spinner />;
  if (isError) return <ErrorMessage message={error.message} />;
  if (!task) return <ErrorMessage message="Task not found" />;

  return (
    <div className="space-y-6 max-w-4xl mx-auto">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <Link to="/tasks" className="text-sm font-medium text-indigo-600 hover:text-indigo-500">&larr; Back</Link>
          <h1 className="text-2xl font-bold text-gray-900">{task.title}</h1>
          <span className="bg-gray-800 text-white text-xs font-bold px-3 py-1 rounded shadow-sm">{task.taskCode}</span>
        </div>
        <Link to={`/tasks/${task.id}/edit`} className="bg-white border border-gray-300 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-50 text-sm font-medium shadow-sm">
          Edit Task
        </Link>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <div className="px-4 py-5 sm:px-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900">Task Details</h3>
          <p className="mt-1 max-w-2xl text-sm text-gray-500 whitespace-pre-wrap">{task.description}</p>
        </div>
        <div className="border-t border-gray-200 px-4 py-5 sm:p-0">
          <dl className="sm:divide-y sm:divide-gray-200">
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Project</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                <Link to={`/projects/${task.projectId}`} className="text-indigo-600 hover:underline">{task.projectName}</Link>
              </dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Assigned To</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{task.assignedToName || 'Unassigned'}</dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Status & Priority</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2 flex space-x-3">
                <Badge type="status" value={task.status} />
                <Badge type="priority" value={task.priority} />
              </dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Estimated Hours</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{task.estimatedHours || 0} hrs</dd>
            </div>
            <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Due Date</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{formatDate(task.dueDate)}</dd>
            </div>
          </dl>
        </div>
      </div>

      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:px-6 border-b border-gray-200">
          <h3 className="text-lg leading-6 font-medium text-gray-900">Comments</h3>
        </div>
        <div className="px-4 py-6 sm:px-6">
          <ul className="space-y-4 mb-6">
            {comments?.length === 0 ? (
              <p className="text-sm text-gray-500">No comments yet.</p>
            ) : (
              comments?.map((comment) => (
                <li key={comment.id} className="bg-gray-50 p-4 rounded-lg">
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-sm font-medium text-gray-900">{comment.username}</span>
                    <span className="text-xs text-gray-500">{formatDateTime(comment.createdAt)}</span>
                  </div>
                  <p className="text-sm text-gray-700 whitespace-pre-wrap">{comment.comment}</p>
                </li>
              ))
            )}
          </ul>
          <form onSubmit={handleAddComment}>
            <textarea
              rows="3"
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm border p-2"
              placeholder="Add a comment..."
              value={commentText}
              onChange={(e) => setCommentText(e.target.value)}
            />
            <div className="mt-3 flex justify-end">
              <Button type="submit" disabled={!commentText.trim()}>Post Comment</Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default TaskDetailPage;
