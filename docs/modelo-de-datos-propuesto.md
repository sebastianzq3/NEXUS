# Modelo de Datos Propuesto para Microsoft SQL Server

## 1. Justificación de la Migración
En la versión **v1.0.0**, NEXUS resolvió la seguridad de escritura en entornos instalados migrando los archivos JSON desde la carpeta del ejecutable hacia `%LocalAppData%\NEXUS\Saves\*.json`. Aunque esto permite una distribución autónoma sin servidor, la persistencia en archivos planos presenta limitaciones estructurales a largo plazo:
- Carencia de transacciones **ACID** y riesgo de corrupción ante cierres inesperados.
- Dificultad para generar rankings globales, auditorías de rendimiento académico en C# o gestión centralizada de preguntas.

Migrar a **Microsoft SQL Server** permite normalizar los datos en **Tercera Forma Normal (3FN)**, asegurar la integridad referencial y escalar tanto el progreso del juego como el banco didáctico de C#.

---

## 2. Estrategia de Acceso a Datos
En coherencia con `analisis-tecnico.md`, se adopta **ADO.NET puro mediante `Microsoft.Data.SqlClient`** con consultas parametrizadas, procedimientos almacenados y transacciones explícitas (`SqlTransaction`) ejecutadas de forma asíncrona (`async/await`). Esto evita la sobrecarga de un ORM pesado, garantiza control total sobre las transacciones y previene inyección SQL.

---

## 3. Diagrama Entidad-Relación Lógico (DER)

```text
+-------------------------------+       +-----------------------------+
|          T_JUGADORES          | 1   1 |      T_PARTIDAS_ACTIVAS     |
+-------------------------------+-------+-----------------------------+
| PK_JugadorId (INT, IDENTITY)  |       | PK_PartidaId (INT, IDENTITY)|
| Nombre (NVARCHAR(50), UQ)     |       | FK_JugadorId (INT, FK, UQ)  |
| Edad (INT, CHECK >= 18)       |       | VidaActual (INT)            |
| Nivel (INT)                   |       | EnergiaActual (INT)         |
| Experiencia (INT)             |       | Fragmentos (INT)            |
| PuntosProficiencia (INT)      |       | Restos (INT)                |
| UpgradesInv (INT)             |       | NombreRealidad (NVARCHAR)   |
| UpgradesVida (INT)            |       | TipoAnomalia (TINYINT)      |
| UpgradesEnergia (INT)         |       | NivelEstabilidad (INT)      |
| PreguntasCSharpRespondidas    |       | CoordX / CoordY (INT)       |
| PreguntasCSharpAcertadas      |       | Extraida (BIT)              |
+---------------+---------------+       +-----------------------------+
                | 1
                |
       +--------+--------------------+
       | N                           | N
       v                             v
+-----------------------------+  +------------------------------------+
|        T_INVENTARIOS        |  |          T_OBSERVACIONES           |
+-----------------------------+  +------------------------------------+
| PK_ItemId (INT, IDENTITY)   |  | PK_ObservacionId (INT, IDENTITY)   |
| FK_JugadorId (INT, FK)      |  | FK_JugadorId (INT, FK)             |
| NombreObjeto (NVARCHAR(80)) |  | TextoPista (NVARCHAR(250))         |
| TipoAnomalia (TINYINT)      |  | FechaRegistro (DATETIME2)          |
| UsosRestantes (INT)         |  +------------------------------------+
+-----------------------------+

+---------------------------------------------------------------------+
|                     T_PREGUNTAS_CSHARP (Catálogo)                   |
+---------------------------------------------------------------------+
| PK_PreguntaId (INT, IDENTITY)                                       |
| Categoria (NVARCHAR(60)) | Dificultad (NVARCHAR(20))                |
| Enunciado (NVARCHAR(300)) | CodigoEjemplo (NVARCHAR(400), NULL)     |
| OpcionA / OpcionB / OpcionC / OpcionD (NVARCHAR(200))               |
| IndiceCorrecto (TINYINT, CHECK 0..3)                                |
| Explicacion (NVARCHAR(400))                                         |
+---------------------------------------------------------------------+
```

---

## 4. Diccionario de Datos (Detalle de Tablas)

### 4.1. `T_Jugadores`
Almacena el perfil persistente del cadete, su progresión global y su desempeño en el módulo didáctico de C#.
- `PK_JugadorId` (`INT IDENTITY(1,1) PRIMARY KEY`): Identificador único del explorador.
- `Nombre` (`NVARCHAR(50) UNIQUE NOT NULL`): Designación del cadete.
- `Edad` (`INT NOT NULL CHECK (Edad >= 18)`): Dato biométrico validado al registro.
- `Nivel` (`INT NOT NULL DEFAULT 1`), `Experiencia` (`INT NOT NULL DEFAULT 0`): Progresión acumulada.
- `PuntosProficiencia` (`INT NOT NULL DEFAULT 0`): Moneda de mejoras.
- `UpgradesInv`, `UpgradesVida`, `UpgradesEnergia` (`INT NOT NULL DEFAULT 0`): Contadores de compras para calcular la curva de costos `[1, 1, 2, 3, 5]`.
- `PreguntasCSharpRespondidas` (`INT NOT NULL DEFAULT 0`): Total de filtros de seguridad C# afrontados.
- `PreguntasCSharpAcertadas` (`INT NOT NULL DEFAULT 0`): Total de respuestas correctas (permite calcular precisión didáctica histórica).

### 4.2. `T_Partidas_Activas`
Mapea el estado actual de la realidad asignada (`Realidad` + recursos volátiles de `Usuario`). Relación `1:1` mediante restricción `UNIQUE` en `FK_JugadorId`.
- `PK_PartidaId` (`INT IDENTITY(1,1) PRIMARY KEY`).
- `FK_JugadorId` (`INT NOT NULL UNIQUE FOREIGN KEY REFERENCES T_Jugadores(PK_JugadorId) ON DELETE CASCADE`).
- `VidaActual` (`INT`), `EnergiaActual` (`INT`), `Fragmentos` (`INT`), `Restos` (`INT`).
- `NombreRealidad` (`NVARCHAR(100) NOT NULL`), `TipoAnomalia` (`TINYINT NOT NULL CHECK (TipoAnomalia BETWEEN 0 AND 3)`).
- `NivelEstabilidad` (`INT NOT NULL CHECK (NivelEstabilidad BETWEEN 0 AND 100)`).
- `CoordX` (`INT`), `CoordY` (`INT`), `Extraida` (`BIT NOT NULL DEFAULT 0`).

### 4.3. `T_Inventarios`
Registra los objetos activos en la mochila del jugador (referenciando los 40 ítems del catálogo de `Objeto`) y su durabilidad restante.
- `PK_ItemId` (`INT IDENTITY(1,1) PRIMARY KEY`).
- `FK_JugadorId` (`INT NOT NULL FOREIGN KEY REFERENCES T_Jugadores(PK_JugadorId) ON DELETE CASCADE`).
- `NombreObjeto` (`NVARCHAR(80) NOT NULL`), `TipoAnomalia` (`TINYINT NOT NULL`), `UsosRestantes` (`INT NOT NULL CHECK (UsosRestantes > 0)`).

### 4.4. `T_Observaciones`
Almacena las pistas sensoriales decodificadas con Fragmentos para deducir la anomalía del sector actual (se depura al extraer la realidad).
- `PK_ObservacionId` (`INT IDENTITY(1,1) PRIMARY KEY`).
- `FK_JugadorId` (`INT NOT NULL FOREIGN KEY REFERENCES T_Jugadores(PK_JugadorId) ON DELETE CASCADE`).
- `TextoPista` (`NVARCHAR(250) NOT NULL`), `FechaRegistro` (`DATETIME2 NOT NULL DEFAULT SYSDATETIME()`).

### 4.5. `T_Preguntas_CSharp`
Permite migrar las 28 preguntas actuales de `BancoPreguntasCSharp.cs` a base de datos para agregar o editar reactivos sin recompilar `NEXUS.exe`.
- `PK_PreguntaId` (`INT IDENTITY(1,1) PRIMARY KEY`).
- `Categoria` (`NVARCHAR(60)`), `Dificultad` (`NVARCHAR(20)`), `Enunciado` (`NVARCHAR(300)`), `CodigoEjemplo` (`NVARCHAR(400) NULL`).
- `OpcionA`, `OpcionB`, `OpcionC`, `OpcionD` (`NVARCHAR(200) NOT NULL`), `IndiceCorrecto` (`TINYINT CHECK (IndiceCorrecto BETWEEN 0 AND 3)`), `Explicacion` (`NVARCHAR(400) NOT NULL`).

---

## 5. Estrategia Transaccional de Guardado y Recuperación
Al ejecutar `GuardarJuegoAsync(DatosPartida datos)` en `NEXUS.DAL`:
1. Se abre una conexión `SqlConnection` y se inicia una transacción explícita (`SqlTransaction`).
2. Se ejecuta un `UPDATE` parametrizado en `T_Jugadores` (nivel, experiencia, mejoras y contadores `PreguntasCSharp*`).
3. Se ejecuta un `MERGE` (o `UPDATE`/`INSERT`) sobre `T_Partidas_Activas` para el `FK_JugadorId`.
4. Se sincronizan `T_Inventarios` y `T_Observaciones` eliminando el estado anterior del jugador e insertando los elementos vigentes dentro de la misma transacción.
5. Si todas las operaciones tienen éxito se ejecuta `transaction.CommitAsync()`; ante cualquier excepción se invoca `transaction.RollbackAsync()`, garantizando consistencia total.