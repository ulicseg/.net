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
- **JWT Bearer** - Autenticación para API
- **AutoMapper** - Mapeo de objetos

### Frontend (SPA)
- **React 18** - Framework de frontend
- **TypeScript** - Tipado estático
- **Vite** - Build tool y dev server
- **Axios** - Cliente HTTP para API
- **React Router** - Enrutamiento
- **Tailwind CSS** - Framework CSS

### Seguridad
- **JWT Tokens** - Autenticación para API
- **ASP.NET Identity** - Hash + salt para contraseñas
- **HTTPS** - Comunicación segura
- **CORS** - Configuración de políticas

## 🚀 Inicio Rápido

### 📋 Prerrequisitos

Asegurate de tener instalado:

- **.NET 9 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 18+** - [Descargar aquí](https://nodejs.org/)
- **Git** - [Descargar aquí](https://git-scm.com/)

### 🛠️ Herramientas Necesarias

Instalar Entity Framework Core Tools:
```bash
dotnet tool install --global dotnet-ef
```

Verificar instalación:
```bash
dotnet --version
dotnet ef --version
node --version
npm --version
```

### 🎬 Configuración del Proyecto

#### 1. **Clonar el repositorio**
```bash
git clone [URL-del-repositorio]
cd .net
```

#### 2. **Configurar la Web API (⚠️ IMPORTANTE: Hacer PRIMERO)**
```bash
cd ReservasApp.WebAPI
dotnet restore
```

**Crear las migraciones iniciales:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Ejecutar la API:**
```bash
dotnet run
```
✅ La API estará disponible en:
- **HTTPS**: `https://localhost:7121` 
- **HTTP**: `http://localhost:5284`
- **Swagger**: `https://localhost:7121/swagger`

#### 3. **Configurar la aplicación MVC**
```bash
cd ../ReservasApp.MVC
dotnet restore
```

**⚠️ IMPORTANTE**: La base de datos ya fue creada por la API, no ejecutes migraciones aquí.

**Ejecutar el MVC:**
```bash
dotnet run
```
✅ La aplicación estará disponible en:
- **HTTPS**: `https://localhost:7092`
- **HTTP**: `http://localhost:5019`

#### 4. **Configurar la SPA React**
```bash
cd ../ReservaApp.SPA
npm install
```

**Crear archivo de variables de entorno:**
```bash
# Crear archivo .env en la raíz de ReservaApp.SPA
echo "VITE_API_URL=https://localhost:7121/api" > .env
```

**Ejecutar la SPA:**
```bash
npm run dev
```
✅ La SPA estará disponible en:
- **Desarrollo**: `http://localhost:5173`

### 🔑 Configuraciones Importantes

#### Variables de Entorno (SPA)
Crear archivo `.env` en `ReservaApp.SPA/`:
```env
VITE_API_URL=https://localhost:7121/api
```

#### Configuración JWT (Producción)
En `ReservasApp.WebAPI/appsettings.json`, cambiar:
```json
{
  "JwtSettings": {
    "Secret": "TU_CLAVE_SECRETA_SUPER_SEGURA_MINIMO_32_CARACTERES",
    "Issuer": "ReservasApp.WebAPI",
    "Audience": "ReservasApp.Clients",
    "ExpirationInMinutes": 60
  }
}
```

#### Configuración SMTP (Para emails)
En `ReservasApp.MVC/appsettings.json`:
```json
{
  "Email": {
    "SimularEnvio": false,
    "From": "tu-email@gmail.com",
    "FromName": "Sistema de Reservas",
    "SMTP": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "tu-email@gmail.com",
      "Password": "tu-password-de-aplicacion"
    }
  }
}
```

### 👨‍💼 Usuario Administrador por Defecto

La API crea automáticamente un usuario administrador:
```
Email: admin@reservas.com
Password: Admin123!
```

### 🔄 Orden de Ejecución Recomendado

1. **Primero**: Web API (puerto 7121)
2. **Segundo**: MVC (puerto 7092)
3. **Tercero**: SPA (puerto 5173)

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

#### QRLink
```csharp
public class QRLink
{
    public int Id { get; set; }
    public string Hash { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaExpiracion { get; set; }
    public int? ReservaId { get; set; }
    public Reserva? Reserva { get; set; }
}
```

### Comandos de Migración

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Eliminar migración pendiente
dotnet ef migrations remove

# Ver historial de migraciones
dotnet ef migrations list
```

## 🔐 Características de Seguridad

### Autenticación
- **Hash + Salt**: Contraseñas seguras con ASP.NET Identity
- **JWT Tokens**: Autenticación stateless para la API (duración: 60 minutos)
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
    var url = $"https://localhost:7092/QR/{hash}";
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
- [x] Documentación Swagger

#### SPA React
- [x] Interfaz de usuario moderna
- [x] Consumo de API
- [x] Ejecución en cliente
- [x] Gestión de tokens JWT
- [x] Componentes reutilizables

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
6. **API**: Probar endpoints con Swagger
7. **SPA**: Navegar por la interfaz React

### 🌐 URLs de Prueba
- **MVC**: `https://localhost:7092`
- **API**: `https://localhost:7121`
- **API Swagger**: `https://localhost:7121/swagger`
- **SPA**: `http://localhost:5173`

### 📝 Endpoints Principales (API)

#### Autenticación
- `POST /api/auth/register` - Registrar usuario
- `POST /api/auth/login` - Iniciar sesión
- `GET /api/auth/profile` - Obtener perfil (requiere JWT)

#### Reservas
- `GET /api/reservas` - Listar reservas (requiere JWT)
- `GET /api/reservas/{id}` - Obtener reserva (requiere JWT)
- `POST /api/reservas` - Crear reserva (requiere JWT)
- `PUT /api/reservas/{id}` - Actualizar reserva (requiere JWT)
- `DELETE /api/reservas/{id}` - Eliminar reserva (requiere JWT)

#### QR
- `POST /api/qr/generate/{reservaId}` - Generar QR (requiere JWT)
- `GET /api/qr/access/{hash}` - Acceder por QR

## 🚨 Troubleshooting

### Problemas Comunes

#### 1. Error de migraciones
```bash
# Solución: Eliminar base de datos y recrear
rm ReservasApp.MVC/reservas.db
cd ReservasApp.WebAPI
dotnet ef database update
```

#### 2. Puerto ocupado
```bash
# Cambiar puerto en launchSettings.json o usar:
dotnet run --urls "https://localhost:9999"
```

#### 3. Error CORS en SPA
```bash
# Verificar que la API esté ejecutándose
# Verificar variable VITE_API_URL en .env
```

#### 4. JWT Token expirado
```bash
# Token expira en 60 minutos
# Hacer login nuevamente en la SPA
```

#### 5. Error de conexión a base de datos
```bash
# Verificar que ambos proyectos apunten a la misma BD
# MVC: "Data Source=reservas.db"
# API: "Data Source=../ReservasApp.MVC/reservas.db"
```

## 📂 Estructura de Carpetas

```
.net/
├── 📁 ReservasApp.MVC/
│   ├── Controllers/          # Controladores MVC
│   ├── Models/              # Modelos de datos
│   ├── Views/               # Vistas Razor
│   ├── Services/            # Servicios de negocio
│   ├── Data/                # Contexto de BD
│   └── reservas.db          # Base de datos SQLite
├── 📁 ReservasApp.WebAPI/
│   ├── Controllers/         # Controladores API
│   ├── Models/             # Modelos de datos
│   ├── Services/           # Servicios JWT, AutoMapper
│   ├── Repositories/       # Patrón Repository
│   ├── DTOs/               # Data Transfer Objects
│   └── Data/               # Contexto de BD
├── 📁 ReservaApp.SPA/
│   ├── src/
│   │   ├── components/     # Componentes React
│   │   ├── services/       # Servicios API
│   │   ├── pages/          # Páginas/rutas
│   │   ├── contexts/       # Context API
│   │   └── types/          # Tipos TypeScript
│   ├── public/             # Archivos estáticos
│   └── .env                # Variables de entorno
└── 📄 README.md            # Este archivo
```

## 🔐 Seguridad en Producción

### Cambios Necesarios

1. **Cambiar JWT Secret** en `appsettings.json`
2. **Configurar HTTPS** con certificados válidos
3. **Configurar SMTP** real para emails
4. **Habilitar confirmación de email**: `RequireConfirmedEmail = true`
5. **Configurar CORS** específico para dominio de producción
6. **Usar base de datos real** (SQL Server, PostgreSQL)

### Variables de Entorno Recomendadas

```env
# Producción
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=...
JwtSettings__Secret=TU_CLAVE_SUPER_SEGURA
Email__SMTP__Username=tu-email@empresa.com
Email__SMTP__Password=tu-password-seguro
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crear rama feature (`git checkout -b feature/NuevaFuncionalidad`)
3. Commit cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/NuevaFuncionalidad`)
5. Abrir Pull Request

## 📜 Scripts Útiles

### Para desarrollo
```bash
# Ejecutar todos los proyectos (requiere 3 terminales)
# Terminal 1 - API
cd ReservasApp.WebAPI && dotnet run

# Terminal 2 - MVC  
cd ReservasApp.MVC && dotnet run

# Terminal 3 - SPA
cd ReservaApp.SPA && npm run dev
```

### Para producción
```bash
# Publicar API
cd ReservasApp.WebAPI
dotnet publish -c Release -o ./publish

# Publicar MVC
cd ReservasApp.MVC
dotnet publish -c Release -o ./publish

# Construir SPA
cd ReservaApp.SPA
npm run build
```

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para detalles.

## 👥 Equipo de Desarrollo

- **Desarrollador**: [Tu Nombre]
- **Materia**: Programación 3
- **Universidad**: [Tu Universidad]
- **Año**: 2025

---

💡 **Nota**: Este README contiene todas las instrucciones necesarias para clonar y ejecutar el proyecto. Si encontrás algún problema, revisá la sección de **Troubleshooting** o creá un issue en el repositorio.

🚀 **¡Listo para usar!** Seguí los pasos en orden y tendrás el sistema completo funcionando.
