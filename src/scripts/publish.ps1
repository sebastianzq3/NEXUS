$ErrorActionPreference = "Stop"

$RootDir    = Split-Path -Parent $PSScriptRoot
$CsprojPath = Join-Path $RootDir "Interfaz\Interfaz.csproj"
$OutputDir  = Join-Path $RootDir "publish\win-x64"

Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "  PUBLICACION SELF-CONTAINED DE NEXUS (win-x64)" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan

if (-not (Test-Path $CsprojPath)) {
    throw "No se encontro el proyecto en: $CsprojPath"
}

if (Test-Path $OutputDir) {
    Write-Host "[1/3] Limpiando publicacion anterior..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $OutputDir
}

Write-Host "[2/3] Ejecutando dotnet publish (Release, win-x64, self-contained, ReadyToRun)..." -ForegroundColor Yellow
dotnet publish $CsprojPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishReadyToRun=true `
    -o $OutputDir

if ($LASTEXITCODE -ne 0) {
    throw "Error al ejecutar dotnet publish."
}

Write-Host "[3/3] Verificando ejecutable y carpeta Assets..." -ForegroundColor Yellow
$ExePath    = Join-Path $OutputDir "NEXUS.exe"
$AssetsDir  = Join-Path $OutputDir "Assets"
$Requeridos = @("nexus_icon.ico", "nexus_icon.png", "nexus_logo.png")

if (-not (Test-Path $ExePath)) {
    throw "No se genero NEXUS.exe en $OutputDir. Verifica <AssemblyName>NEXUS</AssemblyName> en Interfaz.csproj."
}

if (-not (Test-Path $AssetsDir)) {
    throw "Falta la carpeta Assets en $OutputDir."
}

foreach ($archivo in $Requeridos) {
    $ruta = Join-Path $AssetsDir $archivo
    if (-not (Test-Path $ruta)) {
        Write-Warning "No se encontro el asset esperado: $ruta"
    } else {
        Write-Host "  [OK] Assets\$archivo" -ForegroundColor Green
    }
}

Write-Host "`nPublicacion completada exitosamente en: $OutputDir" -ForegroundColor Green