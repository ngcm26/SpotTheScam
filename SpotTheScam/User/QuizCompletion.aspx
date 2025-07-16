<%@ Page Title="Quiz Completed!" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="QuizCompletion.aspx.cs" Inherits="SpotTheScam.User.QuizCompletion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --brand-orange: #D36F2D;
            --brand-navy: #051D40;
        }

        .current-points-badge {
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

        .points-section {
            background: #e8f4e8;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: center;
        }

        .points-earned {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 20px;
            margin-bottom: 25px;
        }

        .points-display {
            background: var(--brand-orange);
            color: white;
            padding: 20px 30px;
            border-radius: 15px;
            font-size: 2rem;
            font-weight: 700;
            min-width: 150px;
        }

        .points-label {
            color: var(--brand-navy);
            font-size: 1.1rem;
            font-weight: 600;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }

        .stat-item {
            background: #f8f9fa;
            border-radius: 10px;
            padding: 25px 20px;
            border-left: 4px solid var(--brand-orange);
            text-align: center;
        }

        .stat-number {
            font-size: 2rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 5px;
        }

        .stat-label {
            color: #6c757d;
            font-weight: 500;
            font-size: 0.9rem;
        }

        .achievement-section {
            background: #fff3cd;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: left;
        }

        .achievement-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }

        .achievement-title::before {
            content: "🏆";
            margin-right: 8px;
            font-size: 1.2rem;
        }

        .achievement-badges {
            display: flex;
            justify-content: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        .badge-item {
            background: #ffd700;
            color: #8b7500;
            padding: 10px 20px;
            border-radius: 25px;
            font-weight: 600;
            font-size: 0.9rem;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .motivational-section {
            background: #f8f9fa;
            border-left: 4px solid var(--brand-orange);
            padding: 20px;
            margin-bottom: 30px;
            border-radius: 0 8px 8px 0;
        }

        .motivational-text {
            font-size: 0.95rem;
            color: #333;
            margin: 0;
        }

        .action-buttons {
            display: flex;
            justify-content: center;
            gap: 20px;
            margin-top: 40px;
            flex-wrap: wrap;
        }

        .btn-primary-custom {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 15px 30px;
            border-radius: 10px;
            font-weight: 600;
            font-size: 1.1rem;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 10px;
            transition: background-color 0.3s ease;
        }

        .btn-primary-custom:hover {
            background: #b45a22;
            color: white;
            text-decoration: none;
        }

        .btn-secondary-custom {
            background: white;
            border: 2px solid var(--brand-navy);
            color: var(--brand-navy);
            padding: 15px 30px;
            border-radius: 10px;
            font-weight: 600;
            font-size: 1.1rem;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 10px;
            transition: all 0.3s ease;
        }

        .btn-secondary-custom:hover {
            background: var(--brand-navy);
            color: white;
            text-decoration: none;
        }

        @media (max-width: 768px) {
            .success-card {
                padding: 25px;
                margin: 0 15px;
            }
            
            .current-points-badge {
                position: static;
                display: inline-block;
                margin-bottom: 20px;
            }
            
            .points-section, .achievement-section {
                padding: 20px;
            }
            
            .page-container {
                padding: 20px 0 40px 0;
            }

            .points-earned {
                flex-direction: column;
                gap: 10px;
            }
            
            .action-buttons {
                flex-direction: column;
                align-items: center;
            }
            
            .btn-primary-custom,
            .btn-secondary-custom {
                width: 100%;
                max-width: 300px;
                justify-content: center;
            }

            .stats-grid {
                grid-template-columns: repeat(2, 1fr);
                gap: 15px;
            }
        }

        @media (max-width: 480px) {
            .stats-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <div class="container">
            <div class="success-card">
                <!-- Current Points Badge -->
                <div class="current-points-badge">
                    <span>Current Points: <asp:Label ID="lblCurrentPoints" runat="server" Text="0" /> ⭐</span>
                </div>
                
                <!-- Success Icon -->
                <div class="success-icon">
                    🏆
                </div>
                
                <!-- Success Message -->
                <h1 class="success-title">QUIZ COMPLETED!</h1>
                <p class="success-subtitle">Great job completing the <strong><asp:Label ID="lblQuizName" runat="server" Text="Phone Scams Quiz" /></strong>!</p>
                
                <!-- Points Section -->
                <div class="points-section">
                    <div class="points-earned">
                        <div class="points-display">
                            +<asp:Label ID="lblPointsEarned" runat="server" Text="0" />
                        </div>
                        <div class="points-label">Points Earned from This Quiz!</div>
                    </div>

                    <div class="stats-grid">
                        <div class="stat-item">
                            <div class="stat-number"><asp:Label ID="lblCorrectAnswers" runat="server" Text="0" /></div>
                            <div class="stat-label">Correct Answers</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-number"><asp:Label ID="lblTotalQuestions" runat="server" Text="10" /></div>
                            <div class="stat-label">Total Questions</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-number"><asp:Label ID="lblAccuracy" runat="server" Text="0" />%</div>
                            <div class="stat-label">Accuracy Rate</div>
                        </div>
                    </div>
                </div>

                <!-- Achievement Badges -->
                <div class="achievement-section">
                    <h3 class="achievement-title">Achievements Unlocked:</h3>
                    <div class="achievement-badges" runat="server" id="achievementBadges">
                        <!-- Badges will be populated from code-behind -->
                    </div>
                </div>
                
                <!-- Motivational Message -->
                <div class="motivational-section">
                    <p class="motivational-text">
                        <strong>Well done!</strong> You've successfully enhanced your knowledge about phone scam protection. 
                        Keep learning to stay safe from evolving fraud tactics!
                    </p>
                </div>
                
                <!-- Action Buttons -->
                <div class="action-buttons">
                    <a href="Quizzes.aspx" class="btn-primary-custom">
                        📚 Take Another Quiz
                    </a>
                    <a href="UserHome.aspx" class="btn-secondary-custom">
                        🏠 Back to Home
                    </a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>