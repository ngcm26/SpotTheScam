<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyEmail.aspx.cs" Inherits="SpotTheScam.User.VerifyEmail" MasterPageFile="User.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap');
        body { background-color: #f0f2f5; font-family: 'DM Sans', sans-serif; color: #051D40; }
        .verify-container { display:flex; align-items:center; justify-content:center; min-height:80vh; padding:2rem 0; }
        .verify-card { background:#fff; padding:2rem 2.5rem; border-radius:15px; box-shadow:0 8px 25px rgba(0,0,0,0.1); width:100%; max-width:480px; }
        .verify-card h2 { text-align:center; color:#051D40; font-weight:600; margin-bottom:1.5rem; }
        .form-control { border-radius:8px; padding:12px 15px; border:1px solid #ced4da; color:#051D40; }
        .form-control:focus { border-color:#D36F2D; box-shadow:0 0 0 0.2rem rgba(211,111,45,0.15); }
        /* Verify button: navy background with white text */
        .btn-login { background-color: var(--brand-navy); border-color: var(--brand-navy); color:#fff; width:100%; padding:12px; font-weight:bold; font-size:1rem; border-radius:8px; transition: background-color 0.2s, border-color 0.2s; border: 1px solid var(--brand-navy); }
        .btn-login:hover { background-color:#082657; border-color:#082657; color:#fff; }
    </style>
    <title>Verify Email</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="verify-container">
        <div class="verify-card">
            <h2>Verify your email</h2>
            <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block text-center" EnableViewState="false" />
            <div class="mb-3">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email" />
            </div>
            <div class="mb-4">
                <label class="form-label">Verification code</label>
                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" Placeholder="6-digit code" MaxLength="6" />
            </div>
            <div class="d-grid gap-2">
                <asp:Button ID="btnVerify" runat="server" Text="Verify" CssClass="btn btn-login" OnClick="btnVerify_Click" />
                <asp:Button ID="btnResend" runat="server" Text="Resend code" CssClass="btn btn-outline-dark" OnClick="btnResend_Click" />
            </div>
        </div>
    </div>
</asp:Content>


