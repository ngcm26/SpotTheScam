<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ForumPage.aspx.cs" Inherits="SpotTheScam.User.ForumPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        body {
        background-color: #f8f9fa; /* A very light gray for a clean background */
        font-family: 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
    }

    .forum-wrapper {
        max-width: 1200px;
        padding: 0 15px; /* Adjust padding for better mobile responsiveness */
        margin: auto; /* Center the wrapper on the page */
    }

    /* Styling for the New Post button container */
    .btn-new-post-container {
        display: flex;
        justify-content: flex-end; /* This aligns the button to the right */
        margin-bottom: 20px;
    }

    /* The new post form panel */
    .card.p-4.mb-4.bg-light {
        background-color: #ffffff !important; /* White background for the card */
        border: 1px solid #e0e0e0; /* Subtle border */
        border-radius: 8px; /* Slightly rounded corners */
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05); /* Soft shadow for depth */
        padding: 24px !important;
    }

    /* Styling the text boxes inside the form */
    .form-control {
        border-radius: 5px;
        border: 1px solid #ced4da;
        padding: 10px 15px;
        font-size: 16px;
    }

    /* Styling for the discussion list cards */
    .card.mb-3 {
        transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
        border: 1px solid #e9ecef;
        border-radius: 8px;
        overflow: hidden; /* Ensures the image and content stay within the border radius */
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
        /* This ensures the image section and text section have a consistent height */
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: #f1f3f5;
    }

    /* Styling for the repeater images */
    .img-fluid.rounded {
        max-height: 150px;
        object-fit: cover;
        width: 100%; /* Make image take full width of its container */
        height: 100%;
        border-radius: 0 8px 8px 0 !important;
    }
    
    /* Ensure the button is styled correctly */
    .btn-new-post {
        margin-bottom: 10px;
        margin-right: 15%;
        margin-top: 30px;
    }



    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- New Post button container -->
    <div class="btn-new-post-container">
        <asp:Button ID="btnToggleForm" runat="server" CssClass="btn btn-primary btn-new-post" Text="New Post" OnClick="btnToggleForm_Click" />
    </div>

    <!-- Collapsible Form Panel (appears below button) -->
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

    <!-- Forum list -->
    <asp:Repeater ID="rptDiscussions" runat="server" OnItemDataBound="rptDiscussions_ItemDataBound">
        <ItemTemplate>
            <div class="forum-wrapper mx-auto">
                <div class="card mb-3">
                    <a href='<%# "IndividualForum.aspx?DiscussionId=" + Eval("DiscussionId") %>' style="text-decoration: none; color: inherit;">
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
                                    <p class="card-text text-muted">
                                        By <%# Eval("Username") %> -
                                        <asp:Label ID="lblCreatedAt" runat="server" />
                                    </p>
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

</asp:Content>
