<%@ Page Title="Registration Successful" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="WebinarRegistrationSuccess.aspx.cs" Inherits="SpotTheScam.User.WebinarRegistrationSuccess" %>
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

        .session-link-section {
            background: #e7f3ff;
            border: 1px solid #b3d9ff;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: left;
        }

        .link-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }

        .link-title::before {
            content: "🔗";
            margin-right: 8px;
            font-size: 1.2rem;
        }

        .session-link {
            background: white;
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 15px;
            font-family: monospace;
            font-size: 0.9rem;
            word-break: break-all;
            margin: 15px 0;
            color: #495057;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }

        .copy-button {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 0.9rem;
            font-weight: 600;
            transition: background 0.3s;
        }

        .copy-button:hover {
            background: #b45a22;
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
            margin: 10px;
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

        .btn-group {
            margin-top: 20px;
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
            
            .info-section, .preparation-section, .session-link-section {
                padding: 20px;
            }
            
            .page-container {
                padding: 20px 0 40px 0;
            }

            .btn-group {
                display: flex;
                flex-direction: column;
                gap: 10px;
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
                    <span>Current Points: <asp:Label ID="lblCurrentPoints" runat="server" Text="0" /> ⭐</span>
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

                <!-- Session Link Section -->
                <div class="session-link-section">
                    <h3 class="link-title">Your Personal Session Link</h3>
                    <p><strong>Important:</strong> Save this link to join your session. You can also find it in "My Sessions".</p>
                    <div class="session-link" id="sessionLinkText">
                        <asp:Label ID="lblSessionLink" runat="server"></asp:Label>
                    </div>
                    <button type="button" class="copy-button" onclick="copySessionLink()">📋 Copy Link</button>
                    <p style="margin-top: 10px; font-size: 0.85rem; color: #666;">
                        💡 <strong>Tip:</strong> You can join 10 minutes before the session starts
                    </p>
                </div>
                
                <!-- How to Join Instructions -->
                <div class="info-section">
                    <h3 class="info-title">How to join your session:</h3>
                    <ul class="step-list">
                        <li class="step-item">
                            <span class="step-number">1</span>
                            <span>Click your session link 10 minutes before the session time</span>
                        </li>
                        <li class="step-item">
                            <span class="step-number">2</span>
                            <span>Or go to "My Sessions" and click "Join Session Now"</span>
                        </li>
                        <li class="step-item">
                            <span class="step-number">3</span>
                            <span>Allow camera and microphone access when prompted</span>
                        </li>
                        <li class="step-item">
                            <span class="step-number">4</span>
                            <span>Wait for the expert to connect and start your session!</span>
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
                
                <!-- Action Buttons -->
                <div class="btn-group">
                    <asp:HyperLink ID="lnkMySession" runat="server" 
                        NavigateUrl="~/User/UserMySessions.aspx" 
                        CssClass="home-btn">📅 View My Sessions</asp:HyperLink>
                        
                    <asp:HyperLink ID="lnkBackToHome" runat="server" 
                        NavigateUrl="~/User/UserHome.aspx" 
                        CssClass="home-btn">🏠 Back to Home</asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <script>
        function copySessionLink() {
            var linkElement = document.getElementById('sessionLinkText');
            var linkText = linkElement.innerText || linkElement.textContent;
            
            navigator.clipboard.writeText(linkText).then(function() {
                alert('Session link copied to clipboard! 📋✅');
            }, function() {
                // Fallback for older browsers
                var textArea = document.createElement('textarea');
                textArea.value = linkText;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                alert('Session link copied to clipboard! 📋✅');
            });
        }
    </script>
</asp:Content>