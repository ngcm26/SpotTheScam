<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilySidebar.ascx.cs" Inherits="SpotTheScam.User.FamilySidebar" %>
<div class="sidebar-card">
    <h4 class="mb-3 text-center">Your Groups</h4>
    <asp:Repeater ID="rptGroups" runat="server">
        <ItemTemplate>
            <asp:LinkButton ID="lnkSelectGroup" runat="server" CommandArgument='<%# Eval("GroupId") %>' OnClick="lnkSelectGroup_Click"
                CssClass='<%# (Eval("GroupId").ToString() == Eval("CurrentGroupId", "{0}") ? "sidebar-group-link active" : "sidebar-group-link") %>'>
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
    <div class="mt-4">
        <a href="FamilyGroupInvites.aspx" class="btn btn-warning w-100">
            Pending Invites
            <% if (PendingInvitesCount > 0) { %>
                <span class="badge badge-danger ml-2"><%= PendingInvitesCount %></span>
            <% } %>
        </a>
    </div>
</div>
