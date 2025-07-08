<%@ Page Title="Quizzes" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Quizzes.aspx.cs" Inherits="SpotTheScam.User.Quizzes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --brand-orange: #D36F2D;
            --brand-navy: #051D40;
        }

        .hero-section {
            background: linear-gradient(135deg, #e8b3ff 0%, #ffa726 100%);
            padding: 30px 0;
            text-align: center;
            color: white;
            margin: -20px -15px 0 -15px;
        }

        .hero-section h1 {
            font-size: 1.8rem;
            font-weight: 700;
            margin-bottom: 0.5rem;
        }

        .hero-section p {
            font-size: 0.95rem;
            font-weight: 400;
            margin-bottom: 0;
        }

        .brain-icon {
            font-size: 2rem;
            margin-bottom: 0.5rem;
            display: block;
        }

        .points-section {
            background: linear-gradient(135deg, #ffa726 0%, #ffcc02 100%);
            padding: 25px;
            border-radius: 15px;
            margin: 25px auto;
            max-width: 1200px;
        }

        .points-card {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 10px;
            padding: 15px;
            margin: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .points-card h5 {
            color: var(--brand-navy);
            font-weight: 600;
            margin-bottom: 8px;
            font-size: 1rem;
        }

        .points-badge {
            background: #ffa726;
            color: white;
            padding: 3px 10px;
            border-radius: 15px;
            font-weight: 600;
            display: inline-block;
            margin-bottom: 5px;
            font-size: 0.8rem;
        }

        .rules-card {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 10px;
            padding: 15px;
            margin: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .rules-card h5 {
            color: var(--brand-navy);
            font-weight: 600;
            margin-bottom: 10px;
            font-size: 1rem;
        }

        .rule-item {
            display: flex;
            align-items: center;
            margin-bottom: 8px;
            padding: 6px;
            border-radius: 5px;
            font-size: 0.85rem;
        }

        .rule-item.unlimited {
            background-color: #e8f5e8;
        }

        .rule-item.retake {
            background-color: #e8f5e8;
        }

        .rule-item.practice {
            background-color: #e8f5e8;
        }

        .rule-item.attempt {
            background-color: #e8f5e8;
        }

        .rule-item.worry {
            background-color: #e3f2fd;
        }

        .rule-icon {
            margin-right: 8px;
            font-size: 1rem;
        }

        .search-section {
            margin: 25px 0;
            text-align: center;
        }

        .search-box {
            max-width: 600px;
            margin: 0 auto;
            position: relative;
        }

        .search-input {
            width: 100%;
            padding: 10px 40px 10px 15px;
            border: 2px solid #ddd;
            border-radius: 20px;
            font-size: 0.9rem;
            outline: none;
        }

        .search-input:focus {
            border-color: var(--brand-orange);
        }

        .search-btn {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            background: transparent;
            border: none;
            color: #666;
            cursor: pointer;
            font-size: 1.2rem;
            padding: 5px;
        }

        .search-btn:hover {
            color: var(--brand-orange);
        }

        .quiz-grid {
            margin-top: 25px;
            margin-bottom: 60px;
            max-width: 1200px;
            margin-left: auto;
            margin-right: auto;
        }

        .quiz-card {
            border-radius: 15px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            cursor: pointer;
            border: 3px solid transparent;
            height: 220px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .quiz-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.15);
        }

        .quiz-card.beginner {
            border-color: #4caf50;
            background: #f0fff0;
        }

        .quiz-card.intermediate {
            border-color: #2196f3;
            background: #f0f8ff;
        }

        .quiz-card.advanced {
            border-color: #ff9800;
            background: #fff8f0;
        }

        .quiz-icon {
            font-size: 2rem;
            margin-bottom: 10px;
            display: block;
            text-align: center;
        }

        .quiz-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 8px;
            text-align: center;
            line-height: 1.3;
        }

        .quiz-description {
            color: #666;
            text-align: center;
            margin-bottom: 10px;
            font-size: 0.9rem;
        }

        .difficulty-section {
            text-align: center;
        }

        .difficulty-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: capitalize;
            margin-bottom: 8px;
        }

        .difficulty-badge.beginner {
            background-color: #4caf50;
            color: white;
        }

        .difficulty-badge.intermediate {
            background-color: #2196f3;
            color: white;
        }

        .difficulty-badge.advanced {
            background-color: #ff9800;
            color: white;
        }

        .difficulty-dots {
            display: flex;
            justify-content: center;
            gap: 4px;
            margin-bottom: 8px;
        }

        .dot {
            width: 6px;
            height: 6px;
            border-radius: 50%;
        }

        .dot.filled.beginner {
            background-color: #4caf50;
        }

        .dot.filled.intermediate {
            background-color: #4caf50;
        }

        .dot.filled.advanced {
            background-color: #4caf50;
        }

        .dot.empty {
            background-color: #ddd;
        }

        .quiz-arrow {
            text-align: center;
            color: var(--brand-orange);
            font-size: 1rem;
        }

        .section-title {
            text-align: center;
            margin-bottom: 20px;
            color: var(--brand-navy);
            font-weight: 600;
            font-size: 1.3rem;
        }

        /* Make container full width */
        .container {
            max-width: 100%;
            padding: 0 15px;
        }

        /* Responsive adjustments */
        @media (max-width: 768px) {
            .quiz-card {
                height: auto;
                min-height: 200px;
            }
            
            .hero-section {
                padding: 20px 0;
            }
            
            .hero-section h1 {
                font-size: 1.5rem;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Hero Section -->
    <div class="hero-section">
        <div class="container">
            <span class="brain-icon">🧠</span>
            <h1>Test your knowledge</h1>
            <p>Learn how to stay safe from scams with our interactive quizzes designed specifically for seniors</p>
        </div>
    </div>

    <!-- Points and Rules Section -->
    <div class="points-section">
        <div class="row">
            <!-- Points Structure -->
            <div class="col-md-6">
                <div class="points-card">
                    <h5>📊 Points Structure</h5>
                    <div class="mb-2">
                        <span class="points-badge">+10</span>
                        <span>Correct Answer (each question)</span>
                    </div>
                    <div class="mb-2">
                        <span class="points-badge">+5</span>
                        <span>No Hints Bonus (per question)</span>
                    </div>
                    <div class="mb-2">
                        <span class="points-badge">+10</span>
                        <span>Quiz Completion (base reward)</span>
                    </div>
                    <div class="mb-2">
                        <span class="points-badge">+20</span>
                        <span>First-Time Bonus (one time only)</span>
                    </div>
                    <div class="mb-2">
                        <span class="points-badge">+50</span>
                        <span>Perfect Score Bonus (100% correct)</span>
                    </div>
                    <div class="mb-2">
                        <span class="points-badge">+10</span>
                        <span>Daily Practice (return each day)</span>
                    </div>
                </div>
            </div>

            <!-- Quiz Rules -->
            <div class="col-md-6">
                <div class="rules-card">
                    <h5>📝 Quiz Rules & Retakes</h5>
                    <div class="rule-item unlimited">
                        <span class="rule-icon">♾️</span>
                        <span><strong>Unlimited retakes</strong> - practice as much as you want</span>
                    </div>
                    <div class="rule-item retake">
                        <span class="rule-icon">🔄</span>
                        <span><strong>Points only on first completion</strong> - prevents farming</span>
                    </div>
                    <div class="rule-item practice">
                        <span class="rule-icon">🎯</span>
                        <span><strong>Practice mode Saver</strong> - shows what you've done before</span>
                    </div>
                    <div class="rule-item attempt">
                        <span class="rule-icon">📊</span>
                        <span><strong>Attempt tracking</strong> - keeps count of your practice sessions</span>
                    </div>
                    <div class="rule-item worry">
                        <span class="rule-icon">ℹ️</span>
                        <span><strong>Don't worry about taking retakes!</strong> You can always retakes courses to improve your knowledge</span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Search Section -->
    <div class="search-section">
        <div class="search-box">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search for quizzes..." />
            <button type="button" class="search-btn" onclick="searchQuizzes()">🔍</button>
        </div>
    </div>

    <!-- Educational Quizzes Section -->
    <div class="quiz-grid">
        <h2 class="section-title">Educational Quizzes</h2>
        <p class="text-center text-muted mb-4">Choose a quiz topic to test your knowledge and learn how to protect yourself from different types of scams</p>
        
        <div class="row">
            <!-- Phone Scams Quiz -->
            <div class="col-md-4">
                <div class="quiz-card beginner" onclick="location.href='PhoneScamsQuiz.aspx'">
                    <div>
                        <span class="quiz-icon">📞</span>
                        <h4 class="quiz-title">Phone Scams</h4>
                        <p class="quiz-description">Learn to identify suspicious calls</p>
                    </div>
                    <div class="difficulty-section">
                        <div class="difficulty-badge beginner">Beginner</div>
                        <div class="difficulty-dots">
                            <span class="dot filled beginner"></span>
                            <span class="dot empty"></span>
                            <span class="dot empty"></span>
                        </div>
                        <div class="quiz-arrow">→</div>
                    </div>
                </div>
            </div>

            <!-- Online Banking Safety Quiz -->
            <div class="col-md-4">
                <div class="quiz-card intermediate" onclick="location.href='OnlineBankingQuiz.aspx'">
                    <div>
                        <span class="quiz-icon">🏦</span>
                        <h4 class="quiz-title">Online Banking Safety</h4>
                        <p class="quiz-description">Protect your account from hackers</p>
                    </div>
                    <div class="difficulty-section">
                        <div class="difficulty-badge intermediate">Intermediate</div>
                        <div class="difficulty-dots">
                            <span class="dot filled intermediate"></span>
                            <span class="dot filled intermediate"></span>
                            <span class="dot empty"></span>
                        </div>
                        <div class="quiz-arrow">→</div>
                    </div>
                </div>
            </div>

            <!-- Phishing & Fake Emails Quiz -->
            <div class="col-md-4">
                <div class="quiz-card advanced" onclick="location.href='PhishingEmailsQuiz.aspx'">
                    <div>
                        <span class="quiz-icon">📧</span>
                        <h4 class="quiz-title">Phishing & Fake Emails</h4>
                        <p class="quiz-description">Spot fake emails and messages</p>
                    </div>
                    <div class="difficulty-section">
                        <div class="difficulty-badge advanced">Advanced</div>
                        <div class="difficulty-dots">
                            <span class="dot filled advanced"></span>
                            <span class="dot filled advanced"></span>
                            <span class="dot filled advanced"></span>
                        </div>
                        <div class="quiz-arrow">→</div>
                    </div>
                </div>
            </div>

            <!-- Social Media Safety Quiz -->
            <div class="col-md-4">
                <div class="quiz-card beginner" onclick="location.href='SocialMediaSafetyQuiz.aspx'" style="border-color: #dc3545; background: #fdf2f2;">
                    <div>
                        <span class="quiz-icon">📱</span>
                        <h4 class="quiz-title">Social Media Safety</h4>
                        <p class="quiz-description">Protect your personal information online</p>
                    </div>
                    <div class="difficulty-section">
                        <div class="difficulty-badge beginner">Beginner</div>
                        <div class="difficulty-dots">
                            <span class="dot filled beginner"></span>
                            <span class="dot empty"></span>
                            <span class="dot empty"></span>
                        </div>
                        <div class="quiz-arrow">→</div>
                    </div>
                </div>
            </div>

            <!-- In-person and Door-to-door Scams Quiz -->
            <div class="col-md-4">
                <div class="quiz-card intermediate" onclick="location.href='DoorToDoorScamsQuiz.aspx'" style="border-color: #e91e63; background: #fce4ec;">
                    <div>
                        <span class="quiz-icon">🚪</span>
                        <h4 class="quiz-title">In-person and door-to-door scams</h4>
                        <p class="quiz-description">Spot and avoid face-to-face scams</p>
                    </div>
                    <div class="difficulty-section">
                        <div class="difficulty-badge intermediate">Intermediate</div>
                        <div class="difficulty-dots">
                            <span class="dot filled intermediate"></span>
                            <span class="dot filled intermediate"></span>
                            <span class="dot empty"></span>
                        </div>
                        <div class="quiz-arrow">→</div>
                    </div>
                </div>
            </div>

            <!-- Latest Scam Trends Quiz -->
            <div class="col-md-4">
                <div class="quiz-card advanced" onclick="location.href='LatestScamTrendsQuiz.aspx'" style="border-color: #673ab7; background: #f3f0ff;">
                    <div>
                        <span class="quiz-icon">📈</span>
                        <h4 class="quiz-title">Latest Scam Trends</h4>
                        <p class="quiz-description">Stay updated on the newest scam trends</p>
                    </div>
                    <div class="difficulty-section">
                        <div class="difficulty-badge advanced">Advanced</div>
                        <div class="difficulty-dots">
                            <span class="dot filled advanced"></span>
                            <span class="dot filled advanced"></span>
                            <span class="dot filled advanced"></span>
                        </div>
                        <div class="quiz-arrow">→</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        function searchQuizzes() {
            var searchTerm = document.getElementById('<%= txtSearch.ClientID %>').value.trim();

            if (searchTerm === '') {
                return;
            }

            // Simple client-side search - hide/show quiz cards based on search term
            var quizCards = document.querySelectorAll('.quiz-card');
            var found = false;

            quizCards.forEach(function (card) {
                var title = card.querySelector('.quiz-title').textContent.toLowerCase();
                var description = card.querySelector('.quiz-description').textContent.toLowerCase();
                var searchLower = searchTerm.toLowerCase();

                if (title.includes(searchLower) || description.includes(searchLower)) {
                    card.style.display = 'flex';
                    found = true;
                } else {
                    card.style.display = 'none';
                }
            });

            // Clear search to show all quizzes again
            if (!found) {
                quizCards.forEach(function (card) {
                    card.style.display = 'flex';
                });
                document.getElementById('<%= txtSearch.ClientID %>').value = '';
            }
        }
        
        // Allow Enter key to trigger search
        document.getElementById('<%= txtSearch.ClientID %>').addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                searchQuizzes();
            }
        });
    </script>
</asp:Content>