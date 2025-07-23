<%@ Page Title="My Sessions" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserMySessions.aspx.cs" Inherits="SpotTheScam.User.UserMySessions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-header {
            background: linear-gradient(135deg, var(--brand-orange) 0%, #e67e22 100%);
            color: white;
            padding: 30px 0;
            margin: -20px -15px 30px -15px;
            border-radius: 0 0 15px 15px;
        }

        .sessions-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 15px;
        }

        .session-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            border-left: 5px solid var(--brand-orange);
            transition: transform 0.3s ease;
        }

        .session-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.15);
        }

        .session-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 20px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .session-title {
            font-size: 1.4rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin: 0;
            flex: 1;
        }

        .session-status {
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.85rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .status-upcoming {
            background: #e3f2fd;
            color: #1976d2;
        }

        .status-live {
            background: #e8f5e8;
            color: #2e7d32;
            animation: pulse 2s infinite;
        }

        .status-completed {
            background: #f3e5f5;
            color: #7b1fa2;
        }

        .status-too-early {
            background: #fff3e0;
            color: #f57c00;
        }

        @keyframes pulse {
            0% { opacity: 1; }
            50% { opacity: 0.7; }
            100% { opacity: 1; }
        }

        .session-info {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 20px;
        }

        .info-item {
            display: flex;
            align-items: center;
            font-size: 0.95rem;
            color: #555;
        }

        .info-icon {
            margin-right: 10px;
            color: var(--brand-orange);
            width: 20px;
            font-size: 1.1rem;
        }

        .session-description {
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
            font-size: 0.95rem;
        }

        .session-actions {
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
        }

        .btn-join {
            background: linear-gradient(135deg, #4caf50, #45a049);
            color: white;
            padding: 12px 25px;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 1rem;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-join:hover {
            background: linear-gradient(135deg, #45a049, #4caf50);
            transform: translateY(-2px);
            color: white;
        }

        .btn-join:disabled {
            background: #cccccc;
            cursor: not-allowed;
            transform: none;
        }

        .btn-copy {
            background: #2196f3;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            font-weight: 500;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-copy:hover {
            background: #1976d2;
        }

        .session-link {
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 6px;
            padding: 8px 12px;
            font-family: monospace;
            font-size: 0.85rem;
            word-break: break-all;
            color: #495057;
            margin: 10px 0;
        }

        .time-info {
            background: #f0f8ff;
            border: 1px solid #b3d9ff;
            border-radius: 8px;
            padding: 15px;
            margin-top: 15px;
            font-size: 0.9rem;
        }

        .countdown {
            font-weight: 600;
            color: var(--brand-orange);
        }

        .no-sessions {
            text-align: center;
            padding: 60px 20px;
            color: #666;
        }

        .no-sessions img {
            width: 150px;
            opacity: 0.5;
            margin-bottom: 20px;
        }

        .alert {
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .alert-info {
            background-color: #d1ecf1;
            border: 1px solid #bee5eb;
            color: #0c5460;
        }

        .alert-warning {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
        }

        @media (max-width: 768px) {
            .session-header {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .session-info {
                grid-template-columns: 1fr;
                gap: 15px;
            }
            
            .session-actions {
                flex-direction: column;
                align-items: stretch;
            }
            
            .btn-join {
                justify-content: center;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Page Header -->
    <div class="page-header">
        <h1 style="text-align: center; margin: 0; font-size: 2rem; font-weight: 700;">My Sessions</h1>
        <p style="text-align: center; margin: 10px 0 0 0; opacity: 0.9;">Manage your expert consultation sessions</p>
    </div>

    <div class="sessions-container">
        <!-- Alert Panel -->
        <asp:Panel ID="AlertPanel" runat="server" CssClass="alert" Visible="false">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Info Alert -->
        <div class="alert alert-info">
            <strong>💡 How to Join:</strong> Click "Join Session" when it's available (10 minutes before start time). 
            Make sure you have a stable internet connection and enable camera/microphone permissions.
        </div>

        <!-- Sessions List -->
        <asp:Repeater ID="rptSessions" runat="server">
            <ItemTemplate>
                <div class="session-card">
                    <div class="session-header">
                        <h3 class="session-title"><%# Eval("SessionTitle") %></h3>
                        <span class="session-status status-upcoming">
                            Scheduled
                        </span>
                    </div>
                    
                    <div class="session-info">
                        <div class="info-item">
                            <span class="info-icon">📅</span>
                            <span><strong>Date:</strong> <%# Convert.ToDateTime(Eval("SessionDate")).ToString("dddd, dd MMMM yyyy") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">🕐</span>
                            <span><strong>Time:</strong> <%# Eval("StartTime") %> - <%# Eval("EndTime") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">👨‍💼</span>
                            <span><strong>Expert:</strong> <%# Eval("ExpertName") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">📚</span>
                            <span><strong>Topic:</strong> <%# Eval("SessionTopic") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">⭐</span>
                            <span><strong>Points Used:</strong> <%# Eval("PointsUsed") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">📅</span>
                            <span><strong>Registered:</strong> <%# Convert.ToDateTime(Eval("RegistrationDate")).ToString("dd/MM/yyyy") %></span>
                        </div>
                    </div>
                    
                    <div class="session-description">
                        <%# Eval("SessionDescription") %>
                    </div>
                    
                    <!-- Session Link -->
                    <div style="margin: 15px 0;">
                        <strong>🔗 Session Link:</strong>
                        <div class="session-link" id="link_<%# Eval("Id") %>">
                            <%# Eval("SessionLink") %>
                        </div>
                        <button type="button" class="btn-copy" onclick="copyLink('<%# Eval("Id") %>')">
                            📋 Copy Link
                        </button>
                    </div>
                    
                    <!-- Time Information -->
                    <div class="time-info">
                        <div id="timeInfo_<%# Eval("Id") %>">
                            ⏰ Session scheduled for <%# Convert.ToDateTime(Eval("SessionDate")).ToString("dddd, dd MMMM yyyy") %> at <%# Eval("StartTime") %>
                        </div>
                    </div>
                    
                    <!-- Actions -->
                    <div class="session-actions">
                        <asp:HyperLink ID="lnkJoinSession" runat="server" 
                            NavigateUrl='<%# Eval("SessionLink") %>' 
                            CssClass="btn-join"
                            Target="_blank"
                            Text="🚀 Join Session Now" />
                        
                        <span style="font-size: 0.9rem; color: #666;">
                            You can join 10 minutes before the session starts
                        </span>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- No Sessions Message -->
        <asp:Panel ID="pnlNoSessions" runat="server" CssClass="no-sessions" Visible="false">
            <div style="font-size: 4rem; margin-bottom: 20px;">📅</div>
            <h3>No Sessions Yet</h3>
            <p>You haven't registered for any expert sessions yet.</p>
            <p><a href="UserWebinarSessionListing.aspx" style="color: var(--brand-orange); font-weight: 600;">Browse Available Sessions →</a></p>
        </asp:Panel>
    </div>

    <script>
        function copyLink(sessionId) {
            var linkElement = document.getElementById('link_' + sessionId);
            var linkText = linkElement.innerText || linkElement.textContent;
            
            navigator.clipboard.writeText(linkText).then(function() {
                alert('Session link copied to clipboard!');
            }, function() {
                // Fallback for older browsers
                var textArea = document.createElement('textarea');
                textArea.value = linkText;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                alert('Session link copied to clipboard!');
            });
        }

        // Auto-hide alerts after 5 seconds
        setTimeout(function() {
            var alertPanel = document.getElementById('<%= AlertPanel.ClientID %>');
            if (alertPanel && alertPanel.style.display !== 'none') {
                alertPanel.style.display = 'none';
            }
        }, 5000);

        // Update countdowns every minute (optional)
        setInterval(function () {
            // You can add dynamic time updates here if needed
        }, 60000);
    </script>
</asp:Content>