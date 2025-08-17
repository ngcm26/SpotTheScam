<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="IndividualForum.aspx.cs" Inherits="SpotTheScam.User.IndividualForum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* General container */
        .forum-container {
            max-width: 900px;
            margin: 50px auto;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            color: #333;
        }

        /* Forum card */
        .forum-card {
            background-color: #ffffff;
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 30px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }

        /* Title */
        .forum-title {
            font-size: 1.8rem;
            font-weight: 600;
            color: #2c3e50;
            margin-bottom: 15px;
        }

        /* Meta information */
        .forum-meta {
            font-size: 0.9rem;
            color: #7f8c8d;
            margin-bottom: 20px;
        }

        /* Description */
        .forum-description {
            font-size: 1rem;
            line-height: 1.6;
            margin-bottom: 20px;
        }

        /* Image */
        .forum-image {
            max-width: 100%;
            height: auto;
            border-radius: 6px;
            margin-bottom: 25px;
            border: 1px solid #ccc;
        }

        /* Discussion box */
        .forum-discussion-box {
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
        }

        /* Buttons */
        .btn-back, .btn-success, .btn-primary {
            border-radius: 4px;
            font-size: 0.9rem;
        }

        .btn-back {
            margin-bottom: 25px;
        }

        .btn-success {
            background-color: #2980b9;
            border-color: #2980b9;
        }

            .btn-success:hover {
                background-color: #1f618d;
                border-color: #1f618d;
            }

        .btn-primary {
            background-color: #27ae60;
            border-color: #27ae60;
        }

            .btn-primary:hover {
                background-color: #1e8449;
                border-color: #1e8449;
            }

        /* Comments */
        .mb-3 {
            margin-bottom: 15px;
        }

        .border {
            border: 1px solid #e0e0e0;
        }

        .rounded {
            border-radius: 6px;
        }

        .bg-white {
            background-color: #fff;
        }

        .reply-box {
            margin-top: 10px;
        }

        .comment strong {
            color: #2c3e50;
        }

        .comment span.text-muted {
            color: #95a5a6;
            font-size: 0.85rem;
        }

        .comment div {
            margin-top: 5px;
        }

        .reply-icon img {
            width: 18px;
            height: 18px;
            cursor: pointer;
            transition: transform 0.2s;
        }

            .reply-icon img:hover {
                transform: scale(1.1);
            }

        /* Textareas */
        textarea.form-control {
            border-radius: 4px;
            border: 1px solid #ccc;
            padding: 8px;
        }

            textarea.form-control:focus {
                border-color: #2980b9;
                box-shadow: 0 0 3px rgba(41, 128, 185, 0.3);
            }

        /* Headings inside discussion box */
        .forum-discussion-box h4, .forum-discussion-box h5 {
            font-weight: 600;
            color: #2c3e50;
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
                <h5>Join Discussion</h5>
                <div class="comment-box-wrapper">
                    <asp:TextBox ID="tb_comment" runat="server" CssClass="form-control comment-input"
                        TextMode="MultiLine" Rows="1" placeholder="Join Discussion..."></asp:TextBox>
                    <asp:Button ID="btnSubmitComment" runat="server" Text="Send"
                        CssClass="btn btn-success comment-submit" OnClick="btnSubmitComment_Click" />
                </div>

                <style>
                    .comment-box-wrapper {
                        display: flex;
                        align-items: flex-start;
                        gap: 10px;
                        margin-bottom: 20px;
                    }

                    .comment-input {
                        flex-grow: 1;
                        min-height: 40px; /* shorter height */
                        resize: vertical;
                    }

                    .comment-submit {
                        height: 40px; /* match textbox height */
                        padding: 0 15px;
                        align-self: flex-start;
                    }
                </style>
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
