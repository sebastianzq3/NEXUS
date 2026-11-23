# 🌐 NEXUS - Sistema de Simulación y Gestión Multiversal

![Version](https://img.shields.io/badge/version-1.0.0-00E5FF?style=for-the-badge)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET%2010-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Inno Setup](https://img.shields.io/badge/Inno%20Setup-Installer-D500F9?style=for-the-badge)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)

**NEXUS** es un videojuego RPG táctico de supervivencia y plataforma didáctica desarrollado en **C# (.NET 10 / Windows Forms)** por **Sebastián Zubieta Quiroga** para la asignatura de **Programación II (Ingeniería de Sistemas — UNIFRANZ)**.

## 🎯 Propósito y Evolución del Proyecto
NEXUS evolucionó desde un simulador secuencial en consola (Hito 3) hasta una **aplicación de escritorio instalable y autónoma (`NEXUS_Setup_v1.0.exe`)** con renderizado 2D animado en **GDI+**, identidad visual propia y un **enfoque educativo integrado**: para recargar energía vital, el explorador debe superar filtros de seguridad compuestos por **trivias técnicas de C# y POO** con retroalimentación didáctica inmediata.

## ✨ Funcionalidades Principales (v1.0.0)
- **Pantalla de Carga Animada (`FrmSplash`):** Transición *fade-in*, logo oficial con borde neón pulsante, *scanline* holográfico, barra de progreso cuántica y créditos de autoría.
- **Motor Gráfico GDI+ a ~30 FPS (`BufferedCanvas`):** Mapa táctico $40 \times 20$ con doble búfer, interpolación suave del movimiento del jugador (`@`), auras neón pulsantes, línea *scanline*, sistema de partículas y textos flotantes (`+1 ▤`, `+HP`, `+30% EST`, `+1 FRAG`).
- **Combate Táctico con Retroalimentación Visual:** Turnos con Ataque Ligero, Ataque Pesado, Bloqueo y Escape, acompañados de efectos *flash* y micro-sacudida (*screen shake*) al impactar o recibir daño.
- **Módulo Didáctico de C# (`BancoPreguntasCSharp`):** Banco de **28 preguntas verificadas** (Sintaxis/Tipos, POO, Colecciones/LINQ y Excepciones/Memoria/.NET) gestionado con barajado **Fisher-Yates** sin repeticiones consecutivas, temporizador regresivo de 25 segundos, opción múltiple (A–D), explicación didáctica al responder y registro de precisión por perfil.
- **Máquina de Estados Táctica:** Alternancia estricta entre **Modo Campamento** (gestión habilitada, mapa congelado) y **Modo Exploración** (consumo de energía cada 3 pasos, vigilancia pasiva y activa de la IA antagonista **I.R.I.S.**).
- **Persistencia Segura Multi-Slot:** Guardado automático en `%LocalAppData%\NEXUS\Saves` (compatible con instalaciones en `C:\Program Files\NEXUS`) con migración automática de partidas previas.
- **Instalador Profesional para Windows:** Publicación *self-contained* (`win-x64`, ReadyToRun) empaquetada con **Inno Setup** sin requerir instalación previa del Runtime de .NET.

## 🏗️ Arquitectura y Tecnologías
- **Lenguaje y Runtime:** C# sobre `.NET 10.0` (`net10.0-windows`)
- **Interfaz Gráfica (GUI):** Windows Forms + GDI+ (`BufferedCanvas` con `DoubleBuffered = true`)
- **Paradigma:** Programación Orientada a Objetos (POO) y Arquitectura Orientada a Eventos (*Event-Driven*)
- **Persistencia:** Serialización `System.Text.Json` en `%LocalAppData%\NEXUS\Saves` y diseño relacional proyectado para **Microsoft SQL Server** (`Microsoft.Data.SqlClient`)
- **Empaquetado y Release:** `dotnet publish` (`--self-contained true`) + **Inno Setup**
- **Control de Versiones:** Git / GitHub

## 📂 Estructura del Repositorio
El repositorio excluye compilados temporales y ejecutables pesados (`bin/`, `obj/`, `.vs/`, `publish/`, `installer/output/`) mediante `.gitignore`.

```text
📦 NEXUS
 ┣ 📂 docs/                                   # Documentación de diseño, arquitectura, QA y manuales
 ┃ ┣ 📂 evidencias/                           # Bocetos (wireframes) y capturas de pruebas
 ┃ ┃ ┗ 📜 bocetos-de-interfaces-graficas.md
 ┃ ┣ 📜 analisis-tecnico.md                   # Alternativas evaluadas, decisiones y verificación IA
 ┃ ┣ 📜 arquitectura-actual.md                # Arquitectura actual v1.0 (WinForms + Animación + C# Trivia)
 ┃ ┣ 📜 arquitectura-propuesta.md             # Evolución a N-Capas (BLL/DAL) y SQL Server
 ┃ ┣ 📜 diseno-del-juego.md                   # Visión, mecánicas, UX y módulo didáctico
 ┃ ┣ 📜 modelo-de-datos-propuesto.md          # Modelo relacional 3FN para SQL Server
 ┃ ┣ 📜 pruebas-instalacion-limpia.md         # Protocolo y checklist QA en entorno limpio
 ┃ ┗ 📜 NEXUS_Manual_Tecnico                  # Manual técnico y guía de estudio Hito 3
 ┣ 📂 src/                                    # Código fuente, solución y empaquetado
 ┃ ┣ 📂 Interfaz/                             # Proyecto principal Windows Forms (compila como NEXUS.exe)
 ┃ ┃ ┣ 📂 Assets/                             # Identidad visual (nexus_icon.ico, nexus_icon.png, nexus_logo.png)
 ┃ ┃ ┣ 📜 BancoPreguntasCSharp.cs             # Banco de 28 preguntas C# y cola Fisher-Yates
 ┃ ┃ ┣ 📜 Form1.cs                            # Lógica de eventos, animaciones 30 FPS, modales y dominio
 ┃ ┃ ┣ 📜 Form1.Designer.cs                   # Diseño de controles UI, HUD, BufferedCanvas y Panel Táctico
 ┃ ┃ ┣ 📜 Form1.resx                          # Recursos del formulario principal
 ┃ ┃ ┣ 📜 FrmSplash.cs                        # Pantalla de inicio animada con fade-in y barra cuántica
 ┃ ┃ ┣ 📜 Interfaz.csproj                     # Configuración del proyecto, ícono, Assets y metadatos
 ┃ ┃ ┗ 📜 Program.cs                          # Punto de entrada [STAThread] (FrmSplash -> Form1)
 ┃ ┣ 📂 NEXUS/                                # Proyecto original de consola (legado Hito 3)
 ┃ ┃ ┣ 📂 Properties/
 ┃ ┃ ┣ 📜 NEXUS.csproj
 ┃ ┃ ┗ 📜 Program.cs
 ┃ ┣ 📂 scripts/
 ┃ ┃ ┗ 📜 publish.ps1                         # Script PowerShell de publicación self-contained win-x64
 ┃ ┣ 📂 installer/
 ┃ ┃ ┗ 📜 NEXUS_Setup.iss                     # Script Inno Setup generador de NEXUS_Setup_v1.0.exe
 ┃ ┗ 📜 NEXUS.sln                             # Solución principal de Visual Studio
 ┣ 📜 .gitignore                              # Exclusiones de Visual Studio, publish/ e installer/output/
 ┗ 📜 README.md                               # Documentación principal del repositorio
```

## 🚀 Guía de Instalación y Ejecución

### Opción A: Instalar en Windows (Recomendada para usuarios finales)
1. Descarga **`NEXUS_Setup_v1.0.exe`** desde la sección **Releases** de este repositorio.
2. Ejecuta el instalador y sigue el asistente (no requiere tener instalado Visual Studio ni .NET Runtime).
3. Inicia **NEXUS** desde el acceso directo del Escritorio o el Menú Inicio.

### Opción B: Ejecutar desde el código fuente (Visual Studio)
1. Clona el repositorio:
   ```bash
   git clone [https://github.com/sebastianzq3/NEXUS.git](https://github.com/sebastianzq3/NEXUS.git)
   ```
2. Abre `src/NEXUS.sln` en **Visual Studio Community**.
3. Haz clic derecho sobre el proyecto **`Interfaz`** → **Establecer como proyecto de inicio** (*Set as Startup Project*).
4. Presiona `Ctrl+Shift+B` para compilar y `F5` para ejecutar.

### Opción C: Compilar tu propio instalador `.exe`
Desde PowerShell en la carpeta `src/`:
```powershell
# 1. Publicar binario autónomo (win-x64)
powershell -ExecutionPolicy Bypass -File .\scripts\publish.ps1

# 2. Compilar el instalador con Inno Setup
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" .\installer\NEXUS_Setup.iss
```
El instalador generado se guardará en `src/installer/output/NEXUS_Setup_v1.0.exe`.

## 🧠 Índice de Documentación (`docs/`)
- [`diseno-del-juego.md`](docs/diseno-del-juego.md) y `evidencias/`: Visión, ciclo de juego, UX, animaciones y sistema didáctico de trivias C#.
- [`arquitectura-actual.md`](docs/arquitectura-actual.md): Inventario real de clases, temporizadores, motor GDI+, módulo educativo y empaquetado v1.0.
- [`arquitectura-propuesta.md`](docs/arquitectura-propuesta.md): Proyección de desacoplamiento en capas (`BLL` / `DAL`) hacia SQL Server.
- [`modelo-de-datos-propuesto.md`](docs/modelo-de-datos-propuesto.md): Esquema relacional en 3FN para persistencia de jugadores, partidas, inventarios y preguntas C#.
- [`analisis-tecnico.md`](docs/analisis-tecnico.md): Comparativa de tecnologías, decisiones de ingeniería, empaquetado y verificación de información asistida por IA.
- [`pruebas-instalacion-limpia.md`](docs/pruebas-instalacion-limpia.md): Protocolo de validación QA del instalador en entorno limpio.

---
*Desarrollado por **Sebastián Zubieta Quiroga** — Ingeniería de Sistemas, UNIFRANZ Cochabamba. Asignatura: Programación II.*