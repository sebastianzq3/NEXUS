-- ============================================================================
-- PROYECTO: NEXUS - SISTEMA DE SIMULACIÓN Y GESTIÓN MULTIVERSAL
-- ASIGNATURA: PROGRAMACIÓN II - UNIFRANZ COCHABAMBA
-- AUTOR: SEBASTIÁN ZUBIETA QUIROGA
-- SCRIPT 03: PROCEDIMIENTOS ALMACENADOS (TRANSACCIONES ACID Y CONSULTAS)
-- ============================================================================

USE [NexusDB];
GO

-- 1. Listado de perfiles para la grilla principal
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

-- 2. Guardado atómico de cabecera de perfil y partida activa (Transaccional)
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

        -- 1. Insertar o actualizar jugador
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

        -- 2. Insertar o actualizar partida activa
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

-- 3. Cargar datos completos de un perfil por nombre
IF OBJECT_ID('dbo.sp_CargarPartidaPorNombre', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CargarPartidaPorNombre;
GO
CREATE PROCEDURE dbo.sp_CargarPartidaPorNombre
    @Nombre NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Conjunto 1: Jugador y Partida
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

    -- Conjunto 2: Inventario
    SELECT i.NombreObjeto, i.TipoAnomalia, i.Descripcion, i.UsosRestantes
    FROM dbo.T_Inventarios i
    INNER JOIN dbo.T_Jugadores j ON i.FK_JugadorId = j.PK_JugadorId
    WHERE j.Nombre = @Nombre;

    -- Conjunto 3: Observaciones
    SELECT o.TextoPista, o.FechaRegistro
    FROM dbo.T_Observaciones o
    INNER JOIN dbo.T_Jugadores j ON o.FK_JugadorId = j.PK_JugadorId
    WHERE j.Nombre = @Nombre
    ORDER BY o.FechaRegistro ASC;
END;
GO

-- 4. Purgar / Eliminar perfil por nombre
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

-- 5. Obtener catálogo de preguntas didácticas de C#
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

PRINT '[NEXUS-SQL]: Procedimientos almacenados creados satisfactoriamente.';
GO
