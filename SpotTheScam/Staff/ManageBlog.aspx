<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="ManageBlog.aspx.cs" Inherits="SpotTheScam.Staff.ManageBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Manage Blog Post<br />
    <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged" CssClass="form-select w-25 mb-3">
        <asp:ListItem Text="All Posts" Value="all" />
        <asp:ListItem Text="Approved Posts" Value="approved" />
        <asp:ListItem Text="Unapproved Posts" Value="unapproved" />
    </asp:DropDownList>
    <asp:GridView ID="gv_blog" runat="server" AutoGenerateColumns="False" DataKeyNames="post_id" OnDataBinding="gv_blog_DataBinding" OnSelectedIndexChanged="gv_blog_SelectedIndexChanged" Width="739px" OnRowDeleting="gv_blog_RowDeleting" OnRowCommand="gv_blog_RowCommand">
        <Columns>
            <asp:BoundField DataField="post_id" HeaderText="Post ID" />
            <asp:BoundField DataField="user_id" HeaderText="User ID" />
            <asp:BoundField DataField="title" HeaderText="Blog Title" />
            <asp:BoundField DataField="created_at" HeaderText="Created At" />
            <asp:BoundField DataField="isApproved" HeaderText="Approval" />
            <asp:CommandField ShowSelectButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
            <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnApprove" runat="server" CommandName="Approve"
                    CommandArgument='<%# Eval("post_id") %>' Text="Approve" CssClass="btn btn-success btn-sm"
                    Visible='<%# Eval("isApproved").ToString() == "0" %>' />
            </ItemTemplate>
        </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <br />
</asp:Content>