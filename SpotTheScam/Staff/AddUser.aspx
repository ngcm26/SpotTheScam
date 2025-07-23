<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="SpotTheScam.Staff.AddUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Add User</title>
    <style>
        body, .edit-user-container, .edit-user-container *, .form-label, .form-input, .form-select, .user-info-title {
            font-family: 'DM Sans', Arial, sans-serif !important;
        }
        .edit-user-container {
            display: flex;
            gap: 48px;
            align-items: flex-start;
            margin-top: 24px;
            margin-bottom: 32px;
        }
        .user-info-section {
            flex: 1;
            min-width: 320px;
        }
        .user-info-title {
            font-size: 1.35em;
            color: #222;
            font-weight: 600;
            margin-bottom: 0.2em;
        }
        .user-info-divider {
            border: none;
            border-top: 2px solid #222;
            margin-bottom: 1.2em;
            margin-top: 0.2em;
        }
        .form-label {
            color: #051D40;
            font-weight: 600;
            margin-bottom: 0.2em;
            display: block;
        }
        .form-input, .form-select, .form-textarea {
            width: 100%;
            background: #fff;
            border: 1.5px solid #bfc5ce;
            border-radius: 6px;
            padding: 10px 16px;
            font-size: 1.08em;
            margin-bottom: 1.1em;
            color: #585252;
            font-weight: 400;
            outline: none;
            transition: border-color 0.2s;
            resize: vertical;
        }
        .form-input:focus, .form-select:focus, .form-textarea:focus {
            border-color: #C46A1D;
        }
        .form-actions {
            margin-top: 18px;
            display: flex;
            gap: 12px;
        }
        .btn {
            padding: 10px 24px;
            border-radius: 6px;
            font-weight: 500;
            font-size: 1em;
            border: none;
            cursor: pointer;
            transition: all 0.2s;
        }
        .btn-primary {
            background: #C46A1D;
            color: white;
        }
        .btn-primary:hover, .btn-primary:active, .btn-primary:focus {
            background: #a85a1a;
            color: white;
            outline: none !important;
            box-shadow: none !important;
        }
        .btn-secondary {
            background: #6c757d;
            color: white;
        }
        .btn-secondary:hover {
            background: #5a6268;
        }
        .rfv-error, .rfv-error span {
            font-family: 'DM Sans', Arial, sans-serif !important;
            font-size: 1em !important;
            margin: 2px 0 6px 0 !important;
            padding: 0 !important;
            display: block;
        }
        .success-message {
            color: #28a745;
            font-weight: 500;
            margin-bottom: 1em;
        }
        .error-message {
            color: #dc3545;
            font-weight: 500;
            margin-bottom: 1em;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-bottom: 10px;">
        <h1 style="color: #C46A1D; font-weight: 600; margin-bottom: 0.2em; font-size: 2.1rem;">Add User</h1>
        <div style="color: #344054; font-size: 1.15rem; margin-bottom: 1.2em;">Fill in the details to add a new user.</div>
    </div>
    <div class="edit-user-container">
        <div class="user-info-section">
            <div class="user-info-title">User Information</div>
            <hr class="user-info-divider" />
            <label for="txtUsername" class="form-label">Username:</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input" />
            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
            <label for="txtEmail" class="form-label">Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" TextMode="Email" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
            <label for="txtPhoneNumber" class="form-label">Phone Number:</label>
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-input" />
            <label for="ddlRole" class="form-label">Role:</label>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select">
                <asp:ListItem Text="User" Value="user" />
                <asp:ListItem Text="Staff" Value="staff" />
                <asp:ListItem Text="Admin" Value="admin" />
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvRole" runat="server" ControlToValidate="ddlRole" ErrorMessage="Role is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
            <label for="txtPassword" class="form-label">Password:</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
            <label for="txtConfirmPassword" class="form-label">Confirm Password:</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-input" TextMode="Password" />
            <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" ErrorMessage="Passwords do not match." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
            <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false" />
            <asp:Label ID="lblMessage" runat="server" CssClass="error-message" Visible="false" />
            <div class="form-actions">
                <asp:Button ID="btnAddUser" runat="server" Text="Add User" CssClass="btn btn-primary" OnClick="btnAddUser_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>
</asp:Content>
