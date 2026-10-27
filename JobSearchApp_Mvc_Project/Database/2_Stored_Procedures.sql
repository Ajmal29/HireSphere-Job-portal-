USE [MVCProjectJobSearch]
GO

-- ==========================================================
-- STORED PROCEDURES (EXACT SAME - NO MODIFICATIONS)
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
    DECLARE @ActualCompanyId INT = NULL
    DECLARE @ActualUserId INT = NULL

    IF (@utype = 'Employer')
    BEGIN
        SELECT @ActualCompanyId = CompanyId FROM Company WHERE CompanyId = @rid
        IF (@ActualCompanyId IS NULL)
            SELECT TOP 1 @ActualCompanyId = CompanyId FROM Company ORDER BY CompanyId DESC
    END
    ELSE
    BEGIN
        SELECT @ActualUserId = UserId FROM UserTable WHERE UserId = @rid
        IF (@ActualUserId IS NULL)
            SELECT TOP 1 @ActualUserId = UserId FROM UserTable ORDER BY UserId DESC
    END

    INSERT INTO LoginTable (Email, PasswordHash, UserType, CompanyId, UserId, IsActive)
    VALUES (
        @una, 
        @pw, 
        @utype, 
        @ActualCompanyId, 
        @ActualUserId, 
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

-- 10. Dynamic Job Search Procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_Jobsearches]
    @qry NVARCHAR(MAX) = NULL
AS
BEGIN
    IF @qry IS NOT NULL
    BEGIN
        SET @qry = REPLACE(@qry, 'Job_Location', 'Location')
        SET @qry = REPLACE(@qry, 'Job_Skills', 'Skills')
        SET @qry = REPLACE(@qry, 'Job_Experience', 'ExperienceYears')
        SET @qry = REPLACE(@qry, 'Job_Tittle', 'JobTitle')
        SET @qry = REPLACE(@qry, 'Job_id', 'JobId')
        SET @qry = REPLACE(@qry, 'Company_id', 'CompanyId')
    END

    DECLARE @SQL NVARCHAR(MAX)
    SET @SQL = 'SELECT JobId AS Job_id, CompanyId AS Company_id, JobTitle AS Job_Tittle, JobTitle AS Job_description, Qualification AS Job_Type, ExperienceYears AS Job_Experience, Skills AS Job_Skills, ''Competitive'' AS Job_Salary, EndDate AS Job_enddate, Location AS Job_Location, JobStatus AS Job_Status, * FROM Job WHERE (JobStatus IS NULL OR (JobStatus <> ''Expired'' AND JobStatus <> ''Closed'')) AND (EndDate IS NULL OR CAST(EndDate AS DATE) >= CAST(GETDATE() AS DATE)) ' + ISNULL(@qry, '')
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
    DECLARE @ValidUserId INT = @userid

    IF NOT EXISTS (SELECT 1 FROM UserTable WHERE UserId = @ValidUserId)
    BEGIN
        SELECT @ValidUserId = UserId FROM LoginTable WHERE (UserId = @userid OR LoginId = @userid) AND UserId IN (SELECT UserId FROM UserTable)
        IF (@ValidUserId IS NULL)
            SET @ValidUserId = @userid
    END

    IF EXISTS (SELECT 1 FROM [dbo].[JobApplication] WHERE [UserId] = @ValidUserId AND [JobId] = @Jobid)
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
    DECLARE @ValidUserId INT = @User_id

    IF NOT EXISTS (SELECT 1 FROM UserTable WHERE UserId = @ValidUserId)
    BEGIN
        SELECT @ValidUserId = UserId FROM LoginTable WHERE (UserId = @User_id OR LoginId = @User_id) AND UserId IN (SELECT UserId FROM UserTable)
        IF (@ValidUserId IS NULL OR NOT EXISTS (SELECT 1 FROM UserTable WHERE UserId = @ValidUserId))
        BEGIN
            SELECT TOP 1 @ValidUserId = UserId FROM UserTable ORDER BY UserId DESC
        END
    END

    INSERT INTO [dbo].[JobApplication] ([UserId], [JobId], [ApplicationDate], [Resume], [Status])
    VALUES (@ValidUserId, @Job_Id, @App_Date, @Resume, 1)
END
GO


-- 14. Get User Profile by UserId
CREATE OR ALTER PROCEDURE sp_GetUserProfile
    @uid INT
AS
BEGIN
    SELECT * FROM UserTable WHERE UserId = @uid
END
GO

-- 15. Update User Profile
CREATE OR ALTER PROCEDURE sp_UpdateUserProfile
    @uid INT,
    @una VARCHAR(50),
    @uag INT,
    @uph VARCHAR(50),
    @uem VARCHAR(100),
    @uaddr VARCHAR(200),
    @uqual VARCHAR(100),
    @uexp INT,
    @usk VARCHAR(100),
    @upho VARCHAR(250)
AS
BEGIN
    UPDATE UserTable
    SET UserName = @una,
        Age = @uag,
        Phone = @uph,
        Email = @uem,
        Address = @uaddr,
        Qualification = @uqual,
        ExperienceYears = @uexp,
        Skills = @usk,
        Photo = CASE WHEN @upho <> '' AND @upho IS NOT NULL THEN @upho ELSE Photo END
    WHERE UserId = @uid
END
GO

-- 16. Admin: Get All Candidates
CREATE OR ALTER PROCEDURE sp_AdminGetAllUsers
AS
BEGIN
    SELECT UserId, UserName, Email, Phone, Qualification, ExperienceYears, Skills, Photo FROM UserTable
END
GO

-- 17. Admin: Get All Companies
CREATE OR ALTER PROCEDURE sp_AdminGetAllCompanies
AS
BEGIN
    SELECT CompanyId, CompanyName, Email, Phone, Address FROM Company
END
GO

-- 18. Admin: Get All Job Applications With Full Candidate Details
CREATE OR ALTER PROCEDURE [dbo].[sp_AdminGetAllApplications]
AS
BEGIN
    SELECT 
        a.JobApplicationId,
        a.ApplicationDate,
        a.Resume,
        ISNULL(a.Status, 1) AS ApplicationStatus,
        ISNULL(a.Status, 1) AS Status,
        ISNULL(u.UserId, 0) AS UserId,
        ISNULL(u.UserName, 'Applicant') AS CandidateName,
        ISNULL(u.Age, 0) AS CandidateAge,
        ISNULL(u.Phone, '') AS CandidatePhone,
        ISNULL(u.Email, '') AS CandidateEmail,
        ISNULL(u.Address, '') AS CandidateAddress,
        ISNULL(u.Qualification, '') AS CandidateQualification,
        ISNULL(u.ExperienceYears, 0) AS CandidateExperience,
        ISNULL(u.Skills, '') AS CandidateSkills,
        ISNULL(u.Photo, '') AS CandidatePhoto,
        ISNULL(j.JobId, 0) AS JobId,
        ISNULL(j.JobTitle, 'Job') AS JobTitle,
        ISNULL(j.Qualification, '') AS JobType,
        ISNULL(j.ExperienceYears, 0) AS JobExperience,
        ISNULL(j.Skills, '') AS JobSkills,
        ISNULL(j.Location, '') AS JobLocation,
        j.StartDate AS JobStartDate,
        j.EndDate AS JobEndDate,
        ISNULL(j.JobStatus, 'Active') AS JobStatus,
        ISNULL(c.CompanyId, 0) AS CompanyId,
        ISNULL(c.CompanyName, 'Company') AS CompanyName,
        ISNULL(c.Email, '') AS CompanyEmail,
        ISNULL(c.Phone, '') AS CompanyPhone
    FROM [dbo].[JobApplication] a
    LEFT JOIN [dbo].[UserTable] u ON a.UserId = u.UserId
    LEFT JOIN [dbo].[Job] j ON a.JobId = j.JobId
    LEFT JOIN [dbo].[Company] c ON j.CompanyId = c.CompanyId
    ORDER BY a.ApplicationDate DESC
END
GO

-- 18B. Admin: Update Expired Jobs by comparing EndDate with Current Date
CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateExpiredJobs]
    @curDate DATE = NULL
AS
BEGIN
    IF @curDate IS NULL
        SET @curDate = CAST(GETDATE() AS DATE)

    UPDATE Job
    SET JobStatus = 'Expired'
    WHERE EndDate IS NOT NULL 
      AND CAST(EndDate AS DATE) < @curDate
      AND (JobStatus IS NULL OR JobStatus <> 'Expired')
    
    SELECT @@ROWCOUNT AS UpdatedCount
END
GO

-- 18C. Admin: Get All Jobs
CREATE OR ALTER PROCEDURE [dbo].[sp_AdminGetAllJobs]
AS
BEGIN
    SELECT 
        j.JobId,
        j.CompanyId,
        ISNULL(c.CompanyName, 'Company') AS CompanyName,
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

-- 19. Get User Profile by UserId (sp_profile)
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

-- 20. Update User Profile (sp_profile_Update)
CREATE OR ALTER PROCEDURE [dbo].[sp_profile_Update]
    @id INT,
    @age INT,
    @address VARCHAR(200)
AS
BEGIN
    UPDATE [dbo].[UserTable]
    SET Age = @age,
        Address = @address
    WHERE UserId = @id
END
GO

-- 21. Get Current Password (sp_selectPwd)
CREATE OR ALTER PROCEDURE [dbo].[sp_selectPwd]
    @id INT,
    @uname VARCHAR(100) = NULL
AS
BEGIN
    IF @uname IS NOT NULL AND @uname <> ''
    BEGIN
        SELECT TOP 1 PasswordHash
        FROM [dbo].[LoginTable]
        WHERE Email = @uname AND IsActive = 1
    END
    ELSE
    BEGIN
        SELECT TOP 1 PasswordHash
        FROM [dbo].[LoginTable]
        WHERE (UserId = @id OR LoginId = @id OR CompanyId = @id) AND IsActive = 1
    END
END
GO

-- 22. Change Password (sp_pwdchange)
CREATE OR ALTER PROCEDURE [dbo].[sp_pwdchange]
    @id INT,
    @newpwd VARCHAR(50),
    @uname VARCHAR(100) = NULL
AS
BEGIN
    IF @uname IS NOT NULL AND @uname <> ''
    BEGIN
        UPDATE [dbo].[LoginTable]
        SET PasswordHash = @newpwd
        WHERE Email = @uname
    END
    ELSE
    BEGIN
        UPDATE [dbo].[LoginTable]
        SET PasswordHash = @newpwd
        WHERE (UserId = @id OR LoginId = @id OR CompanyId = @id)
    END
END
GO


