<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SpotTheScam.User.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dashboard-card {
            border-radius: 18px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.07);
            padding: 24px;
            background: #fff;
            margin-bottom: 24px;
        }
        .dashboard-highlight {
            font-size: 1.8rem;
            font-weight: 700;
        }
        .status-chip {
            padding: 0.3em 0.8em;
            border-radius: 20px;
            font-size: 0.9em;
            margin-right: 0.5em;
        }
        .status-flagged { background: #fff3cd; color: #d39e00; }
        .status-approved { background: #d4edda; color: #155724; }
        .status-pending { background: #f8d7da; color: #721c24; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="dashboard-card mb-4">
        <h2>Welcome, <%= Context.User.Identity.Name %>!</h2>
        <p>Role: <strong><%= Session["UserRole"] %></strong></p>
    </div>

    <div class="row">
        <div class="col-md-6">
            <div class="dashboard-card">
                <h4>Total Balance</h4>
                <div class="dashboard-highlight">
                    S$ <asp:Label ID="lblTotalBalance" runat="server" />
                </div>
            </div>

            <div class="dashboard-card">
                <h4>Recent Transactions</h4>
                <asp:Repeater ID="rptRecentTransactions" runat="server">
                    <HeaderTemplate>
                        <table class="table table-sm">
                            <thead>
                                <tr><th>Date</th><th>Description</th><th>Amount</th><th>Status</th></tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("TransactionDate", "{0:dd MMM yyyy}") %></td>
                            <td><%# Eval("Description") %></td>
                            <td>S$<%# Eval("Amount", "{0:N2}") %></td>
                            <td>
                                <%# Convert.ToBoolean(Eval("IsFlagged")) ?
                                    $"<span class='status-chip status-flagged'>Flagged: {Eval("Severity")}</span>" :
                                    "<span class='status-chip status-approved'>Normal</span>" %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody></table>
                    </FooterTemplate>
                </asp:Repeater>
                <a href="UserTransactionLogs.aspx" class="btn btn-link mt-2">View All Transactions</a>
            </div>
        </div>

        <div class="col-md-6">
            <div class="dashboard-card">
                <h4>
                    <i class="fas fa-bell"></i>
                    Alerts & Approvals
                    <asp:Label ID="lblAlertCount" runat="server" CssClass="badge badge-danger ml-2" />
                </h4>
                <asp:Repeater ID="rptAlerts" runat="server">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span>
                                <strong><%# Eval("AlertType") %></strong><br/>
                                <%# Eval("AlertMessage") %>
                                <small class="text-muted d-block">Date: <%# Eval("AlertDate", "{0:dd MMM HH:mm}") %></small>
                            </span>
                            <a href='TransactionDetails.aspx?id=<%# Eval("TransactionId") %>' class='btn btn-sm btn-primary ml-2'>Review</a>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
                <a href="AlertsInbox.aspx" class="btn btn-link mt-2">View All Alerts</a>
            </div>

            <div class="dashboard-card">
                <h4>Quick Actions</h4>
                <a href="AddManualTransaction.aspx" class="btn btn-success mr-2">Add Transaction</a>
                <a href="FamilyGroupManagement.aspx" class="btn btn-info">Manage Family Group</a>
            </div>
        </div>
    </div>
</asp:Content>
