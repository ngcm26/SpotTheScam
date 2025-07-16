<%@ Page Title="Add Bank Account" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="AddBankAccounts.aspx.cs" Inherits="SpotTheScam.User.AddBankAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Add Bank Account</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #f3f4f6;
        }

        .container {
            max-width: 800px; /* Slightly wider for form clarity */
            margin: 2rem auto;
            padding: 1rem;
        }

        .card {
            background-color: #ffffff;
            border-radius: 0.75rem;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            padding: 2rem;
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

        .text-red-500 {
            color: #ef4444;
        }
        .text-xs {
            font-size: 0.75rem;
        }
        .mt-1 {
            margin-top: 0.25rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h1 class="text-3xl font-bold text-gray-800 mb-6">
            Add New Bank Account
            <%-- NEW: Discreet "Seed Mock Data" Icon --%>
            <asp:LinkButton ID="btnSeedMockAccounts" runat="server" OnClick="btnSeedMockAccounts_Click" CausesValidation="false" CssClass="ml-4 text-gray-400 hover:text-gray-600 focus:outline-none" ToolTip="Seed Mock Data (for demo)">
                <i class="fas fa-database fa-sm"></i>
            </asp:LinkButton>
        </h1>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-6">
            <span id="AlertIcon" runat="server" class="alert-icon"></span>
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <div class="card">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label for="<%=txtBankName.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Bank Name:</label>
                    <asp:TextBox ID="txtBankName" runat="server" CssClass="form-input" placeholder="e.g., DBS, OCBC, UOB"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBankName" runat="server" ControlToValidate="txtBankName" ErrorMessage="Bank Name is required." CssClass="text-red-500 text-xs mt-1" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div>
                    <label for="<%=txtAccountType.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Account Type:</label>
                    <asp:TextBox ID="txtAccountType" runat="server" CssClass="form-input" Text="Savings" placeholder="e.g., Savings, Checking, Credit Card"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=txtAccountNumber.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Account Number:</label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-input" placeholder="e.g., 123-456-789"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber" ErrorMessage="Account Number is required." CssClass="text-red-500 text-xs mt-1" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div>
                    <label for="<%=txtAccountNickname.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Account Nickname (Optional):</label>
                    <asp:TextBox ID="txtAccountNickname" runat="server" CssClass="form-input" placeholder="e.g., Main Savings, Travel Card"></asp:TextBox>
                </div>
                <div class="md:col-span-2">
                    <label for="<%=txtBalance.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Initial Balance:</label>
                    <asp:TextBox ID="txtBalance" runat="server" CssClass="form-input" TextMode="Number" placeholder="e.g., 1500.00"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBalance" runat="server" ControlToValidate="txtBalance" ErrorMessage="Initial Balance is required." CssClass="text-red-500 text-xs mt-1" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvBalance" runat="server" ControlToValidate="txtBalance" Type="Currency" Operator="DataTypeCheck" ErrorMessage="*Must be a valid number (e.g., 1500.00)" Display="Dynamic" CssClass="text-red-500 text-xs mt-1"></asp:CompareValidator>
                </div>
            </div>

            <div class="flex flex-wrap gap-4 justify-end mt-6">
                <asp:Button ID="btnAddAccount" runat="server" Text="Add Account" OnClick="btnAddAccount_Click" CssClass="btn-primary" />
                <asp:Button ID="btnBack" runat="server" Text="Back to Accounts" OnClick="btnBack_Click" CssClass="btn-secondary" CausesValidation="false" />
            </div>
        </div>
    </div>
</asp:Content>