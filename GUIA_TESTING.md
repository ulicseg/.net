# 🧪 GUÍA DE TESTING COMPLETA - Sistema de Reservas

## 🎯 ENTORNO ✅ FUNCIONAL
- ✅ **WebAPI**: http://localhost:5284/swagger (funcionando perfectamente)
- ✅ **MVC**: https://localhost:7092 (diseño moderno implementado y funcional)
- ✅ **SPA**: http://localhost:5173 (ejecutándose correctamente)

🔧 **SCRIPT DE INICIO UNIFICADO**:
- ✅ **start-all.ps1** - Script único optimizado y funcional
- ✅ **stop-all.ps1** - Script para detener todos los servicios
- 🗑️ **Scripts obsoletos eliminados** - Solo mantenemos los que funcionan

---

## 📋 PLAN DE TESTING COMPLETO

### **FASE 1: TESTING BÁSICO DE APLICACIONES** 🔧

#### 1.1 Verificar WebAPI (Swagger)
- [x] Abrir http://localhost:5284/swagger
- [X] Verificar que aparecen todos los controllers:
  - `Auth` (Login, Register, ForgotPassword, ResetPassword)
  - `Reservas` (CRUD completo)
  - `QR` (Generate, Access)
- [X] Probar endpoint GET `/api/reservas` (debería dar 401 Unauthorized)

#### 1.2 Verificar MVC ✅ COMPLETADO CON ESTILOS MODERNOS
- [x] Abrir https://localhost:7092
- [x] ✅ Verificar que carga la página de inicio con diseño moderno
- [x] ✅ Hero section con gradientes, animaciones y efectos premium
- [x] ✅ Navegación profesional con iconos y hover effects
- [x] ✅ Botones premium con efectos shimmer y transitions
- [x] ✅ Tarjeta médica visual con elementos flotantes animados
- [x] ✅ Verificar navegación (Home, Login, Register)
- [x] ✅ Comprobar que no hay errores en consola

#### 1.3 Verificar SPA
- [x] Abrir http://localhost:5173
- [x] Verificar que carga la página de login
- [x] Verificar que los estilos Tailwind están aplicados
- [x] Comprobar que no hay errores en consola

---

### **FASE 2: TESTING DE AUTENTICACIÓN** 👥

#### 2.1 Registro de Usuarios (MVC)
**Objetivo**: Usar el usuario admin automático y crear un usuario cliente

**Usuario 1 - Admin (Ya existe automáticamente)**
- ✅ Email: `admin@reservas.com`
- ✅ Password: `Admin123!`
- ✅ Rol: "Admin" 
- **Este usuario se crea automáticamente al iniciar la aplicación**

**Usuario 2 - Cliente (Registro manual)**
- [x] Cerrar sesión si está logueado
- [x] Ir a https://localhost:7092/Account/Register
- [x] Registrar segundo usuario:
  - Nombre: `Cliente`
  - Apellido: `Demo`
  - Email: `cliente@test.com`
  - Password: `Cliente123!`
- [x] Verificar confirmación de registro
- [x] Automáticamente obtiene rol "Cliente"

#### 2.2 Login/Logout (MVC) ✅ COMPLETADO
- [x] ✅ Probar login con admin@reservas.com - **FUNCIONA**
- [x] ✅ Verificar acceso al dashboard - **FUNCIONA**
- [x] ✅ Probar logout - **FUNCIONA**
- [x] ✅ Probar login con cliente@test.com - **FUNCIONA**
- [x] ✅ Probar credenciales incorrectas - **MANEJA ERRORES CORRECTAMENTE**
- 📝 **Nota**: Ambos roles ven las mismas vistas (diferenciación se implementará después del testing)

#### 2.3 Autenticación SPA ✅ COMPLETADO
- [x] ✅ Abrir http://localhost:5173
- [x] ✅ Probar login con admin@reservas.com en la SPA - **FUNCIONA**
- [x] ✅ Verificar que se guarda el token JWT - ✅ **CONFIRMADO: Token JWT válido guardado en localStorage**
- [x] ✅ Verificar acceso al dashboard SPA - **FUNCIONA**
- [x] ✅ Probar logout en SPA - **FUNCIONA**
- [x] ✅ Probar login con cliente@test.com en la SPA - **FUNCIONA**
- [x] ✅ Verificar que funciona igual que admin (mismas vistas por ahora) - **CONFIRMADO**
- 📝 **Corrección aplicada**: URL de API corregida de puerto 7121 a 5284O ✅ FUNCIONAL
- ✅ **WebAPI**: http://localhost:5284/swagger (funcionando perfectamente)
- ✅ **MVC**: https://localhost:7092 (diseño moderno implementado y funcional)
- ✅ **SPA**: http://localhost:5173 (ejecutándose correctamente)

---

## 📋 PLAN DE TESTING COMPLETO

### **FASE 1: TESTING BÁSICO DE APLICACIONES** 🔧

#### 1.1 Verificar WebAPI (Swagger)
- [x] Abrir http://localhost:5284/swagger
- [X] Verificar que aparecen todos los controllers:
  - `Auth` (Login, Register, ForgotPassword, ResetPassword)
  - `Reservas` (CRUD completo)
  - `QR` (Generate, Access)
- [X] Probar endpoint GET `/api/reservas` (debería dar 401 Unauthorized)

#### 1.2 Verificar MVC ✅ COMPLETADO CON ESTILOS MODERNOS
- [x] Abrir https://localhost:7092
- [x] ✅ Verificar que carga la página de inicio con diseño moderno
- [x] ✅ Hero section con gradientes, animaciones y efectos premium
- [x] ✅ Navegación profesional con iconos y hover effects
- [x] ✅ Botones premium con efectos shimmer y transitions
- [x] ✅ Tarjeta médica visual con elementos flotantes animados
- [x] ✅ Verificar navegación (Home, Login, Register)
- [x] ✅ Comprobar que no hay errores en consola

#### 1.3 Verificar SPA
- [x] Abrir http://localhost:5173
- [x] Verificar que carga la página de login
- [x] Verificar que los estilos Tailwind están aplicados
- [x] Comprobar que no hay errores en consola

---

### **FASE 2: TESTING DE AUTENTICACIÓN** 👥

#### 2.1 Registro de Usuarios (MVC)
**Objetivo**: Usar el usuario admin automático y crear un usuario cliente

**Usuario 1 - Admin (Ya existe automáticamente)**
- ✅ Email: `admin@reservas.com`
- ✅ Password: `Admin123!`
- ✅ Rol: "Admin" 
- **Este usuario se crea automáticamente al iniciar la aplicación**

**Usuario 2 - Cliente (Registro manual)**
- [x] Cerrar sesión si está logueado
- [x] Ir a https://localhost:7092/Account/Register
- [x] Registrar segundo usuario:
  - Nombre: `Cliente`
  - Apellido: `Demo`
  - Email: `cliente@test.com`
  - Password: `Cliente123!`
- [x] Verificar confirmación de registro
- [x] Automáticamente obtiene rol "Cliente"

#### 2.2 Login/Logout (MVC) ✅ COMPLETADO
- [x] ✅ Probar login con admin@reservas.com - **FUNCIONA**
- [x] ✅ Verificar acceso al dashboard - **FUNCIONA**
- [x] ✅ Probar logout - **FUNCIONA**
- [x] ✅ Probar login con cliente@test.com - **FUNCIONA**
- [x] ✅ Probar credenciales incorrectas - **MANEJA ERRORES CORRECTAMENTE**
- 📝 **Nota**: Ambos roles ven las mismas vistas (diferenciación se implementará después del testing)

#### 2.3 Autenticación SPA ✅ COMPLETADO
- [x] ✅ Abrir http://localhost:5173 - **FUNCIONA**
- [x] ✅ Probar login con admin@reservas.com en la SPA - **FUNCIONA**
- [x] ✅ Verificar que se guarda el token JWT - **FUNCIONA**
- [x] ✅ Verificar acceso al dashboard SPA - **FUNCIONA**
- [x] ✅ Probar logout en SPA - **FUNCIONA**
- [x] ✅ Probar login con cliente@test.com en la SPA - **FUNCIONA**
- [x] ✅ Verificar que funciona igual que admin (mismas vistas por ahora) - **CONFIRMADO**

---

### **FASE 3: TESTING DE RESERVAS (CRUD COMPLETO)** 📅

#### 3.1 Crear Reservas (MVC) 🔄 EN PROGRESO
**Con usuario admin@reservas.com logueado** ✅

**Reserva 1**
- [x] ✅ Login como admin@reservas.com - **FUNCIONA**
- [x] ✅ Acceso al dashboard - **FUNCIONA**
- [x] ✅ Encontrar "Nueva Reserva" desde el dashboard - **FUNCIONA**
- [x] 🎯 **SIGUIENTE PASO**: Crear primera reserva con estos datos:
  - Tipo Servicio: `Consulta Médica`
  - Fecha: `Mañana` (o fecha futura)
  - Descripción: `Consulta general de rutina`
- [x] Verificar que se crea correctamente
- [x] Verificar que aparece en el listado

**Reserva 2**
- [x] Crear segunda reserva:
  - Tipo Servicio: `Laboratorio`
  - Fecha: `Próxima semana`
  - Descripción: `Análisis de sangre completo`

**Reserva 3**
- [x] Crear tercera reserva:
  - Tipo Servicio: `Radiología`
  - Fecha: `En 3 días`
  - Descripción: `Radiografía de tórax`

#### 3.2 Ver y Gestionar Reservas (MVC) ✅ COMPLETADO
- [x] ✅ Ir al listado de reservas - **FUNCIONA**
- [x] ✅ Verificar que aparecen las 3 reservas - **FUNCIONA**
- [x] ✅ Ver detalles de una reserva - **FUNCIONA**
- [x] ✅ Editar una reserva - **FUNCIONA**
- [x] ✅ Eliminar una reserva - **FUNCIONA**
- [x] ❌ Verificar filtros si existen - **FALTA IMPLEMENTAR**

✅ **CRUD MVC 100% COMPLETO**:
- ✅ Create (Crear) - Funciona
- ✅ Read (Listar + Detalles) - Funciona  
- ✅ Update (Editar) - Funciona
- ✅ Delete (Eliminar) - Funciona

🎯 **SIGUIENTE FASE**: Testing de SPA y generación de QR

#### 3.3 CRUD en SPA
**Con usuario cliente@test.com**

- [x] Login en SPA con cliente@test.com
- [x] Verificar dashboard vacío (sin reservas)
- [x] Crear nueva reserva desde SPA
- [x] Verificar que aparece en la tabla
- [x] Probar editar reserva en SPA
- [x] Verificar estadísticas en dashboard
- [x] Probar eliminar reserva

---

### **FASE 4: TESTING DE CÓDIGOS QR** 🔍

#### 4.1 Generar QR desde SPA
**Con usuario que tiene reservas**

- [x] Ir al dashboard de la SPA
- [x] Buscar el botón púrpura QR en una reserva
- [x] Hacer clic en "Generar QR"
- [x] Verificar que aparece el modal QR
- [x] Verificar que se genera la imagen QR
- [x] Probar copiar URL al portapapeles
- [x] Probar descargar QR como PNG
- [x] Copiar la URL del QR para testing

#### 4.2 Acceso por QR ✅ COMPLETADO Y MEJORADO
- [x] ✅ Tomar la URL copiada del QR - **FUNCIONA**
- [x] ✅ Abrir en nueva pestaña (formato: /qr/HASH) - **FUNCIONA**
- [x] ✅ Verificar que carga la página de acceso QR - **FUNCIONA**
- [x] ✅ Verificar que muestra información de la reserva - **FUNCIONA**
- [x] ✅ Verificar el mensaje "Información Exclusiva por QR" - **"Acceso autorizado via código QR"**
- [x] ✅ Probar imprimir página - **FUNCIONA**
- [x] ✅ Verificar que funciona sin estar logueado - **FUNCIONA EN INCÓGNITO**

**🎯 NUEVA FUNCIONALIDAD - Vista HTML Mejorada**:
- [x] ✅ Endpoint `/api/qr/view/{hash}` - Vista HTML moderna y profesional
- [x] ✅ Diseño responsivo con gradientes y efectos visuales
- [x] ✅ Botones de imprimir y cerrar integrados
- [x] ✅ Auto-close por seguridad después de 30 minutos
- [x] ✅ Información formateada elegantemente con iconos
- [x] ✅ Manejo de errores con páginas HTML estilizadas

📊 **ENDPOINTS QR DISPONIBLES**:
- **JSON**: `/api/qr/access/{hash}` - Devuelve datos en formato JSON (para integraciones)
- **HTML**: `/api/qr/view/{hash}` - Devuelve página web moderna y visual ⭐ **DEFAULT**

📊 **DATOS QR OBTENIDOS (JSON)**:
```json
{
  "reservaId": 6,
  "tipoServicio": "ConsultaMedica", 
  "fechaReserva": "2025-07-27T11:31:00",
  "estado": "Activa",
  "descripcion": "Me duele",
  "clienteNombre": "Cliente Clientee",
  "fechaAcceso": "2025-07-10T14:33:02.5009889Z",
  "mensajeAcceso": "Acceso autorizado via código QR"
}
```

✅ **URLs DE PRUEBA**: 
- JSON: `https://localhost:5284/api/qr/access/HASH_VALIDO`
- HTML: `https://localhost:5284/api/qr/view/HASH_VALIDO` ⭐ **NUEVO**

#### 4.3 QR Expirado/Inválido ✅ COMPLETADO
- [x] ✅ Intentar acceder con URL QR malformada - **ERROR: "Enlace QR no válido"**
- [x] ✅ Verificar mensaje de error apropiado - **FUNCIONA CORRECTAMENTE**
- [x] ✅ Esperar 10 minutos y probar QR expirado - **EXPIRA EN 10 MINUTOS**

#### 4.4 Testing de Vista HTML QR ⭐ NUEVA FUNCIONALIDAD IMPLEMENTADA
**🎯 OBJETIVO**: Probar la nueva vista HTML mejorada para QR

**✅ CORRECCIÓN APLICADA**: URLs QR ahora apuntan directamente al endpoint visual:
- **Antes**: `http://localhost:5284/api/qr/access/{hash}` (JSON)
- **Ahora**: `http://localhost:5284/api/qr/view/{hash}` (HTML visual) ⭐

**Con QR válido**:
- [x] 🔄 **TESTING ACTIVO**: Generar nuevo QR desde SPA
- [x] Copiar hash del QR generado
- [x] Verificar que la URL generada sea: `/api/qr/view/{hash}` ⭐
- [x] Probar endpoint HTML: `http://localhost:5284/api/qr/view/{hash}` ⭐
- [x] Verificar diseño moderno y responsivo
- [x] Probar botón "Imprimir" 
- [x] Probar botón "Cerrar"
- [x] Verificar que funciona en incógnito
- [x] ✅ Confirmar expiración de 10 minutos

**Con QR inválido**:
- [x] ✅ Probar hash inexistente: `http://localhost:5284/api/qr/view/hash_falso` - **PÁGINA DE ERROR ELEGANTE**
- [x] ✅ Verificar página de error elegante - **CONFIRMADO**
- [x] ✅ Verificar mensaje de error claro - **"Enlace QR no válido"**
- [x] Probar botón "Cerrar" en página de error

**Comparación de formatos**:
- [x] Verificar que JSON endpoint sigue funcionando: `/api/qr/access/{hash}`
- [x] Verificar que HTML es más visual y fácil de usar: `/api/qr/view/{hash}`
- [x] Confirmar que ambos endpoints manejan la misma validación
- [x] ✅ **QR por defecto apunta a vista HTML** ⭐

📋 **SEGURIDAD QR VERIFICADA**:
- ✅ Hash inválido → Error apropiado (JSON + HTML)
- ✅ URL malformada → Error apropiado (JSON + HTML)
- ✅ Expiración temporal → 10 minutos
- ✅ JSON con datos correctos → Funciona
- ✅ HTML con vista moderna → Implementado ⭐

🎯 **MEJORA COMPLETADA**: ✅ Vista visual moderna implementada para QR

---

### **FASE 5: TESTING DE API ENDPOINTS** 🔧

#### 5.1 Endpoints de Autenticación
**Usando Swagger (http://localhost:5284/swagger)**

- [x] Probar POST `/api/auth/register` con datos válidos
- [x] Probar POST `/api/auth/login` con credenciales correctas
- [x] Copiar el token JWT devuelto
- [x] Usar "Authorize" en Swagger con `Bearer TOKEN`

#### 5.2 Endpoints de Reservas
**Con token autorizado**

- [x] GET `/api/reservas` - Listar reservas
- [x] POST `/api/reservas` - Crear nueva reserva
- [x] GET `/api/reservas/{id}` - Ver reserva específica
- [x] PUT `/api/reservas/{id}` - Actualizar reserva
- [x] DELETE `/api/reservas/{id}` - Eliminar reserva

#### 5.3 Endpoints de QR
- [x] POST `/api/qr/generate/{reservaId}` - Generar QR
- [x] GET `/api/qr/access/{hash}` - Acceder por QR (sin auth)

#### 5.4 Endpoints de Recuperación de Contraseña ✅ COMPLETADO
**🔧 Configuración de Email**:
- ✅ Email configurado: `sistemagestorlegislativo@gmail.com`
- ✅ Contraseña de aplicación configurada
- ✅ SMTP Gmail habilitado (puerto 587)
- ✅ Modo simulación desactivado

**Testing POST `/api/auth/forgot-password`**:
- [x] ✅ Con email válido existente (`cliente@test.com`) - **RESPUESTA 200 OK**
- [x] ✅ Con email válido no registrado (`ulicseg@gmail.com`) - **RESPUESTA 200 OK** 
- [x] ✅ Verificar respuesta exitosa - **"Si el email existe, recibirás un enlace de recuperación"**
- [x] ✅ Verificar que no revela si el email existe (seguridad) - **CONFIRMADO**

**Testing POST `/api/auth/reset-password`**:
- [x] ✅ Con token falso válido - **ERROR 400: "Token inválido o expirado"**
- [x] ✅ Con email inexistente - **ERROR 400: "Token inválido o expirado"**
- [x] ✅ Verificar manejo de errores - **FUNCIONA CORRECTAMENTE**
- [x] ✅ Verificar validación de ConfirmPassword - **REQUERIDO Y VALIDADO**

**🎯 NUEVA FUNCIONALIDAD - SPA Reset Password**:
- [x] ✅ Componente `ResetPassword.tsx` creado - **IMPLEMENTADO**
- [x] ✅ Ruta `/reset-password` agregada al router - **FUNCIONAL**
- [x] ✅ Enlace de email corregido para usar HTTP (no HTTPS) - **CORREGIDO ERR_CONNECTION_REFUSED**
- [x] ✅ Formulario con validación de contraseñas - **IMPLEMENTADO**
- [x] ✅ Integración con `authService.resetPassword()` - **IMPLEMENTADO**
- [x] ✅ Manejo de errores y estados de carga - **IMPLEMENTADO**
- [x] ✅ Redirección automática después del reset exitoso - **IMPLEMENTADO**

**🆕 NUEVA FUNCIONALIDAD - SPA Forgot Password**:
- [x] ✅ Componente `ForgotPassword.tsx` creado - **IMPLEMENTADO**
- [x] ✅ Ruta `/forgot-password` agregada al router - **FUNCIONAL**
- [x] ✅ Enlace funcional desde página de login - **PROBLEMA RESUELTO**
- [x] ✅ Formulario con validación de email - **IMPLEMENTADO**
- [x] ✅ Integración con `authService.forgotPassword()` - **IMPLEMENTADO**
- [x] ✅ Página de confirmación después del envío - **IMPLEMENTADO**
- [x] ✅ Manejo de errores y estados de carga - **IMPLEMENTADO**

**🎯 FLUJO COMPLETO OPTIMIZADO - Reset Password**:
- [x] ✅ Método `authService.resetPassword()` agregado - **FUNCIÓN IMPLEMENTADA**
- [x] ✅ Componente `ResetPassword.tsx` rediseñado - **DISEÑO MODERNO**
- [x] ✅ Validación robusta de contraseñas - **MÍNIMO 6 CARACTERES**
- [x] ✅ Mostrar/ocultar contraseñas con iconos - **UX MEJORADA**
- [x] ✅ Página de éxito con auto-redirección - **3 SEGUNDOS**
- [x] ✅ Página de error elegante si faltan parámetros - **MANEJO COMPLETO**
- [x] ✅ Estados de carga y validación en tiempo real - **INTERACTIVO**
- [x] ✅ Diseño responsive y moderno - **GRADIENTES Y SOMBRAS**

**📊 Estructura de Request**:
```json
// forgot-password
{ "email": "cliente@test.com" }

// reset-password  
{
  "email": "cliente@test.com",
  "token": "token_del_email",
  "newPassword": "NuevaPassword123!",
  "confirmPassword": "NuevaPassword123!"
}
```

**🔒 Seguridad Verificada**:
- ✅ No revela existencia de emails
- ✅ Tokens de reset seguros
- ✅ Validación de contraseñas
- ✅ Manejo de errores apropiado
- ✅ Enlace funciona correctamente (HTTP, no HTTPS)

**🌐 URLs del Flujo de Recuperación**:
- ✅ **Inicio**: `http://localhost:5173/login` → Click "¿Olvidaste tu contraseña?"
- ✅ **Forgot Password**: `http://localhost:5173/forgot-password` → Ingresar email
- ✅ **Confirmación**: Página de éxito → "Email enviado"
- ✅ **Email Link**: `http://localhost:5173/reset-password?email={email}&token={token}`
- ✅ **Reset Password**: Formulario para nueva contraseña → Redirección a login
- ✅ **Funciona sin estar logueado**
- ✅ **Valida parámetros requeridos**
- ✅ **Error claro si faltan parámetros**

---

### **FASE 6: TESTING DE INTEGRACIÓN** 🔗

🎯 **FLUJOS COMPLETADOS**:
- ✅ Autenticación (Login/Register/Logout) - MVC y SPA
- ✅ CRUD de Reservas - MVC y SPA 
- ✅ Generación y acceso QR - Mejorado con vista HTML
- ✅ Recuperación de contraseña - Con SPA integrado
- 🔄 **AHORA**: Verificar integración entre aplicaciones

#### 6.1 Sincronización entre Aplicaciones
- [ ] Crear reserva en MVC
- [ ] Verificar que aparece en SPA (refrescar)
- [ ] Crear reserva en SPA
- [ ] Verificar que aparece en MVC
- [ ] Verificar que los datos son consistentes

#### 6.2 Testing de Estados
- [ ] Cambiar estado de reserva en MVC
- [ ] Verificar cambio en SPA
- [ ] Verificar estadísticas actualizadas

#### 6.3 Testing de Roles y Permisos
- [ ] Verificar que cada usuario solo ve sus reservas
- [ ] Probar acceso no autorizado a reservas ajenas
- [ ] Verificar que QR solo lo puede generar el dueño

---

### **FASE 7: TESTING DE UX/UI** 🎨

#### 7.1 Responsividad
- [ ] Probar en diferentes tamaños de pantalla
- [ ] Verificar mobile en SPA
- [ ] Verificar navegación móvil

#### 7.2 Usabilidad
- [ ] Verificar mensajes de error claros
- [ ] Verificar mensajes de confirmación
- [ ] Verificar loading states
- [ ] Verificar formularios intuitivos

#### 7.3 Performance
- [ ] Verificar tiempos de carga
- [ ] Verificar que no hay memory leaks
- [ ] Verificar que las imágenes QR se generan rápido

---

## ✅ CHECKLIST FINAL

### **Funcionalidades Core**
- [x] ✅ Registro y login funcionan en ambas apps
- [x] ✅ CRUD completo de reservas funciona
- [x] ✅ Generación y acceso QR funciona (con vista HTML mejorada)
- [x] ✅ Recuperación de contraseña funciona completamente (backend + SPA)
- [ ] 🔄 Integración entre MVC, API y SPA funciona

### **Seguridad**
- [x] ✅ Solo usuarios autenticados ven sus reservas
- [x] ✅ QR tiene acceso exclusivo y temporal (10 minutos)
- [x] ✅ Tokens JWT funcionan correctamente
- [x] ✅ Endpoints protegidos rechazan acceso no autorizado
- [x] ✅ Reset password con enlaces seguros funciona

### **Calidad**
- [x] ✅ No hay errores en consola
- [x] ✅ No hay warnings en builds críticos
- [x] ✅ Todas las páginas cargan correctamente
- [x] ✅ UX es intuitiva y profesional
- [ ] 🔄 Testing de responsividad móvil

---

## 🚀 RESULTADOS ESPERADOS

Al completar este testing, deberías poder decir:

> ✅ **"El sistema funciona completamente"**
> - Autenticación robusta ✅
> - CRUD de reservas ✅  
> - Generación de QR ✅
> - Acceso exclusivo por QR ✅
> - Integración perfecta entre apps ✅
> - API completamente funcional ✅

---

**🎯 ¡Comenzemos el testing paso a paso!**
