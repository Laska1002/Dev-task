import React, { forwardRef } from 'react';

const Input = forwardRef(({ label, error, className = '', readOnly, ...props }, ref) => {
  return (
    <div className={`flex flex-col mb-4 ${className}`}>
      {label && <label className="mb-1 text-sm font-medium text-gray-700">{label}</label>}
      <input
        ref={ref}
        readOnly={readOnly}
        className={`px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 ${
          error ? 'border-red-500 focus:border-red-500 focus:ring-red-500' : 'border-gray-300 focus:border-indigo-500'
        } ${readOnly ? 'bg-gray-100 text-gray-600 cursor-not-allowed' : 'bg-white'}`}
        {...props}
      />
      {error && <span className="mt-1 text-sm text-red-500">{error.message}</span>}
    </div>
  );
});

Input.displayName = 'Input';
export default Input;
