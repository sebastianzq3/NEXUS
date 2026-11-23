-- ============================================================================
-- PROYECTO: NEXUS - SISTEMA DE SIMULACIÓN Y GESTIÓN MULTIVERSAL
-- ASIGNATURA: PROGRAMACIÓN II - UNIFRANZ COCHABAMBA
-- AUTOR: SEBASTIÁN ZUBIETA QUIROGA
-- SCRIPT 05: SCRIPT COMPLETO INTEGRADO (DDL + DML + PROCEDIMIENTOS + SEMILLAS)
-- Permite recrear toda la base de datos NexusDB en una sola ejecución.
-- ============================================================================

PRINT '========================================================================';
PRINT '  INICIANDO DESPLIEGUE INTEGRAL DE BASE DE DATOS NEXUS (HITO 4)';
PRINT '========================================================================';

-- 1. BASE DE DATOS
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'NexusDB')
BEGIN
    CREATE DATABASE [NexusDB] COLLATE Latin1_General_CI_AS;
    PRINT '[NEXUS-SQL]: Base de datos NexusDB creada.';
END
GO

USE [NexusDB];
GO

-- 2. TABLAS Y RELACIONES
IF OBJECT_ID('dbo.T_Historial_Incursiones', 'U') IS NOT NULL DROP TABLE dbo.T_Historial_Incursiones;
IF OBJECT_ID('dbo.T_Observaciones', 'U') IS NOT NULL DROP TABLE dbo.T_Observaciones;
IF OBJECT_ID('dbo.T_Inventarios', 'U') IS NOT NULL DROP TABLE dbo.T_Inventarios;
IF OBJECT_ID('dbo.T_Partidas_Activas', 'U') IS NOT NULL DROP TABLE dbo.T_Partidas_Activas;
IF OBJECT_ID('dbo.T_Jugadores', 'U') IS NOT NULL DROP TABLE dbo.T_Jugadores;
IF OBJECT_ID('dbo.T_Preguntas_CSharp', 'U') IS NOT NULL DROP TABLE dbo.T_Preguntas_CSharp;
GO

CREATE TABLE dbo.T_Jugadores
(
    PK_JugadorId                INT IDENTITY(1,1) NOT NULL,
    Nombre                      NVARCHAR(50)      NOT NULL,
    Edad                        INT               NOT NULL,
    Nivel                       INT               NOT NULL CONSTRAINT DF_Jugadores_Nivel DEFAULT 1,
    Experiencia                 INT               NOT NULL CONSTRAINT DF_Jugadores_Exp DEFAULT 0,
    VidaMax                     INT               NOT NULL CONSTRAINT DF_Jugadores_VidaMax DEFAULT 100,
    EnergiaMax                  INT               NOT NULL CONSTRAINT DF_Jugadores_EnergiaMax DEFAULT 100,
    DanoBase                    INT               NOT NULL CONSTRAINT DF_Jugadores_Dano DEFAULT 15,
    CapacidadInventario         INT               NOT NULL CONSTRAINT DF_Jugadores_CapInv DEFAULT 4,
    PuntosProficiencia          INT               NOT NULL CONSTRAINT DF_Jugadores_PuntosProf DEFAULT 0,
    UpgradesInv                 INT               NOT NULL CONSTRAINT DF_Jugadores_UpgradesInv DEFAULT 0,
    UpgradesVida                INT               NOT NULL CONSTRAINT DF_Jugadores_UpgradesVida DEFAULT 0,
    UpgradesEnergia             INT               NOT NULL CONSTRAINT DF_Jugadores_UpgradesEnergia DEFAULT 0,
    PreguntasCSharpRespondidas  INT               NOT NULL CONSTRAINT DF_Jugadores_PResp DEFAULT 0,
    PreguntasCSharpAcertadas    INT               NOT NULL CONSTRAINT DF_Jugadores_PAcert DEFAULT 0,
    FechaRegistro               DATETIME2(3)      NOT NULL CONSTRAINT DF_Jugadores_FechaReg DEFAULT SYSDATETIME(),
    UltimaSincronizacion        DATETIME2(3)      NOT NULL CONSTRAINT DF_Jugadores_UltSinc DEFAULT SYSDATETIME(),

    CONSTRAINT PK_T_Jugadores PRIMARY KEY CLUSTERED (PK_JugadorId),
    CONSTRAINT UQ_T_Jugadores_Nombre UNIQUE (Nombre),
    CONSTRAINT CHK_Jugadores_Edad CHECK (Edad >= 18),
    CONSTRAINT CHK_Jugadores_Nivel CHECK (Nivel >= 1),
    CONSTRAINT CHK_Jugadores_Exp CHECK (Experiencia >= 0),
    CONSTRAINT CHK_Jugadores_VidaMax CHECK (VidaMax > 0),
    CONSTRAINT CHK_Jugadores_EnergiaMax CHECK (EnergiaMax > 0),
    CONSTRAINT CHK_Jugadores_CapInv CHECK (CapacidadInventario >= 1),
    CONSTRAINT CHK_Jugadores_StatsCSharp CHECK (PreguntasCSharpAcertadas <= PreguntasCSharpRespondidas)
);
GO

CREATE TABLE dbo.T_Partidas_Activas
(
    PK_PartidaId        INT IDENTITY(1,1) NOT NULL,
    FK_JugadorId        INT               NOT NULL,
    VidaActual          INT               NOT NULL,
    EnergiaActual       INT               NOT NULL,
    Fragmentos          INT               NOT NULL CONSTRAINT DF_Partidas_Fragmentos DEFAULT 0,
    Restos              INT               NOT NULL CONSTRAINT DF_Partidas_Restos DEFAULT 0,
    NombreRealidad      NVARCHAR(100)     NOT NULL,
    TipoAnomalia        TINYINT           NOT NULL,
    NivelEstabilidad    INT               NOT NULL,
    CoordX              INT               NOT NULL CONSTRAINT DF_Partidas_CoordX DEFAULT 0,
    CoordY              INT               NOT NULL CONSTRAINT DF_Partidas_CoordY DEFAULT 0,
    Extraida            BIT               NOT NULL CONSTRAINT DF_Partidas_Extraida DEFAULT 0,
    Cartografiada       BIT               NOT NULL CONSTRAINT DF_Partidas_Cartografiada DEFAULT 0,
    FechaGuardado       DATETIME2(3)      NOT NULL CONSTRAINT DF_Partidas_FechaGuardado DEFAULT SYSDATETIME(),

    CONSTRAINT PK_T_Partidas_Activas PRIMARY KEY CLUSTERED (PK_PartidaId),
    CONSTRAINT UQ_T_Partidas_FK_JugadorId UNIQUE (FK_JugadorId),
    CONSTRAINT FK_Partidas_T_Jugadores FOREIGN KEY (FK_JugadorId)
        REFERENCES dbo.T_Jugadores(PK_JugadorId) ON DELETE CASCADE,
    CONSTRAINT CHK_Partidas_Vida CHECK (VidaActual >= 0),
    CONSTRAINT CHK_Partidas_Energia CHECK (EnergiaActual >= 0),
    CONSTRAINT CHK_Partidas_Fragmentos CHECK (Fragmentos >= 0),
    CONSTRAINT CHK_Partidas_Restos CHECK (Restos >= 0),
    CONSTRAINT CHK_Partidas_TipoAnomalia CHECK (TipoAnomalia BETWEEN 0 AND 3),
    CONSTRAINT CHK_Partidas_Estabilidad CHECK (NivelEstabilidad BETWEEN 0 AND 100),
    CONSTRAINT CHK_Partidas_CoordX CHECK (CoordX BETWEEN -1 AND 45),
    CONSTRAINT CHK_Partidas_CoordY CHECK (CoordY BETWEEN -1 AND 25)
);
GO

CREATE TABLE dbo.T_Inventarios
(
    PK_ItemId           INT IDENTITY(1,1) NOT NULL,
    FK_JugadorId        INT               NOT NULL,
    NombreObjeto        NVARCHAR(100)     NOT NULL,
    TipoAnomalia        TINYINT           NOT NULL,
    Descripcion         NVARCHAR(300)     NULL,
    UsosRestantes       INT               NOT NULL,
    FechaObtencion      DATETIME2(3)      NOT NULL CONSTRAINT DF_Inventarios_FechaObt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_T_Inventarios PRIMARY KEY CLUSTERED (PK_ItemId),
    CONSTRAINT FK_Inventarios_T_Jugadores FOREIGN KEY (FK_JugadorId)
        REFERENCES dbo.T_Jugadores(PK_JugadorId) ON DELETE CASCADE,
    CONSTRAINT CHK_Inventarios_TipoAnomalia CHECK (TipoAnomalia BETWEEN 0 AND 3),
    CONSTRAINT CHK_Inventarios_Usos CHECK (UsosRestantes > 0)
);
GO

CREATE TABLE dbo.T_Observaciones
(
    PK_ObservacionId    INT IDENTITY(1,1) NOT NULL,
    FK_JugadorId        INT               NOT NULL,
    TextoPista          NVARCHAR(300)     NOT NULL,
    FechaRegistro       DATETIME2(3)      NOT NULL CONSTRAINT DF_Observaciones_FechaReg DEFAULT SYSDATETIME(),

    CONSTRAINT PK_T_Observaciones PRIMARY KEY CLUSTERED (PK_ObservacionId),
    CONSTRAINT FK_Observaciones_T_Jugadores FOREIGN KEY (FK_JugadorId)
        REFERENCES dbo.T_Jugadores(PK_JugadorId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.T_Preguntas_CSharp
(
    PK_PreguntaId       INT IDENTITY(1,1) NOT NULL,
    Categoria           NVARCHAR(80)      NOT NULL,
    Dificultad          NVARCHAR(20)      NOT NULL CONSTRAINT DF_Preguntas_Dif DEFAULT 'Intermedio',
    Enunciado           NVARCHAR(400)     NOT NULL,
    CodigoEjemplo       NVARCHAR(1000)    NULL,
    OpcionA             NVARCHAR(250)     NOT NULL,
    OpcionB             NVARCHAR(250)     NOT NULL,
    OpcionC             NVARCHAR(250)     NOT NULL,
    OpcionD             NVARCHAR(250)     NOT NULL,
    IndiceCorrecto      TINYINT           NOT NULL,
    Explicacion         NVARCHAR(600)     NOT NULL,
    Activa              BIT               NOT NULL CONSTRAINT DF_Preguntas_Activa DEFAULT 1,
    FechaCreacion       DATETIME2(3)      NOT NULL CONSTRAINT DF_Preguntas_FechaCreacion DEFAULT SYSDATETIME(),

    CONSTRAINT PK_T_Preguntas_CSharp PRIMARY KEY CLUSTERED (PK_PreguntaId),
    CONSTRAINT CHK_Preguntas_IndiceCorrecto CHECK (IndiceCorrecto BETWEEN 0 AND 3),
    CONSTRAINT CHK_Preguntas_Dificultad CHECK (Dificultad IN ('Básico', 'Intermedio', 'Avanzado'))
);
GO

CREATE TABLE dbo.T_Historial_Incursiones
(
    PK_IncursionId      INT IDENTITY(1,1) NOT NULL,
    FK_JugadorId        INT               NOT NULL,
    NombreRealidad      NVARCHAR(100)     NOT NULL,
    TipoAnomalia        TINYINT           NOT NULL,
    Resultado           NVARCHAR(30)      NOT NULL,
    EstabilidadFinal    INT               NOT NULL,
    FragmentosObtenidos INT               NOT NULL CONSTRAINT DF_Historial_Fragmentos DEFAULT 0,
    FechaFin            DATETIME2(3)      NOT NULL CONSTRAINT DF_Historial_FechaFin DEFAULT SYSDATETIME(),

    CONSTRAINT PK_T_Historial_Incursiones PRIMARY KEY CLUSTERED (PK_IncursionId),
    CONSTRAINT FK_Historial_T_Jugadores FOREIGN KEY (FK_JugadorId)
        REFERENCES dbo.T_Jugadores(PK_JugadorId) ON DELETE CASCADE,
    CONSTRAINT CHK_Historial_Resultado CHECK (Resultado IN ('EXTRAIDA', 'EVACUADA', 'COLAPSO', 'EN_CURSO')),
    CONSTRAINT CHK_Historial_Estabilidad CHECK (EstabilidadFinal BETWEEN 0 AND 100)
);
GO

CREATE NONCLUSTERED INDEX IX_Partidas_FK_JugadorId ON dbo.T_Partidas_Activas(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Inventarios_FK_JugadorId ON dbo.T_Inventarios(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Observaciones_FK_JugadorId ON dbo.T_Observaciones(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Historial_FK_JugadorId ON dbo.T_Historial_Incursiones(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Preguntas_Categoria ON dbo.T_Preguntas_CSharp(Categoria, Activa);
GO

-- 3. PROCEDIMIENTOS ALMACENADOS
IF OBJECT_ID('dbo.sp_ListarPerfiles', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ListarPerfiles;
GO
CREATE PROCEDURE dbo.sp_ListarPerfiles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        j.PK_JugadorId,
        j.Nombre,
        j.Edad,
        j.Nivel,
        j.Experiencia,
        ISNULL(p.VidaActual, j.VidaMax) AS VidaActual,
        j.VidaMax,
        ISNULL(p.EnergiaActual, j.EnergiaMax) AS EnergiaActual,
        j.EnergiaMax,
        ISNULL(p.NombreRealidad, N'Sector Desconocido') AS NombreRealidad,
        ISNULL(p.NivelEstabilidad, 50) AS NivelEstabilidad,
        j.PuntosProficiencia,
        j.PreguntasCSharpRespondidas,
        j.PreguntasCSharpAcertadas,
        j.UltimaSincronizacion
    FROM dbo.T_Jugadores j
    LEFT JOIN dbo.T_Partidas_Activas p ON j.PK_JugadorId = p.FK_JugadorId
    ORDER BY j.UltimaSincronizacion DESC;
END;
GO

IF OBJECT_ID('dbo.sp_GuardarPartidaCabecera', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GuardarPartidaCabecera;
GO
CREATE PROCEDURE dbo.sp_GuardarPartidaCabecera
    @Nombre                 NVARCHAR(50),
    @Edad                   INT,
    @Nivel                  INT,
    @Experiencia            INT,
    @VidaMax                INT,
    @EnergiaMax             INT,
    @DanoBase               INT,
    @CapacidadInventario    INT,
    @PuntosProficiencia     INT,
    @UpgradesInv            INT,
    @UpgradesVida           INT,
    @UpgradesEnergia        INT,
    @PreguntasRespondidas   INT,
    @PreguntasAcertadas     INT,
    @VidaActual             INT,
    @EnergiaActual          INT,
    @Fragmentos             INT,
    @Restos                 INT,
    @NombreRealidad         NVARCHAR(100),
    @TipoAnomalia           TINYINT,
    @NivelEstabilidad       INT,
    @CoordX                 INT,
    @CoordY                 INT,
    @Extraida               BIT,
    @Cartografiada          BIT,
    @JugadorIdOutput        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @JugadorId INT;
        SELECT @JugadorId = PK_JugadorId FROM dbo.T_Jugadores WHERE Nombre = @Nombre;

        IF @JugadorId IS NULL
        BEGIN
            INSERT INTO dbo.T_Jugadores (
                Nombre, Edad, Nivel, Experiencia, VidaMax, EnergiaMax,
                DanoBase, CapacidadInventario, PuntosProficiencia,
                UpgradesInv, UpgradesVida, UpgradesEnergia,
                PreguntasCSharpRespondidas, PreguntasCSharpAcertadas,
                FechaRegistro, UltimaSincronizacion
            )
            VALUES (
                @Nombre, @Edad, @Nivel, @Experiencia, @VidaMax, @EnergiaMax,
                @DanoBase, @CapacidadInventario, @PuntosProficiencia,
                @UpgradesInv, @UpgradesVida, @UpgradesEnergia,
                @PreguntasRespondidas, @PreguntasAcertadas,
                SYSDATETIME(), SYSDATETIME()
            );
            SET @JugadorId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE dbo.T_Jugadores
            SET Edad = @Edad,
                Nivel = @Nivel,
                Experiencia = @Experiencia,
                VidaMax = @VidaMax,
                EnergiaMax = @EnergiaMax,
                DanoBase = @DanoBase,
                CapacidadInventario = @CapacidadInventario,
                PuntosProficiencia = @PuntosProficiencia,
                UpgradesInv = @UpgradesInv,
                UpgradesVida = @UpgradesVida,
                UpgradesEnergia = @UpgradesEnergia,
                PreguntasCSharpRespondidas = @PreguntasRespondidas,
                PreguntasCSharpAcertadas = @PreguntasAcertadas,
                UltimaSincronizacion = SYSDATETIME()
            WHERE PK_JugadorId = @JugadorId;
        END

        IF EXISTS (SELECT 1 FROM dbo.T_Partidas_Activas WHERE FK_JugadorId = @JugadorId)
        BEGIN
            UPDATE dbo.T_Partidas_Activas
            SET VidaActual = @VidaActual,
                EnergiaActual = @EnergiaActual,
                Fragmentos = @Fragmentos,
                Restos = @Restos,
                NombreRealidad = @NombreRealidad,
                TipoAnomalia = @TipoAnomalia,
                NivelEstabilidad = @NivelEstabilidad,
                CoordX = @CoordX,
                CoordY = @CoordY,
                Extraida = @Extraida,
                Cartografiada = @Cartografiada,
                FechaGuardado = SYSDATETIME()
            WHERE FK_JugadorId = @JugadorId;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.T_Partidas_Activas (
                FK_JugadorId, VidaActual, EnergiaActual, Fragmentos, Restos,
                NombreRealidad, TipoAnomalia, NivelEstabilidad,
                CoordX, CoordY, Extraida, Cartografiada, FechaGuardado
            )
            VALUES (
                @JugadorId, @VidaActual, @EnergiaActual, @Fragmentos, @Restos,
                @NombreRealidad, @TipoAnomalia, @NivelEstabilidad,
                @CoordX, @CoordY, @Extraida, @Cartografiada, SYSDATETIME()
            );
        END

        SET @JugadorIdOutput = @JugadorId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('dbo.sp_CargarPartidaPorNombre', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CargarPartidaPorNombre;
GO
CREATE PROCEDURE dbo.sp_CargarPartidaPorNombre
    @Nombre NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        j.PK_JugadorId, j.Nombre, j.Edad, j.Nivel, j.Experiencia,
        j.VidaMax, j.EnergiaMax, j.DanoBase, j.CapacidadInventario,
        j.PuntosProficiencia, j.UpgradesInv, j.UpgradesVida, j.UpgradesEnergia,
        j.PreguntasCSharpRespondidas, j.PreguntasCSharpAcertadas,
        p.VidaActual, p.EnergiaActual, p.Fragmentos, p.Restos,
        p.NombreRealidad, p.TipoAnomalia, p.NivelEstabilidad,
        p.CoordX, p.CoordY, p.Extraida, p.Cartografiada
    FROM dbo.T_Jugadores j
    LEFT JOIN dbo.T_Partidas_Activas p ON j.PK_JugadorId = p.FK_JugadorId
    WHERE j.Nombre = @Nombre;

    SELECT i.NombreObjeto, i.TipoAnomalia, i.Descripcion, i.UsosRestantes
    FROM dbo.T_Inventarios i
    INNER JOIN dbo.T_Jugadores j ON i.FK_JugadorId = j.PK_JugadorId
    WHERE j.Nombre = @Nombre;

    SELECT o.TextoPista, o.FechaRegistro
    FROM dbo.T_Observaciones o
    INNER JOIN dbo.T_Jugadores j ON o.FK_JugadorId = j.PK_JugadorId
    WHERE j.Nombre = @Nombre
    ORDER BY o.FechaRegistro ASC;
END;
GO

IF OBJECT_ID('dbo.sp_EliminarPerfil', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_EliminarPerfil;
GO
CREATE PROCEDURE dbo.sp_EliminarPerfil
    @Nombre NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.T_Jugadores WHERE Nombre = @Nombre;
    SELECT @@ROWCOUNT AS FilasEliminadas;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerPreguntasCSharp', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ObtenerPreguntasCSharp;
GO
CREATE PROCEDURE dbo.sp_ObtenerPreguntasCSharp
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        PK_PreguntaId,
        Categoria,
        Dificultad,
        Enunciado,
        CodigoEjemplo,
        OpcionA,
        OpcionB,
        OpcionC,
        OpcionD,
        IndiceCorrecto,
        Explicacion
    FROM dbo.T_Preguntas_CSharp
    WHERE Activa = 1
    ORDER BY PK_PreguntaId ASC;
END;
GO

-- 4. POBLADO DE 28 PREGUNTAS DIDÁCTICAS DE C#
SET IDENTITY_INSERT dbo.T_Preguntas_CSharp ON;
GO

INSERT INTO dbo.T_Preguntas_CSharp 
(PK_PreguntaId, Categoria, Dificultad, Enunciado, CodigoEjemplo, OpcionA, OpcionB, OpcionC, OpcionD, IndiceCorrecto, Explicacion, Activa)
VALUES
(1, N'Sintaxis y Tipos de Datos', N'Básico',
 N'¿Qué tipo de dato infiere el compilador para la variable ''energia'' al usar ''var''?',
 N'var energia = 10;',
 N'object (tipado dinámico)', N'int (tipado estático en compilación)', N'dynamic (resuelto en ejecución)', N'double por defecto',
 1, N'''var'' realiza inferencia de tipos en tiempo de compilación: ''energia'' queda fuertemente tipada como int y no puede cambiar de tipo después.', 1),

(2, N'Sintaxis y Tipos de Datos', N'Básico',
 N'¿Qué valor queda almacenado en ''resultado'' tras ejecutar este fragmento?',
 N'int a = 7;
int b = 2;
double resultado = a / b;',
 N'3.0', N'3.5', N'4.0', N'Error de compilación',
 0, N'Como ''a'' y ''b'' son int, C# ejecuta primero una división entera (7 / 2 = 3) y recién después convierte ese 3 a double (3.0). Para obtener 3.5 habría que hacer (double)a / b.', 1),

(3, N'Sintaxis y Tipos de Datos', N'Básico',
 N'¿Qué hace el operador de coalescencia nula ''??'' en C#?',
 N'string alias = nombreIngresado ?? "Cadete";',
 N'Compara si dos cadenas son iguales ignorando mayúsculas', N'Lanza una excepción si la variable es nula', N'Devuelve el operando derecho cuando el izquierdo es null', N'Convierte cualquier objeto a string',
 2, N'El operador ''??'' evalúa el operando izquierdo; si no es null lo devuelve, y si es null retorna el valor alternativo de la derecha.', 1),

(4, N'Sintaxis y Tipos de Datos', N'Básico',
 N'¿Cuál es la forma correcta de usar interpolación de cadenas en C#?',
 N'int nivel = 5;',
 N'"Nivel: {nivel}"', N'$"Nivel: {nivel}"', N'@\"Nivel: {nivel}\"', N'#"Nivel: {nivel}"',
 1, N'El prefijo ''$'' delante de las comillas habilita la interpolación de cadenas, permitiendo evaluar expresiones dentro de llaves {}.', 1),

(5, N'Sintaxis y Tipos de Datos', N'Intermedio',
 N'¿Cuál es la diferencia clave entre ''const'' y ''readonly'' en C#?',
 N'const int MAX = 100;
readonly string id = Guid.NewGuid().ToString();',
 N'No existe diferencia, son sinónimos', N'''const'' solo funciona con strings y ''readonly'' con enteros', N'''readonly'' puede modificarse en cualquier método de la clase', N'''const'' se define en compilación; ''readonly'' puede asignarse en el constructor',
 3, N'Un campo ''const'' debe conocerse en tiempo de compilación, mientras que ''readonly'' puede recibir un valor en tiempo de ejecución dentro del constructor.', 1),

(6, N'Sintaxis y Tipos de Datos', N'Básico',
 N'¿Qué ocurre al ejecutar ''int.TryParse'' con una cadena no numérica?',
 N'bool ok = int.TryParse("NEXUS", out int valor);',
 N'ok es false y valor queda en 0, sin lanzar excepción', N'Se lanza FormatException inmediatamente', N'ok es true y valor toma el código ASCII', N'El programa se detiene por desbordamiento',
 0, N'TryParse intenta convertir de forma segura: si falla, devuelve false y asigna el valor por defecto del tipo (0 para int) sin lanzar excepciones.', 1),

(7, N'Sintaxis y Tipos de Datos', N'Intermedio',
 N'¿Qué tipo numérico de C# es el recomendado para cálculos financieros o monetarios exactos?',
 NULL,
 N'float', N'double', N'decimal', N'long',
 2, N'''decimal'' usa representación en base 10 de 128 bits, evitando los pequeños errores de redondeo binario propios de float y double.', 1),

(8, N'Programación Orientada a Objetos', N'Básico',
 N'¿Cómo funciona la herencia de clases e interfaces en C#?',
 N'class Usuario : Entidad, IGuardable, ICombate { }',
 N'Una clase puede heredar de múltiples clases base simultáneamente', N'Herencia simple de clase base, pero múltiple implementación de interfaces', N'Una clase solo puede implementar una interfaz a la vez', N'Las interfaces no pueden ser implementadas junto a una clase base',
 1, N'C# restringe la herencia a una sola clase base para evitar ambigüedades (problema del diamante), pero permite implementar tantas interfaces como se necesiten.', 1),

(9, N'Programación Orientada a Objetos', N'Intermedio',
 N'¿Para qué se combinan las palabras clave ''virtual'' y ''override''?',
 N'public virtual void Atacar() { }
public override void Atacar() { }',
 N'Para permitir que una clase derivada redefina un método (polimorfismo)', N'Para ocultar una clase del ensamblado', N'Para impedir que una clase sea heredada', N'Para ejecutar métodos en paralelo en otro hilo',
 0, N'Marcar un método como ''virtual'' en la clase base autoriza a las subclases a proporcionar su propia implementación mediante ''override'', habilitando el polimorfismo.', 1),

(10, N'Programación Orientada a Objetos', N'Básico',
 N'¿Qué efecto tiene declarar una propiedad con ''public string Nombre { get; private set; }''?',
 N'public string Nombre { get; private set; }',
 N'Nadie puede leer ni escribir la propiedad', N'Cualquier clase externa puede modificar Nombre libremente', N'Cualquiera puede leer Nombre, pero solo la propia clase puede modificarlo', N'La propiedad se vuelve estática automáticamente',
 2, N'Es un patrón estándar de encapsulamiento: expone el valor para lectura pública mientras protege su modificación interna dentro de la clase.', 1),

(11, N'Programación Orientada a Objetos', N'Básico',
 N'¿Qué nivel de acceso otorga el modificador ''protected'' a un miembro de clase?',
 N'protected int energiaBase = 10;',
 N'Solo visible dentro de la misma clase exacta', N'Visible en la misma clase y en sus clases derivadas', N'Visible para cualquier proyecto externo', N'Solo visible dentro de métodos static',
 1, N'''protected'' permite que las subclases accedan a atributos o métodos heredados sin exponerlos públicamente al resto del programa.', 1),

(12, N'Programación Orientada a Objetos', N'Básico',
 N'¿Qué característica define a un método o propiedad declarada como ''static''?',
 N'AtlasRealidades.GenerarAtlas();',
 N'Solo puede llamarse una vez durante la ejecución', N'Requiere crear una instancia con ''new'' antes de usarse', N'Se guarda automáticamente en disco duro', N'Pertenece al tipo/clase en sí y no a una instancia específica',
 3, N'Los miembros ''static'' se invocan directamente sobre el nombre de la clase (ej. AtlasRealidades.GenerarAtlas()) y se comparten en toda la aplicación.', 1),

(13, N'Programación Orientada a Objetos', N'Intermedio',
 N'¿Cuál afirmación es correcta respecto a una ''abstract class'' en C#?',
 N'public abstract class EntidadBase { public abstract void Actuar(); }',
 N'No puede instanciarse con ''new'' y puede combinar métodos abstractos y concretos', N'No puede contener campos ni constructores', N'Todos sus métodos deben ser privados', N'Es idéntica a una clase ''sealed''',
 0, N'Una clase abstracta actúa como plantilla base: no se instancia directamente, pero sí puede tener estado, constructores y lógica compartida.', 1),

(14, N'Programación Orientada a Objetos', N'Intermedio',
 N'¿Por qué suele añadirse un constructor vacío ''public Usuario() { }'' al usar System.Text.Json?',
 N'public Usuario() { Inventario = new List<Objeto>(); }',
 N'Porque C# prohíbe tener constructores con parámetros', N'Para que el programa compile más rápido', N'Para que el deserializador JSON pueda instanciar el objeto y llenar sus propiedades', N'Para eliminar la necesidad de usar propiedades públicas',
 2, N'Por defecto, JsonSerializer.Deserialize busca un constructor público sin parámetros para crear el objeto y luego asignar cada propiedad desde el JSON.', 1),

(15, N'Colecciones y LINQ', N'Básico',
 N'En ''Queue<EntidadIris>'', ¿qué orden sigue la colección y qué hace el método ''Peek()''?',
 N'Queue<EntidadIris> cola = new Queue<EntidadIris>();
var actual = cola.Peek();',
 N'LIFO; Peek() elimina el último elemento', N'FIFO; Peek() devuelve el primer elemento sin quitarlo de la cola', N'Orden aleatorio; Peek() vacía la cola', N'FIFO; Peek() extrae y borra el primer elemento',
 1, N'Queue<T> es FIFO (First In, First Out). Peek() inspecciona el elemento al frente sin retirarlo, a diferencia de Dequeue() que sí lo extrae.', 1),

(16, N'Colecciones y LINQ', N'Básico',
 N'¿Cuál es la principal ventaja de ''Dictionary<TKey, TValue>'' frente a buscar en una ''List<T>''?',
 N'Dictionary<string, int> puntajes = new Dictionary<string, int>();',
 N'Acceso promedio O(1) mediante clave única hash', N'Permite almacenar claves duplicadas sin restricción', N'Consume siempre menos memoria que un arreglo simple', N'Ordena automáticamente las claves alfabéticamente',
 0, N'Dictionary<TKey, TValue> usa una tabla hash interna, permitiendo búsquedas por clave en tiempo promedio constante O(1) en vez de recorrer toda la lista O(n).', 1),

(17, N'Colecciones y LINQ', N'Básico',
 N'¿Qué realiza el operador LINQ ''Where'' sobre una colección?',
 N'var fuertes = enemigos.Where(e => e.DañoBase >= 13);',
 N'Ordena la colección de mayor a menor', N'Modifica directamente la lista original borrando elementos', N'Filtra y devuelve los elementos que cumplen la condición booleana', N'Cuenta cuántos elementos existen y devuelve un int',
 2, N'Where recibe un predicado (expresión lambda que retorna bool) y produce una secuencia filtrada sin mutar la colección original.', 1),

(18, N'Colecciones y LINQ', N'Intermedio',
 N'¿Cuándo se ejecuta realmente una consulta LINQ con ejecución diferida como ''Where'' o ''Select''?',
 N'var consulta = objetos.Where(o => o.Usos > 1);',
 N'Exactamente en la línea donde se declara la variable ''consulta''', N'Al compilar el proyecto en Visual Studio', N'Solo cuando se cierra el programa', N'Cuando se itera (foreach) o se materializa (ToList(), Count(), First())',
 3, N'La ejecución diferida (deferred execution) posterga la evaluación hasta que los datos son efectivamente recorridos o convertidos con ToList()/ToArray().', 1),

(19, N'Colecciones y LINQ', N'Básico',
 N'¿Qué devuelve el método LINQ ''Select'' en el siguiente ejemplo?',
 N'var nombres = inventario.Select(obj => obj.Nombre);',
 N'El primer objeto del inventario', N'Una secuencia proyectada con los nombres (IEnumerable<string>)', N'Un booleano indicando si el inventario tiene elementos', N'La cantidad total de usos disponibles',
 1, N'Select transforma (proyecta) cada elemento de entrada en un nuevo valor o tipo; en este caso extrae solo la propiedad Nombre de cada Objeto.', 1),

(20, N'Colecciones y LINQ', N'Intermedio',
 N'¿Qué devuelve ''FirstOrDefault()'' si ningún elemento de una ''List<Objeto>'' cumple la condición?',
 N'Objeto hallado = inventario.FirstOrDefault(o => o.Usos > 99);',
 N'null (valor por defecto para tipos por referencia)', N'Lanza InvalidOperationException', N'Crea un Objeto nuevo automáticamente', N'Devuelve el último elemento de la lista',
 0, N'A diferencia de First() que lanza excepción si no hay coincidencias, FirstOrDefault() devuelve el valor por defecto del tipo (null para clases, 0 para int).', 1),

(21, N'Colecciones y LINQ', N'Básico',
 N'¿Cuál es la diferencia principal entre un arreglo ''Objeto[]'' y una ''List<Objeto>''?',
 NULL,
 N'Los arreglos no permiten acceso por índice', N'List<T> solo puede guardar enteros y cadenas', N'El arreglo tiene tamaño fijo; List<T> crece y decrece dinámicamente', N'List<T> no puede recorrerse con foreach',
 2, N'Un arreglo en C# tiene longitud fija al crearse, mientras que List<T> administra un arreglo interno que se redimensiona automáticamente al usar Add/Remove.', 1),

(22, N'Excepciones, Memoria y .NET', N'Básico',
 N'En un bloque ''try / catch / finally'', ¿cuándo se ejecuta el bloque ''finally''?',
 N'try { Abrir(); } catch { Log(); } finally { Cerrar(); }',
 N'Únicamente si ocurrió una excepción', N'Siempre, tanto si hubo excepción como si todo terminó bien', N'Solo cuando no ocurre ningún error', N'Únicamente en compilación Debug',
 1, N'El bloque ''finally'' se ejecuta incondicionalmente al salir del try/catch, siendo ideal para cerrar archivos, conexiones o liberar recursos.', 1),

(23, N'Excepciones, Memoria y .NET', N'Intermedio',
 N'¿Por qué es buena práctica usar ''using'' al crear objetos GDI+ como ''Pen'', ''Brush'' o ''Font''?',
 N'using Pen lapiz = new Pen(Color.Cyan, 2f);',
 N'Garantiza llamar a Dispose() automáticamente y evita fugas de memoria', N'Hace que el dibujo se ejecute en la GPU de forma directa', N'Aumenta la resolución del monitor', N'Evita tener que importar System.Drawing',
 0, N'Pen, Brush y Font envuelven recursos nativos de Windows (GDI+) e implementan IDisposable; ''using'' asegura su liberación inmediata al salir del ámbito.', 1),

(24, N'Excepciones, Memoria y .NET', N'Intermedio',
 N'¿Cómo se comporta la asignación entre variables de un tipo ''struct'' frente a un tipo ''class''?',
 N'Point p1 = new Point(1, 2); Point p2 = p1;
Usuario u1 = new Usuario(); Usuario u2 = u1;',
 N'Ambos copian solo la dirección de memoria', N'Ambos clonan todos los datos de forma independiente', N'struct copia el valor completo; class copia la referencia al mismo objeto', N'struct siempre vive en disco y class en RAM',
 2, N'struct es tipo por valor (modificar p2 no afecta a p1), mientras que class es tipo por referencia (u1 y u2 apuntan al mismo objeto en el heap).', 1),

(25, N'Excepciones, Memoria y .NET', N'Básico',
 N'¿Qué causa una ''NullReferenceException'' en tiempo de ejecución?',
 N'Usuario cadete = null;
string n = cadete.Nombre;',
 N'Dividir un entero entre cero', N'Acceder a un índice negativo de un arreglo', N'Escribir un nombre de variable mal escrito', N'Intentar acceder a una propiedad o método de una variable que vale null',
 3, N'NullReferenceException ocurre al desreferenciar una variable de tipo referencia que no apunta a ninguna instancia válida en memoria.', 1),

(26, N'Excepciones, Memoria y .NET', N'Intermedio',
 N'¿Para qué sirve el atributo ''[JsonIgnore]'' sobre la propiedad ''MapaLocal'' en NEXUS?',
 N'[JsonIgnore]
public char[,] MapaLocal { get; private set; }',
 N'Para cifrar la propiedad dentro del archivo JSON', N'Para excluir esa propiedad al serializar y deserializar con System.Text.Json', N'Para convertir la matriz 2D en texto plano automáticamente', N'Para evitar que el compilador verifique errores de tipo',
 1, N'System.Text.Json no serializa arreglos multidimensionales (char[,]) directamente; [JsonIgnore] indica que esa propiedad debe omitirse en el JSON.', 1),

(27, N'Excepciones, Memoria y .NET', N'Básico',
 N'¿Cuál es la función principal del Garbage Collector (GC) en .NET?',
 NULL,
 N'Liberar automáticamente la memoria administrada de objetos que ya no tienen referencias', N'Eliminar archivos temporales del escritorio de Windows', N'Detectar errores de sintaxis antes de compilar', N'Comprimir imágenes PNG dentro de la carpeta Assets',
 0, N'El GC rastrea objetos en el heap administrado y recupera la memoria de aquellos que ya no son alcanzables desde el código activo.', 1),

(28, N'Excepciones, Memoria y .NET', N'Básico',
 N'¿Por qué conviene usar ''Path.Combine(carpeta, archivo)'' en lugar de concatenar cadenas con ''+''?',
 N'string ruta = Path.Combine(directorioSaves, "perfil.json");',
 N'Porque crea automáticamente el archivo en el disco', N'Porque valida que el JSON no esté corrupto', N'Porque gestiona separadores de directorio de forma segura y evita barras mal formadas', N'Porque otorga permisos de administrador en Program Files',
 2, N'Path.Combine inserta el separador correcto del sistema operativo y evita errores comunes por barras faltantes o duplicadas.', 1);

SET IDENTITY_INSERT dbo.T_Preguntas_CSharp OFF;
GO

PRINT '========================================================================';
PRINT '  [NEXUS-SQL]: DESPLIEGUE INTEGRAL COMPLETADO CON ÉXITO.';
PRINT '========================================================================';
GO
