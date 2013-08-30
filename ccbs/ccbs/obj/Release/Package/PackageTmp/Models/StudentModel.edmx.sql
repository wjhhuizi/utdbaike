
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/04/2013 23:14:47
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

-- Creating table 'LocalHelpVolunteer'
CREATE TABLE [dbo].[LocalHelpVolunteer] (
    [LocalHelps_Id] int  NOT NULL,
    [Volunteers_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [LocalHelps_Id], [Volunteers_Id] in table 'LocalHelpVolunteer'
ALTER TABLE [dbo].[LocalHelpVolunteer]
ADD CONSTRAINT [PK_LocalHelpVolunteer]
    PRIMARY KEY NONCLUSTERED ([LocalHelps_Id], [Volunteers_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [LocalHelps_Id] in table 'LocalHelpVolunteer'
ALTER TABLE [dbo].[LocalHelpVolunteer]
ADD CONSTRAINT [FK_LocalHelpVolunteer_LocalHelp]
    FOREIGN KEY ([LocalHelps_Id])
    REFERENCES [dbo].[LocalHelps]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Volunteers_Id] in table 'LocalHelpVolunteer'
ALTER TABLE [dbo].[LocalHelpVolunteer]
ADD CONSTRAINT [FK_LocalHelpVolunteer_Volunteer]
    FOREIGN KEY ([Volunteers_Id])
    REFERENCES [dbo].[Volunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocalHelpVolunteer_Volunteer'
CREATE INDEX [IX_FK_LocalHelpVolunteer_Volunteer]
ON [dbo].[LocalHelpVolunteer]
    ([Volunteers_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------