-- Check if the foreign key references are correct
SELECT 
    ws.SessionId,
    ws.Title,
    e.ExpertName,
    ws.ExpertId
FROM WebinarSessions ws
LEFT JOIN Experts e ON ws.ExpertId = e.ExpertId
WHERE ws.SessionId = 1