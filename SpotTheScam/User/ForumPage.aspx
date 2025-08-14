<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ForumPage.aspx.cs" Inherits="SpotTheScam.User.ForumPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 1399px;
        }

        .forum-wrapper {
            max-width: 1200px; /* adjust this for tighter or wider layout */
            padding: 0 50px; /* 50px spacing left & right */
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
                <div class="forum-wrapper mx-auto">
                    <div class="card mb-3">
                        <a href='<%# "IndividualForum.aspx?DiscussionId=" + (int)Eval("DiscussionId") %>' style="text-decoration: none; color: inherit;">
                            <div class="row g-0">
                                <!-- Left: Text content -->
                                <div class="col-md-8">
                                    <div class="card-body">
                                        <h5 class="card-title"><%# Eval("Title") %></h5>
                                        <p class="card-text">
                                            <%# Eval("Description").ToString().Length > 100 
                                    ? Eval("Description").ToString().Substring(0, 100) + "..." 
                                    : Eval("Description").ToString() %>
                                        </p>
                                        <p class="card-text text-muted">By <%# Eval("Username") %> - <%# Eval("CreatedAt") %></p>
                                    </div>
                                </div>

                                <!-- Right: Image -->
                                <div class="col-md-4 d-flex align-items-center justify-content-center">
                                    <asp:Image ID="img_forum" runat="server"
                                        CssClass="img-fluid rounded"
                                        Style="max-height: 150px; object-fit: cover;"
                                        ImageUrl='<%# ResolveUrl("~/Uploads/forum_pictures/" + Eval("ImagePath")) %>' />
                                </div>
                            </div>
                        </a>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </p>



</asp:Content>
