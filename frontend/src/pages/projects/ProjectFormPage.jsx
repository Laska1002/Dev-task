import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { createProject, getProject, updateProject, assignDevelopers } from '../../api/projectsApi';
import { getProjectTypes } from '../../api/projectTypesApi';
import { getTechnologies } from '../../api/technologiesApi';
import { PROJECT_STATUSES } from '../../utils/constants';
import Input from '../../components/ui/Input';
import Select from '../../components/ui/Select';
import Button from '../../components/ui/Button';
import Spinner from '../../components/ui/Spinner';
import DeveloperSelector from '../../components/DeveloperSelector';

const projectSchema = z.object({
  name: z.string().min(3, 'El nombre es requerido'),
  description: z.string().optional(),
  startDate: z.string().min(1, 'La fecha de inicio es requerida'),
  dueDate: z.string().min(1, 'La fecha de fin es requerida'),
  status: z.string().min(1, 'El estado es requerido'),
  projectTypeId: z.coerce.number().min(1, 'El tipo de proyecto es requerido'),
  technologyId: z.coerce.number().min(1, 'El lenguaje/tecnología es requerido'),
  estimatedHours: z.coerce.number().min(1, 'Las horas deben ser mayor a 0').optional(),
});

const ProjectFormPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);
  const [loading, setLoading] = useState(isEditMode);
  const [projectCode, setProjectCode] = useState('');
  const [selectedDevs, setSelectedDevs] = useState([]);
  
  const { register, handleSubmit, formState: { errors, isSubmitting }, setValue, watch, setError } = useForm({
    resolver: zodResolver(projectSchema),
    defaultValues: { status: 'planning' }
  });

  const selectedProjectTypeId = watch('projectTypeId');
  const selectedTechnologyId = watch('technologyId');

  const { data: projectTypes } = useQuery({
    queryKey: ['projectTypes'],
    queryFn: getProjectTypes
  });

  const { data: technologies, isLoading: loadingTechs } = useQuery({
    queryKey: ['technologies', selectedProjectTypeId],
    queryFn: () => getTechnologies(selectedProjectTypeId),
    enabled: !!selectedProjectTypeId
  });

  useEffect(() => {
    if (isEditMode) {
      getProject(id)
        .then(data => {
          setValue('name', data.name);
          setValue('description', data.description || '');
          setValue('startDate', data.startDate ? data.startDate.split('T')[0] : '');
          setValue('dueDate', data.dueDate ? data.dueDate.split('T')[0] : '');
          setValue('status', data.status);
          setValue('projectTypeId', data.projectTypeId);
          
          // data.technologyId from backend DTO is mapped to TechnologyId
          setValue('technologyId', data.technologyId);
          setValue('estimatedHours', data.estimatedHours);
          setProjectCode(data.projectCode);
          
          if (data.developers) {
            setSelectedDevs(data.developers.map(d => d.developerId));
          }
          setLoading(false);
        })
        .catch(err => {
          console.error(err);
          navigate('/projects');
        });
    }
  }, [id, isEditMode, setValue, navigate]);

  const onSubmit = async (data) => {
    try {
      const payload = {
        ...data
      };
      
      let projectId = id;

      if (isEditMode) {
        await updateProject(id, payload);
      } else {
        const created = await createProject(payload);
        projectId = created.id;
      }

      if (selectedDevs.length > 0) {
        await assignDevelopers(projectId, { developerIds: selectedDevs, roleInProject: 'Developer' });
      }

      navigate('/projects');
    } catch (err) {
      const backendErrors = err.response?.data?.errors;
      if (backendErrors) {
        Object.entries(backendErrors).forEach(([field, msg]) => {
          setError(field.toLowerCase(), { message: Array.isArray(msg) ? msg[0] : msg });
        });
      } else {
        alert(err.response?.data?.message || 'Error al guardar el proyecto');
      }
    }
  };

  const projectTypeOptions = projectTypes?.map(pt => ({ value: pt.id, label: pt.name })) || [];
  const techOptions = technologies?.map(t => ({ value: t.id, label: t.name })) || [];

  if (loading) return <Spinner />;

  return (
    <div className="max-w-3xl mx-auto bg-white p-6 rounded-lg shadow">
      <div className="flex items-center justify-between mb-6 border-b pb-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{isEditMode ? 'Editar Proyecto' : 'Nuevo Proyecto'}</h1>
          {isEditMode && (
            <span className="mt-2 inline-block bg-gray-800 text-white px-3 py-1 rounded text-sm font-mono shadow">
              {projectCode}
            </span>
          )}
        </div>
        <Link to="/projects" className="text-sm font-medium text-gray-500 hover:text-gray-700">Cancelar</Link>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Input label="Nombre del Proyecto" {...register('name')} error={errors.name} className="sm:col-span-2" />
          
          <Select label="Tipo de Proyecto" options={projectTypeOptions} {...register('projectTypeId')} error={errors.projectTypeId} />
          
          {loadingTechs ? (
            <div className="flex items-center"><Spinner /></div>
          ) : (
            <Select label="Lenguaje / Tecnología" options={techOptions} {...register('technologyId')} error={errors.technologyId} disabled={!selectedProjectTypeId} />
          )}

          <Select label="Estado" options={PROJECT_STATUSES} {...register('status')} error={errors.status} />

          <Input type="date" label="Fecha de Inicio" {...register('startDate')} error={errors.startDate} />
          <Input type="date" label="Fecha de Fin" {...register('dueDate')} error={errors.dueDate} />
          
          <Input type="number" step="0.5" label="Horas Estimadas" {...register('estimatedHours')} error={errors.estimatedHours} className="sm:col-span-2" />
          
          <div className="sm:col-span-2">
            <label className="mb-1 text-sm font-medium text-gray-700 block">Descripción</label>
            <textarea
              {...register('description')}
              rows={3}
              className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 ${errors.description ? 'border-red-500' : 'border-gray-300'}`}
            />
          </div>
        </div>

        {/* DeveloperSelector cascaded from projectTypeId */}
        <div className="mt-6 pt-4 border-t">
          {selectedProjectTypeId ? (
             <DeveloperSelector selectedIds={selectedDevs} onChange={setSelectedDevs} projectTypeId={selectedProjectTypeId} />
          ) : (
            <p className="text-sm text-gray-500 italic">Seleccione un Tipo de Proyecto para asignar desarrolladores calificados.</p>
          )}
        </div>

        <div className="pt-4 flex justify-end space-x-3">
          <Button type="button" variant="secondary" onClick={() => navigate('/projects')}>Cancelar</Button>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Guardando...' : 'Guardar Proyecto'}
          </Button>
        </div>
      </form>
    </div>
  );
};

export default ProjectFormPage;
