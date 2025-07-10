#!/usr/bin/env pwsh

# Script para iniciar todo el entorno de desarrollo
# Sistema de Reservas - Trabajo Practico

Write-Host "Sistema de Reservas - Iniciando entorno completo" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Cyan

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "Practico.sln")) {
    Write-Host "ERROR: No se encuentra Practico.sln" -ForegroundColor Red
    Write-Host "Ejecutar desde el directorio raiz del proyecto" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "1. Iniciando WebAPI en puerto 5284..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; dotnet run --project ReservasApp.WebAPI --launch-profile http"
Start-Sleep -Seconds 2

Write-Host "2. Iniciando MVC en puerto 7092..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; dotnet run --project ReservasApp.MVC --launch-profile https"
Start-Sleep -Seconds 2

Write-Host "3. Iniciando SPA en puerto 5173..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD/ReservaApp.SPA'; npm run dev"
Start-Sleep -Seconds 2

Write-Host ""
Write-Host "*** Todas las aplicaciones estan iniciando..." -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "URLs de acceso:"
Write-Host "   WebAPI:  http://localhost:5284/swagger/index.html" -ForegroundColor Blue
Write-Host "   MVC:     https://localhost:7092" -ForegroundColor Blue  
Write-Host "   SPA:     http://localhost:5173" -ForegroundColor Blue
Write-Host ""
Write-Host "*** Espera unos segundos a que todas las aplicaciones terminen de cargar..." -ForegroundColor Yellow
Write-Host "*** Para detener: Cierra las ventanas de PowerShell correspondientes" -ForegroundColor Gray
Write-Host ""
Write-Host "Listo para testing! Presiona ENTER para continuar..."
Read-Host
