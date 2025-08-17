<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="IndividualBlogUser.aspx.cs" Inherits="SpotTheScam.User.IndividualBlogUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
.blog-container {
    max-width: 850px;
    margin: 60px auto;
    background-color: #ffffff; /* clean white */
    padding: 40px;
    border-radius: 12px;
    box-shadow: 0 8px 24px rgba(0,0,0,0.08);
    font-family: 'Segoe UI', Roboto, Arial, sans-serif;
}

.blog-title {
    font-size: 2.2rem;
    color: #222;
    font-weight: 700;
    margin-bottom: 25px;
    line-height: 1.4;
    text-align: center; /* centered title for better focus */
}

.blog-image {
    width: 100%;
    max-height: 480px;
    object-fit: cover;
    border-radius: 10px;
    margin: 0 auto 25px auto;
    display: block;
    box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}

.blog-content {
    font-size: 1.05rem;
    color: #444;
    line-height: 1.8;
    text-align: justify;
    margin-top: 20px;
}

.alert-message {
    text-align: center;
    color: #d9534f; /* Bootstrap danger red */
    font-weight: 600;
    margin-bottom: 20px;
}

.btn-back {
    background-color: #00a2c7; /* brand blue */
    color: #fff;
    font-weight: 600;
    padding: 10px 22px;
    border: none;
    border-radius: 8px;
    transition: background-color 0.3s ease, transform 0.2s ease;
    box-shadow: 0 4px 10px rgba(0, 162, 199, 0.25);
    margin-bottom: 25px;
}

.btn-back:hover {
    background-color: #008eb0;
    transform: translateY(-2px);
    box-shadow: 0 6px 14px rgba(0, 162, 199, 0.3);
}

.btn-back:focus {
    outline: none;
    box-shadow: 0 0 0 3px rgba(0, 162, 199, 0.35);
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
