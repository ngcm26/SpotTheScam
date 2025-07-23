<%@ Page Title="Manage Users" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="SpotTheScam.Staff.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Users</title>
    <style>
        body, .users-table, .users-table th, .users-table td, .users-table input, .users-table select, .users-table button {
            font-family: 'DM Sans', Arial, sans-serif !important;
        }
        .users-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            background: #fff;
            font-size: 1rem;
            border: none;
            box-shadow: none;
        }
        .users-table th {
            background: #C46A1D;
            color: #fff;
            font-weight: 500;
            padding: 12px 10px;
            border: none;
            text-align: left;
            vertical-align: middle;
            border-radius: 0;
        }
        .users-table th:first-child,
        .users-table th:last-child {
            border-radius: 0;
        }
        .users-table td {
            background: #F9FAFB;
            color: #222;
            padding: 10px 10px;
            border-top: 1px solid #eee;
            vertical-align: middle;
            border: none;
        }
        .users-table tr:hover td {
            background: #f3f0ed;
        }
        .action-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            height: 32px;
            width: 32px;
            border-radius: 50%;
            transition: background 0.15s;
        }
        .action-btn:focus {
            outline: none;
            box-shadow: 0 0 0 2px #C46A1D33;
        }
        .action-btn i {
            font-size: 1.1em;
            font-weight: 400;
        }
        .action-btn.edit i {
            color: #051D40 !important;
        }
        .action-btn.edit:hover i {
            color: #16305a !important;
        }
        .action-btn.delete i {
            color: #DC3545 !important;
        }
        .action-btn.delete:hover i {
            color: #a01a1a !important;
        }
        .users-table .fa {
            pointer-events: none;
        }
        .users-table-pager {
            margin-top: 10px;
            text-align: right;
        }
        .users-table td:last-child {
            text-align: center;
            vertical-align: middle;
        }
        .btn-navy {
            background-color: #fff !important;
            border: 1.5px solid #051D40 !important;
            color: #051D40 !important;
            border-radius: 6px !important;
            font-weight: 500;
        }
        .btn-navy:hover, .btn-navy:focus {
            background-color: #f8f9fa !important;
            border-color: #051D40 !important;
            color: #051D40 !important;
        }
        .btn-navy:focus {
            outline: none !important;
            box-shadow: 0 0 0 2px rgba(5, 29, 64, 0.2) !important;
        }
        .btn-navy:active, .btn-navy.active {
            background-color: #e9ecef !important;
            border-color: #051D40 !important;
            color: #051D40 !important;
            box-shadow: none !important;
        }
        .filter-pill, .custom-dropdown, .filter-btn {
            border-radius: 6px !important;
            border: 1.5px solid #bfc5ce !important;
            background: #fff !important;
            box-shadow: none !important;
            font-size: 1.08em;
            color: #222;
            outline: none;
            transition: border-color 0.2s;
            height: 42px;
            box-sizing: border-box;
        }
        .filter-pill {
            padding: 7px 22px 7px 38px !important;
            min-width: 120px;
        }
        .filter-pill:focus, .filter-pill:active,
        .custom-dropdown:focus, .custom-dropdown:active,
        .filter-btn:focus, .filter-btn:active {
            border-color: #C46A1D !important;
            outline: none !important;
            box-shadow: none !important;
        }
        .filter-pill option {
            color: #222;
            background: #fff;
            font-size: 1em;
        }
        .filter-icon {
            position: absolute;
            left: 16px;
            top: 50%;
            transform: translateY(-50%);
            color: #6c757d;
            font-size: 1.1em;
            pointer-events: none;
            z-index: 10;
        }
        .filter-group {
            position: relative;
            display: inline-block;
        }
        /* Ensure all filter dropdowns have consistent styling */
        .filter-group select.filter-pill {
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            background-image: url("data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%236c757d' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6,9 12,15 18,9'%3e%3c/polyline%3e%3c/svg%3e");
            background-repeat: no-repeat;
            background-position: right 12px center;
            background-size: 16px;
            padding: 7px 40px 7px 16px !important;
        }
        /* Consistent hover states */
        .filter-pill:hover {
            border-color: #C46A1D !important;
        }
        /* Ensure consistent focus states */
        .filter-pill:focus-visible {
            outline: 2px solid #C46A1D;
            outline-offset: 2px;
        }
        .filter-btn {
            color: #bfc5ce !important;
            font-weight: 400;
            padding: 7px 28px !important;
            min-width: 0;
            transition: border-color 0.2s, background 0.2s, color 0.2s;
        }
        .filter-btn:hover, .filter-btn:focus {
            border-color: #C46A1D !important;
            color: #C46A1D !important;
            background: #f3f0ed !important;
        }
        .filter-btn, .filter-pill, .custom-dropdown, .btn-navy {
            color: #585252 !important;
        }
        .role-user {
            background: #28a745;
            color: #fff;
            padding: 2px 14px;
            border-radius: 16px;
            font-weight: 400;
            font-size: 0.95em;
            display: inline-block;
        }
        .role-admin {
            background: #dc3545;
            color: #fff;
            padding: 2px 14px;
            border-radius: 16px;
            font-weight: 400;
            font-size: 0.95em;
            display: inline-block;
        }
        .role-staff {
            background: #ffc107;
            color: #212529;
            padding: 2px 14px;
            border-radius: 16px;
            font-weight: 400;
            font-size: 0.95em;
            display: inline-block;
        }
        .form-select {
            width: 100%;
            background: #fff;
            border: 1.5px solid #bfc5ce;
            border-radius: 6px;
            padding: 8px 12px;
            font-size: 1em;
            color: #585252;
            font-weight: 400;
            outline: none;
            transition: border-color 0.2s;
        }
        .form-select:focus {
            border-color: #C46A1D;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-bottom: 10px;">
        <h1 style="color: #C46A1D; font-weight: 600; margin-bottom: 0.2em; font-size: 2.1rem;">Manage Users</h1>
        <div style="color: #344054; font-size: 1.15rem; margin-bottom: 1.2em;">This page allows staff to view and manage user accounts.</div>
    </div>
    <div class="users-filter-bar" style="display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 18px;">
        <div style="display: flex; gap: 16px; align-items: center; flex-wrap: wrap;">
            <div class="filter-group" style="min-width: 220px;">
                <span class="filter-icon"><i class="fa fa-search"></i></span>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-pill" placeholder="Search users..." />
            </div>
            <asp:Button ID="btnFilter" runat="server" Text="Apply" CssClass="filter-btn" OnClick="btnFilter_Click" />
            <asp:Button ID="btnClearFilters" runat="server" Text="Clear" CssClass="filter-btn" OnClick="btnClearFilters_Click" />
        </div>
        <a href="AddUser.aspx" class="filter-btn" style="text-decoration: none;">+ Add User</a>
    </div>
    
    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False"
        DataKeyNames="Id"
        CssClass="users-table"
        OnRowDeleting="gvUsers_RowDeleting"
        OnRowCommand="gvUsers_RowCommand"
        ShowHeaderWhenEmpty="true"
        PagerStyle-CssClass="users-table-pager">

        <Columns>
            <asp:BoundField DataField="Username" HeaderText="Username" ReadOnly="False" />
            <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="False" />
            <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" ReadOnly="False" />
            <asp:TemplateField HeaderText="Role">
                <ItemTemplate>
                    <span class='<%# GetRoleClass(Eval("Role").ToString()) %>'>
                        <%# Eval("Role") %>
                    </span>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select" SelectedValue='<%# Bind("Role") %>'>
                        <asp:ListItem Text="User" Value="user" />
                        <asp:ListItem Text="Admin" Value="admin" />
                        <asp:ListItem Text="Staff" Value="staff" />
                    </asp:DropDownList>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditUser" CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit" ToolTip="Edit">
                        <i class="fa-regular fa-edit"></i>
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="action-btn delete" ToolTip="Delete" OnClientClick="return confirm('Are you sure you want to delete this user?');">
                        <i class="fa-regular fa-trash-can"></i>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
