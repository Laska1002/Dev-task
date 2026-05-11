-- ============================================================
-- DEV TASK MANAGER — Schema MySQL 8 (Refactored)
-- ============================================================
CREATE DATABASE IF NOT EXISTS devtaskmanager
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE devtaskmanager;

-- 1. USERS (autenticación)
CREATE TABLE users (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(50) NOT NULL UNIQUE,
  email VARCHAR(100) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  role ENUM('admin','developer') NOT NULL DEFAULT 'developer',
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_email (email),
  INDEX idx_role (role)
) ENGINE=InnoDB;

-- 2. PROJECT TYPES (catálogo)
CREATE TABLE project_types (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(50) NOT NULL UNIQUE,
  description VARCHAR(255),
  is_active TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- 2.1 TECHNOLOGIES / SKILLS (catálogo por tipo de proyecto)
CREATE TABLE technologies (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  project_type_id INT UNSIGNED NOT NULL,
  name VARCHAR(100) NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  FOREIGN KEY (project_type_id) REFERENCES project_types(id) ON DELETE CASCADE,
  UNIQUE KEY uq_technology_name (name)
) ENGINE=InnoDB;

-- 3. DEVELOPERS (perfil + cédula)
CREATE TABLE developers (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  user_id INT UNSIGNED NULL,
  full_name VARCHAR(100) NOT NULL,
  cedula VARCHAR(13) NOT NULL UNIQUE COMMENT 'Validado con algoritmo módulo 10 en backend',
  project_type_id INT UNSIGNED NOT NULL COMMENT 'Tipo de desarrollador (Front, Back, etc)',
  seniority_level ENUM('junior','mid','senior') NOT NULL DEFAULT 'mid',
  availability_status ENUM('available','busy','vacation') NOT NULL DEFAULT 'available',
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL ON UPDATE CASCADE,
  FOREIGN KEY (project_type_id) REFERENCES project_types(id) ON DELETE RESTRICT,
  INDEX idx_availability (availability_status),
  INDEX idx_seniority (seniority_level)
) ENGINE=InnoDB;

-- 3.1 DEVELOPER_TECHNOLOGIES (habilidades)
CREATE TABLE developer_technologies (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  developer_id INT UNSIGNED NOT NULL,
  technology_id INT UNSIGNED NOT NULL,
  FOREIGN KEY (developer_id) REFERENCES developers(id) ON DELETE CASCADE,
  FOREIGN KEY (technology_id) REFERENCES technologies(id) ON DELETE CASCADE,
  UNIQUE KEY uq_dev_tech (developer_id, technology_id)
) ENGINE=InnoDB;

-- 4. PROJECTS
CREATE TABLE projects (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  project_code VARCHAR(20) NOT NULL UNIQUE COMMENT 'Generado en backend: PRJ-YYYY-001. NUNCA editable.',
  name VARCHAR(150) NOT NULL,
  description TEXT,
  project_type_id INT UNSIGNED NOT NULL,
  technology_id INT UNSIGNED NOT NULL COMMENT 'Lenguaje principal asociado al proyecto',
  status ENUM('planning','in_progress','on_hold','completed','cancelled') NOT NULL DEFAULT 'planning',
  start_date DATE,
  due_date DATE,
  estimated_hours DECIMAL(8,2),
  created_by INT UNSIGNED NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (project_type_id) REFERENCES project_types(id) ON DELETE RESTRICT,
  FOREIGN KEY (technology_id) REFERENCES technologies(id) ON DELETE RESTRICT,
  FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE RESTRICT,
  INDEX idx_status (status),
  INDEX idx_due_date (due_date)
) ENGINE=InnoDB;

-- 5. PROJECT_DEVELOPERS (pivote N:N)
CREATE TABLE project_developers (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  project_id INT UNSIGNED NOT NULL,
  developer_id INT UNSIGNED NOT NULL,
  role_in_project VARCHAR(100),
  assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE,
  FOREIGN KEY (developer_id) REFERENCES developers(id) ON DELETE CASCADE,
  UNIQUE KEY uq_project_developer (project_id, developer_id)
) ENGINE=InnoDB;

-- 6. TASKS
CREATE TABLE tasks (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  task_code VARCHAR(20) NOT NULL UNIQUE COMMENT 'Generado en backend: TSK-YYYY-001. NUNCA editable.',
  project_id INT UNSIGNED NOT NULL,
  title VARCHAR(200) NOT NULL,
  description TEXT,
  assigned_to INT UNSIGNED NULL,
  status ENUM('todo','in_progress','review','done','blocked') NOT NULL DEFAULT 'todo',
  priority ENUM('low','medium','high','critical') NOT NULL DEFAULT 'medium',
  estimated_hours DECIMAL(6,2),
  due_date DATE,
  completed_at TIMESTAMP NULL,
  created_by INT UNSIGNED NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE,
  FOREIGN KEY (assigned_to) REFERENCES developers(id) ON DELETE SET NULL,
  FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE RESTRICT,
  INDEX idx_task_status (status),
  INDEX idx_task_priority (priority)
) ENGINE=InnoDB;

-- 7. TASK_COMMENTS
CREATE TABLE task_comments (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  task_id INT UNSIGNED NOT NULL,
  user_id INT UNSIGNED NOT NULL,
  comment TEXT NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (task_id) REFERENCES tasks(id) ON DELETE CASCADE,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;
