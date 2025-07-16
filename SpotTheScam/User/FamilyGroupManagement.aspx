<%@ Page Title="Family Group Management" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="FamilyGroupManagement.aspx.cs" Inherits="SpotTheScam.User.FamilyGroupManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Family Group Management</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        .card {
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

        .role-elderly {
            background: #ecc94b;
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
        .role-primary { 
            background: #fbbf24; color: #7c4700; 
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="card">
        <h1 class="text-2xl font-bold mb-4">Family Group Management</h1>
        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-4">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>
        <div class="mb-6">
            <asp:Label CssClass="form-label" AssociatedControlID="txtGroupName" runat="server" Text="Create New Family Group:" />
            <asp:TextBox ID="txtGroupName" runat="server" CssClass="form-input" placeholder="Enter group name..." />
            <asp:Button ID="btnCreateGroup" runat="server" Text="Create Group" CssClass="action-btn" OnClick="btnCreateGroup_Click" />
        </div>
        <asp:Panel ID="pnlGroupInfo" runat="server" Visible="false">
            <h2 class="text-xl font-semibold mb-2">Current Group:
                <asp:Literal ID="ltGroupName" runat="server" /></h2>
            <div class="mb-4">
                <asp:Label CssClass="form-label" AssociatedControlID="txtAddMemberEmail" runat="server" Text="Add Member by Email:" />
                <asp:TextBox ID="txtAddMemberEmail" runat="server" CssClass="form-input" placeholder="Enter member's email..." />
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-input">
                    <asp:ListItem Text="Elderly" Value="elderly" />
                    <asp:ListItem Text="Trusted Family Member" Value="trusted" />
                    <asp:ListItem Text="Admin" Value="admin" />
                </asp:DropDownList>
                <asp:Button ID="btnAddMember" runat="server" Text="Add Member" CssClass="action-btn" OnClick="btnAddMember_Click" />
            </div>
            <h3 class="text-lg font-semibold mb-2">Group Members</h3>
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


        </asp:Panel>
    </div>
</asp:Content>
