<%@ Page Title="Manage Bank Accounts" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBankAccounts.aspx.cs" Inherits="SpotTheScam.User.UserBankAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Bank Accounts</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap" rel="stylesheet"> <%-- Added DM Sans font --%>

    <style>
        body {
            font-family: 'DM Sans', sans-serif; /* Updated font family */
            background-color: #f0f2f5; /* Light grey background */
            color: #051D40; /* Dark blue for general text */
        }

        .container {
            max-width: 960px;
            margin: 2rem auto; /* Center container with top/bottom margin */
            padding: 1rem;
        }

        .card {
            background-color: #ffffff;
            border-radius: 15px; /* Softer rounded corners */
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1); /* Stronger shadow */
            padding: 2rem;
        }

        h1, h2 {
            color: #051D40; /* Dark blue for headings */
            font-weight: 700; /* Bolder headings */
        }

        .btn-primary {
            background-color: #D36F2D; /* Primary orange color */
            border-color: #D36F2D;
            color: #ffffff;
            padding: 0.75rem 1.5rem;
            font-weight: 500;
            border-radius: 8px; /* Slightly softer button corners */
            transition: background-color 0.2s ease, border-color 0.2s ease;
        }

        .btn-primary:hover {
            background-color: #b45a22; /* Darker orange on hover */
            border-color: #b45a22;
        }

        .btn-secondary {
            background-color: #6b7280; /* Keep current grey for secondary */
            color: #ffffff;
            padding: 0.75rem 1.5rem;
            font-weight: 500;
            border-radius: 8px;
            transition: background-color 0.2s ease;
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
            font-weight: 500;
        }

        .alert-success {
            background-color: #d1fae5;
            color: #065f46;
        }

        .alert-danger {
            background-color: #fee2e2;
            color: #991b1b;
        }

        .alert-icon {
            font-size: 1.25rem;
        }

        .form-input {
            padding: 0.75rem;
            border: 1px solid #ced4da; /* Lighter border for inputs */
            border-radius: 8px; /* Match button/card radius */
            width: 100%;
            color: #051D40;
            font-family: 'DM Sans', sans-serif;
            transition: border-color .15s ease-in-out, box-shadow .15s ease-in-out;
        }
        .form-input:focus {
            border-color: #D36F2D; /* Highlight color on focus */
            box-shadow: 0 0 0 0.2rem rgba(211, 111, 45, 0.15);
        }

        .gv-edit-input {
            width: 100%;
            padding: 0.5rem;
            border: 1px solid #ccc;
            border-radius: 0.375rem;
        }

        .gv-header th {
            background-color: #e5e7eb; /* Keep light grey for table header */
            padding: 0.75rem;
            text-align: left;
            font-weight: 600;
            color: #051D40; /* Darker text for header */
        }

        .gv-row td {
            padding: 0.75rem;
            border-bottom: 1px solid #e5e7eb;
            color: #1f2937; /* Slightly darker text for table rows */
        }

        .gv-row:nth-child(even) {
            background-color: #f9fafb;
        }

        .gv-actions a {
            color: #D36F2D; /* Use primary color for actions */
            text-decoration: none;
            margin-right: 0.5rem;
        }

        .gv-actions a:hover {
            text-decoration: underline;
            color: #b45a22; /* Darker orange on hover */
        }

        /* Account Carousel Styling */
        .account-card-container {
            display: flex;
            overflow-x: hidden;
            scroll-behavior: smooth;
            -webkit-overflow-scrolling: touch;
        }

        .account-card {
            flex: 0 0 100%;
            padding: 1.5rem;
            border-radius: 15px; /* Match main card radius */
            background: linear-gradient(135deg, #D36F2D 0%, #E85A4F 100%); /* Orange gradient */
            color: white;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            margin-right: 1rem;
        }

        .account-card:last-child {
            margin-right: 0;
        }

        .account-card h3 {
            font-size: 1.5rem;
            font-weight: bold;
            margin-bottom: 0.5rem;
            color: white; /* Ensure heading is white */
        }

        .account-card p {
            font-size: 1rem;
            margin-bottom: 0.25rem;
            color: rgba(255,255,255,0.9); /* Slightly muted white */
        }

        .account-card .balance {
            font-size: 2rem;
            font-weight: bold;
            margin-top: 1rem;
            color: white;
        }

        .pagination-dots {
            display: flex;
            justify-content: center;
            margin-top: 1rem;
        }

        .pagination-dot {
            height: 10px;
            width: 10px;
            background-color: rgba(211, 111, 45, 0.4); /* Muted orange */
            border-radius: 50%;
            display: inline-block;
            margin: 0 5px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .pagination-dot.active {
            background-color: #D36F2D; /* Solid orange when active */
        }

        /* General text styling adjustments */
        .text-gray-800 { color: #051D40; } /* Mapping dark gray to dark blue */
        .text-gray-700 { color: #051D40; }
        .text-gray-600 { color: #1f2937; } /* Slightly lighter dark blue */
        .text-indigo-600, .text-indigo-700 { color: #D36F2D; } /* Mapping indigo to primary orange */
        .hover\:underline:hover { text-decoration: underline; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto p-4">
        <h1 class="text-3xl font-bold mb-6">Manage Bank Accounts</h1>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-6">
            <i id="AlertIcon" runat="server" class="alert-icon"></i>
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <%-- Account Overview Section --%>
        <div class="card p-6 mb-6">
            <div class="flex justify-between items-center mb-4">
                <h2 class="text-2xl font-semibold">Account Overview</h2>
                <div>
                    <asp:LinkButton ID="lnkViewAllAccounts" runat="server" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="text-link hover:underline">Refresh Accounts</asp:LinkButton>
                    <%-- Link to the new Add Bank Accounts page --%>
                    <asp:HyperLink ID="hlAddAccount" runat="server" NavigateUrl="~/User/AddBankAccounts.aspx" CssClass="ml-4 text-link hover:underline">Add New Account</asp:HyperLink>
                </div>
            </div>

            <asp:Panel ID="pnlAccountOverview" runat="server" Visible="false">
                <div class="mb-4">
                    <p class="text-secondary-text text-lg">Total Balance Across All Accounts:</p>
                    <p class="text-4xl font-bold text-highlight-text"><asp:Literal ID="ltTotalBalance" runat="server"></asp:Literal></p>
                </div>

                <div class="relative flex items-center">
                    <asp:LinkButton ID="btnPrevAccount" runat="server" OnClick="btnPrevAccount_Click" CssClass="carousel-nav-btn left-0">
                        <i class="fas fa-chevron-left"></i>
                    </asp:LinkButton>

                    <asp:Panel ID="pnlAccountCards" runat="server" CssClass="account-card-container flex-grow">
                        <asp:Repeater ID="rptAccounts" runat="server">
                            <ItemTemplate>
                                <div class="account-card flex-shrink-0 w-full">
                                    <h3 class="font-semibold"><%# Eval("AccountNickname") %></h3>
                                    <p class="text-sm"><%# Eval("BankName") %> - <%# Eval("AccountType") %></p>
                                    <p class="text-sm">Account No: <%# Eval("AccountNumber") %></p>
                                    <p class="balance"><%# Eval("Balance", "{0:C2}") %></p>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>

                    <asp:LinkButton ID="btnNextAccount" runat="server" OnClick="btnNextAccount_Click" CssClass="carousel-nav-btn right-0">
                        <i class="fas fa-chevron-right"></i>
                    </asp:LinkButton>
                </div>
                <div id="accountDots" runat="server" class="pagination-dots"></div>
            </asp:Panel>

            <asp:Panel ID="pnlNoAccounts" runat="server" Visible="false" CssClass="text-center py-8">
                <p class="text-secondary-text text-lg mb-4">No bank accounts linked yet.</p>
                <asp:Button ID="btnAddFirstAccount" runat="server" Text="Add Your First Account" PostBackUrl="~/User/AddBankAccounts.aspx" CssClass="btn-primary" />
            </asp:Panel>
        </div>

        <h2 class="text-2xl font-semibold mb-4 mt-6">Your Linked Bank Accounts</h2>
        <div class="card p-6">
            <asp:GridView ID="gvBankAccounts" runat="server" AutoGenerateColumns="False" DataKeyNames="AccountId"
                OnRowEditing="gvBankAccounts_RowEditing" OnRowCancelingEdit="gvBankAccounts_RowCancelingEdit"
                OnRowUpdating="gvBankAccounts_RowUpdating" OnRowDeleting="gvBankAccounts_RowDeleting"
                OnRowDataBound="gvBankAccounts_RowDataBound"
                CssClass="min-w-full divide-y divide-gray-200" HeaderStyle-CssClass="gv-header" RowStyle-CssClass="gv-row">
                <Columns>
                    <asp:BoundField DataField="BankName" HeaderText="Bank Name" />
                    <asp:BoundField DataField="AccountType" HeaderText="Account Type" />
                    <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" />
                    <asp:BoundField DataField="AccountNickname" HeaderText="Nickname" />
                    <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:C2}" />
                    <asp:BoundField DataField="DateAdded" HeaderText="Date Added" DataFormatString="{0:d}" />
                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" ControlStyle-CssClass="gv-actions" HeaderText="Actions" />
                </Columns>
                <EmptyDataTemplate>
                    <p class="text-gray-500 text-center py-4">No bank accounts linked yet.</p>
                </EmptyDataTemplate>
                <PagerStyle CssClass="pagination-style" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>