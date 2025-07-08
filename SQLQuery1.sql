
-- 9. USER QUIZ SESSIONS TABLE
CREATE TABLE UserQuizSessions (
    SessionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    QuizId INT NOT NULL,
    StartTime DATETIME DEFAULT GETDATE(),
    EndTime DATETIME,
    TotalQuestions INT NOT NULL,
    CorrectAnswers INT DEFAULT 0,
    TotalPointsEarned INT DEFAULT 0,
    SessionStatus NVARCHAR(20) DEFAULT 'In Progress',
    CONSTRAINT FK_UserQuizSessions_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserQuizSessions_Quizzes FOREIGN KEY (QuizId) REFERENCES Quizzes(QuizId)
);

-- 10. USER QUIZ ANSWERS TABLE
CREATE TABLE UserQuizAnswers (
    AnswerId INT IDENTITY(1,1) PRIMARY KEY,
    SessionId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedOption NVARCHAR(1),
    IsCorrect BIT DEFAULT 0,
    HintUsed BIT DEFAULT 0,
    PointsEarned INT DEFAULT 0,
    TimeSpent INT,
    AnswerTime DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserQuizAnswers_Sessions FOREIGN KEY (SessionId) REFERENCES UserQuizSessions(SessionId),
    CONSTRAINT FK_UserQuizAnswers_Questions FOREIGN KEY (QuestionId) REFERENCES QuizQuestions(QuestionId)
);

-- 11. POINTS TRANSACTIONS TABLE
CREATE TABLE PointsTransactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TransactionType NVARCHAR(20) NOT NULL,
    Points INT NOT NULL,
    Description NVARCHAR(200),
    SessionId INT NULL,
    BookingId INT NULL,
    TransactionDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_PointsTransactions_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_PointsTransactions_Sessions FOREIGN KEY (SessionId) REFERENCES UserQuizSessions(SessionId),
    CONSTRAINT FK_PointsTransactions_Bookings FOREIGN KEY (BookingId) REFERENCES VideoCallBookings(BookingId)
);

-- 12. POINTS STORE ITEMS TABLE
CREATE TABLE PointsStoreItems (
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    PointsCost INT NOT NULL,
    ItemType NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- 13. USER STORE PURCHASES TABLE
CREATE TABLE UserStorePurchases (
    PurchaseId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ItemId INT NOT NULL,
    PointsSpent INT NOT NULL,
    PurchaseDate DATETIME DEFAULT GETDATE(),
    DeliveryStatus NVARCHAR(20) DEFAULT 'Delivered',
    DownloadLink NVARCHAR(255),
    CONSTRAINT FK_UserStorePurchases_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserStorePurchases_Items FOREIGN KEY (ItemId) REFERENCES PointsStoreItems(ItemId)
);

-- 14. ACHIEVEMENTS TABLE
CREATE TABLE Achievements (
    AchievementId INT IDENTITY(1,1) PRIMARY KEY,
    AchievementName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    RequirementType NVARCHAR(50) NOT NULL,
    RequirementValue INT NOT NULL,
    BonusPoints INT DEFAULT 0,
    BadgeColor NVARCHAR(20),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- 15. USER ACHIEVEMENTS TABLE
CREATE TABLE UserAchievements (
    UserAchievementId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    AchievementId INT NOT NULL,
    UnlockedDate DATETIME DEFAULT GETDATE(),
    Progress INT DEFAULT 0,
    IsCompleted BIT DEFAULT 0,
    CONSTRAINT FK_UserAchievements_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserAchievements_Achievements FOREIGN KEY (AchievementId) REFERENCES Achievements(AchievementId)
);

-- 16. USER STATS TABLE
CREATE TABLE UserStats (
    StatsId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TotalPoints INT DEFAULT 0,
    QuizzesDone INT DEFAULT 0,
    AverageScore DECIMAL(5,2) DEFAULT 0,
    CurrentStreak INT DEFAULT 0,
    LongestStreak INT DEFAULT 0,
    LastQuizDate DATE,
    TotalTimeSpent INT DEFAULT 0,
    LastUpdated DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserStats_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);