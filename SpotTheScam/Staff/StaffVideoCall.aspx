<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="StaffVideoCall.aspx.cs" Inherits="SpotTheScam.Staff.StaffVideoCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/peerjs/1.4.7/peerjs.min.js"></script>
    <style>
        .main-container {
            max-width: 1400px;
            margin: 0 auto;
            padding: 20px;
        }

        .session-overview {
            background: linear-gradient(135deg, #D36F2D 0%, #e67e22 100%);
            color: white;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 30px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .session-overview h2 {
            color: white;
            margin: 0 0 15px 0;
            font-size: 1.8rem;
            font-weight: 600;
        }

        .session-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-top: 20px;
        }

        .stat-card {
            background: rgba(255,255,255,0.1);
            padding: 15px;
            border-radius: 10px;
            text-align: center;
            backdrop-filter: blur(10px);
        }

        .stat-number {
            font-size: 2rem;
            font-weight: bold;
            display: block;
        }

        .stat-label {
            font-size: 0.9rem;
            opacity: 0.9;
        }

        .connection-modes {
            background: white;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .mode-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: #051D40;
            margin: 0 0 20px 0;
        }

        .mode-buttons {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 15px;
        }

        .mode-btn {
            background: linear-gradient(135deg, #D36F2D, #e67e22);
            color: white;
            border: none;
            padding: 20px;
            border-radius: 12px;
            font-weight: 600;
            font-size: 1rem;
            cursor: pointer;
            transition: all 0.3s ease;
            text-align: center;
            box-shadow: 0 3px 10px rgba(0,0,0,0.1);
        }

        .mode-btn:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.2);
        }

        .mode-btn.secondary {
            background: linear-gradient(135deg, #6c757d, #5a6268);
        }

        .mode-btn.success {
            background: linear-gradient(135deg, #28a745, #218838);
        }

        .participants-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            margin-bottom: 30px;
        }

        .participants-section {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .section-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: #051D40;
            margin: 0;
        }

        .participant-count {
            background: #D36F2D;
            color: white;
            padding: 5px 12px;
            border-radius: 15px;
            font-size: 0.85rem;
            font-weight: 600;
        }

        .participant-card {
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 15px;
            transition: all 0.3s ease;
        }

        .participant-card:hover {
            background: #e9f2ff;
            border-color: #D36F2D;
        }

        .participant-card.online {
            border-left: 4px solid #28a745;
            background: #f0fff4;
        }

        .participant-card.connected {
            border-left: 4px solid #007bff;
            background: #f0f8ff;
        }

        .participant-info {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .participant-details h4 {
            color: #051D40;
            margin: 0 0 5px 0;
            font-size: 1rem;
        }

        .participant-meta {
            color: #666;
            font-size: 0.85rem;
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
        }

        .participant-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .btn-connect {
            background: #28a745;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 0.85rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-connect:hover {
            background: #218838;
        }

        .btn-connect:disabled {
            background: #6c757d;
            cursor: not-allowed;
        }

        .btn-disconnect {
            background: #dc3545;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 0.85rem;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-disconnect:hover {
            background: #c82333;
        }

        .status-indicator {
            display: inline-block;
            width: 10px;
            height: 10px;
            border-radius: 50%;
            margin-right: 8px;
        }

        .status-waiting {
            background: #ffc107;
        }

        .status-online {
            background: #28a745;
        }

        .status-connected {
            background: #007bff;
        }

        .status-offline {
            background: #6c757d;
        }

        .video-container {
            display: block;
            background: white;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .video-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
        }

        .video-wrapper {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 10px;
            text-align: center;
        }

        .video-wrapper h4 {
            color: #051D40;
            margin: 0 0 10px 0;
            font-size: 1rem;
        }

        video {
            width: 100%;
            max-width: 400px;
            border-radius: 8px;
            background: #000;
        }

        .control-panel {
            background: white;
            border-radius: 15px;
            padding: 25px;
            text-align: center;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .status-message {
            padding: 15px 20px;
            margin: 15px 0;
            border-radius: 10px;
            text-align: center;
            font-weight: 500;
        }

        .error {
            background-color: #ffebee;
            color: #c62828;
            border: 1px solid #ef9a9a;
        }

        .info {
            background-color: #e3f2fd;
            color: #1976d2;
            border: 1px solid #90caf9;
        }

        .success {
            background-color: #e8f5e8;
            color: #2e7d32;
            border: 1px solid #81c784;
        }

        .no-participants {
            text-align: center;
            padding: 40px;
            color: #666;
        }

        .refresh-btn {
            background: #6c757d;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 0.85rem;
            cursor: pointer;
            margin-left: 10px;
        }

        .refresh-btn:hover {
            background: #5a6268;
        }

        .search-participants {
            margin-bottom: 20px;
        }

        .search-input {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 0.9rem;
        }

        .search-input:focus {
            outline: none;
            border-color: #D36F2D;
            box-shadow: 0 0 0 2px rgba(211, 111, 45, 0.2);
        }

        .bulk-notification {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            display: none;
        }

        .bulk-notification.show {
            display: block;
        }

        .notification-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 15px;
        }

        .notification-info {
            flex: 1;
        }

        .notification-actions {
            display: flex;
            gap: 10px;
        }

        .btn-notify {
            background: #17a2b8;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-notify:hover {
            background: #138496;
        }

        @media (max-width: 768px) {
            .participants-grid {
                grid-template-columns: 1fr;
            }
            
            .session-stats {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .mode-buttons {
                grid-template-columns: 1fr;
            }
        }
        /* ADD THESE STYLES TO YOUR EXISTING <style> SECTION IN StaffVideoCall.aspx */

        /* MULTI-PARTICIPANT VIDEO CALL STYLES */
        .expert-video-section {
            background: linear-gradient(135deg, #D36F2D, #e67e22);
            padding: 20px;
            border-radius: 12px;
            margin-bottom: 20px;
        }

        .expert-video {
            text-align: center;
        }

        .expert-video h4 {
            color: white;
            margin: 0 0 15px 0;
            font-size: 1.2rem;
            font-weight: 600;
        }

        .expert-video video {
            width: 100%;
            max-width: 400px;
            height: 300px;
            border-radius: 10px;
            background: #000;
            object-fit: cover;
        }

        .video-controls {
            margin-top: 15px;
            display: flex;
            justify-content: center;
            gap: 10px;
        }

        .video-controls button {
            background: rgba(255, 255, 255, 0.2);
            color: white;
            border: 1px solid rgba(255, 255, 255, 0.3);
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
            font-weight: 500;
            transition: all 0.3s;
        }

        .video-controls button:hover {
            background: rgba(255, 255, 255, 0.3);
            transform: translateY(-2px);
        }

        .participants-video-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 20px;
            margin-bottom: 20px;
            min-height: 300px;
        }

        .participant-video-wrapper {
            background: #f8f9fa;
            border: 2px solid #e9ecef;
            border-radius: 12px;
            padding: 15px;
            transition: all 0.3s ease;
            position: relative;
        }

        .participant-video-wrapper:hover {
            border-color: #D36F2D;
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.1);
        }

        .participant-video-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 10px;
        }

        .participant-video-header h4 {
            color: #051D40;
            margin: 0;
            font-size: 1rem;
            font-weight: 600;
        }

        .participant-actions {
            display: flex;
            gap: 5px;
        }

        .btn-mute-participant,
        .btn-disconnect-participant {
            background: transparent;
            border: 1px solid #ddd;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 0.75rem;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-mute-participant:hover {
            background: #ffc107;
            border-color: #ffc107;
            color: white;
        }

        .btn-disconnect-participant:hover {
            background: #dc3545;
            border-color: #dc3545;
            color: white;
        }

        .participant-video-wrapper video {
            width: 100%;
            height: 200px;
            border-radius: 8px;
            background: #000;
            object-fit: cover;
        }

        .participant-status {
            margin-top: 10px;
            display: flex;
            align-items: center;
            font-size: 0.85rem;
            color: #666;
        }

        .status-indicator.online {
            width: 8px;
            height: 8px;
            background: #28a745;
            border-radius: 50%;
            margin-right: 8px;
        }

        .no-participants-connected {
            grid-column: 1 / -1;
            text-align: center;
            padding: 60px 20px;
            background: #f8f9fa;
            border: 2px dashed #ddd;
            border-radius: 12px;
            color: #666;
        }

        .no-participants-connected p {
            margin: 10px 0;
        }

        .no-participants-connected p:first-child {
            font-weight: 600;
            font-size: 1.1rem;
            color: #333;
        }

        .multi-controls {
            display: flex;
            justify-content: center;
            gap: 15px;
            flex-wrap: wrap;
            margin-top: 20px;
            padding: 20px;
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .btn-connect-all {
            background: linear-gradient(135deg, #28a745, #20c997);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            box-shadow: 0 3px 10px rgba(40, 167, 69, 0.3);
        }

        .btn-connect-all:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(40, 167, 69, 0.4);
        }

        .btn-disconnect-all {
            background: linear-gradient(135deg, #dc3545, #c82333);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            box-shadow: 0 3px 10px rgba(220, 53, 69, 0.3);
        }

        .btn-disconnect-all:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(220, 53, 69, 0.4);
        }

        .btn-layout {
            background: linear-gradient(135deg, #6c757d, #5a6268);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-layout:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(108, 117, 125, 0.4);
        }

        .btn-cleanup {
            background: linear-gradient(135deg, #17a2b8, #138496);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-cleanup:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(23, 162, 184, 0.4);
        }

        /* RESPONSIVE LAYOUTS */

        /* 1 participant */
        .participants-video-grid:has(.participant-video-wrapper:only-child) {
            grid-template-columns: 1fr;
            max-width: 600px;
            margin: 0 auto 20px auto;
        }

        /* 2 participants */
        .participants-video-grid:has(.participant-video-wrapper:nth-child(2):last-child) {
            grid-template-columns: repeat(2, 1fr);
        }

        /* 3-4 participants */
        .participants-video-grid:has(.participant-video-wrapper:nth-child(3)) {
            grid-template-columns: repeat(2, 1fr);
        }

        /* 5-6 participants */
        .participants-video-grid:has(.participant-video-wrapper:nth-child(5)) {
            grid-template-columns: repeat(3, 1fr);
        }

        /* 7+ participants */
        .participants-video-grid:has(.participant-video-wrapper:nth-child(7)) {
            grid-template-columns: repeat(4, 1fr);
        }

        /* Mobile responsive */
        @media (max-width: 768px) {
            .participants-video-grid {
                grid-template-columns: 1fr !important;
                gap: 15px;
            }
            
            .participant-video-wrapper video {
                height: 250px;
            }
            
            .expert-video video {
                max-width: 100%;
                height: 250px;
            }
            
            .multi-controls {
                flex-direction: column;
                align-items: center;
            }
            
            .multi-controls button {
                width: 100%;
                max-width: 300px;
            }
        }

        @media (max-width: 1200px) and (min-width: 769px) {
            .participants-video-grid:has(.participant-video-wrapper:nth-child(3)) {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .participants-video-grid:has(.participant-video-wrapper:nth-child(5)) {
                grid-template-columns: repeat(2, 1fr);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main-container">
        <asp:HiddenField ID="hdnSessionId" runat="server" />
        <asp:HiddenField ID="hdnCurrentConnection" runat="server" />
        
        <!-- Session Overview -->
        <div class="session-overview">
            <h2>Expert Video Session Management</h2>
            <asp:Label ID="lblSessionInfo" runat="server" />
            
            <div class="session-stats">
                <div class="stat-card">
                    <span class="stat-number" id="totalParticipants"><asp:Label ID="lblTotalParticipants" runat="server" Text="0" /></span>
                    <span class="stat-label">Total Registered</span>
                </div>
                <div class="stat-card">
                    <span class="stat-number" id="onlineParticipants">0</span>
                    <span class="stat-label">Currently Online</span>
                </div>
                <div class="stat-card">
                    <span class="stat-number" id="connectedParticipants">0</span>
                    <span class="stat-label">In Video Call</span>
                </div>
                <div class="stat-card">
                    <span class="stat-number" id="sessionDuration">0:00</span>
                    <span class="stat-label">Session Duration</span>
                </div>
            </div>
        </div>

        <!-- Connection Mode Selection -->
        <div class="connection-modes">
            <h3 class="mode-title">🎯 Choose Your Session Mode</h3>
            <div class="mode-buttons">
                <button type="button" class="mode-btn" onclick="startBroadcastMode()">
                    📢 Broadcast Mode<br>
                    <small>Connect to all participants simultaneously</small>
                </button>
                <button type="button" class="mode-btn secondary" onclick="startGroupMode()">
                    👥 Group Call Mode<br>
                    <small>Select specific participants for group discussion</small>
                </button>
                <button type="button" class="mode-btn success" onclick="startOneOnOneMode()">
                    📞 One-on-One Mode<br>
                    <small>Individual consultations with participants</small>
                </button>
            </div>
        </div>

        <!-- Bulk Notification Panel -->
        <div id="bulkNotification" class="bulk-notification">
            <div class="notification-content">
                <div class="notification-info">
                    <strong>📢 Ready to start?</strong>
                    <p>Notify all participants that the expert session is about to begin.</p>
                </div>
                <div class="notification-actions">
                    <button type="button" class="btn-notify" onclick="notifyAllParticipants()">
                        📤 Notify All Participants
                    </button>
                </div>
            </div>
        </div>

        <!-- Participants Grid -->
        <div class="participants-grid">
            <!-- Registered Participants -->
            <div class="participants-section">
                <div class="section-header">
                    <h3 class="section-title">📋 Registered Participants</h3>
                    <span class="participant-count">
                        <asp:Label ID="lblRegisteredCount" runat="server" Text="0" />
                    </span>
                </div>
                
                <div class="search-participants">
                    <input type="text" class="search-input" placeholder="🔍 Search participants by name or phone..." 
                           onkeyup="filterParticipants(this.value)" />
                </div>

                <div id="registeredParticipants">
                    <asp:Repeater ID="rptRegisteredParticipants" runat="server">
                        <ItemTemplate>
                            <div class="participant-card" data-phone="<%# Eval("CustomerPhone") %>" data-name="<%# Eval("CustomerName") %>">
                                <div class="participant-info">
                                    <div class="participant-details">
                                        <h4>
                                            <span class="status-indicator status-waiting" id="status_<%# Eval("CustomerPhone").ToString().Replace("+", "").Replace(" ", "") %>"></span>
                                            <%# Eval("CustomerName") %>
                                        </h4>
                                        <div class="participant-meta">
                                            <span>📱 <%# Eval("CustomerPhone") %></span>
                                            <span>📧 <%# Eval("CustomerEmail") %></span>
                                            <span>📅 <%# Eval("BookingDate", "{0:dd/MM HH:mm}") %></span>
                                            <span>🎯 <%# Eval("ScamConcerns") ?? "General" %></span>
                                        </div>
                                    </div>
                                    <div class="participant-actions">
                                        <button type="button" class="btn-connect" 
                                                onclick="connectToParticipant('<%# Eval("CustomerPhone") %>', '<%# Eval("CustomerName") %>')"
                                                id="btn_<%# Eval("CustomerPhone").ToString().Replace("+", "").Replace(" ", "") %>">
                                            📞 Connect
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Panel ID="pnlNoRegistered" runat="server" CssClass="no-participants" Visible="false">
                        <p><strong>No registered participants</strong></p>
                        <p>Participants will appear here once they register for this session.</p>
                    </asp:Panel>
                </div>
            </div>

            <!-- Online Participants -->
            <div class="participants-section">
                <div class="section-header">
                    <h3 class="section-title">🟢 Online & Available</h3>
                    <span class="participant-count" id="onlineCount">0</span>
                    <button type="button" class="refresh-btn" onclick="checkOnlineParticipants()">🔄</button>
                </div>

                <div id="onlineParticipantsList">
                    <!-- This will be populated dynamically via JavaScript -->
                    <div class="no-participants">
                        <p>Checking for online participants...</p>
                        <p><small>Participants who are currently waiting to join will appear here</small></p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Video Call Interface -->
        <div id="videoCallInterface" class="video-container">
            <h3 style="color: #051D40; margin-bottom: 20px;">📹 Active Video Session</h3>
            <div class="video-grid">
                <div class="video-wrapper">
                    <h4>Your Video (Expert)</h4>
                    <video id="localVideo" autoplay muted playsinline></video>
                </div>
                <div class="video-wrapper">
                    <h4 id="remoteVideoTitle">Participant Video</h4>
                    <video id="remoteVideo" autoplay playsinline></video>
                </div>
            </div>
            
            <!-- Multiple participant videos for group mode -->
            <div id="groupVideoGrid" style="display: none;">
                <h4>Group Session Participants</h4>
                <div id="groupVideos" class="video-grid">
                    <!-- Dynamically populated -->
                </div>
            </div>
        </div>

        <!-- Control Panel -->
        <div class="control-panel">
            <asp:Label ID="lblStatus" runat="server" CssClass="status-message info" 
                Text="Choose a session mode above to start connecting with participants" />
            
            <div style="margin-top: 20px; display: none;" id="activeControls">
                <button type="button" class="mode-btn secondary" onclick="switchToNextParticipant()" 
                        id="nextParticipantBtn">
                    ⏭️ Next Participant
                </button>
                <button type="button" class="mode-btn" onclick="endCurrentSession()" 
                        id="endSessionBtn">
                    📞 End Session
                </button>
            </div>
        </div>
    </div>

<script type="text/javascript">
    // ENHANCED MULTI-PARTICIPANT VIDEO CALL SYSTEM - COMPLETE CORRECTED VERSION
    // Complete JavaScript for StaffVideoCall.aspx with duplicate prevention

    let peer;
    let localStream;
    let connections = new Map(); // Store multiple participant connections
    let activeParticipants = new Map(); // Track active participants
    let sessionId = null;
    let sessionMode = 'none';
    let isConnectionActive = false;
    let onlineParticipants = [];
    let sessionStartTime = null;
    let selectedParticipantIds = '';
    let currentParticipantIndex = 0;
    let participantsList = [];

    // Real-time participant status management
    let participantStatuses = {};
    let updateInterval = null;

    // IMPROVED: Multi-participant video container
    function createMultiParticipantInterface() {
        const videoContainer = document.getElementById('videoCallInterface');
        if (!videoContainer) return;

        videoContainer.innerHTML = `
        <h3 style="color: #051D40; margin-bottom: 20px;">
            📹 Active Multi-Participant Session (${activeParticipants.size} connected)
        </h3>
        
        <!-- Expert Video (Always Visible) -->
        <div class="expert-video-section">
            <div class="video-wrapper expert-video">
                <h4>🎯 You (Expert)</h4>
                <video id="localVideo" autoplay="true" muted="true" playsinline="true"></video>
                <div class="video-controls">
                    <button onclick="toggleMute()" id="muteBtn">🎤 Mute</button>
                    <button onclick="toggleVideo()" id="videoBtn">📹 Video</button>
                </div>
            </div>
        </div>
        
        <!-- Participants Grid -->
        <div class="participants-video-grid" id="participantsGrid">
            <div class="no-participants-connected">
                <p>No participants connected yet</p>
                <p>Click "Connect to All" or individual "Connect" buttons to start</p>
            </div>
        </div>
        
        <!-- Multi-Participant Controls -->
        <div class="multi-controls">
            <button onclick="connectToAllParticipants()" class="btn-connect-all">
                📢 Connect to All Participants
            </button>
            <button onclick="disconnectAll()" class="btn-disconnect-all">
                🛑 Disconnect All
            </button>
            <button onclick="toggleLayout()" class="btn-layout">
                🔄 Switch Layout
            </button>
            <button onclick="cleanupDuplicateParticipants()" class="btn-cleanup">
                🧹 Clean Duplicates
            </button>
        </div>
    `;
    }

    // IMPROVED: Initialize expert system for multi-participant
    async function setupMultiParticipantExpert() {
        try {
            // Get media stream
            localStream = await navigator.mediaDevices.getUserMedia({
                video: { width: 640, height: 480 },
                audio: true
            });

            const localVideo = document.getElementById('localVideo');
            if (localVideo) {
                localVideo.srcObject = localStream;
            }

            // Create expert peer
            const expertId = `expert_session_${sessionId}`;
            peer = new Peer(expertId, {
                host: '0.peerjs.com',
                port: 443,
                path: '/',
                secure: true,
                debug: 2
            });

            peer.on('open', (id) => {
                console.log('✅ Expert peer ready for multi-participant:', id);
                isConnectionActive = true;
                updateStatus('Expert system ready for multi-participant video!', 'success');
            });

            // Handle incoming calls from participants
            peer.on('call', (call) => {
                console.log('📞 Incoming call from:', call.peer);
                handleIncomingCall(call);
            });

            peer.on('error', (err) => {
                console.error('❌ Peer error:', err);
                updateStatus('Connection error: ' + err.message, 'error');
            });

            peer.on('disconnected', () => {
                console.warn('⚠️ Peer disconnected, attempting reconnection...');
                isConnectionActive = false;
                updateStatus('Connection lost. Attempting to reconnect...', 'info');

                setTimeout(() => {
                    if (peer && !peer.destroyed) {
                        peer.reconnect();
                    }
                }, 2000);
            });

        } catch (err) {
            console.error('Media error:', err);
            updateStatus('Camera/microphone access required. Please allow permissions.', 'error');
        }
    }

    // FIXED: Handle incoming participant calls with duplicate prevention
    function handleIncomingCall(call) {
        // Extract participant info
        const participantId = call.peer.replace('customer_', '');

        console.log('📞 Incoming call from participant ID:', participantId);

        // CHECK: Prevent duplicate connections
        if (connections.has(participantId)) {
            console.log('⚠️ Participant already connected, closing duplicate:', participantId);
            call.close();
            return;
        }

        // CHECK: Prevent duplicate video elements
        const existingElement = document.getElementById(`participant_${participantId}`);
        if (existingElement) {
            console.log('⚠️ Removing existing video element for:', participantId);
            existingElement.remove();
            activeParticipants.delete(participantId);
        }

        // Answer the call with local stream
        call.answer(localStream);

        call.on('stream', (remoteStream) => {
            console.log('✅ Received stream from participant:', participantId);
            addParticipantToGrid(participantId, remoteStream, call);
        });

        call.on('close', () => {
            console.log('📞 Call ended with participant:', participantId);
            removeParticipantFromGrid(participantId);
        });

        call.on('error', (err) => {
            console.error('📞 Call error with participant:', participantId, err);
            removeParticipantFromGrid(participantId);
        });

        // Store the connection
        connections.set(participantId, call);
    }

    // FIXED: Add participant video to grid with duplicate prevention
    function addParticipantToGrid(participantId, stream, call) {
        const grid = document.getElementById('participantsGrid');
        if (!grid) return;

        // PREVENT DUPLICATES: Check if participant already exists
        const existingParticipant = document.getElementById(`participant_${participantId}`);
        if (existingParticipant) {
            console.log('⚠️ Participant already in grid, updating stream:', participantId);

            // Update existing video stream instead of creating new element
            const existingVideo = document.getElementById(`video_${participantId}`);
            if (existingVideo) {
                existingVideo.srcObject = stream;
            }

            // Update the stored call reference
            const existingParticipantData = activeParticipants.get(participantId);
            if (existingParticipantData) {
                existingParticipantData.call = call;
                existingParticipantData.stream = stream;
            }

            return; // Don't create duplicate
        }

        // Remove "no participants" message
        const noParticipants = grid.querySelector('.no-participants-connected');
        if (noParticipants) {
            noParticipants.remove();
        }

        // Create participant video element
        const participantDiv = document.createElement('div');
        participantDiv.className = 'participant-video-wrapper';
        participantDiv.id = `participant_${participantId}`;

        // Get participant name from the UI
        const participantName = getParticipantName(participantId) || `Participant ${participantId}`;

        // FIXED: HTML5 compliant video attributes
        participantDiv.innerHTML = `
        <div class="participant-video-header">
            <h4>👤 ${participantName}</h4>
            <div class="participant-actions">
                <button onclick="muteParticipant('${participantId}')" class="btn-mute-participant">
                    🔇 Mute
                </button>
                <button onclick="disconnectParticipant('${participantId}')" class="btn-disconnect-participant">
                    ❌ Disconnect
                </button>
            </div>
        </div>
        <video id="video_${participantId}" autoplay="true" playsinline="true"></video>
        <div class="participant-status">
            <span class="status-indicator online"></span>
            Connected
        </div>
    `;

        grid.appendChild(participantDiv);

        // Set video stream
        const video = document.getElementById(`video_${participantId}`);
        if (video) {
            video.srcObject = stream;
        }

        // Update active participants
        activeParticipants.set(participantId, {
            name: participantName,
            call: call,
            stream: stream,
            muted: false
        });

        // Update UI
        updateParticipantCount();
        updateGridLayout();

        console.log('✅ Added participant to grid:', participantName, participantId);
    }

    // IMPROVED: Remove participant from grid
    function removeParticipantFromGrid(participantId) {
        const participantElement = document.getElementById(`participant_${participantId}`);
        if (participantElement) {
            participantElement.remove();
        }

        // Remove from active participants
        activeParticipants.delete(participantId);
        connections.delete(participantId);

        // Update UI
        updateParticipantCount();
        updateGridLayout();

        // Show "no participants" message if no one is connected
        const grid = document.getElementById('participantsGrid');
        if (grid && activeParticipants.size === 0) {
            grid.innerHTML = `
                    <div class="no-participants-connected">
                        <p>No participants connected</p>
                        <p>Click "Connect to All" to start group session</p>
                    </div>
                `;
        }
    }

    // FIXED: Connect to all participants with duplicate prevention
    async function connectToAllParticipants() {
        if (!peer || !localStream) {
            updateStatus('Expert system not ready. Please wait...', 'error');
            return;
        }

        updateStatus('🔄 Connecting to all participants...', 'info');

        // Get all participant phone numbers from the UI
        const participantCards = document.querySelectorAll('.participant-card');
        let connectedCount = 0;
        let totalParticipants = participantCards.length;

        if (totalParticipants === 0) {
            updateStatus('❌ No registered participants found to connect to.', 'error');
            return;
        }

        // PREVENT DUPLICATES: Track processed participants
        const processedParticipants = new Set();

        for (const card of participantCards) {
            const phone = card.getAttribute('data-phone');
            const name = card.getAttribute('data-name');

            if (phone && name) {
                const cleanPhone = phone.replace(/[^0-9]/g, '');

                // PREVENT DUPLICATES: Skip if already processed
                if (processedParticipants.has(cleanPhone)) {
                    console.log('⚠️ Skipping duplicate participant:', name, cleanPhone);
                    continue;
                }
                processedParticipants.add(cleanPhone);

                const participantId = `customer_${cleanPhone}`;

                // Skip if already connected
                if (connections.has(cleanPhone)) {
                    console.log('Already connected to:', name);
                    connectedCount++; // Count existing connections
                    continue;
                }

                try {
                    console.log('📞 Calling participant:', name, participantId);

                    // Call participant
                    const call = peer.call(participantId, localStream);

                    if (call) {
                        // Set up call handlers
                        call.on('stream', (remoteStream) => {
                            console.log('✅ Connected to:', name);
                            addParticipantToGrid(cleanPhone, remoteStream, call);
                            connectedCount++;
                            updateStatus(`Connected ${connectedCount}/${totalParticipants} participants`, 'info');
                        });

                        call.on('error', (err) => {
                            console.warn('Failed to connect to:', name, err);
                        });

                        call.on('close', () => {
                            removeParticipantFromGrid(cleanPhone);
                        });

                        // Store connection immediately
                        connections.set(cleanPhone, call);
                    }

                    // Small delay between calls
                    await new Promise(resolve => setTimeout(resolve, 1000));

                } catch (error) {
                    console.error('Error calling participant:', name, error);
                }
            }
        }

        setTimeout(() => {
            if (connectedCount > 0) {
                updateStatus(`✅ Multi-participant session active! ${connectedCount} participants connected.`, 'success');
            } else {
                updateStatus('❌ No participants could be reached. Make sure they are online and have joined the session.', 'error');
            }
        }, 5000);
    }

    // IMPROVED: Disconnect all participants
    function disconnectAll() {
        console.log('🛑 Disconnecting all participants');

        connections.forEach((call, participantId) => {
            try {
                call.close();
            } catch (error) {
                console.warn('Error closing call:', error);
            }
        });

        connections.clear();
        activeParticipants.clear();

        // Clear the grid
        const grid = document.getElementById('participantsGrid');
        if (grid) {
            grid.innerHTML = `
                    <div class="no-participants-connected">
                        <p>All participants disconnected</p>
                        <p>Click "Connect to All" to start new session</p>
                    </div>
                `;
        }

        updateParticipantCount();
        updateStatus('All participants disconnected', 'info');
    }

    // IMPROVED: Update participant count display
    function updateParticipantCount() {
        const connectedCount = activeParticipants.size;

        // Update session title
        const sessionTitle = document.querySelector('#videoCallInterface h3');
        if (sessionTitle) {
            sessionTitle.textContent = `📹 Active Multi-Participant Session (${connectedCount} connected)`;
        }

        // Update dashboard counters
        const connectedElements = document.querySelectorAll('#connectedParticipants');
        connectedElements.forEach(el => {
            el.textContent = connectedCount;
        });

        console.log('📊 Updated participant count:', connectedCount);
    }

    // IMPROVED: Dynamic grid layout based on participant count
    function updateGridLayout() {
        const grid = document.getElementById('participantsGrid');
        if (!grid) return;

        const participantCount = activeParticipants.size;

        // Adjust grid layout based on number of participants
        if (participantCount <= 2) {
            grid.style.gridTemplateColumns = 'repeat(2, 1fr)';
        } else if (participantCount <= 4) {
            grid.style.gridTemplateColumns = 'repeat(2, 1fr)';
        } else if (participantCount <= 6) {
            grid.style.gridTemplateColumns = 'repeat(3, 1fr)';
        } else {
            grid.style.gridTemplateColumns = 'repeat(4, 1fr)';
        }
    }

    // FIXED: Clean up function to remove all duplicates
    function cleanupDuplicateParticipants() {
        console.log('🧹 Cleaning up duplicate participants...');

        const grid = document.getElementById('participantsGrid');
        if (!grid) return;

        const participantElements = grid.querySelectorAll('.participant-video-wrapper');
        const seenParticipants = new Set();

        participantElements.forEach(element => {
            const participantId = element.id.replace('participant_', '');

            if (seenParticipants.has(participantId)) {
                console.log('🗑️ Removing duplicate element for:', participantId);
                element.remove();
            } else {
                seenParticipants.add(participantId);
            }
        });

        // Clean up activeParticipants map
        const activeKeys = Array.from(activeParticipants.keys());
        const uniqueKeys = [...new Set(activeKeys)];

        if (activeKeys.length !== uniqueKeys.length) {
            console.log('🧹 Cleaning up activeParticipants map');
            const newActiveParticipants = new Map();
            uniqueKeys.forEach(key => {
                newActiveParticipants.set(key, activeParticipants.get(key));
            });
            activeParticipants = newActiveParticipants;
        }

        updateParticipantCount();
        updateGridLayout();
        updateStatus('🧹 Duplicates cleaned up!', 'success');
    }

    // UTILITY FUNCTIONS

    function getParticipantName(participantId) {
        const cards = document.querySelectorAll('.participant-card');
        for (const card of cards) {
            const phone = card.getAttribute('data-phone');
            if (phone && phone.replace(/[^0-9]/g, '') === participantId) {
                return card.getAttribute('data-name');
            }
        }
        return null;
    }

    function muteParticipant(participantId) {
        const participant = activeParticipants.get(participantId);
        if (participant && participant.stream) {
            const audioTracks = participant.stream.getAudioTracks();
            audioTracks.forEach(track => {
                track.enabled = !track.enabled;
            });
            participant.muted = !participant.muted;

            const muteBtn = document.querySelector(`#participant_${participantId} .btn-mute-participant`);
            if (muteBtn) {
                muteBtn.textContent = participant.muted ? '🔊 Unmute' : '🔇 Mute';
            }
        }
    }

    function disconnectParticipant(participantId) {
        const call = connections.get(participantId);
        if (call) {
            call.close();
        }
        removeParticipantFromGrid(participantId);
    }

    function toggleMute() {
        if (localStream) {
            const audioTracks = localStream.getAudioTracks();
            audioTracks.forEach(track => {
                track.enabled = !track.enabled;
            });

            const muteBtn = document.getElementById('muteBtn');
            if (muteBtn) {
                muteBtn.textContent = audioTracks[0]?.enabled ? '🎤 Mute' : '🔊 Unmute';
            }
        }
    }

    function toggleVideo() {
        if (localStream) {
            const videoTracks = localStream.getVideoTracks();
            videoTracks.forEach(track => {
                track.enabled = !track.enabled;
            });

            const videoBtn = document.getElementById('videoBtn');
            if (videoBtn) {
                videoBtn.textContent = videoTracks[0]?.enabled ? '📹 Video Off' : '📹 Video On';
            }
        }
    }

    function toggleLayout() {
        const grid = document.getElementById('participantsGrid');
        if (!grid) return;

        // Cycle through different layouts
        const currentCols = grid.style.gridTemplateColumns;

        if (currentCols.includes('repeat(2,') || currentCols.includes('repeat(2 ')) {
            grid.style.gridTemplateColumns = 'repeat(3, 1fr)';
        } else if (currentCols.includes('repeat(3,')) {
            grid.style.gridTemplateColumns = 'repeat(4, 1fr)';
        } else {
            grid.style.gridTemplateColumns = 'repeat(2, 1fr)';
        }
    }

    // ENHANCED MODE FUNCTIONS

    function startBroadcastMode() {
        if (!isConnectionActive) {
            updateStatus('Expert system not ready. Please wait...', 'error');
            return;
        }

        sessionMode = 'broadcast';
        updateStatus('📢 Broadcast Mode: Connecting to all participants...', 'info');

        // Clean up any existing duplicates first
        cleanupDuplicateParticipants();

        createMultiParticipantInterface();
        setTimeout(() => {
            connectToAllParticipants();
        }, 1000);
    }

    function startGroupMode() {
        if (!isConnectionActive) {
            updateStatus('Expert system not ready. Please wait...', 'error');
            return;
        }

        sessionMode = 'group';
        updateStatus('👥 Group Mode: Multi-participant video enabled', 'info');

        // Clean up any existing duplicates first
        cleanupDuplicateParticipants();

        createMultiParticipantInterface();
    }

    function startOneOnOneMode() {
        if (!isConnectionActive) {
            updateStatus('Expert system not ready. Please wait...', 'error');
            return;
        }

        sessionMode = 'oneOnOne';
        updateStatus('📞 One-on-One mode: Click on any participant to start individual consultation.', 'info');

        // Clean up any existing duplicates first
        cleanupDuplicateParticipants();

        createMultiParticipantInterface();
    }

    // FIXED: Connect to individual participant with duplicate prevention
    function connectToParticipant(phone, name) {
        if (!isConnectionActive) {
            updateStatus('Expert system not ready. Please wait...', 'error');
            return;
        }

        console.log('🔄 Attempting to connect to participant:', name, phone);

        const cleanPhone = phone.replace(/[^0-9]/g, '');
        const participantId = `customer_${cleanPhone}`;

        // PREVENT DUPLICATES: Check if already connected
        if (connections.has(cleanPhone)) {
            updateStatus(`Already connected to ${name}`, 'info');
            return;
        }

        // PREVENT DUPLICATES: Check if video element already exists
        const existingElement = document.getElementById(`participant_${cleanPhone}`);
        if (existingElement) {
            console.log('⚠️ Removing existing duplicate element for:', name);
            existingElement.remove();
            activeParticipants.delete(cleanPhone);
        }

        updateStatus(`Connecting to ${name} (${phone})...`, 'info');

        // Call participant
        const call = peer.call(participantId, localStream);

        if (call) {
            // Set up call handlers
            call.on('stream', (remoteStream) => {
                console.log('✅ Connected to:', name);
                addParticipantToGrid(cleanPhone, remoteStream, call);
                updateStatus(`Connected with ${name}! Video consultation in progress.`, 'success');
            });

            call.on('error', (err) => {
                console.warn('Failed to connect to:', name, err);
                updateStatus(`Failed to connect to ${name}. They may not be online.`, 'error');
            });

            call.on('close', () => {
                console.log('Call ended with:', name);
                removeParticipantFromGrid(cleanPhone);
            });

            // Store connection
            connections.set(cleanPhone, call);
        } else {
            updateStatus(`Failed to initiate call to ${name}`, 'error');
        }
    }

    // UTILITY FUNCTIONS FOR COMPATIBILITY

    function checkOnlineParticipants() {
        console.log('🔄 Checking online participants...');

        const totalRegisteredElement = document.getElementById('<%= lblTotalParticipants.ClientID %>');
        const totalRegistered = totalRegisteredElement ? totalRegisteredElement.textContent : '0';

        const totalParticipantsElement = document.getElementById('totalParticipants');
        if (totalParticipantsElement) {
            totalParticipantsElement.textContent = totalRegistered;
        }

        const participantCards = document.querySelectorAll('.participant-card');
        console.log('🔄 Found', participantCards.length, 'registered participants');

        participantCards.forEach((card, index) => {
            const phone = card.getAttribute('data-phone')?.replace(/[^0-9]/g, '') || '';
            const name = card.getAttribute('data-name') || '';
            const statusIndicator = card.querySelector('.status-indicator');

            if (phone && name && statusIndicator) {
                const status = 'waiting';
                participantStatuses[phone] = {
                    name: name,
                    status: status,
                    bookingStatus: 'Confirmed'
                };
                statusIndicator.className = `status-indicator status-${status}`;
            }
        });

        console.log('✅ Online participants check completed');
    }

    function updateStatus(message, type) {
        console.log(`📱 Status (${type}):`, message);

        const statusLabel = document.getElementById('<%= lblStatus.ClientID %>');
        if (statusLabel) {
            statusLabel.textContent = message;
            statusLabel.className = `status-message ${type}`;
        }
    }

    function updateParticipantCounts() {
        const totalRegisteredElement = document.getElementById('<%= lblTotalParticipants.ClientID %>');
            const registeredCountElement = document.getElementById('<%= lblRegisteredCount.ClientID %>');
            
            const totalRegistered = totalRegisteredElement ? totalRegisteredElement.textContent : '0';
            const registeredCount = registeredCountElement ? registeredCountElement.textContent : '0';
            
            const dashboardTotal = document.getElementById('totalParticipants');
            if (dashboardTotal) {
                dashboardTotal.textContent = totalRegistered;
            }
            
            const participantCards = document.querySelectorAll('.participant-card');
            if (participantCards.length > 0 && totalRegistered === '0') {
                const cardCount = participantCards.length.toString();
                if (dashboardTotal) dashboardTotal.textContent = cardCount;
                if (registeredCountElement) registeredCountElement.textContent = cardCount;
            }
        }

        async function notifyAllParticipants() {
            try {
                const totalParticipantsElement = document.getElementById('<%= lblTotalParticipants.ClientID %>');
                const totalParticipants = totalParticipantsElement ? parseInt(totalParticipantsElement.textContent || '0') : 0;
                
                if (totalParticipants === 0) {
                    updateStatus('No participants to notify.', 'error');
                    return;
                }
                
                updateStatus('📤 Notifying all participants...', 'info');
                
                const participantCards = document.querySelectorAll('.participant-card');
                const participantIds = [];
                
                participantCards.forEach(card => {
                    const phone = card.getAttribute('data-phone');
                    if (phone) {
                        participantIds.push(phone.replace(/[^0-9]/g, ''));
                    }
                });
                
                if (participantIds.length > 0) {
                    const response = await fetch('<%= Page.ResolveUrl("~/Staff/StaffVideoCall.aspx/NotifyParticipants") %>', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify({
                            sessionId: parseInt(sessionId),
                            participantIds: participantIds.join(','),
                            message: 'Expert session starting soon!'
                        })
                    });

                    const data = await response.json();
                    const result = JSON.parse(data.d);

                    if (result.success) {
                        updateStatus(`✅ ${result.notifiedCount} participants notified successfully!`, 'success');

                        const bulkNotification = document.getElementById('bulkNotification');
                        if (bulkNotification) {
                            bulkNotification.classList.remove('show');
                        }
                    } else {
                        updateStatus('❌ Error notifying participants: ' + result.message, 'error');
                    }
                } else {
                    updateStatus('No valid participant IDs found.', 'error');
                }
            } catch (error) {
                console.error('Error notifying participants:', error);
                updateStatus('Error sending notifications. Please try again.', 'error');
            }
        }

        function filterParticipants(searchTerm) {
            const cards = document.querySelectorAll('.participant-card');
            const term = searchTerm.toLowerCase();

            cards.forEach(card => {
                const name = (card.getAttribute('data-name') || '').toLowerCase();
                const phone = (card.getAttribute('data-phone') || '').toLowerCase();

                if (name.includes(term) || phone.includes(term)) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        }

        // UPDATED INITIALIZATION: Clean up duplicates and prevent issues
        window.onload = function() {
            const hdnSessionIdElement = document.getElementById('<%= hdnSessionId.ClientID %>');
            sessionId = hdnSessionIdElement ? hdnSessionIdElement.value : null;

            if (sessionId) {
                console.log('🚀 Initializing multi-participant video system for session:', sessionId);
                
                // Clear any existing connections first
                connections.clear();
                activeParticipants.clear();
                
                // Create the multi-participant interface immediately
                createMultiParticipantInterface();
                
                // Set up the expert peer system
                setupMultiParticipantExpert();
                
                // Clean up any duplicates after a short delay
                setTimeout(() => {
                    cleanupDuplicateParticipants();
                }, 2000);
                
                // Initialize other systems
                checkOnlineParticipants();
                updateParticipantCounts();

                // Show bulk notification if there are participants
                const totalParticipantsElement = document.getElementById('<%= lblTotalParticipants.ClientID %>');
            const totalParticipants = totalParticipantsElement ? parseInt(totalParticipantsElement.textContent || '0') : 0;

            if (totalParticipants > 0) {
                const bulkNotification = document.getElementById('bulkNotification');
                if (bulkNotification) {
                    bulkNotification.classList.add('show');
                }
            }

            // Start session timer
            sessionStartTime = new Date();
            setInterval(() => {
                const now = new Date();
                const diff = now - sessionStartTime;
                const minutes = Math.floor(diff / 60000);
                const seconds = Math.floor((diff % 60000) / 1000);

                const durationElement = document.getElementById('sessionDuration');
                if (durationElement) {
                    durationElement.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;
                }
            }, 1000);

            // Periodic cleanup of duplicates every 30 seconds
            setInterval(() => {
                cleanupDuplicateParticipants();
            }, 30000);

        } else {
            console.error('❌ No session ID found');
            updateStatus('Session ID not found. Please return to session management.', 'error');
        }
    };

    // Cleanup on page unload
    window.onbeforeunload = function () {
        console.log('🧹 Cleaning up resources...');

        if (localStream) {
            localStream.getTracks().forEach(track => track.stop());
        }

        disconnectAll();

        if (peer && isConnectionActive) {
            peer.destroy();
        }
    };
</script>
</asp:Content>