<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ScamChecker.aspx.cs" Inherits="SpotTheScam.User.ScamChecker" ValidateRequest="false" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Scam Checker</title>
    <link href="https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght@400&display=swap" rel="stylesheet" />
    <style>
        /* Reset margins and padding - CRITICAL FIX */
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body, html {
            font-family: 'DM Sans', sans-serif !important;
            margin: 0 !important;
            padding: 0 !important;
            width: 100%;
            box-sizing: border-box;
            overflow-x: hidden;
        }
        * {
            font-family: 'DM Sans', sans-serif !important;
        }
        
        /* MAIN FIX: Full width background with no white space */
        .checker-main-bg {
            background: #FBECC3;
            width: 100%;
            margin: 0 !important;
            padding: 0;
            display: flex;
            flex-direction: column;
            align-items: center;
            box-sizing: border-box;
            overflow-x: hidden;
            left: 0;
            right: 0;
            padding-bottom: 8px;
            transition: padding-bottom 0.3s;
        }
        .checker-main-bg.expanded {
            padding-bottom: 120px;
        }
        
        /* Top Navigation Tabs */
        .checker-tabs {
            display: flex;
            justify-content: center;
            gap: 56px;
            margin-top: 48px;
            margin-bottom: -20px;
            width: 100%;
            max-width: 700px;
            font-size: 1.2rem;
        }
        .checker-tab {
            font-size: 1rem;
            color: #052940;
            background: none;
            border: none;
            outline: none;
            cursor: pointer;
            font-weight: 400;
            padding: 12px 0;
            border-bottom: 2px solid transparent;
            transition: all 0.2s ease;
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 8px;
            text-decoration: none;
        }
        .checker-tab.active {
            color: #D36F2D;
            border-bottom: 2px solid #D36F2D;
            font-weight: 400;
        }
        .checker-tab:hover {
            color: #D36F2D;
        }
        .tab-icon {
            font-size: 2.7rem;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .tab-label {
            font-size: 1.18rem;
            letter-spacing: 0.01em;
            margin-top: 2px;
            color: #052940;
        }
        .checker-tab.active .tab-label {
            color: #D36F2D;
        }
        .material-symbols-outlined {
            font-family: 'Material Symbols Outlined', sans-serif !important;
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 48;
            font-size: 2.7rem !important;
            line-height: 1;
            color: #052940;
            transition: color 0.2s;
            display: inline-flex;
            vertical-align: middle;
        }
        .checker-tab.active .material-symbols-outlined {
            color: #D36F2D;
        }
        .checker-tab .tab-icon svg {
            width: 2rem;
            height: 2rem;
            display: block;
            margin: 0 auto;
            fill: #052940;
            transition: fill 0.2s;
        }
        .checker-tab.active .tab-icon svg {
            fill: #D36F2D;
        }
        
        /* Main Content Card */
        .checker-card {
            background: transparent;
            width: 100%;
            border-radius: 0;
            box-shadow: none;
            padding: 48px 0 40px 0;
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            margin: 0;
            box-sizing: border-box;
        }
        
        .checker-title {
            font-size: 2.2rem;
            font-weight: 700;
            color: #2d3748;
            text-align: center;
            margin-bottom: 12px;
            line-height: 1.2;
        }
        .checker-desc {
            color: #666;
            font-size: 1.1rem;
            text-align: center;
            margin-bottom: 30px;
            line-height: 1.5;
        }
        
        /* Input Section */
        .input-section, .result-section {
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
        }
        .checker-input {
            width: 100%;
            min-height: 120px;
            max-width: 600px;
            border-radius: 12px;
            border: 2px solid #e2e8f0;
            padding: 16px 20px;
            font-size: 1rem;
            margin-bottom: 20px;
            background: #fff !important;
            resize: vertical;
            box-sizing: border-box;
            transition: border-color 0.2s ease;
            font-family: 'DM Sans', sans-serif;
        }
        textarea.checker-input, .checker-input[type="text"], .checker-input[type="email"], .checker-input[type="search"] {
            background: #fff !important;
        }
        .checker-input:focus {
            border-color: #b07a3c;
            outline: none;
            box-shadow: 0 0 0 3px rgba(176, 122, 60, 0.1);
        }
        .checker-input::placeholder {
            color: #a0aec0;
        }
        
        .checker-upload {
            width: 100%;
            max-width: 600px;
            margin-bottom: 24px;
            margin-left: auto;
            margin-right: auto;
        }
        .checker-upload input[type="file"] {
            width: 100%;
            padding: 16px 20px;
            border-radius: 12px;
            border: 2px solid #e2e8f0;
            background: #fff;
            font-size: 1rem;
            color: #a0aec0;
            cursor: pointer;
            transition: border-color 0.2s ease;
            text-align: center;
            position: relative;
        }
        .checker-upload input[type="file"]:hover {
            border-color: #b07a3c;
        }
        .checker-upload input[type="file"]::file-selector-button {
            display: none;
        }
        .checker-upload input[type="file"]::before {
            content: '⬇ Upload photo';
            display: block;
            width: 100%;
            text-align: center;
            color: #a0aec0;
            font-size: 1rem;
            font-family: 'DM Sans', sans-serif;
        }
        
        .checker-analyze-btn {
            width: 100%;
            max-width: 160px;
            background: #D36F2D;
            color: #fff;
            border: none;
            border-radius: 25px;
            font-size: 1.1rem;
            font-weight: 600;
            padding: 14px 32px;
            margin-top: 8px;
            cursor: pointer;
            transition: all 0.2s ease;
            text-transform: capitalize;
        }
        .checker-analyze-btn:hover {
            background: #b45a22;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(176, 122, 60, 0.3);
        }
        
        /* Result Section */
        .result-section {
            margin-top: 30px;
            padding: 20px;
            border-radius: 12px;
            background: none;
            border: none;
            width: 100%;
            max-width: 600px;
            text-align: left;
            display: block;
            margin-left: auto;
            margin-right: auto;
        }
        .result-section b {
            display: block;
            margin-top: 12px;
            margin-bottom: 4px;
            font-size: 1.08rem;
        }
        .result-success {
            background: rgba(151, 207, 151, 0.5);
            border: 1.5px solid #3C6E3C;
        }
        .result-danger {
            background: rgba(236, 119, 119, 0.5);
            border: 1.5px solid #A22424;
        }
        .hidden {
            display: none !important;
        }
        .result-title {
            font-weight: 700;
            margin-bottom: 12px;
            font-size: 1.2rem;
            color: #2d3748;
        }
        .result-content {
            line-height: 1.6;
            font-size: 1rem;
            color: #4a5568;
        }
        .result-danger-box {
            background: rgba(236, 119, 119, 0.5);
            border: 1.5px solid #A22424;
            color: #A22424;
            border-radius: 12px;
            padding: 20px;
            width: 100%;
            max-width: 600px;
            margin: 20px auto 0 auto;
            font-size: 1.08rem;
            box-sizing: border-box;
            text-align: left;
        }
        
        /* Responsive Design */
        @media (max-width: 768px) {
            .checker-main-bg {
                padding: 40px 16px 40px 16px;
            }
            .checker-tabs {
                gap: 40px;
                margin-bottom: 40px;
            }
            .checker-card {
                padding: 32px 24px 32px 24px;
            }
            .checker-title {
                font-size: 1.8rem;
            }
            .checker-desc {
                font-size: 1rem;
            }
        }
        
        @media (max-width: 480px) {
            .checker-tabs {
                gap: 30px;
            }
            .tab-icon {
                font-size: 1.5rem;
            }
            .checker-tab {
                font-size: 0.9rem;
            }
            .checker-title {
                font-size: 1.6rem;
            }
        }
        .checker-divider {
            width: 100%;
            max-width: 600px;
            height: 2px;
            background: #D36F2D;
            border-radius: 1px;
            margin: 0 0 20px 0;
            margin-left: auto;
            margin-right: auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="checker-main-bg">
        <!-- Top Navigation Tabs -->
        <div class="checker-tabs">
            <button type="button" class="checker-tab" onclick="window.location.href='/User/LinkChecker.aspx'">
                <span class="tab-icon"><span class="material-symbols-outlined">link</span></span>
                <span class="tab-label">Link Checker</span>
            </button>
            <button type="button" class="checker-tab active">
                <span class="tab-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px"><path d="M240-400h320v-80H240v80Zm0-120h480v-80H240v80Zm0-120h480v-80H240v80ZM80-80v-720q0-33 23.5-56.5T160-880h640q33 0 56.5 23.5T880-800v480q0 33-23.5 56.5T800-240H240L80-80Zm126-240h594v-480H160v525l46-45Zm-46 0v-480 480Z"/></svg>
                </span>
                <span class="tab-label">SMS / Email Checker</span>
            </button>
        </div>
        
        <!-- Main Content Card -->
        <div class="checker-card">
            <div class="checker-title">Is this message safe?</div>
            <div class="checker-desc">Detect if an SMS or Email is a phishing message or is malicious.</div>
            
            <div class="input-section">
                <asp:TextBox ID="txtUserInput" runat="server" TextMode="MultiLine" Rows="6" CssClass="checker-input" placeholder="Paste SMS content here" />
                <div class="checker-divider"></div>
                <div class="checker-upload">
                    <asp:FileUpload ID="fileScreenshot" runat="server" CssClass="" />
                </div>
                <div style="display: flex; gap: 12px; justify-content: center; align-items: center; margin-top: 8px;">
                    <asp:Button ID="btnCheckScam" runat="server" Text="Analyze" OnClick="btnCheckScam_Click" CssClass="checker-analyze-btn" />
                    <button type="button" id="btnClearInputs" class="checker-analyze-btn" style="background: #aaa; color: #fff;" onclick="clearCheckerInputs()">Clear</button>
                </div>
                <div id="checkerErrorMsg" style="display: none; width: 100%; max-width: 600px; margin: 18px auto 0 auto; padding: 12px 18px; border-radius: 8px; background: rgba(236, 119, 119, 0.5); border: 1.5px solid #A22424; color: #A22424; font-size: 1rem; text-align: center; box-shadow: 0 2px 8px rgba(162,36,36,0.07);"></div>
            </div>
            
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="result-section hidden" id="resultSection">
                        <div class="result-title">Result</div>
                        <asp:Label ID="lblResult" runat="server" CssClass="result-content"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- Why Use Scam Checker Section -->
    <div class="why-scam-section">
        <div class="why-scam-container">
            <h2 style="font-size: 2rem; font-weight: bold; font-family: 'DM Sans', sans-serif; color: #051D40; margin-bottom: 0;">What does a Scam Checker do?</h2>
            <div style="font-size: 20px; color: #051D40; font-family: 'DM Sans', sans-serif; margin-bottom: 1.2rem;">Scam Checker is a URL checker tool designed to help you detect scams, phishing attacks, and fake websites.</div>
            <div class="why-scam-content" style="margin-top: 40px;">
                <div class="why-scam-illustration">
                    <!-- You can replace this with an actual SVG or image -->
                    <img src="/Images/person.svg" alt="Scam Checker Illustration" style="width: 380px; max-width: 100%; display: block; margin: 0 auto;" />
                </div>
                <div class="why-scam-features">
                    <div class="why-scam-feature">
                        <div class="why-scam-feature-title" style="font-size: 2rem; font-weight: bold; font-family: 'DM Sans', sans-serif; color: #051D40; margin-bottom: 0;">Avoid phishing attacks</div>
                        <div class="why-scam-feature-desc" style="margin-top: 0;">Got a suspicious email or text? Check the link before clicking — it will significantly reduce the chances of you falling for a phishing attack.</div>
                    </div>
                    <div class="why-scam-feature">
                        <div class="why-scam-feature-title" style="font-size: 2rem; font-weight: bold; font-family: 'DM Sans', sans-serif; color: #051D40; margin-bottom: 0;">Block malicious websites</div>
                        <div class="why-scam-feature-desc" style="margin-top: 0;">Some websites are ridden with malware that waits to be downloaded and executed on your device. Link Checker will let you know about it before it’s too late.</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <style>
        .why-scam-section {
            background: #f6f8fa;
            width: 100%;
            padding: 64px 0 72px 0;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .why-scam-container {
            width: 100%;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
        }
        .why-scam-title {
            font-size: 2.6rem;
            font-weight: 700;
            color: #222b3a;
            margin-bottom: 18px;
            text-align: center;
            font-family: 'DM Sans', sans-serif;
        }
        .why-scam-desc {
            color: #444;
            font-size: 1.35rem;
            text-align: center;
            margin-bottom: 48px;
            font-family: 'DM Sans', sans-serif;
            max-width: 900px;
        }
        .why-scam-content {
            display: flex;
            flex-direction: row;
            align-items: center;
            gap: 72px;
            width: 100%;
            justify-content: center;
            max-width: 1200px;
            margin: 0 auto;
        }
        .why-scam-illustration {
            flex: 0 0 320px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .why-scam-illustration img {
            width: 260px;
            max-width: 100%;
        }
        .why-scam-features {
            display: flex;
            flex-direction: column;
            gap: 40px;
            max-width: 520px;
        }
        .why-scam-feature-title {
            font-size: 1.45rem;
            font-weight: 700;
            color: #222b3a;
            margin-bottom: 10px;
            font-family: 'DM Sans', sans-serif;
        }
        .why-scam-feature-desc {
            color: #444;
            font-size: 1.15rem;
            font-family: 'DM Sans', sans-serif;
        }
        @media (max-width: 1200px) {
            .why-scam-content {
                max-width: 98vw;
                gap: 36px;
            }
        }
        @media (max-width: 900px) {
            .why-scam-content {
                flex-direction: column;
                align-items: center;
                gap: 32px;
            }
            .why-scam-illustration {
                margin-bottom: 18px;
            }
        }
        @media (max-width: 600px) {
            .why-scam-title {
                font-size: 1.5rem;
            }
            .why-scam-desc {
                font-size: 1rem;
            }
            .why-scam-feature-title {
                font-size: 1.1rem;
            }
            .why-scam-feature-desc {
                font-size: 0.95rem;
            }
            .why-scam-illustration img {
                width: 140px;
            }
        }
    </style>
    <script type="text/javascript">
        // Optionally, add JS for tab switching or file upload UX
        function setResultBoxStyle() {
            var box = document.getElementById('resultSection');
            var label = document.getElementById('<%= lblResult.ClientID %>');
            if (!box || !label) return;
            box.classList.remove('result-success', 'result-danger');
            var text = label.innerText || label.textContent || '';
            var yesMatch = /\b(is this a scam\?\s*)?yes\b/i.test(text);
            var noMatch = /\b(is this a scam\?\s*)?no\b/i.test(text);
            if (yesMatch) {
                box.classList.add('result-danger');
            } else if (noMatch) {
                box.classList.add('result-success');
            }
        }

        // --- NEW: Clear and input logic ---
        function clearCheckerInputs() {
            var txt = document.getElementById('<%= txtUserInput.ClientID %>');
            var file = document.getElementById('<%= fileScreenshot.ClientID %>');
            if (txt) txt.value = '';
            if (file) file.value = '';
            // Hide result section and reset background
            var box = document.getElementById('resultSection');
            if (box) box.classList.add('hidden');
            var mainBg = document.querySelector('.checker-main-bg');
            if (mainBg) mainBg.classList.remove('expanded');
            // Hide error message
            var err = document.getElementById('checkerErrorMsg');
            if (err) { err.style.display = 'none'; err.textContent = ''; }
        }

        window.addEventListener('DOMContentLoaded', function() {
            var txt = document.getElementById('<%= txtUserInput.ClientID %>');
            var file = document.getElementById('<%= fileScreenshot.ClientID %>');
            var err = document.getElementById('checkerErrorMsg');
            if (txt && file) {
                txt.addEventListener('input', function(e) {
                    if (file.value) {
                        // Prevent typing and show error
                        txt.value = '';
                        if (err) {
                            err.textContent = 'Only one type of media can be analyzed at a time. Please clear the uploaded photo first.';
                            err.style.display = 'block';
                        }
                        e.preventDefault();
                        return false;
                    } else {
                        if (err) { err.style.display = 'none'; err.textContent = ''; }
                    }
                });
                file.addEventListener('click', function(e) {
                    // No disabling, just let the user try
                });
                file.addEventListener('change', function(e) {
                    if (txt.value.trim().length > 0) {
                        // Prevent file selection and show error
                        file.value = '';
                        if (err) {
                            err.textContent = 'Only one type of media can be analyzed at a time. Please clear the text input first.';
                            err.style.display = 'block';
                        }
                        e.preventDefault();
                        return false;
                    } else {
                        if (err) { err.style.display = 'none'; err.textContent = ''; }
                    }
                });
            }
        });
    </script>
</asp:Content>