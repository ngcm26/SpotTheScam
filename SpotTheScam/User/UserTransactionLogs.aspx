<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserTransactionLogs.aspx.cs" Inherits="SpotTheScam.User.UserTransactionLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Transaction Logs</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #f3f4f6;
        }

        .container {
            max-width: 1200px;
        }

        .card {
            background-color: #ffffff;
            border-radius: 0.75rem;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }

        .btn-primary {
            background-color: #4f46e5;
            color: #ffffff;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            transition: background-color 0.3s ease;
        }

        .btn-primary:hover {
            background-color: #4338ca;
        }

        .btn-secondary {
            background-color: #6b7280;
            color: #ffffff;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            transition: background-color 0.3s ease;
        }

        .btn-secondary:hover {
            background-color: #4b5563;
        }

        .alert {
            padding: 1rem;
            border-radius: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.75rem;
        }

        .alert-success {
            background-color: #d1fae5;
            color: #065f46;
        }

        .alert-error {
            background-color: #fee2e2;
            color: #991b1b;
        }

        .alert-icon {
            font-size: 1.25rem;
        }

        .form-input {
            padding: 0.75rem;
            border: 1px solid #d1d5db;
            border-radius: 0.5rem;
            width: 100%;
        }

        .gv-header th {
            background-color: #e5e7eb;
            padding: 0.75rem;
            text-align: left;
            font-weight: 600;
        }

        .gv-row td {
            padding: 0.75rem;
            border-bottom: 1px solid #e5e7eb;
        }

        .gv-row:nth-child(even) {
            background-color: #f9fafb;
        }

        .gv-actions a {
            color: #4f46e5;
            text-decoration: none;
            margin-right: 0.5rem;
        }

        .gv-actions a:hover {
            text-decoration: underline;
        }

        /* Account Carousel Styling */
        .account-card-container {
            display: flex;
            overflow-x: hidden; /* Hide scrollbar */
            scroll-behavior: smooth;
            -webkit-overflow-scrolling: touch;
        }

        .account-card {
            flex: 0 0 100%; /* Each card takes full width */
            padding: 1.5rem;
            border-radius: 0.75rem;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); /* Example gradient */
            color: white;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            margin-right: 1rem; /* Space between cards */
        }

        .account-card:last-child {
            margin-right: 0;
        }

        .account-card h3 {
            font-size: 1.5rem;
            font-weight: bold;
            margin-bottom: 0.5rem;
        }

        .account-card p {
            font-size: 1rem;
            margin-bottom: 0.25rem;
        }

        .account-card .balance {
            font-size: 2rem;
            font-weight: bold;
            margin-top: 1rem;
        }

        .pagination-dots {
            display: flex;
            justify-content: center;
            margin-top: 1rem;
        }

        .pagination-dot {
            height: 10px;
            width: 10px;
            background-color: #d1d5db;
            border-radius: 50%;
            display: inline-block;
            margin: 0 5px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .pagination-dot.active {
            background-color: #4f46e5;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto p-4">
        <h1 class="text-3xl font-bold text-gray-800 mb-6">Your Transaction Logs</h1>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-6">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <%-- Account Overview Section --%>
        <div class="card p-6 mb-6">
            <div class="flex justify-between items-center mb-4">
                <h2 class="text-2xl font-semibold text-gray-700">Account Overview</h2>
                <asp:LinkButton ID="lnkViewAllAccounts" runat="server" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="text-indigo-600 hover:underline">View All Accounts</asp:LinkButton>
            </div>

            <asp:Panel ID="pnlAccountOverview" runat="server" Visible="false">
                <div class="mb-4">
                    <p class="text-gray-600 text-lg">Total Balance Across All Accounts:</p>
                    <p class="text-4xl font-bold text-indigo-700"><asp:Literal ID="ltTotalBalance" runat="server"></asp:Literal></p>
                </div>

                <div class="relative flex items-center">
                    <asp:LinkButton ID="btnPrevAccount" runat="server" OnClick="btnPrevAccount_Click" CssClass="absolute left-0 z-10 p-2 rounded-full bg-gray-200 hover:bg-gray-300 focus:outline-none -ml-4">
                        <i class="fas fa-chevron-left text-gray-700"></i>
                    </asp:LinkButton>

                    <asp:Panel ID="pnlAccountCards" runat="server" CssClass="account-card-container flex-grow">
                        <asp:Repeater ID="rptAccounts" runat="server">
                            <ItemTemplate>
                                <div class="account-card flex-shrink-0 w-full">
                                    <h3 class="font-semibold text-white"><%# Eval("AccountNickname") %></h3>
                                    <p class="text-sm text-gray-200"><%# Eval("BankName") %> - <%# Eval("AccountType") %></p>
                                    <p class="text-sm text-gray-200">Account No: <%# Eval("AccountNumber") %></p>
                                    <p class="balance text-white"><%# Eval("Balance", "{0:C2}") %></p>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>

                    <asp:LinkButton ID="btnNextAccount" runat="server" OnClick="btnNextAccount_Click" CssClass="absolute right-0 z-10 p-2 rounded-full bg-gray-200 hover:bg-gray-300 focus:outline-none -mr-4">
                        <i class="fas fa-chevron-right text-gray-700"></i>
                    </asp:LinkButton>
                </div>
                <div id="accountDots" runat="server" class="pagination-dots"></div>
            </asp:Panel>

            <asp:Panel ID="pnlNoAccounts" runat="server" Visible="false" CssClass="text-center py-8">
                <p class="text-gray-500 text-lg mb-4">You haven't linked any bank accounts yet.</p>
                <asp:Button ID="btnAddFirstAccount" runat="server" Text="Add Your First Account" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="btn-primary" />
            </asp:Panel>
        </div>

        <%-- Transaction Filters --%>
        <div class="card p-6 mb-6">
            <h2 class="text-2xl font-semibold text-gray-700 mb-4">Filter Transactions</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-4">
                <div>
                    <label for="<%=txtStartDate.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Start Date:</label>
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-input"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=txtEndDate.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">End Date:</label>
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-input"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=ddlTransactionType.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Transaction Type:</label>
                    <asp:DropDownList ID="ddlTransactionType" runat="server" CssClass="form-input">
                        <asp:ListItem Text="All Types" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Credit" Value="Credit"></asp:ListItem>
                        <asp:ListItem Text="Debit" Value="Debit"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="lg:col-span-2">
                    <label for="<%=txtSearch.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Search Description/Recipient:</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-input" placeholder="e.g., Groceries, John Doe"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=ddlAccount.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Select Account:</label>
                    <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-input"></asp:DropDownList>
                </div>
            </div>
            <div class="flex flex-wrap gap-4 justify-end">
                <asp:Button ID="btnApplyFilters" runat="server" Text="Apply Filters" OnClick="btnApplyFilters_Click" CssClass="btn-primary" />
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" OnClick="btnClearFilters_Click" CssClass="btn-secondary" />
                <asp:Button ID="btnExportCsv" runat="server" Text="Export to CSV" OnClick="btnExportCsv_Click" CssClass="btn-secondary" />
            </div>
        </div>

        <%-- Transactions Grid --%>
        <h2 class="text-2xl font-semibold text-gray-700 mb-4">All Transactions</h2>
        <div class="card p-6 overflow-x-auto">
            <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvTransactions_PageIndexChanging"
                CssClass="min-w-full divide-y divide-gray-200" HeaderStyle-CssClass="gv-header" RowStyle-CssClass="gv-row">
                <Columns>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="TransactionTime" HeaderText="Time" /> <%-- MODIFIED LINE --%>
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:BoundField DataField="TransactionType" HeaderText="Type" />
                    <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C2}" />
                    <asp:BoundField DataField="SenderRecipient" HeaderText="Sender/Recipient" />
                    <asp:BoundField DataField="BalanceAfterTransaction" HeaderText="Balance After" DataFormatString="{0:C2}" />
                    <asp:BoundField DataField="BankName" HeaderText="Bank" />
                    <asp:BoundField DataField="AccountNickname" HeaderText="Account" />
                </Columns>
                <EmptyDataTemplate>
                    <p class="text-gray-500 text-center py-4">No transactions found for the selected filters.</p>
                </EmptyDataTemplate>
                <PagerStyle CssClass="pagination-style" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
