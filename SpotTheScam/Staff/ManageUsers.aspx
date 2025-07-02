<%@ Page Title="Manage Users" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="SpotTheScam.Staff.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2 class="mb-4">Manage Users</h2>
    <asp:GridView ID="gvUsers" runat="server" CssClass="table table-striped"
        AutoGenerateColumns="False"
        DataKeyNames="Id"
        OnRowDeleting="gvUsers_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True" />
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
    </asp:GridView>
</asp:Content>
