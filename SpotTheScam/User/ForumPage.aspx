<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ForumPage.aspx.cs" Inherits="SpotTheScam.User.ForumPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        body {
        background-color: #f8f9fa; /* A very light gray for a clean background */
        font-family: 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
    }

    .forum-wrapper {
        max-width: 1200px;
        padding: 0 15px; 
        margin: auto; 
    }


    .btn-new-post-container {
        display: flex;
        justify-content: flex-end; 
        margin-bottom: 20px;
    }

    /* The new post form panel */
    .card.p-4.mb-4.bg-light {
        background-color: #ffffff !important; 
        border: 1px solid #e0e0e0; 
        border-radius: 8px; 
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05); 
        padding: 24px !important;
    }

    .form-control {
        border-radius: 5px;
        border: 1px solid #ced4da;
        padding: 10px 15px;
        font-size: 16px;
    }

    .card.mb-3 {
        transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
        border: 1px solid #e9ecef;
        border-radius: 8px;
        overflow: hidden; 
    }

    .card.mb-3:hover {
        transform: translateY(-5px);
        box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
    }

    .card a {
        text-decoration: none;
        color: inherit;
    }

    .card-body {
        padding: 20px;
    }

    .card-title {
        font-weight: 600;
        color: #343a40;
        margin-bottom: 0.5rem;
    }

    .card-text {
        color: #6c757d;
        font-size: 1rem;
    }

    .card-text.text-muted {
        font-size: 0.875rem;
        margin-top: 10px;
    }

    .card .row.g-0 {
        align-items: stretch;
    }

    .card .col-md-4 {
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: #f1f3f5;
    }


    .img-fluid.rounded {
        max-height: 150px;
        object-fit: cover;
        width: 100%; 
        height: 100%;
        border-radius: 0 8px 8px 0 !important;
    }
    

    .btn-new-post {
        margin-bottom: 10px;
        margin-right: 15%;
        margin-top: 30px;
    }



    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert alert-success d-block mb-3" />


    <div class="btn-new-post-container">
        <asp:Button ID="btnToggleForm" runat="server" CssClass="btn btn-primary btn-new-post" Text="New Post" OnClick="btnToggleForm_Click" />
    </div>


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

    <asp:Repeater ID="rptDiscussions" runat="server" OnItemDataBound="rptDiscussions_ItemDataBound">
        <ItemTemplate>
            <div class="forum-wrapper mx-auto">
                <div class="card mb-3">
                    <a href='<%# "IndividualForum.aspx?DiscussionId=" + Eval("DiscussionId") %>' style="text-decoration: none; color: inherit;">
                        <div class="row g-0">
                            <div class="col-md-8">
                                <div class="card-body">
                                    <h5 class="card-title"><%# Eval("Title") %></h5>
                                    <p class="card-text">
                                        <%# Eval("Description").ToString().Length > 100 
                                            ? Eval("Description").ToString().Substring(0, 100) + "..." 
                                            : Eval("Description").ToString() %>
                                    </p>
                                    <p class="card-text text-muted">
                                        By <%# Eval("Username") %> -
                                        <asp:Label ID="lblCreatedAt" runat="server" />
                                    </p>
                                </div>
                            </div>

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

</asp:Content>
