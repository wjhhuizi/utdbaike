
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/13/2013 01:52:55
-- Generated from EDMX file: E:\courses\Web-Design\asp.mvc\design\UTDBaike\ccbs\ccbs\Models\BangbangDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BangbangDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_BBActivityBBRegisterEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBRegisterEntries] DROP CONSTRAINT [FK_BBActivityBBRegisterEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_BBUserBBActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBActivities] DROP CONSTRAINT [FK_BBUserBBActivity];
GO
IF OBJECT_ID(N'[dbo].[FK_BBUserBBRegisterEntry_BBUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBUserBBRegisterEntry] DROP CONSTRAINT [FK_BBUserBBRegisterEntry_BBUser];
GO
IF OBJECT_ID(N'[dbo].[FK_BBUserBBRegisterEntry_BBRegisterEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBUserBBRegisterEntry] DROP CONSTRAINT [FK_BBUserBBRegisterEntry_BBRegisterEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_BBActivityBBComment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBComments] DROP CONSTRAINT [FK_BBActivityBBComment];
GO
IF OBJECT_ID(N'[dbo].[FK_BBCategoryBBActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBActivities] DROP CONSTRAINT [FK_BBCategoryBBActivity];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationBBUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BBUsers] DROP CONSTRAINT [FK_LocationBBUser];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[BBUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BBUsers];
GO
IF OBJECT_ID(N'[dbo].[BBActivities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BBActivities];
GO
IF OBJECT_ID(N'[dbo].[BBRegisterEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BBRegisterEntries];
GO
IF OBJECT_ID(N'[dbo].[BBComments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BBComments];
GO
IF OBJECT_ID(N'[dbo].[BBCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BBCategories];
GO
IF OBJECT_ID(N'[dbo].[Locations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Locations];
GO
IF OBJECT_ID(N'[dbo].[BBUserBBRegisterEntry]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BBUserBBRegisterEntry];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BBUsers'
CREATE TABLE [dbo].[BBUsers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Gender] int  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Major] nvarchar(max)  NULL,
    [Year] int  NOT NULL,
    [Avatar] nvarchar(max)  NULL,
    [Username] nvarchar(max)  NOT NULL,
    [ComeFrom] nvarchar(max)  NOT NULL,
    [RegTime] datetime  NOT NULL,
    [LocationId] int  NOT NULL,
    [IsActive] bit  NOT NULL
);
GO

-- Creating table 'BBActivities'
CREATE TABLE [dbo].[BBActivities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [BBUserId] int  NOT NULL,
    [PostDate] datetime  NOT NULL,
    [LastUpdate] datetime  NOT NULL,
    [IsDraft] bit  NOT NULL,
    [BBCategoryId] int  NOT NULL
);
GO

-- Creating table 'BBRegisterEntries'
CREATE TABLE [dbo].[BBRegisterEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Time] datetime  NOT NULL,
    [Address] nvarchar(max)  NULL,
    [Contact] nvarchar(max)  NULL,
    [UpCount] int  NOT NULL,
    [LowerCount] int  NOT NULL,
    [GenderLimit] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [BBActivityId] int  NOT NULL
);
GO

-- Creating table 'BBComments'
CREATE TABLE [dbo].[BBComments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Body] nvarchar(max)  NULL,
    [LastUpdate] datetime  NOT NULL,
    [Postby] nvarchar(max)  NOT NULL,
    [BBActivityId] int  NOT NULL
);
GO

-- Creating table 'BBCategories'
CREATE TABLE [dbo].[BBCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CreateDate] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Locations'
CREATE TABLE [dbo].[Locations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Domain] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'BBUserBBRegisterEntry'
CREATE TABLE [dbo].[BBUserBBRegisterEntry] (
    [RegisteredBBUsers_Id] int  NOT NULL,
    [BBRegisterEntries_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'BBUsers'
ALTER TABLE [dbo].[BBUsers]
ADD CONSTRAINT [PK_BBUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BBActivities'
ALTER TABLE [dbo].[BBActivities]
ADD CONSTRAINT [PK_BBActivities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BBRegisterEntries'
ALTER TABLE [dbo].[BBRegisterEntries]
ADD CONSTRAINT [PK_BBRegisterEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BBComments'
ALTER TABLE [dbo].[BBComments]
ADD CONSTRAINT [PK_BBComments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BBCategories'
ALTER TABLE [dbo].[BBCategories]
ADD CONSTRAINT [PK_BBCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [PK_Locations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [RegisteredBBUsers_Id], [BBRegisterEntries_Id] in table 'BBUserBBRegisterEntry'
ALTER TABLE [dbo].[BBUserBBRegisterEntry]
ADD CONSTRAINT [PK_BBUserBBRegisterEntry]
    PRIMARY KEY NONCLUSTERED ([RegisteredBBUsers_Id], [BBRegisterEntries_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [BBActivityId] in table 'BBRegisterEntries'
ALTER TABLE [dbo].[BBRegisterEntries]
ADD CONSTRAINT [FK_BBActivityBBRegisterEntry]
    FOREIGN KEY ([BBActivityId])
    REFERENCES [dbo].[BBActivities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BBActivityBBRegisterEntry'
CREATE INDEX [IX_FK_BBActivityBBRegisterEntry]
ON [dbo].[BBRegisterEntries]
    ([BBActivityId]);
GO

-- Creating foreign key on [BBUserId] in table 'BBActivities'
ALTER TABLE [dbo].[BBActivities]
ADD CONSTRAINT [FK_BBUserBBActivity]
    FOREIGN KEY ([BBUserId])
    REFERENCES [dbo].[BBUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BBUserBBActivity'
CREATE INDEX [IX_FK_BBUserBBActivity]
ON [dbo].[BBActivities]
    ([BBUserId]);
GO

-- Creating foreign key on [RegisteredBBUsers_Id] in table 'BBUserBBRegisterEntry'
ALTER TABLE [dbo].[BBUserBBRegisterEntry]
ADD CONSTRAINT [FK_BBUserBBRegisterEntry_BBUser]
    FOREIGN KEY ([RegisteredBBUsers_Id])
    REFERENCES [dbo].[BBUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [BBRegisterEntries_Id] in table 'BBUserBBRegisterEntry'
ALTER TABLE [dbo].[BBUserBBRegisterEntry]
ADD CONSTRAINT [FK_BBUserBBRegisterEntry_BBRegisterEntry]
    FOREIGN KEY ([BBRegisterEntries_Id])
    REFERENCES [dbo].[BBRegisterEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BBUserBBRegisterEntry_BBRegisterEntry'
CREATE INDEX [IX_FK_BBUserBBRegisterEntry_BBRegisterEntry]
ON [dbo].[BBUserBBRegisterEntry]
    ([BBRegisterEntries_Id]);
GO

-- Creating foreign key on [BBActivityId] in table 'BBComments'
ALTER TABLE [dbo].[BBComments]
ADD CONSTRAINT [FK_BBActivityBBComment]
    FOREIGN KEY ([BBActivityId])
    REFERENCES [dbo].[BBActivities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BBActivityBBComment'
CREATE INDEX [IX_FK_BBActivityBBComment]
ON [dbo].[BBComments]
    ([BBActivityId]);
GO

-- Creating foreign key on [BBCategoryId] in table 'BBActivities'
ALTER TABLE [dbo].[BBActivities]
ADD CONSTRAINT [FK_BBCategoryBBActivity]
    FOREIGN KEY ([BBCategoryId])
    REFERENCES [dbo].[BBCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BBCategoryBBActivity'
CREATE INDEX [IX_FK_BBCategoryBBActivity]
ON [dbo].[BBActivities]
    ([BBCategoryId]);
GO

-- Creating foreign key on [LocationId] in table 'BBUsers'
ALTER TABLE [dbo].[BBUsers]
ADD CONSTRAINT [FK_LocationBBUser]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationBBUser'
CREATE INDEX [IX_FK_LocationBBUser]
ON [dbo].[BBUsers]
    ([LocationId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------