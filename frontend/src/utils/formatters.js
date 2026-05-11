export const formatDate = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('es-EC', {
    year: 'numeric',
    month: 'short',
    day: '2-digit'
  }).format(date);
};

export const formatDateTime = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('es-EC', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

export const formatStatus = (status) => {
  if (!status) return '';
  return status.split('_').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
};
