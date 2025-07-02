<%@ Page Title="UserHome" Language="C#" MasterPageFile="User.master" AutoEventWireup="true" CodeBehind="UserHome.aspx.cs" Inherits="SpotTheScam.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="text-center">
        <!-- Shown if user is logged in -->
        <asp:PlaceHolder ID="phWelcome" runat="server" Visible="false">
            <h1 class="display-4">Hello, <asp:Label ID="lblName" runat="server" />!</h1>
            <p class="lead">Welcome back to SpotTheScam. Ready to protect yourself today?</p>
            <a class="btn btn-success btn-lg" href="#">Start Scanning</a>
        </asp:PlaceHolder>

        <!-- Shown if user is not logged in -->
        <asp:PlaceHolder ID="phGuest" runat="server" Visible="false">
            <h1 class="display-4">Welcome to SpotTheScam</h1>
            <p class="lead">Join us to detect scams and stay safe online.</p>
            <a class="btn btn-primary btn-lg me-2" href="UserLogin.aspx">Login</a>
            <a class="btn btn-outline-primary btn-lg" href="UserRegister.aspx">Create an Account</a>
        </asp:PlaceHolder>
    </div>
</asp:Content>
