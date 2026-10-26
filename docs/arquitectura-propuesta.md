# Arquitectura Propuesta: Modelo N-Capas

## 1. Visión General y Justificación
El código original de NEXUS (H3) sufría de un anti-patrón conocido como *God Class* o Monolito, donde la clase `Program.cs` contenía la interfaz de consola, las reglas del juego, y la persistencia de datos.
Para la versión en **Windows Forms**, es obligatorio separar las responsabilidades para garantizar la mantenibilidad y escalabilidad del código. Se propone una arquitectura en **3 Capas (3-Tier Architecture)**.

## 2. Capas del Sistema

### 2.1. Capa de Presentación (NEXUS.GUI)
Esta capa será un proyecto del tipo *Windows Forms App*.
*   **Responsabilidad:** Capturar la entrada del usuario (clicks, teclado) y renderizar la información en pantalla.
*   **Componentes:**
    *   `FrmMain`: Menú de selección de perfiles.
    *   `FrmExploracion`: Pantalla principal del mapa táctico.
    *   `FrmCombate`: Ventana modal para enfrentamientos.
    *   `FrmInventario`: Gestor de recursos y crafteo.
*   *Regla de oro:* Ningún formulario debe calcular daños, experiencia o gestionar bases de datos. Solo delegan la acción a la capa inferior.

### 2.2. Capa de Lógica de Negocio (NEXUS.BLL)
Esta capa será un proyecto del tipo *Class Library* (Biblioteca de clases).
*   **Responsabilidad:** Contener todas las reglas del universo de NEXUS.
*   **Componentes:**
    *   **Entidades Core:** Aquí migran las clases puras del H3: `Usuario`, `Enemigo`, `Realidad`, `Objeto`, `EntidadIris`.
    *   **Controladores/Managers:** Clases encargadas de gestionar mecánicas. Por ejemplo, un `ExplorationManager` que verifique si una coordenada es válida para moverse, o un `CombatEngine` que procese las fórmulas de daño y bloqueo.

### 2.3. Capa de Acceso a Datos (NEXUS.DAL)
Esta capa será un proyecto del tipo *Class Library*.
*   **Responsabilidad:** Actuar como puente entre la Lógica de Negocio y SQL Server.
*   **Componentes:**
    *   **DbContext:** Si se utiliza Entity Framework Core, aquí reside el contexto de la base de datos (`NexusContext`), configurando los `DbSet` para mapear las tablas.
    *   **Repositorios:** Clases estandarizadas (ej. `JugadorRepository`) con métodos CRUD (`ObtenerTodos()`, `GuardarPartida()`, `EliminarPerfil()`).

## 3. Diagrama de Flujo y Dependencias

```text
[ INTERFAZ GRÁFICA ] (NEXUS.GUI)
       │
       │ (Llama a métodos y eventos)
       ▼
[ LÓGICA DE NEGOCIO ] (NEXUS.BLL)
       │
       │ (Solicita guardar/cargar datos de entidades)
       ▼
[ ACCESO A DATOS ] (NEXUS.DAL)
       │
       │ (Consultas SQL vía EF Core / ADO.NET)
       ▼
[ BASE DE DATOS SQL SERVER ]
```

## 4. Evolución del Bucle de Juego (Game Loop)
El mayor cambio arquitectónico será abandonar el bucle síncrono `while(conectado)` del H3. 
Windows Forms es **orientado a eventos (Event-Driven)**. 
* El juego estará "pausado" esperando que el usuario interactúe (ej. presionar una tecla dispara el evento `Form_KeyDown`).
* Las amenazas de fondo, como la IA `EntidadIris` escaneando la estabilidad de la realidad, se ejecutarán usando componentes `System.Windows.Forms.Timer` con intervalos (ticks) específicos, evitando así que la interfaz se congele.