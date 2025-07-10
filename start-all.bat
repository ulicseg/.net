@echo off
title Sistema de Reservas - Iniciador
color 0B

echo.
echo =============================================
echo  🚀 Sistema de Reservas - Iniciador
echo =============================================
echo.

echo 📋 Iniciando aplicaciones...
echo.

echo 1️⃣ Iniciando WebAPI (puerto 7121)...
start "WebAPI" cmd /k "cd /d %~dp0ReservasApp.WebAPI && echo 🔌 WebAPI iniciando... && dotnet run --launch-profile https"

echo    ⏳ Esperando 3 segundos...
timeout /t 3 /nobreak >nul

echo 2️⃣ Iniciando MVC (puerto 7092)...
start "MVC" cmd /k "cd /d %~dp0ReservasApp.MVC && echo 🌐 MVC iniciando... && dotnet run --launch-profile https"

echo    ⏳ Esperando 2 segundos...
timeout /t 2 /nobreak >nul

echo 3️⃣ Iniciando SPA (puerto 5173)...
start "SPA" cmd /k "cd /d %~dp0ReservaApp.SPA && echo ⚛️ SPA iniciando... && npm run dev"

echo.
echo ✅ Todas las aplicaciones están iniciando!
echo.
echo 📌 URLs de acceso:
echo    🔌 WebAPI:  https://localhost:7121/swagger
echo    🌐 MVC:     https://localhost:7092
echo    ⚛️ SPA:     http://localhost:5173
echo.
echo ⏰ Espera unos segundos a que carguen completamente...
echo 🛑 Para detener: Cierra las ventanas correspondientes
echo.
pause
