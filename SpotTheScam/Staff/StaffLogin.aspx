<%@ Page Title="Staff Login" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="StaffLogin.aspx.cs" Inherits="SpotTheScam.Staff.StaffLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            background-color: #f0f2f5;
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
        }

        .login-container {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 80vh;
            padding: 2rem 0;
        }

        .login-card {
            background-color: #ffffff;
            padding: 2rem 2.5rem;
            border-radius: 15px;
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 480px;
        }

        .login-card h2 {
            text-align: center;
            color: #333;
            font-weight: 600;
            margin-bottom: 1.5rem;
            font-family: 'Georgia', serif;
        }

        .form-control {
            border-radius: 8px;
            padding: 12px 15px;
            border: 1px solid #ced4da;
            transition: border-color .15s ease-in-out, box-shadow .15s ease-in-out;
        }

        .form-control:focus {
            border-color: #C88A58;
            box-shadow: 0 0 0 0.2rem rgba(200, 138, 88, 0.25);
        }

        .btn-login {
            background-color: #C88A58;
            border-color: #C88A58;
            color: #fff;
            width: 100%;
            padding: 12px;
            font-weight: bold;
            font-size: 1rem;
            border-radius: 8px;
            transition: background-color 0.2s;
        }

        .btn-login:hover {
            background-color: #b57b4f;
            border-color: #b57b4f;
            color: #fff;
        }

        .error-message {
            color: #dc3545;
            font-size: 0.875em;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="login-container">
        <div class="login-card">
            <h2>Staff Login</h2>

            <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block text-center" EnableViewState="false"></asp:Label>

            <div class="mb-3">
                <asp:Label ID="lblUsername" runat="server" Text="Username" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Username is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="mb-4">
                <asp:Label ID="lblPassword" runat="server" Text="Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="d-grid">
                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-login" OnClick="btnLogin_Click" />
            </div>
        </div>
    </div>
</asp:Content>
