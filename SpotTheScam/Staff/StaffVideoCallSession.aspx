<%@ Page Title="Video Call Session" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="StaffVideoCall.aspx.cs" Inherits="SpotTheScam.Staff.StaffVideoCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .video-container {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px;
            background: white;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            color: #e67e22;
        }

        .join-form {
            max-width: 600px;
            margin: 30px auto;
            padding: 30px;
            background: white;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            color: #e67e22;
        }

        .hidden {
            display: none !important;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #e67e22;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
            color: #e67e22;
        }

        .form-control::placeholder {
            color: #e67e22;
            opacity: 0.7;
        }

        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: bold;
        }

        .btn-primary {
            background-color: #e67e22;
            color: white;
        }

        .btn-primary:hover {
            background-color: #d35400;
        }

        .btn-danger {
            background-color: #dc3545;
            color: white;
        }

        .btn-danger:hover {
            background-color: #c82333;
        }

        .btn-success {
            background-color: #28a745;
            color: white;
        }

        .btn-success:hover {
            background-color: #218838;
        }

        .alert {
            padding: 15px;
            margin-bottom: 20px;
            border: 1px solid transparent;
            border-radius: 4px;
            color: #e67e22;
        }

        .alert-success {
            color: #e67e22;
            background-color: #d4edda;
            border-color: #c3e6cb;
        }

        .alert-error {
            color: #e67e22;
            background-color: #f8d7da;
            border-color: #f5c6cb;
        }

        .session-info {
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            color: #e67e22;
        }

        .session-info h4 {
            color: #e67e22;
            margin-bottom: 15px;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 15px;
        }

        .info-item {
            display: flex;
            flex-direction: column;
        }

        .info-label {
            font-weight: bold;
            color: #e67e22;
            font-size: 12px;
            text-transform: uppercase;
            margin-bottom: 3px;
        }

        .info-value {
            color: #e67e22;
            font-size: 14px;
        }

        .video-placeholder {
            width: 100%;
            height: 400px;
            background: #000;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 18px;
            margin-bottom: 20px;
        }

        .active-sessions {
            margin-top: 30px;
        }

        .session-card {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 10px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            color: #e67e22;
        }

        .session-details {
            flex: 1;
        }

        .session-name {
            font-weight: bold;
            color: #e67e22;
            margin-bottom: 5px;
        }

        .session-phone {
            color: #e67e22;
            font-size: 14px;
            margin-bottom: 5px;
            opacity: 0.8;
        }

        .session-concerns {
            color: #e67e22;
            font-size: 13px;
        }

        .no-sessions {
            text-align: center;
            color: #e67e22;
            padding: 40px;
            font-style: italic;
        }

        h2 {
            color: #e67e22;
            border-bottom: 2px solid #e67e22;
            padding-bottom: 10px;
            margin-bottom: 30px;
        }

        h3 {
            color: #e67e22;
            margin-bottom: 20px;
        }

        h5 {
            color: #e67e22;
            margin-bottom: 15px;
        }

        /* Additional orange styling for various elements */
        .text-muted {
            color: #e67e22 !important;
            opacity: 0.7;
        }

        ul {
            color: #e67e22;
        }

        p {
            color: #e67e22;
        }

        small {
            color: #e67e22;
        }

        /* Consultation guidelines section */
        .consultation-guidelines {
            margin-top: 30px; 
            padding: 20px; 
            background: #e3f2fd; 
            border-radius: 8px;
            color: #e67e22;
        }

        .consultation-guidelines h5 {
            color: #e67e22;
            margin-bottom: 15px;
        }

        .consultation-guidelines ul {
            margin: 0; 
            padding-left: 20px; 
            color: #e67e22;
        }

        .consultation-guidelines li {
            color: #e67e22;
        }

        /* Scam concerns display area */
        .scam-concerns-display {
            margin-top: 5px; 
            padding: 10px; 
            background: white; 
            border-radius: 5px;
            color: #e67e22;
        }
    </style>

    <!-- Alert Panel -->
    <asp:Panel ID="AlertPanel" runat="server" CssClass="alert" Visible="false">
        <asp:Label ID="AlertMessage" runat="server"></asp:Label>
    </asp:Panel>

    <!-- Join Session Form -->
    <div id="JoinForm" runat="server" class="join-form">
        <h2>Join Video Call Session</h2>
        
        <div class="form-group">
            <label for="SessionIdInput">Enter Session ID or Customer Phone Number:</label>
            <asp:TextBox ID="SessionIdInput" runat="server" CssClass="form-control" 
                        placeholder="e.g., 12345 or +65 9123 4567"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <asp:Button ID="JoinSessionButton" runat="server" 
                       Text="Join Session" 
                       CssClass="btn btn-primary" 
                       OnClick="JoinSessionButton_Click" />
        </div>

        <!-- Active Waiting Sessions -->
        <div class="active-sessions">
            <h3>Customers Waiting for Help</h3>
            
            <asp:Panel ID="NoActiveSessionsPanel" runat="server" CssClass="no-sessions">
                <p>No customers are currently waiting for assistance.</p>
                <p><small>This list will automatically refresh when customers join sessions.</small></p>
            </asp:Panel>

            <asp:Repeater ID="ActiveSessionsRepeater" runat="server">
                <ItemTemplate>
                    <div class="session-card">
                        <div class="session-details">
                            <div class="session-name"><%# Eval("CustomerName") %></div>
                            <div class="session-phone">📞 <%# Eval("CustomerPhone") %></div>
                            <div class="session-concerns">
                                <strong>Concerns:</strong> <%# Eval("ScamConcerns") %>
                            </div>
                            <small class="text-muted">Joined: <%# Eval("JoinedAt", "{0:HH:mm:ss}") %></small>
                        </div>
                        <div>
                            <asp:Button ID="QuickJoinButton" runat="server" 
                                       Text="Join Now" 
                                       CssClass="btn btn-success" 
                                       CommandArgument='<%# Eval("Id") %>'
                                       OnClick="QuickJoinButton_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Video Call Interface -->
    <div id="VideoCallInterface" runat="server" class="video-container hidden">
        <h2>Video Call in Progress</h2>
        
        <!-- Session Information -->
        <div class="session-info">
            <h4>Session Information</h4>
            <div class="info-grid">
                <div class="info-item">
                    <span class="info-label">Session ID</span>
                    <span class="info-value">
                        <asp:Label ID="SessionIdLabel" runat="server"></asp:Label>
                    </span>
                </div>
                <div class="info-item">
                    <span class="info-label">Customer Name</span>
                    <span class="info-value">
                        <asp:Label ID="CustomerNameLabel" runat="server"></asp:Label>
                    </span>
                </div>
                <div class="info-item">
                    <span class="info-label">Phone Number</span>
                    <span class="info-value">
                        <asp:Label ID="CustomerPhoneLabel" runat="server"></asp:Label>
                    </span>
                </div>
                <div class="info-item">
                    <span class="info-label">Session Date</span>
                    <span class="info-value">
                        <asp:Label ID="SessionDateLabel" runat="server"></asp:Label>
                    </span>
                </div>
                <div class="info-item">
                    <span class="info-label">Session Time</span>
                    <span class="info-value">
                        <asp:Label ID="SessionTimeLabel" runat="server"></asp:Label>
                    </span>
                </div>
            </div>
            <div style="margin-top: 15px;">
                <span class="info-label">Customer's Scam Concerns</span>
                <div class="scam-concerns-display">
                    <asp:Label ID="ScamConcernsLabel" runat="server"></asp:Label>
                </div>
            </div>
        </div>

        <!-- Video Call Area -->
        <div class="video-placeholder">
            <div style="text-align: center;">
                <div style="font-size: 24px; margin-bottom: 10px;">📹</div>
                <div>Video Call Interface</div>
                <div style="font-size: 14px; margin-top: 10px; opacity: 0.7;">
                    Video calling functionality would be integrated here<br/>
                    (WebRTC, Zoom SDK, or similar video calling solution)
                </div>
            </div>
        </div>

        <!-- Call Controls -->
        <div style="text-align: center;">
            <asp:Button ID="EndCallButton" runat="server" 
                       Text="End Call" 
                       CssClass="btn btn-danger" 
                       OnClick="EndCallButton_Click"
                       OnClientClick="return confirm('Are you sure you want to end this call?');" />
        </div>

        <!-- Helper Information -->
        <div class="consultation-guidelines">
            <h5>💡 Consultation Guidelines</h5>
            <ul>
                <li>Listen carefully to the customer's specific scam concerns</li>
                <li>Provide clear, actionable advice on scam prevention</li>
                <li>Share relevant examples and warning signs</li>
                <li>Recommend security best practices for their situation</li>
                <li>Document any important points for follow-up if needed</li>
            </ul>
        </div>
    </div>

    <!-- Auto-refresh script for active sessions -->
    <script type="text/javascript">
        // Auto-refresh active sessions every 30 seconds
        setInterval(function() {
            if (document.getElementById('<%= JoinForm.ClientID %>').style.display !== 'none') {
                __doPostBack('RefreshSessions', '');
            }
        }, 30000);
    </script>
</asp:Content>