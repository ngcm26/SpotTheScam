<%@ Page Title="Member Accounts" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="MemberAccounts.aspx.cs"
    Inherits="SpotTheScam.User.MemberAccounts" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap{max-width:1000px;margin:24px auto;padding:0 12px}
        .card{background:#fff;border:1px solid #eee;border-radius:12px;padding:18px;margin-bottom:18px}
        table{width:100%} th,td{padding:10px 8px;border-bottom:1px solid #f2f2f2;text-align:left}
        .muted{color:#6b7280}
        .btn{display:inline-block;padding:8px 12px;border-radius:8px;background:#2563eb;color:#fff;text-decoration:none}
        .msg{padding:10px 12px;border-radius:8px;margin-bottom:12px;font-weight:600}
        .ok{background:#e8f5e9;color:#1b5e20;border:1px solid #c8e6c9}
        .err{background:#ffebee;color:#b71c1c;border:1px solid #ffcdd2}
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="wrap">
    <div class="card">
      <h2>Accounts for <asp:Label ID="lblMember" runat="server" /></h2>
      <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg"><asp:Label ID="lblMsg" runat="server" /></asp:Panel>

      <asp:GridView ID="gvAccounts" runat="server" AutoGenerateColumns="false" GridLines="None">
        <Columns>
          <asp:BoundField DataField="AccountNickname" HeaderText="Nickname" />
          <asp:BoundField DataField="BankName" HeaderText="Bank" />
          <asp:BoundField DataField="AccountNumberMasked" HeaderText="Account #" />
          <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:C}" />
          <asp:TemplateField HeaderText="">
            <ItemTemplate>
              <a class="btn"
                 href='<%# "~/User/MemberTransactionLogs.aspx?groupId=" + Request["groupId"] + "&userId=" + Request["userId"] + "&accountId=" + Eval("AccountId") %>'>
                 View transactions
              </a>
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>

      <div class="muted" style="margin-top:8px">
        Showing all accounts owned by this member.
      </div>

      <div style="margin-top:16px">
        <a class="btn" href='<%# "~/User/ManageGroup.aspx?groupId=" + Request["groupId"] %>'>Back to group</a>
      </div>
    </div>
  </div>
</asp:Content>
