-- ============================================================================
-- PROYECTO: NEXUS - SISTEMA DE SIMULACIÓN Y GESTIÓN MULTIVERSAL
-- ASIGNATURA: PROGRAMACIÓN II - UNIFRANZ COCHABAMBA
-- AUTOR: SEBASTIÁN ZUBIETA QUIROGA
-- SCRIPT 02: CREACIÓN DE TABLAS, CLAVES Y RELACIONES (3FN)
-- ============================================================================

USE [NexusDB];
GO

-- 1. Eliminación en orden inverso para preservar integridad referencial si ya existen
IF OBJECT_ID('dbo.T_Historial_Incursiones', 'U') IS NOT NULL DROP TABLE dbo.T_Historial_Incursiones;
IF OBJECT_ID('dbo.T_Observaciones', 'U') IS NOT NULL DROP TABLE dbo.T_Observaciones;
IF OBJECT_ID('dbo.T_Inventarios', 'U') IS NOT NULL DROP TABLE dbo.T_Inventarios;
IF OBJECT_ID('dbo.T_Partidas_Activas', 'U') IS NOT NULL DROP TABLE dbo.T_Partidas_Activas;
IF OBJECT_ID('dbo.T_Jugadores', 'U') IS NOT NULL DROP TABLE dbo.T_Jugadores;
IF OBJECT_ID('dbo.T_Preguntas_CSharp', 'U') IS NOT NULL DROP TABLE dbo.T_Preguntas_CSharp;
GO

-- ============================================================================
-- TABLA 1: T_Jugadores
-- Entidad central del cadete explorador. Conserva perfil biométrico, progreso,
-- mejoras de atributos adquiridas y métricas del módulo didáctico de C#.
-- ============================================================================
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

-- ============================================================================
-- TABLA 2: T_Partidas_Activas
-- Mapea el estado actual de la realidad asignada y los recursos volátiles.
-- Relación 1 a 1 con T_Jugadores (garantizada por restricción UNIQUE en FK_JugadorId).
-- ============================================================================
CREATE TABLE dbo.T_Partidas_Activas
(
    PK_PartidaId        INT IDENTITY(1,1) NOT NULL,
    FK_JugadorId        INT               NOT NULL,
    VidaActual          INT               NOT NULL,
    EnergiaActual       INT               NOT NULL,
    Fragmentos          INT               NOT NULL CONSTRAINT DF_Partidas_Fragmentos DEFAULT 0,
    Restos              INT               NOT NULL CONSTRAINT DF_Partidas_Restos DEFAULT 0,
    NombreRealidad      NVARCHAR(100)     NOT NULL,
    TipoAnomalia        TINYINT           NOT NULL, -- 0: Tiempo, 1: Espacio, 2: Gravedad, 3: Entropía
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

-- ============================================================================
-- TABLA 3: T_Inventarios
-- Mochila de objetos tácticos del cadete. Relación 1 a N con T_Jugadores.
-- ============================================================================
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

-- ============================================================================
-- TABLA 4: T_Observaciones
-- Pistas sensoriales decodificadas con Fragmentos para deducir la anomalía.
-- Relación 1 a N con T_Jugadores.
-- ============================================================================
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

-- ============================================================================
-- TABLA 5: T_Preguntas_CSharp
-- Catálogo centralizado del módulo didáctico de C#. Permite actualizar preguntas
-- sin recompilar el cliente de escritorio.
-- ============================================================================
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

-- ============================================================================
-- TABLA 6: T_Historial_Incursiones (Adicional para auditoría y telemetría)
-- Registra el historial de incursiones completadas, evacuadas o colapsadas.
-- Relación 1 a N con T_Jugadores.
-- ============================================================================
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

-- ============================================================================
-- ÍNDICES NON-CLUSTERED PARA RENDIMIENTO DE CONSULTAS FRECUENTES
-- ============================================================================
CREATE NONCLUSTERED INDEX IX_Partidas_FK_JugadorId ON dbo.T_Partidas_Activas(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Inventarios_FK_JugadorId ON dbo.T_Inventarios(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Observaciones_FK_JugadorId ON dbo.T_Observaciones(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Historial_FK_JugadorId ON dbo.T_Historial_Incursiones(FK_JugadorId);
CREATE NONCLUSTERED INDEX IX_Preguntas_Categoria ON dbo.T_Preguntas_CSharp(Categoria, Activa);
GO

PRINT '[NEXUS-SQL]: 6 tablas y sus relaciones creadas exitosamente con restricciones de integridad.';
GO
