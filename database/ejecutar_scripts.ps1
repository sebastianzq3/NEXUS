<#
.SYNOPSIS
    Script de automatización para la creación y despliegue de la Base de Datos NexusDB en Microsoft SQL Server.
.DESCRIPTION
    Ejecuta en secuencia los scripts SQL versionados para el proyecto NEXUS (Hito 4 - UNIFRANZ).
    Permite reproducir la persistencia relacional desde un clon limpio con una sola instrucción.
.PARAMETER ServerInstance
    Nombre o instancia del servidor SQL Server (por defecto: .\SQLEXPRESS).
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\database\ejecutar_scripts.ps1
    powershell -ExecutionPolicy Bypass -File .\database\ejecutar_scripts.ps1 -ServerInstance "localhost"
#>

param(
    [string]$ServerInstance = ".\SQLEXPRESS"
)

Write-Host "==========================================================================" -ForegroundColor Cyan
Write-Host "  NEXUS - DESPLIEGUE AUTOMÁTICO DE BASE DE DATOS SQL SERVER (HITO 4)" -ForegroundColor Cyan
Write-Host "==========================================================================" -ForegroundColor Cyan
Write-Host "Servidor destino : $ServerInstance" -ForegroundColor Yellow
Write-Host "Directorio scripts: $PSScriptRoot" -ForegroundColor Yellow
Write-Host ""

$archivosSql = @(
    "01_CrearBaseDatos.sql",
    "02_CrearTablasYRelaciones.sql",
    "03_ProcedimientosAlmacenados.sql",
    "04_CargaInicialPreguntas.sql"
)

$errores = 0

foreach ($archivo in $archivosSql) {
    $ruta = Join-Path $PSScriptRoot $archivo
    if (Test-Path $ruta) {
        Write-Host "► Ejecutando [$archivo]..." -ForegroundColor Green
        & sqlcmd -S $ServerInstance -E -C -f 65001 -i $ruta
        if ($LASTEXITCODE -ne 0) {
            Write-Host "✖ Error al ejecutar $archivo (Código: $LASTEXITCODE)" -ForegroundColor Red
            $errores++
            break
        }
    } else {
        Write-Host "✖ No se encontró el archivo: $ruta" -ForegroundColor Red
        $errores++
        break
    }
}

Write-Host ""
if ($errores -eq 0) {
    Write-Host "✔ Base de datos NexusDB configurada y poblada exitosamente." -ForegroundColor Green
    Write-Host "  Cadena de conexión recomendada:" -ForegroundColor Gray
    Write-Host "  Server=$ServerInstance;Database=NexusDB;Trusted_Connection=True;TrustServerCertificate=True;" -ForegroundColor White
} else {
    Write-Host "✖ Ocurrieron errores durante el despliegue de NexusDB." -ForegroundColor Red
}
Write-Host "==========================================================================" -ForegroundColor Cyan
