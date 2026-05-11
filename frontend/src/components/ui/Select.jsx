import React, { forwardRef } from 'react';

const Select = forwardRef(({ label, options, error, className = '', ...props }, ref) => {
  return (
    <div className={`flex flex-col mb-4 ${className}`}>
      {label && <label className="mb-1 text-sm font-medium text-gray-700">{label}</label>}
      <select
        ref={ref}
        className={`px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 ${
          error ? 'border-red-500 focus:border-red-500 focus:ring-red-500' : 'border-gray-300 focus:border-indigo-500'
        } bg-white`}
        {...props}
      >
        <option value="">Select an option</option>
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && <span className="mt-1 text-sm text-red-500">{error.message}</span>}
    </div>
  );
});

Select.displayName = 'Select';
export default Select;
