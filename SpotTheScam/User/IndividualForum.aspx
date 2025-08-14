a<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="IndividualForum.aspx.cs" Inherits="SpotTheScam.User.IndividualForum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .forum-container {
            max-width: 800px;
            margin: 40px auto;
        }

        .forum-card {
            background-color: #fff5db;
            border: 1px solid #e0e0e0;
            border-left: 5px solid #d36f2d;
            border-radius: 8px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
        }

        .forum-title {
            font-size: 2rem;
            font-weight: bold;
            color: #d36f2d;
            margin-bottom: 20px;
        }

        .forum-meta {
            font-size: 0.9rem;
            color: #888;
            margin-bottom: 10px;
        }

        .forum-image {
            max-width: 100%;
            height: auto;
            margin-top: 15px;
            margin-bottom: 15px;
            border-radius: 6px;
        }

        .forum-discussion-box {
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #ccc;
        }

        .btn-back {
            margin-bottom: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    <div class="forum-container">
        <asp:Button ID="btn_backForum" runat="server" CssClass="btn btn-secondary btn-back" Text="Back" OnClick="Button1_Click" />

        <asp:Label ID="Label1" runat="server" ForeColor="Red" />

        <div class="forum-card">
            <div class="forum-meta">
                Posted by <asp:Label ID="lb_username" runat="server" /> on <asp:Label ID="lb_createdAt" runat="server" />
            </div>

            <div class="forum-title">
                <asp:Label ID="lb_title" runat="server" />
            </div>

            <div class="forum-description">
                <asp:Label ID="lb_description" runat="server" />
            </div>

            <asp:Image ID="img_forum" runat="server" CssClass="forum-image" />

            <div class="forum-discussion-box">
                <h4>Discussion</h4>

                <asp:Repeater ID="rptComments" runat="server">
                    <ItemTemplate>
                        <div class="mb-3 p-2 border rounded bg-white">
                            <strong><%# Eval("Username") %></strong> <span class="text-muted small">(<%# Eval("CreatedAt") %>)</span>
                            <div><%# Eval("Content") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <hr />
                <h5>Leave a Comment</h5>
                <asp:TextBox ID="tb_comment" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="3" placeholder="Write your comment..."></asp:TextBox>
                <asp:Button ID="btnSubmitComment" runat="server" Text="Post Comment" CssClass="btn btn-success" OnClick="btnSubmitComment_Click" />
            </div>
        </div>
    </div>

</asp:Content>
