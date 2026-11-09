# Diseño del Juego y Experiencia de Usuario: NEXUS (Windows Forms)

## 1. Visión del Juego
NEXUS ha evolucionado de una aventura conversacional en consola a un **RPG táctico de supervivencia** en escritorio con perspectiva top-down 2D sobre **Windows Forms**. El jugador actúa como un Explorador encargado de adentrarse en sectores multiversales colapsados para estabilizarlos.
La motivación principal del jugador radica en el *riesgo vs. recompensa*: cada paso en el sector consume energía vital, pero explorar más lejos otorga recursos esenciales (Restos Tecnológicos `▤` y Fragmentos de Anomalía) para sintetizar equipamiento y deducir cómo sellar las fisuras cuánticas (`಄`).

## 2. Ciclo Principal de Juego (Core Loop) y Máquina de Estados
El ciclo de juego opera bajo una máquina de estados estricta que alterna entre dos modos contextuales:

### 2.1. Modo Campamento (Mapa Congelado / Fase de Gestión)
Al cargar un perfil o regresar de una incursión, el jugador se encuentra a salvo en el campamento:
* **Estado del Mapa:** El lienzo $40 \times 20$ se muestra congelado bajo un velo táctico traslúcido, bloqueando el desplazamiento para evitar gastos accidentales de energía.
* **Opciones Habilitadas en el Panel Táctico:**
  * **Explorar realidad:** Descongela el mapa, repuebla las entidades del sector y asigna una misión secundaria de expedición.
  * **Inventario y síntesis:** Abre el gestor para inspeccionar ítems, descartar equipo roto, sintetizar pistas (3 Fragmentos) o ensamblar objetos (3 Restos `▤`).
  * **Recuperar energía [+5 Recarga]:** Inicia el protocolo de soporte vital mediante minijuegos de seguridad.
  * **Tienda de proficiencia [Mejoras]:** Permite invertir Puntos de Proficiencia para ampliar la mochila, la vida máxima o la batería cuántica.
  * **Estado y manual / Atlas de realidades:** Consulta de progreso y escaneo del mapa multiversal de $18 \times 12$ dividido en 4 cuadrantes.
  * **¡Iniciar extracción total! (100%):** Disponible al alcanzar el 100% de estabilidad para asegurar $+100\text{ EXP}$ y saltar a una nueva realidad.
  * **Guardar y cambiar perfil:** Almacena el estado en JSON y retorna al selector biométrico.

### 2.2. Modo Exploración (Incursión Táctica Activa)
Al desplegarse en la realidad, el mapa se activa y los botones administrativos del campamento se bloquean temporalmente:
* **Desplazamiento y Supervivencia:** El jugador se mueve por la grilla $40 \times 20$. Cada 3 pasos consume 1 punto de Energía. Si la energía cae a 0, el soporte vital colapsa y el explorador es extraído de emergencia de vuelta al campamento.
* **Retorno Contextual (`Volver al campamento [ESPACIO]`):** Durante la exploración, el botón superior del Panel Táctico permanece deshabilitado salvo cuando el cadete se posiciona exactamente sobre el **Ancla Base (`☗`)** o un **Campamento Temporal (`☖`)**. En ese instante, el botón **se ilumina en color verde brillante** y permite regresar al Modo Campamento (consumiendo el campamento temporal si se usó `☖`).

## 3. Mecánicas y Reglas en Interfaz Gráfica
* **Movimiento Híbrido:** El jugador se desplaza utilizando el teclado (`W, A, S, D` o Flechas direccionales) mediante el evento `KeyDown`, o haciendo clic directamente con el ratón sobre cualquier casilla ortogonalmente adyacente.
* **Recarga de Energía Contextual:** Los protocolos de recarga solo pueden iniciarse manualmente desde el Campamento (mapa congelado), o bien **se disparan automáticamente** en medio de la exploración cuando el jugador pisa un Núcleo de Energía (`Φ`).
* **Feedback Visual de I.R.I.S.:** Un temporizador asíncrono (`timerIris`) evalúa la estabilidad de la realidad. Si cae por debajo del 50%, el marco del mapa parpadea en naranja/rojo y la IA emite alertas en el registro de telemetría. Al bajar del 20%, interrumpe la partida exigiendo una purga total de energía.
* **Interacciones Modales:** Chocar con un enemigo (`Ж`) o pisar una fisura (`಄`) despliega ventanas modales dedicadas que pausan el movimiento en el mapa hasta resolver el encuentro.

## 4. Diseño de Pantallas Implementadas

### Pantalla 1: Gestor Biométrico de Perfiles
* **Componentes:** Tabla `DataGridView` central que lista todos los perfiles alojados en `Saves/*.json` (Designación, Edad, Nivel, Vida, Energía, Sector y Estabilidad).
* **Acciones:** Panel de registro con validación de edad mínima (18 años), botón de carga rápida y purga definitiva de archivos.

### Pantalla 2: Interfaz Principal (HUD + Mapa GDI+ + Panel Táctico)
* **HUD Superior:** Indicadores de nivel, estado de I.R.I.S., objetivo secundario activo y tres barras de progreso (`ProgressBar`) para Vida, Energía y Estabilidad.
* **Lienzo Central (`BufferedCanvas`):** Panel personalizado de $840 \times 520\text{ px}$ con `DoubleBuffered = true` que dibuja la matriz $40 \times 20$ mediante GDI+ sin parpadeos.
* **Panel Táctico Lateral (Derecho):** Botonera limpia sin numeración con bloqueo contextual según el estado del juego, acompañada en la parte inferior por el **Registro de Telemetría (`RichTextBox`)** con marcas de tiempo y código de colores.

### Pantalla 3: Ventanas Modales (Inventario, Combate, Tienda, Soporte Vital y Atlas)
* **Inventario y Síntesis:** Selector interactivo (`ListBox`) vinculado en tiempo real a un visor de análisis técnico, lista de observaciones desbloqueadas y botones de crafteo/despliegue en anomalías.
* **Combate Táctico:** Barras de salud enfrentadas para el Cadete y el Enemigo con botones para *Ataque Ligero (0 E)*, *Ataque Pesado (1 E)*, *Bloquear (-50% daño)* y *Escapar (1 E)*.
* **Atlas de Realidades:** Renderizado gráfico GDI+ de la matriz $18 \times 12$ coloreada por cuadrantes cuánticos (Tiempo, Espacio, Mente y Silencio) con escáner de coordenadas.

## 5. Experiencia de Usuario (UX) y Rendimiento
* **Consistencia Visual:** Paleta cyberpunk oscura (`RGB(14, 16, 22)`) con tipografía monoespaciada (`Consolas`) y acentos neón semánticos (Verde para éxito/jugador, Cian para sistema, Violeta para anomalías, Rojo para peligro).
* **Optimización GDI+:** El uso de un único control `BufferedCanvas` evita la creación de 800 controles individuales en memoria, garantizando una respuesta instantánea al teclado y ratón.