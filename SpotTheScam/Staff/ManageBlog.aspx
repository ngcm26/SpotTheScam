<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="ManageBlog.aspx.cs" Inherits="SpotTheScam.Staff.ManageBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .manage-blog-container {
            max-width: 1100px;
            margin: 30px auto;
            background: #ffffff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
        }
        .manage-blog-header {
            font-size: 1.6rem;
            font-weight: 600;
            margin-bottom: 20px;
        }
        .filter-bar {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 20px;
        }
        .gridview-styled {
            border: none;
            width: 100%;
        }
        .gridview-styled th {
            background-color: #f8f9fa;
            color: #333;
            padding: 12px;
            border-bottom: 2px solid #dee2e6;
            text-align: left;
        }
        .gridview-styled td {
            padding: 10px;
            border-bottom: 1px solid #dee2e6;
            vertical-align: middle;
        }
        .gridview-styled tr:hover {
            background-color: #f1f1f1;
        }
        .btn-sm {
            padding: 5px 10px;
            font-size: 0.85rem;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="manage-blog-container">
        <div class="manage-blog-header">Manage Blog Posts</div>

        <div class="filter-bar">
            <label for="ddlFilter" class="form-label fw-bold">Filter:</label>
            <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged"
                CssClass="form-select w-auto">
                <asp:ListItem Text="All Posts" Value="all" />
                <asp:ListItem Text="Approved Posts" Value="approved" />
                <asp:ListItem Text="Unapproved Posts" Value="unapproved" />
            </asp:DropDownList>
        </div>

        <asp:GridView ID="gv_blog" runat="server" AutoGenerateColumns="False"
            CssClass="gridview-styled table table-hover"
            DataKeyNames="post_id"
            OnDataBinding="gv_blog_DataBinding"
            OnSelectedIndexChanged="gv_blog_SelectedIndexChanged"
            OnRowDeleting="gv_blog_RowDeleting"
            OnRowCommand="gv_blog_RowCommand">
            <Columns>
                <asp:BoundField DataField="post_id" HeaderText="Post ID" />
                <asp:BoundField DataField="user_id" HeaderText="User ID" />
                <asp:BoundField DataField="title" HeaderText="Blog Title" />
                <asp:BoundField DataField="created_at" HeaderText="Created At" />
                <asp:BoundField DataField="isApproved" HeaderText="Approval" />
                <asp:CommandField ShowSelectButton="True" SelectText="View" />
                <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnApprove" runat="server" CommandName="Approve"
                            CommandArgument='<%# Eval("post_id") %>'
                            Text="Approve"
                            CssClass="btn btn-success btn-sm"
                            Visible='<%# Eval("isApproved").ToString() == "0" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>