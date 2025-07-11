<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBankAccounts.aspx.cs" Inherits="SpotTheScam.User.UserBankAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Bank Accounts</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #f3f4f6;
        }

        .container {
            max-width: 960px;
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

        .alert-danger {
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

        .gv-edit-input {
            width: 100%;
            padding: 0.5rem;
            border: 1px solid #ccc;
            border-radius: 0.375rem;
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

        .bank-option-card {
            border: 1px solid #e5e7eb;
            border-radius: 0.75rem;
            padding: 2rem;
            text-align: center;
            transition: all 0.2s ease-in-out;
            cursor: pointer;
        }
        .bank-option-card:hover {
            border-color: #4f46e5;
            box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
            transform: translateY(-5px);
        }
        .bank-logo {
            font-size: 3rem; /* Adjust size as needed */
            margin-bottom: 1rem;
            color: #4f46e5; /* Example color */
        }
        .bank-name {
            font-size: 1.5rem;
            font-weight: bold;
            color: #1f2937;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto p-4">
        <h1 class="text-3xl font-bold text-gray-800 mb-6">Manage Bank Accounts</h1>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-6">
            <i id="AlertIcon" runat="server" class="alert-icon"></i>
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <%-- Bank Selection Panel --%>
        <asp:Panel ID="pnlBankSelection" runat="server" CssClass="card p-6 mb-6">
            <h2 class="text-2xl font-semibold text-gray-700 mb-4">Link a New Bank Account</h2>
            <p class="text-gray-600 mb-6">Choose your bank to proceed with linking your account. This is a simulated process for demonstration purposes.</p>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <a href="FakeUOBLogin.aspx" class="bank-option-card flex flex-col items-center justify-center">
                    <span class="bank-logo">🏦</span>
                    <span class="bank-name">UOB (Simulated)</span>
                    <p class="text-sm text-gray-500 mt-2">Click to link your UOB account.</p>
                </a>
                <a href="FakeOCBCLogin.aspx" class="bank-option-card flex flex-col items-center justify-center">
                    <span class="bank-logo">🏦</span>
                    <span class="bank-name">OCBC (Simulated)</span>
                    <p class="text-sm text-gray-500 mt-2">Click to link your OCBC account.</p>
                </a>
            </div>
        </asp:Panel>

        <%-- Existing Add Account Form (Hidden by default) --%>
        <asp:Panel ID="pnlAddAccountForm" runat="server" CssClass="card p-6 mb-6">
            <h2 class="text-2xl font-semibold text-gray-700 mb-4">Add New Bank Account</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                <div>
                    <asp:Label ID="lblBankName" runat="server" Text="Bank Name:" CssClass="block text-gray-700 text-sm font-bold mb-2"></asp:Label>
                    <asp:TextBox ID="txtBankName" runat="server" CssClass="form-input"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBankName" runat="server" ControlToValidate="txtBankName" CssClass="text-red-500 text-xs italic" ErrorMessage="Bank Name is required."></asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label ID="lblAccountType" runat="server" Text="Account Type:" CssClass="block text-gray-700 text-sm font-bold mb-2"></asp:Label>
                    <asp:TextBox ID="txtAccountType" runat="server" Text="Savings" CssClass="form-input"></asp:TextBox>
                </div>
                <div>
                    <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number:" CssClass="block text-gray-700 text-sm font-bold mb-2"></asp:Label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-input"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber" CssClass="text-red-500 text-xs italic" ErrorMessage="Account Number is required."></asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label ID="lblAccountNickname" runat="server" Text="Account Nickname (Optional):" CssClass="block text-gray-700 text-sm font-bold mb-2"></asp:Label>
                    <asp:TextBox ID="txtAccountNickname" runat="server" CssClass="form-input"></asp:TextBox>
                </div>
                <div class="col-span-1 md:col-span-2">
                    <asp:Label ID="lblBalance" runat="server" Text="Initial Balance:" CssClass="block text-gray-700 text-sm font-bold mb-2"></asp:Label>
                    <asp:TextBox ID="txtBalance" runat="server" TextMode="Number" CssClass="form-input"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBalance" runat="server" ControlToValidate="txtBalance" CssClass="text-red-500 text-xs italic" ErrorMessage="Balance is required."></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvBalance" runat="server" ControlToValidate="txtBalance" Type="Currency" Operator="DataTypeCheck" CssClass="text-red-500 text-xs italic" ErrorMessage="Balance must be a valid number."></asp:CompareValidator>
                </div>
            </div>
            <asp:Button ID="btnAddAccount" runat="server" Text="Add Account" OnClick="btnAddAccount_Click" CssClass="btn-primary" />
        </asp:Panel>

        <h2 class="text-2xl font-semibold text-gray-700 mb-4">Your Linked Bank Accounts</h2>
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
            </asp:GridView>
        </div>
    </div>
</asp:Content>
