# User Service - Microservicio de Usuarios

## 📋 Descripción

Microservicio para la gestión de usuarios, autenticación y roles del sistema de Control de Tareas y Calificaciones.

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas (Clean Architecture):

```
UserService/
├── src/
│   ├── UserService.API/          # Capa de presentación (Controllers, Middleware)
│   ├── UserService.Application/  # Lógica de aplicación (Services, DTOs)
│   ├── UserService.Domain/       # Entidades de dominio e interfaces
│   └── UserService.Infrastructure/ # Implementación de datos (EF Core, Repositorios)
```

## 🚀 Tecnologías

- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM
- **SQL Server** - Base de datos (LocalDB para desarrollo)
- **JWT** - Autenticación
- **BCrypt** - Hash de contraseñas
- **Swagger** - Documentación de API

## 📦 Requisitos Previos

- .NET 8 SDK
- SQL Server LocalDB (incluido en Visual Studio)
- Visual Studio 2022 o VS Code

## 🛠️ Instalación y Configuración

### 1. Clonar o ubicarse en el directorio del proyecto

```bash
cd D:\user\desktop\microservicio\UserService
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Aplicar migraciones de base de datos

```bash
cd src/UserService.API
dotnet ef migrations add InitialCreate --project ../UserService.Infrastructure
dotnet ef database update
```

### 4. Ejecutar el proyecto

```bash
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📚 Endpoints Principales

### Autenticación (Sin autenticación requerida)

#### Registro de Usuario
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "juanperez",
  "email": "juan@example.com",
  "password": "Password123!",
  "fullName": "Juan Pérez"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "juan@example.com",
  "password": "Password123!"
}
```

### Usuarios (Requiere autenticación)

#### Obtener todos los usuarios (Solo Admin)
```http
GET /api/users
Authorization: Bearer {token}
```

#### Obtener usuario por ID
```http
GET /api/users/{id}
Authorization: Bearer {token}
```

#### Actualizar usuario
```http
PUT /api/users/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "fullName": "Juan Carlos Pérez",
  "email": "juancarlos@example.com"
}
```

#### Eliminar usuario (Solo Admin)
```http
DELETE /api/users/{id}
Authorization: Bearer {token}
```

#### Asignar rol a usuario (Solo Admin)
```http
POST /api/users/assign-role
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 1,
  "roleId": 2
}
```

## 🔐 Roles del Sistema

El sistema incluye 3 roles por defecto:

1. **Admin** (ID: 1) - Administrador del sistema
2. **Profesor** (ID: 2) - Profesor de cursos
3. **Estudiante** (ID: 3) - Estudiante de cursos (rol por defecto al registrarse)

## 🗄️ Modelo de Datos

### Tabla: Users
- `Id` (INT, PK)
- `Username` (NVARCHAR, UNIQUE)
- `Email` (NVARCHAR, UNIQUE)
- `PasswordHash` (NVARCHAR)
- `FullName` (NVARCHAR)
- `CreatedAt` (DATETIME)
- `IsEnabled` (BIT)

### Tabla: Roles
- `Id` (INT, PK)
- `Name` (NVARCHAR, UNIQUE)
- `Description` (NVARCHAR)

### Tabla: UserRoles
- `Id` (INT, PK)
- `UserId` (INT, FK)
- `RoleId` (INT, FK)
- `AssignedAt` (DATETIME)

## 🧪 Pruebas con Swagger

1. Ejecutar el proyecto
2. Navegar a `https://localhost:5001/swagger`
3. Registrar un usuario con `/api/auth/register`
4. Hacer login con `/api/auth/login` para obtener el token
5. Hacer clic en "Authorize" y pegar el token
6. Probar los demás endpoints

## 📝 Comandos Útiles

### Crear nueva migración
```bash
dotnet ef migrations add NombreMigracion --project src/UserService.Infrastructure --startup-project src/UserService.API
```

### Aplicar migraciones
```bash
dotnet ef database update --project src/UserService.Infrastructure --startup-project src/UserService.API
```

### Eliminar última migración
```bash
dotnet ef migrations remove --project src/UserService.Infrastructure --startup-project src/UserService.API
```

### Compilar el proyecto
```bash
dotnet build
```

### Ejecutar pruebas
```bash
dotnet test
```

## 🚢 Preparación para AWS

Este microservicio está listo para ser desplegado en AWS. Consideraciones:

1. **Base de datos**: Cambiar connection string a AWS RDS (SQL Server)
2. **Variables de entorno**: Configurar JWT Key como secreto en AWS Secrets Manager
3. **Container**: Crear Dockerfile para despliegue en ECS/EKS
4. **API Gateway**: Configurar AWS API Gateway para enrutamiento

### Ejemplo de Connection String para AWS RDS:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=myserver.rds.amazonaws.com;Database=UserServiceDB;User Id=admin;Password=password;TrustServerCertificate=true;"
}
```

## 🤝 Comunicación con otros Microservicios

Este microservicio se comunicará con:
- **Course Service**: Para validar que usuarios existen al inscribirse en cursos
- **Task Service**: Para validar permisos de profesores y estudiantes

## 📄 Licencia

Proyecto educativo - Universidad

## 👨‍💻 Autor

Desarrollado para el curso de Desarrollo Web
