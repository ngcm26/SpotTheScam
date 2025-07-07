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
        .main-content {
            padding: 0 !important;
            margin: 0 !important;
        }
        .hero-section {
            background: linear-gradient(90deg, #D36F2D 52%, #FBECC3 100%);
            color: white;
            padding: 1.5rem 0 1rem 0; /* Reduced padding for shorter banner */
            position: relative;
            width: 100vw;
            margin-left: calc(-50vw + 50%);
            clip-path: polygon(0 0, 100% 0, 100% 85%, 0 100%); /* Full width trapezoid */
            overflow: visible; /* Allow SVG to extend beyond boundaries */
        }
        .hero-section .row {
            position: relative;
            z-index: 2;
        }
        .hero-heading {
            font-family: 'Archivo Black', sans-serif;
            font-weight: 400; /* Archivo Black only has one weight */
            font-size: 4rem; /* Bigger heading */
            line-height: 1.1;
            margin-bottom: 1rem;
            margin-top: -1rem; /* Move text up */
        }
        .hero-subtext {
            font-family: 'DM Sans', sans-serif;
            font-size: 1.5rem;
            color: white;
            font-weight: 400;
            margin-top: 0.5rem; /* Move text up */
        }
        .hero-image {
            width: 600px; /* Larger SVG */
            max-width: 100%;
            height: auto;
            position: relative;
            z-index: 10; /* Much higher z-index to be clearly on top */
            margin-top: -60px; /* Move SVG up more to overlap background */
        }
        .hero-section .container-fluid {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 15px;
        }
        .content-below-hero {
            background-color: #F4F6F8;
            padding: 4rem 0;
            width: 100vw;
            margin-left: calc(-50vw + 50%);
            margin-top: -2rem; /* Pull content up to reduce gap */
        }
        .content-section {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 15px;
        }
        /* Ensure text column has proper positioning */
        .text-column {
            position: relative;
            z-index: 2;
            padding-top: 0;
        }
        
        /* Ensure image column allows overflow */
        .image-column {
            position: relative;
            z-index: 3;
            overflow: visible;
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
                    <div class="col-md-6 ps-5 text-column">
                        <h1 class="display-4 hero-heading">STAY SHARP.<br>STAY SAFE.</h1>
                        <p class="hero-subtext mt-3">Learn how to stay safe with Spot The Scam</p>
                    </div>
                    <div class="col-md-6 text-center image-column">
                        <img src="/Images/scammer.svg" alt="Hero Visual" class="hero-image" />
                    </div>
                </div>
            </div>
        </section>
        <!-- Content below hero section with grey background -->
        <div class="content-below-hero">
            <div class="content-section">
                <!-- Add your content here -->
                <!-- This is where the article content would go -->
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>