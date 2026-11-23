# Análisis Técnico: Alternativas, Decisiones y Verificación Tecnológica

## 1. Introducción y Contexto del Problema
El proyecto **NEXUS** requirió una evolución técnica integral desde su prototipo en consola (H3) hacia un videojuego gráfico animado en **C# (.NET 10 / Windows Forms)** con enfoque didáctico, instalador autónomo para Windows y arquitectura preparada para persistencia en **Microsoft SQL Server**.

Este documento evalúa las alternativas técnicas analizadas, justifica las decisiones implementadas en la versión **v1.0.0** y las proyectadas para la fase SQL Server, detalla las fuentes de referencia y documenta la verificación crítica de la información obtenida con apoyo de Inteligencia Artificial.

---

## 2. Evaluación de Alternativas Técnicas

### 2.1. Capa de Presentación (Interfaz Gráfica)

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **Windows Forms + GDI+ (`BufferedCanvas`)** | Integración nativa con .NET 10; control directo de pintado 2D (`PaintEventArgs`) ideal para grilla $40 \times 20$ y animaciones ligeras; cumple el requisito contractual de la asignatura. | Renderizado por CPU; requiere doble búfer explícito y gestión manual (`Dispose`) de pinceles, fuentes y temporizadores para evitar parpadeo y fugas de memoria. | **SELECCIONADA** |
| **WPF (Windows Presentation Foundation)** | Aceleración por hardware (DirectX) y separación declarativa con XAML/MVVM. | Curva de configuración mayor; no se alinea con el requerimiento específico de Windows Forms establecido para el proyecto. | **DESCARTADA** |
| **Consola Avanzada (`Spectre.Console`)** | Mantiene la estética retro original de H3 y es multiplataforma. | Sin interacción fluida por ratón, sin animaciones gráficas reales ni identidad visual mediante assets PNG/ICO. | **DESCARTADA** |

---

### 2.2. Estrategia de Animación 2D en Windows Forms

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **Un `Timer` de UI a ~30 FPS (33 ms) + GDI+ en `BufferedCanvas`** | Movimiento interpolado suave, pulsos neón, *scanline* y partículas sin crear múltiples controles; cero parpadeo gracias a `DoubleBuffered = true`. | Requiere liberar recursos GDI+ con `using` en cada `OnPaint` y detener el temporizador en `FormClosing`. | **SELECCIONADA** |
| **Matriz de 800 `PictureBox` / `Button`** | Sencillo de conceptualizar inicialmente. | Agotamiento de manejadores de ventana (*GDI Handle exhaustion*), alto consumo de RAM y parpadeo severo al redibujar. | **DESCARTADA** |
| **Motor de Juegos Externo (MonoGame / Godot)** | Rendimiento gráfico superior por GPU. | Rompe la restricción de utilizar Windows Forms estándar y exige reescribir por completo los modales administrativos. | **DESCARTADA** |

---

### 2.3. Ruta de Almacenamiento Local y Estrategia de Persistencia

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **JSON en `%LocalAppData%\NEXUS\Saves` (v1.0.0 Actual)** | Totalmente compatible con instalaciones en `C:\Program Files\NEXUS` (sin errores UAC); cero dependencias externas para el instalador autónomo; migración automática desde `Saves/` local. | Sin soporte multiusuario concurrente ni consultas relacionales avanzadas. | **IMPLEMENTADA (v1.0)** |
| **JSON en carpeta relativa `"Saves"` junto al `.exe`** | Fácil de ubicar en desarrollo dentro de `bin\Debug`. | Falla con `UnauthorizedAccessException` al instalar el software en `Program Files` como usuario estándar. | **DESCARTADA (Sustituida)** |
| **SQL Server + ADO.NET (`Microsoft.Data.SqlClient`)** | Integridad referencial en 3FN, transacciones ACID (`SqlTransaction`), control total de consultas parametrizadas y soporte para banco centralizado de preguntas C#. | Requiere servicio SQL Server activo; mayor volumen de código de mapeo frente a un ORM. | **RECOMENDADA (Fase SQL)** |
| **Entity Framework Core (EF Core)** | Mapeo automático ORM y migraciones *Code-First*. | Mayor sobrecarga de memoria y abstracción menos transparente para demostrar control SQL y transacciones manuales en Programación II. | **DESCARTADA** |

---

### 2.4. Empaquetado y Distribución del Instalador (`.exe`)

| Alternativa Evaluada | Ventajas | Desventajas | Veredicto |
| :--- | :--- | :--- | :--- |
| **`dotnet publish` Self-Contained (`win-x64`, ReadyToRun) + Inno Setup** | Funciona en cualquier PC Windows limpia sin requerir .NET Runtime preinstalado; genera un único instalador profesional (`NEXUS_Setup_v1.0.exe`) con accesos directos, ícono y desinstalador limpio. | Tamaño mayor del ejecutable final al incluir el runtime embebido (~60–90 MB comprimido). | **SELECCIONADA** |
| **Publicación *Framework-Dependent* en archivo `.zip`** | Peso mínimo (pocos megabytes). | Falla si la PC destino no tiene instalado exactamente `.NET 10 Desktop Runtime`; experiencia menos profesional sin asistente ni desinstalador. | **DESCARTADA** |
| **ClickOnce / MSIX** | Integración directa con Visual Studio y actualizaciones automáticas. | Exige certificados de firma digital de confianza para evitar bloqueos en equipos limpios fuera de entorno corporativo. | **DESCARTADA** |

---

## 3. Decisiones Arquitectónicas Recomendadas y Aplicadas
1. **Separación de Temporizadores y Ciclo de Vida Seguro:** `timerAnimacion` (33 ms) queda restringido al renderizado en `BufferedCanvas`, mientras `timerIris` (1200 ms) gestiona la amenaza y los modales (Combate y Trivia C#) manejan sus propios temporizadores efímeros con liberación obligatoria en `FormClosed`.
2. **Desacoplamiento del Módulo Didáctico:** La lógica de preguntas de C# reside en `BancoPreguntasCSharp.cs` con barajado **Fisher-Yates**, lista para ser migrada a un repositorio SQL (`T_Preguntas_CSharp`) sin alterar el modal de interfaz.
3. **Resiliencia de Recursos Gráficos:** Todos los recursos de `Assets/` se cargan desde `AppContext.BaseDirectory` con captura segura de excepciones y respaldo dibujado en GDI+, evitando caídas si un archivo gráfico se mueve o elimina.
4. **Automatización de Release:** El flujo reproducible `scripts/publish.ps1` → `installer/NEXUS_Setup.iss` → `docs/pruebas-instalacion-limpia.md` asegura que cada nueva versión sea compilable y auditable en entorno limpio.

---

## 4. Fuentes Técnicas y Bibliografía de Referencia
1. **Microsoft Learn — Windows Forms & GDI+:** Documentación sobre `DoubleBuffered`, `ControlStyles.OptimizedDoubleBuffer`, `PaintEventArgs` y liberación de objetos `IDisposable` (`Pen`, `Brush`, `Font`).  
   URL: `https://learn.microsoft.com/dotnet/desktop/winforms/`
2. **Microsoft Learn — .NET Application Publishing (`dotnet publish`):** Guías oficiales sobre despliegues *self-contained*, identificadores de runtime (`win-x64`) y compilación *ReadyToRun*.  
   URL: `https://learn.microsoft.com/dotnet/core/deploying/`
3. **Microsoft Learn — ADO.NET (`Microsoft.Data.SqlClient`):** Buenas prácticas de ejecución asíncrona (`OpenAsync`, `ExecuteReaderAsync`) y manejo de transacciones `SqlTransaction`.  
   URL: `https://learn.microsoft.com/sql/connect/ado-net/`
4. **JRSoftware — Inno Setup Documentation:** Referencia oficial de directivas `[Setup]`, `[Files]`, `[Icons]`, `[Run]` y constante `{autopf}` para instaladores Windows de 64 bits.  
   URL: `https://jrsoftware.org/ishelp/`
5. **Martin, Robert C. — *Clean Architecture*:** Separación de intereses (SoC) y desacoplamiento entre presentación, lógica de dominio y persistencia.

---

## 5. Protocolo de Verificación de Información Obtenida con IA
Cada propuesta técnica asistida por IA fue sometida a validación práctica antes de integrarse al repositorio:

### 5.1. Verificación de Permisos de Escritura en `C:\Program Files`
- **Riesgo detectado:** El código original guardaba las partidas en `"Saves"` (ruta relativa junto al `.exe`), y una primera configuración del instalador no contemplaba restricciones UAC de Windows.
- **Prueba de verificación:** Al instalar en `{autopf}\NEXUS`, Windows restringe la escritura de usuarios estándar dentro de `Program Files`, provocando `UnauthorizedAccessException` al guardar perfiles JSON.
- **Corrección aplicada:** Se redirigió el almacenamiento a `Environment.SpecialFolder.LocalApplicationData` (`%LocalAppData%\NEXUS\Saves`) con migración automática de partidas locales y se incorporó su validación explícita en `docs/pruebas-instalacion-limpia.md`.

### 5.2. Verificación de Consumo de Memoria en Animación GDI+ a 30 FPS
- **Hallazgo de IA verificado:** Se propuso crear instancias de `SolidBrush`, `Pen` y `Font` en cada tick del `Paint` (30 veces por segundo) para partículas y textos flotantes.
- **Prueba de verificación:** Sin liberación explícita, GDI+ acumula manejadores nativos no administrados hasta degradar la interfaz.
- **Corrección aplicada:** Se envolvieron todas las instancias gráficas en declaraciones `using` y se aseguró la detención y `Dispose()` de `timerAnimacion`, `timerTrivia`, `timerFx` y `logoPerfilesImg` al cerrar los formularios.

### 5.3. Verificación de Bloqueos en Hilo de UI y Coherencia Relacional
- **Hallazgo de IA verificado:** Se evaluaron consultas síncronas (`ExecuteReader()`) y almacenamiento de inventarios como texto JSON dentro de una columna `NVARCHAR(MAX)` en SQL Server.
- **Prueba y corrección:** Ambas prácticas se descartaron: las llamadas síncronas congelan el *UI Thread* de WinForms (corregido mediante `async/await`), y el JSON embebido incumple la normalización 3FN exigida por la cátedra (corregido mediante tablas relacionales `T_Inventarios` y `T_Observaciones` con llaves foráneas en `modelo-de-datos-propuesto.md`).