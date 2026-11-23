-- ============================================================================
-- PROYECTO: NEXUS - SISTEMA DE SIMULACIÓN Y GESTIÓN MULTIVERSAL
-- ASIGNATURA: PROGRAMACIÓN II - UNIFRANZ COCHABAMBA
-- AUTOR: SEBASTIÁN ZUBIETA QUIROGA
-- SCRIPT 01: CREACIÓN DE LA BASE DE DATOS 'NexusDB'
-- ============================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'NexusDB')
BEGIN
    CREATE DATABASE [NexusDB]
    COLLATE Latin1_General_CI_AS;
    PRINT '[NEXUS-SQL]: Base de datos NexusDB creada satisfactoriamente.';
END
ELSE
BEGIN
    PRINT '[NEXUS-SQL]: La base de datos NexusDB ya existe en el servidor.';
END
GO

USE [NexusDB];
GO

-- Configuración recomendada para integridad transaccional
ALTER DATABASE [NexusDB] SET ANSI_NULL_DEFAULT OFF;
ALTER DATABASE [NexusDB] SET ANSI_NULLS ON;
ALTER DATABASE [NexusDB] SET ANSI_PADDING ON;
ALTER DATABASE [NexusDB] SET ANSI_WARNINGS ON;
ALTER DATABASE [NexusDB] SET ARITHABORT ON;
ALTER DATABASE [NexusDB] SET CONCAT_NULL_YIELDS_NULL ON;
ALTER DATABASE [NexusDB] SET NUMERIC_ROUNDABORT OFF;
ALTER DATABASE [NexusDB] SET QUOTED_IDENTIFIER ON;
GO

PRINT '[NEXUS-SQL]: Parámetros de entorno configurados para NexusDB.';
GO
