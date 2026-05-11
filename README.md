# Dev Task Manager

Este es un sistema de gestión de tareas y desarrolladores. Consta de un backend en .NET (C#) y un frontend web, utilizando una base de datos MySQL.

## Estructura del Proyecto

- `backend/`: Código fuente del API en ASP.NET Core y C#.
- `frontend/`: Código del cliente web desarrollado con React/Vite.
- `database/`: Scripts de inicialización y seeders de la base de datos MySQL.

## Credenciales por defecto (Seed)

El sistema incluye datos de prueba para iniciar sesión:

**Administrador:**
- **Usuario:** `admin`
- **Contraseña:** `1234`

**Desarrollador (Prueba):**
- **Usuario:** `dev_juan`
- **Contraseña:** `1234`

## Ejecutar Localmente

### Backend
1. Asegurarse de tener la cadena de conexión a MySQL correcta.
2. Navegar a `backend/DevTaskManager.API`.
3. Ejecutar `dotnet run`.

### Frontend
1. Navegar a `frontend/`.
2. Ejecutar `npm install` (si no se ha hecho).
3. Ejecutar `npm run dev`.

