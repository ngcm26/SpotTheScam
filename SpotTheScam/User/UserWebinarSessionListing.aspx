<%@ Page Title="Webinar Sessions" Language="C#" MasterPageFile="User.Master" AutoEventWireup="true" CodeBehind="UserWebinarSessionListing.aspx.cs" Inherits="SpotTheScam.User.UserWebinarSessionListing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .header-section {
            background: linear-gradient(135deg, var(--brand-orange) 0%, #e67e22 100%);
            color: white;
            padding: 25px 0;
            margin-bottom: 30px;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
        }

        .header-title {
            font-size: 1.2rem;
            font-weight: 600;
            margin: 0;
            color: white;
        }

        .filter-section {
            padding: 20px 0;
            margin-bottom: 30px;
        }

        .filter-label {
            font-weight: 600;
            margin-right: 15px;
            color: #333;
        }

        .filter-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 8px 20px;
            border-radius: 20px;
            margin-right: 10px;
            margin-bottom: 10px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.3s ease;
            text-decoration: none;
        }

        .filter-btn.active {
            background: #b45a22;
        }

        .filter-btn.inactive {
            background: #e9ecef;
            color: #6c757d;
        }

        .filter-btn:hover {
            background: #b45a22;
            color: white;
        }

        .topic-dropdown {
            border: 1px solid #ddd;
            border-radius: 20px;
            padding: 8px 15px;
            background: white;
            color: #333;
            min-width: 150px;
        }

        .points-badge {
            background: #FFC107;
            color: #000;
            padding: 5px 15px;
            border-radius: 15px;
            font-weight: 600;
            font-size: 0.9rem;
        }

        .session-card {
            background: white;
            border: 1px solid #e9ecef;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .session-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
        }

        .session-header {
            display: flex;
            align-items: center;
            margin-bottom: 15px;
        }

        .session-date {
            color: #6c757d;
            margin-right: 20px;
            font-size: 0.9rem;
        }

        .session-time {
            color: #6c757d;
            margin-right: 20px;
            font-size: 0.9rem;
        }

        .session-type {
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
        }

        .type-free {
            background: #d4edda;
            color: #155724;
        }

        .type-premium {
            background: #f8d7da;
            color: #721c24;
        }

        .session-title {
            font-size: 1.3rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 10px;
        }

        .session-description {
            color: #666;
            font-size: 0.95rem;
            line-height: 1.5;
            margin-bottom: 20px;
        }

        .expert-info {
            display: flex;
            align-items: center;
            background: #f8f9fa;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
        }

        .expert-avatar {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            margin-right: 15px;
            object-fit: cover;
        }

        .expert-name {
            font-weight: 600;
            color: var(--brand-navy);
            margin: 0;
        }

        .expert-title {
            color: #666;
            font-size: 0.9rem;
            margin: 0;
        }

        .session-details {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
        }

        .detail-item {
            display: flex;
            align-items: center;
            font-size: 0.9rem;
            color: #666;
        }

        .detail-icon {
            width: 20px;
            height: 20px;
            margin-right: 8px;
            color: var(--brand-orange);
        }

        .session-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .spots-remaining {
            font-size: 0.9rem;
            color: #666;
        }

        .spots-count {
            color: var(--brand-orange);
            font-weight: 600;
        }

        .register-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 20px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .register-btn:hover {
            background: #b45a22;
            color: white;
        }

        .reserve-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 20px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .reserve-btn:hover {
            background: #b45a22;
        }

        .points-required {
            background: #dc3545;
            color: white;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 0.8rem;
            font-weight: 600;
            margin-bottom: 10px;
            display: inline-block;
        }

        .need-points-btn {
            background: #6c757d;
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 20px;
            font-weight: 600;
            cursor: pointer;
        }

        .available-badge {
            background: #28a745;
            color: white;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 0.8rem;
            font-weight: 600;
            margin-bottom: 10px;
            display: inline-block;
        }

        @media (max-width: 768px) {
            .session-footer {
                flex-direction: column;
                gap: 15px;
                align-items: stretch;
            }
            
            .register-btn, .reserve-btn, .need-points-btn {
                width: 100%;
                text-align: center;
            }
            
            .session-details {
                flex-direction: column;
                gap: 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Header Section -->
    <div class="header-section">
        <div class="container">
            <div class="row align-items-center justify-content-center">
                <div class="col-auto">
                    <h1 class="header-title">Book your spot in live sessions with cybersecurity experts and fraud specialists</h1>
                </div>
            </div>
        </div>
    </div>

    <div class="container">
        <!-- Filter Section -->
        <div class="filter-section">
            <div class="row align-items-center">
                <div class="col-md-8">
                    <div class="row align-items-center">
                        <div class="col-auto">
                            <span class="filter-label">View:</span>
                        </div>
                        <div class="col-auto">
                            <asp:Button ID="btnAllSessions" runat="server" 
                                CssClass="filter-btn active" 
                                OnClick="FilterSessions_Click" 
                                CommandArgument="All"
                                Text="All Sessions" />
                            <asp:Button ID="btnFreeOnly" runat="server" 
                                CssClass="filter-btn inactive" 
                                OnClick="FilterSessions_Click" 
                                CommandArgument="Free"
                                Text="Free Only" />
                            <asp:Button ID="btnPremium" runat="server" 
                                CssClass="filter-btn inactive" 
                                OnClick="FilterSessions_Click" 
                                CommandArgument="Premium"
                                Text="Premium" />
                        </div>
                    </div>
                    <div class="row align-items-center mt-2">
                        <div class="col-auto">
                            <span class="filter-label">Topic:</span>
                        </div>
                        <div class="col-auto">
                            <asp:DropDownList ID="ddlTopics" runat="server" 
                                CssClass="topic-dropdown" 
                                AutoPostBack="true" 
                                OnSelectedIndexChanged="ddlTopics_SelectedIndexChanged">
                                <asp:ListItem Value="All" Text="All Topics" Selected="True" />
                                <asp:ListItem Value="Banking" Text="Banking Security" />
                                <asp:ListItem Value="Phone" Text="Phone Scams" />
                                <asp:ListItem Value="Social" Text="Social Media Safety" />
                                <asp:ListItem Value="Email" Text="Email Scams" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="col-md-4 text-end">
                    <div>
                        <span class="filter-label">Current Points:</span>
                        <span class="points-badge">75 ⭐</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Session Cards -->
        <div class="row">
            <div class="col-12">
                <!-- Session 1 -->
                <div class="session-card" runat="server" id="session1">
                    <div class="session-header">
                        <span class="session-date">📅 June 15, 2025</span>
                        <span class="session-time">🕐 2:00 PM - 3:00 PM</span>
                        <span class="session-type type-free">FREE</span>
                    </div>
                    
                    <h3 class="session-title">Protecting Your Online Banking</h3>
                    <p class="session-description">
                        Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.
                    </p>
                    
                    <div class="expert-info">
                        <img src="/Images/expert2.jpg" alt="Dr Harvey Blue" class="expert-avatar" />
                        <div>
                            <div class="expert-name">Dr Harvey Blue</div>
                            <div class="expert-title">Cybersecurity Specialist, 15+ years experience</div>
                        </div>
                    </div>
                    
                    <div class="session-details">
                        <div class="detail-item">
                            <span class="detail-icon">👥</span>
                            <span>Up to 100 Participants</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">⏱️</span>
                            <span>60 minutes + Q&A</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">💻</span>
                            <span>Live via video call</span>
                        </div>
                    </div>
                    
                    <div class="session-footer">
                        <div class="spots-remaining">
                            <span class="spots-count">47</span> spots remaining
                        </div>
                        <button class="register-btn">Register Now</button>
                    </div>
                </div>

                <!-- Session 2 -->
                <div class="session-card" runat="server" id="session2">
                    <div class="session-header">
                        <span class="session-date">📅 June 17, 2025</span>
                        <span class="session-time">🕐 10:00 AM - 11:30 AM</span>
                        <span class="session-type type-premium">PREMIUM</span>
                    </div>
                    
                    <h3 class="session-title">Small Group: Latest Phone Scam Tactics</h3>
                    <p class="session-description">
                        Intimate session with max 10 participants. Deep dive into current phone scam methods with personalized Q&A time.
                    </p>
                    
                    <div class="expert-info">
                        <img src="/Images/expert3.jpg" alt="Officer James Wilson" class="expert-avatar" />
                        <div>
                            <div class="expert-name">Officer James Wilson</div>
                            <div class="expert-title">Investigating phone and romance scams for 10+ years, helping victims recover</div>
                        </div>
                    </div>
                    
                    <div class="session-details">
                        <div class="detail-item">
                            <span class="detail-icon">👥</span>
                            <span>Max 10 Participants</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">⏱️</span>
                            <span>90 minutes</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">💡</span>
                            <span>Personalized advice</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">📞</span>
                            <span>Extended Q&A time</span>
                        </div>
                    </div>
                    
                    <div class="session-footer">
                        <div>
                            <div class="points-required">50 POINTS</div>
                            <div class="spots-remaining">
                                Only <span class="spots-count">3</span> spots remaining
                            </div>
                        </div>
                        <button class="reserve-btn">Reserve Spot</button>
                    </div>
                </div>

                <!-- Session 3 -->
                <div class="session-card" runat="server" id="session3">
                    <div class="session-header">
                        <span class="session-date">📅 June 19, 2025</span>
                        <span class="session-time">🕐 3:00 PM - 4:00 PM</span>
                        <span class="session-type type-premium">PREMIUM</span>
                    </div>
                    
                    <h3 class="session-title">VIP One-on-One Safety Consultation</h3>
                    <p class="session-description">
                        Private consultation to review your personal digital security, analyze any suspicious communications, and create a personalized safety plan.
                    </p>
                    
                    <div class="expert-info">
                        <img src="/Images/expert1.jpg" alt="Maria Rodriguez" class="expert-avatar" />
                        <div>
                            <div class="expert-name">Maria Rodriguez</div>
                            <div class="expert-title">Digital Safety Educator, Senior Specialist</div>
                        </div>
                    </div>
                    
                    <div class="session-details">
                        <div class="detail-item">
                            <span class="detail-icon">👤</span>
                            <span>One-on-one session</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">⏱️</span>
                            <span>60 minutes</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">📋</span>
                            <span>Personal safety plan</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">📞</span>
                            <span>Follow-up support</span>
                        </div>
                    </div>
                    
                    <div class="session-footer">
                        <div>
                            <div class="points-required">100 POINTS</div>
                            <div class="available-badge">Available</div>
                        </div>
                        <button class="need-points-btn">Need More Points</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>