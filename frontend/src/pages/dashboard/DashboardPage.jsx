import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { getDashboardStats } from '../../api/dashboardApi';
import { getProjects } from '../../api/projectsApi';
import { getTasks } from '../../api/tasksApi';
import { Link } from 'react-router-dom';
import Spinner from '../../components/ui/Spinner';
import Badge from '../../components/ui/Badge';
import DueDateBadge from '../../components/ui/DueDateBadge';

// ── Paleta de colores por estado ──────────────────────────────────────────────
const STATUS_COLORS = {
  todo:        { bg: '#EFF6FF', bar: '#3B82F6', text: '#1D4ED8', label: 'Por hacer' },
  in_progress: { bg: '#FFFBEB', bar: '#F59E0B', text: '#B45309', label: 'En progreso' },
  review:      { bg: '#F5F3FF', bar: '#8B5CF6', text: '#6D28D9', label: 'En revisión' },
  done:        { bg: '#F0FDF4', bar: '#22C55E', text: '#15803D', label: 'Completadas' },
  blocked:     { bg: '#FFF1F2', bar: '#F43F5E', text: '#BE123C', label: 'Bloqueadas' },
};

const PRIORITY_COLORS = {
  low:      { bar: '#6EE7B7', text: '#065F46', label: 'Baja' },
  medium:   { bar: '#93C5FD', text: '#1E40AF', label: 'Media' },
  high:     { bar: '#FCA5A5', text: '#991B1B', label: 'Alta' },
  critical: { bar: '#F87171', text: '#7F1D1D', label: 'Crítica' },
};

const AVAILABILITY_COLORS = {
  available: { bar: '#34D399', label: 'Disponibles' },
  busy:      { bar: '#FBBF24', label: 'Ocupados' },
  vacation:  { bar: '#60A5FA', label: 'Vacaciones' },
};

const SENIORITY_COLORS = {
  junior: { bar: '#A78BFA', label: 'Junior' },
  mid:    { bar: '#34D399', label: 'Mid' },
  senior: { bar: '#F59E0B', label: 'Senior' },
};

// ── Componente: Tarjeta de estadística ─────────────────────────────────────
const StatCard = ({ title, value, subtitle, icon, color = '#6366F1' }) => (
  <div style={{
    background: '#fff',
    borderRadius: 16,
    padding: '24px',
    boxShadow: '0 1px 3px rgba(0,0,0,0.08), 0 4px 16px rgba(0,0,0,0.04)',
    display: 'flex',
    alignItems: 'center',
    gap: 20,
    border: '1px solid #F3F4F6',
  }}>
    <div style={{
      width: 56, height: 56, borderRadius: 14,
      background: `${color}18`,
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontSize: 26, flexShrink: 0,
    }}>
      {icon}
    </div>
    <div>
      <p style={{ fontSize: 13, color: '#9CA3AF', fontWeight: 500, margin: 0 }}>{title}</p>
      <p style={{ fontSize: 32, fontWeight: 700, color: '#111827', margin: '2px 0 0' }}>{value}</p>
      {subtitle && <p style={{ fontSize: 12, color: '#6B7280', margin: '2px 0 0' }}>{subtitle}</p>}
    </div>
  </div>
);

// ── Componente: Barra de progreso horizontal ───────────────────────────────
const ProgressBar = ({ label, value, total, color, textColor }) => {
  const pct = total > 0 ? Math.round((value / total) * 100) : 0;
  return (
    <div style={{ marginBottom: 12 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
        <span style={{ fontSize: 13, color: textColor || '#374151', fontWeight: 500 }}>{label}</span>
        <span style={{ fontSize: 13, color: '#9CA3AF' }}>{value} ({pct}%)</span>
      </div>
      <div style={{ height: 8, background: '#F3F4F6', borderRadius: 99, overflow: 'hidden' }}>
        <div style={{
          height: '100%', width: `${pct}%`,
          background: color, borderRadius: 99,
          transition: 'width 0.6s ease',
        }} />
      </div>
    </div>
  );
};

// ── Componente: Tarjeta de sección con barras ─────────────────────────────
const BarSection = ({ title, icon, children }) => (
  <div style={{
    background: '#fff', borderRadius: 16, padding: '24px',
    boxShadow: '0 1px 3px rgba(0,0,0,0.08), 0 4px 16px rgba(0,0,0,0.04)',
    border: '1px solid #F3F4F6',
  }}>
    <h3 style={{ margin: '0 0 20px', fontSize: 15, fontWeight: 600, color: '#111827', display: 'flex', alignItems: 'center', gap: 8 }}>
      <span>{icon}</span> {title}
    </h3>
    {children}
  </div>
);

// ─────────────────────────────────────────────────────────────────────────────
// PÁGINA PRINCIPAL DEL DASHBOARD
// Consume el nuevo endpoint GET /api/dashboard/stats
// ─────────────────────────────────────────────────────────────────────────────
const DashboardPage = () => {
  // ✅ Query al nuevo endpoint de estadísticas consolidadas
  const { data: stats, isLoading: loadingStats } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: getDashboardStats,
    refetchInterval: 30_000, // Auto-refresh cada 30 segundos
  });

  // Proyectos recientes y tareas pendientes (datos existentes)
  const { data: projectsData, isLoading: loadingProjects } = useQuery({
    queryKey: ['projects', { size: 5 }],
    queryFn: () => getProjects({ size: 5 }),
  });

  const { data: tasksData, isLoading: loadingTasks } = useQuery({
    queryKey: ['tasks', { size: 5, status: 'todo' }],
    queryFn: () => getTasks({ size: 5, status: 'todo' }),
  });

  if (loadingStats) return (
    <div style={{ display: 'flex', justifyContent: 'center', padding: 80 }}>
      <Spinner />
    </div>
  );

  const t = stats?.tasks;
  const p = stats?.projects;
  const d = stats?.developers;
  const totalTasks = t?.total || 0;

  return (
    <div style={{ maxWidth: 1280, margin: '0 auto' }}>
      {/* Header */}
      <div style={{ marginBottom: 32 }}>
        <h1 style={{ margin: 0, fontSize: 28, fontWeight: 700, color: '#111827' }}>
          📊 Dashboard Ejecutivo
        </h1>
        <p style={{ margin: '6px 0 0', color: '#6B7280', fontSize: 14 }}>
          Estadísticas en tiempo real · Actualizado {stats?.generatedAt
            ? new Date(stats.generatedAt).toLocaleTimeString('es-EC')
            : '—'}
        </p>
      </div>

      {/* ── Fila 1: KPIs Principales ─────────────────────────────────── */}
      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
        gap: 16, marginBottom: 24,
      }}>
        <StatCard title="Total Proyectos"    value={p?.total ?? '—'} icon="📁" color="#6366F1"
          subtitle={`${p?.byStatus?.inProgress ?? 0} en progreso`} />
        <StatCard title="Total Tareas"       value={t?.total ?? '—'} icon="✅" color="#22C55E"
          subtitle={`${t?.completedThisMonth ?? 0} completadas este mes`} />
        <StatCard title="Desarrolladores"    value={d?.active ?? '—'} icon="👨‍💻" color="#F59E0B"
          subtitle={`${d?.byAvailability?.available ?? 0} disponibles`} />
        <StatCard title="Tareas Vencidas"    value={t?.overdueCount ?? '—'} icon="⚠️" color="#EF4444"
          subtitle="Requieren atención" />
      </div>

      {/* ── Fila 2: Barras de estado ──────────────────────────────────── */}
      <div style={{
        display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))',
        gap: 16, marginBottom: 24,
      }}>
        {/* Tareas por estado */}
        <BarSection title="Tareas por Estado" icon="📋">
          {Object.entries(STATUS_COLORS).map(([key, c]) => (
            <ProgressBar key={key}
              label={c.label}
              value={t?.byStatus?.[key === 'in_progress' ? 'inProgress' : key] ?? 0}
              total={totalTasks}
              color={c.bar} textColor={c.text} />
          ))}
        </BarSection>

        {/* Tareas por prioridad */}
        <BarSection title="Tareas por Prioridad" icon="🎯">
          {Object.entries(PRIORITY_COLORS).map(([key, c]) => (
            <ProgressBar key={key}
              label={c.label}
              value={t?.byPriority?.[key] ?? 0}
              total={totalTasks}
              color={c.bar} textColor={c.text} />
          ))}
        </BarSection>

        {/* Desarrolladores */}
        <BarSection title="Desarrolladores" icon="🧑‍🤝‍🧑">
          <p style={{ fontSize: 12, color: '#9CA3AF', marginTop: 0, marginBottom: 12 }}>Por disponibilidad</p>
          {Object.entries(AVAILABILITY_COLORS).map(([key, c]) => (
            <ProgressBar key={key}
              label={c.label}
              value={d?.byAvailability?.[key] ?? 0}
              total={d?.active ?? 1}
              color={c.bar} />
          ))}
          <p style={{ fontSize: 12, color: '#9CA3AF', marginTop: 16, marginBottom: 12 }}>Por seniority</p>
          {Object.entries(SENIORITY_COLORS).map(([key, c]) => (
            <ProgressBar key={key}
              label={c.label}
              value={d?.bySeniority?.[key] ?? 0}
              total={d?.active ?? 1}
              color={c.bar} />
          ))}
        </BarSection>

        {/* Proyectos por estado */}
        <BarSection title="Proyectos por Estado" icon="📁">
          {[
            { key: 'planning',   label: 'Planificación', color: '#93C5FD' },
            { key: 'inProgress', label: 'En progreso',   color: '#FCD34D' },
            { key: 'onHold',     label: 'En pausa',      color: '#D1D5DB' },
            { key: 'completed',  label: 'Completados',   color: '#6EE7B7' },
            { key: 'cancelled',  label: 'Cancelados',    color: '#FCA5A5' },
          ].map(({ key, label, color }) => (
            <ProgressBar key={key}
              label={label}
              value={p?.byStatus?.[key] ?? 0}
              total={p?.total || 1}
              color={color} />
          ))}
        </BarSection>
      </div>

      {/* ── Fila 3: Listas recientes ─────────────────────────────────── */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
        {/* Proyectos recientes */}
        <div style={{
          background: '#fff', borderRadius: 16, overflow: 'hidden',
          boxShadow: '0 1px 3px rgba(0,0,0,0.08)', border: '1px solid #F3F4F6',
        }}>
          <div style={{
            padding: '20px 24px', borderBottom: '1px solid #F3F4F6',
            display: 'flex', justifyContent: 'space-between', alignItems: 'center',
          }}>
            <h3 style={{ margin: 0, fontSize: 15, fontWeight: 600, color: '#111827' }}>
              📁 Proyectos Recientes
            </h3>
            <Link to="/projects" style={{ fontSize: 13, color: '#6366F1', textDecoration: 'none', fontWeight: 500 }}>
              Ver todos →
            </Link>
          </div>
          {loadingProjects ? <div style={{ padding: 24 }}><Spinner /></div> : (
            <ul style={{ margin: 0, padding: 0, listStyle: 'none' }}>
              {projectsData?.items?.length === 0 ? (
                <li style={{ padding: '16px 24px', color: '#9CA3AF', fontSize: 14 }}>Sin proyectos.</li>
              ) : projectsData?.items?.map(project => (
                <li key={project.id} style={{ borderBottom: '1px solid #F9FAFB' }}>
                  <Link to={`/projects/${project.id}`} style={{ display: 'block', padding: '14px 24px', textDecoration: 'none', transition: 'background 0.15s' }}
                    onMouseEnter={e => e.currentTarget.style.background = '#F9FAFB'}
                    onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <p style={{ margin: 0, fontSize: 14, fontWeight: 500, color: '#111827' }}>{project.name}</p>
                      <Badge type="status" value={project.status} />
                    </div>
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 4 }}>
                      <p style={{ margin: 0, fontSize: 12, color: '#9CA3AF' }}>{project.projectCode}</p>
                      <DueDateBadge dueDate={project.dueDate} />
                    </div>
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Tareas pendientes */}
        <div style={{
          background: '#fff', borderRadius: 16, overflow: 'hidden',
          boxShadow: '0 1px 3px rgba(0,0,0,0.08)', border: '1px solid #F3F4F6',
        }}>
          <div style={{
            padding: '20px 24px', borderBottom: '1px solid #F3F4F6',
            display: 'flex', justifyContent: 'space-between', alignItems: 'center',
          }}>
            <h3 style={{ margin: 0, fontSize: 15, fontWeight: 600, color: '#111827' }}>
              ⏳ Tareas Pendientes
            </h3>
            <Link to="/tasks" style={{ fontSize: 13, color: '#6366F1', textDecoration: 'none', fontWeight: 500 }}>
              Ver todas →
            </Link>
          </div>
          {loadingTasks ? <div style={{ padding: 24 }}><Spinner /></div> : (
            <ul style={{ margin: 0, padding: 0, listStyle: 'none' }}>
              {tasksData?.items?.length === 0 ? (
                <li style={{ padding: '16px 24px', color: '#9CA3AF', fontSize: 14 }}>Sin tareas pendientes.</li>
              ) : tasksData?.items?.map(task => (
                <li key={task.id} style={{ borderBottom: '1px solid #F9FAFB' }}>
                  <Link to={`/tasks/${task.id}`} style={{ display: 'block', padding: '14px 24px', textDecoration: 'none', transition: 'background 0.15s' }}
                    onMouseEnter={e => e.currentTarget.style.background = '#F9FAFB'}
                    onMouseLeave={e => e.currentTarget.style.background = 'transparent'}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <p style={{ margin: 0, fontSize: 14, fontWeight: 500, color: '#111827' }}>{task.title}</p>
                      <Badge type="priority" value={task.priority} />
                    </div>
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 4 }}>
                      <p style={{ margin: 0, fontSize: 12, color: '#9CA3AF' }}>{task.taskCode}</p>
                      <DueDateBadge dueDate={task.dueDate} />
                    </div>
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
