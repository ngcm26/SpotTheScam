<%@ Page Title="Staff Dashboard" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="StaffDashboard.aspx.cs" Inherits="SpotTheScam.Staff.StaffDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Shown if logged in -->
    <asp:PlaceHolder ID="phDashboard" runat="server" Visible="false">
        <h1 class="mb-4">Welcome, <asp:Label ID="lblStaffName" runat="server" /></h1>
        <p class="lead">This is your staff dashboard. Choose an action:</p>
        <a href="ManageUsers.aspx" class="btn btn-primary btn-lg me-2">Manage Users</a>
        <a href="StaffLogout.aspx" class="btn btn-danger btn-lg">Logout</a>
    </asp:PlaceHolder>

    <!-- Shown if NOT logged in -->
    <asp:PlaceHolder ID="phPleaseLogin" runat="server" Visible="false">
        <div class="alert alert-warning" role="alert">
            <h4 class="alert-heading">Access Denied</h4>
            <p>Please log in to view this page.</p>
            <hr>
            <a href="../User/UserLogin.aspx" class="btn btn-primary">Go to Login</a>
        </div>
    </asp:PlaceHolder>
</asp:Content>
