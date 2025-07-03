<%@ Page Title="Video Call Session" Language="C#" MasterPageFile="~/Staff.Master" AutoEventWireup="true" CodeBehind="StaffVideoCall.aspx.cs" Inherits="SpotTheScam.StaffVideoCall" UnobtrusiveValidationMode="None" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            background-color: #f4f1e8;
        }
        
        .video-container {
            max-width: 1200px;
            margin: 20px auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        
        .session-info {
            background-color: #f8f9fa;
            padding: 20px;
            border-bottom: 1px solid #ddd;
        }
        
        .session-details {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 0;
        }
        
        .detail-item {
            display: flex;
            align-items: center;
        }
        
        .detail-label {
            font-weight: bold;
            color: #666;
            margin-right: 10px;
            min-width: 100px;
        }
        
        .detail-value {
            color: #333;
        }
        
        .video-section {
            padding: 20px;
        }
        
        .video-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 20px;
        }
        
        .video-panel {
            background-color: #f8f9fa;
            border-radius: 10px;
            overflow: hidden;
            border: 2px solid #ddd;
        }
        
        .video-header {
            background-color: #e67e22;
            color: white;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }
        
        .video-frame {
            position: relative;
            height: 300px;
            background-color: #000;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .customer-video {
            width: 100%;
            height: 100%;
            background-color: #333;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #666;
            font-size: 16px;
        }
        
        .staff-video {
            width: 100%;
            height: 100%;
            background-color: #000;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #666;
            font-size: 16px;
        }
        
        .connection-status {
            position: absolute;
            bottom: 10px;
            left: 10px;
            background-color: rgba(40, 167, 69, 0.9);
            color: white;
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
        }
        
        .controls {
            text-align: center;
            padding: 20px;
            background-color: #f8f9fa;
            border-top: 1px solid #ddd;
        }
        
        .btn {
            background-color: #e67e22;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            font-size: 14px;
            cursor: pointer;
            margin: 0 10px;
        }
        
        .btn:hover {
            background-color: #d35400;
        }
        
        .btn-danger {
            background-color: #e74c3c;
        }
        
        .btn-danger:hover {
            background-color: #c0392b;
        }
        
        .join-form {
            max-width: 600px;
            margin: 30px auto;
            background: white;
            padding: 30px;
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            text-align: center;
        }
        
        .form-group {
            margin-bottom: 20px;
            text-align: left;
        }
        
        label {
            display: block;
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
        }
        
        input[type="text"], .form-control {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 8px;
            font-size: 16px;
            box-sizing: border-box;
        }
        
        input[type="text"]:focus, .form-control:focus {
            border-color: #e67e22;
            outline: none;
        }
        
        .alert {
            padding: 15px;
            margin-bottom: 20px;
            border-radius: 8px;
            font-weight: bold;
        }
        
        .alert-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .alert-error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        h2 {
            color: #e67e22;
            margin-bottom: 20px;
        }
        
        h3 {
            color: #e67e22;
        }
        
        .hidden {
            display: none !important;
        }
        
        .video-container.hidden {
            display: none !important;
        }
        
        .join-form.hidden {
            display: none !important;
        }
        
        .active-sessions {
            max-width: 800px;
            margin: 20px auto;
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 3px 10px rgba(0,0,0,0.1);
        }
        
        .session-card {
            border: 2px solid #ddd;
            border-radius: 10px;
            padding: 15px;
            margin: 10px 0;
            transition: all 0.3s ease;
        }
        
        .session-card:hover {
            border-color: #e67e22;
            box-shadow: 0 3px 10px rgba(230, 126, 34, 0.2);
        }
        
        .session-urgent {
            border-color: #e74c3c;
            background-color: #fff5f5;
        }
        
        @media (max-width: 768px) {
            .video-grid {
                grid-template-columns: 1fr;
            }
            
            .session-details {
                grid-template-columns: 1fr;
            }
        }
    </style>

    <!-- Join Session Form (Default View) -->
    <div id="JoinForm" runat="server" class="join-form">
        <h2>🎥 Join Video Session</h2>
        <p>Enter a Session ID to join an active video call with a customer</p>
        
        <!-- Alert Panel -->
        <asp:Panel ID="AlertPanel" runat="server" CssClass="hidden">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>
        
        <div class="form-group">
            <label for="<%= SessionIdInput.ClientID %>">Session ID or Customer Phone Number:</label>
            <asp:TextBox ID="SessionIdInput" runat="server" placeholder="Enter Session ID (e.g., 123) or Phone (+65 XXXX XXXX)" CssClass="form-control"></asp:TextBox>
        </div>
        
        <asp:Button ID="JoinSessionButton" runat="server" 
                   Text="Join Session" 
                   CssClass="btn" 
                   OnClick="JoinSessionButton_Click" />
    </div>

    <!-- Active Sessions List -->
    <div class="active-sessions">
        <h3>🔴 Active Sessions Waiting for Expert</h3>
        <asp:Panel ID="NoActiveSessionsPanel" runat="server">
            <div style="text-align: center; padding: 20px; color: #666;">
                No active sessions at the moment. Customers will appear here when they join.
            </div>
        </asp:Panel>
        
        <asp:Repeater ID="ActiveSessionsRepeater" runat="server">
            <ItemTemplate>
                <div class="session-card session-urgent">
                    <div style="display: flex; justify-content: between; align-items: center;">
                        <div style="flex: 1;">
                            <strong>📞 Customer Waiting:</strong> <%# Eval("CustomerName") %><br>
                            <strong>📱 Phone:</strong> <%# Eval("CustomerPhone") %><br>
                            <strong>🕒 Joined:</strong> <%# Eval("JoinedAt", "{0:HH:mm:ss}") %><br>
                            <strong>🎯 Concerns:</strong> <%# Eval("ScamConcerns") %>
                        </div>
                        <div>
                            <asp:Button ID="QuickJoinButton" runat="server" 
                                       Text="Join Now" 
                                       CssClass="btn" 
                                       CommandArgument='<%# Eval("Id") %>'
                                       OnClick="QuickJoinButton_Click" />
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- Video Call Interface (Hidden by default) -->
    <div id="VideoCallInterface" runat="server" class="video-container hidden">
        <!-- Session Information -->
        <div class="session-info">
            <h3>🛡️ Scam Prevention Video Session</h3>
            <div class="session-details">
                <div class="detail-item">
                    <span class="detail-label">Session ID:</span>
                    <span class="detail-value"><asp:Label ID="SessionIdLabel" runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-label">Customer:</span>
                    <span class="detail-value"><asp:Label ID="CustomerNameLabel" runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-label">Phone:</span>
                    <span class="detail-value"><asp:Label ID="CustomerPhoneLabel" runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-label">Session Date:</span>
                    <span class="detail-value"><asp:Label ID="SessionDateLabel" runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-label">Session Time:</span>
                    <span class="detail-value"><asp:Label ID="SessionTimeLabel" runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-label">Scam Concerns:</span>
                    <span class="detail-value"><asp:Label ID="ScamConcernsLabel" runat="server"></asp:Label></span>
                </div>
            </div>
        </div>
        
        <!-- Video Section -->
        <div class="video-section">
            <div class="video-grid">
                <!-- Customer Video -->
                <div class="video-panel">
                    <div class="video-header">Customer Video</div>
                    <div class="video-frame">
                        <div class="customer-video">
                            <div style="text-align: center;">
                                <div style="font-size: 48px; margin-bottom: 10px;">👤</div>
                                <div>Customer Connected</div>
                                <div style="font-size: 14px; opacity: 0.8;">Waiting for you to join...</div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Staff Video -->
                <div class="video-panel">
                    <div class="video-header">Your Video (Expert)</div>
                    <div class="video-frame">
                        <div class="staff-video">
                            <div style="text-align: center;">
                                <div style="font-size: 48px; margin-bottom: 10px;">👨‍💼</div>
                                <div>Your Camera</div>
                                <div style="font-size: 14px; opacity: 0.8;">Click Camera to enable</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Connection Status -->
            <div class="connection-status" id="ConnectionStatus">
                ✅ Connected - Ready to help customer
            </div>
        </div>
        
        <!-- Controls -->
        <div class="controls">
            <button type="button" class="btn" onclick="toggleMute()">🎤 Mute</button>
            <button type="button" class="btn" onclick="toggleCamera()">📹 Camera</button>
            <button type="button" class="btn" onclick="openChat()">💬 Chat</button>
            <button type="button" class="btn" onclick="shareScreen()">🖥️ Share Screen</button>
            <asp:Button ID="EndCallButton" runat="server" 
                       Text="📞 End Call" 
                       CssClass="btn btn-danger" 
                       OnClick="EndCallButton_Click"
                       OnClientClick="return confirm('Are you sure you want to end the call?');" />
        </div>
    </div>

    <script>
        // Auto-refresh active sessions every 10 seconds
        setInterval(function() {
            if (document.getElementById('<%= JoinForm.ClientID %>').style.display !== 'none') {
                __doPostBack('RefreshSessions', '');
            }
        }, 10000);

        // Video call controls
        let isMuted = false;
        let isCameraOn = false;

        function toggleMute() {
            isMuted = !isMuted;
            const btn = event.target;
            btn.textContent = isMuted ? '🔇 Unmute' : '🎤 Mute';
            btn.style.backgroundColor = isMuted ? '#e74c3c' : '#e67e22';
        }

        function toggleCamera() {
            isCameraOn = !isCameraOn;
            const btn = event.target;
            btn.textContent = isCameraOn ? '📹 Camera On' : '📷 Camera Off';
            btn.style.backgroundColor = isCameraOn ? '#e67e22' : '#e74c3c';
        }

        function openChat() {
            alert('Chat feature coming soon!');
        }

        function shareScreen() {
            alert('Screen sharing will help show scam examples to the customer!');
        }
    </script>
</asp:Content>