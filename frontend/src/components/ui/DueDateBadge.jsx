import React from 'react';
import { formatDate } from '../../utils/formatters';

const getDueStatus = (dueDate) => {
  if (!dueDate) return { status: 'none', label: 'Sin Fecha', color: 'text-gray-500', bg: 'bg-gray-100', icon: null };

  const today = new Date();
  today.setHours(0, 0, 0, 0);
  
  const due = new Date(dueDate);
  due.setHours(0, 0, 0, 0);

  const diffTime = due - today;
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

  if (diffDays < 0) {
    return {
      status: 'overdue',
      label: `Vencido (hace ${Math.abs(diffDays)} días)`,
      color: 'text-red-700',
      bg: 'bg-red-100',
      icon: (
        <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      )
    };
  } else if (diffDays <= 3) {
    return {
      status: 'warning',
      label: diffDays === 0 ? 'Vence Hoy' : `Vence en ${diffDays} días`,
      color: 'text-yellow-700',
      bg: 'bg-yellow-100',
      icon: (
        <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
      )
    };
  } else {
    return {
      status: 'ontime',
      label: 'A tiempo',
      color: 'text-green-700',
      bg: 'bg-green-100',
      icon: (
        <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      )
    };
  }
};

const DueDateBadge = ({ dueDate }) => {
  if (!dueDate) {
    return <span className="text-sm text-gray-500">-</span>;
  }

  const { label, color, bg, icon } = getDueStatus(dueDate);

  return (
    <div className="flex flex-col">
      <span className="text-sm text-gray-900">{formatDate(dueDate)}</span>
      <span className={`inline-flex items-center mt-1 px-2.5 py-0.5 rounded-full text-xs font-medium ${bg} ${color} w-max`}>
        {icon}
        {label}
      </span>
    </div>
  );
};

export default DueDateBadge;
