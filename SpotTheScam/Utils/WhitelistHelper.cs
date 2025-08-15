using System;
using System.Data.SqlClient;

namespace SpotTheScam.Utils
{
    public static class WhitelistHelper
    {
        // Overload A: pass a Bank AccountId (helper resolves the Primary user + their groups)
        public static bool IsWhitelistedRecipientForAccount(
            SqlConnection con, SqlTransaction tx,
            int accountId, string recipientName, string recipientAccountNumber)
        {
            int primaryUserId;
            using (var cmd = new SqlCommand("SELECT UserId FROM dbo.BankAccounts WHERE AccountId=@a", con, tx))
            {
                cmd.Parameters.AddWithValue("@a", accountId);
                var r = cmd.ExecuteScalar();
                if (r == null) return false; // unknown account -> treat as not whitelisted
                primaryUserId = Convert.ToInt32(r);
            }
            return IsWhitelistedRecipient(con, tx, primaryUserId, recipientName, recipientAccountNumber);
        }

        // Overload B: pass the Primary user id directly
        public static bool IsWhitelistedRecipient(
            SqlConnection con, SqlTransaction tx,
            int primaryUserId, string recipientName, string recipientAccountNumber)
        {
            string normName = (recipientName ?? "").Trim();
            string normAcct = (recipientAccountNumber ?? "").Replace(" ", "").Replace("-", "").Trim();

            // Match by:
            //  - User-specific whitelist (UserId)
            //  - Any active group the user belongs to (GroupId IN (...))
            //  - Either exact account number (spaces/dashes ignored) OR exact name (case-insensitive)
            using (var cmd = new SqlCommand(@"
SELECT TOP 1 1
FROM dbo.FamilyGroupMemberWhitelistedRecipients w
WHERE
(
    w.UserId = @uid
    OR w.GroupId IN (SELECT GroupId FROM dbo.FamilyGroupMembers WHERE UserId=@uid AND Status='Active')
)
AND
(
    (w.RecipientAccountNumber IS NOT NULL AND
     REPLACE(REPLACE(LTRIM(RTRIM(w.RecipientAccountNumber)),' ',''),'-','') = @acct)
 OR (w.RecipientName IS NOT NULL AND
     UPPER(LTRIM(RTRIM(w.RecipientName))) = UPPER(@name))
);", con, tx))
            {
                cmd.Parameters.AddWithValue("@uid", primaryUserId);
                cmd.Parameters.AddWithValue("@acct", (object)(normAcct ?? (object)DBNull.Value) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@name", (object)(normName ?? (object)DBNull.Value) ?? DBNull.Value);
                var hit = cmd.ExecuteScalar();
                return hit != null;
            }
        }
    }
}
