import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { createDeveloper, getDeveloper, updateDeveloper } from '../../api/developersApi';
import { getProjectTypes } from '../../api/projectTypesApi';
import { getTechnologies } from '../../api/technologiesApi';
import { SENIORITY_LEVELS, AVAILABILITY_STATUSES } from '../../utils/constants';
import Input from '../../components/ui/Input';
import Select from '../../components/ui/Select';
import Button from '../../components/ui/Button';
import Spinner from '../../components/ui/Spinner';

const developerSchema = z.object({
  fullName: z.string().min(3, 'El nombre debe tener al menos 3 caracteres'),
  cedula: z.string().length(10, 'La cédula debe tener exactamente 10 dígitos').regex(/^\d+$/, 'Solo números permitidos'),
  projectTypeId: z.coerce.number().min(1, 'El tipo de desarrollador es requerido'),
  technologyIds: z.array(z.number()).min(1, 'Debe seleccionar al menos una habilidad'),
  seniorityLevel: z.string().min(1, 'El nivel es requerido'),
  availabilityStatus: z.string().min(1, 'La disponibilidad es requerida'),
  userId: z.string().optional().nullable(),
});

const DeveloperFormPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);
  const [loading, setLoading] = useState(isEditMode);

  // Queries para selects
  const { data: projectTypes } = useQuery({
    queryKey: ['projectTypes'],
    queryFn: getProjectTypes
  });

  const { register, handleSubmit, formState: { errors, isSubmitting }, setValue, watch, setError } = useForm({
    resolver: zodResolver(developerSchema),
    defaultValues: {
      seniorityLevel: 'junior',
      availabilityStatus: 'available',
      technologyIds: []
    }
  });

  const selectedProjectTypeId = watch('projectTypeId');
  const selectedTechs = watch('technologyIds') || [];

  const { data: technologies, isLoading: isLoadingTechs } = useQuery({
    queryKey: ['technologies', selectedProjectTypeId],
    queryFn: () => getTechnologies(selectedProjectTypeId),
    enabled: !!selectedProjectTypeId
  });

  useEffect(() => {
    if (isEditMode) {
      getDeveloper(id)
        .then(data => {
          setValue('fullName', data.fullName);
          setValue('cedula', data.cedula);
          setValue('projectTypeId', data.projectTypeId);
          
          getTechnologies(data.projectTypeId).then(techs => {
            const techIds = techs.filter(t => data.technologies?.includes(t.name)).map(t => t.id);
            setValue('technologyIds', techIds);
            setLoading(false);
          });
          
          setValue('seniorityLevel', data.seniorityLevel);
          setValue('availabilityStatus', data.availabilityStatus);
          setValue('userId', data.userId?.toString() || '');
        })
        .catch(err => {
          console.error(err);
          navigate('/developers');
        });
    }
  }, [id, isEditMode, setValue, navigate]);

  // Manejar checkboxes manualmente
  const handleTechChange = (techId) => {
    const newTechs = selectedTechs.includes(techId)
      ? selectedTechs.filter(t => t !== techId)
      : [...selectedTechs, techId];
    setValue('technologyIds', newTechs, { shouldValidate: true });
  };

  const onSubmit = async (data) => {
    try {
      const payload = {
        ...data,
        userId: data.userId ? parseInt(data.userId) : null
      };

      if (isEditMode) {
        await updateDeveloper(id, payload);
      } else {
        await createDeveloper(payload);
      }
      navigate('/developers');
    } catch (err) {
      const backendErrors = err.response?.data?.errors;
      if (backendErrors) {
        Object.entries(backendErrors).forEach(([field, msg]) => {
          setError(field.toLowerCase(), { message: Array.isArray(msg) ? msg[0] : msg });
        });
      } else if (err.response?.data?.message) {
        if (err.response.data.message.toLowerCase().includes('cédula') || err.response.data.message.toLowerCase().includes('cedula')) {
          setError('cedula', { message: err.response.data.message });
        } else {
          alert(err.response.data.message);
        }
      }
    }
  };

  if (loading) return <Spinner />;

  const typeOptions = projectTypes?.map(pt => ({ value: pt.id, label: pt.name })) || [];

  return (
    <div className="max-w-2xl mx-auto bg-white p-6 rounded-lg shadow">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{isEditMode ? 'Editar Desarrollador' : 'Nuevo Desarrollador'}</h1>
        <Link to="/developers" className="text-sm font-medium text-gray-500 hover:text-gray-700">Cancelar</Link>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Input label="Nombre Completo" {...register('fullName')} error={errors.fullName} />
        
        <Input 
          label="Cédula" 
          {...register('cedula')} 
          error={errors.cedula} 
          readOnly={isEditMode} 
          placeholder="10 dígitos"
          maxLength={10}
        />
        
        <Select label="Tipo de Desarrollador" options={typeOptions} {...register('projectTypeId')} error={errors.projectTypeId} />
        
        {/* Habilidades Multi-Select */}
        {selectedProjectTypeId && (
          <div className="mt-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">Habilidades (Tecnologías)</label>
            {isLoadingTechs ? <Spinner /> : (
              <div className={`grid grid-cols-2 gap-2 p-3 border rounded-md ${errors.technologyIds ? 'border-red-500' : 'border-gray-300'}`}>
                {technologies?.map(tech => (
                  <label key={tech.id} className="flex items-center space-x-2">
                    <input 
                      type="checkbox"
                      className="form-checkbox text-indigo-600 rounded"
                      checked={selectedTechs.includes(tech.id)}
                      onChange={() => handleTechChange(tech.id)}
                    />
                    <span className="text-sm">{tech.name}</span>
                  </label>
                ))}
              </div>
            )}
            {errors.technologyIds && <p className="text-red-500 text-xs mt-1">{errors.technologyIds.message}</p>}
          </div>
        )}

        <Select label="Nivel de Experiencia" options={SENIORITY_LEVELS} {...register('seniorityLevel')} error={errors.seniorityLevel} />
        <Select label="Estado de Disponibilidad" options={AVAILABILITY_STATUSES} {...register('availabilityStatus')} error={errors.availabilityStatus} />
        
        <div className="pt-4 flex justify-end space-x-3">
          <Button type="button" variant="secondary" onClick={() => navigate('/developers')}>Cancelar</Button>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Guardando...' : 'Guardar Desarrollador'}
          </Button>
        </div>
      </form>
    </div>
  );
};

export default DeveloperFormPage;
