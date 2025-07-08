<%@ Page Title="Bank Accounts" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBankAccounts.aspx.cs" Inherits="SpotTheScam.User.UserBankAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Your Bank Accounts - SpotTheScam</title>
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
        .input-label {
            display: block;
            font-size: 0.875rem; /* text-sm */
            font-weight: 500; /* font-medium */
            color: #4B5563; /* gray-700 */
            margin-bottom: 0.25rem;
        }

        /* Alert panel styling */
        .alert {
            padding: 1rem;
            border-radius: 0.5rem;
            margin-bottom: 1rem;
            font-weight: 600;
            display: flex;
            align-items: center;
        }
        .alert-success {
            background-color: #D4EDDA; /* Light green */
            color: #155724; /* Dark green text */
            border-color: #C3E6CB;
        }
        .alert-danger { /* Renamed from alert-error to alert-danger for consistency */
            background-color: #F8D7DA; /* Light red */
            color: #721C24; /* Dark red text */
            border-color: #F5C6CB;
        }
        .alert-icon {
            margin-right: 0.75rem;
            font-size: 1.25rem;
        }

        /* GridView styling */
        .gv-header th {
            background-color: #051D40; /* Dark blue header */
            color: white;
            padding: 0.75rem 1rem;
            text-align: left;
            font-weight: 700;
            border-top-left-radius: 0.5rem;
            border-top-right-radius: 0.5rem;
        }
        .gv-row td {
            padding: 0.75rem 1rem;
            border-bottom: 1px solid #E5E7EB; /* Light grey border between rows */
            color: #374151;
        }
        .gv-row:last-child td {
            border-bottom: none; /* No border on the last row */
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
        .gv-actions a {
            color: #D36F2D;
            text-decoration: none;
            font-weight: 600;
            margin-right: 0.75rem;
            transition: color 0.2s ease;
        }
        .gv-actions a:hover {
            color: #C15F22;
        }
        .gv-actions a.delete-link {
            color: #EF4444; /* Red for delete */
        }
        .gv-actions a.delete-link:hover {
            color: #DC2626;
        }
        .gv-edit-input {
            width: 100%;
            padding: 0.5rem;
            border: 1px solid #D1D5DB;
            border-radius: 0.25rem;
            box-sizing: border-box;
        }
        .gv-edit-input:focus {
            outline: none;
            border-color: #D36F2D;
            box-shadow: 0 0 0 2px rgba(211, 111, 45, 0.2);
        }
    </style>
    <!-- Font Awesome for icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="header-bg py-12 mb-8">
        <div class="container">
            <h1 class="text-4xl font-bold mb-2">Your Bank Accounts</h1>
            <p class="text-lg opacity-90">Manage your linked bank accounts and cards.</p>
        </div>
    </div>

    <div class="container py-8">
        <!-- Alert Panel -->
        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert">
            <i id="AlertIcon" runat="server" class="alert-icon"></i>
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Add New Account Form -->
        <div class="bg-white p-6 rounded-lg shadow-md mb-8">
            <h2 class="text-2xl font-bold text-[#051D40] mb-6">Add New Account</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                <div>
                    <asp:Label ID="lblBankName" runat="server" Text="Bank Name" CssClass="input-label"></asp:Label>
                    <asp:TextBox ID="txtBankName" runat="server" CssClass="input-field" Placeholder="e.g., DBS Bank"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBankName" runat="server" ControlToValidate="txtBankName"
                        ErrorMessage="Bank Name is required." ForeColor="#EF4444" Display="Dynamic" CssClass="text-sm mt-1"></asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label ID="lblAccountType" runat="server" Text="Account Type" CssClass="input-label"></asp:Label>
                    <asp:TextBox ID="txtAccountType" runat="server" CssClass="input-field" Text="Savings" Placeholder="e.g., Savings, Checking, Credit Card"></asp:TextBox>
                    <%-- No RequiredFieldValidator for AccountType as it has a default value --%>
                </div>
                <div>
                    <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number" CssClass="input-label"></asp:Label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="input-field" Placeholder="e.g., 1234567890"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber"
                        ErrorMessage="Account Number is required." ForeColor="#EF4444" Display="Dynamic" CssClass="text-sm mt-1"></asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label ID="lblAccountNickname" runat="server" Text="Account Nickname (Optional)" CssClass="input-label"></asp:Label>
                    <asp:TextBox ID="txtAccountNickname" runat="server" CssClass="input-field" Placeholder="e.g., Main Savings, Daily Spending"></asp:TextBox>
                </div>
                <div>
                    <asp:Label ID="lblBalance" runat="server" Text="Current Balance" CssClass="input-label"></asp:Label>
                    <asp:TextBox ID="txtBalance" runat="server" TextMode="Number" CssClass="input-field" Placeholder="e.g., 1000.00"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBalance" runat="server" ControlToValidate="txtBalance"
                        ErrorMessage="Balance is required." ForeColor="#EF4444" Display="Dynamic" CssClass="text-sm mt-1"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvBalance" runat="server" ControlToValidate="txtBalance" Type="Currency" Operator="DataTypeCheck"
                        ErrorMessage="Balance must be a valid number." ForeColor="#EF4444" Display="Dynamic" CssClass="text-sm mt-1"></asp:CompareValidator>
                </div>
            </div>
            <asp:Button ID="btnAddAccount" runat="server" Text="Add Account" OnClick="btnAddAccount_Click" CssClass="btn-primary w-full md:w-auto" />
        </div>

        <!-- Existing Bank Accounts Display -->
        <div class="bg-white p-6 rounded-lg shadow-md">
            <h2 class="text-2xl font-bold text-[#051D40] mb-6">Your Linked Accounts</h2>
            <asp:GridView ID="gvBankAccounts" runat="server" AutoGenerateColumns="False"
                CssClass="w-full text-sm text-left text-gray-500"
                HeaderStyle-CssClass="gv-header"
                RowStyle-CssClass="gv-row"
                EmptyDataRowStyle-CssClass="gv-empty-row"
                AllowEditing="True" AllowDeleting="True" DataKeyNames="AccountId"
                OnRowEditing="gvBankAccounts_RowEditing"
                OnRowCancelingEdit="gvBankAccounts_RowCancelingEdit"
                OnRowUpdating="gvBankAccounts_RowUpdating"
                OnRowDeleting="gvBankAccounts_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="BankName" HeaderText="Bank Name" />
                    <asp:BoundField DataField="AccountType" HeaderText="Account Type" />
                    <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" />
                    <asp:BoundField DataField="AccountNickname" HeaderText="Nickname" />
                    <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:C2}" />
                    <asp:BoundField DataField="DateAdded" HeaderText="Date Added" DataFormatString="{0:d}" ReadOnly="True" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="gv-actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" Text="Edit" CssClass="text-[#D36F2D] hover:text-[#C15F22] mr-3"></asp:LinkButton>
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" Text="Delete" CssClass="text-red-500 hover:text-red-700"
                                OnClientClick="return confirm('Are you sure you want to delete this account?');"></asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="text-green-600 hover:text-green-800 mr-3"></asp:LinkButton>
                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="text-gray-500 hover:text-gray-700"></asp:LinkButton>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    No bank accounts added yet. Use the form above to add your first account!
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
