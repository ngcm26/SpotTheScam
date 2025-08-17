<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBlog.aspx.cs" Inherits="SpotTheScam.User.UserBlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .blog-card {
    width: 100%;
    max-width: 380px; /* bigger than 320px */
    border: 1px solid #e0e0e0;
    border-radius: 16px; /* more modern rounded corners */
    background-color: #fff;
    box-shadow: 0 6px 14px rgba(0, 0, 0, 0.08);
    overflow: hidden;
    transition: transform 0.25s ease, box-shadow 0.25s ease;
    height: 420px; /* taller to give more breathing room */
    display: flex;
    flex-direction: column;
}

.blog-card:hover {
    transform: translateY(-6px);
    box-shadow: 0 10px 20px rgba(0, 0, 0, 0.12);
}

.blog-card img {
    width: 100%;
    height: 200px; /* taller image for stronger visual impact */
    object-fit: cover;
}

.blog-body {
    flex: 1; /* fills remaining space */
    padding: 18px 20px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
}

.blog-title {
    font-size: 1.2rem; /* slightly larger title */
    font-weight: 700;
    margin-bottom: 8px;
    color: #212529;
    line-height: 1.4;
}

.blog-author {
    font-size: 0.9rem;
    color: #00a2c7;
    font-weight: 600;
    margin-bottom: 10px;
}

.blog-snippet {
    font-size: 0.95rem;
    color: #555;
    line-height: 1.5;
    margin-bottom: 15px;
    flex-grow: 1;
}

.read-more {
    font-size: 0.85rem;
    font-weight: 600;
    text-decoration: none;
    color: #00a2c7;
    align-self: flex-end;
    transition: color 0.2s;
}

.read-more:hover {
    color: #008eb0;
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
            margin-bottom: 100px;
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
