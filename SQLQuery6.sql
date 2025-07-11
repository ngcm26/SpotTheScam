SELECT * FROM WebinarSessions

SELECT Id, Username, Email, Points FROM Users

-- First, let's see what's in WebinarSessions
SELECT COUNT(*) as SessionCount FROM WebinarSessions;

-- Add sample sessions if none exist
IF NOT EXISTS (SELECT * FROM WebinarSessions WHERE SessionId = 1)
BEGIN
    -- Make sure we have experts first
    IF NOT EXISTS (SELECT * FROM Experts)
    BEGIN
        INSERT INTO Experts (ExpertName, ExpertTitle, ExpertImage, Bio, Specialization, IsActive)
        VALUES 
            ('Dr Harvey Blue', 'Cybersecurity Specialist, 15+ years experience', '/Images/expert2.jpg', 
             'Experienced cybersecurity specialist', 'Cybersecurity', 1);
    END

    -- Now add sessions
    INSERT INTO WebinarSessions (Title, Description, SessionDate, StartTime, EndTime, MaxParticipants, PointsRequired, SessionType, ExpertId, CreatedBy)
    VALUES 
        ('Protecting Your Online Banking', 
         'Learn essential security practices for online banking', 
         '2025-08-15', '14:00', '15:00', 100, 0, 'Free', 1, 1);
END

SELECT 'Data added successfully' as Result;