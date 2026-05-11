import { useState } from 'react';

export const usePagination = (initialPage = 1, initialSize = 10) => {
  const [page, setPage] = useState(initialPage);
  const [size, setSize] = useState(initialSize);

  const handlePageChange = (newPage) => {
    setPage(newPage);
  };

  const handleSizeChange = (newSize) => {
    setSize(newSize);
    setPage(1); // Reset to first page when size changes
  };

  return { page, size, handlePageChange, handleSizeChange };
};
