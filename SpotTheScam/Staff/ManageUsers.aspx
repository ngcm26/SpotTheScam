<%@ Page Title="Manage Users" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="SpotTheScam.Staff.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table {
            width: 100%;
            margin-top: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Manage Users</h2>

    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False"
        DataKeyNames="Id"
        CssClass="table table-bordered"
        OnRowDeleting="gvUsers_RowDeleting"
        OnRowEditing="gvUsers_RowEditing"
        OnRowCancelingEdit="gvUsers_RowCancelingEdit"
        OnRowUpdating="gvUsers_RowUpdating">

        <Columns>
            <asp:BoundField DataField="Username" HeaderText="Username" ReadOnly="False" />
            <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="False" />
            <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" ReadOnly="False" />

            <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
        </Columns>
    </asp:GridView>
</asp:Content>
