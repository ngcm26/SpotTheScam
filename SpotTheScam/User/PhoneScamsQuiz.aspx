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
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 10px 20px;
            border-radius: 5px;
            color: white;
            font-weight: bold;
            opacity: 0;
            transform: translateX(100px);
            transition: all 0.3s ease;
            z-index: 1000;
        }

        .points-notification.correct {
            background-color: #4CAF50;
        }

        .points-notification.incorrect {
            background-color: #f44336;
        }

        .points-notification.show {
            opacity: 1;
            transform: translateX(0);
        }

        .quiz-card {
            background: white;
            border-radius: 15px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            padding: 30px;
            margin-bottom: 20px;
        }

        .question-number {
            background: var(--brand-orange);
            color: white;
            border-radius: 50%;
            width: 40px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 600;
            margin-bottom: 20px;
        }

        .question-text {
            font-size: 1.3rem;
            font-weight: 600;
            color: var(--brand-navy);
            margin-bottom: 20px;
            line-height: 1.4;
        }

        .scenario-box {
            background: #fff8e1;
            border-left: 4px solid #ff9800;
            padding: 15px;
            margin-bottom: 25px;
            border-radius: 0 8px 8px 0;
        }

        .scenario-text {
            margin: 0;
            font-style: italic;
            color: #333;
        }

        .options-container {
            margin-bottom: 25px;
        }

        .option-item {
            background: #f8f9fa;
            border: 2px solid #e9ecef;
            border-radius: 10px;
            margin-bottom: 10px;
            transition: all 0.3s ease;
            cursor: pointer;
        }

        .option-item:hover {
            border-color: var(--brand-orange);
            background: #fff;
        }

        .option-item.selected {
            border-color: var(--brand-orange);
            background: #fff3e0;
        }

        .option-item input[type="radio"] {
            display: none;
        }

        .option-label {
            display: block;
            padding: 15px;
            cursor: pointer;
            font-weight: 500;
        }

        .option-letter {
            background: var(--brand-orange);
            color: white;
            border-radius: 50%;
            width: 25px;
            height: 25px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            margin-right: 10px;
            font-size: 0.9rem;
            font-weight: 600;
        }

        .hint-container {
            margin-bottom: 25px;
        }

        .hint-btn {
            background: #17a2b8;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.9rem;
            cursor: pointer;
            transition: background 0.3s ease;
        }

        .hint-btn:hover:not(:disabled) {
            background: #138496;
        }

        .hint-btn:disabled {
            background: #6c757d;
            cursor: not-allowed;
        }

        .hint-content {
            background: #e8f4f8;
            border-left: 4px solid #17a2b8;
            padding: 15px;
            margin-top: 10px;
            border-radius: 0 8px 8px 0;
            display: none;
        }

        .navigation-buttons {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 25px;
        }

        .btn-primary-custom {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 25px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn-primary-custom:hover:not(:disabled) {
            background: #b8591f;
            transform: translateY(-2px);
            box-shadow: 0 4px 15px rgba(211, 111, 45, 0.3);
        }

        .btn-secondary-custom {
            background: #6c757d;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 25px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn-secondary-custom:hover:not(:disabled) {
            background: #545b62;
        }

        .btn-primary-custom:disabled,
        .btn-secondary-custom:disabled {
            opacity: 0.6;
            cursor: not-allowed;
            transform: none;
            box-shadow: none;
        }

        .exit-quiz-btn {
            background: #dc3545;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.9rem;
            cursor: pointer;
            transition: background 0.3s ease;
        }

        .exit-quiz-btn:hover {
            background: #c82333;
        }

        .feedback {
            margin-top: 15px;
            padding: 15px;
            border-radius: 5px;
            border-left: 4px solid;
            display: none;
        }

        .feedback.correct {
            background-color: #d4edda;
            border-color: #28a745;
            color: #155724;
        }

        .feedback.incorrect {
            background-color: #f8d7da;
            border-color: #dc3545;
            color: #721c24;
        }

        .feedback-icon {
            font-weight: bold;
            margin-right: 10px;
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
                Current Points: <asp:Label ID="lblCurrentPoints" runat="server" Text="0" />
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

        <!-- Quiz Card -->
        <div class="quiz-card">
            <div class="question-number" id="questionNumber">1</div>
            <div class="question-text" id="questionText">What should you do if someone calls claiming to be from your bank?</div>
            
            <div class="scenario-box" id="scenarioBox">
                <p class="scenario-text" id="scenarioText">📧 Scenario: You receive a call from someone saying, "This John from ABC Bank. We've detected suspicious activity on your account and we need to verify your information immediately"</p>
            </div>

            <!-- Answer Options -->
            <div class="options-container">
                <div class="option-item" onclick="selectOption('A')">
                    <input type="radio" id="optionA" name="quizAnswer" value="A">
                    <label class="option-label" for="optionA">
                        <span class="option-letter">A</span>
                        <span id="optionAText">Give them your account number and PIN to verify your identity</span>
                    </label>
                </div>

                <div class="option-item" onclick="selectOption('B')">
                    <input type="radio" id="optionB" name="quizAnswer" value="B">
                    <label class="option-label" for="optionB">
                        <span class="option-letter">B</span>
                        <span id="optionBText">Hang up and call your bank directly using the number on your card or statement</span>
                    </label>
                </div>

                <div class="option-item" onclick="selectOption('C')">
                    <input type="radio" id="optionC" name="quizAnswer" value="C">
                    <label class="option-label" for="optionC">
                        <span class="option-letter">C</span>
                        <span id="optionCText">Ask them to call you back in 10 minutes to give you time to find your information</span>
                    </label>
                </div>

                <div class="option-item" onclick="selectOption('D')">
                    <input type="radio" id="optionD" name="quizAnswer" value="D">
                    <label class="option-label" for="optionD">
                        <span class="option-letter">D</span>
                        <span id="optionDText">Transfer the call to a family member who is better with technology</span>
                    </label>
                </div>
            </div>

            <!-- Hint Section -->
            <div class="hint-container">
                <button type="button" id="hintBtn" class="hint-btn" onclick="showHint()">💡 Show Hint</button>
                <div id="hintContent" class="hint-content">
                    <strong>💡 Hint:</strong> <span id="hintText">Legitimate banks have security procedures that don't require you to provide sensitive information over unsolicited calls.</span>
                </div>
            </div>

            <!-- Feedback Section -->
            <div id="feedback" class="feedback">
                <span id="feedbackIcon" class="feedback-icon"></span>
                <span id="feedbackText"></span>
            </div>

            <!-- Navigation Buttons -->
            <div class="navigation-buttons">
                <button type="button" id="backBtn" class="btn-secondary-custom" onclick="previousQuestion()">← Back</button>
                <button type="button" class="exit-quiz-btn" onclick="exitQuiz()">Exit Quiz</button>
                <button type="button" id="nextBtn" class="btn-primary-custom" onclick="nextQuestion()" disabled>Next Question →</button>
            </div>
        </div>
    </div>

    <!-- Hidden Fields for State Management -->
    <asp:HiddenField ID="hdnCurrentQuestionIndex" runat="server" Value="0" />
    <asp:HiddenField ID="hdnSelectedAnswer" runat="server" Value="" />
    <asp:HiddenField ID="hdnHintUsed" runat="server" Value="false" />
    <asp:HiddenField ID="hdnQuizData" runat="server" />

    <script type="text/javascript">
        // Quiz data and state variables
        let currentQuestionIndex = 0;
        let answeredQuestions = [];
        let hintUsed = false;

        // Points tracking variables
        let userStartingPoints = 0;
        let totalPointsEarnedFromQuiz = 0;
        let totalCurrentPoints = 0;
        let correctAnswersCount = 0;

        // Track all question results for debugging
        let questionResults = [];

        // Quiz session management
        let isNewQuizSession = true;

        // Quiz data - Complete set of questions
        const quizData = [
            {
                question: "What should you do if someone calls claiming to be from your bank?",
                scenario: "You receive a call from someone saying, \"This John from ABC Bank. We've detected suspicious activity on your account and we need to verify your information immediately\"",
                options: {
                    A: "Give them your account number and PIN to verify your identity",
                    B: "Hang up and call your bank directly using the number on your card or statement",
                    C: "Ask them to call you back in 10 minutes to give you time to find your information",
                    D: "Transfer the call to a family member who is better with technology"
                },
                correct: "B",
                explanation: "Banks will never ask for sensitive information over the phone. Always hang up and call your bank directly using official numbers.",
                feedback: {
                    A: "Never give personal information to unsolicited callers, even if they claim to be from your bank.",
                    C: "Scammers will often agree to call back to seem legitimate. Always verify independently.",
                    D: "This doesn't solve the problem - the caller could still be a scammer."
                },
                hint: "Legitimate banks have security procedures that don't require you to provide sensitive information over unsolicited calls."
            },
            {
                question: "Someone calls claiming you've won a prize, but asks for payment to claim it. What do you do?",
                scenario: "You receive a call: \"Congratulations! You've won $10,000 in our sweepstakes! To claim your prize, we just need a $200 processing fee via gift card.\"",
                options: {
                    A: "Pay the fee immediately to claim your prize",
                    B: "Ask for their company information and hang up to verify",
                    C: "Negotiate a lower processing fee",
                    D: "Give them your credit card information for the fee"
                },
                correct: "B",
                explanation: "Legitimate prizes never require upfront payments. This is a classic scam tactic.",
                feedback: {
                    A: "Real contests never require payment to claim prizes. This is always a scam.",
                    C: "Any request for payment to claim a prize is fraudulent, regardless of the amount.",
                    D: "Never give payment information to unsolicited callers claiming you've won something."
                },
                hint: "Real sweepstakes and contests are free to enter and free to win."
            },
            {
                question: "A caller claims to be from the IRS and threatens immediate arrest. What's your response?",
                scenario: "Caller says: \"This is the IRS. You owe $5,000 in back taxes. Pay immediately or we'll have you arrested within the hour.\"",
                options: {
                    A: "Pay immediately to avoid arrest",
                    B: "Hang up - the IRS doesn't make threatening calls",
                    C: "Ask them to mail you the bill",
                    D: "Give them your Social Security number to verify"
                },
                correct: "B",
                explanation: "The IRS never calls to demand immediate payment or threaten arrest. They communicate through official mail.",
                feedback: {
                    A: "The IRS never threatens immediate arrest or demands instant payment over the phone.",
                    C: "While asking for written documentation is good practice, this caller is definitely a scammer.",
                    D: "Never give your SSN to unsolicited callers, especially threatening ones."
                },
                hint: "Government agencies follow proper procedures and don't make threatening phone calls."
            },
            {
                question: "Someone calls about your 'expired' car warranty. How do you handle this?",
                scenario: "\"We're calling about your car's extended warranty that's about to expire. Press 1 to speak with an agent or press 2 to be removed from our list.\"",
                options: {
                    A: "Press 1 to learn more about the warranty",
                    B: "Press 2 to be removed from the list",
                    C: "Hang up without pressing anything",
                    D: "Stay on the line to tell them you're not interested"
                },
                correct: "C",
                explanation: "Pressing any number confirms your phone number is active and leads to more scam calls. Just hang up.",
                feedback: {
                    A: "These warranty calls are almost always scams designed to steal your information.",
                    B: "Pressing any number, even to 'opt out,' confirms your number is active and increases scam calls.",
                    D: "Engaging with scammers in any way can lead to more aggressive tactics and calls."
                },
                hint: "Interacting with robocalls in any way often leads to more unwanted calls."
            },
            {
                question: "A caller offers a great deal on home security but needs your address to 'check availability.' What do you do?",
                scenario: "\"We're offering 50% off home security systems in your area today only! What's your address so I can check if we service your location?\"",
                options: {
                    A: "Give them your address since it's for a legitimate service check",
                    B: "Give them your zip code instead of full address",
                    C: "Ask for their company details and research them first",
                    D: "Tell them you'll call them back and ask for their number"
                },
                correct: "C",
                explanation: "Legitimate companies can be researched and verified. Never give personal information to unsolicited callers.",
                feedback: {
                    A: "Your address is personal information that can be used for identity theft or burglary planning.",
                    B: "Even partial address information can be dangerous when given to unverified callers.",
                    D: "Scammers often provide fake callback numbers or refuse to give them at all."
                },
                hint: "Real businesses have verifiable contact information and don't pressure for immediate decisions."
            },
            {
                question: "Someone calls claiming your computer has a virus and offers to fix it. What's your response?",
                scenario: "\"This is Microsoft support. We've detected malicious software on your computer. Allow us remote access to fix it immediately.\"",
                options: {
                    A: "Let them access your computer to fix the virus",
                    B: "Hang up - Microsoft doesn't make unsolicited calls",
                    C: "Ask them to prove they're from Microsoft",
                    D: "Pay them to remove the virus"
                },
                correct: "B",
                explanation: "Microsoft and other tech companies never make unsolicited calls about computer problems.",
                feedback: {
                    A: "Giving remote access allows scammers to steal personal information and install actual malware.",
                    C: "Scammers have convincing fake credentials. The key is that real tech support doesn't call you.",
                    D: "This is a scam - you'd be paying for nothing and potentially giving them payment information."
                },
                hint: "Legitimate tech companies only provide support when you contact them first."
            },
            {
                question: "A caller claims to be from your utility company threatening disconnection. What do you do?",
                scenario: "\"Your electricity will be shut off in 2 hours unless you pay $300 immediately via prepaid card. Call this number: 555-SCAM now!\"",
                options: {
                    A: "Pay immediately to avoid having power shut off",
                    B: "Call your utility company directly using the number on your bill",
                    C: "Ask the caller for a payment plan",
                    D: "Give them your bank account information for automatic payment"
                },
                correct: "B",
                explanation: "Utility companies provide written notice before disconnection and accept standard payment methods, not gift cards.",
                feedback: {
                    A: "Real utility companies don't demand immediate payment via gift cards or threaten same-day disconnection.",
                    C: "While payment plans exist, this caller is a scammer based on their tactics and payment demands.",
                    D: "Never give banking information to callers threatening immediate disconnection."
                },
                hint: "Utility companies have official procedures and don't require gift card payments."
            },
            {
                question: "Someone calls offering a free cruise but asks for your credit card for 'taxes and fees.' What do you do?",
                scenario: "\"You've qualified for a FREE 7-day Caribbean cruise! We just need your credit card for $199 in port fees and taxes.\"",
                options: {
                    A: "Provide your credit card since it's just for fees",
                    B: "Ask to pay the fees when you arrive at the port",
                    C: "Hang up - free cruises requiring payment aren't free",
                    D: "Ask for written confirmation before paying"
                },
                correct: "C",
                explanation: "If something is truly free, you shouldn't need to pay anything upfront. This is a classic scam.",
                feedback: {
                    A: "Once scammers have your credit card, they can make unauthorized charges beyond the stated fees.",
                    B: "The cruise doesn't exist - this is just a way to steal your payment information.",
                    D: "Scammers can create fake documents. The red flag is requiring payment for something 'free.'"
                },
                hint: "Anything that's truly 'free' shouldn't require upfront payment."
            },
            {
                question: "A caller claims you have unpaid debt and threatens legal action. How do you respond?",
                scenario: "\"You owe $2,000 to ABC Collections. Pay now or we'll garnish your wages and ruin your credit score.\"",
                options: {
                    A: "Pay immediately to protect your credit",
                    B: "Ask for written verification of the debt",
                    C: "Give them your Social Security number to verify the debt",
                    D: "Negotiate a payment plan over the phone"
                },
                correct: "B",
                explanation: "Legitimate debt collectors must provide written verification when requested. Scammers often can't or won't.",
                feedback: {
                    A: "Real debt has a paper trail. Paying without verification could mean paying for fake debt.",
                    C: "Never give your SSN to verify debt - legitimate collectors already have your information.",
                    D: "Don't negotiate until you've verified the debt is real and the collector is legitimate."
                },
                hint: "Legitimate debt collectors are required by law to provide written verification when requested."
            },
            {
                question: "Someone calls claiming to be your grandchild in trouble and needs money. What do you do?",
                scenario: "\"Grandma/Grandpa, it's me! I'm in jail and need $1,000 for bail. Please don't tell my parents. Can you wire the money right away?\"",
                options: {
                    A: "Wire the money immediately to help your grandchild",
                    B: "Hang up and call your grandchild's known phone number",
                    C: "Ask the caller detailed questions about family",
                    D: "Call other family members to verify the situation"
                },
                correct: "B",
                explanation: "This is a common 'grandparent scam.' Always verify by calling the person directly at their known number.",
                feedback: {
                    A: "Scammers prey on grandparents' love and urgency. Always verify before sending money.",
                    C: "Scammers research families on social media and may know personal details.",
                    D: "While involving family is good, directly calling the supposed person in trouble is most efficient."
                },
                hint: "Family emergency scams rely on emotion and urgency. Take time to verify independently."
            }
        ];

        function selectOption(answer) {
            console.log('selectOption called with: ' + answer);

            // Remove previous selections
            document.querySelectorAll('.option-item').forEach(item => {
                item.classList.remove('selected');
            });

            // Select current option
            document.querySelector('#option' + answer).checked = true;
            document.querySelector('#option' + answer).closest('.option-item').classList.add('selected');

            // Store selected answer
            var hdnSelectedAnswer = document.getElementById('<%= hdnSelectedAnswer.ClientID %>');
            if (hdnSelectedAnswer) {
                hdnSelectedAnswer.value = answer;
            }

            // Enable next button
            document.getElementById('nextBtn').disabled = false;

            // Auto-submit answer after a short delay to show selection
            setTimeout(function () {
                checkAnswer(answer);
            }, 500);
        }

        function checkAnswer(answer) {
            console.log('checkAnswer called for question ' + (currentQuestionIndex + 1) + ' with answer: ' + answer);

            // Prevent multiple answers for the same question in one session
            if (answeredQuestions[currentQuestionIndex]) {
                console.log("Question already answered in this session");
                return;
            }

            const question = quizData[currentQuestionIndex];
            const isCorrect = answer === question.correct;

            console.log('Question ' + (currentQuestionIndex + 1) + ': Answer ' + answer + ', Correct answer: ' + question.correct + ', Is correct: ' + isCorrect);

            // Mark question as answered for this session
            answeredQuestions[currentQuestionIndex] = true;

            // Track results properly
            questionResults[currentQuestionIndex] = {
                questionNumber: currentQuestionIndex + 1,
                selectedAnswer: answer,
                correctAnswer: question.correct,
                isCorrect: isCorrect,
                hintUsed: hintUsed
            };

            // Calculate points for this question
            let pointsEarned = 0;
            if (isCorrect) {
                pointsEarned = hintUsed ? 10 : 15; // 10 points + 5 bonus for no hint
                totalPointsEarnedFromQuiz += pointsEarned; // Add to quiz total
                totalCurrentPoints = userStartingPoints + totalPointsEarnedFromQuiz; // Recalculate total
                correctAnswersCount++; // CRITICAL: Increment correct count

                console.log('CORRECT! Points earned: ' + pointsEarned);
                console.log('Total correct answers so far: ' + correctAnswersCount);
                console.log('Total points earned from quiz: ' + totalPointsEarnedFromQuiz);
                console.log('Total current points: ' + totalCurrentPoints);

                showPointsNotification(pointsEarned, true);
                showFeedback(true, question.explanation);
            } else {
                console.log('WRONG! No points earned');
                showPointsNotification(0, false);
                showFeedback(false, question.feedback[answer]);
            }

            // Update points display immediately with current total
            var lblCurrentPoints = document.getElementById('<%= lblCurrentPoints.ClientID %>');
            if (lblCurrentPoints) {
                lblCurrentPoints.textContent = totalCurrentPoints;
            }

            // Save progress to database
            saveQuizProgress(currentQuestionIndex + 1, answer, isCorrect, hintUsed, pointsEarned);

            // Disable hint button and options for this viewing
            document.getElementById('hintBtn').disabled = true;
            document.querySelectorAll('.option-item').forEach(item => {
                item.style.pointerEvents = 'none';
            });

            // Update navigation buttons
            updateNavigationButtons();
        }

        function completeQuiz() {
            console.log("=== COMPLETING QUIZ ===");
            console.log('Final Results:');
            console.log('- Correct answers: ' + correctAnswersCount + ' out of ' + quizData.length);
            console.log('- Total points earned from quiz: ' + totalPointsEarnedFromQuiz);
            console.log('- Current total points: ' + totalCurrentPoints);
            console.log('- Question results:', questionResults);

            // Validate the data before sending
            if (correctAnswersCount < 0 || correctAnswersCount > quizData.length) {
                console.error("ERROR: Invalid correctAnswersCount:", correctAnswersCount);
                correctAnswersCount = Math.max(0, Math.min(correctAnswersCount, quizData.length));
            }

            if (totalPointsEarnedFromQuiz < 0) {
                console.error("ERROR: Invalid totalPointsEarnedFromQuiz:", totalPointsEarnedFromQuiz);
                totalPointsEarnedFromQuiz = Math.max(0, totalPointsEarnedFromQuiz);
            }

            // Call server-side method to complete quiz
            fetch('<%= ResolveUrl("~/User/PhoneScamsQuiz.aspx/CompleteQuiz") %>', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    totalPointsEarned: totalPointsEarnedFromQuiz,
                    correctAnswers: correctAnswersCount
                })
            })
                .then(response => response.json())
                .then(data => {
                    console.log("Quiz completion saved:", data);

                    // Pass correct data to completion page
                    const params = new URLSearchParams({
                        quiz: 'Phone Scams Quiz',
                        points: totalPointsEarnedFromQuiz,  // Points earned THIS quiz
                        correct: correctAnswersCount,       // ACTUAL correct answers
                        total: quizData.length,            // Total questions (10)
                        currentPoints: totalCurrentPoints   // User's total points after quiz
                    });

                    console.log("Redirecting with params:", params.toString());
                    window.location.href = 'QuizCompletion.aspx?' + params.toString();
                })
                .catch(error => {
                    console.error("Error completing quiz:", error);
                    alert("Error completing quiz. Please try again.");
                });
        }

        function loadQuestion() {
            console.log('Loading question ' + (currentQuestionIndex + 1) + ' of ' + quizData.length);
            const question = quizData[currentQuestionIndex];

            // Update question display
            document.getElementById('questionNumber').textContent = currentQuestionIndex + 1;
            var lblCurrentQuestion = document.getElementById('<%= lblCurrentQuestion.ClientID %>');
            if (lblCurrentQuestion) {
                lblCurrentQuestion.textContent = (currentQuestionIndex + 1).toString();
            }

            document.getElementById('questionText').textContent = question.question;
            document.getElementById('scenarioText').textContent = question.scenario;
            document.getElementById('hintText').textContent = question.hint;

            // Load options
            const options = ['A', 'B', 'C', 'D'];
            options.forEach(option => {
                document.getElementById('option' + option + 'Text').textContent = question.options[option];
                document.getElementById('option' + option).checked = false;
            });

            // Reset option styles
            document.querySelectorAll('.option-item').forEach(item => {
                item.classList.remove('selected');
                item.style.pointerEvents = 'auto';
            });

            // Update progress bar
            const progressPercent = ((currentQuestionIndex) / quizData.length) * 100;
            document.querySelector('.progress-fill').style.width = progressPercent + '%';

            // Reset hint state for new question
            hintUsed = false;
            document.getElementById('hintBtn').disabled = false;
            document.getElementById('hintContent').style.display = 'none';

            // Clear previous feedback
            const feedbackDiv = document.getElementById('feedback');
            if (feedbackDiv) {
                feedbackDiv.style.display = 'none';
            }

            // Update navigation buttons
            updateNavigationButtons();

            // Reset the answered status for this question view
            answeredQuestions[currentQuestionIndex] = false;
        }

        function nextQuestion() {
            if (!answeredQuestions[currentQuestionIndex]) {
                alert("Please answer the current question before proceeding.");
                return;
            }

            if (currentQuestionIndex < quizData.length - 1) {
                currentQuestionIndex++;
                loadQuestion();
            } else {
                // Quiz completed
                completeQuiz();
            }
        }

        function previousQuestion() {
            if (currentQuestionIndex > 0) {
                currentQuestionIndex--;
                loadQuestion();
            }
        }

        function showHint() {
            hintUsed = true;
            document.getElementById('hintContent').style.display = 'block';
            document.getElementById('hintBtn').disabled = true;
            var hdnHintUsed = document.getElementById('<%= hdnHintUsed.ClientID %>');
            if (hdnHintUsed) {
                hdnHintUsed.value = 'true';
            }
        }

        function showPointsNotification(points, isCorrect) {
            const notification = document.createElement('div');
            notification.className = 'points-notification ' + (isCorrect ? 'correct' : 'incorrect');
            notification.textContent = isCorrect ? '+' + points + ' points!' : 'No points earned';
            
            document.body.appendChild(notification);
            
            setTimeout(function() {
                notification.classList.add('show');
            }, 100);
            
            setTimeout(function() {
                notification.classList.remove('show');
                setTimeout(function() {
                    if (document.body.contains(notification)) {
                        document.body.removeChild(notification);
                    }
                }, 300);
            }, 2000);
        }

        function showFeedback(isCorrect, message) {
            const feedbackDiv = document.getElementById('feedback');
            const feedbackText = document.getElementById('feedbackText');
            const feedbackIcon = document.getElementById('feedbackIcon');

            if (feedbackDiv && feedbackText && feedbackIcon) {
                feedbackText.textContent = message;
                feedbackIcon.textContent = isCorrect ? '✓' : '✗';
                feedbackDiv.className = 'feedback ' + (isCorrect ? 'correct' : 'incorrect');
                feedbackDiv.style.display = 'block';
            }
        }

        function saveQuizProgress(questionNumber, selectedAnswer, isCorrect, hintUsed, pointsEarned) {
            console.log("Saving progress:", {
                questionNumber: questionNumber,
                selectedAnswer: selectedAnswer,
                isCorrect: isCorrect,
                hintUsed: hintUsed,
                pointsEarned: pointsEarned
            });

            fetch('<%= ResolveUrl("~/User/PhoneScamsQuiz.aspx/SaveQuizProgress") %>', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    questionNumber: questionNumber,
                    selectedAnswer: selectedAnswer,
                    isCorrect: isCorrect,
                    hintUsed: hintUsed,
                    pointsEarned: pointsEarned
                })
            })
            .then(response => response.json())
            .then(data => {
                console.log("Progress saved:", data);
            })
            .catch(error => {
                console.error("Error saving progress:", error);
            });
        }

        function updateNavigationButtons() {
            const backBtn = document.getElementById('backBtn');
            const nextBtn = document.getElementById('nextBtn');

            if (backBtn) {
                backBtn.disabled = currentQuestionIndex === 0;
            }

            if (nextBtn) {
                if (currentQuestionIndex === quizData.length - 1) {
                    nextBtn.textContent = 'Complete Quiz';
                } else {
                    nextBtn.textContent = 'Next Question →';
                }
                nextBtn.disabled = !answeredQuestions[currentQuestionIndex];
            }
        }

        function exitQuiz() {
            if (confirm("Are you sure you want to exit the quiz?")) {
                window.location.href = 'Quizzes.aspx';
            }
        }

        // Window onload to properly initialize points tracking
        window.onload = function () {
            console.log("=== QUIZ INITIALIZATION ===");
            
            // Get the user's ACTUAL current points from the server
            var serverPointsLabel = document.getElementById('<%= lblCurrentPoints.ClientID %>');
            var serverPoints = serverPointsLabel ? parseInt(serverPointsLabel.textContent) : 0;
            console.log("User's actual points from server:", serverPoints);

            // Initialize points tracking properly
            userStartingPoints = serverPoints; // User's starting balance
            totalPointsEarnedFromQuiz = 0; // Points earned in this quiz session
            totalCurrentPoints = serverPoints; // Current total (starting + earned)
            correctAnswersCount = 0; // Reset correct answers count

            // Reset quiz state
            answeredQuestions = new Array(quizData.length).fill(false);
            questionResults = new Array(quizData.length);
            currentQuestionIndex = 0;

            console.log("Points initialization:");
            console.log("- Starting points:", userStartingPoints);
            console.log("- Quiz points earned:", totalPointsEarnedFromQuiz);
            console.log("- Current total:", totalCurrentPoints);
            console.log("- Correct answers:", correctAnswersCount);

            loadQuestion();
        };
    </script>
</asp:Content>