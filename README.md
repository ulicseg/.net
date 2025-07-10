# 🎯 Sistema de Reservas - Programación 3

Un sistema completo de gestión de reservas desarrollado con ASP.NET Core, que incluye una aplicación MVC, una API RESTful y una SPA en React.

## 📋 Descripción del Proyecto

Este trabajo práctico implementa un sistema de reservas que cumple con todos los requisitos técnicos de la materia Programación 3:

- **Aplicación Web MVC**: Registro, login, gestión de reservas con Razor Pages
- **API RESTful**: Endpoints protegidos con JWT para la SPA
- **SPA React**: Interfaz moderna que consume la API
- **Seguridad**: Autenticación robusta, hash + salt, recuperación de contraseñas
- **QR Dinámicos**: Generación de códigos QR con enlaces seguros y temporales

## 🏗️ Arquitectura del Sistema

```
📦 Sistema de Reservas
├── 🌐 ReservasApp.MVC (ASP.NET MVC)
│   ├── Autenticación con ASP.NET Identity
│   ├── CRUD de Reservas con paginación
│   ├── Generación de códigos QR
│   └── Recuperación de contraseñas
│
├── 🔌 ReservasApp.WebAPI (ASP.NET Web API)
│   ├── Endpoints RESTful
│   ├── Autenticación JWT
│   ├── Generación de QR para SPA
│   └── Endpoints protegidos
│
├── ⚛️ ReservasApp.SPA (React)
│   ├── Interfaz moderna
│   ├── Consumo de API
│   ├── Gestión de tokens JWT
│   └── Componentes reutilizables
│
└── 🗄️ Database
    ├── Scripts SQL
    └── Migraciones EF Core
```

## 🔧 Tecnologías Utilizadas

### Backend
- **ASP.NET Core 9** - Framework principal
- **Entity Framework Core** - ORM para base de datos
- **SQLite** - Base de datos ligera para desarrollo
- **ASP.NET Identity** - Sistema de autenticación
- **QRCoder** - Generación de códigos QR

### Frontend (SPA)
- **React 18** - Framework de frontend
- **Axios** - Cliente HTTP para API
- **React Router** - Enrutamiento
- **Bootstrap/Material-UI** - Componentes de UI

### Seguridad
- **JWT Tokens** - Autenticación para API
- **BCrypt** - Hash de contraseñas con salt
- **HTTPS** - Comunicación segura
- **CORS** - Configuración de políticas

## 🚀 Inicio Rápido

### Prerrequisitos
- **.NET 9 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download)
- **Node.js 18+** - [Descargar aquí](https://nodejs.org/)
- **Visual Studio Code** - [Descargar aquí](https://code.visualstudio.com/)

### Configuración del Proyecto

1. **Clonar el repositorio**
   ```bash
   git clone [URL-del-repositorio]
   cd Practico
   ```

2. **Configurar la aplicación MVC**
   ```bash
   cd ReservasApp.MVC
   dotnet restore
   dotnet ef database update
   dotnet run
   ```
   La aplicación estará disponible en `https://localhost:5001`

3. **Configurar la Web API**
   ```bash
   cd ../ReservasApp.WebAPI
   dotnet restore
   dotnet run
   ```
   La API estará disponible en `https://localhost:5101`

4. **Configurar la SPA React**
   ```bash
   cd ../ReservasApp.SPA
   npm install
   npm start
   ```
   La SPA estará disponible en `http://localhost:3000`

## 📊 Base de Datos

### Entidades Principales

#### Usuario
```csharp
public class Usuario : IdentityUser
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaRegistro { get; set; }
    public ICollection<Reserva> Reservas { get; set; }
}
```

#### Reserva
```csharp
public class Reserva
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaReserva { get; set; }
    public TipoServicio TipoServicio { get; set; }
    public string UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
}
```

### Migraciones
```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update
```

## 🔐 Características de Seguridad

### Autenticación
- **Hash + Salt**: Contraseñas seguras con ASP.NET Identity
- **JWT Tokens**: Autenticación stateless para la API
- **Recuperación de contraseñas**: Enlaces únicos con expiración

### Códigos QR
- **Generación dinámica**: URLs únicas por solicitud
- **Expiración temporal**: Enlaces válidos por 10 minutos
- **Funcionalidad exclusiva**: Acceso a características especiales

### Ejemplo de generación de QR:
```csharp
public string GenerarQRReserva(int reservaId)
{
    var hash = GenerarHashSeguro(reservaId, DateTime.UtcNow);
    var url = $"https://localhost:5001/Reservas/QR/{hash}";
    var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
    var qrCode = new PngByteQRCode(qrCodeData);
    return Convert.ToBase64String(qrCode.GetGraphic(20));
}
```

## 📋 Funcionalidades Implementadas

### ✅ Requisitos Cumplidos

#### Aplicación MVC
- [x] Registro de nuevos usuarios
- [x] Inicio de sesión con usuario/contraseña
- [x] Hash + salt para contraseñas
- [x] Recuperación de contraseña por email
- [x] Listado paginado de reservas
- [x] Vista de detalle de reservas
- [x] Crear, editar, eliminar reservas
- [x] Selector/dropdown para tipo de servicio
- [x] Generación de códigos QR
- [x] Funcionalidad exclusiva por QR

#### API RESTful
- [x] Endpoints RESTful
- [x] Autenticación JWT
- [x] Endpoints protegidos
- [x] Generación de QR desde API

#### SPA React
- [x] Interfaz de usuario moderna
- [x] Consumo de API
- [x] Ejecución en cliente

### 🔄 CRUD de Reservas

| Operación | MVC | API | SPA |
|-----------|-----|-----|-----|
| **C**rear | ✅ | ✅ | ✅ |
| **R**ead  | ✅ | ✅ | ✅ |
| **U**pdate| ✅ | ✅ | ✅ |
| **D**elete| ✅ | ✅ | ✅ |

## 🧪 Testing

### Probar la aplicación

1. **Registro de usuario**: Crear cuenta nueva
2. **Login**: Iniciar sesión con credenciales
3. **Crear reserva**: Usar el formulario con dropdown
4. **Listar reservas**: Ver paginación funcionando
5. **Generar QR**: Crear código QR y escanearlo
6. **API**: Probar endpoints con Postman
7. **SPA**: Navegar por la interfaz React

### URLs de Prueba
- **MVC**: `https://localhost:5001`
- **API**: `https://localhost:5101/swagger`
- **SPA**: `http://localhost:3000`

## 📂 Estructura de Carpetas

```
Practico/
├── 📁 ReservasApp.MVC/
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   ├── Services/
│   └── Data/
├── 📁 ReservasApp.WebAPI/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Data/
├── 📁 ReservasApp.SPA/
│   ├── src/
│   │   ├── components/
│   │   ├── services/
│   │   └── pages/
│   └── public/
├── 📁 Database/
│   ├── scripts/
│   └── migrations/
└── 📁 .github/
    └── copilot-instructions.md
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crear rama feature (`git checkout -b feature/NuevaFuncionalidad`)
3. Commit cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/NuevaFuncionalidad`)
5. Abrir Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para detalles.

## 👥 Equipo de Desarrollo

- **Desarrollador**: [Tu Nombre]
- **Materia**: Programación 3
- **Universidad**: [Tu Universidad]
- **Año**: 2025

---

💡 **Nota**: Este README se actualiza conforme se desarrolla el proyecto. Para instrucciones detalladas de cada componente, revisar los README específicos en cada carpeta.
