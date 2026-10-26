# Diseño del Juego y Experiencia de Usuario: NEXUS

## 1. Visión del Juego
NEXUS transiciona de una aventura conversacional abstracta en consola a un **RPG táctico de supervivencia** con perspectiva top-down 2D (vista cenital). El jugador actúa como un Explorador encargado de adentrarse en sectores multiversales colapsados para estabilizarlos. 
La motivación principal del jugador será el *riesgo vs. recompensa*: cada paso consume energía vital, pero explorar más lejos otorga mejores recursos (restos y fragmentos) para mejorar su equipamiento base. Si la energía llega a cero, se pierde el progreso de esa expedición (mecánica *roguelike*).

## 2. Ciclo Principal de Juego (Core Loop)
El ciclo de juego se divide en dos fases claramente diferenciadas:

1. **Fase de Gestión (Hub/Base de Datos):**
   * El jugador inicia sesión (carga su perfil desde la BD).
   * Gasta sus "Puntos de Proficiencia" en la Tienda para mejorar sus estadísticas globales (más inventario, más vida, más energía).
   * Selecciona una nueva incursión al multiverso.

2. **Fase de Incursión (Exploración Táctica):**
   * Aparece en una grilla 2D ($40 \times 20$ casillas visuales).
   * Se mueve estratégicamente. Cada 3 pasos consume 1 punto de Energía.
   * Resuelve eventos locales: recoge restos (`▤`), recarga energía (`Φ`), o entra en combate contra virus (`Ж`).
   * **Objetivo principal:** Encontrar la Anomalía (`಄`), abrir el inventario y deducir qué objeto usar para sellarla.
   * **Extracción:** Llegar al ancla (`☗`) antes de morir para asegurar la experiencia ganada.

## 3. Mecánicas y Reglas Adaptadas a Interfaz Gráfica
* **Movimiento:** En lugar de `Console.ReadKey()`, el jugador hará clic en la casilla adyacente o usará las flechas del teclado, activando un evento `KeyDown` en Windows Forms que desplazará su avatar (sprite).
* **Feedback Visual de I.R.I.S.:** La amenaza constante de la IA se representará visualmente. Si la estabilidad cae, los bordes de la pantalla (el formulario) parpadearán en rojo y la música se distorsionará.
* **Combate Modal:** Al chocar con un enemigo, se abrirá un formulario modal (`FrmCombate.ShowDialog()`). Aquí el combate por turnos será impulsado por botones gráficos (*Ataque Ligero*, *Pesado*, *Bloqueo*), con barras de progreso animadas para la salud.

## 4. Diseño de Pantallas (Wireframes y Navegación)

### Pantalla 1: Menú Principal (Gestor de Perfiles)
* **Visual:** Fondo tecnológico oscuro. Un título grande "NEXUS" en la parte superior.
* **Componentes:** Un `DataGridView` central listando los exploradores registrados (Nombre, Nivel, Experiencia).
* **Acciones:** Botones para "Crear Nuevo Explorador", "Seleccionar Perfil", "Borrar Registro".

### Pantalla 2: Interfaz de Exploración (El Mapa)
* **HUD Superior:** Barras de estado usando `ProgressBar` modificadas para Vida (Rojo), Energía (Cian) y Estabilidad (Morado).
* **Área Central:** Un control `TableLayoutPanel` o un área de dibujado (`Paint` event) donde cada celda renderiza un icono o color en lugar de caracteres ANSI. El cadete será un icono distintivo.
* **Panel Lateral (Derecho):** Un cuadro de texto (`RichTextBox`) que actuará como Log del sistema, mostrando mensajes como *"Absorbiste un núcleo (+3 E)"*. Botones de acceso rápido al "Inventario" y "Extraer".

### Pantalla 3: Inventario y Síntesis
* **Visual:** Ventana emergente tipo "Pip-Boy".
* **Izquierda:** Lista de los objetos actuales del jugador.
* **Derecha:** Panel de detalles mostrando descripción, usos restantes y botones de "Equipar/Usar" y "Descartar". En la parte inferior, los recursos totales y la opción de "Ensamblar Objeto".

### Pantalla 4: Confrontación (Combate)
* **Visual:** Fondo estático de batalla.
* **Arriba:** Stats del enemigo (Avatar y barra de vida).
* **Abajo:** Stats del jugador y panel de 4 grandes botones de acción (Ataque Ligero, Pesado, Bloquear, Escapar). 

## 5. Experiencia de Usuario (UX) y Restricciones
* **Consistencia:** Se usarán paletas de colores oscuras (fondos grises/negros) con acentos neón (cyan, magenta, verde) para mantener la identidad visual del H3.
* **Retroalimentación:** Toda acción del usuario (moverse, atacar, craftear) debe tener un impacto visual inmediato (actualización de barras, texto en el log, cambio de color en la casilla).
* **Restricciones Técnicas:** Se usará estrictamente GDI+ nativo de Windows Forms. No se implementarán animaciones complejas de interpolación (como en motores Unity/Unreal) para no saturar el hilo principal de la UI.