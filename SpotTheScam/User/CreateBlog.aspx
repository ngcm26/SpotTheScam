<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="CreateBlog.aspx.cs" Inherits="SpotTheScam.User.CreateBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style3 {
            width: 1052px;
            height: 15px;
        }
        .auto-style4 {
            height: 15px;
        }
    </style>
    <script src="https://cdn.ckeditor.com/4.21.0/standard/ckeditor.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="text-start">
        &nbsp;<asp:Button ID="btn_back" runat="server" Text="Back" />
        <table class="w-100">
            <tr>
                <td class="auto-style3">Blog Title:</td>
                <td class="auto-style4">
                    <asp:Button ID="btn_Publish" runat="server" Text="Publish" OnClick="btn_Publish_Click" />
                </td>
            </tr>
        </table>
        <asp:TextBox ID="tb_BlogTitle" runat="server" OnTextChanged="TextBox3_TextChanged" Width="288px"></asp:TextBox>
        <br />
        <br />
        <br />
        Blog Image:<br />
        <asp:FileUpload ID="blog_FileUpload" runat="server" />
        <br />
        <br />
        <br />
        Content:<br />
        <asp:TextBox ID="tb_BlogContent" runat="server" TextMode="MultiLine" Rows="10" Columns="80"></asp:TextBox>

        <script>
            CKEDITOR.replace('<%= tb_BlogContent.ClientID %>');
        </script>
        <br />

    </div>
</asp:Content>
