
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/04/2013 12:21:50
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

ALTER TABLE [dbo].[NewStudents]
ADD [ApplyFacssDepartment_Id] int  NULL;
Go

-- Creating table 'Students'
CREATE TABLE [dbo].[Students] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Gender] int  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NULL,
    [Major] nvarchar(max)  NULL,
    [Year] int  NOT NULL,
    [Term] nvarchar(max)  NOT NULL,
    [ComeFrom] nvarchar(max)  NULL,
    [CnName] nvarchar(max)  NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [RegDate] datetime  NOT NULL,
    [LastUpdate] datetime  NOT NULL,
    [PickupVolunteer_Id] int  NULL,
    [HostVolunteer_Id] int  NULL,
    [HostOrganization_Id] int  NULL,
    [HostGroup_Id] int  NULL,
    [ManualPickupVolunteer_Id] int  NULL,
    [ManualHostVolunteer_Id] int  NULL
);
GO

-- Creating table 'ManualVolunteers'
CREATE TABLE [dbo].[ManualVolunteers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Gender] int  NOT NULL,
    [Email] nvarchar(max)  NULL,
    [Phone] nvarchar(max)  NULL,
    [Address] nvarchar(max)  NULL
);
GO

-- Creating table 'FacssDepartments'
CREATE TABLE [dbo].[FacssDepartments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Introduction] nvarchar(max)  NULL,
    [IsOpenForApply] bit  NOT NULL,
    [ApplyNotification] nvarchar(max)  NULL
);
GO

-- Creating table 'RegisterEntryStudent'
CREATE TABLE [dbo].[RegisterEntryStudent] (
    [RegisterEntries_Id] int  NOT NULL,
    [Students_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [PK_Students]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ManualVolunteers'
ALTER TABLE [dbo].[ManualVolunteers]
ADD CONSTRAINT [PK_ManualVolunteers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FacssDepartments'
ALTER TABLE [dbo].[FacssDepartments]
ADD CONSTRAINT [PK_FacssDepartments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [RegisterEntries_Id], [Students_Id] in table 'RegisterEntryStudent'
ALTER TABLE [dbo].[RegisterEntryStudent]
ADD CONSTRAINT [PK_RegisterEntryStudent]
    PRIMARY KEY NONCLUSTERED ([RegisterEntries_Id], [Students_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------
-- Creating foreign key on [PickupVolunteer_Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [FK_VolunteerStudent]
    FOREIGN KEY ([PickupVolunteer_Id])
    REFERENCES [dbo].[Volunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VolunteerStudent'
CREATE INDEX [IX_FK_VolunteerStudent]
ON [dbo].[Students]
    ([PickupVolunteer_Id]);
GO

-- Creating foreign key on [HostVolunteer_Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [FK_HostStudent]
    FOREIGN KEY ([HostVolunteer_Id])
    REFERENCES [dbo].[Volunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HostStudent'
CREATE INDEX [IX_FK_HostStudent]
ON [dbo].[Students]
    ([HostVolunteer_Id]);
GO

-- Creating foreign key on [HostOrganization_Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [FK_HostOrganization]
    FOREIGN KEY ([HostOrganization_Id])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HostOrganization'
CREATE INDEX [IX_FK_HostOrganization]
ON [dbo].[Students]
    ([HostOrganization_Id]);
GO

-- Creating foreign key on [HostGroup_Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [FK_GroupHostStudent]
    FOREIGN KEY ([HostGroup_Id])
    REFERENCES [dbo].[Groups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupHostStudent'
CREATE INDEX [IX_FK_GroupHostStudent]
ON [dbo].[Students]
    ([HostGroup_Id]);
GO

-- Creating foreign key on [ManualPickupVolunteer_Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [FK_ManualPickupAssign]
    FOREIGN KEY ([ManualPickupVolunteer_Id])
    REFERENCES [dbo].[ManualVolunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ManualPickupAssign'
CREATE INDEX [IX_FK_ManualPickupAssign]
ON [dbo].[Students]
    ([ManualPickupVolunteer_Id]);
GO

-- Creating foreign key on [ManualHostVolunteer_Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [FK_ManualHostAssign]
    FOREIGN KEY ([ManualHostVolunteer_Id])
    REFERENCES [dbo].[ManualVolunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ManualHostAssign'
CREATE INDEX [IX_FK_ManualHostAssign]
ON [dbo].[Students]
    ([ManualHostVolunteer_Id]);
GO

-- Creating foreign key on [RegisterEntries_Id] in table 'RegisterEntryStudent'
ALTER TABLE [dbo].[RegisterEntryStudent]
ADD CONSTRAINT [FK_RegisterEntryStudent_RegisterEntry]
    FOREIGN KEY ([RegisterEntries_Id])
    REFERENCES [dbo].[RegisterEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Students_Id] in table 'RegisterEntryStudent'
ALTER TABLE [dbo].[RegisterEntryStudent]
ADD CONSTRAINT [FK_RegisterEntryStudent_Student]
    FOREIGN KEY ([Students_Id])
    REFERENCES [dbo].[Students]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RegisterEntryStudent_Student'
CREATE INDEX [IX_FK_RegisterEntryStudent_Student]
ON [dbo].[RegisterEntryStudent]
    ([Students_Id]);
GO

-- Creating foreign key on [ApplyFacssDepartment_Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [FK_ApplyForFacssDepartment]
    FOREIGN KEY ([ApplyFacssDepartment_Id])
    REFERENCES [dbo].[FacssDepartments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ApplyForFacssDepartment'
CREATE INDEX [IX_FK_ApplyForFacssDepartment]
ON [dbo].[NewStudents]
    ([ApplyFacssDepartment_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------