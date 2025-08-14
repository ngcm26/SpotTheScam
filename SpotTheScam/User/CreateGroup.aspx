<%@ Page Title="Create Family Group" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="CreateGroup.aspx.cs" Inherits="SpotTheScam.User.CreateGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card { max-width:720px;margin:24px auto;padding:24px;background:#fff;border-radius:12px;
                border:1px solid #eee;box-shadow:0 2px 8px rgba(0,0,0,.05);}
        .form-row { margin-bottom:14px; }
        .btn { padding:10px 16px;border:none;border-radius:8px;background:#2563eb;color:#fff;cursor:pointer; }
        .btn:disabled { opacity:.5;cursor:not-allowed; }
        .msg { padding:12px 14px;border-radius:8px;margin-bottom:14px;font-weight:600; }
        .msg.ok { background:#e8f5e9; color:#1b5e20; border:1px solid #c8e6c9;}
        .msg.err{ background:#ffebee; color:#b71c1c; border:1px solid #ffcdd2;}
        label { font-weight:600;display:block;margin-bottom:6px;}
        input[type=text], textarea { width:100%;padding:10px;border-radius:8px;border:1px solid #d1d5db; }
        textarea { min-height:100px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="card">
        <h2>Create Family Group</h2>

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
            <asp:Label ID="lblMsg" runat="server" />
        </asp:Panel>

        <div class="form-row">
            <label for="<%= txtName.ClientID %>">Group Name</label>
            <asp:TextBox ID="txtName" runat="server" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server"
                ControlToValidate="txtName" ErrorMessage="Group name is required"
                Display="Dynamic" ForeColor="Red" />
        </div>

        <div class="form-row">
            <label for="<%= txtDesc.ClientID %>">Description (optional)</label>
            <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" MaxLength="250" />
        </div>

        <asp:Button ID="btnCreate" runat="server" Text="Create Group" CssClass="btn"
            OnClick="btnCreate_Click" />
    </div>
</asp:Content>
