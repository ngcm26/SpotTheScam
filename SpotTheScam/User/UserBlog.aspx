<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBlog.aspx.cs" Inherits="SpotTheScam.User.UserBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .blog-section {
            background-color: #fff5db;
            padding: 30px;
        }

        .blog-card {
            background-color: white;
            border: 1px solid #e5e5e5;
            border-left: 5px solid #d36f2d;
            border-radius: 8px;
            margin-bottom: 20px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            overflow: hidden;
            transition: transform 0.2s;
        }

        .blog-card:hover {
            transform: translateY(-5px);
        }

        .blog-card img {
            width: 100%;
            height: auto;
            max-height: 250px;
            object-fit: cover;
        }

        .blog-body {
            padding: 20px;
        }

        .blog-title {
            font-size: 1.5rem;
            font-weight: bold;
            color: #d36f2d;
        }

        .blog-author {
            font-size: 0.9rem;
            color: #888;
            margin-bottom: 10px;
        }

        .btn-create {
            float: right;
            background-color: #d36f2d;
            color: white;
            border: none;
        }

        .btn-create:hover {
            background-color: #a7531e;
        }
    </style>
</asp:Content>
    
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="blog-section">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="text-dark">Learn more about online scams through blogs!</h2>
            <asp:Button ID="btn_CreateBlog" CssClass="btn btn-create" runat="server" Text="Create a Blog!" OnClick="btn_CreateBlog_Click" />
        </div>

        <div class="row">
    <asp:Repeater ID="rptBlog" runat="server" OnItemCommand="rptBlog_ItemCommand1">
        <ItemTemplate>
            <div class="col-md-6 col-lg-4 mb-4">
                <a href='<%# "../Staff/IndividualBlogStafff.aspx?postID=" + (int)Eval("post_id") %>' style="text-decoration: none;">
                <div class="blog-card h-100 d-flex flex-column">
                    <asp:Image CssClass="img-fluid" ImageUrl='<%# "~/Uploads/Blog_Pictures/" + Eval("cover_image") %>' runat="server" />
                    <div class="blog-body flex-grow-1">
                        <div class="blog-title"><%# Eval("title") %></div>
                        <div class="blog-author">By <%# Eval("username") %></div>
                        <p><%# Eval("content").ToString().Length > 100 ? Eval("content").ToString().Substring(0, 100) + "..." : Eval("content") %></p>
                    </div>
                </div>
                </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
    </div>
    
</asp:Content>