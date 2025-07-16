<%@ Page Title="Available Modules" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserModules.aspx.cs" Inherits="SpotTheScam.User.UserModules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Available Modules</title>
    <link href="https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap" rel="stylesheet" />
    <style>
        /* Main content styling */
        .content-container {
            background: #D36F2D;
            width: 100vw;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            position: relative;
            padding: 48px 20px 32px 20px;
            font-family: 'DM Sans', sans-serif;
            z-index: 1;
        }

        .content-container h2 {
            color: white;
            font-size: 2.5rem;
            font-weight: 600;
            text-align: center;
            margin-bottom: 18px;
            text-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .module-desc {
            color: #fff;
            text-align: center;
            font-size: 1.08rem;
            max-width: 700px;
            margin: 0 auto 30px auto;
            line-height: 1.6;
            font-family: 'DM Sans', sans-serif;
        }

        .modules-wrapper {
            max-width: 1400px;
            margin: 0 auto;
            display: flex;
            justify-content: center;
            position: relative;
            z-index: 2;
            margin-top: 40px;
        }

        .modules-row {
            display: flex;
            gap: 40px;
            justify-content: center;
            flex-wrap: nowrap;
            overflow: hidden;
            width: 100%;
            max-width: 1100px;
            transition: all 0.5s cubic-bezier(0.4, 0.2, 0.2, 1);
        }

        .module-card {
            background: #fff;
            border-radius: 20px;
            box-shadow: 0 4px 16px rgba(5,29,64,0.08);
            width: 310px;
            height: 440px;
            display: flex;
            flex-direction: column;
            align-items: stretch;
            justify-content: flex-start;
            padding: 0;
            margin-bottom: 0;
            overflow: hidden;
        }

        .module-card:hover {
            transform: translateY(-8px);
            box-shadow: 0 20px 40px rgba(0,0,0,0.2);
        }

        /* Remove any tint/overlay from the card images */
        .module-card::before { display: none !important; }

        .module-card-content {
            position: relative;
            z-index: 2;
            height: 100%;
            display: flex;
            flex-direction: column;
            padding: 40px 30px 35px;
        }

        .module-card img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            border-radius: 0;
            margin: 0;
            background: none;
            box-shadow: none;
        }

        .module-progress {
            width: 90%;
            height: 8px;
            background-color: #f0f0f0;
            border-radius: 6px;
            margin: 18px auto 0 auto;
            overflow: hidden;
        }
        .module-progress::after {
            content: '';
            display: block;
            width: 0%;
            height: 100%;
            background: linear-gradient(90deg, #E85A4F, #D2691E);
            border-radius: 6px;
            transition: width 0.3s ease;
        }
        .progress-text {
            font-size: 1.1rem;
            color: #051D40;
            font-weight: 600;
            text-align: right;
            width: 90%;
            margin: 4px auto 0 auto;
        }

        .module-title {
            font-size: 1.15rem;
            font-weight: 700;
            color: #051D40;
            margin-top: 18px;
            margin-bottom: 0;
            line-height: 1.3;
            padding: 0 10px;
            text-align: center;
            font-family: 'DM Sans', sans-serif;
        }
        /* Remove progress bar and text for now */
        .module-progress, .progress-text { display: none; }

        /* Add background color to the bottom section */
        .bottom-section {
            position: relative;
            z-index: 1;
            margin-top: -150px;
            padding-top: 200px;
        }

        /* Navigation dots (if needed for carousel) */
        .navigation-dots {
            display: flex;
            justify-content: center;
            gap: 10px;
            margin-top: 40px;
        }

        .dot {
            width: 12px;
            height: 12px;
            border-radius: 50%;
            background-color: rgba(255,255,255,0.4);
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .dot.active {
            background-color: white;
        }

        /* Skills section styling */
        .skills-section {
            font-family: 'DM Sans', sans-serif;
            margin-top: 40px;
            background: #FFF8DC;
            padding: 80px 20px;
            margin-top: 0;
        }

        .skills-container {
            max-width: 1200px;
            margin: 0 auto;
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 60px;
            align-items: center;
        }

        .skills-text {
            padding-right: 30px;
        }

        .skills-text h3 {
            font-size: 2.5rem;
            color: #D2691E;
            margin-bottom: 25px;
            font-weight: 600;
            line-height: 1.2;
        }

        .skills-text p {
            color: #051D40;
            line-height: 1.6;
            font-size: 1.3rem;
            font-family: 'DM Sans', sans-serif;
        }

        .skills-list {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .skill-item {
            background: #D36F2D;
            color: white;
            padding: 25px 30px;
            border-radius: 15px;
            font-weight: 500;
            box-shadow: 0 4px 15px rgba(232, 90, 79, 0.3);
            font-size: 1.1rem;
            line-height: 1.4;
            font-family: 'DM Sans', sans-serif;
        }

        .skill-number {
            font-size: 1.3rem;
            font-weight: 700;
            margin-bottom: 10px;
            display: block;
        }

        /* Remove bold from skill-item description text */
        .skill-item {
            font-weight: 400;
        }
        .skill-number {
            font-weight: 700;
        }

        /* Partners section */
        .partners-section {
            background: white;
            padding: 60px 20px;
            text-align: center;
        }

        .partners-section h3 {
            font-size: 2rem;
            color: #D36F2D;
            font-family: 'DM Sans', sans-serif;
            font-weight: bold;
            margin-bottom: 30px;
        }

        .partners-text {
            max-width: 800px;
            margin: 0 auto 40px;
            color: #051D40;
            line-height: 1.6;
            font-family: 'DM Sans', sans-serif;
        }

        .partners-logos {
            display: flex;
            justify-content: center;
            gap: 40px;
            align-items: center;
            flex-wrap: wrap;
        }

        .partner-logo {
            height: 60px;
            opacity: 0.8;
            transition: opacity 0.3s ease;
        }

        .partner-logo:hover {
            opacity: 1;
        }

        /* Message styling */
        #lblMessage {
            display: block;
            text-align: center;
            margin-bottom: 30px;
            padding: 15px;
            border-radius: 10px;
            background: rgba(255,255,255,0.1);
            color: white;
        }

        /* Responsive design */
        @media (max-width: 768px) {
            .content-container {
                padding: 40px 15px 0;
                min-height: 200px;
                height: 200px;
            }
            
            .content-container h2 {
                font-size: 2.5rem;
            }
            
            .modules-row {
                flex-direction: column;
                align-items: center;
                gap: 30px;
            }
            
            .module-card {
                width: 100%;
                max-width: 310px;
                height: 270px;
            }
            
            .skills-container {
                grid-template-columns: 1fr;
                gap: 40px;
            }
            
            .skills-text {
                padding-right: 0;
                text-align: center;
            }
            
            .skills-text h3 {
                font-size: 2rem;
            }
            
            .skills-text p {
                font-size: 1.1rem;
            }
            
            .partners-logos {
                gap: 20px;
            }
            .modules-wrapper {
                margin-top: -100px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-container">
        <h2>Pick a module to start</h2>
        <div class="module-desc">Stay safe from scams with our free anti-scam module. Learn to spot fake messages, suspicious links, and common tricks through real-life examples. Protect yourself and your loved ones - all with simple, practical steps anyone can follow.</div>
        <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
    </div>
    
    <div class="modules-wrapper">
        <button id="carousel-left" class="carousel-arrow" type="button" aria-label="Previous modules" disabled>
            <span class="arrow-triangle left"></span>
        </button>
        <div class="modules-row" id="modules-carousel-row">
            <asp:Repeater ID="rptModules" runat="server">
                <ItemTemplate>
                    <div class="carousel-card-wrapper" style="display: none; flex-direction: column; align-items: center;">
                        <a href='ModuleInformation.aspx?module_id=<%# Eval("module_id") %>' class="module-link" style="text-decoration:none;">
                            <div class="module-card">
                                <img src='<%# ResolveUrl(Eval("cover_image").ToString()) %>' alt="Module Cover" />
                            </div>
                        </a>
                        <div class="module-title"><%# Eval("module_name") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <button id="carousel-right" class="carousel-arrow" type="button" aria-label="Next modules">
            <span class="arrow-triangle right"></span>
        </button>
    </div>

    <script type="text/javascript">
        // Set isLoggedIn from server-side session
        var isLoggedIn = '<%=(Session["Username"] != null || Session["UserId"] != null) ? "true" : "false" %>';
        document.addEventListener('DOMContentLoaded', function() {
            const cards = Array.from(document.querySelectorAll('.carousel-card-wrapper'));
            const leftBtn = document.getElementById('carousel-left');
            const rightBtn = document.getElementById('carousel-right');
            let startIdx = 0;
            const visibleCount = 3;

            console.log('isLoggedIn:', isLoggedIn);

            // No need for attachModuleClickHandlers, links now go directly to ModuleInformation.aspx

            function updateCarousel() {
                cards.forEach((card, idx) => {
                    card.style.display = (idx >= startIdx && idx < startIdx + visibleCount) ? 'flex' : 'none';
                });
                leftBtn.disabled = startIdx === 0;
                rightBtn.disabled = (startIdx + visibleCount >= cards.length);
                // No need to re-attach handlers here as the links now navigate directly
            }

            leftBtn.addEventListener('click', function() {
                if (startIdx > 0) {
                    startIdx -= 1;
                    if (startIdx < 0) startIdx = 0;
                    updateCarousel();
                } else if (cards.length > visibleCount) {
                    // Jump to the last possible set
                    startIdx = cards.length - visibleCount;
                    updateCarousel();
                }
            });
            rightBtn.addEventListener('click', function() {
                if (startIdx + visibleCount < cards.length) {
                    startIdx += 1;
                    updateCarousel();
                }
            });

            updateCarousel();
        });
    </script>

    <style>
        .modules-wrapper {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0;
            max-width: 1400px;
            margin: 0 auto;
            position: relative;
            z-index: 2;
            margin-top: 40px;
        }
        .modules-row {
            display: flex;
            gap: 40px;
            justify-content: center;
            flex-wrap: nowrap;
            overflow: hidden;
            width: 100%;
            max-width: 1100px;
            transition: all 0.5s cubic-bezier(0.4, 0.2, 0.2, 1);
        }
        .carousel-card-wrapper {
            transition: opacity 0.5s cubic-bezier(0.4, 0.2, 0.2, 1), transform 0.5s cubic-bezier(0.4, 0.2, 0.2, 1);
        }
        .module-card {
            background: #fff;
            border-radius: 20px;
            box-shadow: 0 4px 16px rgba(5,29,64,0.08);
            width: 310px;
            height: 440px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: flex-start;
            padding: 0;
            margin-bottom: 0;
            overflow: hidden;
        }
        .module-card img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            border-radius: 0;
            margin: 0;
            background: none;
            box-shadow: none;
        }
        .module-progress {
            width: 90%;
            height: 8px;
            background-color: #f0f0f0;
            border-radius: 6px;
            margin: 18px auto 0 auto;
            overflow: hidden;
        }
        .module-progress::after {
            content: '';
            display: block;
            width: 0%;
            height: 100%;
            background: linear-gradient(90deg, #E85A4F, #D2691E);
            border-radius: 6px;
            transition: width 0.3s ease;
        }
        .progress-text {
            font-size: 1.1rem;
            color: #051D40;
            font-weight: 600;
            text-align: right;
            width: 90%;
            margin: 4px auto 0 auto;
        }
        .module-title {
            font-size: 1.15rem;
            font-weight: 700;
            color: #051D40;
            margin-top: 18px;
            margin-bottom: 0;
            line-height: 1.3;
            padding: 0 10px;
            text-align: center;
            font-family: 'DM Sans', sans-serif;
        }
        .carousel-arrow {
            background: none;
            border: none;
            width: 44px;
            height: 44px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 10px;
            cursor: pointer;
            padding: 0;
            transition: background 0.2s;
        }
        .arrow-triangle {
            display: inline-block;
            width: 0;
            height: 0;
            border-top: 18px solid transparent;
            border-bottom: 18px solid transparent;
        }
        .arrow-triangle.left {
            border-right: 24px solid #D36F2D;
        }
        .arrow-triangle.right {
            border-left: 24px solid #D36F2D;
        }
        .carousel-arrow:disabled .arrow-triangle {
            opacity: 0.3;
        }
        .carousel-arrow:not(:disabled):hover .arrow-triangle {
            filter: brightness(1.2);
        }
        @media (max-width: 1100px) {
            .modules-row {
                max-width: 90vw;
            }
        }
        @media (max-width: 768px) {
            .modules-row {
                gap: 20px;
            }
            .module-card {
                max-width: 310px;
                height: 270px;
                padding: 18px 8px 8px 8px;
            }
            .carousel-arrow {
                width: 32px;
                height: 32px;
            }
            .arrow-triangle {
                border-top: 12px solid transparent;
                border-bottom: 12px solid transparent;
            }
            .arrow-triangle.left {
                border-right: 16px solid #D36F2D;
            }
            .arrow-triangle.right {
                border-left: 16px solid #D36F2D;
            }
        }
    </style>
        
        <!-- Navigation dots (optional) -->
        <div class="navigation-dots">
            <div class="dot active"></div>
            <div class="dot"></div>
            <div class="dot"></div>
        </div>
    </div>
    
    <!-- Skills Section -->
    <div class="bottom-section">
        <div class="skills-section">
            <div class="skills-container">
                <div class="skills-text">
                    <h3>Learn Practical Skills - Not Just Theory</h3>
                    <p>Spot fake SMS messages, emails and suspicious URLs through realistic scam examples</p>
                </div>
                <div class="skills-list">
                    <div class="skill-item">
                        <span class="skill-number">01. Spot scam messages and emails</span>
                        Learn to identify red flags like unknown senders, fake links, and urgent payment requests.
                    </div>
                    <div class="skill-item">
                        <span class="skill-number">02. Know who to contact</span>
                        Find out who to reach out to including the Anti-Scam Helpline or family.
                    </div>
                    <div class="skill-item">
                        <span class="skill-number">03. Keep your info safe</span>
                        Understand what not to share online and how to protect your personal details.
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Partners Section -->
    <div class="partners-section">
        <h3>Our Partners</h3>
        <p class="partners-text">
            This module is proudly developed in collaboration with GovTech, the Cyber Security Agency of Singapore (CSA) and AgeWell SG as part of a national effort to empower seniors with the knowledge and confidence to navigate the digital world safely.
        </p>
        <div class="partners-logos">
            <!-- Add partner logos here -->
            <img src="/Images/csalogo.png" alt="CSA" class="partner-logo" />
            <img src="/Images/agewelllogo.png" alt="AgeWell SG" class="partner-logo" />
            <img src="/Images/govtechlogo.png" alt="GovTech" class="partner-logo" />
        </div>
    </div>
</asp:Content>