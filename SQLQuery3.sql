-- Step 1: Add Points column to Users table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Points')
BEGIN
    ALTER TABLE Users ADD Points INT NOT NULL DEFAULT 150;
    PRINT 'Added Points column to Users table with default value 150';
END
ELSE
BEGIN
    PRINT 'Points column already exists';
END

-- Step 2: Create Experts table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Experts')
BEGIN
    CREATE TABLE Experts (
        ExpertId INT IDENTITY(1,1) PRIMARY KEY,
        ExpertName NVARCHAR(100) NOT NULL,
        ExpertTitle NVARCHAR(200) NOT NULL,
        ExpertImage NVARCHAR(500),
        Bio NVARCHAR(MAX),
        Specialization NVARCHAR(200),
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Created Experts table';
END
ELSE
BEGIN
    PRINT 'Experts table already exists';
END

-- Create WebinarSessions table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WebinarSessions')
BEGIN
    CREATE TABLE WebinarSessions (
        SessionId INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        SessionDate DATE NOT NULL,
        StartTime NVARCHAR(20) NOT NULL,
        EndTime NVARCHAR(20) NOT NULL,
        MaxParticipants INT NOT NULL DEFAULT 1,
        PointsRequired INT NOT NULL DEFAULT 0,
        SessionType NVARCHAR(50) NOT NULL,
        ExpertId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT,
        FOREIGN KEY (ExpertId) REFERENCES Experts(ExpertId)
    );
    PRINT 'Created WebinarSessions table';
END
ELSE
BEGIN
    PRINT 'WebinarSessions table already exists';
END

-- Create WebinarRegistrations table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WebinarRegistrations')
BEGIN
    CREATE TABLE WebinarRegistrations (
        RegistrationId INT IDENTITY(1,1) PRIMARY KEY,
        SessionId INT NOT NULL,
        UserId INT NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Phone NVARCHAR(20) NOT NULL,
        SecurityConcerns NVARCHAR(100),
        BankingMethods NVARCHAR(500),
        PointsUsed INT NOT NULL DEFAULT 0,
        RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),
        IsActive BIT NOT NULL DEFAULT 1,
        FOREIGN KEY (SessionId) REFERENCES WebinarSessions(SessionId),
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    PRINT 'Created WebinarRegistrations table';
END
ELSE
BEGIN
    PRINT 'WebinarRegistrations table already exists';
END

-- Step 3: Insert sample data

-- Insert sample experts
INSERT INTO Experts (ExpertName, ExpertTitle, ExpertImage, Bio, Specialization, IsActive)
VALUES 
    ('Dr Harvey Blue', 'Cybersecurity Specialist, 15+ years experience', '/Images/expert2.jpg', 
     'Experienced cybersecurity specialist with focus on financial security.', 
     'Cybersecurity & Online Banking', 1),
    ('Officer James Wilson', 'Investigating phone and romance scams for 10+ years', '/Images/expert3.jpg', 
     'Law enforcement specialist in scam investigation and prevention.', 
     'Phone & Romance Scams', 1),
    ('Maria Rodriguez', 'Digital Safety Educator, Senior Specialist', '/Images/expert1.jpg', 
     'Senior digital safety consultant specializing in personal security strategies.', 
     'Digital Safety & Personal Security', 1);

PRINT 'Added sample experts';

-- Insert sample webinar sessions
INSERT INTO WebinarSessions (Title, Description, SessionDate, StartTime, EndTime, MaxParticipants, PointsRequired, SessionType, ExpertId, CreatedBy)
VALUES 
    ('Protecting Your Online Banking', 
     'Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.',
     '2025-08-15', '14:00', '15:00', 100, 0, 'Free', 1, 1),
    ('Small Group: Latest Phone Scam Tactics', 
     'Intimate session with max 10 participants. Deep dive into current phone scam methods with personalized Q&A time.',
     '2025-08-17', '10:00', '11:30', 10, 50, 'Premium', 2, 1),
    ('VIP One-on-One Safety Consultation', 
     'Private consultation to review your personal digital security, analyze any suspicious communications, and create a personalized safety plan.',
     '2025-08-19', '15:00', '16:00', 1, 100, 'VIP Premium', 3, 1);

PRINT 'Added sample webinar sessions';

-- Display results
SELECT 'Setup completed successfully!' as Message;
SELECT 'Users with points: ' + CAST(COUNT(*) AS VARCHAR) as UserCount FROM Users WHERE Points > 0;
SELECT 'Experts created: ' + CAST(COUNT(*) AS VARCHAR) as ExpertCount FROM Experts;
SELECT 'Sessions available: ' + CAST(COUNT(*) AS VARCHAR) as SessionCount FROM WebinarSessions;