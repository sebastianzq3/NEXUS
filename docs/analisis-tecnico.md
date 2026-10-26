# Análisis Técnico: Alternativas, Decisiones y Verificación Tecnológica

## 1. Introducción y Contexto del Problema
El proyecto **NEXUS** requiere una evolución arquitectónica fundamental para superar las restricciones de su versión de consola (H3), con el fin de migrar hacia un videojuego gráfico de escritorio basado en **C# con Windows Forms** y un motor de base de datos relacional robusto sobre **Microsoft SQL Server**.

El objetivo de este documento es evaluar comparativamente las alternativas técnicas viables, fundamentar las decisiones de ingeniería de software adoptadas, detallar las fuentes bibliográficas de referencia y documentar el protocolo de verificación de la información técnica asistida por Inteligencia Artificial.

---

## 2. Evaluación de Alternativas Técnicas

### 2.1. Capa de Presentación (Interfaz Gráfica)

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **Windows Forms (WinForms)** | Integración nativa e inmediata con el runtime de .NET; curva de aprendizaje accesible; manejo directo de eventos GDI+ (`PaintEventArgs`) para renderizado en cuadrícula 2D; requisito contractual del proyecto. | Modelo de pintado clásico por CPU; riesgo de parpadeo (*flickering*) en animaciones si no se configura doble búfer (*DoubleBuffering*); no desacopla el diseño de forma declarativa como XAML. | **SELECCIONADA** |
| **WPF (Windows Presentation Foundation)** | Separación estricta entre vista y lógica mediante XAML y patrón MVVM; aceleración por hardware (DirectX); soporte superior para animaciones y transformaciones de interfaz. | Mayor complejidad de configuración y curva de aprendizaje; no cumple con el requerimiento académico específico establecido para el Hito. | **DESCARTADA** |
| **Consola Avanzada (Spectre.Console / Terminal.Gui)** | Conserva la estética original retro/cyberpunk; portabilidad en múltiples plataformas y terminales. | Persiste la limitación de interacción por ratón; resolución gráfica restringida a caracteres ANSI; no satisface la transición a interfaz gráfica solicitada. | **DESCARTADA** |

---

### 2.2. Capa de Acceso a Datos y Persistencia

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **ADO.NET Puro (`Microsoft.Data.SqlClient`) con Procedimientos Almacenados** | Control granular absoluto sobre las consultas; ejecución de alto rendimiento con latencia mínima; comprensión transparente de la interacción transaccional con SQL Server; ideal para contextos académicos rigurosos. | Requiere mayor volumen de código repetitivo (*boilerplate*); mapeo manual de objetos relacionales (`SqlDataReader` a clases POCO). | **RECOMENDADA (Híbrida)** |
| **Entity Framework Core (EF Core 8/9)** | Mapeo Objeto-Relacional (ORM) automatizado; soporte nativo para migraciones y Change Tracking; abstracción fluida con LINQ. | Sobrecarga de memoria en aplicaciones ligeras de escritorio; curva de configuración de DbContext y generación de claves ajenas complejas para inventarios dinámicos. | **DESCARTADA** |
| **Dapper (Micro-ORM)** | Rendimiento prácticamente idéntico a ADO.NET manual; simplifica el mapeo relacional a objetos de dominio; permite escribir SQL directo sin la sobrecarga de un ORM completo. | Requiere dependencias externas mediante NuGet; menor visibilidad pedagógica de los mecanismos internos de conexión `SqlConnection`/`SqlCommand`. | **COMPLEMENTARIA** |
| **Archivos JSON Planos (Estado Actual)** | Portabilidad simple; cero dependencias de servidores externos. | Nulo aislamiento transaccional (ACID); riesgo crítico de corrupción por concurrencia; ineficiente para consultas estructuradas o auditorías. | **DESCARTADA (Sustituida)** |

---

### 2.3. Motor de Persistencia Relacional

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **Microsoft SQL Server (Developer / Express Edition)** | Soporte enterprise para integridad referencial foránea, tipos complejos, transacciones ACID nativas, índices agrupados y procedimientos almacenados; estándar en la industria .NET. | Requiere la instalación del servicio local o contenedor Docker (`mssql-server`); mayor consumo de recursos en la máquina cliente. | **SELECCIONADA** |
| **SQLite** | Motor embebido sin necesidad de configurar servidor; almacenamiento en archivo único local; altamente portable. | Menor compatibilidad con tipos y sintaxis avanzados de T-SQL; restricciones de concurrencia en escrituras complejas; no cumple con la restricción técnica del hito. | **DESCARTADA** |

---

## 3. Decisiones Arquitectónicas Recomendadas

A partir de la evaluación técnica, se definen los siguientes lineamientos obligatorios para la evolución del código:

1. **Adopción de Arquitectura en Capas Limpias (N-Tier / Clean Architecture Simplificada):**
   * **Capa de Presentación:** Formularios Windows Forms independientes (`FrmMenuPrincipal`, `FrmExploracion`, `FrmCombate`, `FrmInventario`).
   * **Capa de Lógica de Negocio (BLL / Dominio):** Clases puras de C# sin dependencias visuales (`CadeteService`, `CombateManager`, `ExploracionEngine`, `IrisAIController`).
   * **Capa de Acceso a Datos (DAL):** Repositorios dedicados (`UsuarioRepository`, `PartidaRepository`, `RealidadRepository`) que interactúen con SQL Server mediante `Microsoft.Data.SqlClient`.

2. **Mitigación de Rendimiento Visual en Windows Forms:**
   * La cuadrícula de exploración de $40 \times 20$ no debe implementarse mediante una matriz pesada de $800$ controles `Button` o `Label`, ya que esto saturaría el manejador de ventanas del sistema operativo (*Handle exhaustion*).
   * **Solución Técnica:** Se implementará un control de usuario personalizado (*UserControl*) o un `PictureBox` principal con `DoubleBuffered = true`, renderizando el mapa en memoria mediante el evento `OnPaint` y la clase `Graphics` (GDI+).

3. **Estrategia de Normalización en Base de Datos:**
   * Sustituir la persistencia atómica monolítica de JSON por un esquema relacional normalizado en 3FN:
     * `Jugadores` (Datos biométricos y estado base).
     * `Realidades` (Estado, estabilidad, coordenadas y tipo de anomalía).
     * `Inventario` (Relación relacional N:M entre el jugador y los objetos catalogados, registrando durabilidad actual).
     * `Observaciones` (Pistas sensoriales desbloqueadas por partida).
     * `EstadisticasPartida` (Historial de expediciones, enemigos eliminados y veces que IRIS fue repelida).

---

## 4. Fuentes Técnicas y Bibliografía de Referencia

1. **Microsoft Learn - Windows Forms Documentation:**
   * *Architecture and Rendering in Windows Forms:* Documentación sobre el ciclo de vida de controles, despacho de mensajes de Windows (`WndProc`) y gestión de repintado sin parpadeos mediante `SetStyle(ControlStyles.OptimizedDoubleBuffer, true)`.
   * URL: `https://learn.microsoft.com/dotnet/desktop/winforms/`
2. **Microsoft Learn - ADO.NET and SqlClient:**
   * *Best Practices for Asynchronous Database Operations:* Guías para el uso de `OpenAsync()`, `ExecuteReaderAsync()` y transacciones con `SqlTransaction` para evitar congelar el hilo de interfaz de usuario (`UI Thread`).
   * URL: `https://learn.microsoft.com/dotnet/framework/data/adonet/`
3. **Martin, Robert C. (Uncle Bob) - "Clean Architecture":**
   * Principio de Inversión de Dependencias (DIP) y Separación de Intereses (SoC) aplicados al desacoplamiento entre UI y lógica de simulación.
4. **Richter, Jeffrey - "CLR via C#":**
   * Gestión de memoria del Garbage Collector al manipular colecciones dinámicas y matrices bidimensionales continuas en juegos basados en .NET.

---

## 5. Protocolo de Verificación de Información Obtenida con IA

Para garantizar la fiabilidad técnica del código generado y de la documentación, se ejecutó un protocolo de verificación sistemático en tres etapas:

### 5.1. Verificación Sintáctica y de Compatibilidad de APIs
* **Hallazgo de IA verificado:** Durante el análisis de WinForms, se propuso el uso de `ControlStyles.AllPaintingInWmPaint` junto a `DoubleBuffer`.
* **Prueba de Verificación:** Se consultó la documentación oficial de .NET 8/9, confirmando que la asignación directa de la propiedad `DoubleBuffered = true` en controles derivados de `Panel` o formularios encapsula estos flags de forma segura sin requerir llamadas nativas a Win32.
* **Resultado:** **Validado y adoptado.**

### 5.2. Verificación de Bloqueos en Hilos de UI (Deadlocks)
* **Hallazgo de IA verificado:** Se sugirió el uso de llamadas síncronas a base de datos (`cmd.ExecuteReader()`) dentro de botones de acción en formularios.
* **Prueba de Verificación:** Al realizar operaciones de entrada/salida de red contra SQL Server en el hilo principal de WinForms, la ventana se congela e inhabilita eventos de teclado.
* **Corrección:** Se descartó la sugerencia inicial síncrona y se estandarizó el uso de patrones asíncronos `async/await` (`await cmd.ExecuteReaderAsync()`) para mantener una interfaz reactiva.

### 5.3. Integridad Relacional vs. Serialización NoSQL
* **Hallazgo de IA verificado:** Una sugerencia automatizada propuso almacenar el inventario completo como un campo `NVARCHAR(MAX)` con un string JSON embebido dentro de la tabla de SQL Server.
* **Prueba de Verificación:** Aunque técnicamente viable mediante `JSON_VALUE`, esta alternativa viola los estándares de diseño relacional exigidos por la cátedra para el modelado de datos en SQL Server y obstaculiza la integridad referencial de claves foráneas con la tabla `Objetos`.
* **Corrección:** Se forzó el diseño relacional formal mediante una tabla intermedia `InventariosJugador` con llaves foráneas a `Jugadores` y `ObjetosCatalogo`.