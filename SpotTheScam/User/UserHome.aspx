<%@ Page Title="UserHome" Language="C#" MasterPageFile="User.master" AutoEventWireup="true" CodeBehind="UserHome.aspx.cs" Inherits="SpotTheScam.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Google Fonts: Archivo Black & DM Sans -->
    <link href="https://fonts.googleapis.com/css2?family=Archivo+Black&family=DM+Sans:wght@400;500;700&display=swap" rel="stylesheet">
    <style>
        body {
            margin: 0;
            padding: 0;
            background-color: #F4F6F8;
        }
        .hero-section {
            background: linear-gradient(90deg, #D36F2D 52%, #FBECC3 100%);
            color: white;
            padding: 0;
            position: relative;
            width: 100vw;
            margin-left: calc(-50vw + 50%);
            overflow: visible;
        }
        .hero-heading {
            font-family: 'Archivo Black', sans-serif;
            text-transform: uppercase;
            font-size: 5rem;
            line-height: 1;
            margin: 0;
        }
        .hero-subtext {
            font-family: 'DM Sans', sans-serif;
            font-size: 1.5rem;
            font-weight: 400;
            margin-top: 1rem;
            color: white;
        }
        .hero-image {
            width: 550px;
            max-width: 100%;
            height: auto;
            position: relative;
            z-index: 10;
            margin-top: -50px;
            transform: scale(1.25) translateY(47px);
            clip-path: inset(100px 0 100px 0);
        }
        .hero-section .container-fluid {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 15px;
        }
        .text-column {
            z-index: 2;
            position: relative;
            text-align: left;
            padding-left: 0;
            margin-left: -70px; /* THIS pushes only the text left */
        }
        .image-column {
            z-index: 5;
            position: relative;
            overflow: visible;
            padding-left: 50px;
            margin-left: 50px;
        }
        .content-below-hero {
            background: #F4F6F8;
            padding: 4rem 0;
            width: 100vw;
            margin-left: calc(-50vw + 50%);
            margin-top: -2rem;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Shown if user is logged in -->
    <asp:PlaceHolder ID="phWelcome" runat="server" Visible="false">
        <div class="text-center">
            <h1 class="display-4">Hello, <asp:Label ID="lblName" runat="server" />!</h1>
            <p class="lead">Welcome back to SpotTheScam. Ready to protect yourself today?</p>
            <a class="btn btn-success btn-lg" href="#">Start Scanning</a>
        </div>
    </asp:PlaceHolder>

    <!-- Hero section shown if user is NOT logged in -->
    <asp:PlaceHolder ID="phHero" runat="server" Visible="false">
        <section class="hero-section">
            <div class="container-fluid">
                <div class="row align-items-center">
                    <div class="col-md-6 text-column">
                        <h1 class="hero-heading">STAY SHARP. STAY SAFE.</h1>
                        <p class="hero-subtext">Learn how to stay safe with Spot The Scam</p>
                    </div>
                    <div class="col-md-6 text-center image-column">
                        <img src="/Images/scammer.svg" alt="Hero Visual" class="hero-image" />
                    </div>
                </div>
            </div>
        </section>
        <div class="content-below-hero">
            <div class="content-section">
                <!-- Add content here -->
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
