<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ForumPage.aspx.cs" Inherits="SpotTheScam.User.ForumPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 1399px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    

    <p>
        <div class="d-flex justify-content-between align-items-center mb-3">
            <input type="text" class="form-control w-50" placeholder="Search" />
            <asp:Button ID="btnToggleForm" runat="server" CssClass="btn btn-primary ms-2" Text="New Post" OnClick="btnToggleForm_Click" />
        </div>

        <!-- Collapsible Form Panel -->
        <asp:Panel ID="pnlNewPost" runat="server" Visible="false" CssClass="card p-4 mb-4 bg-light">
            <div class="mb-2">
                <asp:TextBox ID="tb_title" runat="server" CssClass="form-control" placeholder="Title" />
            </div>
            <div class="mb-2">
                <asp:TextBox ID="tb_content" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="What's on your mind?" />
            </div>
            <div class="mb-3">
                <asp:FileUpload ID="img_forum" runat="server" />
            </div>
            <asp:Button ID="btnSubmitPost" runat="server" CssClass="btn btn-success" Text="Submit Post" OnClick="btnSubmitPost_Click" />
        </asp:Panel>
        <br />
    </p>
    <p>
        <asp:Repeater ID="rptDiscussions" runat="server">
            <ItemTemplate>
                <div class="card mb-3">
                    <div class="card-body">
                        <h5 class="card-title"><%# Eval("title") %></h5>
                        <p class="card-text">
                            <%# Eval("description").ToString().Length > 100 
                            ? Eval("description").ToString().Substring(0, 100) + "..." 
                            : Eval("description").ToString() %>
                        </p>
                        <p class="card-text text-muted">By <%# Eval("username") %> - <%# Eval("created_at") %></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </p>



</asp:Content>
