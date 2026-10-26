# Arquitectura Actual del Sistema: NEXUS (Versión H3)

## 1. Resumen Ejecutivo y Estado del Proyecto
El estado actual de **NEXUS** (Hito 3) corresponde a un videojuego de rol, exploración procedural y gestión táctica ejecutado en modo consola interactiva sobre la plataforma **.NET (C#)**. El sistema combina mecánicas *roguelike*, combate por turnos, recolección de chatarra/recursos, síntesis de equipo (*crafting*) y resolución de anomalías multiversales mediante deducción lógica.

Técnicamente, el núcleo del programa reside en un archivo monolítico (`Program.cs`) bajo el espacio de nombres `NEXUS`. A pesar de encontrarse en una sola unidad de compilación, implementa Programación Orientada a Objetos (POO) mediante herencia, encapsulamiento básico, polimorfismo incipiente y serialización de datos estructurados en formato JSON.

---

## 2. Diagrama de Estructura de Clases Actual

```
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
| + MapaLocal[,]     |     | + Usos             |     | + VecesRepelida    |
+--------------------+     +--------------------+     +--------------------+

+--------------------+     +--------------------+     +--------------------+
|    DatosPartida    |     |  AtlasRealidades   |     |      Program       |
+--------------------+     +--------------------+     +--------------------+
| + Jugador          |     | + Atlas[,]         |     | + Main()           |
| + RealidadActual   |     | + GenerarAtlas()   |     | + Render / UI ANSI |
+--------------------+     +--------------------+     +--------------------+
```

---

## 3. Inventario y Análisis de Clases Existentes

### 3.1. `Program` (Controlador Central y Capa de Presentación)
* **Propósito:** Actúa como punto de entrada de la aplicación (`Main`), controlador del bucle principal de juego (*Game Loop*), motor de renderizado de texto mediante secuencias de escape ANSI e inyector de interoperabilidad nativa de Windows.
* **Componentes clave:**
  * **P/Invoke Win32 API:** Uso de `kernel32.dll` (`GetStdHandle`, `GetConsoleMode`, `SetConsoleMode`) para activar el flag `ENABLE_VIRTUAL_TERMINAL_PROCESSING` (0x0004), permitiendo la interpretación de colores VT100/ANSI en consolas Windows clásicas.
  * **Sistema de Renderizado:** Métodos auxiliares `DibujarPanelInfo()`, `DibujarBotonMenu()`, `MostrarPortadaAnimada()`, `EfectoMecanografia()` y un algoritmo de ajuste de palabras en pantalla (*Word Wrap*) basado en expresiones regulares (`Regex.Replace` para omitir códigos ANSI en el cómputo de ancho visual).
  * **Bucle de Exploración (Caso 1):** Captura síncrona no bloqueante de pulsaciones (`Console.ReadKey(true)`) para desplazamiento matricial $40 \times 20$, consumo de energía por pasos (1 de energía cada 3 pasos), y resolución de casillas dinámicas.

### 3.2. `Entidad` (Clase Base)
* **Propósito:** Modela el comportamiento de atributos vitales comunes.
* **Atributos:**
  * `VidaMax` (`int`): Límite superior de salud.
  * `Vida` (`int`): Propiedad con encapsulamiento que previene valores negativos ($< 0$) o desbordamientos por encima de `VidaMax`.
  * `DañoBase` (`int`): Potencia base de impacto físico.

### 3.3. `Usuario` (Especialización de `Entidad`)
* **Propósito:** Representa al avatar del jugador (*Cadete*) y su estado persistente.
* **Mecánicas asociadas:**
  * **Energía:** Propiedad `Energia` acotada entre $0$ y `EnergiaMax`.
  * **Nivel y Experiencia:** Acumulador de experiencia donde cada 100 EXP incrementa el `Nivel` y otorga 1 punto a `PuntosProficiencia`.
  * **Inventario y Recursos:** Lista de objetos de tipo `Objeto`, contador de `BolsaFragmentos` (moneda de pistas) y `Restos` (moneda de ensamblaje de objetos).
  * **Árbol de Mejoras (Tienda):** `UpgradesInv`, `UpgradesVida`, `UpgradesEnergia` gobernados por el método matemático `CalcularCosto()`, aplicando una curva de costes indexada `[1, 1, 2, 3, 5]`.

### 3.4. `Enemigo` (Especialización de `Entidad`)
* **Propósito:** Modela las amenazas de combate dentro del sector.
* **Mecánicas asociadas:** Generación aleatoria entre 4 arquetipos (*Glitch-Arácnido, Dron Corrupto, Sombra Cuántica, Espectro Viral*), asignación de salud dinámica ($40-60$), daño ($10-15$) y cálculo de porcentaje de escape ($30\%-80\%$).

### 3.5. `Realidad`
* **Propósito:** Entorno multiversal donde se sitúa la expedición del cadete.
* **Mecánicas asociadas:**
  * Atributo `Estabilidad` ($0-100\%$).
  * Tipo de anomalía asociada (`Anomalia` Enum: Tiempo, Espacio, Mente, Silencio).
  * Generación matricial procedimental: `char[,] MapaLocal` de $40 \times 20$ celdas, con cálculo de degradación/ruido en bordes mediante distancia euclidiana/Manhattan respecto a los límites, posicionamiento fijo del ancla de la base (`☗`) y repoblación de entidades dinámicas mediante `RellenarEntidadesLocales()`.

### 3.6. `Objeto`
* **Propósito:** Ítems recolectables o sintetizables capaces de interactuar con anomalías.
* **Mecánicas asociadas:** Posee un catálogo estático bidimensional de 40 ítems (10 por cada anomalía) con descripciones temáticas de *lore*. Cada objeto contrarresta un tipo de anomalía específica y cuenta con un límite de durabilidad (`Usos`).

### 3.7. `EntidadIris`
* **Propósito:** Simula la IA antagonista principal y amenaza latente del sistema.
* **Mecánicas asociadas:** Máquina de estados finita basada en el enum `EstadoIris` (`Dormida`, `Acechando`, `InvasionCritica`). El método `EscanearVulnerabilidad()` monitorea la estabilidad de la realidad asignada y `Asimilar()` ejecuta una interrupción crítica del ciclo de juego si la estabilidad cae por debajo del $20\%$, exigiendo el sacrificio total de energía del jugador.

### 3.8. `AtlasRealidades`
* **Propósito:** Matriz global de coordenadas espaciales ($18 \times 12$).
* **Mecánicas asociadas:** Estructura estática organizada en 4 cuadrantes cartesianos, donde cada cuadrante alberga anomalías afines (Q1: Tiempo, Q2: Espacio, Q3: Mente, Q4: Silencio). Dibuja el mapa general mediante códigos ANSI diferenciando mundos inestables (`[?]`) de realidades estabilizadas y extraídas (`[*]`).

### 3.9. `DatosPartida`
* **Propósito:** Data Transfer Object (DTO) plano utilizado para la serialización y deserialización del estado del juego en archivos `.json` dentro del directorio local `/Saves`.

---

## 4. Funcionalidades Principales Implementadas

1. **Gestión de Perfiles y Persistencia Multi-Slot:**
   * Creación, lectura de directorio `Saves/`, deserialización tolerante a fallos (`try/catch` para archivos corruptos) y purga de partidas.
2. **Exploración Táctica en Rejilla 2D:**
   * Movimiento en 4 direcciones (WASD / Flechas), detección de colisiones con el vacío (`█`, `▓`), consumo metabólico de energía y eventos interactivos en celdas:
     * `Φ`: Núcleos de recarga energética.
     * `♡`: Regeneración biológica de vida.
     * `▤`: Restos tecnológicos para crafteo.
     * `Ж`: Combate obligatorio por turnos.
     * `಄`: Fisura de anomalía multiversal (requiere resolución por objeto).
     * `?`: Evento estocástico sorpresa.
     * `☗` / `☖`: Puntos de extracción autorizados.
3. **Misiones Secundarias de Expedición:**
   * Generación procedural al iniciar la incursión de objetivos temporales: Exterminador, Curioso o Chatarrero, recompensando con experiencia extra al completarse.
4. **Combate Táctico por Turnos:**
   * Acciones: Ataque Ligero, Ataque Pesado (gasto de energía), Guardia/Bloqueo (reducción del 50% de daño recibido) y Huida bajo probabilidad porcentual.
5. **Crafteo y Deducción Lógica:**
   * Conversión de Fragmentos en pistas sensoriales asociadas al tipo de anomalía.
   * Ensamblado de piezas tecnológicas en objetos funcionales.
   * Cierre de fisuras: el objeto debe coincidir con el tipo oculto de la anomalía para otorgar $+30\%$ de estabilidad; si no coincide, penaliza con $-15\%$.
6. **Mantenimiento y Soporte Vital (Minijuegos):**
   * Protocolo de recarga de emergencia con minijuegos de trivia de *lore*, secuencias matemáticas, memorización alfanumérica o resolución de anagramas.

---

## 5. Limitaciones Reales de la Versión H3

### 5.1. Violación del Principio de Responsabilidad Única (SRP) y Monolitismo
Todo el flujo de ejecución, cálculo de combate, renderizado por consola, validaciones de entrada del teclado y persistencia de datos se encuentran acumulados en la clase `Program.cs`. Esto produce un acoplamiento extremo entre la interfaz de usuario y la lógica matemática/reglas de negocio.

### 5.2. Persistencia Frágil en Archivos JSON Planos
* **Falta de Concurrencia y Transaccionalidad:** El guardado se realiza mediante `File.WriteAllText(JsonSerializer.Serialize())`. No existe soporte para transacciones ACID ni control de concurrencia. Una falla repentina de energía o cierre inesperado durante la escritura corrompe irreversiblemente el perfil.
* **Inconsistencia Relacional:** El catálogo de objetos y la matriz del mapa se instancian *in-memory* de forma fija o aleatoria; no existe un histórico de auditoría, registro relacional de expediciones completadas ni normalización de datos.

### 5.3. Interfaz de Usuario Bloqueante y Síncrona
* La interfaz depende de métodos bloqueantes como `Console.ReadKey(true)` o `Console.ReadLine()`. Esto impide la ejecución fluida de hilos en paralelo para animaciones continuas, timers en tiempo real o eventos dinámicos de red/IA en segundo plano sin congelar la ventana.
* Dependencia obligatoria de la consola de Windows y de la API de Win32 (`kernel32.dll`), limitando la portabilidad y bloqueando cualquier intento de renderizado gráfico de alta fidelidad, animaciones suaves o controles de puntero (ratón).

### 5.4. Estado Global Mutable en Memoria
El uso de miembros estáticos (`Realidad.ListaRealidades`, `AtlasRealidades.Atlas`) introduce efectos colaterales. Si se deseara implementar partidas simultáneas, multijugador o reinicio limpio de la sesión sin terminar el proceso del sistema operativo, el estado queda contaminado en memoria.