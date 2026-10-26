# Modelo de Datos Propuesto para SQL Server

## 1. Justificación de la Migración
El sistema original guardaba la partida completa en archivos `.json` planos (`Saves/Nombre.json`). Esto presenta riesgos de corrupción, dificulta las consultas complejas y no permite extender fácilmente el sistema (por ejemplo, para tablas de clasificación o logs de auditoría).
Migrar a **SQL Server** permite normalizar los datos, garantizar la integridad referencial y preparar el terreno para funcionalidades más complejas.

## 2. Estrategia Técnica (ORM)
Se recomienda el uso de **Entity Framework Core (EF Core)** con enfoque *Code-First*. Las clases POCO (Plain Old C# Objects) de la BLL dictarán la estructura de las tablas mediante anotaciones de datos (Data Annotations) o Fluent API.

## 3. Diagrama Entidad-Relación Lógico (DER)

```text
+-------------------+       +-------------------+       +-------------------+
|     JUGADORES     | 1   1 |     PARTIDAS      | 1   N |    INVENTARIOS    |
+-------------------+-------+-------------------+-------+-------------------+
| PK_JugadorId      |       | PK_PartidaId      |       | PK_ItemId         |
| Nombre            |       | FK_JugadorId (UQ) |       | FK_JugadorId      |
| Edad              |       | VidaActual        |       | NombreObjeto      |
| Nivel             |       | EnergiaActual     |       | UsosRestantes     |
| Experiencia       |       | Fragmentos        |       +-------------------+
| PuntosProficiencia|       | Restos            |
| UpgradesInv       |       | NombreRealidad    |
| UpgradesVida      |       | NivelEstabilidad  |
| UpgradesEnergia   |       | CoordX            |
+-------------------+       | CoordY            |
                            +-------------------+
```

## 4. Diccionario de Datos (Detalle de Tablas)

### Tabla 1: `T_Jugadores`
Almacena el perfil permanente del cadete. Persiste entre incursiones (incluso si muere).
*   `PK_JugadorId` (INT, Primary Key, Identity): Identificador único del explorador.
*   `Nombre` (VARCHAR(50), Unique, Not Null): Nombre del usuario en el sistema.
*   `Edad` (INT): Dato biométrico requerido al crear el perfil.
*   `Nivel` (INT): Nivel global acumulado.
*   `Experiencia` (INT): Progreso hacia el siguiente nivel (0-100).
*   `PuntosProficiencia` (INT): Puntos disponibles para gastar en mejoras.
*   `UpgradesInv`, `UpgradesVida`, `UpgradesEnergia` (INT): Contadores para calcular el coste exponencial de la tienda.

### Tabla 2: `T_Partidas_Activas`
Reemplaza el estado volátil del JSON. Si un jugador abandona el juego o la simulación se cierra, sus stats locales de esa incursión se guardan aquí. Su relación con el Jugador es 1 a 1 temporal (si muere o extrae, esta fila se borra).
*   `PK_PartidaId` (INT, Primary Key, Identity).
*   `FK_JugadorId` (INT, Foreign Key -> T_Jugadores, Unique): Quién está jugando esta partida.
*   `VidaActual` (INT), `EnergiaActual` (INT): Estado vital en el instante de guardado.
*   `Fragmentos` (INT), `Restos` (INT): Inventario de recursos básicos.
*   `NombreRealidad` (VARCHAR(100)): Designación del sector (Ej. "Vacío Olvidado").
*   `NivelEstabilidad` (INT): De 0 a 100%.
*   `CoordX`, `CoordY` (INT): Posición física actual del avatar en el mapa para el sistema de reanudación de coordenadas.

### Tabla 3: `T_Inventarios`
Debido a que el catálogo de objetos base (los 40 objetos) existe como un diccionario/array estático en el código del juego, esta tabla solo necesita registrar *qué* objetos tiene actualmente el jugador en su mochila y cuánto le duran.
*   `PK_ItemId` (INT, Primary Key, Identity).
*   `FK_JugadorId` (INT, Foreign Key -> T_Jugadores): El dueño del objeto.
*   `NombreObjeto` (VARCHAR(50)): Referencia clave para buscar el objeto en el catálogo en código.
*   `UsosRestantes` (INT): Durabilidad actual, decrece al interactuar con anomalías.

## 5. Implementación del Guardado
Cuando el jugador decida "Guardar y Salir" o el juego guarde automáticamente:
1. El `DbContext` eliminará la partida activa actual (si existe).
2. Insertará una nueva fila en `T_Partidas_Activas` y limpiará/reinsertará las filas correspondientes en `T_Inventarios`.
3. Actualizará los campos globales (Experiencia, Nivel) en `T_Jugadores` ejecutando un `SaveChanges()`.