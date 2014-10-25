
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/25/2014 23:47:21
-- Generated from EDMX file: D:\project\UTDBaike\ccbs\ccbs\Models\StudentModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [StudentDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- Creating table 'RegConfs'
CREATE TABLE [dbo].[RegConfs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [State] nvarchar(max)  NOT NULL,
    [ToggleDate] datetime  NOT NULL,
    [NextState] nvarchar(max)  NOT NULL
);
GO

-- Creating primary key on [Id] in table 'RegConfs'
ALTER TABLE [dbo].[RegConfs]
ADD CONSTRAINT [PK_RegConfs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------