# Proyecto SGIO - Sistema de Gestión de Información de Usuarios

## Descripción

El **Sistema de Gestión de Información de Usuarios (SGIO)** es una aplicación web que permite gestionar usuarios, roles y sus estados en un sistema. Los administradores pueden crear usuarios, asignar roles, cambiar el estado de los usuarios (activar o bloquear) y actualizar sus roles. Los usuarios pueden registrarse, iniciar sesión y acceder a sus perfiles, siempre que su cuenta esté activa.

## Características

- **Administradores**:
  - Crear usuarios y asignar roles.
  - Cambiar el estado (activo/inactivo) de los usuarios.
  - Cambiar roles de los usuarios.
  
- **Usuarios**:
  - Registrarse en el sistema.
  - Iniciar sesión.
  - Ver su perfil.

- **Control de acceso**:
  - Bloqueo de usuarios inactivos (si un usuario está bloqueado, no puede iniciar sesión).
  - Autenticación basada en roles (administrador, usuario).

## Tecnologías Utilizadas

- **ASP.NET Core MVC**: Framework para construir la aplicación web.
- **Entity Framework Core**: ORM para interactuar con la base de datos.
- **SQL Server**: Base de datos relacional.
- **Bootstrap 5**: Para el diseño responsivo y los componentes de la interfaz de usuario.
- **DataTables**: Para una gestión interactiva de las tablas de usuarios y roles.
- **Autenticación basada en cookies**: Manejo de sesiones y autenticación.
