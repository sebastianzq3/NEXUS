# Arquitectura Propuesta: Evolución al Modelo N-Capas y SQL Server

## 1. Estado de Avance de la Migración
La evolución arquitectónica de **NEXUS** se estructuró en dos fases estratégicas:

- **Fase 1 (COMPLETADA — v1.0.0 en `src/Interfaz`):**
  - Migración total desde consola hacia **Windows Forms** con arquitectura *Event-Driven*.
  - Renderizado matricial GDI+ con doble búfer (`BufferedCanvas`) y ciclo de animación a ~30 FPS (`timerAnimacion` + partículas + interpolación).
  - Incorporación de pantalla de inicio (`FrmSplash`), branding (`Assets/`) y módulo didáctico desacoplado (`BancoPreguntasCSharp.cs`).
  - Persistencia segura en `%LocalAppData%\NEXUS\Saves` y empaquetado autónomo con **Inno Setup** (`NEXUS_Setup_v1.0.exe`).
- **Fase 2 (EN PROYECCIÓN — Separación N-Tier y SQL Server):**
  - Extracción física de las clases de dominio (actualmente en `Form1.cs`) hacia bibliotecas de clases independientes (`NEXUS.Entities`, `NEXUS.BLL`, `NEXUS.DAL`).
  - Sustitución de archivos `.json` locales por persistencia transaccional en **Microsoft SQL Server** mediante **ADO.NET (`Microsoft.Data.SqlClient`)**.

---

## 2. Capas del Sistema (Arquitectura Objetivo N-Tier)

### 2.1. Capa de Presentación (`NEXUS.GUI` / Proyecto `Interfaz`) — *Implementada*
Proyecto de tipo *Windows Forms App* (`net10.0-windows`).
- **Responsabilidad:** Capturar eventos de usuario (teclado, ratón, timers), ejecutar el renderizado GDI+ y delegar reglas de negocio a la BLL.
- **Componentes:** `FrmSplash`, `Form1` (gestión de perfiles, HUD, `BufferedCanvas`, panel táctico, log de telemetría) y diálogos modales (Combate, Inventario, Filtro de Seguridad C#, Tienda, Atlas).

### 2.2. Capa de Entidades y Lógica de Negocio (`NEXUS.BLL`) — *Validada, lista para extracción*
Proyecto de tipo *Class Library* sin dependencias de `System.Windows.Forms`.
- **Entidades de Dominio:** `Entidad`, `Usuario` (incluyendo métricas didácticas C#), `Enemigo`, `Realidad`, `Objeto`, `EntidadIris`, `AtlasRealidades`, `PreguntaCSharp`.
- **Servicios de Dominio:**
  - `ExploracionService`: validación de movimiento en grilla $40 \times 20$, consumo de energía y eventos de casilla.
  - `CombateService`: resolución de turnos, daño ligero/pesado, bloqueo y probabilidad de escape.
  - `TriviaCSharpService`: administración del banco de preguntas, barajado Fisher-Yates y cálculo de recompensas/penalizaciones.
  - `IrisMonitorService`: evaluación de umbrales de estabilidad (`>50%`, `21-50%`, `<=20%`).

### 2.3. Capa de Acceso a Datos (`NEXUS.DAL`) — *Fase de Base de Datos*
Proyecto de tipo *Class Library* dependiente de `Microsoft.Data.SqlClient`.
- **Responsabilidad:** Encapsular la conectividad segura con SQL Server mediante consultas parametrizadas, procedimientos almacenados y transacciones ACID (`SqlTransaction`), ejecutadas con patrones asíncronos (`async/await`) para no bloquear el hilo de interfaz.
- **Repositorios:**
  - `JugadorRepository`: CRUD de perfiles, progresión de nivel, mejoras y estadísticas de precisión C#.
  - `PartidaRepository`: guardado atómico del estado de la realidad activa, coordenadas y estabilidad.
  - `InventarioRepository`: persistencia de ítems activos con su durabilidad y pistas sensoriales.
  - `PreguntaRepository`: carga extensible del banco de preguntas C# desde base de datos.

---

## 3. Diagrama de Flujo y Dependencias

```text
+-------------------------------------------------------------------+
|               CAPA DE PRESENTACIÓN (Interfaz / GUI)               |
|  FrmSplash | Form1 (HUD + BufferedCanvas 30 FPS) | Modales UI     |
+---------------------------------+---------------------------------+
                                  | Eventos (KeyDown, Click, Timers)
                                  v
+-------------------------------------------------------------------+
|              CAPA DE LÓGICA DE NEGOCIO (NEXUS.BLL)                |
|  Entidades (Usuario, Realidad, Objeto, EntidadIris)               |
|  Servicios (Exploracion, Combate, TriviaCSharp, IrisMonitor)      |
+---------------------------------+---------------------------------+
                                  | DTOs / Contratos de Repositorio
                                  v
+-------------------------------------------------------------------+
|               CAPA DE ACCESO A DATOS (NEXUS.DAL)                  |
|  Actual: JSON en %LocalAppData%\NEXUS\Saves                       |
|  Objetivo: ADO.NET (Microsoft.Data.SqlClient + SqlTransaction)    |
+---------------------------------+---------------------------------+
                                  | Consultas parametrizadas / SP
                                  v
+-------------------------------------------------------------------+
|                   MICROSOFT SQL SERVER (NexusDB)                  |
|  T_Jugadores | T_Partidas_Activas | T_Inventarios | T_Preguntas   |
+-------------------------------------------------------------------+
```

---

## 4. Arquitectura de Temporizadores y Bucle Orientado a Eventos
Para mantener fluidez visual sin congelar la interfaz ni introducir condiciones de carrera, el sistema separa responsabilidades por frecuencia:
1. **`timerAnimacion` (33 ms / ~30 FPS):** Exclusivo para presentación (interpolación `visualPx/visualPy`, pulsos neón, *scanline*, partículas y textos flotantes). Nunca realiza I/O en disco ni red.
2. **`timerIris` (1200 ms):** Monitoreo de amenaza y parpadeo de alerta; se pausa automáticamente durante diálogos críticos (`mostrandoDialogoIris`).
3. **Temporizadores Locales Modales:** El cronómetro de 25 s en `AbrirModalSoporteVital` y el efecto *flash/shake* de 25 ms en `AbrirModalCombate` se instancian dentro del diálogo modal y se liberan obligatoriamente en su evento `FormClosed`.
4. **Proyección Asíncrona hacia SQL Server:** Las operaciones futuras contra `NEXUS.DAL` utilizarán `async/await` (`OpenAsync`, `ExecuteNonQueryAsync`, `ExecuteReaderAsync`) para evitar bloqueos en el *UI Thread*.