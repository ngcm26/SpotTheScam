<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="IndividualForum.aspx.cs" Inherits="SpotTheScam.User.IndividualForum" %>

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

        .reply-box {
            margin-top: 10px;
            width: 100%;
        }

        .comment {
            margin-bottom: 10px;
            padding: 5px;
            border-bottom: 1px solid #eee;
        }

        .reply-toggle {
            font-size: 0.9em;
        }

        .reply-icon {
            cursor: pointer;
            width: 20px;
            height: 20px;
            align-self: flex-start;
        }

            .reply-icon:hover {
                text-decoration: underline;
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
                Posted by
                <asp:Label ID="lb_username" runat="server" />
                on
                <asp:Label ID="lb_createdAt" runat="server" />
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

                <asp:Repeater ID="rptComments" runat="server" OnItemDataBound="rptComments_ItemDataBound" OnItemCommand="rptComments_ItemCommand">
                    <ItemTemplate>
                        <div class="mb-3 p-2 border rounded bg-white d-flex justify-content-between align-items-start"
                            style='margin-left: <%# Convert.ToInt32(Eval("Level")) * 20 %>px;'>

                            <!-- Left: Comment content -->
                            <div style="flex-grow: 1;">
                                <strong><%# Eval("Username") %></strong>
                                <span class="text-muted small">(<asp:Label ID="lblCommentCreatedAt" runat="server" />)</span>

                                <div><%# Eval("Content") %></div>

                                <asp:Repeater ID="rptReplies" runat="server"></asp:Repeater>

                                <!-- Hidden reply box -->
                                <div class="reply-box" style="display: none; margin-top: 10px;">
                                    <asp:TextBox ID="tb_reply" runat="server"
                                        CssClass="form-control mb-2"
                                        TextMode="MultiLine"
                                        Rows="2"
                                        placeholder="Write your reply..."
                                        Style="width: 100%;" />
                                    <asp:Button ID="btnReply" runat="server"
                                        Text="Post"
                                        CommandArgument='<%# Eval("ReplyId") %>'
                                        OnClick="btnReply_Click"
                                        CssClass="btn btn-sm btn-primary" />
                                </div>
                            </div>

                            <!-- Right: Reply icon -->
                            <div style="margin-left: 10px; cursor: pointer;">
                                <img src='<%# ResolveUrl("~/Images/reply.png") %>'
                                    style="width: 20px; height: 20px;"
                                    alt="Reply"
                                    onclick="toggleReplyBox(this)" />
                            </div>

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
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(document).ready(function () {
            $(".reply-toggle").click(function () {
                var targetId = $(this).data("target");
                $("#reply-box-" + targetId).slideToggle();
            });
        });
        function toggleReplyBox(el) {
            // Find the closest comment container (parent div of both content and icon)
            var commentContainer = el.closest('.d-flex'); // flex container of comment
            if (!commentContainer) return;

            // Find the reply-box inside that container
            var replyBox = commentContainer.querySelector('.reply-box');
            if (!replyBox) return;

            // Toggle display
            if (replyBox.style.display === 'none' || replyBox.style.display === '') {
                replyBox.style.display = 'block';
            } else {
                replyBox.style.display = 'none';
            }
        }


    </script>
</asp:Content>
