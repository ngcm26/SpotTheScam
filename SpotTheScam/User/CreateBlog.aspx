<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="CreateBlog.aspx.cs" Inherits="SpotTheScam.User.CreateBlog" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .create-blog-container {
            max-width: 800px;
            margin: 40px auto;
            padding: 30px;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            font-family: 'Segoe UI', sans-serif;
        }

        .form-label {
            font-weight: 600;
            font-size: 1rem;
            margin-bottom: 8px;
            color: #003049;
        }

        .form-input, .form-fileupload {
            width: 100%;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 6px;
            font-size: 1rem;
            margin-bottom: 20px;
        }

            .form-input:focus {
                border-color: #00a2c7;
                outline: none;
                box-shadow: 0 0 5px rgba(0, 162, 199, 0.3);
            }

        .btn-submit, .btn-back {
            background-color: #00a2c7;
            color: #fff;
            font-weight: bold;
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease, transform 0.2s ease;
            margin-right: 10px;
        }

            .btn-submit:hover, .btn-back:hover {
                background-color: #008eb0;
                transform: translateY(-2px);
            }

            .btn-submit:focus, .btn-back:focus {
                outline: none;
                box-shadow: 0 0 0 3px rgba(0, 162, 199, 0.4);
            }

        .btn-area {
            text-align: right;
            margin-top: 20px;
        }

        .page-title {
            font-size: 2rem;
            font-weight: bold;
            text-align: center;
            color: #003049;
            margin-bottom: 30px;
        }
    </style>
    <script src="https://cdn.ckeditor.com/4.21.0/standard/ckeditor.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="create-blog-container">
        <div class="page-title">Create a Blog</div>

        <asp:Button ID="btn_back" CssClass="btn-back" runat="server" Text="← Back" OnClick="btn_back_Click" />

        <div class="form-group mt-3">
            <div class="form-label">Blog Title:</div>
            <asp:TextBox ID="tb_BlogTitle" CssClass="form-input" runat="server" placeholder="Write your blog title..." />
        </div>

        <div class="form-group">
            <div class="form-label">Blog Image:</div>
            <asp:FileUpload ID="blog_FileUpload" CssClass="form-fileupload" runat="server" />
        </div>

        <div class="form-group">
            <div class="form-label">Content:</div>
            <asp:TextBox ID="tb_BlogContent" CssClass="form-input" runat="server" TextMode="MultiLine" Rows="10" Columns="80" placeholder="Write your blog content..." />
        </div>

        <script>
            CKEDITOR.replace('<%= tb_BlogContent.ClientID %>');
        </script>

        <div class="btn-area">
            <asp:Button ID="btn_Publish" CssClass="btn-submit" runat="server" Text="Publish" OnClick="btn_Publish_Click" />
        </div>
    </div>
</asp:Content>
