<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="IndividualBlogStafff.aspx.cs" Inherits="SpotTheScam.Staff.IndividualBlogStafff" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    <h2>
    <asp:Label ID="lbl_title" runat="server"></asp:Label>
    </h2>
    <p>
        <asp:Image ID="img_cover" runat="server" Height="287px" Width="1104px" />
    </p>
    <p>
        
        <asp:Literal ID="lbl_content" runat="server" Mode="PassThrough" />
    </p>
    <p>
        &nbsp;</p>
    <br />
    <br />
    <br />
</asp:Content>