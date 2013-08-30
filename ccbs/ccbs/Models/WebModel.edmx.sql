
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/29/2013 11:20:17
-- Generated from EDMX file: E:\courses\Web-Design\asp.mvc\design\UTDBaike\ccbs\ccbs\Models\WebModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [UTDBaikeDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- Creating table 'Merchants'
CREATE TABLE [dbo].[Merchants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [LastUpdate] datetime  NOT NULL,
    [DueDate] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Merchants'
ALTER TABLE [dbo].[Merchants]
ADD CONSTRAINT [PK_Merchants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO


-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------