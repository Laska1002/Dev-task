import React, { useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getDevelopers } from '../api/developersApi';
import Spinner from './ui/Spinner';
import ErrorMessage from './ui/ErrorMessage';

const DeveloperSelector = ({ selectedIds, onChange, error, technologyId, projectTypeId }) => {
  const { data, isLoading, isError, error: fetchError } = useQuery({
    queryKey: ['developers', { status: 'available', technologyId, projectTypeId }],
    queryFn: () => getDevelopers({ size: 100, status: 'available', technologyId, projectTypeId }),
    enabled: !!technologyId || !!projectTypeId
  });

  const handleCheckboxChange = (devId) => {
    if (selectedIds.includes(devId)) {
      onChange(selectedIds.filter(id => id !== devId));
    } else {
      onChange([...selectedIds, devId]);
    }
  };

  if (isLoading) return <Spinner />;
  if (isError) return <ErrorMessage message={fetchError?.message || 'Error loading developers'} />;

  return (
    <div className="mb-4">
      <label className="mb-1 text-sm font-medium text-gray-700 block">Asignar Desarrolladores</label>
      <div className={`border rounded-md p-3 max-h-48 overflow-y-auto bg-white ${error ? 'border-red-500' : 'border-gray-300'}`}>
        {data?.items?.length === 0 ? (
          <p className="text-sm text-gray-500">No se encontraron desarrolladores disponibles con este criterio.</p>
        ) : (
          <div className="space-y-2">
            {data?.items?.map(dev => (
              <label key={dev.id} className="flex items-center space-x-3 cursor-pointer">
                <input
                  type="checkbox"
                  className="form-checkbox h-4 w-4 text-indigo-600"
                  checked={selectedIds.includes(dev.id)}
                  onChange={() => handleCheckboxChange(dev.id)}
                />
                <span className="text-sm text-gray-700">
                  {dev.fullName} <span className="text-xs text-gray-500">({dev.seniorityLevel} - {dev.projectTypeName || 'Sin especialidad'})</span>
                </span>
              </label>
            ))}
          </div>
        )}
      </div>
      {error && <span className="mt-1 text-sm text-red-500">{error.message}</span>}
    </div>
  );
};

export default DeveloperSelector;
