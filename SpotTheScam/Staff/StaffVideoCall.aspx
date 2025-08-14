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

        .video-container {
            background: white;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

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
            max-width: 500px;
            height: 350px;
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
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
            gap: 20px;
            margin-bottom: 20px;
            min-height: 200px;
        }

        /* FIXED: Consistent sizing for all participant videos */
        .participants-video-grid.single-participant {
            grid-template-columns: repeat(auto-fit, minmax(350px, 400px));
            justify-content: center;
        }

        .participant-video-wrapper {
            background: #f8f9fa;
            border: 2px solid #e9ecef;
            border-radius: 12px;
            padding: 15px;
            transition: all 0.3s ease;
            position: relative;
            max-width: 450px; /* FIXED: Prevent videos from being too wide */
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
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            max-width: 200px;
        }

        .participant-status {
            background: #28a745;
            color: white;
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 0.75rem;
            font-weight: 600;
        }

        /* FIXED: Consistent video height for all participants */
        .participant-video-wrapper video {
            width: 100%;
            height: 200px; /* FIXED: Consistent height matching your screenshot */
            border-radius: 8px;
            background: #000;
            object-fit: cover;
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

        .control-panel {
            background: white;
            border-radius: 15px;
            padding: 25px;
            text-align: center;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .control-buttons {
            display: flex;
            justify-content: center;
            gap: 15px;
            flex-wrap: wrap;
            margin: 20px 0;
        }

        .btn-control {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            box-shadow: 0 3px 10px rgba(211, 111, 45, 0.3);
        }

        .btn-control:hover {
            background: #b45a22;
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(211, 111, 45, 0.4);
        }

        .btn-control.secondary {
            background: #6c757d;
        }

        .btn-control.secondary:hover {
            background: #5a6268;
        }

        .btn-control.danger {
            background: #dc3545;
        }

        .btn-control.danger:hover {
            background: #c82333;
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

        .participants-section {
            background: white;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
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
            padding: 20px;
            margin-bottom: 15px;
            display: grid;
            grid-template-columns: auto 1fr auto;
            align-items: center;
            gap: 20px;
        }

        .participant-info h4 {
            color: #051D40;
            margin: 0 0 5px 0;
            font-size: 1.1rem;
        }

        .participant-details {
            color: #666;
            font-size: 0.9rem;
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
        }

        .participant-actions {
            display: flex;
            gap: 10px;
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

        @media (max-width: 768px) {
            .participants-video-grid {
                grid-template-columns: 1fr;
                gap: 15px;
            }
            
            .participant-video-wrapper video {
                height: 200px;
            }
            
            .expert-video video {
                max-width: 100%;
                height: 250px;
            }
            
            .control-buttons {
                flex-direction: column;
                align-items: center;
            }
            
            .btn-control {
                width: 100%;
                max-width: 300px;
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

        <!-- Video Call Interface -->
        <div id="videoCallInterface" class="video-container">
            <h3 style="color: #051D40; margin-bottom: 20px;">📹 Active Video Session</h3>
            
            <!-- Expert Video (Always Visible) -->
            <div class="expert-video-section">
                <div class="expert-video">
                    <h4>🎯 You (Expert)</h4>
                    <video id="localVideo" autoplay="autoplay" muted="muted" playsinline="playsinline"></video>
                    <div class="video-controls">
                        <button type="button" onclick="toggleMute()" id="muteBtn">🎤 Mute</button>
                        <button type="button" onclick="toggleVideo()" id="videoBtn">📹 Video</button>
                    </div>
                </div>
            </div>
            
            <!-- Participants Grid -->
            <div class="participants-video-grid" id="participantsGrid">
                <div class="no-participants-connected">
                    <p><strong>No participants connected yet</strong></p>
                    <p>Participants will appear here when they join the session</p>
                    <p><small>Registered participants can join using their session links</small></p>
                </div>
            </div>
        </div>

        <!-- Control Panel -->
        <div class="control-panel">
            <h3 style="color: #051D40; margin-bottom: 20px;">📞 Video Controls</h3>
            
            <div class="control-buttons">
                <button type="button" class="btn-control" onclick="connectToAllParticipants()">
                    📢 Connect to All Participants
                </button>
                <button type="button" class="btn-control secondary" onclick="refreshParticipantsList()">
                    🔄 Refresh Participants
                </button>
                <button type="button" class="btn-control danger" onclick="endAllCalls()">
                    📞 End All Calls
                </button>
            </div>
            
            <!-- Status Display -->
            <asp:Label ID="lblStatus" runat="server" CssClass="status-message info" Text="Initializing expert video system..." />
        </div>

        <!-- Registered Participants List -->
        <div class="participants-section">
            <div class="section-header">
                <h3 class="section-title">📋 Registered Participants</h3>
                <span class="participant-count">
                    <asp:Label ID="lblRegisteredCount" runat="server" Text="0" />
                </span>
            </div>

            <div id="registeredParticipants">
                <asp:Repeater ID="rptRegisteredParticipants" runat="server">
                    <ItemTemplate>
                        <div class="participant-card" data-phone="<%# Eval("CustomerPhone") %>" data-name="<%# Eval("CustomerName") %>">
                            <div class="participant-info">
                                <div class="participant-details">
                                    <h4>
                                        <span class="status-indicator status-waiting" id="status_<%# System.Text.RegularExpressions.Regex.Replace(Eval("CustomerPhone").ToString(), @"[^\d]", "") %>"></span>
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
                                            id="btn_<%# System.Text.RegularExpressions.Regex.Replace(Eval("CustomerPhone").ToString(), @"[^\d]", "") %>">
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
    </div>

    <script type="text/javascript">
        // FIXED: Expert Video Call System with Proper Connection Management
        let peer;
        let localStream;
        let connections = new Map();
        let activeParticipants = new Map();
        let sessionId = null;
        let isConnectionActive = false;
        let sessionStartTime = null;
        let statusUpdateInterval = null;

        function debugLog(message) {
            console.log('🔍 STAFF DEBUG:', message);
        }

        // FIXED: Initialize expert system on page load
        window.onload = function () {
            debugLog('Staff video call page loaded');

            // Get session ID
            const hdnSessionIdElement = document.getElementById('<%= hdnSessionId.ClientID %>');
            if (hdnSessionIdElement && hdnSessionIdElement.value) {
                sessionId = hdnSessionIdElement.value;
                debugLog('Session ID found: ' + sessionId);
            } else {
                debugLog('No session ID found');
                updateStatus('Session ID not found. Please select a session first.', 'error');
                return;
            }

            // Initialize the expert system
            initializeExpertSystem();
            
            // Start session timer
            sessionStartTime = new Date();
            setInterval(updateSessionTimer, 1000);

            // Start status updates
            statusUpdateInterval = setInterval(updateParticipantStatuses, 5000);
        };

        // FIXED: Initialize expert video system
        async function initializeExpertSystem() {
            try {
                debugLog('Initializing expert system...');
                updateStatus('Getting camera and microphone access...', 'info');

                // Get media first
                localStream = await navigator.mediaDevices.getUserMedia({
                    video: { width: 640, height: 480 },
                    audio: true
                });

                const localVideo = document.getElementById('localVideo');
                if (localVideo) {
                    localVideo.srcObject = localStream;
                    debugLog('Local video stream set');
                }

                // Create peer connection
                await createExpertPeer();

            } catch (err) {
                debugLog('Media error: ' + err.message);
                updateStatus('Camera/microphone access required. Please allow permissions and refresh.', 'error');
            }
        }

        // FIXED: Create expert peer with proper ID
        async function createExpertPeer() {
            try {
                const expertId = `expert_session_${sessionId}`;
                debugLog('Creating expert peer with ID: ' + expertId);

                peer = new Peer(expertId, {
                    host: '0.peerjs.com',
                    port: 443,
                    path: '/',
                    secure: true,
                    debug: 2
                });

                peer.on('open', (id) => {
                    debugLog('✅ Expert peer ready: ' + id);
                    isConnectionActive = true;
                    updateStatus('✅ Expert system ready! Waiting for participants to connect...', 'success');
                    
                    // Start checking for participants
                    setTimeout(checkForParticipants, 2000);
                });

                peer.on('call', (call) => {
                    debugLog('📞 Incoming call from: ' + call.peer);
                    handleIncomingCall(call);
                });

                peer.on('error', (err) => {
                    debugLog('❌ Peer error: ' + err.type + ' - ' + err.message);
                    updateStatus('Connection error: ' + err.message, 'error');
                    
                    // Try to reconnect
                    setTimeout(() => {
                        if (!isConnectionActive) {
                            createExpertPeer();
                        }
                    }, 5000);
                });

                peer.on('disconnected', () => {
                    debugLog('⚠️ Peer disconnected, attempting reconnection...');
                    isConnectionActive = false;
                    updateStatus('Connection lost. Attempting to reconnect...', 'info');

                    setTimeout(() => {
                        if (peer && !peer.destroyed) {
                            peer.reconnect();
                        }
                    }, 2000);
                });

            } catch (err) {
                debugLog('Error creating peer: ' + err.message);
                updateStatus('Failed to create peer connection. Please refresh and try again.', 'error');
            }
        }

        // FIXED: Handle incoming calls from participants with better duplicate prevention
        function handleIncomingCall(call) {
            const participantId = call.peer;
            debugLog('Handling incoming call from: ' + participantId);

            // FIXED: Check for existing connection and close it first
            if (connections.has(participantId)) {
                debugLog('⚠️ Participant already connected, closing old connection: ' + participantId);
                const oldConnection = connections.get(participantId);
                if (oldConnection && oldConnection !== call) {
                    oldConnection.close();
                }
                // Remove old UI element
                removeParticipantFromGrid(participantId);
            }

            // Answer the call with local stream
            call.answer(localStream);

            call.on('stream', (remoteStream) => {
                debugLog('✅ Received stream from: ' + participantId);
                addParticipantToGrid(participantId, remoteStream, call);
                updateConnectedCount();
            });

            call.on('close', () => {
                debugLog('📞 Call ended with: ' + participantId);
                removeParticipantFromGrid(participantId);
                updateConnectedCount();
            });

            call.on('error', (err) => {
                debugLog('📞 Call error with ' + participantId + ': ' + err.message);
                removeParticipantFromGrid(participantId);
                updateConnectedCount();
            });

            // Store the new connection
            connections.set(participantId, call);
        }

        // FIXED: Add participant to video grid with better duplicate prevention and naming
        function addParticipantToGrid(participantId, stream, call) {
            debugLog('Adding participant to grid: ' + participantId);

            // FIXED: More thorough duplicate removal
            const existingElement = document.getElementById(`participant_${participantId}`);
            if (existingElement) {
                debugLog('Removing existing participant element: ' + participantId);
                existingElement.remove();
                activeParticipants.delete(participantId);
            }

            // Also check for any elements with similar IDs (in case of phone number variations)
            const participantPhoneFromId = participantId.replace('customer_', '');
            const allParticipantElements = document.querySelectorAll('[id^="participant_customer_"]');
            allParticipantElements.forEach(element => {
                const elementPhone = element.id.replace('participant_customer_', '');
                if (elementPhone === participantPhoneFromId && element.id !== `participant_${participantId}`) {
                    debugLog('Removing duplicate participant element: ' + element.id);
                    element.remove();
                }
            });

            const grid = document.getElementById('participantsGrid');
            
            // Remove "no participants" message if present
            const noParticipantsMsg = grid.querySelector('.no-participants-connected');
            if (noParticipantsMsg) {
                noParticipantsMsg.remove();
            }

            // FIXED: Get participant name from server using phone number with retry mechanism
            const participantPhoneNumber = participantId.replace('customer_', '');
            getParticipantRealNameFromServer(participantPhoneNumber).then(participantName => {
                // Double-check no duplicate was created while we were getting the name
                const finalCheck = document.getElementById(`participant_${participantId}`);
                if (finalCheck) {
                    finalCheck.remove();
                }

                const participantDiv = document.createElement('div');
                participantDiv.className = 'participant-video-wrapper';
                participantDiv.id = `participant_${participantId}`;

                participantDiv.innerHTML = `
                    <div class="participant-video-header">
                        <h4>👤 ${participantName}</h4>
                        <span class="participant-status">Connected</span>
                    </div>
                    <video autoplay playsinline></video>
                `;

                grid.appendChild(participantDiv);

                const video = participantDiv.querySelector('video');
                if (video) {
                    video.srcObject = stream;
                    // FIXED: Ensure audio is enabled
                    video.muted = false;
                    video.volume = 1.0;
                    
                    // FIXED: Add error handling for video
                    video.onerror = function(e) {
                        debugLog('Video error for participant ' + participantId + ': ' + e.target.error);
                    };
                }

                activeParticipants.set(participantId, { stream, call, name: participantName });
                
                // FIXED: Update grid layout based on number of participants
                updateGridLayout();
                
                debugLog('✅ Participant successfully added to grid: ' + participantName);
                
                // FIXED: Update participant button status in the registered list
                updateParticipantButtonStatus(participantPhoneNumber, true);
            });
        }

        // FIXED: Get real participant name from server with multiple retry attempts
        async function getParticipantRealNameFromServer(phoneNumber) {
            let retryCount = 0;
            const maxRetries = 3;
            
            while (retryCount < maxRetries) {
                try {
                    debugLog(`Getting real name for phone: ${phoneNumber} (attempt ${retryCount + 1})`);

                    const response = await fetch('<%= Page.ResolveUrl("~/Staff/StaffVideoCall.aspx/GetParticipantRealName") %>', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            sessionId: parseInt(sessionId),
                            phoneNumber: phoneNumber
                        })
                    });

                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }

                    const data = await response.json();
                    const result = JSON.parse(data.d);

                    if (result.success && result.name) {
                        debugLog('Real name found: ' + result.name);
                        return result.name;
                    }
                    
                    retryCount++;
                    if (retryCount < maxRetries) {
                        debugLog(`Name not found, retrying in ${retryCount * 500}ms...`);
                        await new Promise(resolve => setTimeout(resolve, retryCount * 500));
                    }
                } catch (error) {
                    debugLog(`Error getting participant real name (attempt ${retryCount + 1}): ${error.message}`);
                    retryCount++;
                    if (retryCount < maxRetries) {
                        await new Promise(resolve => setTimeout(resolve, retryCount * 500));
                    }
                }
            }

            // Fallback: look in registered participants list
            const participantCards = document.querySelectorAll('.participant-card');
            for (let card of participantCards) {
                const cardPhone = card.getAttribute('data-phone');
                const cleanCardPhone = cardPhone ? cardPhone.replace(/[^0-9]/g, '') : '';
                
                if (cleanCardPhone === phoneNumber) {
                    const name = card.getAttribute('data-name');
                    if (name && name.trim() !== '') {
                        debugLog('Found name in UI: ' + name);
                        return name;
                    }
                }
            }
            
            // Final fallback
            return `Participant (${phoneNumber.substring(0, 4)}...${phoneNumber.substring(phoneNumber.length - 4)})`;
        }

        // FIXED: Update grid layout based on number of participants
        function updateGridLayout() {
            const grid = document.getElementById('participantsGrid');
            const participantCount = activeParticipants.size;
            
            if (participantCount === 1) {
                grid.classList.add('single-participant');
            } else {
                grid.classList.remove('single-participant');
            }
        }

        // FIXED: Remove participant from grid with better cleanup
        function removeParticipantFromGrid(participantId) {
            debugLog('Removing participant from grid: ' + participantId);
            
            const element = document.getElementById(`participant_${participantId}`);
            if (element) {
                element.remove();
                debugLog('✅ Removed participant UI element: ' + participantId);
            }

            activeParticipants.delete(participantId);
            connections.delete(participantId);

            // FIXED: Update participant button status in the registered list
            const participantPhoneFromId = participantId.replace('customer_', '');
            updateParticipantButtonStatus(participantPhoneFromId, false);

            // FIXED: Update grid layout after removal
            updateGridLayout();

            // Show "no participants" message if no one is connected
            const grid = document.getElementById('participantsGrid');
            if (grid.children.length === 0) {
                grid.innerHTML = `
                    <div class="no-participants-connected">
                        <p><strong>No participants connected</strong></p>
                        <p>Waiting for participants to join...</p>
                    </div>
                `;
                grid.classList.remove('single-participant');
            }
        }

        // FIXED: Update participant button status in the registered participants list
        function updateParticipantButtonStatus(phoneNumber, isConnected) {
            const cleanPhone = phoneNumber.replace(/[^0-9]/g, '');
            const buttonId = `btn_${cleanPhone}`;
            const button = document.getElementById(buttonId);
            
            if (button) {
                if (isConnected) {
                    button.textContent = '✅ Connected';
                    button.disabled = true;
                    button.style.background = '#6c757d';
                } else {
                    button.textContent = '📞 Connect';
                    button.disabled = false;
                    button.style.background = '#28a745';
                }
            }
        }

        // FIXED: Connect to specific participant - THIS WAS MISSING
        function connectToParticipant(phone, name) {
            if (!peer || !localStream || !isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }

            const cleanPhone = phone.replace(/[^0-9]/g, '');
            const participantId = `customer_${cleanPhone}`;

            debugLog('Attempting to connect to: ' + participantId + ' (' + name + ')');

            // Check if already connected
            if (connections.has(participantId)) {
                updateStatus(`Already connected to ${name}`, 'info');
                return;
            }

            try {
                const call = peer.call(participantId, localStream);

                if (call) {
                    debugLog('Call initiated to: ' + participantId);

                    call.on('stream', (remoteStream) => {
                        debugLog('Connected to participant: ' + name);
                        addParticipantToGrid(participantId, remoteStream, call);
                        updateConnectedCount();
                        updateStatus(`Connected to ${name}!`, 'success');
                    });

                    call.on('error', (err) => {
                        debugLog('Call error: ' + err.message);
                        updateStatus(`Failed to connect to ${name}: ${err.message}`, 'error');
                    });

                    call.on('close', () => {
                        debugLog('Call closed with: ' + name);
                        removeParticipantFromGrid(participantId);
                        updateConnectedCount();
                        updateStatus(`${name} disconnected`, 'info');
                    });

                    connections.set(participantId, call);
                    updateStatus(`Connecting to ${name}...`, 'info');
                } else {
                    updateStatus(`Failed to initiate call to ${name}`, 'error');
                }
            } catch (err) {
                debugLog('Error calling participant: ' + err.message);
                updateStatus(`Error connecting to ${name}: ${err.message}`, 'error');
            }
        }

        // FIXED: Connect to all registered participants
        function connectToAllParticipants() {
            if (!peer || !localStream || !isConnectionActive) {
                updateStatus('Expert system not ready. Please wait...', 'error');
                return;
            }

            debugLog('Connecting to all participants...');
            const participantCards = document.querySelectorAll('.participant-card');

            if (participantCards.length === 0) {
                updateStatus('No registered participants found', 'info');
                return;
            }

            let connectCount = 0;
            participantCards.forEach((card, index) => {
                const phone = card.getAttribute('data-phone');
                const name = card.getAttribute('data-name');

                if (phone && name) {
                    // Stagger connections to avoid overwhelming the system
                    setTimeout(() => {
                        connectToParticipant(phone, name);
                    }, index * 1000);
                    connectCount++;
                }
            });

            updateStatus(`Connecting to ${connectCount} participants...`, 'info');
        }

        // FIXED: Check for participants periodically
        async function checkForParticipants() {
            if (!sessionId) return;

            try {
                const response = await fetch('<%= Page.ResolveUrl("~/Staff/StaffVideoCall.aspx/GetParticipantUpdates") %>', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ sessionId: parseInt(sessionId) })
                });

                const data = await response.json();
                const result = JSON.parse(data.d);

                if (result.success && result.participants) {
                    updateOnlineParticipants(result.participants.length);
                }
            } catch (error) {
                debugLog('Error checking participants: ' + error.message);
            }

            // Check again in 10 seconds
            setTimeout(checkForParticipants, 10000);
        }

        // Update UI counters
        function updateConnectedCount() {
            const connectedElement = document.getElementById('connectedParticipants');
            if (connectedElement) {
                connectedElement.textContent = activeParticipants.size;
            }
        }

        function updateOnlineParticipants(count) {
            const onlineElement = document.getElementById('onlineParticipants');
            if (onlineElement) {
                onlineElement.textContent = count;
            }
        }

        function updateSessionTimer() {
            if (sessionStartTime) {
                const now = new Date();
                const diff = now - sessionStartTime;
                const minutes = Math.floor(diff / 60000);
                const seconds = Math.floor((diff % 60000) / 1000);

                const durationElement = document.getElementById('sessionDuration');
                if (durationElement) {
                    durationElement.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;
                }
            }
        }

        // Control functions
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

        function refreshParticipantsList() {
            window.location.reload();
        }

        function endAllCalls() {
            debugLog('Ending all calls...');

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            connections.forEach((call) => {
                call.close();
            });

            connections.clear();
            activeParticipants.clear();

            if (peer) {
                peer.destroy();
            }

            if (statusUpdateInterval) {
                clearInterval(statusUpdateInterval);
            }

            updateStatus('All calls ended', 'info');
            
            // Reset the grid
            const grid = document.getElementById('participantsGrid');
            if (grid) {
                grid.innerHTML = `
                    <div class="no-participants-connected">
                        <p><strong>Session ended</strong></p>
                        <p>Refresh the page to start a new session</p>
                    </div>
                `;
            }
        }

        // FIXED: Update status function with better error handling
        function updateStatus(message, type) {
            debugLog(`Status (${type}): ${message}`);

            let statusLabel = document.getElementById('<%= lblStatus.ClientID %>');

            if (!statusLabel) {
                statusLabel = document.querySelector('.status-message');
            }

            if (statusLabel) {
                statusLabel.textContent = message;
                statusLabel.className = `status-message ${type}`;
            } else {
                console.warn('Status label not found');
            }
        }

        // FIXED: Update participant statuses
        function updateParticipantStatuses() {
            const participantCards = document.querySelectorAll('.participant-card');
            participantCards.forEach(card => {
                const phone = card.getAttribute('data-phone');
                if (phone) {
                    const cleanPhone = phone.replace(/[^0-9]/g, '');
                    const participantId = `customer_${cleanPhone}`;
                    const statusIndicator = card.querySelector('.status-indicator');
                    const connectBtn = card.querySelector('.btn-connect');

                    if (connections.has(participantId)) {
                        if (statusIndicator) {
                            statusIndicator.className = 'status-indicator status-connected';
                            statusIndicator.style.background = '#007bff';
                        }
                        if (connectBtn) {
                            connectBtn.textContent = '✅ Connected';
                            connectBtn.disabled = true;
                            connectBtn.style.background = '#6c757d';
                        }
                    } else {
                        if (statusIndicator) {
                            statusIndicator.className = 'status-indicator status-waiting';
                            statusIndicator.style.background = '#ffc107';
                        }
                        if (connectBtn) {
                            connectBtn.textContent = '📞 Connect';
                            connectBtn.disabled = false;
                            connectBtn.style.background = '#28a745';
                        }
                    }
                }
            });
        }

        // Cleanup on page unload
        window.onbeforeunload = function () {
            debugLog('Cleaning up resources...');

            if (statusUpdateInterval) {
                clearInterval(statusUpdateInterval);
            }

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            if (peer && isConnectionActive) {
                peer.destroy();
            }
        };
    </script>
</asp:Content>