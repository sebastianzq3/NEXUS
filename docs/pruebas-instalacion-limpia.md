# Protocolo de Pruebas de Instalación en Entorno Limpio — NEXUS v1.0

## 1. Objetivo
Validar que el instalador `NEXUS_Setup_v1.0.exe` funciona de manera autónoma (*self-contained*) en un equipo Windows limpio (sin Visual Studio, sin .NET SDK y sin Runtime de .NET preinstalado), verificando permisos de escritura, carga de recursos gráficos, módulo didáctico C# y desinstalación limpia.

## 2. Preparación del Entorno Limpio (Windows Sandbox)
1. Abrir **Activar o desactivar las características de Windows** y marcar **Espacio aislado de Windows (Windows Sandbox)** (o iniciar una máquina virtual Windows 10/11 recién instalada).
2. Reiniciar si Windows lo solicita y abrir **Windows Sandbox**.
3. Copiar únicamente el archivo `installer\output\NEXUS_Setup_v1.0.exe` dentro del escritorio de Windows Sandbox.
4. Verificar en PowerShell dentro del Sandbox que `dotnet --version` no está instalado (confirmando entorno limpio).

## 3. Checklist de Verificación (QA)

| # | Caso de Prueba | Pasos a Ejecutar | Resultado Esperado | Estado |
| :-: | :--- | :--- | :--- | :-: |
| **1** | **Instalación sin dependencias** | Ejecutar `NEXUS_Setup_v1.0.exe`, aceptar ruta por defecto (`C:\Program Files\NEXUS`), marcar acceso directo en Escritorio y finalizar. | El asistente finaliza sin errores y sin solicitar descarga de .NET Runtime. | [ ] |
| **2** | **Accesos directos e ícono** | Inspeccionar Escritorio, Menú Inicio y la carpeta `C:\Program Files\NEXUS`. | El ejecutable `NEXUS.exe` y los accesos directos muestran el ícono hexagonal neón (`nexus_icon.ico`). | [ ] |
| **3** | **Splash y Assets** | Iniciar NEXUS desde el acceso directo del Escritorio. | Se muestra `FrmSplash` con fade-in, `nexus_logo.png`, barra cuántica animada y firma de autor; al terminar abre `Form1` con el logo y créditos en `pnlPerfiles`. | [ ] |
| **4** | **Persistencia en `%LocalAppData%`** | Crear un nuevo explorador, entrar a una expedición, moverse, volver al campamento, cerrar la app y volver a abrirla. | No aparece `UnauthorizedAccessException`. El archivo `.json` se crea en `%LocalAppData%\NEXUS\Saves` y el perfil se recarga intacto. | [ ] |
| **5** | **Animaciones, Combate y Trivia C#** | Iniciar incursión, moverse con WASD/clic, recoger Restos (`▤`), combatir un enemigo (`Ж`) con Ataque Pesado y abrir 3 veces **Recuperar energía** (1 acierto, 1 fallo y 1 espera de 25 s). | Deslizamiento suave de `@`, pulsos neón, partículas y texto flotante; *flash* y sacudida en combate; la trivia aplica +5 E al acertar, +1 E / -15% EST al fallar o expirar, y muestra la explicación didáctica. | [ ] |
| **6** | **Desinstalación limpia** | Ir a *Configuración → Aplicaciones → Aplicaciones instaladas*, buscar **NEXUS 1.0.0** y desinstalar. | Se eliminan los accesos directos y la carpeta `C:\Program Files\NEXUS` queda completamente removida. | [ ] |

## 4. Evidencias Recomendadas para la Entrega
- Captura de `NEXUS_Setup_v1.0.exe` ejecutándose en Windows Sandbox.
- Captura de `FrmSplash` y de una trivia de C# con explicación didáctica visible.
- Captura del directorio `%LocalAppData%\NEXUS\Saves` mostrando el archivo `.json` generado.