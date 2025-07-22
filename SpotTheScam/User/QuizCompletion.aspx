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
        }

        .points-display {
            font-size: 3rem;
            font-weight: 700;
            color: #28a745;
            margin: 0;
        }

        .points-label {
            font-size: 1.2rem;
            color: #333;
            font-weight: 600;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
            margin-top: 20px;
        }

        .stat-item {
            text-align: center;
            padding: 15px;
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        .stat-number {
            font-size: 2rem;
            font-weight: 700;
            color: var(--brand-orange);
            margin-bottom: 5px;
        }

        .stat-label {
            font-size: 0.9rem;
            color: #666;
            font-weight: 500;
        }

        .achievement-section {
            background: #f8f9fa;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
        }

        .achievement-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 20px;
            text-align: center;
        }

        .achievements-container {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 15px;
        }

        .achievement-badge {
            display: inline-flex;
            align-items: center;
            padding: 8px 16px;
            border-radius: 20px;
            font-weight: 600;
            font-size: 0.9rem;
            gap: 8px;
        }

        .badge-gold {
            background: linear-gradient(135deg, #FFD700, #FFA500);
            color: #000;
            box-shadow: 0 2px 8px rgba(255, 215, 0, 0.3);
        }

        .badge-platinum {
            background: linear-gradient(135deg, #E5E4E2, #C0C0C0);
            color: #000;
            box-shadow: 0 2px 8px rgba(192, 192, 192, 0.3);
        }

        .badge-silver {
            background: linear-gradient(135deg, #C0C0C0, #A8A8A8);
            color: #000;
            box-shadow: 0 2px 8px rgba(192, 192, 192, 0.2);
        }

        .badge-bronze {
            background: linear-gradient(135deg, #CD7F32, #B87333);
            color: white;
            box-shadow: 0 2px 8px rgba(205, 127, 50, 0.2);
        }

        .badge-default {
            background: linear-gradient(135deg, #6c757d, #5a6268);
            color: white;
        }

        .badge-icon::before {
            content: "🏆";
        }

        .motivational-section {
            background: linear-gradient(135deg, var(--brand-orange), #E8814B);
            color: white;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 30px;
        }

        .motivational-text {
            font-size: 1.1rem;
            line-height: 1.6;
            margin: 0;
            text-align: center;
        }

        .action-buttons {
            display: flex;
            gap: 20px;
            justify-content: center;
            flex-wrap: wrap;
        }

        .btn-primary-custom {
            background: linear-gradient(135deg, var(--brand-orange), #E8814B);
            color: white;
            padding: 12px 30px;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
            font-size: 1rem;
        }

        .btn-primary-custom:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(211, 111, 45, 0.3);
            color: white;
            text-decoration: none;
        }

        .btn-secondary-custom {
            background: white;
            color: var(--brand-orange);
            padding: 12px 30px;
            border: 2px solid var(--brand-orange);
            border-radius: 8px;
            font-weight: 600;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
            font-size: 1rem;
        }

        .btn-secondary-custom:hover {
            background: var(--brand-orange);
            color: white;
            text-decoration: none;
            transform: translateY(-2px);
        }

        /* Responsive Design */
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
                <!-- Current Points Badge - FIXED: No hardcoded values -->
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

    <!-- ENHANCED JavaScript to handle points display and any hardcoded values -->
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            console.log('=== QuizCompletion Points Update START ===');
            
            // Get the actual current points from the server-side label
            var lblCurrentPoints = document.getElementById('<%= lblCurrentPoints.ClientID %>');
            var actualCurrentPoints = lblCurrentPoints ? lblCurrentPoints.textContent : '0';
            
            console.log('Server-side current points:', actualCurrentPoints);
            
            // COMPREHENSIVE search and replace for any hardcoded "30" values
            var allElements = document.querySelectorAll('*');
            var replacementCount = 0;
            
            for (var i = 0; i < allElements.length; i++) {
                var element = allElements[i];
                if (element.children.length === 0) { // Only leaf text elements
                    var text = element.textContent;
                    if (text) {
                        var trimmedText = text.trim();
                        
                        // Replace exact hardcoded matches
                        if (trimmedText === 'Current Points: 30') {
                            element.innerHTML = 'Current Points: ' + actualCurrentPoints;
                            replacementCount++;
                            console.log('Replaced exact match: Current Points: 30 -> Current Points: ' + actualCurrentPoints);
                        }
                        else if (trimmedText === '30') {
                            var parent = element.parentElement;
                            // Check if this is a points display based on parent context
                            if (parent && (parent.className.includes('points') || 
                                         parent.className.includes('badge') ||
                                         parent.textContent.includes('Current Points'))) {
                                element.textContent = actualCurrentPoints;
                                replacementCount++;
                                console.log('Replaced standalone 30 in points context -> ' + actualCurrentPoints);
                            }
                        }
                        // Check for partial matches in longer text
                        else if (text.includes('Current Points: 30')) {
                            element.innerHTML = text.replace('Current Points: 30', 'Current Points: ' + actualCurrentPoints);
                            replacementCount++;
                            console.log('Replaced partial match: ' + text);
                        }
                    }
                }
            }
            
            // Specific badge update
            var badgeSpans = document.querySelectorAll('.current-points-badge span, .points-badge span');
            for (var j = 0; j < badgeSpans.length; j++) {
                var originalText = badgeSpans[j].textContent;
                if (originalText.includes('30') || originalText.includes('Points: 0')) {
                    badgeSpans[j].innerHTML = 'Current Points: ' + actualCurrentPoints + ' ⭐';
                    console.log('Updated badge span: ' + originalText + ' -> Current Points: ' + actualCurrentPoints + ' ⭐');
                }
            }
            
            console.log('Total QuizCompletion replacements made: ' + replacementCount);
            console.log('=== QuizCompletion Points Update END ===');
            
            // Delayed check for any dynamically loaded content
            setTimeout(function() {
                console.log('Running delayed QuizCompletion points check...');
                var delayedElements = document.querySelectorAll('*');
                var delayedReplacements = 0;
                
                for (var k = 0; k < delayedElements.length; k++) {
                    var element = delayedElements[k];
                    if (element.children.length === 0) {
                        var text = element.textContent;
                        if (text && (text.trim() === '30' || text.trim() === 'Current Points: 30')) {
                            var parent = element.parentElement;
                            if (parent && (parent.className.includes('points') || 
                                         parent.className.includes('badge'))) {
                                if (text.trim() === '30') {
                                    element.textContent = actualCurrentPoints;
                                } else {
                                    element.textContent = 'Current Points: ' + actualCurrentPoints;
                                }
                                delayedReplacements++;
                            }
                        }
                    }
                }
                
                if (delayedReplacements > 0) {
                    console.log('Made ' + delayedReplacements + ' delayed QuizCompletion replacements');
                }
            }, 2000);
        });
    </script>
</asp:Content>