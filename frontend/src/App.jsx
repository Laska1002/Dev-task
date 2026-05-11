import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Layout from './components/layout/Layout';

import LoginPage from './pages/auth/LoginPage';
import DashboardPage from './pages/dashboard/DashboardPage';
import ProjectsPage from './pages/projects/ProjectsPage';
import ProjectDetailPage from './pages/projects/ProjectDetailPage';
import ProjectFormPage from './pages/projects/ProjectFormPage';
import TasksPage from './pages/tasks/TasksPage';
import TaskDetailPage from './pages/tasks/TaskDetailPage';
import TaskFormPage from './pages/tasks/TaskFormPage';
import DevelopersPage from './pages/developers/DevelopersPage';
import DeveloperFormPage from './pages/developers/DeveloperFormPage';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          
          <Route element={<Layout />}>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            
            <Route path="/projects" element={<ProjectsPage />} />
            <Route path="/projects/new" element={<ProjectFormPage />} />
            <Route path="/projects/:id" element={<ProjectDetailPage />} />
            <Route path="/projects/:id/edit" element={<ProjectFormPage />} />
            
            <Route path="/tasks" element={<TasksPage />} />
            <Route path="/tasks/new" element={<TaskFormPage />} />
            <Route path="/tasks/:id" element={<TaskDetailPage />} />
            <Route path="/tasks/:id/edit" element={<TaskFormPage />} />
            
            <Route path="/developers" element={<DevelopersPage />} />
            <Route path="/developers/new" element={<DeveloperFormPage />} />
            <Route path="/developers/:id/edit" element={<DeveloperFormPage />} />
          </Route>
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
