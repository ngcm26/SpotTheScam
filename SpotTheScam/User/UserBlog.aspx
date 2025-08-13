<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBlog.aspx.cs" Inherits="SpotTheScam.User.UserBlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .blog-card {
            width: 100%;
            max-width: 320px;
            border: 1px solid #ddd;
            border-radius: 12px;
            background-color: #fff;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);
            overflow: hidden;
            transition: transform 0.2s;
            height: 350px;
        }

            .blog-card:hover {
                transform: translateY(-5px);
            }

            .blog-card img {
                width: 100%;
                height: 160px;
                object-fit: cover;
            }

        .blog-body {
            padding: 14px;
        }

        .blog-title {
            font-size: 1rem;
            font-weight: bold;
            margin-bottom: 4px;
            color: #333;
        }

        .blog-author {
            font-size: 0.8rem;
            color: #00a2c7;
            float: right;
            margin-top: -18px;
        }

        .blog-snippet {
            font-size: 0.85rem;
            color: #555;
            margin-top: 10px;
            margin-bottom: 6px;
        }

        .read-more {
            font-size: 0.75rem;
            text-decoration: underline;
            color: #333;
            float: right;
        }

        .btn-create {
            background-color: #00a2c7;
            color: white;
            font-weight: 600;
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            transition: background-color 0.3s ease, transform 0.2s ease;
            box-shadow: 0 4px 8px rgba(0, 162, 199, 0.2);
        }

            .btn-create:hover {
                background-color: #008eb0;
                transform: translateY(-2px);
                box-shadow: 0 6px 12px rgba(0, 162, 199, 0.3);
            }

            .btn-create:focus {
                outline: none;
                box-shadow: 0 0 0 3px rgba(0, 162, 199, 0.4);
            }

        .blog-header {
            font-size: 2.2rem; /* Large and readable */
            font-weight: 700; /* Bold for emphasis */
            color: #003049; /* Dark blue or brand color */
            text-align: center; /* Centered horizontally */
            margin: 40px 0 30px 40px; /* Top & bottom spacing */
            font-family: 'Segoe UI', sans-serif;
        }

        .blog-container {
            display: flex;
            flex-wrap: wrap;
            gap: 30px; /* spacing between cards */
            justify-content: center; /* or space-between / flex-start */
            padding: 0 200px; /* 200px spacing from sides */
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="blog-section">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="blog-header">Learn more about online scams through blogs!</h2>
            <div class="d-flex justify-content-end pe-5 mt-3">
                <asp:Button ID="btn_CreateBlog" CssClass="btn btn-create" runat="server" Text="Create a Blog!" OnClick="btn_CreateBlog_Click" />
            </div>
        </div>

        <!-- NEW blog-container with blog cards side-by-side -->
        <div class="blog-container">
            <asp:Repeater ID="rptBlog" runat="server" OnItemCommand="rptBlog_ItemCommand1">
                <ItemTemplate>
                    <a href='<%# "IndividualBlogUser.aspx?postID=" + Eval("post_id") %>' style="text-decoration: none;">
                        <div class="blog-card">
                            <asp:Image CssClass="img-fluid" ImageUrl='<%# "~/Uploads/Blog_Pictures/" + Eval("cover_image") %>' runat="server" />
                            <div class="blog-body">
                                <div class="blog-title"><%# Eval("title") %></div>
                                <div class="blog-author">By <%# Eval("username") %></div>
                                <div class="blog-snippet">
                                    <%# Eval("content").ToString().Length > 100 ? Eval("content").ToString().Substring(0, 100) + "..." : Eval("content") %>
                                </div>
                                <div class="read-more">Read more...</div>
                            </div>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
