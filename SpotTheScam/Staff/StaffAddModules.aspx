<%@ Page Title="Add New Module" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="StaffAddModules.aspx.cs" Inherits="SpotTheScam.Staff.StaffAddModules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Add New Module</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Add New Module</h1>

    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />

    <asp:Label ID="lblModuleName" runat="server" Text="Module Name:" AssociatedControlID="txtModuleName" /><br />
    <asp:TextBox ID="txtModuleName" runat="server" /><br /><br />

    <asp:Label ID="lblCoverImage" runat="server" Text="Cover Image:" AssociatedControlID="fuCoverImage" /><br />
    <asp:FileUpload ID="fuCoverImage" runat="server" /><br /><br />

    <asp:Label ID="lblAuthor" runat="server" Text="Author:" AssociatedControlID="txtAuthor" /><br />
    <asp:TextBox ID="txtAuthor" runat="server" /><br /><br />

    <asp:Label ID="lblStatus" runat="server" Text="Status:" AssociatedControlID="ddlStatus" /><br />
    <asp:DropDownList ID="ddlStatus" runat="server">
        <asp:ListItem Text="Draft" Value="draft"></asp:ListItem>
        <asp:ListItem Text="Published" Value="published"></asp:ListItem>
    </asp:DropDownList><br /><br />

    <asp:Button ID="btnAddModule" runat="server" Text="Add Module" OnClick="btnAddModule_Click" />
</asp:Content>
