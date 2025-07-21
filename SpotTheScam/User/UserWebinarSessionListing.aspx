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
            cursor: not-allowed;
            opacity: 0.7;
        }

        .page-container {
            padding-bottom: 60px;
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
            
            .page-container {
                padding-bottom: 40px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
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
                            <span class="points-badge"><asp:Label ID="lblCurrentPoints" runat="server" Text="0" /> ⭐</span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Dynamic Session Cards -->
            <div class="row">
                <div class="col-12">
                    <asp:Repeater ID="rptSessions" runat="server">
                        <ItemTemplate>
                            <div class="session-card">
                                <div class="session-header">
                                    <span class="session-date">📅 <%# Eval("SessionDate", "{0:MMMM dd, yyyy}") %></span>
                                    <span class="session-time">🕐 <%# Eval("StartTime") %> - <%# Eval("EndTime") %></span>
                                    <span class="session-type <%# GetSessionTypeClass(Eval("PointsRequired")) %>"><%# GetSessionTypeText(Eval("PointsRequired"), Eval("SessionType")) %></span>
                                </div>
                                
                                <h3 class="session-title"><%# Eval("Title") %></h3>
                                <p class="session-description"><%# Eval("Description") %></p>
                                
                                <div class="expert-info">
                                    <img src="<%# GetExpertImage(Eval("SessionId")) %>" alt="<%# Eval("ExpertName") %>" class="expert-avatar" />
                                    <div>
                                        <div class="expert-name"><%# Eval("ExpertName") %></div>
                                        <div class="expert-title"><%# Eval("ExpertTitle") %></div>
                                    </div>
                                </div>
                                
                                <div class="session-details">
                                    <div class="detail-item">
                                        <span class="detail-icon">👥</span>
                                        <span><%# GetParticipantText(Eval("CurrentRegistrations"), Eval("MaxParticipants")) %></span>
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
                                    <div>
                                        <asp:PlaceHolder ID="phPointsRequired" runat="server" Visible='<%# Convert.ToInt32(Eval("PointsRequired")) > 0 %>'>
                                            <div class="points-required"><%# GetPointsText(Eval("PointsRequired"), Eval("SessionType")) %></div>
                                        </asp:PlaceHolder>
                                        <div class="spots-remaining">
                                            <span class="spots-count"><%# Eval("AvailableSpots") %></span> spots remaining
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="phSessionButton" runat="server" Visible='<%# IsSessionAvailable(Eval("AvailableSpots")) %>'>
                                        <a href='<%# "UserWebinarRegistration.aspx?sessionId=" + Eval("SessionId") %>' class='<%# GetButtonClass(Eval("AvailableSpots"), Eval("PointsRequired")) %>'><%# GetButtonText(Eval("AvailableSpots"), Eval("PointsRequired")) %></a>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phDisabledButton" runat="server" Visible='<%# !IsSessionAvailable(Eval("AvailableSpots")) %>'>
                                        <button class="need-points-btn" disabled><%# GetButtonText(Eval("AvailableSpots"), Eval("PointsRequired")) %></button>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:Content>