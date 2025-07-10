# Script para iniciar todo el sistema de reservas
# Ejecutar como: .\start-all.ps1

Write-Host "🚀 Iniciando Sistema de Reservas..." -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan

# Función para verificar si un puerto está ocupado
function Test-Port {
    param($Port)
    try {
        $connection = New-Object System.Net.Sockets.TcpClient
        $connection.Connect("localhost", $Port)
        $connection.Close()
        return $true
    }
    catch {
        return $false
    }
}

# Verificar puertos disponibles
Write-Host "🔍 Verificando puertos..." -ForegroundColor Yellow

$ports = @{
    "7121" = "WebAPI (HTTPS)"
    "7092" = "MVC (HTTPS)" 
    "5173" = "SPA (Vite)"
}

foreach ($port in $ports.Keys) {
    if (Test-Port $port) {
        Write-Host "❌ Puerto $port ($($ports[$port])) está ocupado" -ForegroundColor Red
        Write-Host "   Ejecuta: Get-Process -Id (Get-NetTCPConnection -LocalPort $port).OwningProcess" -ForegroundColor Gray
    } else {
        Write-Host "✅ Puerto $port ($($ports[$port])) disponible" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "🎯 Iniciando aplicaciones en orden..." -ForegroundColor Cyan

# 1. Iniciar WebAPI (debe ser primero)
Write-Host "1️⃣ Iniciando WebAPI..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\ReservasApp.WebAPI'; Write-Host '🔌 WebAPI iniciando en https://localhost:7121' -ForegroundColor Green; dotnet run --launch-profile https"

# Esperar 3 segundos
Start-Sleep -Seconds 3

# 2. Iniciar MVC
Write-Host "2️⃣ Iniciando MVC..." -ForegroundColor Yellow  
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\ReservasApp.MVC'; Write-Host '🌐 MVC iniciando en https://localhost:7092' -ForegroundColor Green; dotnet run --launch-profile https"

# Esperar 2 segundos
Start-Sleep -Seconds 2

# 3. Iniciar SPA
Write-Host "3️⃣ Iniciando SPA..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\ReservaApp.SPA'; Write-Host '⚛️ SPA iniciando en http://localhost:5173' -ForegroundColor Green; npm run dev"

Write-Host ""
Write-Host "🎉 Todas las aplicaciones están iniciando..." -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "📌 URLs de acceso:"
Write-Host "   🔌 WebAPI:  https://localhost:7121/swagger" -ForegroundColor Blue
Write-Host "   🌐 MVC:     https://localhost:7092" -ForegroundColor Blue  
Write-Host "   ⚛️ SPA:     http://localhost:5173" -ForegroundColor Blue
Write-Host ""
Write-Host "⏰ Espera unos segundos a que todas las aplicaciones terminen de cargar..." -ForegroundColor Yellow
Write-Host "🛑 Para detener: Cierra las ventanas de PowerShell correspondientes" -ForegroundColor Gray

# Esperar a que el usuario presione una tecla
Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
