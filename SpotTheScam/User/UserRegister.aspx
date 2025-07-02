<%@ Page Title="Create Account" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserRegister.aspx.cs" Inherits="SpotTheScam.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%-- Custom CSS to match the design from the image --%>
    <style>
        /* Sets the background color for the page */
        body {
            background-color: #f0f2f5; /* Light grey background like in the image */
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
        }

        /* Main container to center the form card */
        .register-container {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 80vh;
            padding: 2rem 0;
        }

        /* The white card holding the form elements */
        .register-card {
            background-color: #ffffff;
            padding: 2rem 2.5rem;
            border-radius: 15px;
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 480px;
        }

        /* Styling for the main "Create An Account" heading */
        .register-card h2 {
            text-align: center;
            color: #333;
            font-weight: 600;
            margin-bottom: 1.5rem;
            font-family: 'Georgia', serif; /* A nice serif font for the heading */
        }

        /* Styling for input fields */
        .form-control {
            border-radius: 8px;
            padding: 12px 15px;
            border: 1px solid #ced4da;
            transition: border-color .15s ease-in-out, box-shadow .15s ease-in-out;
        }

        .form-control:focus {
            border-color: #C88A58; /* Highlight color on focus */
            box-shadow: 0 0 0 0.2rem rgba(200, 138, 88, 0.25);
        }
        
        /* Custom styles for the main register button */
        .btn-register {
            background-color: #C88A58; /* Brownish-orange from the image */
            border-color: #C88A58;
            color: #fff;
            width: 100%;
            padding: 12px;
            font-weight: bold;
            font-size: 1rem;
            border-radius: 8px;
            transition: background-color 0.2s;
        }

        .btn-register:hover {
            background-color: #b57b4f; /* Darker shade on hover */
            border-color: #b57b4f;
            color: #fff;
        }

        /* Link for users who already have an account */
        .login-link {
            text-align: center;
            margin-top: 1.5rem;
            display: block;
            color: #555;
        }

        .login-link a {
            color: #C88A58;
            font-weight: 600;
            text-decoration: none;
        }
        
        .login-link a:hover {
            text-decoration: underline;
        }

        /* Style for validation error messages */
        .error-message {
            color: #dc3545; /* Bootstrap's danger color */
            font-size: 0.875em;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="register-container">
        <div class="register-card">
            <h2>Create An Account</h2>

            <%-- This label will show success or failure messages from the server --%>
            <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block text-center" EnableViewState="false"></asp:Label>

            <%-- Username Field --%>
            <div class="mb-3">
                <asp:Label ID="Label1" runat="server" Text="Username" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Choose a unique username"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Username is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <%-- Email Field --%>
            <div class="mb-3">
                <asp:Label ID="Label3" runat="server" Text="Email Address" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="e.g., yourname@example.com"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                    ErrorMessage="Please enter a valid email address." CssClass="error-message" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>

            <%-- Password Field --%>
            <div class="mb-3">
                <asp:Label ID="Label2" runat="server" Text="Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Create a strong password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <%-- Confirm Password Field --%>
            <div class="mb-4">
                <asp:Label ID="Label4" runat="server" Text="Confirm Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password again"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Please confirm your password." CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword"
                    ErrorMessage="Passwords do not match." CssClass="error-message" Display="Dynamic"></asp:CompareValidator>
            </div>

            <%-- Register Button --%>
            <div class="d-grid">
                 <asp:Button ID="btnRegister" runat="server" Text="Continue" CssClass="btn btn-register" OnClick="btnRegister_Click" />
            </div>
           
            <div class="login-link">
                Already have an account? <a href="UserLogin.aspx">Login</a>
            </div>
        </div>
    </div>
</asp:Content>
