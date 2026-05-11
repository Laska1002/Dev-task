import React from 'react';
import Button from './Button';

const Pagination = ({ page, size, total, onPageChange }) => {
  const totalPages = Math.ceil(total / size);

  if (totalPages <= 1) return null;

  return (
    <div className="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 mt-4">
      <div className="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
        <div>
          <p className="text-sm text-gray-700">
            Showing <span className="font-medium">{(page - 1) * size + 1}</span> to{' '}
            <span className="font-medium">{Math.min(page * size, total)}</span> of{' '}
            <span className="font-medium">{total}</span> results
          </p>
        </div>
        <div>
          <nav className="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
            <Button
              variant="secondary"
              className="rounded-l-md rounded-r-none"
              onClick={() => onPageChange(page - 1)}
              disabled={page === 1}
            >
              Previous
            </Button>
            <span className="relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 focus:z-20 focus:outline-offset-0">
              Page {page} of {totalPages}
            </span>
            <Button
              variant="secondary"
              className="rounded-r-md rounded-l-none"
              onClick={() => onPageChange(page + 1)}
              disabled={page === totalPages}
            >
              Next
            </Button>
          </nav>
        </div>
      </div>
    </div>
  );
};

export default Pagination;
