<%@ Page Title="Phone Scams Quiz" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="PhoneScamsQuiz.aspx.cs" Inherits="SpotTheScam.User.PhoneScamsQuiz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --brand-orange: #D36F2D;
            --brand-navy: #051D40;
        }

        .quiz-container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }

        .quiz-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .current-points {
            background: #ffa726;
            color: white;
            padding: 8px 15px;
            border-radius: 20px;
            font-weight: 600;
            font-size: 0.9rem;
        }

        .quiz-title {
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.2rem;
            font-weight: 600;
            color: var(--brand-navy);
        }

        .quiz-icon {
            background: #4caf50;
            color: white;
            border-radius: 50%;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 10px;
            font-size: 1rem;
        }

        .question-info {
            font-size: 0.9rem;
            color: #666;
        }

        .progress-container {
            margin-bottom: 20px;
        }

        .progress-bar {
            background: #e0e0e0;
            height: 8px;
            border-radius: 4px;
            overflow: hidden;
        }

        .progress-fill {
            background: #4caf50;
            height: 100%;
            transition: width 0.3s ease;
        }

        .points-notification {
            text-align: center;
            margin-bottom: 20px;
            padding: 15px;
            border-radius: 10px;
            font-weight: 600;
            display: none;
        }

        .points-notification.correct {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .points-notification.correct-no-hint {
            background: #d1ecf1;
            color: #0c5460;
            border: 1px solid #bee5eb;
        }

        .quiz-card {
            background: white;
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .question-number {
            background: #4caf50;
            color: white;
            border-radius: 50%;
            width: 40px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 600;
            margin-bottom: 15px;
        }

        .question-text {
            font-size: 1.1rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 15px;
        }

        .scenario-box {
            background: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin-bottom: 20px;
            border-radius: 5px;
        }

        .scenario-text {
            font-size: 0.95rem;
            color: #856404;
            margin: 0;
        }

        .answer-option {
            background: #f8f9fa;
            border: 2px solid #e9ecef;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 10px;
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
        }

        .answer-option:hover {
            border-color: #4caf50;
            background: #f0fff0;
        }

        .answer-option.selected {
            border-color: #4caf50;
            background: #e8f5e8;
        }

        .answer-option.correct {
            border-color: #4caf50;
            background: #d4edda;
        }

        .answer-option.incorrect {
            border-color: #dc3545;
            background: #f8d7da;
        }

        .option-letter {
            background: #6c757d;
            color: white;
            border-radius: 50%;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 15px;
            font-weight: 600;
            flex-shrink: 0;
        }

        .answer-option.correct .option-letter {
            background: #28a745;
        }

        .answer-option.incorrect .option-letter {
            background: #dc3545;
        }

        .option-text {
            flex: 1;
            font-size: 0.95rem;
        }

        .feedback-icon {
            margin-left: 10px;
            font-size: 1.2rem;
        }

        .quiz-buttons {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 20px;
        }

        .btn-back {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 0.9rem;
            transition: all 0.3s ease;
        }

        .btn-back:disabled {
            background: #ccc;
            cursor: not-allowed;
            opacity: 0.5;
        }

        .btn-hint {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 0.9rem;
        }

        .btn-hint:disabled {
            background: #ccc;
            cursor: not-allowed;
        }

        .feedback-section {
            margin-top: 20px;
            padding: 20px;
            border-radius: 10px;
            display: none;
        }

        .feedback-section.show {
            display: block;
        }

        .feedback-section.correct {
            background: #d4edda;
            border: 1px solid #c3e6cb;
        }

        .feedback-section.incorrect {
            background: #f8d7da;
            border: 1px solid #f5c6cb;
        }

        .feedback-title {
            font-weight: 600;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
        }

        .feedback-title.correct {
            color: #155724;
        }

        .feedback-title.incorrect {
            color: #721c24;
        }

        .feedback-text {
            font-size: 0.95rem;
            line-height: 1.5;
            margin-bottom: 15px;
        }

        .feedback-text.correct {
            color: #155724;
        }

        .feedback-text.incorrect {
            color: #721c24;
        }

        .continue-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 600;
        }

        .hint-section {
            background: #e3f2fd;
            border: 1px solid #bbdefb;
            border-radius: 10px;
            padding: 15px;
            margin-top: 15px;
            display: none;
        }

        .hint-section.show {
            display: block;
        }

        .hint-title {
            font-weight: 600;
            color: #1565c0;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
        }

        .hint-text {
            color: #1565c0;
            font-size: 0.9rem;
            line-height: 1.4;
        }

        .click-feedback {
            color: #dc3545;
            font-size: 0.85rem;
            margin-left: 10px;
            cursor: pointer;
            text-decoration: underline;
        }

        .disabled {
            pointer-events: none;
            opacity: 0.6;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="quiz-container">
        <!-- Quiz Header -->
        <div class="quiz-header">
            <div class="current-points">
                Current Points: <asp:Label ID="lblCurrentPoints" runat="server" Text="75" />
            </div>
            <div class="quiz-title">
                <div class="quiz-icon">📞</div>
                Phone Scams Quiz
            </div>
            <div class="question-info">
                Question <asp:Label ID="lblCurrentQuestion" runat="server" Text="1" /> of <asp:Label ID="lblTotalQuestions" runat="server" Text="10" />
            </div>
        </div>

        <!-- Progress Bar -->
        <div class="progress-container">
            <div class="progress-bar">
                <div class="progress-fill" id="progressFill" style="width: 10%"></div>
            </div>
        </div>

        <!-- Points Notification -->
        <div id="pointsNotification" class="points-notification">
            <span id="pointsText"></span>
        </div>

        <!-- Quiz Card -->
        <div class="quiz-card">
            <div class="question-number" id="questionNumber">1</div>
            <div class="question-text" id="questionText">What should you do if someone calls claiming to be from your bank?</div>
            
            <div class="scenario-box" id="scenarioBox">
                <p class="scenario-text" id="scenarioText">📧 Scenario: You receive a call from someone saying, "This John from ABC Bank. We've detected suspicious activity on your account and we need to verify your information immediately"</p>
            </div>

            <div id="answerOptions">
                <div class="answer-option" onclick="selectAnswer('A')" id="optionA">
                    <div class="option-letter">A</div>
                    <div class="option-text">Give them your account number and PIN to verify your identity</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
                <div class="answer-option" onclick="selectAnswer('B')" id="optionB">
                    <div class="option-letter">B</div>
                    <div class="option-text">Hang up and call your bank directly using the number on your card or statement</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
                <div class="answer-option" onclick="selectAnswer('C')" id="optionC">
                    <div class="option-letter">C</div>
                    <div class="option-text">Ask them to call you back in 10 minutes to give you the time to find your information</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
                <div class="answer-option" onclick="selectAnswer('D')" id="optionD">
                    <div class="option-letter">D</div>
                    <div class="option-text">Transfer the call to a family member who is better with technology</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
            </div>

            <!-- Hint Section -->
            <div class="hint-section" id="hintSection">
                <div class="hint-title">🔍 Hint</div>
                <div class="hint-text" id="hintText">Real banks will never ask you to give them information they already have. When in doubt, hang up and call the number on your card yourself!</div>
            </div>

            <!-- Feedback Section -->
            <div class="feedback-section" id="feedbackSection">
                <div class="feedback-title" id="feedbackTitle">
                    <span id="feedbackIcon"></span>
                    <span id="feedbackTitleText"></span>
                </div>
                <div class="feedback-text" id="feedbackText"></div>
                <button class="continue-btn" onclick="nextQuestion(); return false;">Continue Learning</button>
            </div>

            <div class="quiz-buttons">
                <button class="btn-back" onclick="goBack()">← Back</button>
                <button class="btn-hint" id="hintBtn" onclick="showHint()">Hint</button>
            </div>
        </div>
    </div>

    <!-- Hidden fields to store data -->
    <asp:HiddenField ID="hdnCurrentQuestionIndex" runat="server" Value="0" />
    <asp:HiddenField ID="hdnSelectedAnswer" runat="server" />
    <asp:HiddenField ID="hdnHintUsed" runat="server" Value="false" />
    <asp:HiddenField ID="hdnQuizData" runat="server" />

    <script>
        let currentQuestionIndex = 0;
        let selectedAnswer = '';
        let hintUsed = false;
        let totalPoints = parseInt('<%= lblCurrentPoints.Text %>');
        let questionAnswered = false;
        let answeredQuestions = []; // Track which questions have been answered
        let correctAnswersCount = 0; // Track number of correct answers

        // Quiz data with 10 questions
        const quizData = [
            {
                question: "What should you do if someone calls claiming to be from your bank?",
                scenario: "📧 Scenario: You receive a call from someone saying, \"This John from ABC Bank. We've detected suspicious activity on your account and we need to verify your information immediately\"",
                options: {
                    A: "Give them your account number and PIN to verify your identity",
                    B: "Hang up and call your bank directly using the number on your card or statement",
                    C: "Ask them to call you back in 10 minutes to give you the time to find your information",
                    D: "Transfer the call to a family member who is better with technology"
                },
                correctAnswer: "B",
                hint: "Real banks will never ask you to give them information they already have. When in doubt, hang up and call the number on your card yourself!",
                feedback: {
                    A: "❌ Never give personal information over the phone. Scammers want your PIN to access your money!",
                    B: "✅ Correct! This is the safest approach. Real banks already have your details.",
                    C: "❌ This gives scammers time to prepare better lies. Hang up instead!",
                    D: "❌ Don't involve family members in potential scams. Handle it yourself by hanging up."
                },
                explanation: "Real banks already have your information and will never ask for your PIN over the phone. Always hang up and call back using official numbers."
            },
            {
                question: "You receive a call saying you've won a prize but need to pay fees first. What should you do?",
                scenario: "📞 Scenario: \"Congratulations! You've won $10,000 in our lottery! To claim your prize, you just need to pay $500 in processing fees first.\"",
                options: {
                    A: "Pay the fees immediately to claim your prize",
                    B: "Ask for their company details and call them back later",
                    C: "Hang up immediately - legitimate prizes never require upfront payments",
                    D: "Negotiate to pay smaller fees first"
                },
                correctAnswer: "C",
                hint: "Remember: If you have to pay to receive money, it's always a scam. Real prizes are free!",
                feedback: {
                    A: "❌ This is a classic advance fee scam. You'll lose your money and get nothing.",
                    B: "❌ Even calling back gives scammers another chance to convince you.",
                    C: "✅ Correct! Legitimate prizes never require any upfront payment.",
                    D: "❌ Any payment request for a 'prize' is a red flag. Don't negotiate with scammers."
                },
                explanation: "Legitimate sweepstakes and contests never require winners to pay fees, taxes, or charges upfront to claim prizes."
            },
            {
                question: "Someone calls claiming to be from the IRS saying you owe taxes. What's the correct response?",
                scenario: "📞 Scenario: \"This is Agent Smith from the IRS. You owe $3,000 in back taxes and must pay immediately or face arrest.\"",
                options: {
                    A: "Provide your SSN to verify your identity",
                    B: "Ask for payment options to resolve the issue quickly",
                    C: "Hang up - the IRS doesn't call about taxes owed",
                    D: "Transfer them to your accountant"
                },
                correctAnswer: "C",
                hint: "The IRS always contacts taxpayers by mail first, never by phone for initial contact about owed taxes.",
                feedback: {
                    A: "❌ Never give your SSN over the phone. The IRS already has this information.",
                    B: "❌ You're falling for the scam. The IRS doesn't demand immediate payment over the phone.",
                    C: "✅ Correct! The IRS contacts people by mail, not phone, for tax issues.",
                    D: "❌ Don't involve others in potential scams. Just hang up."
                },
                explanation: "The IRS never calls taxpayers to demand immediate payment or threatens arrest. They always send official notices by mail first."
            },
            {
                question: "A caller says your computer has viruses and offers to fix it remotely. What should you do?",
                scenario: "📞 Scenario: \"This is Microsoft support. We've detected viruses on your computer. Allow us remote access to fix the problem immediately.\"",
                options: {
                    A: "Give them remote access to fix the problem",
                    B: "Ask for their employee ID and callback number",
                    C: "Hang up - tech companies don't make unsolicited calls",
                    D: "Ask them to prove they're from Microsoft"
                },
                correctAnswer: "C",
                hint: "Microsoft, Apple, and other tech companies never call customers unsolicited about computer problems.",
                feedback: {
                    A: "❌ This gives scammers full access to steal your personal information and money.",
                    B: "❌ Scammers can provide fake credentials. Don't engage with them.",
                    C: "✅ Correct! Legitimate tech companies never make unsolicited support calls.",
                    D: "❌ Don't give scammers opportunities to convince you with fake 'proof'."
                },
                explanation: "Tech support scams are common. Real companies like Microsoft never call customers unsolicited about computer problems."
            },
            {
                question: "You get a call about suspicious activity on your credit card. What's the safest action?",
                scenario: "📞 Scenario: \"We're calling from Visa security. There's been suspicious activity on your card ending in 1234. We need to verify some transactions.\"",
                options: {
                    A: "Provide the full card number to verify it's yours",
                    B: "Give them the CVV code to confirm your identity",
                    C: "Hang up and call the number on the back of your card",
                    D: "Ask them to text you the suspicious transactions"
                },
                correctAnswer: "C",
                hint: "Even if they know some card details, always verify by calling the official number yourself.",
                feedback: {
                    A: "❌ Never give your full card number over the phone, even if they seem legitimate.",
                    B: "❌ Your CVV code should never be shared over the phone with callers.",
                    C: "✅ Correct! Always call the official number to verify any card issues.",
                    D: "❌ Don't give scammers your phone number or continue the conversation."
                },
                explanation: "Even if callers know some of your card details, always hang up and call the official customer service number to verify any issues."
            },
            {
                question: "Someone calls offering a great investment opportunity with guaranteed returns. Your response?",
                scenario: "📞 Scenario: \"I'm calling with an exclusive investment opportunity. Guaranteed 20% returns in just 30 days. But you must decide now!\"",
                options: {
                    A: "Ask for more details about the investment",
                    B: "Request written information to review",
                    C: "Hang up - guaranteed high returns don't exist",
                    D: "Ask to speak with their supervisor"
                },
                correctAnswer: "C",
                hint: "All investments carry risk. Anyone promising guaranteed high returns is lying.",
                feedback: {
                    A: "❌ Engaging gives scammers more opportunity to pressure you into investing.",
                    B: "❌ Scammers can provide fake documents. Don't continue the conversation.",
                    C: "✅ Correct! No legitimate investment guarantees high returns without risk.",
                    D: "❌ Don't waste time with scammers. Their 'supervisor' is just another scammer."
                },
                explanation: "Legitimate investments always carry risk. Anyone promising guaranteed high returns with no risk is running a scam."
            },
            {
                question: "A caller claims to be from your utility company threatening service disconnection. What should you do?",
                scenario: "📞 Scenario: \"This is City Electric. Your power will be shut off in 2 hours unless you pay $400 immediately with a prepaid card.\"",
                options: {
                    A: "Rush to buy a prepaid card to avoid disconnection",
                    B: "Give them your bank account information for payment",
                    C: "Hang up and call your utility company directly",
                    D: "Ask for a supervisor to negotiate payment terms"
                },
                correctAnswer: "C",
                hint: "Utility companies send written notices before disconnection and accept multiple payment methods, not just prepaid cards.",
                feedback: {
                    A: "❌ Prepaid cards are a red flag. Legitimate companies don't demand this payment method.",
                    B: "❌ Never give bank details over the phone to unexpected callers.",
                    C: "✅ Correct! Always verify with your utility company using their official number.",
                    D: "❌ Don't negotiate with scammers. Verify the situation through official channels."
                },
                explanation: "Utility companies send written notices before disconnection and offer multiple payment options. Demanding immediate payment with prepaid cards is a scam tactic."
            },
            {
                question: "You receive a call about a family member in trouble needing bail money. What's your first step?",
                scenario: "📞 Scenario: \"Your grandson is in jail and needs $2,000 bail money immediately. Don't tell his parents - he's embarrassed. Send money right away!\"",
                options: {
                    A: "Send the money immediately to help your grandson",
                    B: "Ask for the jail's phone number to call directly",
                    C: "Hang up and contact your family member directly",
                    D: "Ask the caller to put your grandson on the phone"
                },
                correctAnswer: "C",
                hint: "Scammers use emotional urgency and secrecy to prevent you from verifying the story. Always verify independently.",
                feedback: {
                    A: "❌ This is likely a grandparent scam. Your family member is probably safe at home.",
                    B: "❌ Scammers can provide fake numbers. Contact your family member directly instead.",
                    C: "✅ Correct! Always verify emergencies by contacting family members directly.",
                    D: "❌ Scammers often have accomplices who can pretend to be your family member."
                },
                explanation: "The 'grandparent scam' uses emotional manipulation and urgency. Always verify emergencies by contacting family members directly using numbers you know."
            },
            {
                question: "A caller offers to lower your credit card interest rates. What should you do?",
                scenario: "📞 Scenario: \"This is your final notice! We can lower your credit card interest rates to 0%. Press 1 to speak with an agent about this limited-time offer.\"",
                options: {
                    A: "Press 1 to learn about the offer",
                    B: "Give them your credit card information to check eligibility",
                    C: "Hang up - legitimate card companies don't make these calls",
                    D: "Ask which credit card company they represent"
                },
                correctAnswer: "C",
                hint: "Credit card companies contact customers through mail or secure online messages, not unsolicited phone calls.",
                feedback: {
                    A: "❌ Pressing buttons confirms your number is active and leads to more scam calls.",
                    B: "❌ Never give credit card details to unsolicited callers.",
                    C: "✅ Correct! These are always scams designed to steal your card information.",
                    D: "❌ Don't engage with scammers. They'll claim to represent whatever company you mention."
                },
                explanation: "Credit card interest rate reduction calls are common scams. Legitimate card companies don't make unsolicited offers over the phone."
            },
            {
                question: "Someone calls claiming you're eligible for a government grant. What's the appropriate response?",
                scenario: "📞 Scenario: \"Congratulations! You qualify for a $9,000 government grant that never needs to be repaid. We just need your bank details to deposit it.\"",
                options: {
                    A: "Provide your bank account information for the deposit",
                    B: "Ask for documentation to be mailed to you",
                    C: "Hang up - the government doesn't call about grants",
                    D: "Ask for their government employee ID number"
                },
                correctAnswer: "C",
                hint: "Government agencies communicate through official mail and websites, not unsolicited phone calls about grants.",
                feedback: {
                    A: "❌ This gives scammers direct access to your bank account to steal money.",
                    B: "❌ Don't give scammers your address or continue the conversation.",
                    C: "✅ Correct! Government agencies don't call people about grants out of the blue.",
                    D: "❌ Scammers can make up fake ID numbers. Don't engage with them."
                },
                explanation: "Government grant scams are common. Real government agencies communicate through official channels and don't call unsolicited about grants."
            }
        ];

        // Enhanced saveQuizCompletion with better error reporting
        function saveQuizCompletion(totalPointsEarned, correctAnswers) {
            console.log("=== SAVING QUIZ COMPLETION ===");
            console.log("Total points earned:", totalPointsEarned, "Correct answers:", correctAnswers);

            return new Promise((resolve, reject) => {
                var xhr = new XMLHttpRequest();
                xhr.open('POST', 'PhoneScamsQuiz.aspx/CompleteQuiz', true);
                xhr.setRequestHeader('Content-Type', 'application/json');

                var data = JSON.stringify({
                    totalPointsEarned: totalPointsEarned,
                    correctAnswers: correctAnswers
                });

                console.log("Sending completion data:", data);

                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {
                        console.log("Completion AJAX Response Status:", xhr.status);
                        console.log("Completion AJAX Response Text:", xhr.responseText);

                        if (xhr.status === 200) {
                            try {
                                var response = JSON.parse(xhr.responseText);
                                console.log("Completion response:", response);

                                if (response.d) {
                                    console.log("Server completion response:", response.d);
                                    if (response.d.indexOf("Error") !== -1) {
                                        console.error("Server error during completion:", response.d);
                                        reject(response.d);
                                    } else {
                                        console.log("Quiz completion saved successfully");
                                        resolve(response.d);
                                    }
                                } else {
                                    resolve("Success");
                                }
                            } catch (e) {
                                console.error("Error parsing completion response:", e);
                                console.error("Raw response was:", xhr.responseText);
                                reject(`JSON Parse Error: ${e.message}`);
                            }
                        } else {
                            console.error('Error saving quiz completion. Status:', xhr.status);
                            console.error('Response headers:', xhr.getAllResponseHeaders());
                            console.error('Response text:', xhr.responseText);
                            reject(`HTTP Error ${xhr.status}: ${xhr.responseText}`);
                        }
                    }
                };

                xhr.onerror = function () {
                    console.error("Network error during completion");
                    console.error("XHR object:", xhr);
                    reject("Network error - check your internet connection");
                };

                xhr.ontimeout = function () {
                    console.error("Request timeout during completion");
                    reject("Request timeout - please try again");
                };

                xhr.timeout = 30000; // 30 second timeout

                try {
                    xhr.send(data);
                } catch (sendError) {
                    console.error("Error sending request:", sendError);
                    reject(`Send Error: ${sendError.message}`);
                }
            });
        }

        // Complete quiz function with proper total points calculation
        function completeQuiz() {
            console.log("=== COMPLETING QUIZ ===");

            // Get the ACTUAL starting points from when the page first loaded
            const actualStartingPoints = parseInt(document.getElementById('<%= lblCurrentPoints.ClientID %>').getAttribute('data-starting-points') || '<%= lblCurrentPoints.Text %>');

            // Calculate ONLY the points earned from questions in this session
            const pointsFromQuestions = totalPoints - actualStartingPoints;

            console.log("Starting points:", actualStartingPoints);
            console.log("Current total points:", totalPoints);
            console.log("Points from questions:", pointsFromQuestions);
            console.log("Correct answers:", correctAnswersCount);

            // Save completion with just the question points - bonuses are handled server-side
            saveQuizCompletion(pointsFromQuestions, correctAnswersCount)
                .then(() => {
                    // Calculate the total points earned including bonuses
                    const totalPointsEarned = pointsFromQuestions + getBonusPoints(correctAnswersCount);

                    // Redirect to completion page with all necessary parameters
                    const params = new URLSearchParams({
                        quiz: "Phone Scams Quiz",
                        points: totalPointsEarned, // Total points earned from this quiz
                        correct: correctAnswersCount,
                        total: quizData.length,
                        currentPoints: totalPoints // Pass the current total points
                    });

                    window.location.href = `QuizCompletion.aspx?${params.toString()}`;
                })
                .catch(error => {
                    console.error("Error completing quiz:", error);
                    alert("There was an error completing the quiz. Please try again.");
                });
        }

        // Helper function to calculate bonus points for display
        function getBonusPoints(correctAnswers) {
            let bonusPoints = 0;

            // These should match the server-side bonus calculation
            bonusPoints += 10; // Completion bonus
            bonusPoints += 20; // First time bonus (we'll assume first time for display)

            if (correctAnswers === 10) {
                bonusPoints += 50; // Perfect score bonus
            }

            return bonusPoints;
        }

        function saveQuizProgress(questionNumber, selectedAnswer, isCorrect, hintUsed, pointsEarned) {
            console.log("=== SAVING QUIZ PROGRESS ===");
            console.log("Question:", questionNumber, "Answer:", selectedAnswer, "Correct:", isCorrect, "Points:", pointsEarned);

            // Make AJAX call to save progress
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'PhoneScamsQuiz.aspx/SaveQuizProgress', true);
            xhr.setRequestHeader('Content-Type', 'application/json');

            var data = JSON.stringify({
                questionNumber: questionNumber,
                selectedAnswer: selectedAnswer,
                isCorrect: isCorrect,
                hintUsed: hintUsed,
                pointsEarned: pointsEarned
            });

            console.log("Sending data:", data);

            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    console.log("AJAX Response Status:", xhr.status);
                    console.log("AJAX Response Text:", xhr.responseText);

                    if (xhr.status === 200) {
                        try {
                            var response = JSON.parse(xhr.responseText);
                            console.log("Parsed response:", response);

                            if (response.d) {
                                console.log("Server response:", response.d);
                                if (response.d.indexOf("Error") !== -1) {
                                    console.error("Server error:", response.d);
                                } else {
                                    console.log("Progress saved successfully");
                                }
                            }
                        } catch (e) {
                            console.error("Error parsing response:", e);
                        }
                    } else {
                        console.error('Error saving progress. Status:', xhr.status, 'Response:', xhr.responseText);
                    }
                }
            };

            xhr.onerror = function () {
                console.error("Network error occurred");
            };

            xhr.send(data);
        }

        function selectAnswer(answer) {
            if (questionAnswered) return;

            console.log("Answer selected:", answer);
            selectedAnswer = answer;
            const question = quizData[currentQuestionIndex];

            // Mark as answered
            questionAnswered = true;
            answeredQuestions[currentQuestionIndex] = true;

            // Update hidden field
            document.getElementById('<%= hdnSelectedAnswer.ClientID %>').value = answer;

            // Disable further selections and remove onclick events
            document.querySelectorAll('.answer-option').forEach(option => {
                option.onclick = null;
                option.style.pointerEvents = 'none';

                const letter = option.querySelector('.option-letter').textContent;
                const feedbackLink = option.querySelector('.click-feedback');

                // Show and enable feedback links
                feedbackLink.style.display = 'inline';
                feedbackLink.style.pointerEvents = 'auto';
                feedbackLink.onclick = function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    showOptionFeedback(letter);
                };

                // Color code the answers
                if (letter === question.correctAnswer) {
                    option.classList.add('correct');
                } else {
                    option.classList.add('incorrect');
                }
            });

            // Calculate and show points
            let pointsEarned = 0;
            const isCorrect = (answer === question.correctAnswer);

            if (isCorrect) {
                pointsEarned = hintUsed ? 10 : 15;
                totalPoints += pointsEarned;
                correctAnswersCount++;
                console.log("Points earned:", pointsEarned, "Total points:", totalPoints, "Correct answers:", correctAnswersCount);

                // Store updated points and correct answers in session storage
                sessionStorage.setItem('quizPoints', totalPoints.toString());
                sessionStorage.setItem('correctAnswers', correctAnswersCount.toString());

                showPointsNotification(pointsEarned, true);
                showFeedback(true, question.explanation);
            } else {
                console.log("Wrong answer, no points earned");
                showPointsNotification(0, false);
                showFeedback(false, question.feedback[answer]);

                // Still update session storage for correct answers count
                sessionStorage.setItem('correctAnswers', correctAnswersCount.toString());
            }

            // Update total points display immediately
            document.getElementById('<%= lblCurrentPoints.ClientID %>').textContent = totalPoints;

            // Save progress to database
            saveQuizProgress(currentQuestionIndex + 1, answer, isCorrect, hintUsed, pointsEarned);

            // Disable hint button
            document.getElementById('hintBtn').disabled = true;

            // Update back button state
            updateBackButtonState();
        }

        function loadQuestion() {
            console.log("Loading question:", currentQuestionIndex + 1, "of", quizData.length);
            const question = quizData[currentQuestionIndex];

            document.getElementById('questionNumber').textContent = currentQuestionIndex + 1;
            document.getElementById('questionText').textContent = question.question;
            document.getElementById('scenarioText').textContent = question.scenario;
            document.getElementById('hintText').textContent = question.hint;

            // Update question info
            document.getElementById('<%= lblCurrentQuestion.ClientID %>').textContent = currentQuestionIndex + 1;

            // Update progress bar
            const progress = ((currentQuestionIndex + 1) / quizData.length) * 100;
            document.getElementById('progressFill').style.width = progress + '%';

            // Load answer options
            const options = question.options;
            const answerOptionsHtml = `
                <div class="answer-option" onclick="selectAnswer('A')" id="optionA">
                    <div class="option-letter">A</div>
                    <div class="option-text">${options.A}</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
                <div class="answer-option" onclick="selectAnswer('B')" id="optionB">
                    <div class="option-letter">B</div>
                    <div class="option-text">${options.B}</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
                <div class="answer-option" onclick="selectAnswer('C')" id="optionC">
                    <div class="option-letter">C</div>
                    <div class="option-text">${options.C}</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
                <div class="answer-option" onclick="selectAnswer('D')" id="optionD">
                    <div class="option-letter">D</div>
                    <div class="option-text">${options.D}</div>
                    <span class="click-feedback" style="display: none;">Click to see feedback</span>
                </div>
            `;

            document.getElementById('answerOptions').innerHTML = answerOptionsHtml;

            // Reset states
            resetQuestion();

            // Update hidden field to sync with JavaScript
            document.getElementById('<%= hdnCurrentQuestionIndex.ClientID %>').value = currentQuestionIndex;

            // CRITICAL: Maintain points display - don't let it reset to server value
            console.log("Maintaining points display:", totalPoints);
            document.getElementById('<%= lblCurrentPoints.ClientID %>').textContent = totalPoints;

            // Update back button state
            updateBackButtonState();
        }

        function resetQuestion() {
            // Reset selections and states
            selectedAnswer = '';
            hintUsed = false;
            questionAnswered = false;

            // Reset UI
            document.querySelectorAll('.answer-option').forEach(option => {
                option.className = 'answer-option';
                option.querySelector('.click-feedback').style.display = 'none';
                // Re-enable onclick for answer selection
                const letter = option.querySelector('.option-letter').textContent;
                option.onclick = function () { selectAnswer(letter); };
            });

            document.getElementById('hintSection').classList.remove('show');
            document.getElementById('feedbackSection').classList.remove('show', 'correct', 'incorrect');
            document.getElementById('pointsNotification').style.display = 'none';
            document.getElementById('hintBtn').disabled = false;

            // Enable answer options
            document.getElementById('answerOptions').classList.remove('disabled');

            // Update hidden fields
            document.getElementById('<%= hdnSelectedAnswer.ClientID %>').value = '';
            document.getElementById('<%= hdnHintUsed.ClientID %>').value = 'false';
        }

        function showHint() {
            if (questionAnswered || hintUsed) return;

            hintUsed = true;
            document.getElementById('hintSection').classList.add('show');
            document.getElementById('hintBtn').disabled = true;
            
            // Update hidden field
            document.getElementById('<%= hdnHintUsed.ClientID %>').value = 'true';
        }

        function showPointsNotification(points, isCorrect) {
            const notification = document.getElementById('pointsNotification');
            const pointsText = document.getElementById('pointsText');

            if (isCorrect) {
                if (hintUsed) {
                    notification.className = 'points-notification correct';
                    pointsText.innerHTML = '🎯 +10 points! Correct Answer';
                } else {
                    notification.className = 'points-notification correct-no-hint';
                    pointsText.innerHTML = '🏆 +15 points! (No hints bonus!)';
                }
            } else {
                notification.className = 'points-notification';
                notification.style.background = '#f8d7da';
                notification.style.color = '#721c24';
                notification.style.border = '1px solid #f5c6cb';
                pointsText.innerHTML = '❌ 0 points - Try again next time!';
            }

            notification.style.display = 'block';
        }

        function showFeedback(isCorrect, feedbackText) {
            const feedbackSection = document.getElementById('feedbackSection');
            const feedbackTitle = document.getElementById('feedbackTitle');
            const feedbackTitleText = document.getElementById('feedbackTitleText');
            const feedbackIcon = document.getElementById('feedbackIcon');
            const feedbackTextEl = document.getElementById('feedbackText');

            feedbackSection.classList.add('show');

            if (isCorrect) {
                feedbackSection.classList.add('correct');
                feedbackTitle.classList.add('correct');
                feedbackTextEl.classList.add('correct');
                feedbackIcon.innerHTML = '✅';
                feedbackTitleText.textContent = '+10 points! Correct Answer';
                if (!hintUsed) {
                    feedbackTitleText.textContent = '+15 points! Correct Answer';
                }
            } else {
                feedbackSection.classList.add('incorrect');
                feedbackTitle.classList.add('incorrect');
                feedbackTextEl.classList.add('incorrect');
                feedbackIcon.innerHTML = '❌';
                feedbackTitleText.textContent = "Let's Learn Together!";
            }

            feedbackTextEl.textContent = feedbackText;
        }

        function showOptionFeedback(option) {
            const question = quizData[currentQuestionIndex];
            const feedbackText = question.feedback[option];

            // Create a modal or alert with the specific feedback
            alert(feedbackText);
        }

        function nextQuestion() {
            console.log("nextQuestion called - current index:", currentQuestionIndex, "total questions:", quizData.length);
            
            // Check if this is the last question (index 9 for 10 questions)
            if (currentQuestionIndex >= quizData.length - 1) {
                console.log("This is the last question - completing quiz");
                completeQuiz();
                return false;
            }
            
            // Move to next question
            currentQuestionIndex++;
            console.log("Moving to question:", currentQuestionIndex + 1);
            
            loadQuestion();
            
            // Maintain points display
            document.getElementById('<%= lblCurrentPoints.ClientID %>').textContent = totalPoints;
            
            return false; // Prevent any form submission
        }

        function updateBackButtonState() {
            const backButton = document.querySelector('.btn-back');
            
            // Check if any previous question has been answered
            let canGoBack = false;
            for (let i = 0; i < currentQuestionIndex; i++) {
                if (!answeredQuestions[i]) {
                    canGoBack = true;
                    break;
                }
            }
            
            // If we're on question 1 OR if the current question is answered OR if all previous questions are answered
            if (currentQuestionIndex === 0 || questionAnswered || !canGoBack) {
                backButton.disabled = true;
                backButton.style.opacity = '0.5';
                backButton.style.cursor = 'not-allowed';
                backButton.innerHTML = '← Back (Disabled)';
            } else {
                backButton.disabled = false;
                backButton.style.opacity = '1';
                backButton.style.cursor = 'pointer';
                backButton.innerHTML = '← Back';
            }
        }

        function goBack() {
            // Multiple layers of protection against going back
            
            // Check if current question is answered
            if (questionAnswered) {
                alert("You cannot go back after answering a question. Please continue to the next question.");
                return false;
            }
            
            // Check if any previous questions have been answered
            for (let i = 0; i < currentQuestionIndex; i++) {
                if (answeredQuestions[i]) {
                    alert("You cannot go back to questions you have already answered. Please continue forward.");
                    return false;
                }
            }
            
            // Only allow going back if we're not on the first question and no questions have been answered
            if (currentQuestionIndex > 0) {
                currentQuestionIndex--;
                loadQuestion();
            } else {
                // Go back to quiz selection only if no questions answered
                if (answeredQuestions.length === 0 || !answeredQuestions.some(answered => answered)) {
                    if (confirm("Are you sure you want to exit the quiz? Your progress will be lost.")) {
                        window.location.href = 'Quizzes.aspx';
                    }
                } else {
                    alert("You cannot exit the quiz after answering questions. Please complete the quiz.");
                }
            }
        }

        // Define beforeUnloadHandler as a named function so we can remove it
        function beforeUnloadHandler(e) {
            if (answeredQuestions.some(answered => answered)) {
                e.preventDefault();
                e.returnValue = 'Are you sure you want to leave? Your quiz progress may be lost.';
                return e.returnValue;
            }
        }

        // Window onload to properly handle dynamic starting points
        // Window onload to properly handle dynamic starting points
        window.onload = function () {
            // Check if we have a stored question index from server
            var storedIndex = document.getElementById('<%= hdnCurrentQuestionIndex.ClientID %>').value;
            if (storedIndex && storedIndex !== '') {
                currentQuestionIndex = parseInt(storedIndex);
            }

            // Check if hint was used from server
            var storedHintUsed = document.getElementById('<%= hdnHintUsed.ClientID %>').value;
            if (storedHintUsed === 'true') {
                hintUsed = true;
            }
            
            // Get the user's ACTUAL current points from the server (loaded from database)
            const userActualPoints = parseInt('<%= lblCurrentPoints.Text %>');
            console.log("User's actual points from database:", userActualPoints);

            // Store the starting points as a data attribute for later reference
            document.getElementById('<%= lblCurrentPoints.ClientID %>').setAttribute('data-starting-points', userActualPoints);

            // CRITICAL: Clear session storage if we're starting a fresh quiz (question index = 0)
            if (currentQuestionIndex === 0) {
                console.log("Starting fresh quiz - clearing session storage");
                sessionStorage.removeItem('quizPoints');
                sessionStorage.removeItem('correctAnswers');
            }

            // CRITICAL: Points management with dynamic starting points
            var storedPoints = sessionStorage.getItem('quizPoints');
            var storedCorrectAnswers = sessionStorage.getItem('correctAnswers');

            if (storedPoints && currentQuestionIndex > 0) {
                // Restore from session storage if we're not on the first question
                totalPoints = parseInt(storedPoints);
                correctAnswersCount = parseInt(storedCorrectAnswers || '0');
                console.log("Restored points from session:", totalPoints, "Correct answers:", correctAnswersCount);
            } else {
                // Use the user's actual current points from database as starting point
                totalPoints = userActualPoints;
                correctAnswersCount = 0;
                sessionStorage.setItem('quizPoints', totalPoints.toString());
                sessionStorage.setItem('correctAnswers', correctAnswersCount.toString());
                console.log("Initial load - using user's actual points:", totalPoints);
            }

            // Initialize answered questions array based on current question index
            answeredQuestions = [];
            for (let i = 0; i < currentQuestionIndex; i++) {
                answeredQuestions[i] = true;
            }

            loadQuestion();
        };

        // Add beforeunload protection only when user has answered questions
        window.addEventListener('beforeunload', beforeUnloadHandler);

        // Prevent browser history navigation during quiz
        window.addEventListener('popstate', function (e) {
            if (answeredQuestions.some(answered => answered)) {
                e.preventDefault();
                alert("Please use the quiz navigation buttons to move through the questions.");
                history.pushState(null, null, window.location.href);
            }
        });
    </script>
</asp:Content>