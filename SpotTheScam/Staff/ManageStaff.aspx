<%@ Page Title="Manage Staff" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="ManageStaff.aspx.cs" Inherits="SpotTheScam.Staff.ManageStaff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Staff</title>
    <style>
        body, .users-table, .users-table th, .users-table td, .users-table input, .users-table select, .users-table button { font-family: 'DM Sans', Arial, sans-serif !important; }
        .users-table { width: 100%; border-collapse: separate; border-spacing: 0; background: #fff; font-size: 1rem; border: none; box-shadow: none; }
        .users-table th { background: #C46A1D; color: #fff; font-weight: 500; padding: 12px 10px; border: none; text-align: left; vertical-align: middle; border-radius: 0; }
        .users-table th:first-child, .users-table th:last-child { border-radius: 0; }
        .users-table td { background: #F9FAFB; color: #222; padding: 10px 10px; border-top: 1px solid #eee; vertical-align: middle; border: none; }
        .users-table tr:hover td { background: #f3f0ed; }
        .users-table td:last-child { text-align: center; vertical-align: middle; }

        .action-btn { display: inline-flex; align-items: center; justify-content: center; height: 32px; width: 32px; border-radius: 50%; transition: background 0.15s; }
        .action-btn:focus { outline: none; box-shadow: 0 0 0 2px #C46A1D33; }
        .action-btn i { font-size: 1.1em; font-weight: 400; }
        .action-btn.edit i { color: #051D40 !important; }
        .action-btn.edit:hover i { color: #16305a !important; }
        .action-btn.delete i { color: #DC3545 !important; }
        .action-btn.delete:hover i { color: #a01a1a !important; }
        .users-table .fa { pointer-events: none; }

        .filter-pill, .custom-dropdown, .filter-btn { border-radius: 6px !important; border: 1.5px solid #bfc5ce !important; background: #fff !important; box-shadow: none !important; font-size: 1.08em; color: #222; outline: none; transition: border-color 0.2s; height: 42px; box-sizing: border-box; }
        .filter-pill { padding: 7px 22px 7px 38px !important; min-width: 120px; }
        .filter-pill:hover { border-color: #C46A1D !important; }
        .filter-pill:focus, .filter-pill:active, .custom-dropdown:focus, .custom-dropdown:active, .filter-btn:focus, .filter-btn:active { border-color: #C46A1D !important; outline: none !important; box-shadow: none !important; }
        .filter-icon { position: absolute; left: 16px; top: 50%; transform: translateY(-50%); color: #6c757d; font-size: 1.1em; pointer-events: none; z-index: 10; }
        .filter-group { position: relative; display: inline-block; }

        /* Match Manage Users button styles */
        .filter-btn { color: #bfc5ce !important; font-weight: 400; padding: 7px 28px !important; min-width: 0; transition: border-color 0.2s, background 0.2s, color 0.2s; }
        .filter-btn:hover, .filter-btn:focus { border-color: #C46A1D !important; color: #C46A1D !important; background: #f3f0ed !important; }
        .filter-btn, .filter-pill, .custom-dropdown { color: #585252 !important; }

        .role-staff { background: #ffc107; color: #212529; padding: 2px 14px; border-radius: 16px; font-weight: 400; font-size: 0.95em; display: inline-block; }
        .status-published { background:#28a745; color:#fff; padding:2px 14px; border-radius:16px; font-weight:500; font-size:0.95em; display:inline-block; border:none; }
        .status-draft { background:#dc3545; color:#fff; padding:2px 14px; border-radius:16px; font-weight:500; font-size:0.95em; display:inline-block; border:none; }
        .users-table a.status-published, .users-table a.status-draft { text-decoration:none; color:#fff !important; cursor:pointer; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-bottom: 10px; display:flex; justify-content:space-between; align-items:center;">
        <div>
            <h1 style="color: #C46A1D; font-weight: 600; margin-bottom: 0.2em; font-size: 2.1rem;">Manage Staff</h1>
            <div style="color: #344054; font-size: 1.15rem;">Admins can view and manage staff accounts here.</div>
        </div>
    </div>

    <div class="users-filter-bar" style="display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 18px;">
        <div style="display: flex; gap: 16px; align-items: center; flex-wrap: wrap;">
            <div class="filter-group" style="min-width: 220px;">
                <span class="filter-icon"><i class="fa fa-search"></i></span>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-pill" placeholder="Search staff..." />
            </div>
            <asp:Button ID="btnFilter" runat="server" Text="Apply" CssClass="filter-btn" OnClick="btnFilter_Click" />
            <asp:Button ID="btnClearFilters" runat="server" Text="Clear" CssClass="filter-btn" OnClick="btnClearFilters_Click" />
        </div>
        <a href="AddStaff.aspx" class="filter-btn" style="text-decoration:none;">+ Add Staff</a>
    </div>

    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />

    <asp:GridView ID="gvStaff" runat="server" AutoGenerateColumns="False"
        CssClass="users-table" DataKeyNames="Id" OnRowCommand="gvStaff_RowCommand"
        ShowHeaderWhenEmpty="true">
        <Columns>
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" />
            <asp:TemplateField HeaderText="Role">
                <ItemTemplate>
                    <span class="role-staff">Staff</span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:LinkButton ID="btnToggleStatus" runat="server"
                        CommandName="ToggleStatus"
                        CommandArgument='<%# Eval("Id") + "|" + Eval("Status") %>'
                        CssClass='<%# (Eval("Status").ToString().ToLower()=="active") ? "status-published" : "status-draft" %>'
                        OnClientClick="return confirm('Do you want to suspend this staff account?');"
                        Text='<%# Eval("Status") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditStaff" CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit" ToolTip="Edit">
                        <i class="fa-regular fa-edit"></i>
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteStaff" CommandArgument='<%# Eval("Id") %>' CssClass="action-btn delete" ToolTip="Delete" OnClientClick="return confirm('Delete this staff account?');">
                        <i class="fa-regular fa-trash-can"></i>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
