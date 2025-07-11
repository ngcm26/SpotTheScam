<%@ Page Title="Simulated UOB Login" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="FakeUOBLogin.aspx.cs" Inherits="SpotTheScam.User.FakeUOBLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #f3f4f6;
        }
        .login-container {
            max-width: 400px;
            margin: 4rem auto;
            padding: 2.5rem;
            background-color: #ffffff;
            border-radius: 0.75rem;
            box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
            text-align: center;
        }
        .bank-logo {
            font-size: 4rem; /* Larger icon for prominence */
            color: #ef4444; /* UOB-like red/orange */
            margin-bottom: 1.5rem;
            display: block; /* Ensure it takes full width for centering */
        }
        .bank-name {
            font-size: 2rem;
            font-weight: bold;
            color: #1f2937;
            margin-bottom: 2rem;
        }
        .form-input {
            padding: 0.75rem;
            border: 1px solid #d1d5db;
            border-radius: 0.5rem;
            width: 100%;
            margin-bottom: 1rem;
        }
        .btn-login {
            background-color: #ef4444; /* UOB-like red/orange */
            color: #ffffff;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            transition: background-color 0.3s ease;
            width: 100%;
            font-weight: 600;
        }
        .btn-login:hover {
            background-color: #dc2626;
        }
        .alert {
            padding: 0.75rem;
            border-radius: 0.5rem;
            margin-top: 1.5rem;
            font-size: 0.9rem;
        }
        .alert-success {
            background-color: #d1fae5;
            color: #065f46;
        }
        .alert-danger {
            background-color: #fee2e2;
            color: #991b1b;
        }
        .back-link {
            display: block;
            margin-top: 1.5rem;
            color: #4f46e5;
            text-decoration: none;
            font-size: 0.9rem;
        }
        .back-link:hover {
            text-decoration: underline;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="login-container">
        <span class="bank-logo">🏦</span>
        <h1 class="bank-name">UOB Personal Internet Banking</h1>
        <p class="text-gray-600 mb-6">This is a simulated login page for demonstration purposes only.</p>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <div class="mb-4">
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input" placeholder="Username / NRIC"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" CssClass="text-red-500 text-xs italic" ErrorMessage="Username is required." Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        <div class="mb-6">
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-input" placeholder="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" CssClass="text-red-500 text-xs italic" ErrorMessage="Password is required." Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        
        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" CssClass="btn-login" />

        <a href="UserBankAccounts.aspx" class="back-link">← Back to Bank Accounts</a>
    </div>
</asp:Content>
    