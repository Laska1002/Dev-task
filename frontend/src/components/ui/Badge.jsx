import React from 'react';
import { STATUS_COLORS, PRIORITY_COLORS } from '../../utils/constants';
import { formatStatus } from '../../utils/formatters';

const Badge = ({ type, value }) => {
  const colors = type === 'status' ? STATUS_COLORS : PRIORITY_COLORS;
  const colorClass = colors[value] || 'bg-gray-100 text-gray-800';
  const label = formatStatus(value);

  return (
    <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${colorClass}`}>
      {label}
    </span>
  );
};

export default Badge;
