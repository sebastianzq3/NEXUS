# Arquitectura Actual del Sistema: NEXUS (v1.0.0 — Windows Forms)

## 1. Resumen Ejecutivo y Estado del Proyecto
La versión **v1.0.0** de **NEXUS** es una aplicación de escritorio gráfica e instalable desarrollada en **C# (.NET 10) y Windows Forms** dentro del proyecto `src/Interfaz` (compilado con `<AssemblyName>NEXUS</AssemblyName>` para producir **`NEXUS.exe`**).

El sistema conserva la totalidad de las reglas de negocio del Hito 3 mientras consolida tres grandes pilares arquitectónicos:
1. **Arquitectura Orientada a Eventos (*Event-Driven*) con Motor Animado GDI+:** Renderizado en `BufferedCanvas` a ~30 FPS mediante temporizadores desacoplados (`timerAnimacion` y `timerIris`).
2. **Módulo Educativo Desacoplado (`BancoPreguntasCSharp`):** Sistema de trivias técnicas de C# con barajado Fisher-Yates e historial de precisión persistido por jugador.
3. **Empaquetado Autónomo (*Self-Contained*) y Persistencia Segura:** Almacenamiento JSON en `%LocalAppData%\NEXUS\Saves` y generación de instalador profesional mediante `scripts/publish.ps1` e `installer/NEXUS_Setup.iss`.

---

## 2. Diagrama de Estructura de Clases Actual

```text
+--------------------+
|     Program.cs     |
| [STAThread] Main() |
+---------+----------+
          | 1. ShowDialog()         2. Application.Run()
          ├──────────────────────────┐
          v                          v
+--------------------+     +----------------------------------+
|  FrmSplash : Form  |     |           Form1 : Form           |
+--------------------+     +----------------------------------+
| - timerAnim (33ms) |     | - canvasMapa (BufferedCanvas)    |
| - logoImg          |     | - timerAnimacion (33ms / 30 FPS) |
| + OnPaint()        |     | - timerIris (1200ms)             |
+--------------------+     | - bancoPreguntas                 |
                           | - particulas / textosFlotantes   |
                           | - directorioSaves (%LocalAppData)|
                           | + ProcesarMovimiento()           |
                           | + AbrirModalSoporteVital()       |
                           +--------+---------------+---------+
                                    |               |
                        (Usa)       |               | (Gestiona)
                                    v               v
                     +-----------------------+   +-------------------+
                     | BancoPreguntasCSharp  |   |      Entidad      |
                     +-----------------------+   +-------------------+
                     | - colaBarajada (Queue)|   | + VidaMax / Vida  |
                     | + ObtenerSiguiente()  |   | + DañoBase        |
                     +-----------+-----------+   +---------+---------+
                                 |                         ^
                                 v                         | (Herencia)
                     +-----------------------+    +--------+--------+
                     |    PreguntaCSharp     |    |                 |
                     +-----------------------+ +--+-------+   +-----+----+
                     | + Categoria           | | Usuario  |   | Enemigo  |
                     | + Enunciado / Codigo  | +----------+   +----------+
                     | + Opciones[4]         | | + Nivel  |   | + Nombre |
                     | + IndiceCorrecto      | | + StatsC#|   | + Escape |
                     | + Explicacion         | +----------+   +----------+
                     +-----------------------+

+--------------------+     +--------------------+     +--------------------+
|      Realidad      |     |       Objeto       |     |    EntidadIris     |
+--------------------+     +--------------------+     +--------------------+
| + Estabilidad      |     | + Nombre           |     | + EstadoActual     |
| + Anomalia         |     | + Contrarresta     |     | + NivelAmenaza     |
| + MapaLocal[,]     |     | + Usos             |     | + EscanearVuln()   |
+--------------------+     +--------------------+     +--------------------+

+--------------------+     +--------------------+     +--------------------+
|    DatosPartida    |     |  AtlasRealidades   |     |   BufferedCanvas   |
+--------------------+     +--------------------+     +--------------------+
| + Jugador          |     | + Atlas[,] (18x12) |     | + DoubleBuffered   |
| + RealidadActual   |     | + ActualizarAtlas()|     |   (Herencia Panel) |
+--------------------+     +--------------------+     +--------------------+
```

---

## 3. Inventario y Análisis de Componentes

### 3.1. `Program.cs` y `FrmSplash.cs` (Arranque e Identidad Visual)
- `Program.cs` inicia la configuración DPI de WinForms, ejecuta `FrmSplash` de manera modal (`using (FrmSplash splash = new FrmSplash()) splash.ShowDialog();`) y posteriormente abre `Form1`.
- `FrmSplash` opera sin bordes (`FormBorderStyle.None`, `DoubleBuffered = true`), cargando `Assets/nexus_logo.png` desde `AppContext.BaseDirectory` con respaldo (*fallback*) dibujado en GDI+ si el archivo no existe. Anima la opacidad (`0.0 -> 1.0`), bordes pulsantes, línea *scanline* y barra de carga durante ~2.5 segundos (omitible con clic o tecla) y libera todos sus recursos en `OnFormClosed`.

### 3.2. `Form1.cs` y `Form1.Designer.cs` (Controlador Principal y Animación)
- **Temporizadores Desacoplados:**
  - `timerAnimacion` (`Interval = 33 ms`, ~30 FPS): actualiza la fase sinusoidal de pulsos neón, interpola la posición visual del jugador (`visualPx`, `visualPy`), avanza la línea *scanline* y simula la física/desvanecimiento de `List<Particula>` y `List<TextoFlotante>`.
  - `timerIris` (`Interval = 1200 ms`): evalúa la vulnerabilidad del sector y dispara alertas visuales o invasiones críticas.
  - En `Form1_FormClosing`, ambos timers se detienen, se persiste la partida activa y se invoca `Dispose()` sobre imágenes y recursos GDI+.
- **Identidad Visual y UX:** Carga `Assets/nexus_icon.ico` como ícono de ventana, inserta `Assets/nexus_logo.png` y el pie de créditos en `pnlPerfiles`, aplica efecto *hover* dinámico a botones y ejecuta efectos *flash* + *screen shake* en `AbrirModalCombate`.
- **Persistencia Segura en Windows:** Cambia el almacenamiento desde `"Saves"` relativo hacia `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NEXUS", "Saves")`, incluyendo `MigrarSavesLocalesSiExisten()` para copiar automáticamente archivos `.json` previos creados durante desarrollo.

### 3.3. `BancoPreguntasCSharp.cs` (Módulo Didáctico)
- Encapsula `PreguntaCSharp` y `BancoPreguntasCSharp` con **28 preguntas** distribuidas en 4 categorías: *Sintaxis y Tipos de Datos*, *Programación Orientada a Objetos*, *Colecciones y LINQ*, y *Excepciones, Memoria y .NET*.
- Implementa una cola barajada mediante **Fisher-Yates** evitando repeticiones consecutivas entre rondas.
- Alimenta el modal `AbrirModalSoporteVital`, que gestiona un temporizador regresivo de 25 segundos, bloque de código de ejemplo, validación A–D y despliegue de explicación didáctica.

### 3.4. Clases de Dominio y Serialización JSON
- `Usuario` agrega `PreguntasCSharpRespondidas` y `PreguntasCSharpAcertadas` (inicializadas en `0` para preservar compatibilidad hacia atrás con archivos `.json` existentes).
- `Realidad`, `Objeto`, `Enemigo`, `EntidadIris`, `AtlasRealidades` y `DatosPartida` mantienen sus reglas originales, con `InicializarMapaSiNoExiste()` para reconstruir `MapaLocal` (`[JsonIgnore]`) al deserializar.

### 3.5. Pipeline de Compilación e Instalador (`src/scripts` y `src/installer`)
- `Interfaz.csproj`: declara `<AssemblyName>NEXUS</AssemblyName>`, `<ApplicationIcon>Assets\nexus_icon.ico</ApplicationIcon>`, metadatos de ensamblado y copia automática de `Assets\**\*`.
- `scripts/publish.ps1`: limpia y ejecuta `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true`, verificando la existencia de `NEXUS.exe` y de los 3 archivos de `Assets`.
- `installer/NEXUS_Setup.iss`: script **Inno Setup** que genera `NEXUS_Setup_v1.0.exe`, instala en `{autopf}\NEXUS`, registra accesos directos con ícono oficial y provee desinstalación limpia.

---

## 4. Evolución Frente a Limitaciones Anteriores

| Limitación Previa (H3 / Primer Prototipo) | Solución Implementada en NEXUS v1.0.0 |
| :--- | :--- |
| **Interfaz bloqueante en consola (`ReadKey`/`Sleep`)** | Arquitectura orientada a eventos en Windows Forms con `KeyDown`, `MouseClick` y `Timer`. |
| **Mapa estático sin retroalimentación kinestésica** | Motor GDI+ a ~30 FPS con interpolación de movimiento, pulsos neón, *scanline*, partículas, textos flotantes y *screen shake* en combate. |
| **Minijuegos de recarga genéricos y repetitivos** | Módulo didáctico `BancoPreguntasCSharp` (28 preguntas, Fisher-Yates, 25 s, explicaciones y métricas de precisión). |
| **Error de permisos (`UnauthorizedAccessException`) en `Program Files`** | Migración del guardado a `%LocalAppData%\NEXUS\Saves` con sanitización de nombres de archivo y respaldo de partidas previas. |
| **Ejecución limitada a Visual Studio / PC con .NET SDK** | Publicación *self-contained* (`win-x64`) empaquetada en instalador autónomo `NEXUS_Setup_v1.0.exe`. |

---

## 5. Limitaciones Pendientes (Siguiente Iteración)
1. **Acoplamiento de Clases en `Form1.cs`:** Las entidades de dominio aún residen en el mismo archivo que la vista principal; su extracción hacia bibliotecas `NEXUS.BLL` y `NEXUS.DAL` se documenta en `arquitectura-propuesta.md`.
2. **Persistencia Local en JSON:** Aunque estable para un jugador local en `%LocalAppData%`, no ofrece integridad referencial ni consultas multiusuario centralizadas; su migración a **SQL Server** se detalla en `modelo-de-datos-propuesto.md`.