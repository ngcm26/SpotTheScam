<%@ Page Title="My Groups" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="MyGroups.aspx.cs" Inherits="SpotTheScam.User.MyGroups" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap{max-width:1000px;margin:24px auto;padding:0 12px}
        .card{background:#fff;border:1px solid #eee;border-radius:12px;box-shadow:0 2px 8px rgba(0,0,0,.05);padding:18px;margin-bottom:18px}
        h2{margin:0}
        .header-row{display:flex;justify-content:space-between;align-items:center;margin-bottom:12px}
        .btn{border:none;border-radius:8px;padding:10px 16px;background:#2563eb;color:#fff;cursor:pointer;text-decoration:none;display:inline-block}
        .btn.secondary{background:#6b7280}
        .msg{padding:10px 12px;border-radius:8px;margin-bottom:12px;font-weight:600}
        .ok{background:#e8f5e9;color:#1b5e20;border:1px solid #c8e6c9}
        .err{background:#ffebee;color:#b71c1c;border:1px solid #ffcdd2}
        .muted{color:#6b7280}
        .table{width:100%;border-collapse:collapse}
        .table th,.table td{padding:10px;border-bottom:1px solid #eee;text-align:left}
        .tag{padding:4px 10px;border-radius:12px;background:#eef2ff;color:#3730a3;font-weight:600;font-size:.85rem}
        .pill{padding:4px 10px;border-radius:12px;background:#f3f4f6;color:#374151;font-size:.85rem}
        .empty{padding:16px;border:1px dashed #ddd;border-radius:12px;text-align:center}
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="wrap">
    <div class="card">
        <div class="header-row">
            <h2>My Groups</h2>
            <asp:Button ID="btnCreateGroup" runat="server" Text="Create Group" CssClass="btn" OnClick="btnCreateGroup_OnClick" />
        </div>

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
            <asp:Label ID="lblMsg" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty">
            You’re not in any groups yet. Click <strong>Create Group</strong> to start one.
        </asp:Panel>

        <asp:GridView ID="gvGroups" runat="server" AutoGenerateColumns="false" CssClass="table"
                      OnRowDataBound="gvGroups_RowDataBound" Visible="false">
            <Columns>
                <asp:TemplateField HeaderText="Group">
                    <ItemTemplate>
                        <a href='<%# "ManageGroup.aspx?groupId=" + Eval("GroupId") %>'><%# Eval("Name") %></a>
                        <div class="muted"><%# Eval("Description") %></div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Your Role">
                    <ItemTemplate>
                        <span runat="server" id="spanRole" class="tag"><%# Eval("GroupRole") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span runat="server" id="spanStatus" class="pill"><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a class="btn secondary" href='<%# "ManageGroup.aspx?groupId=" + Eval("GroupId") %>'>Manage</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>
</asp:Content>
