
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/26/2013 00:51:22
-- Generated from EDMX file: E:\courses\Web-Design\asp.mvc\design\UTDBaike\ccbs\ccbs\Models\WebModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [UTDBaikeDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

ALTER TABLE [dbo].[Articles]
ADD [VisitCount] int not null default 0;


ALTER TABLE [dbo].[Questions]
ADD [VisitCount] int not null default 0;


-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------