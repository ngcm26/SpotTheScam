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
        let peer;
        let currentCall;
        let localStream;
        let isConnectionActive = false;
        let sessionId = null;
        let currentParticipant = null;
        let onlineParticipants = [];
        let sessionStartTime = null;
        let sessionMode = 'none'; // 'broadcast', 'group', 'oneOnOne', 'none'
        let selectedParticipantIds = '';
        let groupConnections = {};
        let currentParticipantIndex = 0;
        let participantsList = [];

        // FIXED: Real-time participant status management
        let participantStatuses = {};
        let updateInterval = null;

        // Initialize participant statuses from server data
        function initializeParticipantStatuses(serverData) {
            console.log('🔄 Initializing participant statuses:', serverData);
            
            serverData.forEach(participant => {
                participantStatuses[participant.phone] = {
                    name: participant.name,
                    status: participant.status,
                    bookingStatus: participant.bookingStatus
                };
                
                // Update UI immediately
                updateParticipantStatusUI(participant.phone, participant.status);
            });
            
            updateOnlineParticipantsList();
        }

        // Update participant status in UI
        function updateParticipantStatusUI(phone, status) {
            const statusIndicator = document.getElementById(`status_${phone}`);
            if (statusIndicator) {
                statusIndicator.className = `status-indicator status-${status}`;
            }
            
            // Update button if participant is online
            const btn = document.getElementById(`btn_${phone}`);
            if (btn && status === 'online') {
                btn.textContent = '🟢 Connect Now';
                btn.disabled = false;
            } else if (btn && status === 'connected') {
                btn.textContent = '📞 In Call';
                btn.disabled = true;
            } else if (btn) {
                btn.textContent = '📞 Connect';
                btn.disabled = false;
            }
        }

        // Update online participants list dynamically
        function updateOnlineParticipantsList() {
            const onlineList = document.getElementById('onlineParticipantsList');
            if (!onlineList) return;
            
            let onlineCount = 0;
            let onlineHTML = '';
            
            for (const [phone, participant] of Object.entries(participantStatuses)) {
                if (participant.status === 'online' || participant.status === 'waiting') {
                    onlineCount++;
                    onlineHTML += `
                        <div class="participant-card online">
                            <div class="participant-info">
                                <div class="participant-details">
                                    <h4>
                                        <span class="status-indicator status-${participant.status}"></span>
                                        ${participant.name}
                                    </h4>
                                    <div class="participant-meta">
                                        <span>📱 +${phone}</span>
                                        <span>🟢 Available Now</span>
                                    </div>
                                </div>
                                <div class="participant-actions">
                                    <button type="button" class="btn-connect" onclick="connectToParticipant('+${phone}', '${participant.name}')">
                                        📞 Connect Now
                                    </button>
                                </div>
                            </div>
                        </div>
                    `;
                }
            }
            
            if (onlineCount === 0) {
                onlineHTML = `
                    <div class="no-participants">
                        <p>No participants currently online</p>
                        <p><small>Participants will appear here when they join the session</small></p>
                    </div>
                `;
            }
            
            onlineList.innerHTML = onlineHTML;
            
            // Update counters
            document.getElementById('onlineParticipants').textContent = onlineCount;
            document.getElementById('onlineCount').textContent = onlineCount;
            
            console.log('🔄 Updated online participants list:', onlineCount);
        }

        // Start real-time updates
        function startRealTimeUpdates() {
            if (updateInterval) {
                clearInterval(updateInterval);
            }
            
            updateInterval = setInterval(() => {
                if (sessionId && sessionMode !== 'none') {
                    fetchParticipantUpdates();
                }
            }, 10000); // Check every 10 seconds
            
            console.log('🔄 Started real-time participant updates');
        }

        // Fetch participant updates from server
        async function fetchParticipantUpdates() {
            try {
                const response = await fetch('StaffVideoCall.aspx/GetParticipantUpdates', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ sessionId: parseInt(sessionId) })
                });
                
                const data = await response.json();
                const result = JSON.parse(data.d);
                
                if (result.success) {
                    console.log('📡 Received participant updates:', result.participants);
                    
                    // Update participant statuses
                    let hasChanges = false;
                    
                    result.participants.forEach(participant => {
                        const oldStatus = participantStatuses[participant.phone]?.status;
                        
                        if (oldStatus !== participant.status) {
                            hasChanges = true;
                            console.log(`🔄 Status change: ${participant.name} ${oldStatus} → ${participant.status}`);
                        }
                        
                        participantStatuses[participant.phone] = {
                            name: participant.name,
                            status: participant.status,
                            bookingStatus: participant.bookingStatus
                        };
                        
                        updateParticipantStatusUI(participant.phone, participant.status);
                    });
                    
                    if (hasChanges) {
                        updateOnlineParticipantsList();
                        updateConnectedCount();
                    }
                }
            } catch (error) {
                console.error('❌ Error fetching participant updates:', error);
            }
        }

        // Initialize when page loads
        window.onload = function () {
            sessionId = document.getElementById('<%= hdnSessionId.ClientID %>').value;
            
            if (sessionId) {
                initializeExpertSystem();
                startParticipantMonitoring();
                startSessionTimer();
                
                // Check URL parameters for mode
                const urlParams = new URLSearchParams(window.location.search);
                const urlMode = urlParams.get('mode');
                const urlParticipants = urlParams.get('participants');
                
                if (urlMode) {
                    sessionMode = urlMode;
                    selectedParticipantIds = urlParticipants || '';
                    handleModeFromURL(urlMode);
                }
                
                // Test participant count after initialization
                setTimeout(() => {
                    testParticipantCount();
                }, 2000);
            }
        };

        function handleModeFromURL(mode) {
            updateStatus('Initializing ' + mode + ' mode...', 'info');
            
            switch(mode) {
                case 'broadcast':
                    setTimeout(() => startBroadcastMode(), 1000);
                    break;
                case 'group':
                    if (selectedParticipantIds) {
                        setTimeout(() => startGroupModeWithParticipants(selectedParticipantIds), 1000);
                    } else {
                        setTimeout(() => startGroupMode(), 1000);
                    }
                    break;
                case 'oneOnOne':
                    setTimeout(() => startOneOnOneMode(), 1000);
                    break;
            }
        }

        function initializeExpertSystem() {
            updateStatus('Initializing expert video system...', 'info');
            
            // Setup peer connection for expert
            setupExpertPeer();
            
            // Start checking for online participants
            checkOnlineParticipants();
            
            // Update participant counts
            updateParticipantCounts();
            
            // Show video interface immediately so staff can see their own video
            document.getElementById('videoCallInterface').style.display = 'block';
            
            // Show bulk notification if there are participants
            const totalParticipants = parseInt(document.getElementById('<%= lblTotalParticipants.ClientID %>').textContent || '0');
            if (totalParticipants > 0) {
                document.getElementById('bulkNotification').classList.add('show');
            }
        }

        async function setupExpertPeer() {
            try {
                // Get media stream first
                localStream = await navigator.mediaDevices.getUserMedia({
                    video: true,
                    audio: true
                });

                document.getElementById('localVideo').srcObject = localStream;

                // Create expert peer
                const expertId = `expert_session_${sessionId}`;
                peer = new Peer(expertId, {
                    host: 'localhost',
                    port: 3001,
                    path: '/',
                    debug: 2
                });

                peer.on('open', (id) => {
                    console.log('Expert peer connected:', id);
                    isConnectionActive = true;
                    updateStatus('Expert system ready. Choose a session mode to begin.', 'success');
                });

                peer.on('call', (call) => {
                    console.log('Incoming call from participant');
                    call.answer(localStream);
                    setupCall(call);
                });

                peer.on('error', (err) => {
                    console.error('Peer error:', err);
                    updateStatus('Connection error: ' + err.message, 'error');
                });

            } catch (err) {
                console.error('Media error:', err);
                updateStatus('Camera/microphone access required. Please allow permissions.', 'error');
            }
        }

        // === MODE SELECTION FUNCTIONS ===
        
        function startBroadcastMode() {
            if (!isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }
            
            sessionMode = 'broadcast';
            updateStatus('🎥 Starting broadcast mode - connecting to all participants...', 'info');
            
            // Get all registered participants
            const participantCards = document.querySelectorAll('.participant-card');
            participantsList = [];
            
            participantCards.forEach(card => {
                const phone = card.getAttribute('data-phone');
                const name = card.getAttribute('data-name');
                if (phone && name) {
                    participantsList.push({phone: phone, name: name});
                }
            });
            
            if (participantsList.length === 0) {
                updateStatus('No participants found to connect to.', 'error');
                return;
            }
            
            // Show video interface
            document.getElementById('videoCallInterface').style.display = 'block';
            document.getElementById('activeControls').style.display = 'block';
            
            // Start connecting to participants one by one
            connectToBroadcastParticipants();
        }

        function startGroupMode() {
            if (!isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }
            
            sessionMode = 'group';
            updateStatus('👥 Group mode: Please select participants from the list by clicking on them.', 'info');
            
            // Enable participant selection
            enableParticipantSelection();
        }

        function startGroupModeWithParticipants(participantIds) {
            if (!isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }
            
            sessionMode = 'group';
            updateStatus('👥 Starting group mode with selected participants...', 'info');
            
            // Load participant data and start group call
            loadSelectedParticipantsAndConnect(participantIds);
        }

        function startOneOnOneMode() {
            if (!isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }
            
            sessionMode = 'oneOnOne';
            updateStatus('📞 One-on-One mode: Click on any participant to start individual consultation.', 'info');
            
            // Enable individual participant connection
            enableIndividualConnections();
        }

        // === CONNECTION FUNCTIONS ===

        function connectToParticipant(phone, name) {
            if (!isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }

            // Update participant status to connecting
            updateParticipantStatusOnServer(sessionId, phone.replace(/[^0-9]/g, ''), 'connecting');
            
            // Handle based on current mode
            if (sessionMode === 'oneOnOne') {
                connectOneOnOne(phone, name);
            } else if (sessionMode === 'group') {
                addToGroupCall(phone, name);
            } else {
                // Auto-select one-on-one mode
                sessionMode = 'oneOnOne';
                updateStatus('🔄 Starting one-on-one consultation mode...', 'info');
                connectOneOnOne(phone, name);
            }
        }

        function connectOneOnOne(phone, name) {
            // End current call if exists
            if (currentCall) {
                endCurrentCall();
            }

            currentParticipant = { phone: phone, name: name };
            
            // Update UI
            updateParticipantStatus(phone, 'connecting');
            updateStatus(`Connecting to ${name} (${phone})...`, 'info');

            // Create participant peer ID
            const participantId = `customer_${phone.replace(/[^0-9]/g, '')}`;
            
            // Call participant
            const call = peer.call(participantId, localStream);
            
            if (call) {
                setupCall(call);
                
                // Update button
                const btn = document.getElementById(`btn_${phone.replace(/[^0-9]/g, '')}`);
                if (btn) {
                    btn.textContent = '⏳ Connecting...';
                    btn.disabled = true;
                }
                
                // Show video interface
                document.getElementById('videoCallInterface').style.display = 'block';
                document.getElementById('activeControls').style.display = 'block';
            } else {
                updateStatus(`Failed to initiate call to ${name}`, 'error');
                updateParticipantStatus(phone, 'offline');
            }
        }

        function addToGroupCall(phone, name) {
            const participantId = `customer_${phone.replace(/[^0-9]/g, '')}`;
            
            // Check if already connected
            if (groupConnections[participantId]) {
                updateStatus(`${name} is already in the group call.`, 'info');
                return;
            }
            
            updateStatus(`Adding ${name} to group call...`, 'info');
            
            // Call participant
            const call = peer.call(participantId, localStream);
            
            if (call) {
                groupConnections[participantId] = {
                    call: call,
                    name: name,
                    phone: phone
                };
                
                setupGroupCall(call, name, participantId);
                
                // Show group video interface
                document.getElementById('videoCallInterface').style.display = 'block';
                document.getElementById('groupVideoGrid').style.display = 'block';
                document.getElementById('activeControls').style.display = 'block';
                
                updateParticipantStatus(phone, 'connected');
            }
        }

        function connectToBroadcastParticipants() {
            if (currentParticipantIndex >= participantsList.length) {
                updateStatus('📢 Broadcast mode: All participants contacted!', 'success');
                return;
            }
            
            const participant = participantsList[currentParticipantIndex];
            updateStatus(`📢 Broadcasting to ${participant.name} (${currentParticipantIndex + 1}/${participantsList.length})...`, 'info');
            
            const participantId = `customer_${participant.phone.replace(/[^0-9]/g, '')}`;
            const call = peer.call(participantId, localStream);
            
            if (call) {
                // Store in group connections for broadcast
                groupConnections[participantId] = {
                    call: call,
                    name: participant.name,
                    phone: participant.phone
                };
                
                setupGroupCall(call, participant.name, participantId);
                updateParticipantStatus(participant.phone, 'connected');
            }
            
            currentParticipantIndex++;
            
            // Continue to next participant after a short delay
            setTimeout(() => connectToBroadcastParticipants(), 2000);
        }

        function setupCall(call) {
            currentCall = call;
            
            call.on('stream', (remoteStream) => {
                console.log('Received participant stream');
                document.getElementById('remoteVideo').srcObject = remoteStream;
                document.getElementById('remoteVideoTitle').textContent = 
                    `${currentParticipant.name} - ${currentParticipant.phone}`;
                
                updateStatus(`Connected with ${currentParticipant.name}! Consultation in progress.`, 'success');
                updateConnectedCount();
            });

            call.on('close', () => {
                console.log('Call ended');
                endCurrentCall();
            });

            call.on('error', (err) => {
                console.error('Call error:', err);
                updateStatus(`Call error with ${currentParticipant?.name || 'participant'}: ${err.message}`, 'error');
                endCurrentCall();
            });
        }

        function setupGroupCall(call, name, participantId) {
            call.on('stream', (remoteStream) => {
                console.log(`Received stream from ${name}`);
                
                // Create video element for this participant
                const videoWrapper = document.createElement('div');
                videoWrapper.className = 'video-wrapper';
                videoWrapper.id = `video_${participantId}`;
                
                const title = document.createElement('h4');
                title.textContent = name;
                
                const video = document.createElement('video');
                video.autoplay = true;
                video.playsInline = true;
                video.srcObject = remoteStream;
                
                videoWrapper.appendChild(title);
                videoWrapper.appendChild(video);
                
                document.getElementById('groupVideos').appendChild(videoWrapper);
                
                updateConnectedCount();
                updateStatus(`${name} joined the group call!`, 'success');
            });

            call.on('close', () => {
                console.log(`${name} left the call`);
                const videoElement = document.getElementById(`video_${participantId}`);
                if (videoElement) {
                    videoElement.remove();
                }
                delete groupConnections[participantId];
                updateConnectedCount();
            });

            call.on('error', (err) => {
                console.error(`Call error with ${name}:`, err);
                updateStatus(`Connection error with ${name}: ${err.message}`, 'error');
            });
        }

        // === UI HELPER FUNCTIONS ===

        function enableParticipantSelection() {
            const participantCards = document.querySelectorAll('.participant-card');
            participantCards.forEach(card => {
                card.style.cursor = 'pointer';
                card.style.border = '2px dashed #D36F2D';
                card.addEventListener('click', function() {
                    const phone = this.getAttribute('data-phone');
                    const name = this.getAttribute('data-name');
                    addToGroupCall(phone, name);
                    this.style.border = '2px solid #28a745';
                });
            });
            
            updateStatus('Click on participants to add them to the group call.', 'info');
        }

        function enableIndividualConnections() {
            updateStatus('Click "Connect" next to any participant for one-on-one consultation.', 'info');
        }

        function switchToNextParticipant() {
            if (sessionMode === 'oneOnOne') {
                // Find next participant
                const participantCards = document.querySelectorAll('.participant-card');
                let nextParticipant = null;
                let foundCurrent = false;

                for (let card of participantCards) {
                    const phone = card.getAttribute('data-phone');
                    const name = card.getAttribute('data-name');
                    
                    if (currentParticipant && phone === currentParticipant.phone) {
                        foundCurrent = true;
                        continue;
                    }
                    
                    if (foundCurrent || !currentParticipant) {
                        nextParticipant = { phone, name };
                        break;
                    }
                }

                if (nextParticipant) {
                    connectOneOnOne(nextParticipant.phone, nextParticipant.name);
                } else {
                    // Go back to first participant
                    if (participantCards.length > 0) {
                        const firstCard = participantCards[0];
                        const phone = firstCard.getAttribute('data-phone');
                        const name = firstCard.getAttribute('data-name');
                        connectOneOnOne(phone, name);
                    }
                }
            }
        }

        function endCurrentCall() {
            if (currentCall) {
                currentCall.close();
                currentCall = null;
            }

            if (currentParticipant) {
                updateParticipantStatus(currentParticipant.phone, 'offline');
                
                // Reset button
                const btn = document.getElementById(`btn_${currentParticipant.phone.replace(/[^0-9]/g, '')}`);
                if (btn) {
                    btn.textContent = '📞 Connect';
                    btn.disabled = false;
                }
            }

            document.getElementById('remoteVideo').srcObject = null;
            currentParticipant = null;
            updateConnectedCount();
        }

        function endCurrentSession() {
            // End all connections
            if (currentCall) {
                currentCall.close();
                currentCall = null;
            }
            
            // End all group connections
            Object.keys(groupConnections).forEach(participantId => {
                if (groupConnections[participantId].call) {
                    groupConnections[participantId].call.close();
                }
            });
            groupConnections = {};
            
            // Clear video interfaces
            document.getElementById('videoCallInterface').style.display = 'none';
            document.getElementById('groupVideoGrid').style.display = 'none';
            document.getElementById('activeControls').style.display = 'none';
            document.getElementById('groupVideos').innerHTML = '';
            
            // Reset participant statuses
            const participantCards = document.querySelectorAll('.participant-card');
            participantCards.forEach(card => {
                const phone = card.getAttribute('data-phone');
                updateParticipantStatus(phone, 'offline');
                card.style.cursor = 'default';
                card.style.border = '1px solid #e9ecef';
            });
            
            // Reset session mode
            sessionMode = 'none';
            currentParticipant = null;
            currentParticipantIndex = 0;
            
            updateStatus('Session ended. Choose a new mode to start again.', 'info');
            updateConnectedCount();
        }

        // Update participant status on server
        async function updateParticipantStatusOnServer(sessionId, phone, status) {
            try {
                const response = await fetch('StaffVideoCall.aspx/UpdateParticipantStatus', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ 
                        sessionId: parseInt(sessionId), 
                        phoneNumber: phone, 
                        newStatus: status 
                    })
                });
                
                const data = await response.json();
                const result = JSON.parse(data.d);
                
                if (result.success) {
                    console.log('✅ Updated participant status on server:', phone, status);
                } else {
                    console.warn('⚠️ Failed to update participant status:', result.message);
                }
            } catch (error) {
                console.error('❌ Error updating participant status:', error);
            }
        }

        // === NOTIFICATION FUNCTIONS ===

        async function notifyAllParticipants() {
            try {
                const totalParticipants = parseInt(document.getElementById('<%= lblTotalParticipants.ClientID %>').textContent || '0');
                
                if (totalParticipants === 0) {
                    updateStatus('No participants to notify.', 'error');
                    return;
                }
                
                updateStatus('📤 Notifying all participants...', 'info');
                
                // Get all participant IDs
                const participantCards = document.querySelectorAll('.participant-card');
                const participantIds = [];
                
                participantCards.forEach(card => {
                    const phone = card.getAttribute('data-phone');
                    if (phone) {
                        // Extract participant ID from phone or use phone as ID
                        participantIds.push(phone.replace(/[^0-9]/g, ''));
                    }
                });
                
                if (participantIds.length > 0) {
                    // Call server method to notify participants
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
                        document.getElementById('bulkNotification').classList.remove('show');
                        
                        // Update participant statuses to 'waiting'
                        participantIds.forEach(id => {
                            if (participantStatuses[id]) {
                                participantStatuses[id].status = 'waiting';
                                participantStatuses[id].bookingStatus = 'Expert Ready';
                                updateParticipantStatusUI(id, 'waiting');
                            }
                        });
                        
                        updateOnlineParticipantsList();
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

        async function loadSelectedParticipantsAndConnect(participantIds) {
            try {
                const response = await fetch('<%= Page.ResolveUrl("~/Staff/StaffVideoCall.aspx/GetBulkParticipantData") %>', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        sessionId: parseInt(sessionId),
                        participantIds: participantIds
                    })
                });
                
                const data = await response.json();
                const result = JSON.parse(data.d);
                
                if (result.success && result.participants.length > 0) {
                    updateStatus(`Loading ${result.count} selected participants...`, 'info');
                    
                    // Show video interface
                    document.getElementById('videoCallInterface').style.display = 'block';
                    document.getElementById('groupVideoGrid').style.display = 'block';
                    document.getElementById('activeControls').style.display = 'block';
                    
                    // Connect to each participant
                    result.participants.forEach((participant, index) => {
                        setTimeout(() => {
                            addToGroupCall(participant.phone, participant.name);
                        }, index * 1000); // Stagger connections
                    });
                } else {
                    updateStatus('No participants found with the selected IDs.', 'error');
                }
                
            } catch (error) {
                console.error('Error loading selected participants:', error);
                updateStatus('Error loading participants. Please try again.', 'error');
            }
        }

        // === UTILITY FUNCTIONS ===

        function checkOnlineParticipants() {
            // Get the actual participant count from the server-side label
            const totalRegistered = document.getElementById('<%= lblTotalParticipants.ClientID %>').textContent || '0';
            
            // Update the dashboard stats
            document.getElementById('totalParticipants').textContent = totalRegistered;
            
            // Get all participant cards and simulate online status
            const participantCards = document.querySelectorAll('.participant-card');
            
            console.log('🔄 Checking online status for', participantCards.length, 'participants');
            
            participantCards.forEach((card, index) => {
                const phone = card.getAttribute('data-phone')?.replace(/[^0-9]/g, '') || '';
                const name = card.getAttribute('data-name') || '';
                const statusIndicator = card.querySelector('.status-indicator');
                
                if (phone && name && statusIndicator) {
                    // Check if we have status for this participant
                    const currentStatus = participantStatuses[phone];
                    
                    if (currentStatus) {
                        statusIndicator.className = `status-indicator status-${currentStatus.status}`;
                    } else {
                        // Simulate some participants being online (for demo)
                        const isOnline = Math.random() > 0.6; // 40% chance of being online
                        const status = isOnline ? 'online' : 'waiting';
                        
                        participantStatuses[phone] = {
                            name: name,
                            status: status,
                            bookingStatus: 'Confirmed'
                        };
                        
                        statusIndicator.className = `status-indicator status-${status}`;
                    }
                }
            });
            
            updateOnlineParticipantsList();
            
            console.log('✅ Online participants check completed');
        }

        function updateParticipantCounts() {
            // Get counts from server-side controls
            const totalRegisteredElement = document.getElementById('<%= lblTotalParticipants.ClientID %>');
            const registeredCountElement = document.getElementById('<%= lblRegisteredCount.ClientID %>');
            
            const totalRegistered = totalRegisteredElement ? totalRegisteredElement.textContent : '0';
            const registeredCount = registeredCountElement ? registeredCountElement.textContent : '0';
            
            console.log('Debug: Retrieved counts:', {
                totalRegistered: totalRegistered,
                registeredCount: registeredCount
            });
            
            // Update dashboard elements
            const dashboardTotal = document.getElementById('totalParticipants');
            if (dashboardTotal) {
                dashboardTotal.textContent = totalRegistered;
            }
            
            // Also count participant cards directly and update if server count is 0
            const participantCards = document.querySelectorAll('.participant-card');
            console.log('Participant cards found:', participantCards.length);
            
            // Use the card count if it's more reliable
            if (participantCards.length > 0) {
                document.getElementById('totalParticipants').textContent = participantCards.length.toString();
                // Also update the registered count section
                document.getElementById('<%= lblRegisteredCount.ClientID %>').textContent = participantCards.length.toString();
                console.log('Updated counts to card count:', participantCards.length);
            }
            
            // Force update the stats display
            setTimeout(() => {
                const currentTotal = document.getElementById('totalParticipants').textContent || '0';
                console.log('Final total participants count:', currentTotal);
                
                // If still 0, try to get data from server
                if (currentTotal === '0' && sessionId) {
                    testParticipantCount();
                }
            }, 500);
        }

        function updateParticipantStatus(phone, status) {
            const cleanPhone = phone.replace(/[^0-9]/g, '');
            const statusIndicator = document.getElementById(`status_${cleanPhone}`);
            
            if (statusIndicator) {
                statusIndicator.className = `status-indicator status-${status}`;
            }
        }

        function updateStatus(message, type) {
            const statusLabel = document.getElementById('<%= lblStatus.ClientID %>');
            statusLabel.textContent = message;
            statusLabel.className = `status-message ${type}`;
        }

        function updateConnectedCount() {
            let connectedCount = 0;

            if (currentCall) connectedCount++;
            connectedCount += Object.keys(groupConnections).length;

            document.getElementById('connectedParticipants').textContent = connectedCount;
        }

        function filterParticipants(searchTerm) {
            const cards = document.querySelectorAll('.participant-card');
            const term = searchTerm.toLowerCase();

            cards.forEach(card => {
                const name = card.getAttribute('data-name').toLowerCase();
                const phone = card.getAttribute('data-phone').toLowerCase();

                if (name.includes(term) || phone.includes(term)) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        }

        function startParticipantMonitoring() {
            // Check for online participants every 30 seconds
            setInterval(() => {
                if (sessionMode === 'none') { // Only check when not in active session
                    checkOnlineParticipants();
                }
            }, 30000);
        }

        function startSessionTimer() {
            sessionStartTime = new Date();

            setInterval(() => {
                const now = new Date();
                const diff = now - sessionStartTime;
                const minutes = Math.floor(diff / 60000);
                const seconds = Math.floor((diff % 60000) / 1000);

                document.getElementById('sessionDuration').textContent =
                    `${minutes}:${seconds.toString().padStart(2, '0')}`;
            }, 1000);
        }

        // Add a test function to verify participant count
        async function testParticipantCount() {
            if (!sessionId) return;
            
            try {
                const response = await fetch('<%= Page.ResolveUrl("~/Staff/StaffVideoCall.aspx/TestParticipantCount") %>', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ sessionId: parseInt(sessionId) })
                });

                const data = await response.json();
                const result = JSON.parse(data.d);

                console.log('🧪 Test participant count result:', result);

                if (result.success && result.count > 0) {
                    document.getElementById('totalParticipants').textContent = result.count.toString();
                    updateStatus(`Found ${result.count} confirmed participants`, 'success');
                }
            } catch (error) {
                console.error('Error testing participant count:', error);
            }
        }

        // Cleanup on page unload - stop real-time updates
        window.onbeforeunload = function () {
            if (updateInterval) {
                clearInterval(updateInterval);
            }

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }
            endCurrentSession();
            if (peer && isConnectionActive) {
                peer.destroy();
            }
        };
    </script>
</asp:Content>