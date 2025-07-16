<%@ Page Title="Family Group Management" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="FamilyGroupManagement.aspx.cs" Inherits="SpotTheScam.User.FamilyGroupManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Family Group Management</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        .sidebar-card {
            background: #fff;
            border-radius: 0.75rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            padding: 1.5rem;
            margin-bottom: 2rem;
        }
        .main-card {
            background: #fff;
            border-radius: 0.75rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            padding: 2rem;
            margin-bottom: 2rem;
        }
        .form-label {
            font-weight: 600;
            color: #374151;
        }
        .form-input {
            padding: 0.75rem;
            border: 1px solid #d1d5db;
            border-radius: 0.5rem;
            width: 100%;
            margin-bottom: 1rem;
        }
        .action-btn {
            background-color: #4f46e5;
            color: #fff;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            margin-right: 1rem;
            border: none;
            cursor: pointer;
            transition: background-color 0.2s;
        }
        .action-btn:hover {
            background-color: #4338ca;
        }
        .danger-btn {
            background-color: #e53e3e;
        }
        .danger-btn:hover {
            background-color: #c53030;
        }
        .alert {
            padding: 1rem;
            border-radius: 0.5rem;
            margin-bottom: 1rem;
            font-weight: 500;
        }
        .alert-success {
            background: #d1fae5;
            color: #065f46;
        }
        .alert-danger {
            background: #fee2e2;
            color: #991b1b;
        }
        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 1rem;
        }
        .table th, .table td {
            padding: 0.75rem;
            border-bottom: 1px solid #e5e7eb;
            text-align: left;
        }
        .table th {
            background: #f3f4f6;
            font-weight: 600;
        }
        .role-badge {
            padding: 0.25rem 0.75rem;
            border-radius: 0.5rem;
            font-size: 0.9rem;
        }
        .role-primary {
            background: #fbbf24;
            color: #7c4700;
        }
        .role-trusted {
            background: #4fd1c5;
            color: #234e52;
        }
        .role-admin {
            background: #4f46e5;
            color: #fff;
        }
        .sidebar-group-link {
            display: block;
            padding: 0.75rem 1rem;
            border-radius: 0.5rem;
            margin-bottom: 0.5rem;
            background: #f3f4f6;
            color: #374151;
            font-weight: 600;
            text-decoration: none;
            transition: background 0.2s, color 0.2s;
        }
        .sidebar-group-link.active, .sidebar-group-link:hover {
            background: #4f46e5;
            color: #fff;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <!-- Sidebar -->
        <div class="col-md-3">
            <div class="sidebar-card">
                <h4 class="mb-3 text-center">Your Groups</h4>
                <asp:Repeater ID="rptGroups" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkSelectGroup" runat="server" CommandArgument='<%# Eval("GroupId") %>' CssClass='<%# (Eval("GroupId").ToString() == Eval("CurrentGroupId", "{0}") ? "sidebar-group-link active" : "sidebar-group-link") %>' OnClick="lnkSelectGroup_Click">
                            <strong><%# Eval("GroupName") %></strong><br />
                            <span class="text-muted" style="font-size:0.9em;">Created: <%# Eval("DateCreated", "{0:d}") %></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
                <hr />
                <div class="mt-3">
                    <asp:Label CssClass="form-label" AssociatedControlID="txtGroupNameSidebar" runat="server" Text="Create New Family Group:" />
                    <asp:TextBox ID="txtGroupNameSidebar" runat="server" CssClass="form-input" placeholder="Enter group name..." />
                    <asp:Button ID="btnCreateGroupSidebar" runat="server" Text="Create Group" CssClass="action-btn w-100" OnClick="btnCreateGroupSidebar_Click" />
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="col-md-9">
            <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-4">
                <asp:Label ID="AlertMessage" runat="server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlGroupDetails" runat="server" Visible="false">
                <div class="main-card mb-4">
                    <h2 class="mb-2"><asp:Literal ID="ltGroupNameSidebar" runat="server" /></h2>
                    <p class="mb-2 text-muted">Created: <asp:Literal ID="ltGroupCreatedSidebar" runat="server" /></p>
                    <p class="mb-2">Members: <asp:Literal ID="ltGroupMemberCount" runat="server" /></p>
                </div>
                <!-- Tabs -->
                <ul class="nav nav-tabs mb-3" id="groupTab" role="tablist">
                    <li class="nav-item" role="presentation">
                        <a class="nav-link active" id="overview-tab" data-bs-toggle="tab" href="#overview" role="tab">Overview</a>
                    </li>
                    <li class="nav-item" role="presentation">
                        <a class="nav-link" id="members-tab" data-bs-toggle="tab" href="#members" role="tab">Members</a>
                    </li>
                    <li class="nav-item" role="presentation">
                        <a class="nav-link" id="settings-tab" data-bs-toggle="tab" href="#settings" role="tab">Settings</a>
                    </li>
                </ul>
                <div class="tab-content" id="groupTabContent">
                    <div class="tab-pane fade show active" id="overview" role="tabpanel">
                        <div class="main-card">
                            <h4>Group Overview</h4>
                            <p>Short description or summary can go here.</p>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="members" role="tabpanel">
                        <div class="main-card">
                            <h4>Group Members</h4>
                            <asp:GridView ID="gvMembers" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" OnRowCommand="gvMembers_RowCommand" OnRowDataBound="gvMembers_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="Username" HeaderText="Username" />
                                    <asp:BoundField DataField="Email" HeaderText="Email" />
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <%# 
                                                Eval("Role").ToString() == "primary" ? "<span class='role-badge role-primary'>Primary Account Holder</span>" :
                                                Eval("Role").ToString() == "trusted" ? "<span class='role-badge role-trusted'>Trusted Family Member</span>" :
                                                "<span class='role-badge role-admin'>Admin</span>"
                                            %>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList ID="ddlEditRole" runat="server">
                                                <asp:ListItem Text="Primary Account Holder" Value="primary" />
                                                <asp:ListItem Text="Trusted Family Member" Value="trusted" />
                                                <asp:ListItem Text="Admin" Value="admin" />
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:Button ID="btnEdit" runat="server" Text="Edit Role" CommandName="EditRole" CommandArgument='<%# Eval("UserId") %>' CssClass="action-btn" />
                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="action-btn danger-btn" CommandArgument='<%# Eval("UserId") %>' OnClick="btnRemoveMember_Click" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Button ID="btnSave" runat="server" Text="Save" CommandName="SaveRole" CommandArgument='<%# Eval("UserId") %>' CssClass="action-btn" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="CancelEdit" CssClass="action-btn danger-btn" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div class="mt-4">
                                <asp:Label CssClass="form-label" AssociatedControlID="txtAddMemberEmail" runat="server" Text="Add Member by Email:" />
                                <asp:TextBox ID="txtAddMemberEmail" runat="server" CssClass="form-input" placeholder="Enter member's email..." />
                                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-input">
                                    <asp:ListItem Text="Primary Account Holder" Value="primary" />
                                    <asp:ListItem Text="Trusted Family Member" Value="trusted" />
                                    <asp:ListItem Text="Admin" Value="admin" />
                                </asp:DropDownList>
                                <asp:Button ID="btnAddMember" runat="server" Text="Add Member" CssClass="action-btn" OnClick="btnAddMember_Click" />
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="settings" role="tabpanel">
                        <div class="main-card">
                            <h4>Group Settings</h4>
                            <div class="mb-2">
                                <asp:Label CssClass="form-label" AssociatedControlID="txtRenameGroup" runat="server" Text="Rename Group:" />
                                <asp:TextBox ID="txtRenameGroup" runat="server" CssClass="form-input" />
                                <asp:Button ID="btnRenameGroup" runat="server" Text="Rename" CssClass="action-btn" OnClick="btnRenameGroup_Click" />
                            </div>
                            <div>
                                <asp:Button ID="btnDeleteGroup" runat="server" Text="Delete Group" CssClass="action-btn danger-btn" OnClick="btnDeleteGroup_Click" OnClientClick="return confirm('Are you sure you want to delete this group? This action cannot be undone.');" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlNoGroupSelected" runat="server" Visible="false">
                <div class="main-card p-4 text-center">
                    <h4>Select a group from the sidebar to view details.</h4>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
