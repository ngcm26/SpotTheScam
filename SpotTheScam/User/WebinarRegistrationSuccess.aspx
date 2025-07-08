<%@ Page Title="Registration Successful" Language="C#" MasterPageFile="User.Master" AutoEventWireup="true" CodeBehind="WebinarRegistrationSuccess.aspx.cs" Inherits="SpotTheScam.User.WebinarRegistrationSuccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .points-badge {
            background: #FFC107;
            color: #000;
            padding: 5px 15px;
            border-radius: 15px;
            font-weight: 600;
            font-size: 0.9rem;
            position: absolute;
            top: 20px;
            right: 20px;
        }

        .page-container {
            padding: 40px 0 60px 0;
        }

        .success-card {
            background: white;
            border-radius: 15px;
            box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
            margin: 0 auto;
            max-width: 800px;
            padding: 40px;
            text-align: center;
            position: relative;
        }

        .success-icon {
            width: 80px;
            height: 80px;
            background: #28a745;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 30px;
            font-size: 40px;
            color: white;
        }

        .success-title {
            font-size: 1.5rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 10px;
        }

        .success-subtitle {
            font-size: 1rem;
            color: #666;
            margin-bottom: 40px;
        }

        .info-section {
            background: #e8f4e8;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: left;
        }

        .info-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }

        .info-title::before {
            content: "💻";
            margin-right: 8px;
            font-size: 1.2rem;
        }

        .step-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }

        .step-item {
            display: flex;
            align-items: flex-start;
            margin-bottom: 12px;
            font-size: 0.95rem;
            color: #333;
        }

        .step-number {
            background: #28a745;
            color: white;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 0.8rem;
            font-weight: 600;
            margin-right: 12px;
            flex-shrink: 0;
        }

        .preparation-section {
            background: #fff3cd;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: left;
        }

        .prep-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }

        .prep-title::before {
            content: "📝";
            margin-right: 8px;
            font-size: 1.2rem;
        }

        .prep-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }

        .prep-item {
            display: flex;
            align-items: flex-start;
            margin-bottom: 10px;
            font-size: 0.9rem;
            color: #333;
        }

        .prep-item::before {
            content: "•";
            color: #ffc107;
            font-weight: bold;
            margin-right: 8px;
            font-size: 1.2rem;
        }

        .home-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 15px 40px;
            border-radius: 25px;
            font-weight: 600;
            font-size: 1rem;
            cursor: pointer;
            transition: background-color 0.3s ease;
            text-decoration: none;
            display: inline-block;
            margin-top: 20px;
        }

        .home-btn:hover {
            background: #b45a22;
            color: white;
        }

        .session-reminder {
            background: #f8f9fa;
            border-left: 4px solid var(--brand-orange);
            padding: 20px;
            margin-bottom: 30px;
            border-radius: 0 8px 8px 0;
        }

        .reminder-text {
            font-size: 0.95rem;
            color: #333;
            margin: 0;
        }

        @media (max-width: 768px) {
            .success-card {
                padding: 25px;
                margin: 0 15px;
            }
            
            .points-badge {
                position: static;
                display: inline-block;
                margin-bottom: 20px;
            }
            
            .info-section, .preparation-section {
                padding: 20px;
            }
            
            .page-container {
                padding: 20px 0 40px 0;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <div class="container">
            <div class="success-card">
                <!-- Points Badge -->
                <div class="points-badge">
                    <span>Current Points: 75 ⭐</span>
                </div>
                
                <!-- Success Icon -->
                <div class="success-icon">
                    ✓
                </div>
                
                <!-- Success Message -->
                <h1 class="success-title">REGISTRATION SUCCESSFUL!</h1>
                <p class="success-subtitle">You're all set for the cybersecurity expert session</p>
                
                <!-- Session Reminder -->
                <div class="session-reminder">
                    <p class="reminder-text">
                        <strong>Session:</strong> <asp:Literal ID="ltSessionName" runat="server">Protecting Your Online Banking</asp:Literal><br>
                        <strong>Date & Time:</strong> <asp:Literal ID="ltDateTime" runat="server">June 15, 2025 at 2:00 PM - 3:00 PM</asp:Literal><br>
                        <strong>Expert:</strong> <asp:Literal ID="ltExpertName" runat="server">Dr Harvey Blue</asp:Literal>
                    </p>
                </div>
                
                <!-- How to Join Instructions -->
                <div class="info-section">
                    <h3 class="info-title">How to join your session:</h3>
                    <ul class="step-list">
                        <li class="step-item">
                            <span class="step-number">1</span>
                            <span>Return to this website 10 minutes before your session</span>
                        </li>
                        <li class="step-item">
                            <span class="step-number">2</span>
                            <span>Enter your phone number when prompted</span>
                        </li>
                        <li class="step-item">
                            <span class="step-number">3</span>
                            <span>Click "Join Session" - we'll find you automatically!</span>
                        </li>
                    </ul>
                </div>
                
                <!-- Preparation Tips -->
                <div class="preparation-section">
                    <h3 class="prep-title">Get Ready for Your Session:</h3>
                    <ul class="prep-list">
                        <li class="prep-item">Join 10 minutes early to test your camera and microphone</li>
                        <li class="prep-item">Ensure you have a stable internet connection</li>
                        <li class="prep-item">Have a pen and paper ready for taking notes</li>
                        <li class="prep-item">Prepare any banking security questions you'd like to ask</li>
                        <li class="prep-item">Find a quiet space with good lighting</li>
                    </ul>
                </div>
                
                <!-- Back to Home Button -->
                <asp:HyperLink ID="lnkBackToHome" runat="server" 
                    NavigateUrl="~/User/UserHome.aspx" 
                    CssClass="home-btn">Back to Home</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>