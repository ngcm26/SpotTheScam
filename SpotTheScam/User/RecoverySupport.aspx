<%@ Page Title="Scam Recovery Support" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="RecoverySupport.aspx.cs" Inherits="SpotTheScam.User.RecoverySupport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .hero-banner {
            background: linear-gradient(135deg, #FFF3CD 0%, #FFE4B5 100%);
            padding: 2rem 0;
            margin-bottom: 3rem;
        }

        .assistance-section {
            max-width: 700px;
            margin: 0 auto 4rem auto;
        }

        .assistance-card {
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 2rem;
            background: white;
        }

        .hotline-btn {
            background-color: #D36F2D;
            border: none;
            color: white;
            padding: 1rem 2rem;
            border-radius: 8px;
            font-weight: 600;
            font-size: 1rem;
            width: 100%;
            margin-bottom: 2rem;
        }

        .emergency-section {
            display: flex;
            gap: 1rem;
        }

        .emergency-box {
            flex: 1;
            border: 2px solid #dc3545;
            border-radius: 8px;
            padding: 1.5rem;
            text-align: center;
            background: white;
        }

        .emergency-box .icon {
            font-size: 2rem;
            margin-bottom: 0.5rem;
            color: #dc3545;
        }

        .emergency-box .title {
            font-weight: bold;
            font-size: 1rem;
            margin-bottom: 0.5rem;
        }

        .emergency-box .number {
            font-size: 1.25rem;
            font-weight: bold;
            color: #dc3545;
            margin-bottom: 0.5rem;
        }

        .emergency-box .description {
            font-size: 0.875rem;
            color: #666;
        }

        .report-box {
            border-color: #6c757d;
        }

        .report-box .icon {
            color: #6c757d;
        }

        .report-box .number {
            color: #6c757d;
        }

        .scam-types-section h2 {
            font-size: 1.25rem;
            font-weight: 600;
            color: #333;
            margin-bottom: 2rem;
            text-decoration: underline;
        }

        .scam-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }

        .scam-card {
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 1.5rem;
            background: white;
            display: flex;
            align-items: flex-start;
            gap: 1rem;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .scam-card:hover {
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transform: translateY(-1px);
        }

        .scam-icon {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 1.25rem;
            flex-shrink: 0;
        }

        .scam-content h5 {
            font-size: 1rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            color: #333;
        }

        .scam-content p {
            font-size: 0.875rem;
            color: #666;
            margin: 0;
            line-height: 1.4;
        }

        .banking { background-color: #dc3545; }
        .tech { background-color: #007bff; }
        .shopping { background-color: #ffc107; color: #333 !important; }
        .romance { background-color: #e91e63; }
        .phishing { background-color: #6f42c1; }
        .employment { background-color: #28a745; }
        .medical { background-color: #fd7e14; }
        .lottery { background-color: #17a2b8; }
        .other { background-color: #6c757d; }

        .other-card {
            grid-column: span 1;
            justify-self: center;
            max-width: 300px;
            width: 100%;
        }

        .support-section h2 {
            font-size: 1.25rem;
            font-weight: 600;
            color: #333;
            margin-bottom: 2rem;
            text-decoration: underline;
        }

        .support-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 1rem;
        }

        .support-card {
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 1.5rem;
            background: #f8f9fa;
        }

        .support-card h5 {
            font-size: 1rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            color: #333;
        }

        .support-card .phone {
            font-size: 1rem;
            font-weight: 600;
            color: #007bff;
            margin-bottom: 0.25rem;
        }

        .support-card .description {
            font-size: 0.875rem;
            color: #666;
            margin: 0;
        }

        .chat-bubble {
            display: none;
        }

        @media (max-width: 768px) {
            .emergency-section {
                flex-direction: column;
            }
            
            .scam-grid {
                grid-template-columns: 1fr;
            }
            
            .support-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Hero Banner -->
    <div class="hero-banner">
        <div class="container">
            <h1 style="font-size: 2rem; font-weight: 700; color: #333; margin-bottom: 0.5rem;">Scam Recovery Support</h1>
            <p style="font-size: 1rem; color: #666; margin: 0;">Immediate assistance and resources for scam victims<br>We're here to help you through this difficult time</p>
        </div>
    </div>

    <div class="container">
        <!-- Immediate Assistance Section -->
        <div class="assistance-section">
            <div class="assistance-card">
                <div class="text-center">
                    <h3 style="font-size: 1.125rem; font-weight: 600; color: #333; margin-bottom: 1.5rem;">Need immediate assistance?</h3>
                    <button class="hotline-btn">Call Singapore Scam Hotline: 1799</button>
                </div>
                
                <div class="emergency-section">
                    <div class="emergency-box">
                        <div class="icon">⚠️</div>
                        <div class="title">Emergency</div>
                        <div class="number">999</div>
                        <div class="description">If you feel unsafe right now</div>
                    </div>
                    <div class="emergency-box report-box">
                        <div class="icon">📞</div>
                        <div class="title">Report Scam</div>
                        <div class="number">1800-255-0000</div>
                        <div class="description">Call to file a report after being scammed</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Scam Types Section -->
        <div class="scam-types-section">
            <h2>What kind of scam did you experience?</h2>
            
            <div class="scam-grid">
                <!-- Banking/Financial Scam -->
                <div class="scam-card" onclick="selectScamType('banking')">
                    <div class="scam-icon banking">$</div>
                    <div class="scam-content">
                        <h5>Banking/Financial Scam</h5>
                        <p>Unauthorized transactions, fake bank calls, investment scams, loan scams</p>
                    </div>
                </div>

                <!-- Tech Support/Government -->
                <div class="scam-card" onclick="selectScamType('tech')">
                    <div class="scam-icon tech">💻</div>
                    <div class="scam-content">
                        <h5>Tech Support/Government</h5>
                        <p>Fake Microsoft/Apple calls, IRS scams, Social security calls</p>
                    </div>
                </div>

                <!-- Online Shopping Scam -->
                <div class="scam-card" onclick="selectScamType('shopping')">
                    <div class="scam-icon shopping">🛒</div>
                    <div class="scam-content">
                        <h5>Online Shopping Scam</h5>
                        <p>Fake websites, payment without delivery, counterfeit products</p>
                    </div>
                </div>

                <!-- Social Media/Romance Scam -->
                <div class="scam-card" onclick="selectScamType('romance')">
                    <div class="scam-icon romance">💖</div>
                    <div class="scam-content">
                        <h5>Social Media/Romance Scam</h5>
                        <p>Social media impersonation, fake dating profiles, friendship scams</p>
                    </div>
                </div>

                <!-- Identity Theft/Phishing -->
                <div class="scam-card" onclick="selectScamType('phishing')">
                    <div class="scam-icon phishing">🎭</div>
                    <div class="scam-content">
                        <h5>Identity Theft/Phishing</h5>
                        <p>Personal info stolen, fake emails, password compromise, account takeover</p>
                    </div>
                </div>

                <!-- Employment -->
                <div class="scam-card" onclick="selectScamType('employment')">
                    <div class="scam-icon employment">💼</div>
                    <div class="scam-content">
                        <h5>Employment</h5>
                        <p>Fake job offers, work-from-home scams, advance fees</p>
                    </div>
                </div>

                <!-- Insurance/Medical -->
                <div class="scam-card" onclick="selectScamType('medical')">
                    <div class="scam-icon medical">🏥</div>
                    <div class="scam-content">
                        <h5>Insurance/Medical</h5>
                        <p>Fake Medicare, medical billing scams, health insurance fraud</p>
                    </div>
                </div>

                <!-- Lottery/Prize Scams -->
                <div class="scam-card" onclick="selectScamType('lottery')">
                    <div class="scam-icon lottery">🎁</div>
                    <div class="scam-content">
                        <h5>Lottery/Prize Scams</h5>
                        <p>Fake winnings, advance fee lotteries, prize scams</p>
                    </div>
                </div>
            </div>

            <!-- Other/Not Sure - Centered -->
            <div style="display: flex; justify-content: center;">
                <div class="scam-card other-card" onclick="selectScamType('other')">
                    <div class="scam-icon other">❓</div>
                    <div class="scam-content">
                        <h5>Other/Not Sure</h5>
                        <p>General recovery guidance</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Emotional Support & Reporting Section -->
        <div class="support-section" style="margin-bottom: 4rem;">
            <h2>Emotional Support & Reporting</h2>
            
            <div class="support-grid">
                <!-- Samaritans of Singapore -->
                <div class="support-card">
                    <h5>Samaritans of Singapore</h5>
                    <div class="phone">1800-221-4444</div>
                    <p class="description">24/7 emotional support</p>
                </div>

                <!-- Silver Ribbon Singapore -->
                <div class="support-card">
                    <h5>Silver Ribbon Singapore</h5>
                    <div class="phone">6386 1928</div>
                    <p class="description">Mental health support for seniors</p>
                </div>

                <!-- Report via i-Witness -->
                <div class="support-card">
                    <h5>Report via i-Witness</h5>
                    <div class="phone">police.gov.sg/i-Witness</div>
                    <p class="description">24/7 emotional support</p>
                </div>

                <!-- ScamShield App -->
                <div class="support-card">
                    <h5>ScamShield App</h5>
                    <div class="phone">Download from App Store/Google Play</div>
                    <p class="description">Block scam calls & messages</p>
                </div>
            </div>
        </div>
    </div>

    <script>
        function selectScamType(type) {
            // You can implement navigation to specific guidance pages here
            alert('Selected scam type: ' + type + '\nRedirecting to specific guidance...');
            // Example: window.location.href = 'ScamGuidance.aspx?type=' + type;
        }

        function openChat() {
            alert('Opening chat support...');
            // Implement chat functionality here
        }
    </script>
</asp:Content>