
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/20/2014 09:20:54
-- Generated from EDMX file: E:\Web-Design\asp.mvc\design\UTDBaike\ccbs\ccbs\Models\WebModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [UTDBaikeDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_EmailAccountDailyCount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DailyCounts] DROP CONSTRAINT [FK_EmailAccountDailyCount];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------
IF OBJECT_ID(N'[dbo].[EmailAccounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmailAccounts];
GO

IF OBJECT_ID(N'[dbo].[DailyCounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DailyCounts];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'EmailAccounts'
CREATE TABLE [dbo].[EmailAccounts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Host] nvarchar(max)  NULL,
    [Port] int  NOT NULL,
    [Username] nvarchar(max)  NULL,
    [Password] nvarchar(max)  NULL,
    [From] nvarchar(max)  NULL,
    [SmtpDailyLimit] int  NOT NULL,
    [SmtpPerTimeLimit] int  NOT NULL,
    [Verified] bit  NOT NULL,
    [LastVerifyDate] datetime  NOT NULL,
    [VerifyMessage] nvarchar(max)  NULL
);
GO

-- Creating table 'DailyCounts'
CREATE TABLE [dbo].[DailyCounts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [WhichDate] datetime  NOT NULL,
    [Count] int  NOT NULL,
    [EmailAccountId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'EmailAccounts'
ALTER TABLE [dbo].[EmailAccounts]
ADD CONSTRAINT [PK_EmailAccounts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DailyCounts'
ALTER TABLE [dbo].[DailyCounts]
ADD CONSTRAINT [PK_DailyCounts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [EmailAccountId] in table 'DailyCounts'
ALTER TABLE [dbo].[DailyCounts]
ADD CONSTRAINT [FK_EmailAccountDailyCount]
    FOREIGN KEY ([EmailAccountId])
    REFERENCES [dbo].[EmailAccounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EmailAccountDailyCount'
CREATE INDEX [IX_FK_EmailAccountDailyCount]
ON [dbo].[DailyCounts]
    ([EmailAccountId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------