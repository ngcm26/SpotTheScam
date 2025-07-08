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
        font-size: 1.8rem;  /* slightly bigger too */
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
        margin-left: -70px;
    }
    .image-column {
        z-index: 5;
        position: relative;
        overflow: visible;
        padding-left: 50px;
        margin-left: 50px;
    }

    /* --- Article1 --- */
    .article1 {
        padding: 4rem 0;
        background: transparent;
    }
    .article1 .container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 0 15px;
    }
    .article1-image {
        width: 100%;
        height: auto;
        border-radius: 8px;
    }
    .article1-text {
        font-family: 'DM Sans', sans-serif;
        color: #333;
    }
    .article1-text h5 {
        text-transform: uppercase;
        font-size: 0.9rem;
        letter-spacing: 1px;
        color: #555;
        margin-bottom: 0.5rem;
    }
    .article1-text h2 {
        font-size: 2rem;
        font-weight: bold;
        color: #D36F2D;
        margin-bottom: 1rem;
    }
    .article1-text p {
        font-size: 1.2rem;
        margin-bottom: 1rem;
        color: #051D40;
    }
    .article1-text a.btn-read-more {
        font-size: 1.1rem;
        text-decoration: none;
        color: #333;
        border-bottom: 1px solid #333;
    }

    /* --- Scam Protection --- */
    .scam-protection {
        background: #FBECC3;
        padding: 4rem 0;
        text-align: center;
        font-family: 'DM Sans', sans-serif;
        width: 100vw;
        margin-left: calc(-50vw + 50%);
    }
    .scam-protection h5 {
        text-transform: uppercase;
        letter-spacing: 2px;
        color: #888;
        font-size: 0.8rem;
        margin-bottom: 1rem;
    }
    .scam-protection h2 {
        font-family: 'DM Sans', sans-serif;
        font-weight: bold;
        color: #D36F2D;
        font-size: 2rem;
        margin-bottom: 2rem;
    }
    .scam-protection .row {
        max-width: 1000px;
        margin: 0 auto;
    }
    .scam-protection h4 {
        font-size: 1.3rem;
        font-weight: 700;
        margin-bottom: 1rem;
        color: #051D40;
    }
    .scam-protection p {
        font-size: 1.2rem;
        color: #051D40;
        margin-bottom: 1rem;
    }
    .scam-protection a {
        font-size: 1.1rem;
        color: #051D40;
        text-decoration: underline;
        font-weight: bold;
    }

    /* --- Hotline --- */
    .hotline-section {
        padding: 4rem 0;
        background: #F4F6F8;
    }
    .hotline-section .container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 0 15px;
    }
    .hotline-text {
        font-family: 'DM Sans', sans-serif;
        color: #333;
    }
    .hotline-text h5 {
        text-transform: uppercase;
        font-size: 0.9rem;
        letter-spacing: 2px;
        color: #555;
        margin-bottom: 1rem;
    }
    .hotline-text h2 {
        font-size: 1.8rem;
        font-weight: bold;
        color: #D26F2C;
        margin-bottom: 1rem;
    }
    .hotline-text p {
        font-size: 1.2rem;
        margin-bottom: 0;
        color: #051D40;
    }
    .hotline-image {
        width: 100%;
        max-width: 400px;
        height: auto;
        transform: scale(1.3);
        clip-path: inset(0% 0% 11% 0%);
    }

    /* --- Article2 --- */
    .article2 {
        padding: 4rem 0;
        background: #FBECC3;
        width: 100vw;
        margin-left: calc(-50vw + 50%);
    }
    .article2 .container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 0 15px;
    }
    .article2-text {
        font-family: 'DM Sans', sans-serif;
        color: #333;
    }
    .article2-text h5 {
        text-transform: uppercase;
        font-size: 0.9rem;
        letter-spacing: 1px;
        color: #555;
        margin-bottom: 1rem;
    }
    .article2-text h2 {
        font-size: 1.8rem;
        font-weight: bold;
        color: #D26F2C;
        margin-bottom: 1rem;
    }
    .article2-text p {
        font-size: 1.2rem;
        margin-bottom: 1rem;
        color: #051D40;
    }
    .article2-text a.btn-read-more {
        font-size: 1.1rem;
        text-decoration: none;
        color: #051D40;
        border-bottom: 1px solid #051D40;
        font-weight: bold;
    }
    .article2-image {
        width: 100%;
        height: auto;
        border-radius: 8px;
    }

    /* --- Statistics --- */
    .statistics {
        background: #F4F6F8;
        padding: 4rem 0;
        text-align: center;
        font-family: 'DM Sans', sans-serif;
    }
    .statistics h5 {
        text-transform: uppercase;
        font-size: 0.9rem;
        letter-spacing: 1px;
        color: #555;
        margin-bottom: 0.5rem;
    }
    .statistics h2 {
        font-family: 'DM Sans', sans-serif;
        color: #D26F2C;
        font-size: 2rem;
        margin-bottom: 0.5rem;
        font-weight: bold;
    }
    .statistics p {
        color: #051D40;
        font-size: 1.2rem;
        margin-bottom: 2rem;
    }
    .statistics .row {
        display: flex;
        flex-wrap: nowrap;
        justify-content: space-between;
        align-items: stretch;
        gap: 1rem;
        max-width: 1000px;
        margin: 0 auto;
    }
    .statistics-card {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        flex: 1 1 22%;
        background: #D26F2C;
        color: #fff;
        border-radius: 8px;
        padding: 1.5rem;
        text-align: left;
        box-shadow: 2px 2px 8px rgba(0,0,0,0.1);
        min-height: 180px;
    }
    .statistics-card h4 {
        font-size: 1rem;
        font-weight: 700;
        margin-bottom: 0.5rem;
    }
    .statistics-card p {
        font-size: 1.2rem;
        margin-bottom: 1rem;
        color: #fff;
    }
    .statistics-card a {
        font-size: 1.1rem;
        color: #fff;
        text-decoration: underline;
        font-weight: bold;
        margin-top: auto;
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

      <!-- Article1 Section -->
        <section class="article1">
            <div class="container">
                <div class="row align-items-center">
                    <div class="col-md-6">
                        <img src="/Images/workscam.jpg" alt="Placeholder Image" class="article1-image" />
                    </div>
                    <div class="col-md-6 article1-text ps-md-5">
                        <h5>THINK YOU’RE SAFE FROM SCAMS?</h5>
                        <h2>How Scammers Prey on Job Seekers in Tough Times</h2>
                        <p>As scams grow smarter and harder to detect, what people need most is awareness, empathy, and guidance — not judgment.</p>
                        <a href="https://www.ricemedia.co/how-scammers-prey-on-job-seekers/" class="btn-read-more">READ MORE →</a>
                    </div>
                </div>
            </div>
        </section>

        <!-- Scam Protection Section -->
        <section class="scam-protection">
            <div class="container">
                <h5>GET SCAM PROTECTION</h5>
                <h2>Stay safe with Spot The Scam</h2>
                <div class="row text-start gx-5">
                    <div class="col-md-4 mb-4">
                        <h4>Check for scams</h4>
                        <p>Know if a suspicious message or link is likely a scam</p>
                        <a href="#">Try now</a>
                    </div>
                    <div class="col-md-4 mb-4">
                        <h4>Scam recovery support</h4>
                        <p>Step-by-step help for reporting, freezing accounts, and limiting damage</p>
                        <a href="#">Try now</a>
                    </div>
                    <div class="col-md-4 mb-4">
                        <h4>Get educated on scams</h4>
                        <p>Learn how scammers operate so you won’t be their next target</p>
                        <a href="#">Try now</a>
                    </div>
                </div>
            </div>
        </section>

        <!-- Hotline Section -->
        <section class="hotline-section">
            <div class="container">
                <div class="row align-items-center">
                    <div class="col-md-6 hotline-text">
                        <h5>UNSURE IF IT’S A SCAM?</h5>
                        <h2>Call and check with the Anti-Scam Helpline: 1744</h2>
                        <p>Available 24/7, Monday to Sunday</p>
                    </div>
                    <div class="col-md-6 text-center">
                        <img src="/Images/talking.svg" alt="Helpline Illustration" class="hotline-image" />
                    </div>
                </div>
            </div>
        </section>

        <!-- Article2 Section -->
        <section class="article2">
          <div class="container">
            <div class="row align-items-center">
              <div class="col-md-6">
                <img src="/Images/govscam.jpg" alt="Gov Scam Image" class="article2-image" />
              </div>
              <div class="col-md-6 article2-text ps-md-5">
                <h5>DID YOU KNOW?</h5>
                <h2>Government Officials will never ask you to transfer money</h2>
                <p>S$151.3 million lost to Government Official Impersonation Scams in 2024. Protect yourself: Learn the dos & don’ts of government officials now!</p>
                <a href="#" class="btn-read-more">READ MORE →</a>
              </div>
            </div>
          </div>
        </section>

        <!-- Statistics Section -->
        <section class="statistics">
          <div class="container">
            <h5>STAY INFORMED</h5>
            <h2>Scam Threats</h2>
            <p>Singapore lost at least S$1.1 billion to scams in 2024 across 51,501 cases</p>
            <div class="row gx-4 gy-4 text-start">
              <div class="col-md-3 statistics-card">
                <h4>Government Officials Impersonation Scams</h4>
                <p>~S$100,622 lost per case</p>
                <a href="#">Learn more</a>
              </div>
              <div class="col-md-3 statistics-card">
                <h4>Investment Scams</h4>
                <p>~S$47,077 lost per case</p>
                <a href="#">Learn more</a>
              </div>
              <div class="col-md-3 statistics-card">
                <h4>Job Scams</h4>
                <p>~S$17,281 lost per case</p>
                <a href="#">Learn more</a>
              </div>
              <div class="col-md-3 statistics-card">
                <h4>Phishing Scams</h4>
                <p>~S$6,955 lost per case</p>
                <a href="#">Learn more</a>
              </div>
            </div>
          </div>
        </section>
    </asp:PlaceHolder>
</asp:Content>
