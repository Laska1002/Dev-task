import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { createTask, getTask, updateTask } from '../../api/tasksApi';
import { getProjects, getProjectDevelopers } from '../../api/projectsApi';
import { TASK_STATUSES, TASK_PRIORITIES } from '../../utils/constants';
import Input from '../../components/ui/Input';
import Select from '../../components/ui/Select';
import Button from '../../components/ui/Button';
import Spinner from '../../components/ui/Spinner';

const taskSchema = z.object({
  title: z.string().min(3, 'El título es requerido'),
  description: z.string().optional(),
  projectId: z.coerce.number().min(1, 'El proyecto es requerido'),
  assignedTo: z.coerce.number().optional().nullable(),
  priority: z.string().min(1, 'La prioridad es requerida'),
  status: z.string().min(1, 'El estado es requerido'),
  estimatedHours: z.coerce.number().min(0).optional(),
  dueDate: z.string().optional().nullable(),
});

const TaskFormPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);
  const [loading, setLoading] = useState(isEditMode);
  const [taskCode, setTaskCode] = useState('');

  const { register, handleSubmit, formState: { errors, isSubmitting }, setValue, watch, setError } = useForm({
    resolver: zodResolver(taskSchema),
    defaultValues: { status: 'todo', priority: 'medium' }
  });

  const selectedProjectId = watch('projectId');

  // R2: Dependent dropdowns
  const { data: projectsData } = useQuery({
    queryKey: ['projects-all'],
    queryFn: () => getProjects({ size: 1000 })
  });

  const { data: projectDevs } = useQuery({
    queryKey: ['project-devs', selectedProjectId],
    queryFn: () => getProjectDevelopers(selectedProjectId),
    enabled: !!selectedProjectId
  });

  // R2: Reset assigned_to when project changes
  useEffect(() => {
    // Only reset if it's not the initial load of edit mode
    if (!loading) {
      setValue('assignedTo', '');
    }
  }, [selectedProjectId, setValue]);

  useEffect(() => {
    if (isEditMode) {
      getTask(id)
        .then(data => {
          setValue('title', data.title);
          setValue('description', data.description || '');
          setValue('projectId', data.projectId);
          // Small delay to let projectDevs fetch before setting assignedTo to avoid overriding
          setTimeout(() => {
            setValue('assignedTo', data.assignedTo || '');
          }, 100);
          setValue('priority', data.priority);
          setValue('status', data.status);
          setValue('estimatedHours', data.estimatedHours || 0);
          setValue('dueDate', data.dueDate ? data.dueDate.split('T')[0] : '');
          
          setTaskCode(data.taskCode); // R3
          setLoading(false);
        })
        .catch(err => {
          console.error(err);
          navigate('/tasks');
        });
    }
  }, [id, isEditMode, setValue, navigate]);

  const onSubmit = async (data) => {
    try {
      const payload = {
        ...data,
        assignedTo: data.assignedTo ? parseInt(data.assignedTo) : null,
        dueDate: data.dueDate || null,
      };

      if (isEditMode) {
        // R3: Never include task_code in payload
        await updateTask(id, payload);
      } else {
        await createTask(payload);
      }
      navigate('/tasks');
    } catch (err) {
      const backendErrors = err.response?.data?.errors;
      if (backendErrors) {
        Object.entries(backendErrors).forEach(([field, msg]) => {
          setError(field.toLowerCase(), { message: Array.isArray(msg) ? msg[0] : msg });
        });
      } else {
        alert(err.response?.data?.message || 'Error al guardar la tarea');
      }
    }
  };

  const projectOptions = projectsData?.items?.map(p => ({ value: p.id, label: p.name })) || [];
  const assignedOptions = projectDevs?.map(dev => ({ value: dev.developerId, label: dev.fullName })) || [];

  if (loading) return <Spinner />;

  return (
    <div className="max-w-3xl mx-auto bg-white p-6 rounded-lg shadow">
      <div className="flex items-center justify-between mb-6 border-b pb-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{isEditMode ? 'Editar Tarea' : 'Nueva Tarea'}</h1>
          {/* R3: Read-only task_code displayed as a prominent badge */}
          {isEditMode && (
            <span className="mt-2 inline-block bg-gray-100 border border-gray-300 text-gray-800 px-3 py-1 rounded text-sm font-mono shadow-sm">
              {taskCode}
            </span>
          )}
        </div>
        <Link to="/tasks" className="text-sm font-medium text-gray-500 hover:text-gray-700">Cancelar</Link>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Input label="Título" {...register('title')} error={errors.title} />
        
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {/* R2: Cascading selects */}
          <Select label="Proyecto" options={projectOptions} {...register('projectId')} error={errors.projectId} disabled={isEditMode} />
          
          <Select 
            label="Asignado a" 
            options={assignedOptions} 
            {...register('assignedTo')} 
            error={errors.assignedTo} 
            disabled={!selectedProjectId}
          />
          
          <Select label="Estado" options={TASK_STATUSES} {...register('status')} error={errors.status} />
          <Select label="Prioridad" options={TASK_PRIORITIES} {...register('priority')} error={errors.priority} />
          
          <Input type="number" step="0.5" label="Horas Estimadas" {...register('estimatedHours')} error={errors.estimatedHours} />
          <Input type="date" label="Fecha Límite" {...register('dueDate')} error={errors.dueDate} />
        </div>

        <div>
          <label className="mb-1 text-sm font-medium text-gray-700 block">Descripción</label>
          <textarea
            {...register('description')}
            rows={4}
            className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 ${errors.description ? 'border-red-500' : 'border-gray-300'}`}
          />
        </div>

        <div className="pt-4 flex justify-end space-x-3">
          <Button type="button" variant="secondary" onClick={() => navigate('/tasks')}>Cancelar</Button>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Guardando...' : 'Guardar Tarea'}
          </Button>
        </div>
      </form>
    </div>
  );
};

export default TaskFormPage;
