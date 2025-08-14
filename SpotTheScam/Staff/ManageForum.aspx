<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="ManageForum.aspx.cs" Inherits="SpotTheScam.Staff.ManageForum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Manage Forum</h2>
<p>
    <asp:GridView ID="gv_forum" runat="server" AutoGenerateColumns="False" Width="899px" DataKeyNames="DiscussionId" OnRowDeleting="gv_forum_RowDeleting" OnSelectedIndexChanged="gv_forum_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="DiscussionId" HeaderText="Forum ID" />
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="Title" HeaderText="Title" />
            <asp:BoundField DataField="CreatedAt" HeaderText="Created At" />
            <asp:CommandField ShowSelectButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
    </asp:GridView>
</p>
</asp:Content>
