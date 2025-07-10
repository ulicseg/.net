# 🚀 Scripts de Desarrollo - Sistema de Reservas

Este directorio contiene scripts para facilitar el desarrollo del sistema de reservas.

## 📋 Archivos Disponibles

### 🎯 Scripts Principales
- **`start-all.ps1`** - Script PowerShell para iniciar todo el sistema
- **`start-all.bat`** - Script Batch para iniciar todo el sistema 
- **`stop-all.ps1`** - Script PowerShell para detener todo el sistema

### ⚙️ Configuración VS Code
- **`.vscode/tasks.json`** - Tareas de VS Code para desarrollo

## 🚀 Uso Rápido

### Opción 1: Script PowerShell (Recomendado)
```powershell
# Iniciar todo el sistema
.\start-all.ps1

# Detener todo el sistema
.\stop-all.ps1
```

### Opción 2: Script Batch
```cmd
# Iniciar todo el sistema
start-all.bat
```

### Opción 3: VS Code (Ctrl+Shift+P)
1. Abrir Command Palette (`Ctrl+Shift+P`)
2. Escribir "Tasks: Run Task"
3. Seleccionar:
   - `🚀 Iniciar Todo el Sistema`
   - `🛑 Detener Todo el Sistema`
   - `🔌 Iniciar Solo WebAPI`
   - `🌐 Iniciar Solo MVC`
   - `⚛️ Iniciar Solo SPA`

## 🌐 URLs de Acceso

Después de ejecutar los scripts, las aplicaciones estarán disponibles en:

| Aplicación | URL | Descripción |
|------------|-----|-------------|
| **WebAPI** | https://localhost:7121/swagger | Documentación Swagger de la API |
| **MVC** | https://localhost:7092 | Aplicación web principal |
| **SPA** | http://localhost:5173 | Single Page Application (React) |

## ⚡ Orden de Inicio

Los scripts inician las aplicaciones en el orden correcto:

1. **WebAPI** (puerto 7121) - Primero, porque es la base de datos
2. **MVC** (puerto 7092) - Segundo, aplicación principal
3. **SPA** (puerto 5173) - Tercero, consume la API

## 🔧 Solución de Problemas

### Puerto Ocupado
Si algún puerto está ocupado, puedes:

```powershell
# Ver qué proceso usa el puerto
Get-NetTCPConnection -LocalPort 7121 | Select-Object OwningProcess
Get-Process -Id [PID_DEL_PROCESO]

# Detener el proceso
Stop-Process -Id [PID_DEL_PROCESO] -Force
```

### Permisos de PowerShell
Si PowerShell no ejecuta scripts:

```powershell
# Permitir ejecución temporal
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process

# O ejecutar directamente
powershell -ExecutionPolicy Bypass -File .\start-all.ps1
```

### Error de NPM
Si la SPA no inicia:

```cmd
cd ReservaApp.SPA
npm install
npm run dev
```

## 🎯 Desarrollo Recomendado

1. **Primera vez**: Ejecuta `start-all.ps1` para verificar que todo funciona
2. **Desarrollo normal**: Usa las tareas de VS Code para mayor control
3. **Solo API**: Usa "Iniciar Solo WebAPI" si solo trabajas en backend
4. **Solo Frontend**: Usa "Iniciar Solo SPA" si solo trabajas en React

## 📝 Notas

- Los scripts verifican automáticamente si los puertos están disponibles
- Se abren ventanas separadas para cada aplicación para facilitar el debugging
- Todos los logs son visibles en sus respectivas ventanas
- Para detener, simplemente cierra las ventanas o usa `stop-all.ps1`
