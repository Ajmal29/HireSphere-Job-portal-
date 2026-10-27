-- ==========================================================
-- 1. DATABASE CREATION
-- ==========================================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MVCProjectJobSearch')
BEGIN
    CREATE DATABASE [MVCProjectJobSearch];
END
GO

USE [MVCProjectJobSearch]
GO

-- ==========================================================
-- 2. TABLES CREATION
-- ==========================================================

-- 1. Company Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Company')
BEGIN
    CREATE TABLE [dbo].[Company] (
        [CompanyId] INT IDENTITY(1,1) PRIMARY KEY,
        [CompanyName] VARCHAR(200) NOT NULL,
        [Email] VARCHAR(250) NOT NULL,
        [Phone] VARCHAR(50) NOT NULL,
        [Address] VARCHAR(200) NOT NULL
    );
END
GO

-- 2. User / Employee Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserTable')
BEGIN
    CREATE TABLE [dbo].[UserTable] (
        [UserId] INT IDENTITY(1,1) PRIMARY KEY,
        [UserName] VARCHAR(50) NOT NULL,
        [Age] INT NULL,
        [Phone] VARCHAR(50) NULL,
        [Email] VARCHAR(100) NULL,
        [Address] VARCHAR(200) NULL,
        [Qualification] VARCHAR(100) NULL,
        [ExperienceYears] INT NULL,
        [Skills] VARCHAR(100) NULL,
        [Photo] VARCHAR(250) NULL,
        [Status] INT NULL
    );
END
GO

-- 3. Multi-User Login Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoginTable')
BEGIN
    CREATE TABLE [dbo].[LoginTable] (
        [LoginId] INT IDENTITY(1,1) PRIMARY KEY,
        [Email] VARCHAR(50) NOT NULL,
        [PasswordHash] VARCHAR(50) NOT NULL,
        [UserType] VARCHAR(50) NOT NULL,
        [CompanyId] INT NULL,
        [UserId] INT NULL,
        [IsActive] BIT DEFAULT 1
    );
END
GO

-- 4. Job Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Job')
BEGIN
    CREATE TABLE [dbo].[Job] (
        [JobId] INT IDENTITY(1,1) PRIMARY KEY,
        [CompanyId] INT NULL,
        [JobTitle] VARCHAR(100) NOT NULL,
        [ExperienceYears] INT NULL,
        [Skills] VARCHAR(100) NULL,
        [Qualification] VARCHAR(100) NULL,
        [Location] VARCHAR(100) NULL,
        [StartDate] DATE NULL,
        [EndDate] DATE NULL,
        [JobStatus] VARCHAR(50) NULL,
        CONSTRAINT FK_Job_Company FOREIGN KEY (CompanyId) REFERENCES [dbo].[Company](CompanyId)
    );
END
GO

-- 5. Job Application Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'JobApplication')
BEGIN
    CREATE TABLE [dbo].[JobApplication] (
        [JobApplicationId] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NULL,
        [JobId] INT NULL,
        [ApplicationDate] DATE DEFAULT GETDATE(),
        [Resume] VARCHAR(250) NULL,
        [Status] INT DEFAULT 1,
        CONSTRAINT FK_JobApp_User FOREIGN KEY (UserId) REFERENCES [dbo].[UserTable](UserId),
        CONSTRAINT FK_JobApp_Job FOREIGN KEY (JobId) REFERENCES [dbo].[Job](JobId)
    );
END
GO
