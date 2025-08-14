<%@ Page Title="Points Store" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserPointsStore.aspx.cs" Inherits="SpotTheScam.User.UserPointsStore" %>

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
        
        .store-card {
            border: 2px solid #e5e7eb;
            border-radius: 12px;
            padding: 16px;
            height: 100%;
            transition: all 0.3s ease;
            background: white;
            max-width: 380px;
            margin: 0 auto;
        }
        
        .store-grid {
            max-width: 800px;
            margin: 0 auto;
        }
        
        .store-card.featured {
            border-color: #22c55e;
            position: relative;
        }
        
        .store-card.unavailable {
            border-color: #ef4444;
            opacity: 0.7;
        }
        
        .store-card:hover:not(.unavailable) {
            transform: translateY(-3px);
            box-shadow: 0 6px 15px rgba(0,0,0,0.1);
        }
        
        .card-icon {
            width: 36px;
            height: 36px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            margin-bottom: 12px;
        }
        
        .points-cost {
            color: #f59e0b;
            font-weight: 700;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 4px;
        }
        
        .purchase-btn {
            background: #22c55e;
            border: none;
            color: white;
            padding: 8px 16px;
            border-radius: 20px;
            font-weight: 600;
            width: 100%;
            transition: all 0.3s ease;
            font-size: 14px;
        }
        
        .purchase-btn:hover {
            background: #16a34a;
            color: white;
        }
        
        .purchase-btn:disabled {
            background: #d1d5db !important;
            color: #6b7280 !important;
            cursor: not-allowed !important;
        }
        
        .purchase-btn:disabled:hover {
            background: #d1d5db !important;
            color: #6b7280 !important;
            cursor: not-allowed !important;
            transform: none !important;
        }
        
        .store-description {
            color: #6b7280;
            font-size: 12px;
            line-height: 1.4;
        }
        
        .store-title {
            font-size: 16px;
            font-weight: 700;
            margin-bottom: 8px;
        }
        
        .title-section {
            margin-left: 200px;
            max-width: 600px;
            position: relative;
            top: -80px;
        }
    </style>
    
    <script type="text/javascript">
        function updateStoreItemStyles() {
            var currentPoints = parseInt('<%= Session["CurrentPoints"] != null ? Session["CurrentPoints"].ToString() : "0" %>');

            // Define item costs
            var itemCosts = {
                'report': 50,
                'guide': 40,
                'quiz': 50,
                'webinar': 150
            };

            // Update each store card based on available points
            updateCardStyle('report-card', currentPoints >= itemCosts.report);
            updateCardStyle('guide-card', currentPoints >= itemCosts.guide);
            updateCardStyle('quiz-card', currentPoints >= itemCosts.quiz);
            updateCardStyle('webinar-card', false); // Always unavailable as per design
        }

        function updateCardStyle(cardId, canAfford) {
            var card = document.getElementById(cardId);
            if (card) {
                if (canAfford) {
                    card.className = 'store-card featured';
                } else {
                    card.className = 'store-card unavailable';
                }
            }
        }

        // Run when page loads
        window.onload = function () {
            updateStoreItemStyles();
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <!-- Navigation Tabs -->
        <div class="navbar-tabs">
            <a href="#" class="nav-tab active">
                🏪 Points Store
            </a>
            <a href="UserAchievements.aspx" class="nav-tab">
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
                    <h1 class="h3 fw-bold mb-2">Points Store</h1>
                    <p class="text-muted mb-0" style="font-size: 14px;">Spend your hard-earned points on useful resources and premium features</p>
                </div>
            </div>
        </div>
        
        <!-- Store Items Grid -->
        <div class="store-grid" style="margin-top: -80px;">
            <div class="row g-2 gy-4">
                <!-- Detailed Scam Report -->
                <div class="col-6">
                    <div id="report-card" class="store-card featured">
                        <div class="card-icon" style="background: linear-gradient(135deg, #3b82f6, #1d4ed8);">
                            📊
                        </div>
                        <h5 class="store-title">Detailed Scam Report</h5>
                        <div class="points-cost mb-2">
                            ⭐ 50 points
                        </div>
                        <p class="store-description mb-3">
                            Get a personalized report on the latest scam trends in your area with prevention tips
                        </p>
                        <asp:Button ID="btnPurchaseReport" runat="server" 
                            CssClass="purchase-btn" 
                            Text="Purchase Now"
                            OnClick="PurchaseItem_Click"
                            CommandArgument="DetailedScamReport|50" />
                    </div>
                </div>
                
                <!-- Safety Guide PDF -->
                <div class="col-6">
                    <div id="guide-card" class="store-card featured">
                        <div class="card-icon" style="background: linear-gradient(135deg, #22c55e, #16a34a);">
                            📄
                        </div>
                        <h5 class="store-title">Safety Guide PDF</h5>
                        <div class="points-cost mb-2">
                            ⭐ 40 points
                        </div>
                        <p class="store-description mb-3">
                            Download a comprehensive 20-page safety guide you can share with family
                        </p>
                        <asp:Button ID="btnPurchaseGuide" runat="server" 
                            CssClass="purchase-btn" 
                            Text="Purchase Now"
                            OnClick="PurchaseItem_Click"
                            CommandArgument="SafetyGuidePDF|40" />
                    </div>
                </div>
                
                <!-- Advanced Quiz -->
                <div class="col-6">
                    <div id="quiz-card" class="store-card featured">
                        <div class="card-icon" style="background: linear-gradient(135deg, #f59e0b, #d97706);">
                            🔒
                        </div>
                        <h5 class="store-title">Advanced Quiz</h5>
                        <div class="points-cost mb-2">
                            ⭐ 50 points
                        </div>
                        <p class="store-description mb-3">
                            Unlock expert-level quizzes with real-world scam scenarios and case-studies
                        </p>
                        <asp:Button ID="btnPurchaseQuiz" runat="server" 
                            CssClass="purchase-btn" 
                            Text="Purchase Now"
                            OnClick="PurchaseItem_Click"
                            CommandArgument="AdvancedQuiz|50" />
                    </div>
                </div>
                
                <!-- Reserve Live Expert Webinar -->
                <div class="col-6">
                    <div id="webinar-card" class="store-card unavailable">
                        <div class="card-icon" style="background: linear-gradient(135deg, #06b6d4, #0891b2);">
                            💻
                        </div>
                        <h5 class="store-title">Reserve Live Expert Webinar</h5>
                        <div class="points-cost mb-2">
                            ⭐ 150 points
                        </div>
                        <p class="store-description mb-3">
                            Book your spot in upcoming live expert sessions on scam prevention and digital safety
                        </p>
                        <asp:Button ID="btnPurchaseWebinar" runat="server" 
                            CssClass="purchase-btn" 
                            Text="Purchase Now"
                            OnClick="PurchaseItem_Click"
                            CommandArgument="ExpertWebinar|150"
                            Enabled="false" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>