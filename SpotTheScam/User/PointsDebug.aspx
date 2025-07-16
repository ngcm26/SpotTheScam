<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PointsDebug.aspx.cs" Inherits="SpotTheScam.User.PointsDebug" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Points Debug</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .section { margin-bottom: 30px; padding: 15px; border: 1px solid #ccc; border-radius: 5px; }
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Points System Debug Page</h1>
        
        <div class="section">
            <h2>Current Session Info</h2>
            <p><strong>Username:</strong> <asp:Label ID="lblUsername" runat="server" Text="Not logged in" /></p>
            <p><strong>UserID:</strong> <asp:Label ID="lblUserId" runat="server" Text="N/A" /></p>
        </div>

        <div class="section">
            <h2>Database Table Check</h2>
            <asp:Label ID="lblTableCheck" runat="server" />
        </div>

        <div class="section">
            <h2>Points from Users Table</h2>
            <asp:Label ID="lblUsersPoints" runat="server" />
        </div>

        <div class="section">
            <h2>Points from PointsTransactions Table</h2>
            <asp:Label ID="lblTransactionsPoints" runat="server" />
        </div>

        <div class="section">
            <h2>Recent Transactions</h2>
            <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="true" CssClass="table" />
        </div>

        <div class="section">
            <h2>Quiz Sessions</h2>
            <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="true" CssClass="table" />
        </div>

        <div class="section">
            <h2>Test Add Points</h2>
            <asp:Button ID="btnAddTestPoints" runat="server" Text="Add 10 Test Points" OnClick="btnAddTestPoints_Click" />
            <asp:Label ID="lblTestResult" runat="server" />
        </div>

        <div class="section">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh Data" OnClick="Page_Load" />
        </div>
    </form>
</body>
</html>