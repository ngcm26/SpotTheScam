<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="ManageBlog.aspx.cs" Inherits="SpotTheScam.Staff.ManageBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Manage Blog Post<br />
    <asp:GridView ID="gv_blog" runat="server" AutoGenerateColumns="False" OnDataBinding="gv_blog_DataBinding" OnSelectedIndexChanged="gv_blog_SelectedIndexChanged" Width="739px">
        <Columns>
            <asp:BoundField DataField="post_id" HeaderText="Post ID" />
            <asp:BoundField DataField="user_id" HeaderText="User ID" />
            <asp:BoundField DataField="title" HeaderText="Blog Title" />
            <asp:BoundField DataField="created_at" HeaderText="Created At" />
            <asp:BoundField DataField="isApproved" HeaderText="Approval" />
            <asp:CommandField ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
    <br />
    <br />
</asp:Content>
