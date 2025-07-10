# Instrucciones para GitHub Copilot

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Contexto del Proyecto
Este es un sistema de reservas desarrollado como trabajo práctico de Programación 3. El proyecto consta de tres componentes principales:

1. **ReservasApp.MVC**: Aplicación ASP.NET MVC con Razor Pages para registro/login, recuperación de contraseña y gestión de reservas
2. **ReservasApp.WebAPI**: API RESTful en ASP.NET Web API con autenticación JWT y generación de QR
3. **ReservasApp.SPA**: Single Page Application en React que consume la API

## Tecnologías y Patrones
- **Backend**: ASP.NET Core 9, Entity Framework Core, SQLite
- **Autenticación**: ASP.NET Identity, JWT tokens
- **Seguridad**: Hash + Salt para contraseñas, enlaces seguros con expiración
- **Base de datos**: SQLite con Entity Framework Core migraciones
- **QR**: Librería QRCoder para generar códigos QR con enlaces dinámicos
- **Patrones**: MVC, Repository, Controller-Service-Repository

## Entidades Principales
- **Usuario**: Manejo de autenticación e identidad
- **Reserva**: Entidad principal del CRUD con campos como tipo de servicio, fecha, descripción
- **QR Links**: Enlaces seguros con expiración de 10 minutos

## Características de Seguridad
- Contraseñas con hash y salt usando ASP.NET Identity
- Recuperación de contraseña por correo con enlaces únicos
- Protección de endpoints con JWT
- Enlaces QR con expiración temporal (10 minutos)

## Estructura de Carpetas
- Mantener separación clara entre capas (Models, Views, Controllers, Services)
- Usar migraciones para cambios en la base de datos
- Implementar buenas prácticas de validación y manejo de errores

## Instrucciones Específicas
- Implementar paginación para listados
- Incluir al menos un dropdown/selector en formularios
- Generar códigos QR desde el backend con URLs dinámicas
- Mantener funcionalidad exclusiva accesible solo por QR
- Usar buenas prácticas de seguridad web
