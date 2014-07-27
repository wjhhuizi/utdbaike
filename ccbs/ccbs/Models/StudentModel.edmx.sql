
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/19/2014 21:06:59
-- Generated from EDMX file: E:\Web-Design\asp.mvc\design\UTDBaike\ccbs\ccbs\Models\StudentModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [StudentDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_NewStudentVolunteer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudents] DROP CONSTRAINT [FK_NewStudentVolunteer];
GO
IF OBJECT_ID(N'[dbo].[FK_NewStudentVolunteer1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudents] DROP CONSTRAINT [FK_NewStudentVolunteer1];
GO
IF OBJECT_ID(N'[dbo].[FK_NewStudentOrg]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudents] DROP CONSTRAINT [FK_NewStudentOrg];
GO
IF OBJECT_ID(N'[dbo].[FK_AdminOrganization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Volunteers] DROP CONSTRAINT [FK_AdminOrganization];
GO
IF OBJECT_ID(N'[dbo].[FK_OrgRequestOrganization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrgRequests] DROP CONSTRAINT [FK_OrgRequestOrganization];
GO
IF OBJECT_ID(N'[dbo].[FK_OrganizationVolunteer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Volunteers] DROP CONSTRAINT [FK_OrganizationVolunteer];
GO
IF OBJECT_ID(N'[dbo].[FK_NewStudentManualAssignInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ManualAssignInfoes] DROP CONSTRAINT [FK_NewStudentManualAssignInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_OrganizationGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Groups] DROP CONSTRAINT [FK_OrganizationGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_VolunteerAdminGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Volunteers] DROP CONSTRAINT [FK_VolunteerAdminGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupNewStudent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudents] DROP CONSTRAINT [FK_GroupNewStudent];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupVolunteer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Volunteers] DROP CONSTRAINT [FK_GroupVolunteer];
GO
IF OBJECT_ID(N'[dbo].[FK_TempPoolNewStudent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudents] DROP CONSTRAINT [FK_TempPoolNewStudent];
GO
IF OBJECT_ID(N'[dbo].[FK_NewStudentRegisterEntry_NewStudent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudentRegisterEntry] DROP CONSTRAINT [FK_NewStudentRegisterEntry_NewStudent];
GO
IF OBJECT_ID(N'[dbo].[FK_NewStudentRegisterEntry_RegisterEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudentRegisterEntry] DROP CONSTRAINT [FK_NewStudentRegisterEntry_RegisterEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_RegisterEntryGuestParticipant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GuestParticipants] DROP CONSTRAINT [FK_RegisterEntryGuestParticipant];
GO
IF OBJECT_ID(N'[dbo].[FK_LocalHelpRegisterEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegisterEntries] DROP CONSTRAINT [FK_LocalHelpRegisterEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_OrganizationLocalHelp]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LocalHelps] DROP CONSTRAINT [FK_OrganizationLocalHelp];
GO
IF OBJECT_ID(N'[dbo].[FK_NewStudentEmailHistory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmailHistories] DROP CONSTRAINT [FK_NewStudentEmailHistory];
GO
IF OBJECT_ID(N'[dbo].[FK_LocalHelpVolunteer_LocalHelp]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LocalHelpVolunteer] DROP CONSTRAINT [FK_LocalHelpVolunteer_LocalHelp];
GO
IF OBJECT_ID(N'[dbo].[FK_LocalHelpVolunteer_Volunteer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LocalHelpVolunteer] DROP CONSTRAINT [FK_LocalHelpVolunteer_Volunteer];
GO
IF OBJECT_ID(N'[dbo].[FK_VolunteerStudent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Students] DROP CONSTRAINT [FK_VolunteerStudent];
GO
IF OBJECT_ID(N'[dbo].[FK_HostStudent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Students] DROP CONSTRAINT [FK_HostStudent];
GO
IF OBJECT_ID(N'[dbo].[FK_HostOrganization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Students] DROP CONSTRAINT [FK_HostOrganization];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupHostStudent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Students] DROP CONSTRAINT [FK_GroupHostStudent];
GO
IF OBJECT_ID(N'[dbo].[FK_ManualPickupAssign]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Students] DROP CONSTRAINT [FK_ManualPickupAssign];
GO
IF OBJECT_ID(N'[dbo].[FK_ManualHostAssign]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Students] DROP CONSTRAINT [FK_ManualHostAssign];
GO
IF OBJECT_ID(N'[dbo].[FK_RegisterEntryStudent_RegisterEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegisterEntryStudent] DROP CONSTRAINT [FK_RegisterEntryStudent_RegisterEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_RegisterEntryStudent_Student]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegisterEntryStudent] DROP CONSTRAINT [FK_RegisterEntryStudent_Student];
GO
IF OBJECT_ID(N'[dbo].[FK_ApplyForFacssDepartment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NewStudents] DROP CONSTRAINT [FK_ApplyForFacssDepartment];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[NewStudents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NewStudents];
GO
IF OBJECT_ID(N'[dbo].[Volunteers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Volunteers];
GO
IF OBJECT_ID(N'[dbo].[Organizations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Organizations];
GO
IF OBJECT_ID(N'[dbo].[OrgRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrgRequests];
GO
IF OBJECT_ID(N'[dbo].[LocalHelps]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LocalHelps];
GO
IF OBJECT_ID(N'[dbo].[GuestParticipants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GuestParticipants];
GO
IF OBJECT_ID(N'[dbo].[ManualAssignInfoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ManualAssignInfoes];
GO
IF OBJECT_ID(N'[dbo].[Groups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Groups];
GO
IF OBJECT_ID(N'[dbo].[OperationRecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OperationRecords];
GO
IF OBJECT_ID(N'[dbo].[TempPools]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TempPools];
GO
IF OBJECT_ID(N'[dbo].[RegisterEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegisterEntries];
GO
IF OBJECT_ID(N'[dbo].[EmailHistories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmailHistories];
GO
IF OBJECT_ID(N'[dbo].[Students]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Students];
GO
IF OBJECT_ID(N'[dbo].[ManualVolunteers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ManualVolunteers];
GO
IF OBJECT_ID(N'[dbo].[FacssDepartments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FacssDepartments];
GO
IF OBJECT_ID(N'[dbo].[Disclaimers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Disclaimers];
GO
IF OBJECT_ID(N'[dbo].[NewStudentRegisterEntry]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NewStudentRegisterEntry];
GO
IF OBJECT_ID(N'[dbo].[LocalHelpVolunteer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LocalHelpVolunteer];
GO
IF OBJECT_ID(N'[dbo].[RegisterEntryStudent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegisterEntryStudent];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'NewStudents'
CREATE TABLE [dbo].[NewStudents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Gender] int  NOT NULL,
    [Email] nvarchar(max)  NULL,
    [Phone] nvarchar(max)  NULL,
    [Major] nvarchar(max)  NULL,
    [Year] int  NOT NULL,
    [Term] nvarchar(max)  NULL,
    [ArrivalTime] datetime  NOT NULL,
    [Flight] nvarchar(max)  NULL,
    [NeedPickup] bit  NOT NULL,
    [NeedTempHousing] bit  NOT NULL,
    [Note] nvarchar(max)  NULL,
    [EntryPort] nvarchar(max)  NULL,
    [ComeFrom] nvarchar(max)  NULL,
    [CnName] nvarchar(max)  NULL,
    [RegTime] datetime  NOT NULL,
    [HasApt] bit  NOT NULL,
    [WhenApt] nvarchar(max)  NULL,
    [WhereApt] nvarchar(max)  NULL,
    [WillingToHelp] bit  NOT NULL,
    [HelpNote] nvarchar(max)  NULL,
    [Marked] bit  NOT NULL,
    [DaysOfHousing] int  NOT NULL,
    [Confirmed] int  NOT NULL,
    [LastUpdate] datetime  NOT NULL,
    [IsManualAssigned] bit  NOT NULL,
    [ManualAssignedPickup] nvarchar(max)  NULL,
    [ManualAssignedHost] nvarchar(max)  NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [PickupVolunteer_Id] int  NULL,
    [TempHouseVolunteer_Id] int  NULL,
    [Organization_Id] int  NULL,
    [Group_Id] int  NULL,
    [TempPool_Id] int  NULL,
    [ApplyFacssDepartment_Id] int  NULL
);
GO

-- Creating table 'Volunteers'
CREATE TABLE [dbo].[Volunteers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Gender] int  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NULL,
    [BriefIntro] nvarchar(max)  NULL,
    [Note] nvarchar(max)  NULL,
    [Address] nvarchar(max)  NULL,
    [OrganizationId] int  NOT NULL,
    [RegTime] datetime  NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [HelpType] nvarchar(max)  NULL,
    [RelationToUTD] nvarchar(max)  NULL,
    [GroupId] int  NULL,
    [AdminOrg_Id] int  NULL,
    [AdminGroup_Id] int  NULL
);
GO

-- Creating table 'Organizations'
CREATE TABLE [dbo].[Organizations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Passcode] nvarchar(max)  NULL,
    [ModelType] int  NOT NULL
);
GO

-- Creating table 'OrgRequests'
CREATE TABLE [dbo].[OrgRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NumOfNews] int  NOT NULL,
    [Note] nvarchar(max)  NULL,
    [Reply] nvarchar(max)  NULL,
    [Progress] nvarchar(max)  NULL,
    [RequestDate] datetime  NULL,
    [Organization_Id] int  NOT NULL
);
GO

-- Creating table 'LocalHelps'
CREATE TABLE [dbo].[LocalHelps] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Restriction] int  NOT NULL,
    [OrganizationId] int  NULL
);
GO

-- Creating table 'GuestParticipants'
CREATE TABLE [dbo].[GuestParticipants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [IsChristian] bit  NOT NULL,
    [RegisterEntryId] int  NOT NULL
);
GO

-- Creating table 'ManualAssignInfoes'
CREATE TABLE [dbo].[ManualAssignInfoes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [VolName] nvarchar(max)  NOT NULL,
    [VolGender] int  NOT NULL,
    [VolEmail] nvarchar(max)  NULL,
    [VolPhone] nvarchar(max)  NULL,
    [VolAddr] nvarchar(max)  NULL,
    [LastUpdate] datetime  NOT NULL,
    [NewStudentId] int  NOT NULL
);
GO

-- Creating table 'Groups'
CREATE TABLE [dbo].[Groups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Passcode] nvarchar(max)  NULL,
    [OrganizationId] int  NOT NULL
);
GO

-- Creating table 'OperationRecords'
CREATE TABLE [dbo].[OperationRecords] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [Arg1] int  NOT NULL,
    [Arg2] int  NOT NULL,
    [Data] nvarchar(max)  NULL,
    [CreatedDate] datetime  NOT NULL,
    [Link] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'TempPools'
CREATE TABLE [dbo].[TempPools] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LastUpdate] datetime  NOT NULL,
    [UserName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RegisterEntries'
CREATE TABLE [dbo].[RegisterEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Time] datetime  NOT NULL,
    [Address] nvarchar(max)  NULL,
    [Contact] nvarchar(max)  NULL,
    [IsActive] bit  NOT NULL,
    [LocalHelpId] int  NOT NULL
);
GO

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

-- Creating table 'Disclaimers'
CREATE TABLE [dbo].[Disclaimers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [LastUpdate] datetime  NOT NULL
);
GO

-- Creating table 'NewStudentRegisterEntry'
CREATE TABLE [dbo].[NewStudentRegisterEntry] (
    [NewStudents_Id] int  NOT NULL,
    [RegisterEntries_Id] int  NOT NULL
);
GO

-- Creating table 'LocalHelpVolunteer'
CREATE TABLE [dbo].[LocalHelpVolunteer] (
    [LocalHelps_Id] int  NOT NULL,
    [Volunteers_Id] int  NOT NULL
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

-- Creating primary key on [Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [PK_NewStudents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Volunteers'
ALTER TABLE [dbo].[Volunteers]
ADD CONSTRAINT [PK_Volunteers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Organizations'
ALTER TABLE [dbo].[Organizations]
ADD CONSTRAINT [PK_Organizations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrgRequests'
ALTER TABLE [dbo].[OrgRequests]
ADD CONSTRAINT [PK_OrgRequests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LocalHelps'
ALTER TABLE [dbo].[LocalHelps]
ADD CONSTRAINT [PK_LocalHelps]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GuestParticipants'
ALTER TABLE [dbo].[GuestParticipants]
ADD CONSTRAINT [PK_GuestParticipants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ManualAssignInfoes'
ALTER TABLE [dbo].[ManualAssignInfoes]
ADD CONSTRAINT [PK_ManualAssignInfoes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Groups'
ALTER TABLE [dbo].[Groups]
ADD CONSTRAINT [PK_Groups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OperationRecords'
ALTER TABLE [dbo].[OperationRecords]
ADD CONSTRAINT [PK_OperationRecords]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TempPools'
ALTER TABLE [dbo].[TempPools]
ADD CONSTRAINT [PK_TempPools]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegisterEntries'
ALTER TABLE [dbo].[RegisterEntries]
ADD CONSTRAINT [PK_RegisterEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmailHistories'
ALTER TABLE [dbo].[EmailHistories]
ADD CONSTRAINT [PK_EmailHistories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

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

-- Creating primary key on [Id] in table 'Disclaimers'
ALTER TABLE [dbo].[Disclaimers]
ADD CONSTRAINT [PK_Disclaimers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [NewStudents_Id], [RegisterEntries_Id] in table 'NewStudentRegisterEntry'
ALTER TABLE [dbo].[NewStudentRegisterEntry]
ADD CONSTRAINT [PK_NewStudentRegisterEntry]
    PRIMARY KEY NONCLUSTERED ([NewStudents_Id], [RegisterEntries_Id] ASC);
GO

-- Creating primary key on [LocalHelps_Id], [Volunteers_Id] in table 'LocalHelpVolunteer'
ALTER TABLE [dbo].[LocalHelpVolunteer]
ADD CONSTRAINT [PK_LocalHelpVolunteer]
    PRIMARY KEY NONCLUSTERED ([LocalHelps_Id], [Volunteers_Id] ASC);
GO

-- Creating primary key on [RegisterEntries_Id], [Students_Id] in table 'RegisterEntryStudent'
ALTER TABLE [dbo].[RegisterEntryStudent]
ADD CONSTRAINT [PK_RegisterEntryStudent]
    PRIMARY KEY NONCLUSTERED ([RegisterEntries_Id], [Students_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [PickupVolunteer_Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [FK_NewStudentVolunteer]
    FOREIGN KEY ([PickupVolunteer_Id])
    REFERENCES [dbo].[Volunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NewStudentVolunteer'
CREATE INDEX [IX_FK_NewStudentVolunteer]
ON [dbo].[NewStudents]
    ([PickupVolunteer_Id]);
GO

-- Creating foreign key on [TempHouseVolunteer_Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [FK_NewStudentVolunteer1]
    FOREIGN KEY ([TempHouseVolunteer_Id])
    REFERENCES [dbo].[Volunteers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NewStudentVolunteer1'
CREATE INDEX [IX_FK_NewStudentVolunteer1]
ON [dbo].[NewStudents]
    ([TempHouseVolunteer_Id]);
GO

-- Creating foreign key on [Organization_Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [FK_NewStudentOrg]
    FOREIGN KEY ([Organization_Id])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NewStudentOrg'
CREATE INDEX [IX_FK_NewStudentOrg]
ON [dbo].[NewStudents]
    ([Organization_Id]);
GO

-- Creating foreign key on [AdminOrg_Id] in table 'Volunteers'
ALTER TABLE [dbo].[Volunteers]
ADD CONSTRAINT [FK_AdminOrganization]
    FOREIGN KEY ([AdminOrg_Id])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdminOrganization'
CREATE INDEX [IX_FK_AdminOrganization]
ON [dbo].[Volunteers]
    ([AdminOrg_Id]);
GO

-- Creating foreign key on [Organization_Id] in table 'OrgRequests'
ALTER TABLE [dbo].[OrgRequests]
ADD CONSTRAINT [FK_OrgRequestOrganization]
    FOREIGN KEY ([Organization_Id])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrgRequestOrganization'
CREATE INDEX [IX_FK_OrgRequestOrganization]
ON [dbo].[OrgRequests]
    ([Organization_Id]);
GO

-- Creating foreign key on [OrganizationId] in table 'Volunteers'
ALTER TABLE [dbo].[Volunteers]
ADD CONSTRAINT [FK_OrganizationVolunteer]
    FOREIGN KEY ([OrganizationId])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrganizationVolunteer'
CREATE INDEX [IX_FK_OrganizationVolunteer]
ON [dbo].[Volunteers]
    ([OrganizationId]);
GO

-- Creating foreign key on [NewStudentId] in table 'ManualAssignInfoes'
ALTER TABLE [dbo].[ManualAssignInfoes]
ADD CONSTRAINT [FK_NewStudentManualAssignInfo]
    FOREIGN KEY ([NewStudentId])
    REFERENCES [dbo].[NewStudents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NewStudentManualAssignInfo'
CREATE INDEX [IX_FK_NewStudentManualAssignInfo]
ON [dbo].[ManualAssignInfoes]
    ([NewStudentId]);
GO

-- Creating foreign key on [OrganizationId] in table 'Groups'
ALTER TABLE [dbo].[Groups]
ADD CONSTRAINT [FK_OrganizationGroup]
    FOREIGN KEY ([OrganizationId])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrganizationGroup'
CREATE INDEX [IX_FK_OrganizationGroup]
ON [dbo].[Groups]
    ([OrganizationId]);
GO

-- Creating foreign key on [AdminGroup_Id] in table 'Volunteers'
ALTER TABLE [dbo].[Volunteers]
ADD CONSTRAINT [FK_VolunteerAdminGroup]
    FOREIGN KEY ([AdminGroup_Id])
    REFERENCES [dbo].[Groups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VolunteerAdminGroup'
CREATE INDEX [IX_FK_VolunteerAdminGroup]
ON [dbo].[Volunteers]
    ([AdminGroup_Id]);
GO

-- Creating foreign key on [Group_Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [FK_GroupNewStudent]
    FOREIGN KEY ([Group_Id])
    REFERENCES [dbo].[Groups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupNewStudent'
CREATE INDEX [IX_FK_GroupNewStudent]
ON [dbo].[NewStudents]
    ([Group_Id]);
GO

-- Creating foreign key on [GroupId] in table 'Volunteers'
ALTER TABLE [dbo].[Volunteers]
ADD CONSTRAINT [FK_GroupVolunteer]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[Groups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupVolunteer'
CREATE INDEX [IX_FK_GroupVolunteer]
ON [dbo].[Volunteers]
    ([GroupId]);
GO

-- Creating foreign key on [TempPool_Id] in table 'NewStudents'
ALTER TABLE [dbo].[NewStudents]
ADD CONSTRAINT [FK_TempPoolNewStudent]
    FOREIGN KEY ([TempPool_Id])
    REFERENCES [dbo].[TempPools]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TempPoolNewStudent'
CREATE INDEX [IX_FK_TempPoolNewStudent]
ON [dbo].[NewStudents]
    ([TempPool_Id]);
GO

-- Creating foreign key on [NewStudents_Id] in table 'NewStudentRegisterEntry'
ALTER TABLE [dbo].[NewStudentRegisterEntry]
ADD CONSTRAINT [FK_NewStudentRegisterEntry_NewStudent]
    FOREIGN KEY ([NewStudents_Id])
    REFERENCES [dbo].[NewStudents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [RegisterEntries_Id] in table 'NewStudentRegisterEntry'
ALTER TABLE [dbo].[NewStudentRegisterEntry]
ADD CONSTRAINT [FK_NewStudentRegisterEntry_RegisterEntry]
    FOREIGN KEY ([RegisterEntries_Id])
    REFERENCES [dbo].[RegisterEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NewStudentRegisterEntry_RegisterEntry'
CREATE INDEX [IX_FK_NewStudentRegisterEntry_RegisterEntry]
ON [dbo].[NewStudentRegisterEntry]
    ([RegisterEntries_Id]);
GO

-- Creating foreign key on [RegisterEntryId] in table 'GuestParticipants'
ALTER TABLE [dbo].[GuestParticipants]
ADD CONSTRAINT [FK_RegisterEntryGuestParticipant]
    FOREIGN KEY ([RegisterEntryId])
    REFERENCES [dbo].[RegisterEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RegisterEntryGuestParticipant'
CREATE INDEX [IX_FK_RegisterEntryGuestParticipant]
ON [dbo].[GuestParticipants]
    ([RegisterEntryId]);
GO

-- Creating foreign key on [LocalHelpId] in table 'RegisterEntries'
ALTER TABLE [dbo].[RegisterEntries]
ADD CONSTRAINT [FK_LocalHelpRegisterEntry]
    FOREIGN KEY ([LocalHelpId])
    REFERENCES [dbo].[LocalHelps]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocalHelpRegisterEntry'
CREATE INDEX [IX_FK_LocalHelpRegisterEntry]
ON [dbo].[RegisterEntries]
    ([LocalHelpId]);
GO

-- Creating foreign key on [OrganizationId] in table 'LocalHelps'
ALTER TABLE [dbo].[LocalHelps]
ADD CONSTRAINT [FK_OrganizationLocalHelp]
    FOREIGN KEY ([OrganizationId])
    REFERENCES [dbo].[Organizations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrganizationLocalHelp'
CREATE INDEX [IX_FK_OrganizationLocalHelp]
ON [dbo].[LocalHelps]
    ([OrganizationId]);
GO

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