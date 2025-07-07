<%@ Page Title="Login" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="SpotTheScam.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            background-color: #f0f2f5; /* Light grey background */
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

        .register-link {
            text-align: center;
            margin-top: 1.5rem;
            display: block;
            color: #555;
        }

        .register-link a {
            color: #C88A58;
            font-weight: 600;
            text-decoration: none;
        }
        
        .register-link a:hover {
            text-decoration: underline;
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
            <h2>Welcome Back!</h2>

            <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block text-center" EnableViewState="false"></asp:Label>

            <div class="mb-3">
                <asp:Label ID="Label1" runat="server" Text="Email" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email address"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="mb-4">
                <asp:Label ID="Label2" runat="server" Text="Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="d-grid">
                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-login" OnClick="btnLogin_Click" />
            </div>

            <div class="register-link">
                Don't have an account? <a href="UserRegister.aspx">Sign Up</a>
            </div>
        </div>
    </div>
</asp:Content>
