<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ScamChecker.aspx.cs" Inherits="SpotTheScam.User.ScamChecker" ValidateRequest="false" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Scam Checker</title>
    <link href="https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap" rel="stylesheet" />
    <style>
        /* Main content styling */
        .content-container {
            background: #D36F2D;
            width: 100vw;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            position: relative;
            padding: 48px 20px 32px 20px;
            font-family: 'DM Sans', sans-serif;
            z-index: 1;
        }

        .content-container h2 {
            color: white;
            font-size: 2.5rem;
            font-weight: 600;
            text-align: center;
            margin-bottom: 18px;
            text-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .scam-checker-desc {
            color: #fff;
            text-align: center;
            font-size: 1.08rem;
            max-width: 700px;
            margin: 0 auto 30px auto;
            line-height: 1.6;
            font-family: 'DM Sans', sans-serif;
        }

        .scam-checker-wrapper {
            max-width: 800px;
            margin: 0 auto;
            background: white;
            border-radius: 20px;
            box-shadow: 0 4px 16px rgba(5,29,64,0.08);
            padding: 40px;
            margin-top: 40px;
            font-family: 'DM Sans', sans-serif;
        }

        .input-section {
            margin-bottom: 30px;
        }

        .input-label {
            display: block;
            margin-bottom: 15px;
            font-weight: 600;
            color: #051D40;
            font-size: 1.1rem;
            font-family: 'DM Sans', sans-serif;
        }

        .text-input {
            width: 100%;
            min-height: 120px;
            padding: 15px;
            border: 2px solid #e0e0e0;
            border-radius: 10px;
            font-size: 14px;
            font-family: 'DM Sans', sans-serif;
            resize: vertical;
            box-sizing: border-box;
            transition: border-color 0.3s ease;
        }

        .text-input:focus {
            outline: none;
            border-color: #D36F2D;
            box-shadow: 0 0 0 3px rgba(211, 111, 45, 0.1);
        }

        .check-button {
            background: linear-gradient(135deg, #D36F2D, #E85A4F);
            color: white;
            border: none;
            padding: 15px 30px;
            border-radius: 25px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            font-family: 'DM Sans', sans-serif;
            margin-top: 20px;
        }

        .check-button:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(211, 111, 45, 0.3);
        }

        .check-button:disabled {
            background: #ccc;
            cursor: not-allowed;
            transform: none;
            box-shadow: none;
        }

        .result-section {
            margin-top: 30px;
            padding: 25px;
            border-radius: 15px;
            border-left: 5px solid #D36F2D;
            background: #f8f9fa;
            font-family: 'DM Sans', sans-serif;
        }

        .result-title {
            font-weight: 700;
            margin-bottom: 15px;
            font-size: 1.2rem;
            color: #051D40;
        }

        .result-content {
            line-height: 1.6;
            font-size: 14px;
            color: #333;
        }

        .examples-section {
            margin-top: 40px;
            background: #FFF8DC;
            padding: 30px;
            border-radius: 15px;
            border: 1px solid #e9ecef;
        }

        .examples-title {
            font-weight: 600;
            margin-bottom: 20px;
            color: #D36F2D;
            font-size: 1.2rem;
            font-family: 'DM Sans', sans-serif;
        }

        .example-item {
            background: white;
            padding: 15px 20px;
            margin: 10px 0;
            border-radius: 10px;
            border-left: 4px solid #D36F2D;
            cursor: pointer;
            transition: all 0.3s ease;
            font-family: 'DM Sans', sans-serif;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }

        .example-item:hover {
            background: #f8f9fa;
            transform: translateX(5px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }

        .loading-spinner {
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 3px solid #f3f3f3;
            border-top: 3px solid #D36F2D;
            border-radius: 50%;
            animation: spin 1s linear infinite;
            margin-right: 10px;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        /* Responsive design */
        @media (max-width: 768px) {
            .content-container {
                padding: 40px 15px 0;
            }
            
            .content-container h2 {
                font-size: 2rem;
            }
            
            .scam-checker-wrapper {
                margin: 20px;
                padding: 25px;
            }
            
            .check-button {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    
    <div class="content-container">
        <h2>Anti Scam Checker</h2>
        <div class="scam-checker-desc">Paste any suspicious message, email, or text to check if it's a potential scam. Our AI will analyze it and provide you with clear, easy-to-understand feedback.</div>
    </div>
    
    <div class="scam-checker-wrapper">
        <div class="input-section">
            <label for="txtUserInput" class="input-label">Enter the suspicious message:</label>
            <asp:TextBox ID="txtUserInput" runat="server" TextMode="MultiLine" Rows="8" CssClass="text-input" placeholder="Paste the suspicious message here... For example: You've won $1,000! Click here to claim your prize!" />
            <br />
            <asp:Button ID="btnCheckScam" runat="server" Text="🔍 Check for Scam" OnClick="btnCheckScam_Click" CssClass="check-button"/>
        </div>
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="result-section" id="resultSection">
                    <div class="result-title">Scam Check Result</div>
                    <asp:Label ID="lblResult" runat="server" CssClass="result-content"></asp:Label>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <div class="examples-section">
            <div class="examples-title">💡 Example messages to test:</div>
            <div class="example-item" onclick="setExample('You have won $1,000! Click here to claim your prize immediately!')">
                You have won $1,000! Click here to claim your prize immediately!
            </div>
            <div class="example-item" onclick="setExample('Your bank account has been suspended. Call this number immediately to verify your identity.')">
                Your bank account has been suspended. Call this number immediately to verify your identity.
            </div>
            <div class="example-item" onclick="setExample('Hi, this is your grandson. I need money urgently for an emergency. Please send $500.')">
                Hi, this is your grandson. I need money urgently for an emergency. Please send $500.
            </div>
            <div class="example-item" onclick="setExample('Your package delivery is delayed. Click here to reschedule your delivery.')">
                Your package delivery is delayed. Click here to reschedule your delivery.
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        function setExample(text) {
            document.getElementById('<%= txtUserInput.ClientID %>').value = text;
        }
        
        function showLoading() {
            var button = document.getElementById('<%= btnCheckScam.ClientID %>');
            button.disabled = true;
            button.innerHTML = '<span class="loading-spinner"></span>Analyzing...';
        }
        
        function hideLoading() {
            var button = document.getElementById('<%= btnCheckScam.ClientID %>');
            button.disabled = false;
            button.innerHTML = '🔍 Check for Scam';
        }
    </script>
</asp:Content>
