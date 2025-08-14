<%@ Page Title="Connect Bank" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="ConnectBank.aspx.cs" Inherits="SpotTheScam.User.ConnectBank" %>

<asp:Content ID="headC" ContentPlaceHolderID="head" runat="server">
  <style>
    .card{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:24px;max-width:720px;margin:24px auto;box-shadow:0 1px 3px rgba(0,0,0,.06)}
    .btn{display:inline-block;padding:12px 18px;border-radius:10px;border:0;font-weight:600;cursor:pointer}
    .btn-primary{background:#2563eb;color:#fff}
    .btn-danger{background:#ef4444;color:#fff}
    .muted{color:#6b7280}
    .stack>*{margin-right:12px;margin-bottom:12px}
    .alert{padding:12px 16px;border-radius:10px;margin-bottom:16px}
    .alert-success{background:#d1fae5;color:#065f46}
    .alert-warn{background:#fef3c7;color:#92400e}
  </style>
</asp:Content>

<asp:Content ID="bodyC" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="card">
    <h2>Connect a Mock Bank</h2>
    <p class="muted">One click will create 2 realistic accounts and seed recent transactions for your user only.</p>

    <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert">
      <asp:Label ID="lblMsg" runat="server" />
    </asp:Panel>

    <div class="stack">
      <asp:Button ID="btnSeed" runat="server" Text="Create Mock Accounts"
          CssClass="btn btn-primary" OnClick="btnSeed_Click" />
      <asp:Button ID="btnWipe" runat="server" Text="Remove My Mock Data"
          CssClass="btn btn-danger" OnClick="btnWipe_Click" />
    </div>

    <hr />
    <h4>Your current accounts</h4>
    <asp:Repeater ID="rptAccounts" runat="server">
      <HeaderTemplate><ul></HeaderTemplate>
      <ItemTemplate>
        <li>
          <strong><%# Eval("AccountNickname") %></strong> —
          <%# Eval("BankName") %> — <%# Eval("AccountNumberMasked") %> —
          $<%# string.Format("{0:N2}", Eval("Balance")) %>
        </li>
      </ItemTemplate>
      <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
  </div>
</asp:Content>
