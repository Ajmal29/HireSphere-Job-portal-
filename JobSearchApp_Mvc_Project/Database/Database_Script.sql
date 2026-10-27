USE [MVCProjectJobSearch]
GO

-- ==========================================================
-- 1. TABLES CREATION (Matches Stored Procedures 100%)
-- ==========================================================

-- Table 1: Company Table
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

-- Table 2: User / Employee Table
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

-- Table 3: Multi-User Login Table
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

-- Table 4: Job Table
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

-- Table 5: Job Application Table
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


-- ==========================================================
-- 2. YOUR EXACT STORED PROCEDURES (NO MODIFICATIONS)
-- ==========================================================

-- 1. Company Registration Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Companyreg]
    @cna VARCHAR(200),
    @caddr VARCHAR(200),
    @cphone VARCHAR(50),
    @cemail VARCHAR(250)
AS
BEGIN
    INSERT INTO Company (CompanyName, Email, Phone, Address)
    VALUES (@cna, @cemail, @cphone, @caddr)
END
GO

-- 2. User Registration Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Userreg]
    @uid INT,
    @una VARCHAR(50),
    @uag INT,
    @uem VARCHAR(100),
    @uaddr VARCHAR(200),
    @uph VARCHAR(50),
    @upho VARCHAR(250),
    @uqual VARCHAR(100),
    @uexp INT,
    @usk VARCHAR(100),
    @usta VARCHAR(50)
AS
BEGIN
    INSERT INTO UserTable (UserName, Age, Phone, Email, Address, Qualification, ExperienceYears, Skills, Photo, Status)
    VALUES (@una, @uag, @uph, @uem, @uaddr, @uqual, @uexp, @usk, @upho, TRY_CONVERT(INT, @usta))
END
GO

-- 3. Login Insert Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Loginsert]
    @rid INT,
    @una VARCHAR(50),
    @pw VARCHAR(50),
    @utype VARCHAR(50)
AS
BEGIN
    INSERT INTO LoginTable (Email, PasswordHash, UserType, CompanyId, UserId, IsActive)
    VALUES (
        @una, 
        @pw, 
        @utype, 
        CASE WHEN @utype = 'Employer' THEN @rid ELSE NULL END, 
        CASE WHEN @utype <> 'Employer' THEN @rid ELSE NULL END, 
        1
    )
END
GO

-- 4. Get Max ID Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Maxidlogin]
AS
BEGIN
    SELECT ISNULL(MAX(LoginId), 0) FROM LoginTable
END
GO

-- 5. Login Count Check Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Logincountid]
    @una VARCHAR(50),
    @pwd VARCHAR(50)
AS
BEGIN
    SELECT COUNT(LoginId) FROM LoginTable WHERE Email = @una AND PasswordHash = @pwd AND IsActive = 1
END
GO

-- 6. Get Reg_Id / LoginId on Login
CREATE OR ALTER PROCEDURE [dbo].[sp_Loginid]
    @una VARCHAR(50),
    @pwd VARCHAR(50)
AS
BEGIN
    SELECT ISNULL(CompanyId, UserId) FROM LoginTable WHERE Email = @una AND PasswordHash = @pwd AND IsActive = 1
END
GO

-- 7. Get User Type on Login
CREATE OR ALTER PROCEDURE [dbo].[sp_Logtype]
    @una VARCHAR(50),
    @pwd VARCHAR(50)
AS
BEGIN
    SELECT UserType FROM LoginTable WHERE Email = @una AND PasswordHash = @pwd AND IsActive = 1
END
GO

-- 8. Job Insert Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Jobinsert]
    @cid INT,
    @jbt VARCHAR(100),
    @jbexp INT,
    @jskill VARCHAR(100),
    @jqual VARCHAR(100),
    @jloc VARCHAR(100),
    @sdate DATE,
    @edate DATE,
    @jsta VARCHAR(50)
AS
BEGIN
    INSERT INTO Job (CompanyId, JobTitle, ExperienceYears, Skills, Qualification, Location, StartDate, EndDate, JobStatus)
    VALUES (@cid, @jbt, @jbexp, @jskill, @jqual, @jloc, @sdate, @edate, @jsta)
END
GO

-- 9. Select All Jobs Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Selectall]
AS
BEGIN
    SELECT * FROM Job
END
GO

-- 10. Dynamic Job Search Procedure (Excludes Expired / Closed Jobs)
CREATE OR ALTER PROCEDURE [dbo].[sp_Jobsearches]
    @qry NVARCHAR(MAX) = NULL
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX)
    SET @SQL = 'SELECT JobId AS Job_id, CompanyId AS Company_id, JobTitle AS Job_Tittle, JobTitle AS Job_description, Qualification AS Job_Type, ExperienceYears AS Job_Experience, Skills AS Job_Skills, ''Competitive'' AS Job_Salary, EndDate AS Job_enddate, Location AS Job_Location, JobStatus AS Job_Status, * 
                FROM Job 
                WHERE (JobStatus IS NULL OR (JobStatus <> ''Expired'' AND JobStatus <> ''Closed''))
                  AND (EndDate IS NULL OR CAST(EndDate AS DATE) >= CAST(GETDATE() AS DATE)) ' + ISNULL(@qry, '')
    EXEC sp_executesql @SQL
END
GO

-- 11. Count User Applied Job With ID
CREATE OR ALTER PROCEDURE [dbo].[SP_CountUserAppliedJobWithID]
    @userid INT,
    @Jobid INT,
    @status INT OUTPUT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [dbo].[JobApplication] WHERE [UserId] = @userid AND [JobId] = @Jobid)
    BEGIN
        SET @status = 1
    END
    ELSE
    BEGIN
        SET @status = 0
    END
END
GO

-- 12. Get Single Job Details by Job ID
CREATE OR ALTER PROCEDURE [dbo].[SP_GetJobDatawithID]
    @jobid INT
AS
BEGIN
    SELECT JobId AS Job_id, CompanyId AS Company_id, JobTitle AS Job_Tittle, JobTitle AS Job_description, Qualification AS Job_Type, ExperienceYears AS Job_Experience, Skills AS Job_Skills, 'Competitive' AS Job_Salary, EndDate AS Job_enddate, Location AS Job_Location, JobStatus AS Job_Status, *
    FROM [dbo].[Job]
    WHERE [JobId] = @jobid
END
GO

-- 13. Post / Submit Job Application
CREATE OR ALTER PROCEDURE [dbo].[SP_PostApplication]
    @User_id INT,
    @Company_id INT,
    @Job_Id INT,
    @Resume NVARCHAR(MAX),
    @App_Date DATE,
    @App_Status NVARCHAR(30)
AS
BEGIN
    INSERT INTO [dbo].[JobApplication] ([UserId], [JobId], [ApplicationDate], [Resume], [Status])
    VALUES (@User_id, @Job_Id, @App_Date, @Resume, 1)
END
GO

-- 14. Get User Profile by UserId
CREATE OR ALTER PROCEDURE [dbo].[sp_profile]
    @id INT
AS
BEGIN
    SELECT 
        UserId,
        UserName AS [Name],
        Age,
        Address,
        Email,
        Photo,
        Phone,
        Qualification,
        ExperienceYears,
        Skills
    FROM [dbo].[UserTable]
    WHERE UserId = @id
END
GO

-- 15. Update User Profile
CREATE OR ALTER PROCEDURE [dbo].[sp_profile_Update]
    @id INT,
    @age INT,
    @address VARCHAR(200),
    @photo VARCHAR(250) = NULL
AS
BEGIN
    UPDATE [dbo].[UserTable]
    SET Age = @age,
        Address = @address,
        Photo = CASE WHEN @photo IS NOT NULL AND @photo <> '' THEN @photo ELSE Photo END
    WHERE UserId = @id
END
GO

-- 16. Get Current Password
CREATE OR ALTER PROCEDURE [dbo].[sp_selectPwd]
    @id INT
AS
BEGIN
    SELECT PasswordHash
    FROM [dbo].[LoginTable]
    WHERE (UserId = @id OR LoginId = @id OR CompanyId = @id) AND IsActive = 1
END
GO

-- 17. Change Password
CREATE OR ALTER PROCEDURE [dbo].[sp_pwdchange]
    @id INT,
    @newpwd VARCHAR(50)
AS
BEGIN
    UPDATE [dbo].[LoginTable]
    SET PasswordHash = @newpwd
    WHERE (UserId = @id OR LoginId = @id OR CompanyId = @id)
END
GO

-- 18. Update Expired Jobs (Admin side)
CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateExpiredJobs]
    @curDate DATE = NULL
AS
BEGIN
    DECLARE @today DATE
    IF (@curDate IS NULL)
        SET @today = CAST(GETDATE() AS DATE)
    ELSE
        SET @today = @curDate

    -- Compare EndDate with current date and update JobStatus
    UPDATE [dbo].[Job]
    SET JobStatus = 'Expired'
    WHERE CAST(EndDate AS DATE) < @today
      AND (JobStatus IS NULL OR JobStatus <> 'Expired')
    
    SELECT @@ROWCOUNT AS UpdatedCount
END
GO

-- 19. Admin: Get All Applications With Full Candidate Details
CREATE OR ALTER PROCEDURE [dbo].[sp_AdminGetAllApplications]
AS
BEGIN
    SELECT 
        a.JobApplicationId,
        a.ApplicationDate,
        a.Resume,
        a.Status AS ApplicationStatus,
        u.UserId,
        u.UserName AS CandidateName,
        u.Age AS CandidateAge,
        u.Phone AS CandidatePhone,
        u.Email AS CandidateEmail,
        u.Address AS CandidateAddress,
        u.Qualification AS CandidateQualification,
        u.ExperienceYears AS CandidateExperience,
        u.Skills AS CandidateSkills,
        u.Photo AS CandidatePhoto,
        j.JobId,
        j.JobTitle,
        j.Qualification AS JobType,
        j.ExperienceYears AS JobExperience,
        j.Skills AS JobSkills,
        j.Location AS JobLocation,
        j.StartDate AS JobStartDate,
        j.EndDate AS JobEndDate,
        j.JobStatus,
        c.CompanyId,
        c.CompanyName,
        c.Email AS CompanyEmail,
        c.Phone AS CompanyPhone
    FROM [dbo].[JobApplication] a
    INNER JOIN [dbo].[UserTable] u ON a.UserId = u.UserId
    INNER JOIN [dbo].[Job] j ON a.JobId = j.JobId
    LEFT JOIN [dbo].[Company] c ON j.CompanyId = c.CompanyId
    ORDER BY a.ApplicationDate DESC
END
GO

-- 20. Admin: Get All Jobs
CREATE OR ALTER PROCEDURE [dbo].[sp_AdminGetAllJobs]
AS
BEGIN
    SELECT 
        j.JobId,
        j.CompanyId,
        c.CompanyName,
        j.JobTitle,
        j.ExperienceYears,
        j.Skills,
        j.Qualification,
        j.Location,
        j.StartDate,
        j.EndDate,
        j.JobStatus,
        (SELECT COUNT(*) FROM JobApplication a WHERE a.JobId = j.JobId) AS TotalApplicants
    FROM [dbo].[Job] j
    LEFT JOIN [dbo].[Company] c ON j.CompanyId = c.CompanyId
    ORDER BY j.JobId DESC
END
GO


