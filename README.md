# 🌐 NEXUS - Sistema de Simulación y Gestión Multiversal

![Version](https://img.shields.io/badge/version-1.1.0-00E5FF?style=for-the-badge)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET%2010-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2025%20%2F%20Express-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Inno Setup](https://img.shields.io/badge/Inno%20Setup-Installer-D500F9?style=for-the-badge)

**NEXUS** es un videojuego RPG táctico de supervivencia y plataforma didáctica desarrollado en **C# (.NET 10 / Windows Forms)** por **Sebastián Zubieta Quiroga** para la asignatura de **Programación II (Ingeniería de Sistemas — UNIFRANZ)**.

---

## 🎯 Operación Memoria (Hito 4)
En cumplimiento de los requerimientos de la Unidad de Desarrollo (Hito 4), NEXUS evolucionó para resolver la persistencia volátil del progreso del explorador mediante una **arquitectura de persistencia dual**:
1. **Persistencia Relacional en Microsoft SQL Server (`NexusDB`):** Base de datos relacional normalizada en **Tercera Forma Normal (3FN)** con 6 tablas relacionadas, claves foráneas con eliminación en cascada (`ON DELETE CASCADE`), restricciones de integridad (`CHECK`, `UNIQUE`, `DEFAULT`), procedimientos almacenados y transacciones atómicas **ACID (`SqlTransaction`)** mediante **ADO.NET puro (`Microsoft.Data.SqlClient`)**.
2. **Persistencia Local en Archivos Planos JSON:** Modo autónomo en `%LocalAppData%\NEXUS\Saves\*.json` para ejecución ligera y portable sin requerir base de datos activa.
3. **Herramienta de Migración Integrada:** Un clic permite sincronizar e importar todo el historial de partidas JSON a la base de datos SQL Server.

---

## ✨ Funcionalidades Principales (v1.1.0)
- **Persistencia Dual Conmutable en Caliente:** Selector visual en la pantalla principal para alternar entre **Modo JSON** y **Modo SQL Server (NexusDB)**, con botón de prueba de conexión en vivo y panel de configuración de cadena.
- **Transacciones ACID y Supervivencia:** Guardado integral y atómico de la entidad `Usuario`, el sector `Realidad`, la mochila `T_Inventarios` y las pistas sensoriales `T_Observaciones`. Al cerrar NEXUS y volver a ejecutar, la partida se restaura exactamente en su último estado guardado.
- **Banco Didáctico de C# Centralizado en SQL:** 28 preguntas técnicas (Sintaxis, POO, LINQ/Colecciones y Memoria/.NET) cargadas en `T_Preguntas_CSharp`, consumidas dinámicamente o con respaldo local Fisher-Yates.
- **Pantalla de Carga Animada (`FrmSplash`):** Transición *fade-in*, logo oficial neón pulsante, *scanline* holográfico y barra de progreso cuántica.
- **Motor Gráfico GDI+ a ~30 FPS (`BufferedCanvas`):** Mapa táctico $40 \times 20$ con doble búfer, interpolación suave del cadete (`@`), auras neón, partículas y textos flotantes.
- **Combate Táctico con Retroalimentación Visual:** Turnos con Ataque Ligero, Ataque Pesado, Bloqueo y Escape, acompañados de efectos *flash* y *screen shake*.
- **Máquina de Estados Táctica:** Alternancia entre **Modo Campamento** (gestión y tienda) y **Modo Exploración** (consumo de energía y vigilancia activa de la IA antagonista **I.R.I.S.**).
- **Instalador Autónomo para Windows:** Publicación *self-contained* (`win-x64`, ReadyToRun) generada con **Inno Setup**.

---

## 🏗️ Arquitectura y Tecnologías
- **Lenguaje y Framework:** C# sobre `.NET 10.0` (`net10.0-windows`)
- **Interfaz Gráfica (GUI):** Windows Forms + GDI+ (`BufferedCanvas` con `DoubleBuffered = true`)
- **Motor de Base de Datos:** Microsoft SQL Server 2025 / 2022 / Express Edition
- **Capa de Acceso a Datos (DAL):** ADO.NET (`Microsoft.Data.SqlClient` v7.1.2) con consultas parametrizadas y `SqlTransaction`
- **Serialización Alternativa:** `System.Text.Json` en `%LocalAppData%\NEXUS\Saves`
- **Control de Versiones:** Git / GitHub

---

## 📂 Estructura del Repositorio

```text
📦 NEXUS
 ┣ 📂 database/                               # Scripts SQL versionados y automatización
 ┃ ┣ 📜 01_CrearBaseDatos.sql                # DDL: Creación y configuración de NexusDB
 ┃ ┣ 📜 02_CrearTablasYRelaciones.sql        # DDL: 6 tablas 3FN, PK, FK Cascade y CHECKs
 ┃ ┣ 📜 03_ProcedimientosAlmacenados.sql     # DML: SPs transaccionales (Guardar, Cargar, Listar, Purgar)
 ┃ ┣ 📜 04_CargaInicialPreguntas.sql         # DML: Poblado de 28 preguntas pedagógicas C#
 ┃ ┣ 📜 05_ScriptCompleto_NexusDB.sql        # Despliegue todo-en-uno integrado
 ┃ ┗ 📜 ejecutar_scripts.ps1                 # Script PowerShell para despliegue automático
 ┣ 📂 docs/                                   # Documentación técnica, diseño y manuales
 ┃ ┣ 📂 evidencias/                           # Bocetos (wireframes) y capturas QA
 ┃ ┣ 📜 analisis-tecnico.md                   # Alternativas evaluadas y verificación asistida por IA
 ┃ ┣ 📜 arquitectura-actual.md                # Arquitectura v1.1.0 con persistencia dual
 ┃ ┣ 📜 arquitectura-propuesta.md             # Evolución N-Tier y justificación ADO.NET
 ┃ ┣ 📜 diseno-del-juego.md                   # Mecánicas, lore y ciclo de juego
 ┃ ┣ 📜 modelo-de-datos-propuesto.md          # Diagrama ER (Mermaid) y diccionario de datos 3FN
 ┃ ┣ 📜 presentacion-hito4-memoria.md         # Diapositivas técnicas estructuradas (Entrega 1)
 ┃ ┣ 📜 pruebas-instalacion-limpia.md         # Protocolo y checklist QA
 ┃ ┗ 📜 NEXUS_Manual_Tecnico                  # Manual técnico y guía de estudio
 ┣ 📂 src/                                    # Código fuente y solución
 ┃ ┣ 📂 Interfaz/                             # Proyecto principal Windows Forms (compila NEXUS.exe)
 ┃ ┃ ┣ 📂 Assets/                             # Identidad visual (íconos, logo)
 ┃ ┃ ┣ 📜 BancoPreguntasCSharp.cs             # Módulo didáctico y cola Fisher-Yates
 ┃ ┃ ┣ 📜 Form1.cs                            # Controlador UI, animaciones GDI+ y eventos
 ┃ ┃ ┣ 📜 Form1.Designer.cs                   # Diseño del HUD, Canvas y Panel de Persistencia
 ┃ ┃ ┣ 📜 Form1.resx                          # Recursos WinForms
 ┃ ┃ ┣ 📜 FrmSplash.cs                        # Splash screen animado
 ┃ ┃ ┣ 📜 Interfaz.csproj                     # Configuración de compilación y paquete SqlClient
 ┃ ┃ ┣ 📜 NexusSqlService.cs                  # Capa de Acceso a Datos (DAL ADO.NET SQL Server)
 ┃ ┃ ┗ 📜 Program.cs                          # Punto de entrada STAThread
 ┃ ┣ 📂 NEXUS/                                # Proyecto original de consola (legado Hito 3)
 ┃ ┣ 📂 scripts/
 ┃ ┃ ┗ 📜 publish.ps1                         # Publicación self-contained win-x64
 ┃ ┣ 📂 installer/
 ┃ ┃ ┗ 📜 NEXUS_Setup.iss                     # Script Inno Setup
 ┃ ┗ 📜 NEXUS.sln                             # Solución de Visual Studio
 ┣ 📜 .gitignore                              # Exclusiones de compilación y credenciales
 ┗ 📜 README.md                               # Documentación principal
```

---

## 🚀 Despliegue de la Base de Datos (SQL Server)

Para recrear y reproducir la base de datos `NexusDB` desde un clon limpio en cualquier equipo:

### Método 1: Despliegue Automatizado con PowerShell (Recomendado)
Abre PowerShell en la raíz del repositorio y ejecuta:
```powershell
powershell -ExecutionPolicy Bypass -File .\database\ejecutar_scripts.ps1
```
*Si tu instancia de SQL Server tiene otro nombre (por ejemplo `localhost` o `SQLEXPRESS01`), pásala como parámetro:*
```powershell
powershell -ExecutionPolicy Bypass -File .\database\ejecutar_scripts.ps1 -ServerInstance "localhost"
```

### Método 2: Despliegue con `sqlcmd` (Script Todo-en-Uno)
```powershell
sqlcmd -S ".\SQLEXPRESS" -E -C -f 65001 -i .\database\05_ScriptCompleto_NexusDB.sql
```

### Cadena de Conexión Predeterminada
```text
Server=.\SQLEXPRESS;Database=NexusDB;Trusted_Connection=True;TrustServerCertificate=True;
```
*(No requiere contraseñas hardcodeadas en el código; utiliza Autenticación Integrada de Windows / Trusted Connection).*

---

## 🎮 Guía de Ejecución del Videojuego

### Desde Visual Studio:
1. Clona el repositorio:
   ```bash
   git clone https://github.com/sebastianzq3/NEXUS.git
   ```
2. Ejecuta el script de despliegue de base de datos (`database/ejecutar_scripts.ps1`).
3. Abre `src/NEXUS.sln` en **Visual Studio Community**.
4. Establece **`Interfaz`** como proyecto de inicio.
5. Presiona `F5` para compilar y ejecutar.
6. En la pantalla principal, selecciona **🗄️ SQL Server Relacional (NexusDB)**, registra un nuevo explorador y entra a la simulación.

### Prueba de Supervivencia:
1. Inicia sesión con un explorador en modo SQL Server.
2. Inicia una incursión, gasta energía, recolecta fragmentos o responde una trivia de C#.
3. Regresa al campamento y cierra completamente la aplicación.
4. Vuelve a ejecutar NEXUS: selecciona el perfil y haz clic en **► CARGAR PERFIL SELECCIONADO**.
5. Los puntos de salud, energía, inventario, fragmentos y progreso se restaurarán con total fidelidad desde SQL Server.

---

*Desarrollado por **Sebastián Zubieta Quiroga** — Ingeniería de Sistemas, UNIFRANZ Cochabamba. Asignatura: Programación II.*
