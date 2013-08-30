
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/15/2013 13:26:46
-- Generated from EDMX file: E:\courses\Web-Design\asp.mvc\design\UTDBaike\ccbs\ccbs\Models\StudentModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [StudentDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'EmailHistories'
CREATE TABLE [dbo].[EmailHistories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [LastSend] datetime  NOT NULL,
    [Title] nvarchar(max)  NULL,
    [Body] nvarchar(max)  NULL,
    [Confirmed] bit  NOT NULL,
    [ToEmail] nvarchar(max)  NOT NULL,
    [NewStudentId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'EmailHistories'
ALTER TABLE [dbo].[EmailHistories]
ADD CONSTRAINT [PK_EmailHistories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [NewStudentId] in table 'EmailHistories'
ALTER TABLE [dbo].[EmailHistories]
ADD CONSTRAINT [FK_NewStudentEmailHistory]
    FOREIGN KEY ([NewStudentId])
    REFERENCES [dbo].[NewStudents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NewStudentEmailHistory'
CREATE INDEX [IX_FK_NewStudentEmailHistory]
ON [dbo].[EmailHistories]
    ([NewStudentId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------