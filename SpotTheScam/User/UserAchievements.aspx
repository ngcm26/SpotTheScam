<%@ Page Title="Achievements" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserAchievements.aspx.cs" Inherits="SpotTheScam.User.UserAchievements" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .navbar-tabs {
            display: flex;
            justify-content: center;
            gap: 8px;
            margin-bottom: 50px;
            margin-top: 40px;
            padding: 0 20px;
        }
        
        .nav-tab {
            background: #f8f9fa;
            border: 1px solid #e5e7eb;
            color: #6b7280;
            padding: 10px 20px;
            border-radius: 12px;
            text-decoration: none;
            font-weight: 500;
            font-size: 14px;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            gap: 6px;
        }
        
        .nav-tab.active {
            background: #D36F2D;
            color: white;
            border-color: #D36F2D;
        }
        
        .nav-tab:hover:not(.active) {
            background: #e5e7eb;
            color: #374151;
            text-decoration: none;
        }
        
        .points-badge {
            background: #FFC107;
            color: #000;
            padding: 5px 15px;
            border-radius: 15px;
            font-weight: 600;
            font-size: 0.9rem;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }
        
        .current-points-label {
            color: #333;
            font-weight: 600;
        }
        
        .title-section {
            margin-left: 200px;
            max-width: 600px;
            position: relative;
            top: -80px;
        }
        
        .achievements-grid {
            max-width: 800px;
            margin: 0 auto;
            margin-top: -80px;
        }
        
        .achievement-card {
            border: 2px solid #e5e7eb;
            border-radius: 12px;
            padding: 20px;
            height: 100%;
            transition: all 0.3s ease;
            background: white;
            text-align: center;
        }
        
        .achievement-card.unlocked {
            border-color: #22c55e;
            background: white;
        }
        
        .achievement-card.locked {
            border-color: #d1d5db;
            opacity: 0.6;
            background: #f9fafb;
        }
        
        .achievement-card:hover:not(.locked) {
            transform: translateY(-3px);
            box-shadow: 0 6px 15px rgba(0,0,0,0.1);
        }
        
        .achievement-icon {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            margin: 0 auto 15px auto;
        }
        
        .achievement-icon.unlocked {
            background: linear-gradient(135deg, #22c55e, #16a34a);
            color: white;
        }
        
        .achievement-icon.locked {
            background: #e5e7eb;
            color: #9ca3af;
        }
        
        .achievement-title {
            font-size: 16px;
            font-weight: 700;
            margin-bottom: 8px;
            color: #1f2937;
        }
        
        .achievement-description {
            color: #6b7280;
            font-size: 12px;
            line-height: 1.4;
            margin-bottom: 15px;
        }
        
        .achievement-reward {
            background: #FFC107;
            color: #000;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            display: inline-block;
        }
        
        .achievement-date {
            color: #22c55e;
            font-size: 11px;
            font-weight: 500;
            margin-top: 8px;
        }
        
        .progress-bar {
            width: 100%;
            height: 6px;
            background: #e5e7eb;
            border-radius: 3px;
            margin: 10px 0;
            overflow: hidden;
        }
        
        .progress-fill {
            height: 100%;
            background: linear-gradient(90deg, #22c55e, #16a34a);
            transition: width 0.3s ease;
        }
        
        .progress-text {
            font-size: 11px;
            color: #6b7280;
            margin-top: 5px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <!-- Navigation Tabs -->
        <div class="navbar-tabs">
            <a href="UserPointsStore.aspx" class="nav-tab">
                🏪 Points Store
            </a>
            <a href="#" class="nav-tab active">
                🏆 Achievements
            </a>
            <a href="UserMyStats.aspx" class="nav-tab">
                📊 My Stats
            </a>
        </div>
        
        <!-- Header Section -->
        <div class="row mb-4">
            <div class="col-12">
                <!-- Current Points Display - Now Dynamic -->
                <div class="container">
                    <div class="text-end mb-4">
                        <span class="me-3 current-points-label">Current Points:</span>
                        <span class="points-badge">
                            ⭐ <asp:Label ID="lblCurrentPoints" runat="server" Text="0" />
                        </span>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Page Title and Description -->
        <div class="row mb-4" style="margin-top: -20px;">
            <div class="col-12 d-flex">
                <div class="title-section">
                    <h1 class="h3 fw-bold mb-2">Achievements</h1>
                    <p class="text-muted mb-0" style="font-size: 14px;">Track your progress and unlock rewards as you master scam prevention skills</p>
                </div>
            </div>
        </div>
        
        <!-- Achievements Grid -->
        <div class="achievements-grid">
            <div class="row g-4 gy-5">
                <!-- Bronze Protector Achievement (Unlocked) -->
                <div class="col-6">
                    <div class="achievement-card unlocked" style="border-color: #D36F2D;">
                        <div class="achievement-icon unlocked" style="background: #D36F2D;">
                            🥉
                        </div>
                        <h5 class="achievement-title">Bronze Protector</h5>
                        <p class="achievement-description">
                            Complete 3 quizzes
                        </p>
                        <div class="achievement-status" style="color: #22c55e; font-weight: 600; font-size: 12px; margin-bottom: 8px;">
                            ✅ UNLOCKED
                        </div>
                        <div class="achievement-reward">+50 bonus points</div>
                    </div>
                </div>
                
                <!-- Silver Guardian Achievement (In Progress) -->
                <div class="col-6">
                    <div class="achievement-card locked">
                        <div class="achievement-icon locked">
                            🥈
                        </div>
                        <h5 class="achievement-title">Silver Guardian</h5>
                        <p class="achievement-description">
                            Complete 6 quizzes
                        </p>
                        <div class="progress-text" style="margin-bottom: 8px;">Progress: 3/10</div>
                        <div class="achievement-reward">+100 bonus points</div>
                    </div>
                </div>
                
                <!-- Gold Defender Achievement (Locked) -->
                <div class="col-6">
                    <div class="achievement-card locked">
                        <div class="achievement-icon locked">
                            🥇
                        </div>
                        <h5 class="achievement-title">Gold Defender</h5>
                        <p class="achievement-description">
                            Master all categories
                        </p>
                        <div class="progress-text" style="margin-bottom: 8px;">Progress: 1/6</div>
                        <div class="achievement-reward">+200 bonus points</div>
                    </div>
                </div>
                
                <!-- Streak Master Achievement (In Progress) -->
                <div class="col-6">
                    <div class="achievement-card locked">
                        <div class="achievement-icon locked">
                            🔥
                        </div>
                        <h5 class="achievement-title">Streak Master</h5>
                        <p class="achievement-description">
                            7-day learning streak
                        </p>
                        <div class="progress-text" style="margin-bottom: 8px;">Progress: 2/7</div>
                        <div class="achievement-reward">+75 bonus points</div>
                    </div>
                </div>
                
                <!-- Perfect Score Achievement (Locked) -->
                <div class="col-12" style="margin-top: 30px;">
                    <div class="row justify-content-center">
                        <div class="col-6">
                            <div class="achievement-card locked">
                                <div class="achievement-icon locked">
                                    🎯
                                </div>
                                <h5 class="achievement-title">Perfect Score</h5>
                                <p class="achievement-description">
                                    100% on any quiz
                                </p>
                                <div class="progress-text" style="margin-bottom: 8px;">Progress: 0/1</div>
                                <div class="achievement-reward">+150 bonus points</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>