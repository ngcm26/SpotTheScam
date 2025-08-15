<%@ Page Title="Login" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="SpotTheScam.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap');
        body {
            background-color: #f0f2f5; /* Light grey background */
            font-family: 'DM Sans', sans-serif;
            color: #051D40;
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
            font-family: 'DM Sans', sans-serif;
        }

        .login-card h2 {
            text-align: center;
            color: #051D40;
            font-weight: 600;
            margin-bottom: 1.5rem;
            font-family: 'DM Sans', sans-serif;
        }


        .form-control {
            border-radius: 8px;
            padding: 12px 15px;
            border: 1px solid #ced4da;
            transition: border-color .15s ease-in-out, box-shadow .15s ease-in-out;
            font-family: 'DM Sans', sans-serif;
            color: #051D40;
        }

        .form-control:focus {
            border-color: #D36F2D; 
            box-shadow: 0 0 0 0.2rem rgba(211, 111, 45, 0.15);
        }
        
        .btn-login {
            background-color: #D36F2D;
            border-color: #D36F2D;
            color: #fff;
            width: 100%;
            padding: 12px;
            font-weight: bold;
            font-size: 1rem;
            border-radius: 8px;
            transition: background-color 0.2s;
            font-family: 'DM Sans', sans-serif;
        }

        .btn-login:hover {
            background-color: #b45a22;
            border-color: #b45a22;
            color: #fff;
        }

        .register-link {
            text-align: center;
            margin-top: 1.5rem;
            display: block;
            color: #051D40;
            font-family: 'DM Sans', sans-serif;
        }

        .register-link a {
            color: #D36F2D;
            font-weight: 600;
            text-decoration: none;
            font-family: 'DM Sans', sans-serif;
        }
        
        .register-link a:hover {
            text-decoration: underline;
            color: #b45a22;
        }

        .error-message {
            color: #dc3545; 
            font-size: 0.875em;
            font-family: 'DM Sans', sans-serif;
        }
        .form-label {
            color: #051D40;
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
                Don't have an account? <a href="UserRegister.aspx">Sign Up </a>
            </div>
            <div class="register-link">
                <a href="ForgotPassword.aspx">Forgot your password?</a>
            </div>
        </div>
    </div>
</asp:Content>
