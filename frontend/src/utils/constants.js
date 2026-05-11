export const STATUS_COLORS = {
  todo: 'bg-gray-100 text-gray-700',
  in_progress: 'bg-blue-100 text-blue-700',
  review: 'bg-yellow-100 text-yellow-700',
  done: 'bg-green-100 text-green-700',
  blocked: 'bg-red-100 text-red-700',
  planning: 'bg-purple-100 text-purple-700',
  completed: 'bg-green-100 text-green-700',
  on_hold: 'bg-orange-100 text-orange-700',
  cancelled: 'bg-gray-100 text-gray-500',
};

export const PRIORITY_COLORS = {
  low: 'bg-gray-100 text-gray-600',
  medium: 'bg-yellow-100 text-yellow-700',
  high: 'bg-orange-100 text-orange-700',
  critical: 'bg-red-100 text-red-700',
};

export const SENIORITY_LEVELS = [
  { value: 'junior', label: 'Junior' },
  { value: 'mid', label: 'Mid-Level' },
  { value: 'senior', label: 'Senior' }
];

export const AVAILABILITY_STATUSES = [
  { value: 'available', label: 'Disponible' },
  { value: 'busy', label: 'Ocupado' },
  { value: 'vacation', label: 'Vacaciones' }
];

export const PROJECT_STATUSES = [
  { value: 'planning', label: 'Planificación' },
  { value: 'in_progress', label: 'En Progreso' },
  { value: 'on_hold', label: 'En Espera' },
  { value: 'completed', label: 'Completado' },
  { value: 'cancelled', label: 'Cancelado' }
];

export const TASK_STATUSES = [
  { value: 'todo', label: 'Por Hacer' },
  { value: 'in_progress', label: 'En Progreso' },
  { value: 'review', label: 'Revisión' },
  { value: 'done', label: 'Hecho' },
  { value: 'blocked', label: 'Bloqueado' }
];

export const TASK_PRIORITIES = [
  { value: 'low', label: 'Baja' },
  { value: 'medium', label: 'Media' },
  { value: 'high', label: 'Alta' },
  { value: 'critical', label: 'Crítica' }
];
