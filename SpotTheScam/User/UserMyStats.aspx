<%@ Page Title="My Stats" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserMyStats.aspx.cs" Inherits="SpotTheScam.User.UserMyStats" %>

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
        
        .stats-container {
            max-width: 900px;
            margin: 0 auto;
            margin-top: -40px;
        }
        
        .stats-card {
            background: linear-gradient(135deg, #FFC107, #FF8C00);
            border-radius: 20px;
            padding: 40px;
            color: white;
            text-align: center;
            box-shadow: 0 8px 25px rgba(255, 193, 7, 0.3);
        }
        
        .stats-icon {
            width: 80px;
            height: 80px;
            background: rgba(255, 255, 255, 0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin: 0 auto 20px auto;
        }
        
        .stats-title {
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 10px;
        }
        
        .stats-subtitle {
            font-size: 16px;
            margin-bottom: 30px;
            opacity: 0.9;
        }
        
        .stats-grid {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-top: 20px;
        }
        
        .stat-item {
            text-align: center;
        }
        
        .stat-number {
            font-size: 36px;
            font-weight: 700;
            line-height: 1;
            margin-bottom: 5px;
        }
        
        .stat-label {
            font-size: 14px;
            opacity: 0.9;
            font-weight: 500;
        }
        
        .progress-bar-container {
            background: rgba(255, 255, 255, 0.2);
            border-radius: 10px;
            height: 8px;
            margin-top: 10px;
            overflow: hidden;
        }
        
        .progress-bar {
            background: rgba(255, 255, 255, 0.8);
            height: 100%;
            border-radius: 10px;
            transition: width 0.3s ease;
        }
        
        @media (max-width: 768px) {
            .stats-grid {
                gap: 15px;
            }
            
            .stats-card {
                padding: 30px 20px;
            }
            
            .stat-number {
                font-size: 28px;
            }
        }
    </style>
    
    <script type="text/javascript">
        function updateCurrentPoints() {
            // Get current points from session (same pattern as UserPointsStore)
            var currentPoints = '<%= Session["CurrentPoints"] != null ? Session["CurrentPoints"].ToString() : "0" %>';
            var lblCurrentPoints = document.getElementById('<%= lblCurrentPoints.ClientID %>');
            var lblTotalPoints = document.getElementById('<%= lblTotalPoints.ClientID %>');
            
            // Update both current points and total points displays
            if (lblCurrentPoints) {
                lblCurrentPoints.innerText = currentPoints;
            }
            if (lblTotalPoints) {
                lblTotalPoints.innerText = currentPoints;
            }
        }

        // Run when page loads
        window.onload = function () {
            updateCurrentPoints();
            var progressValue = document.getElementById('<%= hiddenQuizProgress.ClientID %>').value;
            var progressBars = document.querySelectorAll('.progress-bar');
            if (progressBars.length > 0) {
                progressBars[0].style.width = progressValue + '%';
            }
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <!-- Navigation Tabs -->
        <div class="navbar-tabs">
            <a href="UserPointsStore.aspx" class="nav-tab">
                Points Store
            </a>
            <a href="UserAchievements.aspx" class="nav-tab">
                Achievements
            </a>
            <a href="#" class="nav-tab active">
                My Stats
            </a>
        </div>
        
        <!-- Header Section -->
        <div class="row mb-4">
            <div class="col-12">
                <!-- Current Points Display - Now Dynamic (same as UserPointsStore) -->
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
                    <h1 class="h3 fw-bold mb-2">My Stats</h1>
                    <p class="text-muted mb-0" style="font-size: 14px;">Track your learning progress and see how you're improving your scam protection skills</p>
                </div>
            </div>
        </div>
        
        <!-- Stats Container -->
        <div class="stats-container">
            <div class="row justify-content-center">
                <div class="col-12 col-lg-10">
                    <div class="stats-card">
                        <h2 class="stats-title">Your Learning Journey</h2>
                        <p class="stats-subtitle">
                            <asp:Label ID="lblMotivationalMessage" runat="server" Text="Keep up the great work protecting yourself from scams!"></asp:Label>
                        </p>
                        
                        <div class="stats-grid">
                            <div class="stat-item">
                                <div class="stat-number">
                                    <asp:Label ID="lblTotalPoints" runat="server" Text="0"></asp:Label>
                                </div>
                                <div class="stat-label">Total Points</div>
                            </div>
                            <div class="stat-item">
                                <div class="stat-number">
                                    <asp:Label ID="lblQuizzesCompleted" runat="server" Text="1"></asp:Label>/<asp:Label ID="lblTotalQuizzes" runat="server" Text="6"></asp:Label>
                                </div>
                                <div class="stat-label">Quizzes Done</div>
                                <div class="progress-bar-container">
                                    <div class="progress-bar">
                                        <asp:HiddenField ID="hiddenQuizProgress" runat="server" Value="17" />
                                    </div>
                                </div>
                            </div>
                            <div class="stat-item">
                                <div class="stat-number">
                                    <asp:Label ID="lblCurrentStreak" runat="server" Text="1"></asp:Label>
                                </div>
                                <div class="stat-label">Day Streak</div>
                            </div>
                            <div class="stat-item">
                                <div class="stat-number">
                                    <asp:Label ID="lblAchievementsUnlocked" runat="server" Text="1"></asp:Label>
                                </div>
                                <div class="stat-label">Achievements Unlocked</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>