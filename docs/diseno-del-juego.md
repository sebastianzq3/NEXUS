# Diseño del Juego y Experiencia de Usuario: NEXUS (v1.0 Windows Forms)

## 1. Visión del Juego
**NEXUS** es un **RPG táctico de supervivencia con componente educativo** para escritorio Windows (C# / Windows Forms). El jugador asume el rol de un Explorador encargado de estabilizar realidades multiversales colapsadas mientras evade a la IA desertora **I.R.I.S.**

La propuesta combina dos pilares de motivación:
1. **Riesgo vs. Recompensa:** Cada 3 pasos en la cuadrícula consumen 1 punto de Energía vital, obligando a planificar rutas hacia Restos Tecnológicos (`▤`), Núcleos de Energía (`Φ`), Enemigos (`Ж`) y Fisuras Cuánticas (`಄`).
2. **Aprendizaje Gamificado de C#:** Recargar energía exige superar un **Filtro de Seguridad C#** con preguntas reales de programación y POO, transformando la supervivencia en práctica activa de conocimientos universitarios.

---

## 2. Ciclo Principal de Juego (Core Loop) y Máquina de Estados
Al ejecutar `NEXUS.exe`, el jugador visualiza primero la pantalla de carga animada (`FrmSplash`), ingresa al **Gestor Biométrico de Perfiles** y entra a un ciclo gobernado por dos estados excluyentes:

### 2.1. Modo Campamento (Mapa Congelado / Fase de Gestión)
Al cargar un perfil o regresar de una expedición, el jugador permanece protegido en base:
- **Estado del Mapa:** El lienzo $40 \times 20$ se renderiza bajo un velo táctico traslúcido con borde neón pulsante, bloqueando desplazamientos accidentales.
- **Acciones Habilitadas en el Panel Táctico:**
  - **Explorar realidad:** Descongela el mapa, repuebla entidades según el nivel del jugador y asigna un objetivo secundario de expedición.
  - **Inventario y síntesis:** Inspección de ítems, descarte, síntesis de pistas sensoriales (3 Fragmentos) y crafteo de objetos (3 Restos `▤`).
  - **Recuperar energía [+5 Recarga]:** Abre el **Filtro de Seguridad C#** (trivia didáctica).
  - **Tienda de proficiencia [Mejoras]:** Inversión de Puntos de Proficiencia para ampliar mochila, vida máxima o batería cuántica.
  - **Estado y manual / Atlas de realidades:** Muestra el estado del explorador, el **porcentaje de precisión en trivias C#**, el manual de supervivencia y el escáner multiversal ($18 \times 12$).
  - **¡Iniciar extracción total! (100%):** Al alcanzar 100% de estabilidad otorga $+100\text{ EXP}$ y asigna una nueva realidad.
  - **Guardar y cambiar perfil:** Persiste el progreso en `%LocalAppData%\NEXUS\Saves` y vuelve al selector de perfiles.

### 2.2. Modo Exploración (Incursión Táctica Activa)
Al desplegarse en el sector, se bloquean los menús administrativos y el lienzo $40 \times 20$ cobra vida:
- **Desplazamiento y Energía:** Movimiento por teclado (`W, A, S, D` o Flechas) o clic ortogonal con el ratón. Cada 3 pasos se descuenta 1 de Energía; llegar a 0 activa una extracción de emergencia al campamento.
- **Retorno Contextual (`Volver al campamento [ESPACIO]`):** Solo se habilita (iluminándose en verde brillante) cuando el avatar `@` pisa el **Ancla Base (`☗`)** o un **Campamento Temporal (`☖`)**.

---

## 3. Mecánicas, Reglas y Retroalimentación Visual

### 3.1. Sistema Didáctico: Filtro de Seguridad C# (Recarga de Energía)
- **Disparadores:** Puede abrirse manualmente desde el Campamento o se dispara automáticamente al pisar un Núcleo de Energía (`Φ`) durante la exploración.
- **Dinámica:** Presenta 1 pregunta seleccionada sin repeticiones consecutivas de un banco barajado (Fisher-Yates) de **28 preguntas de C# y POO** (Sintaxis/Tipos, POO, Colecciones/LINQ y Excepciones/Memoria/.NET).
- **Presión Táctica:** Temporizador regresivo visual de **25 segundos** (barra + contador que cambia de verde a amarillo a los 10 s y rojo a los 5 s).
- **Resolución y Aprendizaje:** Al seleccionar una de las 4 opciones (`A, B, C, D`) o agotarse el tiempo, los botones se bloquean, se resalta la opción correcta en verde y el error en rojo, y se despliega una **Explicación Didáctica** obligatoria antes de pulsar `CONTINUAR`.
- **Balance:**
  - Acierto: **`+5 Energía`**.
  - Fallo o tiempo agotado con `Estabilidad > 50%`: **`+1 Energía`**.
  - Fallo o tiempo agotado con `Estabilidad <= 50%`: **`+1 Energía` y `-15% Estabilidad`**.

### 3.2. Animaciones y Feedback Kinestésico (30 FPS)
- **Deslizamiento Interpolado:** El marcador del cadete (`@`) se interpola suavemente entre casillas en lugar de teletransportarse bruscamente.
- **Auras Pulsantes y Scanline:** El jugador (`@`), las anomalías (`಄`), núcleos (`Φ`) y enemigos (`Ж`) emiten pulsos neón continuos; un *scanline* táctico translúcido recorre el mapa verticalmente.
- **Partículas y Textos Flotantes:** Eventos clave generan estallidos de partículas neón y texto ascendente que se desvanece (`+1 ▤`, `+HP`, `+30% EST`, `+1 FRAG`, `+75 EXP`, `+5 E`).
- **Combate Dinámico:** Los ataques pesados y el daño recibido disparan un destello de color (*flash*) y micro-sacudida (*screen shake*) breve en la ventana de combate.
- **Hover Interactivo:** Todos los botones tácticos y modales responden visualmente al paso del cursor.

### 3.3. Vigilancia de I.R.I.S.
- `Estabilidad > 50%`: Estado **Dormida**.
- `20% < Estabilidad <= 50%`: Estado **Acechando** (borde del mapa parpadea en naranja y alertas en telemetría).
- `Estabilidad <= 20%`: **Invasión Crítica** (interrupción modal que exige purgar toda la energía para recuperar `+15%` estabilidad o sufrir desconexión).

---

## 4. Pantallas Implementadas

| Pantalla | Componentes Clave | Propósito UX |
| :--- | :--- | :--- |
| **0. Splash Screen (`FrmSplash`)** | Logo panorámico (`nexus_logo.png`), borde neón pulsante, *scanline*, barra cuántica animada, créditos de autor (`Sebastián Zubieta Quiroga`). | Identidad visual profesional de arranque (omitible con clic o tecla). |
| **1. Gestor Biométrico (`pnlPerfiles`)** | Banner superior con `nexus_logo.png`, tabla `DataGridView` de perfiles en `%LocalAppData%`, formulario de alta (edad ≥ 18) y pie de créditos. | Selección, creación y purga rápida de partidas multi-slot. |
| **2. Simulador Principal (`pnlJuego`)** | HUD superior con 3 `ProgressBar` (Vida, Energía, Estabilidad), lienzo animado `BufferedCanvas` ($40 \times 20$), Panel Táctico contextual y Log `RichTextBox`. | Visibilidad total del estado del sector sin saturación cognitiva. |
| **3. Modales Especializados** | **Combate** (barras + FX), **Inventario/Síntesis**, **Filtro C#** (temporizador 25 s + código + explicación), **Tienda de Proficiencia**, **Atlas ($18 \times 12$)** y **Estado/Manual** (incluye precisión C#). | Enfocar decisiones críticas pausando el desplazamiento en el mapa. |

---

## 5. Criterios de Experiencia de Usuario (UX) y Alcance
- **Consistencia Estética:** Paleta cyberpunk oscura (`#0A0C12` / `RGB(14,16,22)`), tipografía monoespaciada `Consolas`, acentos semánticos (verde = jugador/éxito, cian = sistema, magenta = anomalías, amarillo = pistas/alertas, rojo = peligro).
- **Prevención de Errores:** Bloqueo contextual de botones según modo Campamento/Exploración, confirmación en acciones destructivas (purgar perfil/descartar ítem) y sanitización de nombres de archivo al crear perfiles.
- **Restricciones Respetadas:** Evolución fiel del código H3 hacia Windows Forms con instalador autónomo funcional en Windows y persistencia compatible con futuras migraciones a SQL Server.