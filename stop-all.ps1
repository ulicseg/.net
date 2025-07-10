# Script para detener todas las aplicaciones del sistema de reservas
Write-Host "🛑 Deteniendo Sistema de Reservas..." -ForegroundColor Red
Write-Host "===============================================" -ForegroundColor Red

# Función para detener procesos por puerto
function Stop-ProcessByPort {
    param($Port, $Name)
    try {
        $connections = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
        foreach ($conn in $connections) {
            $process = Get-Process -Id $conn.OwningProcess -ErrorAction SilentlyContinue
            if ($process) {
                Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
                Write-Host "✅ $Name (puerto $Port) detenido" -ForegroundColor Green
            }
        }
        if (-not $connections) {
            Write-Host "ℹ️ $Name (puerto $Port) no estaba ejecutándose" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "⚠️ No se pudo detener $Name en puerto $Port" -ForegroundColor Yellow
    }
}

# Detener por puertos conocidos
Stop-ProcessByPort 5284 "WebAPI"
Stop-ProcessByPort 7092 "MVC"  
Stop-ProcessByPort 5173 "SPA"

# Detener procesos de dotnet y node relacionados con el proyecto
Write-Host ""
Write-Host "🔍 Buscando procesos relacionados..." -ForegroundColor Yellow

# Detener procesos dotnet del proyecto
$dotnetProcesses = Get-Process dotnet -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*Proyectos\Practico*" }
foreach ($proc in $dotnetProcesses) {
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    Write-Host "✅ Proceso dotnet detenido (PID: $($proc.Id))" -ForegroundColor Green
}

# Detener procesos node del proyecto  
$nodeProcesses = Get-Process node -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*Proyectos\Practico*" -or $_.CommandLine -like "*ReservaApp.SPA*" }
foreach ($proc in $nodeProcesses) {
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    Write-Host "✅ Proceso node detenido (PID: $($proc.Id))" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎉 Sistema completamente detenido" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
