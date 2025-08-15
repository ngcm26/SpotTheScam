<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="IndividualBlogUser.aspx.cs" Inherits="SpotTheScam.User.IndividualBlogUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    .blog-container {
        max-width: 900px;
        margin: 50px auto;
        background-color: #fff5db;
        padding: 30px;
        border-radius: 10px;
        box-shadow: 0 0 15px rgba(0,0,0,0.1);
    }

    .blog-title {
        font-size: 2rem;
        color: #d36f2d;
        font-weight: bold;
        margin-bottom: 20px;
    }

    .blog-image {
        width: 100%;
        height: auto;
        max-height: 450px;
        object-fit: cover;
        border-radius: 10px;
        margin-bottom: 20px;
    }

    .blog-content {
        font-size: 1.1rem;
        color: #333;
        line-height: 1.8;
    }

    .alert-message {
        text-align: center;
        color: red;
        margin-bottom: 20px;
    }
    .btn-back {
        background-color: #6c757d; /* Muted gray */
        color: #fff;
        font-weight: 600;
        padding: 10px 20px;
        border: none;
        border-radius: 8px;
        transition: background-color 0.3s ease, transform 0.2s ease;
        box-shadow: 0 4px 8px rgba(108, 117, 125, 0.2); /* subtle gray */
    }

    .btn-back:hover {
        background-color: #5a6268;
        transform: translateY(-2px);
        box-shadow: 0 6px 12px rgba(108, 117, 125, 0.3);
    }

    .btn-back:focus {
        outline: none;
        box-shadow: 0 0 0 3px rgba(108, 117, 125, 0.4);
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />

    <div class="blog-container">
        <asp:Button ID="btn_back" CssClass="btn-back" runat="server" Text="Back" OnClick="btn_back_Click" />

    <asp:Label ID="Label1" runat="server" CssClass="alert-message" />

    <h2 class="blog-title">
        <asp:Label ID="lbl_title" runat="server"></asp:Label>
    </h2>

    <asp:Image ID="img_cover" runat="server" CssClass="blog-image" />

    <div class="blog-content">
        <asp:Literal ID="lbl_content" runat="server" Mode="PassThrough" />
    </div>
</div>
</asp:Content>
