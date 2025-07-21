-- Step 1: Check current state
SELECT 'Users Table' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'PointsTransactions Table', COUNT(*) FROM PointsTransactions
UNION ALL
SELECT 'UserQuizSessions Table', COUNT(*) FROM UserQuizSessions;

SELECT Id, Username, Points FROM Users WHERE Username IN ('test', 'john', 'admin');

-- Step 3: Show current PointsTransactions (should be empty)
SELECT * FROM PointsTransactions ORDER BY TransactionDate DESC;

-- Step 4: Initialize PointsTransactions with data from Users table
-- This creates initial transactions for any existing points
INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
SELECT 
    Id as UserId,
    'Initial' as TransactionType,
    Points as Points,
    'Initial points balance' as Description,
    GETDATE() as TransactionDate
FROM Users 
WHERE Points > 0 
AND Id NOT IN (SELECT DISTINCT UserId FROM PointsTransactions WHERE UserId IS NOT NULL);

-- Step 5: Verify the initialization
SELECT 'After Initialization' as Status;
SELECT u.Id, u.Username, u.Points as UsersTablePoints, 
       ISNULL(SUM(pt.Points), 0) as PointsTransactionsTotal
FROM Users u
LEFT JOIN PointsTransactions pt ON u.Id = pt.UserId
GROUP BY u.Id, u.Username, u.Points
ORDER BY u.Id;

-- Step 6: Show recent transactions
SELECT TOP 10 
    pt.UserId, 
    u.Username,
    pt.TransactionType, 
    pt.Points, 
    pt.Description, 
    pt.TransactionDate
FROM PointsTransactions pt
JOIN Users u ON pt.UserId = u.Id
ORDER BY pt.TransactionDate DESC;
