# Arquitectura Actual del Sistema: NEXUS (Versión Windows Forms)

## 1. Resumen Ejecutivo y Estado del Proyecto
El estado actual de **NEXUS** corresponde a una aplicación gráfica interactiva de escritorio desarrollada sobre **.NET (C#) y Windows Forms** dentro del proyecto `Interfaz`. El sistema ha completado su migración desde el entorno monolítico de consola (Hito 3) hacia una arquitectura **Orientada a Eventos (*Event-Driven*)** con renderizado gráfico 2D mediante **GDI+**.

El núcleo del sistema conserva el 100% de la lógica de dominio orientada a objetos (herencia, polimorfismo, encapsulamiento de estados, generación procedural de mapas y persistencia multi-slot en JSON), eliminando por completo las llamadas bloqueantes de consola (`Console.ReadKey`, `Thread.Sleep`) y las dependencias de `kernel32.dll` para códigos ANSI.

---

## 2. Diagrama de Estructura de Clases Actual

```text
                        +------------------------+
                        |      Form1 : Form      |
                        +------------------------+
                        | - canvasMapa           |
                        | - timerIris            |
                        | - enExpedicion: bool   |
                        | + ProcesarMovimiento() |
                        | + ActualizarHUD()      |
                        +-----------+------------+
                                    | (Controla y Renderiza)
                                    v
                        +-------------------+
                        |      Entidad      |
                        +-------------------+
                        | + VidaMax: int    |
                        | - _vida: int      |
                        | + DañoBase: int   |
                        +-------------------+
                                  ^
                                  | (Herencia)
                 +----------------+----------------+
                 |                                 |
        +-----------------+               +-----------------+
        |     Usuario     |               |     Enemigo     |
        +-----------------+               +-----------------+
        | + Nombre        |               | + Nombre        |
        | + EnergiaMax    |               | + ProbEscape    |
        | + Inventario    |               +-----------------+
        | + Observaciones |
        +-----------------+

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
| + Jugador          |     | + Atlas[,]         |     | + DoubleBuffered   |
| + RealidadActual   |     | + ActualizarAtlas()|     |   (Herencia Panel) |
+--------------------+     +--------------------+     +--------------------+
```

---

## 3. Inventario y Análisis de Componentes

### 3.1. `Form1` y `Form1.Designer.cs` (Controlador de UI y Eventos)
* **Propósito:** Gestiona el ciclo de vida visual de la aplicación, alternando entre el panel de selección de perfiles (`pnlPerfiles`) y el entorno de simulación (`pnlJuego`).
* **Componentes técnicos clave:**
  * **Máquina de Estados (`enExpedicion`):** Regula de forma reactiva qué botones del Panel Táctico están habilitados. Cuando `enExpedicion == false` (Modo Campamento), el mapa permanece congelado y se habilitan todas las funciones de gestión; cuando `enExpedicion == true`, bloquea las opciones de campamento y solo habilita el botón verde `"Volver al campamento [ESPACIO]"` al pisar las casillas `☗` o `☖`.
  * **Captura de Entrada No Bloqueante:** Uso de `Form1_KeyDown` (`KeyPreview = true`) y `CanvasMapa_MouseClick` para traducir pulsaciones y clics ortogonales en desplazamientos sobre la matriz $40 \times 20$.
  * **Monitoreo Asíncrono con `Timer`:** El componente `timerIris` ejecuta ticks periódicos cada $1200\text{ ms}$ para evaluar la vulnerabilidad del sector y renderizar alertas visuales sin congelar el hilo principal de la UI (*UI Thread*).
  * **Sub-formularios Modales Dinámicos:** Generación encapsulada de diálogos modales (`ShowDialog`) para Combate, Inventario/Síntesis, Soporte Vital (con temporizador propio para pruebas de memoria), Tienda de Proficiencia y el Atlas Multiversal.

### 3.2. `BufferedCanvas` (Motor de Renderizado GDI+)
* **Propósito:** Clase derivada de `System.Windows.Forms.Panel` que activa `DoubleBuffered = true` y los estilos `AllPaintingInWmPaint`, `UserPaint` y `OptimizedDoubleBuffer`.
* **Ventaja arquitectónica:** Evita la instanciación de $800$ controles visuales para el mapa de $40 \times 20$. En su lugar, dibuja toda la escena en memoria de video mediante `CanvasMapa_Paint` (`Graphics`), eliminando el parpadeo (*flickering*).

### 3.3. Clases de Dominio (`Entidad`, `Usuario`, `Enemigo`, `Realidad`, `Objeto`, `EntidadIris`, `AtlasRealidades`)
* Conservan las reglas de negocio del Hito 3 adaptadas para serialización limpia en `System.Text.Json`:
  * `Realidad` incluye el método `InicializarMapaSiNoExiste()` para reconstruir la matriz `char[,] MapaLocal` (marcada con `[JsonIgnore]`) tras deserializar un perfil guardado.
  * `EntidadIris` desacopla sus mensajes de consola devolviendo alertas mediante parámetros `out string mensajeAlerta` que se inyectan directamente en el `RichTextBox` de telemetría.

---

## 4. Superación de las Limitaciones de la Versión H3 (Consola)

| Limitación en H3 (Consola) | Solución Implementada en Windows Forms (`Interfaz`) |
| :--- | :--- |
| **Interfaz bloqueante (`Console.ReadKey` / `Thread.Sleep`)** | Reemplazada por eventos asíncronos de Windows Forms (`KeyDown`, `MouseClick`, `Timer.Tick`). |
| **Dependencia de `kernel32.dll` y códigos ANSI** | Eliminada. El renderizado utiliza primitivas gráficas nativas de GDI+ (`Graphics`, `SolidBrush`, `Pen`). |
| **Falta de control de estado en menús** | Implementación de bloqueo contextual de botones (`ConfigurarEstadoBoton`) según si el jugador está en Campamento o en Exploración. |
| **Pérdida de propiedades al deserializar JSON** | Ajuste de descriptores de acceso (`get; set;`) en `Objeto` y `Realidad` para garantizar la reconstrucción íntegra del inventario y estabilidad. |