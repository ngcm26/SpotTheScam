<%@ Page Title="Manage Group" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="ManageGroup.aspx.cs" Inherits="SpotTheScam.User.ManageGroup" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap { max-width: 1000px; margin: 24px auto; padding: 0 12px }
        .card { background: #fff; border: 1px solid #eee; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,.05); padding: 18px; margin-bottom: 18px }
        h2 { margin: 0 0 8px 0 }

        .row { display: flex; gap: 12px; flex-wrap: wrap }
        .row > div { flex: 1 1 260px }

        label { display: block; font-weight: 600; margin-bottom: 6px }
        input[type=text], select { width: 100%; padding: 10px; border: 1px solid #d1d5db; border-radius: 8px }

        .form-control { width:100%; padding:10px; border:1px solid #d1d5db; border-radius:8px }

        .btn { border: none; border-radius: 8px; padding: 10px 16px; background: #2563eb; color: #fff; cursor: pointer }
        .btn.secondary { background: #6b7280 }
        .btn-primary { background:#2563eb; }
        .btn-danger { background:#dc2626; }
        .btn-link { background: transparent; color:#2563eb; padding:0; border-radius:0; }
        .btn-sm { font-size:.85rem; padding:4px 8px; }

        .msg { padding: 10px 12px; border-radius: 8px; margin-bottom: 12px; font-weight: 600 }
        .ok { background: #e8f5e9; color: #1b5e20; border: 1px solid #c8e6c9 }
        .err { background: #ffebee; color: #b71c1c; border: 1px solid #ffcdd2 }
        .muted { color: #6b7280 }

        .table { width: 100%; border-collapse: collapse }
        .table th, .table td { padding: 10px; border-bottom: 1px solid #eee; text-align: left }

        .tag { padding: 4px 10px; border-radius: 12px; background: #eef2ff; color: #3730a3; font-weight: 600; font-size: .85rem }
        .pill { padding: 4px 10px; border-radius: 12px; background: #f3f4f6; color: #374151; font-size: .85rem }

        .inline-actions a { margin-right: 8px }

        /* tiny utilities so your classes work */
        .flex { display:flex; }
        .gap-2 { gap:8px; }
        .items-end { align-items:flex-end; }
        .flex-1 { flex:1; }
        .mb-3 { margin-bottom:12px; }
        .mt-1 { margin-top: 6px; }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="wrap">

        <div class="card">
            <h2 id="hGroup" runat="server">Group</h2>
            <p class="muted" id="pDesc" runat="server"></p>

            <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
                <asp:Label ID="lblMsg" runat="server" />
            </asp:Panel>

            <!-- Invite existing user by email -->
            <div class="row">
                <div>
                    <label for="<%= txtInviteEmail.ClientID %>">Invite user by email</label>
                    <asp:TextBox ID="txtInviteEmail" runat="server" />
                </div>
                <div>
                    <label for="<%= ddlInviteRole.ClientID %>">As role</label>
                    <asp:DropDownList ID="ddlInviteRole" runat="server">
                        <asp:ListItem Text="Guardian" Value="Guardian" />
                        <asp:ListItem Text="Primary" Value="Primary" />
                    </asp:DropDownList>
                </div>
                <div style="align-self: flex-end">
                    <asp:Button ID="btnInvite" runat="server" Text="Send Invite" CssClass="btn" OnClick="btnInvite_Click" />
                </div>
            </div>
        </div>

        <div class="card">
            <h2>Members</h2>
            <asp:GridView ID="gvMembers" runat="server" AutoGenerateColumns="false" CssClass="table"
                OnRowCommand="gvMembers_RowCommand" OnRowDataBound="gvMembers_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Username" HeaderText="Name" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />

                    <asp:TemplateField HeaderText="Role">
                        <ItemTemplate>
                            <span class="tag"><%# Eval("GroupRole") %></span>
                            <asp:Panel ID="pnlRoleEdit" runat="server" Visible="false" CssClass="mt-1">
                                <asp:DropDownList ID="ddlRoleRow" runat="server">
                                    <asp:ListItem Text="Guardian" Value="Guardian" />
                                    <asp:ListItem Text="Primary" Value="Primary" />
                                </asp:DropDownList>
                                <asp:LinkButton ID="btnChangeRole" runat="server" Text="Save"
                                    CommandName="ChangeRole" CommandArgument='<%# Eval("UserId") %>' CssClass="btn btn-link btn-sm" />
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate><span class="pill"><%# Eval("Status") %></span></ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <span class="inline-actions">
                                <asp:HyperLink ID="hlView" runat="server" Text="View Accounts" />
                                <asp:LinkButton ID="lnkRestrict" runat="server" CommandName="Restrict"
                                    CommandArgument='<%# Eval("UserId") %>' Text="Set Restrictions" />
                                <asp:LinkButton ID="lnkRemove" runat="server" CommandName="Remove"
                                    CommandArgument='<%# Eval("UserId") %>' Text="Remove"
                                    OnClientClick="return confirm('Remove this member from the group?');" />
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <!-- Restrictions editor (appears when you click Set Restrictions) -->
        <asp:Panel ID="pnlRestrict" runat="server" Visible="false" CssClass="card">
            <h2>Set Restrictions for
                <asp:Label ID="lblEditingUser" runat="server" /></h2>

            <div class="row">
                <div>
                    <label for="<%= txtSingleLimit.ClientID %>">Single Transaction Limit ($)</label>
                    <asp:TextBox ID="txtSingleLimit" runat="server" />
                    <div class="muted">Leave blank for no per-transaction limit.</div>
                </div>
                <div>
                    <label for="<%= txtDailyLimit.ClientID %>">Daily Limit ($)</label>
                    <asp:TextBox ID="txtDailyLimit" runat="server" />
                    <div class="muted">Leave blank for no daily limit.</div>
                </div>
            </div>

            <div class="row">
                <div>
                    <label for="<%= ddlStartHour.ClientID %>">Allowed Start Time</label>
                    <asp:DropDownList ID="ddlStartHour" runat="server" />
                </div>
                <div>
                    <label for="<%= ddlEndHour.ClientID %>">Allowed End Time</label>
                    <asp:DropDownList ID="ddlEndHour" runat="server" />
                </div>
            </div>

            <div style="margin-top: 12px">
                <asp:Button ID="btnSaveRestrictions" runat="server" Text="Save Restrictions" CssClass="btn" OnClick="btnSaveRestrictions_Click" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn secondary" OnClick="btnCancelEdit_Click" />
            </div>
        </asp:Panel>

        <!-- Owner-only group meta editor -->
        <asp:Panel ID="pnlOwner" runat="server" Visible="false" CssClass="mb-3">
            <div class="flex gap-2 items-end">
                <div>
                    <label>Group name</label>
                    <asp:TextBox ID="txtGroupName" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="flex-1">
                    <label>Description</label>
                    <asp:TextBox ID="txtGroupDesc" runat="server" CssClass="form-control" MaxLength="250" />
                </div>
                <asp:Button ID="btnSaveGroupMeta" runat="server" Text="Save" CssClass="btn btn-primary"
                    OnClick="btnSaveGroupMeta_Click" />
                <asp:Button ID="btnDeleteGroup" runat="server" Text="Delete Group" CssClass="btn btn-danger"
                    OnClientClick="return confirm('Delete this group? This cannot be undone.');"
                    OnClick="btnDeleteGroup_Click" />
            </div>
        </asp:Panel>

    </div>
</asp:Content>
