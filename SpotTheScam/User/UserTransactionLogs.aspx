<%@ Page Title="Transaction Logs" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserTransactionLogs.aspx.cs" Inherits="SpotTheScam.User.UserTransactionLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Transaction Logs - SpotTheScam</title>
    <!-- Tailwind CSS CDN -->
    <script src="https://cdn.tailwindcss.com"></script>
    <link href="https://fonts.googleapis.com/css2?family=Archivo+Black&family=DM+Sans:wght@400;700&display=swap" rel="stylesheet">
    <style>
        /* Base styles for the body and container */
        body {
            font-family: 'DM Sans', sans-serif;
            background-color: #F4F6F8; /* Light grey background */
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 1rem;
        }

        /* Header section styling, matching homepage gradient */
        .header-bg {
            background: linear-gradient(90deg, #D36F2D 52%, #FBECC3 100%);
            color: white;
        }

        /* Button primary style, matching homepage orange */
        .btn-primary {
            background-color: #D36F2D;
            color: white;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            font-weight: 700;
            transition: background-color 0.3s ease;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .btn-primary:hover {
            background-color: #C15F22; /* Slightly darker orange on hover */
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }

        /* Button secondary style, matching homepage dark blue */
        .btn-secondary {
            background-color: #051D40;
            color: white;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            font-weight: 700;
            transition: background-color 0.3s ease;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .btn-secondary:hover {
            background-color: #03122A; /* Slightly darker blue on hover */
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }

        /* Input field common styling */
        .input-field {
            border: 1px solid #D1D5DB; /* Light grey border */
            border-radius: 0.5rem;
            padding: 0.75rem 1rem;
            width: 100%;
            box-sizing: border-box; /* Include padding in width */
            font-size: 1rem;
            color: #374151;
        }
        .input-field:focus {
            outline: none;
            border-color: #D36F2D;
            box-shadow: 0 0 0 3px rgba(211, 111, 45, 0.2);
        }

        /* Alert panel styling */
        .alert {
            padding: 1rem;
            border-radius: 0.5rem;
            margin-bottom: 1rem;
            font-weight: 600;
        }
        .alert-success {
            background-color: #D4EDDA; /* Light green */
            color: #155724; /* Dark green text */
            border-color: #C3E6CB;
        }
        .alert-error {
            background-color: #F8D7DA; /* Light red */
            color: #721C24; /* Dark red text */
            border-color: #F5C6CB;
        }

        /* GridView styling */
        .gv-header th {
            background-color: #051D40; /* Dark blue header */
            color: white;
            padding: 0.75rem 1rem;
            text-align: left;
            font-weight: 700;
        }
        .gv-row td {
            padding: 0.75rem 1rem;
            border-bottom: 1px solid #E5E7EB; /* Light grey border between rows */
        }
        .gv-row:nth-child(even) {
            background-color: #F9FAFB; /* Slightly off-white for even rows */
        }
        .gv-row:hover {
            background-color: #F3F4F6; /* Light grey on hover */
        }
        .gv-empty-row td {
            text-align: center;
            padding: 1.5rem;
            color: #6B7280; /* Medium grey text */
        }
        .gv-pager table {
            width: 100%;
            margin-top: 1rem;
        }
        .gv-pager td {
            padding: 0.5rem;
            text-align: center;
        }
        .gv-pager a, .gv-pager span {
            display: inline-block;
            padding: 0.5rem 0.75rem;
            border-radius: 0.25rem;
            border: 1px solid #D1D5DB;
            color: #374151;
            text-decoration: none;
            margin: 0 0.25rem;
        }
        .gv-pager span {
            background-color: #D36F2D; /* Active pager button matches primary button */
            color: white;
            border-color: #D36F2D;
            font-weight: bold;
        }
        .gv-pager a:hover {
            background-color: #E5E7EB;
        }

        /* Account Card Specific Styles */
        .account-card {
            background-color: #051D40; /* Dark blue background for cards */
            color: white;
            border-radius: 0.75rem; /* rounded-xl */
            padding: 1.5rem; /* p-6 */
            margin-bottom: 1rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            min-height: 150px; /* Ensure cards have a consistent height */
        }
        .account-card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 0.5rem;
        }
        .account-type {
            font-size: 0.875rem; /* text-sm */
            opacity: 0.8;
        }
        .account-balance {
            font-size: 2.25rem; /* text-4xl */
            font-weight: 700; /* font-bold */
            margin-top: 0.5rem;
        }
        .account-number-masked {
            font-size: 1rem; /* text-base */
            opacity: 0.9;
            margin-top: 0.5rem;
        }
        .pagination-dot {
            display: inline-block;
            width: 0.75rem;
            height: 0.75rem;
            background-color: #FBECC3; /* Light yellow for inactive dots */
            border-radius: 50%;
            margin: 0 0.25rem;
            cursor: pointer;
            transition: background-color 0.3s ease, transform 0.3s ease;
        }
        .pagination-dot.active {
            background-color: #D36F2D; /* Orange for active dot */
            transform: scale(1.2);
        }
        .no-accounts-panel {
            background-color: #FFF;
            padding: 2rem;
            border-radius: 0.75rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            text-align: center;
        }
    </style>
    <!-- Font Awesome for icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="header-bg py-12 mb-8">
        <div class="container">
            <h1 class="text-4xl font-bold mb-2">Your Transaction Logs</h1>
            <p class="text-lg opacity-90">View and manage all your bank account transactions.</p>
        </div>
    </div>

    <div class="container py-8">
        <!-- Alert Panel for messages -->
        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Total Balance and Account Overview Section -->
        <div class="bg-white p-6 rounded-lg shadow-md mb-8">
            <div class="flex justify-between items-center mb-4">
                <h2 class="text-2xl font-bold text-gray-800">Total Balance</h2>
                <asp:LinkButton ID="lnkViewAllAccounts" runat="server" Text="View All Accounts" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="text-[#D36F2D] font-semibold hover:underline"></asp:LinkButton>
            </div>
            <asp:Panel ID="pnlAccountOverview" runat="server">
                <h3 class="text-5xl font-bold text-[#051D40] mb-6">
                    <asp:Literal ID="ltTotalBalance" runat="server" Text="$0.00"></asp:Literal>
                </h3>

                <!-- Account Cards Carousel/Repeater -->
                <asp:Panel ID="pnlAccountCards" runat="server" CssClass="relative">
                    <asp:Repeater ID="rptAccounts" runat="server">
                        <ItemTemplate>
                            <div class="account-card" id="accountCard_<%# Eval("AccountId") %>">
                                <div class="account-card-header">
                                    <div>
                                        <div class="account-type"><%# Eval("AccountType") %></div>
                                        <div class="text-xl font-semibold mt-1"><%# Eval("BankName") %></div>
                                    </div>
                                    <!-- Placeholder for bank/card logo, e.g., Visa icon -->
                                    <i class="fa-brands fa-cc-visa text-4xl text-[#FBECC3]"></i>
                                </div>
                                <div>
                                    <div class="account-number-masked">**** **** **** <%# Eval("AccountNumber").ToString().Length >= 4 ? Eval("AccountNumber").ToString().Substring(Eval("AccountNumber").ToString().Length - 4) : Eval("AccountNumber") %></div>
                                    <div class="account-balance"><%# Eval("Balance", "{0:C2}") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <!-- Navigation Buttons for Carousel -->
                    <div class="flex justify-between items-center mt-4">
                        <asp:LinkButton ID="btnPrevAccount" runat="server" OnClick="btnPrevAccount_Click" CssClass="text-[#D36F2D] text-lg font-semibold hover:underline">
                            <i class="fas fa-chevron-left mr-1"></i> Previous
                        </asp:LinkButton>
                        <div id="accountDots" runat="server" class="flex">
                            <!-- Dots will be rendered by JS or code-behind -->
                        </div>
                        <asp:LinkButton ID="btnNextAccount" runat="server" OnClick="btnNextAccount_Click" CssClass="text-[#D36F2D] text-lg font-semibold hover:underline">
                            Next <i class="fas fa-chevron-right ml-1"></i>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>

                <!-- No Accounts Message -->
                <asp:Panel ID="pnlNoAccounts" runat="server" Visible="false" CssClass="no-accounts-panel">
                    <p class="text-lg text-gray-600 mb-4">You don't have any bank accounts linked yet.</p>
                    <asp:Button ID="btnAddFirstAccount" runat="server" Text="Add Your First Account" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="btn-primary" />
                </asp:Panel>
            </asp:Panel>
        </div>

        <!-- Filters and Search Section -->
        <div class="bg-white p-6 rounded-lg shadow-md mb-8">
            <h2 class="text-2xl font-bold text-[#051D40] mb-6">Filter Transactions</h2>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                <!-- Start Date Filter -->
                <div>
                    <label for="<%= txtStartDate.ClientID %>" class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="input-field"></asp:TextBox>
                </div>
                <!-- End Date Filter -->
                <div>
                    <label for="<%= txtEndDate.ClientID %>" class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="input-field"></asp:TextBox>
                </div>
                <!-- Transaction Type Filter -->
                <div>
                    <label for="<%= ddlTransactionType.ClientID %>" class="block text-sm font-medium text-gray-700 mb-1">Transaction Type</label>
                    <asp:DropDownList ID="ddlTransactionType" runat="server" CssClass="input-field">
                        <asp:ListItem Value="All" Text="All Types"></asp:ListItem>
                        <asp:ListItem Value="Credit" Text="Money In"></asp:ListItem>
                        <asp:ListItem Value="Debit" Text="Money Out"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                <!-- Search Bar -->
                <div>
                    <label for="<%= txtSearch.ClientID %>" class="block text-sm font-medium text-gray-700 mb-1">Search Description/Recipient</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="input-field" Placeholder="e.g., Groceries, John Doe"></asp:TextBox>
                </div>
                <!-- Account Filter -->
                <div>
                    <label for="<%= ddlAccount.ClientID %>" class="block text-sm font-medium text-gray-700 mb-1">Filter by Account</label>
                    <asp:DropDownList ID="ddlAccount" runat="server" CssClass="input-field"></asp:DropDownList>
                </div>
            </div>
            <div class="flex flex-col sm:flex-row gap-4">
                <!-- Apply Filters Button -->
                <asp:Button ID="btnApplyFilters" runat="server" Text="Apply Filters" OnClick="btnApplyFilters_Click" CssClass="btn-primary w-full sm:w-auto" />
                <!-- Clear Filters Button -->
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" OnClick="btnClearFilters_Click" CssClass="btn-secondary w-full sm:w-auto" />
                <!-- Export to CSV Button -->
                <asp:Button ID="btnExportCsv" runat="server" Text="Export to CSV" OnClick="btnExportCsv_Click" CssClass="btn-primary w-full sm:w-auto" />
            </div>
        </div>

        <!-- Transaction Logs Display -->
        <div class="bg-white p-6 rounded-lg shadow-md">
            <h2 class="text-2xl font-bold text-[#051D40] mb-6">Transaction History</h2>
            <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False"
                CssClass="w-full text-sm text-left text-gray-500"
                HeaderStyle-CssClass="gv-header"
                RowStyle-CssClass="gv-row"
                EmptyDataRowStyle-CssClass="gv-empty-row"
                PagerStyle-CssClass="gv-pager"
                AllowPaging="True" PageSize="10" OnPageIndexChanging="gvTransactions_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="TransactionTime" HeaderText="Time" DataFormatString="{0:hh\:mm tt}" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:BoundField DataField="SenderRecipient" HeaderText="Sender/Recipient" />
                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("Amount", "{0:C2}") %>'
                                CssClass='<%# Eval("TransactionType").ToString() == "Credit" ? "text-green-600 font-semibold" : "text-red-600 font-semibold" %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="BalanceAfterTransaction" HeaderText="Balance" DataFormatString="{0:C2}" />
                    <asp:BoundField DataField="BankName" HeaderText="Bank" />
                    <asp:BoundField DataField="AccountNickname" HeaderText="Account" />
                </Columns>
                <EmptyDataTemplate>
                    No transactions found for the selected filters.
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
