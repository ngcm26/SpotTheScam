<%@ Page Title="Add Transaction" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="AddTransactions.aspx.cs"
    Inherits="SpotTheScam.User.AddTransactions" %>

<asp:Content ID="HeadC" ContentPlaceHolderID="head" runat="server">
  <style>
    .card{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:24px;max-width:800px;margin:24px auto;box-shadow:0 1px 3px rgba(0,0,0,.06)}
    .row{display:flex;gap:16px;flex-wrap:wrap}
    .col{flex:1 1 280px}
    .formc{width:100%;padding:10px 12px;border:1px solid #d1d5db;border-radius:8px}
    .label{font-weight:600;margin:8px 0 6px}
    .btn{display:inline-block;padding:12px 18px;border-radius:10px;border:0;font-weight:600;cursor:pointer}
    .btn-primary{background:#10b981;color:#fff}
    .btn-secondary{background:#6b7280;color:#fff}
    .alert{padding:12px 16px;border-radius:10px;margin-bottom:16px}
    .alert-ok{background:#d1fae5;color:#065f46}
    .alert-bad{background:#fee2e2;color:#991b1b}
    .list{margin-top:20px}
    .chip{display:inline-block;padding:2px 8px;border-radius:999px;font-size:12px;border:1px solid #e5e7eb}
  </style>
</asp:Content>

<asp:Content ID="BodyC" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="card">
    <h2>Add Transaction</h2>
    <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert">
      <asp:Label ID="lblMsg" runat="server" />
    </asp:Panel>

    <div class="row">
      <div class="col">
        <div class="label">Account</div>
        <asp:DropDownList ID="ddlAccount" runat="server" CssClass="formc"
          AutoPostBack="true" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" />
      </div>
      <div class="col">
        <div class="label">Type</div>
        <asp:DropDownList ID="ddlType" runat="server" CssClass="formc">
          <asp:ListItem Text="Withdrawal" Value="Withdrawal" Selected="True" />
          <asp:ListItem Text="Deposit" Value="Deposit" />
        </asp:DropDownList>
      </div>
    </div>

    <div class="row">
      <div class="col">
        <div class="label">Amount</div>
        <asp:TextBox ID="txtAmount" runat="server" CssClass="formc" TextMode="Number" />
      </div>
      <div class="col">
        <div class="label">Recipient / Sender (optional)</div>
        <asp:TextBox ID="txtRecipient" runat="server" CssClass="formc" MaxLength="255" />
      </div>
    </div>

    <div class="label">Description</div>
    <asp:TextBox ID="txtDescription" runat="server" CssClass="formc" MaxLength="255" />

    <div style="margin-top:16px">
      <asp:Button ID="btnSubmit" runat="server" Text="Save Transaction"
        CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
      <a href="ConnectBank.aspx" class="btn btn-secondary">Connect Bank</a>
    </div>

    <div class="list">
      <h3 style="margin-top:24px">Recent Transactions (selected account)</h3>
      <asp:Repeater ID="rptRecent" runat="server">
        <HeaderTemplate><ul style="padding-left:18px"></HeaderTemplate>
        <ItemTemplate>
          <li>
            <strong><%# Eval("TransactionDate","{0:dd MMM}") %></strong> —
            <%# Eval("Description") %> —
            <%# Eval("TransactionType") %> $
            <%# string.Format("{0:N2}", Eval("Amount")) %>
            <span class="chip">Bal: $<%# string.Format("{0:N2}", Eval("BalanceAfterTransaction")) %></span>
          </li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
      </asp:Repeater>
    </div>
  </div>
</asp:Content>
