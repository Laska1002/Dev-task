USE devtaskmanager;

-- 1. Users (passwords: 1234 hasheados con BCrypt)
INSERT INTO users (username, email, password_hash, role) VALUES
('admin', 'admin@devtask.com', '$2a$11$QiU9guXHdxbLzG4xH6V4auCS8Yoo9RkYBPrhPRMLF6J9N1ZMo/5lm', 'admin'),
('dev_juan', 'juan@devtask.com', '$2a$11$TJB/SJM.8yAwqKc29pq.BOUcxEazqC.qErOj.DrlKQXQcIN0LJEhq', 'developer');

-- 2. Tipos de proyecto
INSERT INTO project_types (id, name, description) VALUES
(1, 'Backend', 'APIs, microservicios, bases de datos'),
(2, 'Frontend', 'Interfaces web y móviles'),
(3, 'Fullstack', 'Proyecto completo end-to-end'),
(4, 'DevOps', 'Infraestructura, CI/CD, despliegues'),
(5, 'Mobile', 'Apps nativas o híbridas');

-- 3. Tecnologías / Lenguajes
INSERT INTO technologies (id, project_type_id, name) VALUES
(1, 1, 'C# / .NET'),
(2, 1, 'Node.js'),
(3, 1, 'Java / Spring'),
(4, 2, 'React.js'),
(5, 2, 'Angular'),
(6, 2, 'Vue.js'),
(7, 3, 'Next.js'),
(8, 4, 'AWS / Docker'),
(9, 5, 'Flutter'),
(10, 5, 'React Native');

-- 4. Developers
INSERT INTO developers (id, user_id, full_name, cedula, project_type_id, seniority_level) VALUES
(1, 2, 'Juan Pérez García', '1710034065', 1, 'senior'),
(2, NULL, 'María López Suárez', '1726524716', 2, 'mid'),
(3, NULL, 'Carlos Vega Mora', '0102568954', 4, 'senior');

-- 5. Developer Technologies (Habilidades)
INSERT INTO developer_technologies (developer_id, technology_id) VALUES
(1, 1), -- Juan sabe C# / .NET
(1, 2), -- Juan sabe Node.js
(2, 4), -- Maria sabe React.js
(2, 5), -- Maria sabe Angular
(3, 8); -- Carlos sabe AWS / Docker

-- 6. Projects
INSERT INTO projects (id, project_code, name, description, project_type_id, technology_id, status, start_date, due_date, estimated_hours, created_by) VALUES
(1, 'PRJ-2024-001', 'API Gateway Institucional', 'Construcción de API Gateway para microservicios internos', 1, 1, 'in_progress', '2024-01-15', '2024-06-30', 480.00, 1),
(2, 'PRJ-2024-002', 'Dashboard Analítico', 'Panel de métricas en tiempo real para operaciones', 2, 4, 'planning', '2024-03-01', '2024-08-31', 320.00, 1);

-- 7. Project Developers
INSERT INTO project_developers (project_id, developer_id, role_in_project) VALUES
(1, 1, 'Tech Lead'),
(2, 2, 'Frontend Developer');
