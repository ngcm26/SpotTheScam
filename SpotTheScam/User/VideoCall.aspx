<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="VideoCall.aspx.cs" Inherits="SpotTheScam.User.VideoCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/peerjs/1.4.7/peerjs.min.js"></script>
    <style>
        .main-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        .phone-entry-container {
            background: linear-gradient(135deg, #D36F2D 0%, #e67e22 100%);
            color: white;
            padding: 40px 30px;
            border-radius: 15px;
            text-align: center;
            margin-bottom: 20px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .video-container {
            display: none;
            gap: 20px;
            padding: 20px;
            margin-bottom: 20px;
        }

        .video-wrapper {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 15px;
            text-align: center;
            flex: 1;
            max-width: 500px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
        }

        .video-wrapper h3 {
            color: #051D40;
            margin-bottom: 15px;
            font-size: 1.1rem;
            font-weight: 600;
        }

        video {
            width: 100%;
            max-width: 480px;
            height: 360px;
            border-radius: 10px;
            background: #000;
            object-fit: cover;
        }

        .session-info {
            margin-bottom: 20px;
            padding: 25px;
            background: #f8f9fa;
            border-radius: 15px;
            display: none;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
        }

        .control-panel {
            text-align: center;
            margin: 20px 0;
            padding: 20px;
            background: white;
            border-radius: 15px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
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

        .phone-input {
            width: 100%;
            max-width: 350px;
            padding: 18px 20px;
            font-size: 18px;
            border: 2px solid rgba(255,255,255,0.3);
            border-radius: 10px;
            margin: 15px 0;
            text-align: center;
            background: rgba(255,255,255,0.1);
            color: white;
        }

        .phone-input::placeholder {
            color: rgba(255,255,255,0.7);
        }

        .join-btn {
            background: white;
            color: #D36F2D;
            padding: 18px 35px;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            margin-top: 10px;
        }

        .join-btn:hover {
            background: #f8f9fa;
            transform: translateY(-2px);
        }

        .join-btn:disabled {
            background: rgba(255,255,255,0.5);
            cursor: not-allowed;
            transform: none;
        }

        .expert-consultation-badge {
            background: linear-gradient(45deg, #FFC107, #FF9800);
            color: #000;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.85rem;
            font-weight: 600;
            display: inline-block;
            margin-bottom: 15px;
        }

        .other-participants-section {
            background: white;
            border-radius: 15px;
            padding: 20px;
            margin-top: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            display: none;
        }

        .other-participants-section h3 {
            color: #051D40;
            margin-bottom: 15px;
            font-size: 1.2rem;
            font-weight: 600;
        }

        .participants-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-top: 15px;
        }

        .participant-video-card {
            background: #f8f9fa;
            border: 2px solid #e9ecef;
            border-radius: 12px;
            padding: 15px;
            transition: all 0.3s ease;
        }

        .participant-video-card:hover {
            border-color: #D36F2D;
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.1);
        }

        .participant-video-card video {
            width: 100%;
            height: 200px;
            border-radius: 8px;
            background: #000;
            object-fit: cover;
        }

        .participant-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 12px;
        }

        .participant-header h4 {
            margin: 0;
            color: #051D40;
            font-size: 1rem;
            font-weight: 600;
        }

        .participant-status {
            background: #28a745;
            color: white;
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 0.75rem;
            font-weight: 600;
        }

        .control-buttons {
            display: flex;
            justify-content: center;
            gap: 15px;
            margin: 20px 0;
            flex-wrap: wrap;
        }

        .btn-control {
            background: #D36F2D;
            color: white;
            padding: 12px 25px;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-control:hover {
            background: #b45a22;
            transform: translateY(-2px);
        }

        .btn-control.danger {
            background: #dc3545;
        }

        .btn-control.danger:hover {
            background: #c82333;
        }

        @media (max-width: 768px) {
            .video-container {
                flex-direction: column;
            }
            
            .participants-grid {
                grid-template-columns: 1fr;
            }
            
            .participant-video-card video {
                height: 200px;
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
        <asp:HiddenField ID="hdnCustomerPhone" runat="server" />
        <asp:HiddenField ID="hdnUserPhone" runat="server" />
        
        <!-- Phone Entry Section -->
        <div id="phoneEntrySection" class="phone-entry-container">
            <h2>Join Expert Consultation</h2>
            <p>Enter your phone number to connect with our scam prevention expert</p>
            <div class="expert-consultation-badge">🛡️ Expert Video Consultation</div>
            <input type="text" id="phoneInput" class="phone-input" placeholder="Enter your phone number" />
            <br />
            <button id="joinBtn" class="join-btn" type="button" onclick="joinSession()">Connect with Expert</button>
            <div id="phoneStatus" class="status-message" style="display: none;"></div>
        </div>

        <!-- Session Info -->
        <div id="sessionInfo" class="session-info">
            <h2>Expert Consultation Session</h2>
            <div class="expert-consultation-badge">🎯 Live with Expert</div>
            <asp:Label ID="lblSessionInfo" runat="server" />
        </div>

        <!-- Video Interface -->
        <div id="videoCallInterface" class="video-container">
            <div class="video-wrapper">
                <h3>Your Video</h3>
                <video id="localVideo" autoplay="autoplay" muted="muted" playsinline="playsinline"></video>
            </div>
            <div class="video-wrapper">
                <h3>Expert Video</h3>
                <video id="remoteVideo" autoplay="autoplay" playsinline="playsinline"></video>
            </div>
        </div>

        <!-- Control Panel -->
        <div class="control-panel">
            <asp:Label ID="lblStatus" runat="server" CssClass="status-message info" 
                Text="Enter your phone number above to join the expert consultation" />
            
            <div class="control-buttons" id="controlButtons" style="display: none;">
                <button type="button" class="btn-control" onclick="toggleMute()" id="muteBtn">🎤 Mute</button>
                <button type="button" class="btn-control" onclick="toggleVideo()" id="videoBtn">📹 Video</button>
                <button type="button" class="btn-control danger" onclick="endCall()" id="endCallBtn">📞 End Call</button>
            </div>
        </div>

        <!-- Other Participants Section (Created Dynamically) -->
        <!-- This section will be created by JavaScript when other participants join -->
    </div>

    <script type="text/javascript">
        // COMPLETE: User Video Call System with Working Participant Discovery
        let peer;
        let localStream;
        let expertCall;
        let isConnected = false;
        let sessionId = null;
        let customerPhone = '';
        let participantConnections = new Map();
        let participantNames = new Map();
        let connectionRetryCount = 0;
        let maxRetries = 3;
        let participantDiscoveryInterval = null;
        let myPeerId = null;
        let lastParticipantCheck = [];

        function debugLog(message) {
            console.log('🔍 USER DEBUG:', message);
        }

        // Page load initialization
        window.onload = function () {
            debugLog('User video call page loaded');

            sessionId = getSessionId();
            customerPhone = getCustomerPhone();

            debugLog('Session ID: ' + sessionId + ', Phone: ' + customerPhone);

            if (sessionId && customerPhone) {
                debugLog('Auto-joining session');
                document.getElementById('phoneInput').value = customerPhone;
                document.getElementById('phoneInput').style.display = 'none';
                document.getElementById('joinBtn').innerText = 'Connecting to Your Session...';
                setTimeout(() => joinSession(), 1000);
            }
        };

        // Join session with better error handling
        async function joinSession() {
            const phoneInput = document.getElementById('phoneInput');
            const joinBtn = document.getElementById('joinBtn');

            const phone = phoneInput.value.trim();

            if (!phone) {
                updateStatus('Please enter your phone number', 'error');
                return;
            }

            joinBtn.disabled = true;
            joinBtn.innerText = 'Connecting...';
            debugLog('Starting join process with phone: ' + phone);

            try {
                const response = await fetch('VideoCall.aspx/CheckSession', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ phoneNumber: phone })
                });

                const data = await response.json();
                const result = JSON.parse(data.d);

                debugLog('Session check result: ' + JSON.stringify(result));

                if (result.success) {
                    sessionId = result.sessionId;
                    customerPhone = phone;

                    // Update hidden fields
                    document.getElementById('<%= hdnSessionId.ClientID %>').value = sessionId;
                    document.getElementById('<%= hdnCustomerPhone.ClientID %>').value = phone;
                    document.getElementById('<%= hdnUserPhone.ClientID %>').value = phone;

                    // Update UI
                    document.getElementById('phoneEntrySection').style.display = 'none';
                    document.getElementById('sessionInfo').style.display = 'block';

                    debugLog('Session found, initializing video...');
                    await initializeVideo();

                } else {
                    updateStatus(result.message, 'error');
                    joinBtn.disabled = false;
                    joinBtn.innerText = 'Connect with Expert';
                }

            } catch (error) {
                debugLog('Error in joinSession: ' + error.message);
                updateStatus('Connection error. Please try again.', 'error');
                joinBtn.disabled = false;
                joinBtn.innerText = 'Connect with Expert';
            }
        }

        // Initialize video with proper error handling
        async function initializeVideo() {
            try {
                debugLog('Getting user media...');
                updateStatus('Setting up your camera and microphone...', 'info');

                localStream = await navigator.mediaDevices.getUserMedia({
                    video: { width: 640, height: 480 },
                    audio: true
                });

                const localVideo = document.getElementById('localVideo');
                if (localVideo) {
                    localVideo.srcObject = localStream;
                    debugLog('Local video stream set');
                }

                // Show video interface and controls
                document.getElementById('videoCallInterface').style.display = 'flex';
                document.getElementById('controlButtons').style.display = 'flex';

                debugLog('Creating peer connection...');
                await createPeerConnection();

            } catch (err) {
                debugLog('Media error: ' + err.message);
                updateStatus('Camera/microphone access required. Please allow permissions and refresh.', 'error');
            }
        }

        // Create peer connection with aggressive participant discovery
        async function createPeerConnection() {
            try {
                const cleanPhone = customerPhone.replace(/[^0-9]/g, '');
                const peerId = `customer_${cleanPhone}`;
                myPeerId = peerId;

                debugLog('Creating peer with ID: ' + peerId);

                // Destroy existing peer if it exists
                if (peer && !peer.destroyed) {
                    debugLog('Destroying existing peer before creating new one');
                    peer.destroy();
                }

                peer = new Peer(peerId, {
                    host: '0.peerjs.com',
                    port: 443,
                    path: '/',
                    secure: true,
                    debug: 1,
                    config: {
                        'iceServers': [
                            { urls: 'stun:stun.l.google.com:19302' },
                            { urls: 'stun:global.stun.twilio.com:3478' }
                        ]
                    }
                });

                peer.on('open', (id) => {
                    debugLog('✅ Peer connected with ID: ' + id);
                    myPeerId = id;
                    connectionRetryCount = 0;
                    updateStatus('Connected! Looking for expert and other participants...', 'success');

                    // Connect to expert first
                    setTimeout(() => connectToExpert(), 2000);

                    // Start aggressive participant discovery
                    setTimeout(() => {
                        debugLog('🚀 Starting aggressive participant discovery');
                        discoverParticipants();

                        // Set up frequent participant discovery - every 3 seconds for better responsiveness
                        participantDiscoveryInterval = setInterval(() => {
                            if (sessionId && peer && !peer.destroyed) {
                                discoverParticipants();
                            }
                        }, 3000);
                    }, 1000);
                });

                peer.on('call', (call) => {
                    debugLog('📞 Incoming call from: ' + call.peer);
                    handleIncomingCall(call);
                });

                peer.on('error', (err) => {
                    debugLog('Peer error: ' + err.type + ' - ' + err.message);

                    if (err.type === 'peer-unavailable') {
                        if (err.message.includes('expert_session_')) {
                            updateStatus('Expert not available yet. Retrying...', 'info');
                            setTimeout(() => connectToExpert(), 3000);
                        } else {
                            debugLog('Participant peer unavailable: ' + err.message);
                        }
                    } else if (err.type === 'network') {
                        updateStatus('Network connection issue. Checking connection...', 'info');
                        handleNetworkError();
                    } else if (err.type === 'peer-destroyed') {
                        debugLog('Peer was destroyed, this is expected during reconnection');
                    } else {
                        updateStatus('Connection error: ' + err.message, 'error');
                    }
                });

                peer.on('disconnected', () => {
                    debugLog('Peer disconnected');

                    if (!peer.destroyed) {
                        updateStatus('Connection lost. Attempting to reconnect...', 'info');

                        setTimeout(() => {
                            if (peer && !peer.destroyed && connectionRetryCount < maxRetries) {
                                connectionRetryCount++;
                                debugLog(`Reconnection attempt ${connectionRetryCount}/${maxRetries}`);
                                peer.reconnect();
                            } else if (connectionRetryCount >= maxRetries) {
                                updateStatus('Connection failed after multiple attempts. Please refresh the page.', 'error');
                            }
                        }, 2000);
                    }
                });

            } catch (err) {
                debugLog('Error creating peer: ' + err.message);
                updateStatus('Failed to create connection. Please refresh and try again.', 'error');
            }
        }

        // Connect to expert
        function connectToExpert() {
            if (!peer || !localStream || !sessionId) {
                debugLog('Cannot connect to expert - missing requirements');
                setTimeout(() => connectToExpert(), 3000);
                return;
            }

            if (peer.disconnected || peer.destroyed) {
                debugLog('Peer not ready for expert connection, waiting...');
                setTimeout(() => connectToExpert(), 2000);
                return;
            }

            const expertId = `expert_session_${sessionId}`;
            debugLog('Attempting to call expert: ' + expertId);

            if (expertCall && !expertCall.open) {
                expertCall.close();
                expertCall = null;
            }

            try {
                const call = peer.call(expertId, localStream);

                if (call) {
                    expertCall = call;

                    call.on('stream', (remoteStream) => {
                        debugLog('✅ Received expert video stream');
                        handleExpertStream(remoteStream);
                    });

                    call.on('error', (err) => {
                        debugLog('Expert call error: ' + err.message);
                        expertCall = null;

                        if (!isConnected) {
                            updateStatus('Expert not available. Retrying...', 'info');
                            setTimeout(() => connectToExpert(), 5000);
                        }
                    });

                    call.on('close', () => {
                        debugLog('Expert call ended');
                        expertCall = null;
                        handleExpertDisconnect();
                    });

                    updateStatus('Calling expert...', 'info');
                } else {
                    debugLog('Failed to create call to expert');
                    setTimeout(() => connectToExpert(), 5000);
                }
            } catch (err) {
                debugLog('Error calling expert: ' + err.message);
                setTimeout(() => connectToExpert(), 5000);
            }
        }

        // Handle incoming calls
        function handleIncomingCall(call) {
            debugLog('Handling incoming call from: ' + call.peer);

            if (call.peer.includes('expert_session_')) {
                debugLog('Expert is calling us directly');

                if (expertCall && expertCall !== call) {
                    debugLog('Closing existing expert call to accept new one');
                    expertCall.close();
                }
                expertCall = call;
            } else if (call.peer.includes('customer_')) {
                const participantId = call.peer;

                if (participantConnections.has(participantId)) {
                    const existing = participantConnections.get(participantId);
                    if (existing.call && existing.call !== call) {
                        debugLog('Closing existing participant call: ' + participantId);
                        existing.call.close();
                        removeParticipant(participantId);
                    }
                }
            }

            call.answer(localStream);

            call.on('stream', (remoteStream) => {
                debugLog('Received stream from: ' + call.peer);

                if (call.peer.includes('expert_session_')) {
                    handleExpertStream(remoteStream);
                } else if (call.peer.includes('customer_')) {
                    handleParticipantStream(call.peer, remoteStream, call);
                }
            });

            call.on('close', () => {
                debugLog('Call closed from: ' + call.peer);
                if (call.peer.includes('expert_session_')) {
                    expertCall = null;
                    handleExpertDisconnect();
                } else {
                    removeParticipant(call.peer);
                }
            });

            call.on('error', (err) => {
                debugLog('Call error from ' + call.peer + ': ' + err.message);
                if (call.peer.includes('expert_session_')) {
                    expertCall = null;
                    handleExpertDisconnect();
                } else {
                    removeParticipant(call.peer);
                }
            });
        }

        // Handle expert stream
        function handleExpertStream(stream) {
            debugLog('Setting expert video stream');

            const remoteVideo = document.getElementById('remoteVideo');
            if (remoteVideo) {
                remoteVideo.srcObject = stream;
                remoteVideo.muted = false;
                remoteVideo.volume = 1.0;

                isConnected = true;
                updateStatus('🎉 Connected with expert! Looking for other participants...', 'success');

                // Continue participant discovery after expert connection
                setTimeout(() => {
                    debugLog('Continuing participant discovery after expert connection');
                    discoverParticipants();
                }, 1000);
            }
        }

        // Handle expert disconnect
        function handleExpertDisconnect() {
            debugLog('Expert disconnected');

            const remoteVideo = document.getElementById('remoteVideo');
            if (remoteVideo) {
                remoteVideo.srcObject = null;
            }

            isConnected = false;
            expertCall = null;
            updateStatus('Expert disconnected. Waiting for reconnection...', 'info');

            setTimeout(() => connectToExpert(), 3000);
        }

        // Handle participant stream with guaranteed UI display
        function handleParticipantStream(participantId, stream, call) {
            debugLog('✅ Adding participant stream: ' + participantId);

            participantConnections.set(participantId, { call, stream });

            // Get real participant name
            getParticipantRealName(participantId).then(realName => {
                participantNames.set(participantId, realName);
                addParticipantToUI(participantId, stream, realName);

                // CRITICAL FIX: Always update status and ensure section is visible
                const participantCount = participantConnections.size;
                updateStatus(`🎉 Connected with expert and ${participantCount} other participant(s)!`, 'success');

                // Ensure participant section is visible
                ensureParticipantSectionVisible();

                debugLog(`📺 Total participants now: ${participantCount}`);
            });
        }

        // Get participant name from server
        async function getParticipantRealName(participantId) {
            try {
                const cleanPhone = participantId.replace('customer_', '');

                const response = await fetch('VideoCall.aspx/GetParticipantName', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        sessionId: parseInt(sessionId),
                        phoneNumber: cleanPhone
                    })
                });

                const data = await response.json();
                const result = JSON.parse(data.d);

                if (result.success && result.name) {
                    return result.name;
                }
            } catch (error) {
                debugLog('Error getting participant name: ' + error.message);
            }

            const cleanPhone = participantId.replace('customer_', '');
            return `Participant (${cleanPhone.substring(0, Math.min(4, cleanPhone.length))}...${cleanPhone.substring(Math.max(0, cleanPhone.length - 4))})`;
        }

        // Add participant to UI with guaranteed visibility
        function addParticipantToUI(participantId, stream, participantName) {
            debugLog('📺 Adding participant to UI: ' + participantName + ' (' + participantId + ')');

            let participantsSection = document.querySelector('.other-participants-section');
            if (!participantsSection) {
                const mainContainer = document.querySelector('.main-container');
                participantsSection = document.createElement('div');
                participantsSection.className = 'other-participants-section';
                participantsSection.innerHTML = `
                    <h3>👥 Other Participants</h3>
                    <div class="participants-grid" id="participantsGrid"></div>
                `;
                mainContainer.appendChild(participantsSection);
                debugLog('✅ Created participants section');
            }

            // CRITICAL FIX: Always show the section when we have participants
            participantsSection.style.display = 'block';
            debugLog('✅ Participant section is now visible');

            const existingParticipant = document.getElementById(`participant_${participantId}`);
            if (existingParticipant) {
                debugLog('Participant already exists in UI, updating stream');
                const existingVideo = existingParticipant.querySelector('video');
                if (existingVideo) {
                    existingVideo.srcObject = stream;
                }

                const nameElement = existingParticipant.querySelector('h4');
                if (nameElement) {
                    nameElement.textContent = `👤 ${participantName}`;
                }
                return;
            }

            const grid = document.getElementById('participantsGrid');
            const participantDiv = document.createElement('div');
            participantDiv.className = 'participant-video-card';
            participantDiv.id = `participant_${participantId}`;

            participantDiv.innerHTML = `
                <div class="participant-header">
                    <h4>👤 ${participantName}</h4>
                    <span class="participant-status">Online</span>
                </div>
                <video autoplay playsinline></video>
            `;

            grid.appendChild(participantDiv);

            const video = participantDiv.querySelector('video');
            if (video) {
                video.srcObject = stream;
                video.muted = false;
                video.volume = 1.0;

                // Add error handling for video
                video.onerror = function (e) {
                    debugLog('❌ Video error for participant ' + participantId + ': ' + e.message);
                };

                video.onloadedmetadata = function () {
                    debugLog('✅ Video loaded for participant: ' + participantName);
                };
            }

            debugLog('✅ Participant added to UI successfully: ' + participantName);

            // FINAL CHECK: Ensure the section is definitely visible
            setTimeout(() => {
                if (participantsSection.style.display !== 'block') {
                    participantsSection.style.display = 'block';
                    debugLog('🔧 Force-showing participant section');
                }
            }, 100);
        }

        // Enhanced participant discovery with better connection logic AND fallback for incoming calls
        async function discoverParticipants() {
            if (!sessionId || !peer || peer.destroyed) {
                debugLog('Cannot discover participants - requirements not met');
                return;
            }

            try {
                debugLog('🔍 Discovering participants for session: ' + sessionId);

                const response = await fetch('VideoCall.aspx/GetSessionParticipants', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ sessionId: parseInt(sessionId) })
                });

                const data = await response.json();
                const result = JSON.parse(data.d);

                if (result.success && result.participants) {
                    debugLog(`📊 Found ${result.participants.length} total participants for session ${sessionId}`);

                    const myPhone = customerPhone.replace(/[^0-9]/g, '');
                    let newParticipantsFound = 0;
                    let validParticipants = [];

                    result.participants.forEach((participant) => {
                        const cleanPhone = participant.phone.replace(/[^0-9]/g, '');

                        // Skip self, empty phones, and test phones
                        if (cleanPhone === myPhone || !cleanPhone || cleanPhone === '' || cleanPhone === '12345678') {
                            debugLog(`⏭️ Skipping invalid/self phone: ${cleanPhone} (my phone: ${myPhone})`);
                            return;
                        }

                        validParticipants.push(participant);
                        const participantId = `customer_${cleanPhone}`;

                        if (!participantConnections.has(participantId)) {
                            debugLog(`🆕 Found new participant: ${participantId} (${participant.name})`);
                            newParticipantsFound++;

                            // Try to connect immediately and also with a slight delay for better success rate
                            setTimeout(() => {
                                connectToParticipant(participantId, participant.name);
                            }, Math.random() * 1000);

                            // Also try again after a delay in case the first attempt fails
                            setTimeout(() => {
                                if (!participantConnections.has(participantId)) {
                                    debugLog(`🔄 Retry connecting to: ${participantId}`);
                                    connectToParticipant(participantId, participant.name);
                                }
                            }, 3000 + Math.random() * 2000);
                        } else {
                            debugLog(`✅ Already connected to: ${participantId}`);
                        }
                    });

                    // Update status based on discovery results
                    if (newParticipantsFound > 0) {
                        debugLog(`🚀 Attempting to connect to ${newParticipantsFound} new participants`);
                        updateStatus(`Found ${newParticipantsFound} other participants. Connecting...`, 'info');
                    } else if (validParticipants.length > 0) {
                        debugLog(`👥 All ${validParticipants.length} other participants already connected`);
                        if (isConnected) {
                            updateStatus(`🎉 Connected with expert and ${participantConnections.size} other participant(s)!`, 'success');
                        }
                    } else {
                        debugLog(`👤 Only you in this session currently (or database discovery failed)`);

                        // CRITICAL FIX: If database discovery failed but we have active connections, still update UI
                        if (isConnected && participantConnections.size > 0) {
                            debugLog(`📺 Database discovery failed but we have ${participantConnections.size} active connections`);
                            updateStatus(`🎉 Connected with expert and ${participantConnections.size} other participant(s)!`, 'success');

                            // Ensure participant section is visible
                            ensureParticipantSectionVisible();
                        } else if (isConnected) {
                            updateStatus('🎉 Connected with expert! Waiting for other participants...', 'success');
                        }
                    }

                    // Keep track of current participants for comparison
                    lastParticipantCheck = validParticipants.map(p => p.phone);

                } else {
                    debugLog('❌ Database discovery failed: ' + (result.message || 'Unknown error'));

                    // CRITICAL FIX: Even if database discovery fails, check if we have active connections
                    if (isConnected && participantConnections.size > 0) {
                        debugLog(`📺 Database discovery failed but we have ${participantConnections.size} active connections via incoming calls`);
                        updateStatus(`🎉 Connected with expert and ${participantConnections.size} other participant(s)!`, 'success');

                        // Ensure participant section is visible
                        ensureParticipantSectionVisible();
                    }
                }
            } catch (error) {
                debugLog('❌ Error discovering participants: ' + error.message);

                // CRITICAL FIX: Even if there's an error, check if we have active connections
                if (isConnected && participantConnections.size > 0) {
                    debugLog(`📺 Discovery error but we have ${participantConnections.size} active connections via incoming calls`);
                    updateStatus(`🎉 Connected with expert and ${participantConnections.size} other participant(s)!`, 'success');

                    // Ensure participant section is visible
                    ensureParticipantSectionVisible();
                }
            }
        }

        // CRITICAL FIX: New function to ensure participant section is visible
        function ensureParticipantSectionVisible() {
            let participantsSection = document.querySelector('.other-participants-section');
            if (!participantsSection && participantConnections.size > 0) {
                debugLog('🔧 Creating participant section as it does not exist');
                const mainContainer = document.querySelector('.main-container');
                participantsSection = document.createElement('div');
                participantsSection.className = 'other-participants-section';
                participantsSection.innerHTML = `
                    <h3>👥 Other Participants</h3>
                    <div class="participants-grid" id="participantsGrid"></div>
                `;
                mainContainer.appendChild(participantsSection);
            }

            if (participantsSection && participantConnections.size > 0) {
                participantsSection.style.display = 'block';
                debugLog('✅ Participant section is now visible');
            }
        }

        // Connect to participant with better error handling and retry logic
        function connectToParticipant(participantId, participantName) {
            if (!peer || !localStream || peer.destroyed) {
                debugLog('Cannot connect to participant - peer not ready');
                return;
            }

            debugLog('🤝 Connecting to participant: ' + participantId + ' (' + participantName + ')');

            try {
                const call = peer.call(participantId, localStream);

                if (call) {
                    // Set a timeout for the connection attempt
                    const connectionTimeout = setTimeout(() => {
                        if (!participantConnections.has(participantId)) {
                            debugLog('⏰ Connection timeout for: ' + participantId);
                            if (call && !call.open) {
                                call.close();
                            }
                        }
                    }, 10000); // 10 second timeout

                    call.on('stream', (remoteStream) => {
                        clearTimeout(connectionTimeout);
                        debugLog('✅ Connected to participant: ' + participantId);
                        handleParticipantStream(participantId, remoteStream, call);
                    });

                    call.on('error', (err) => {
                        clearTimeout(connectionTimeout);
                        debugLog('❌ Participant call error: ' + err.message);

                        // Remove any partial connection
                        if (participantConnections.has(participantId)) {
                            participantConnections.delete(participantId);
                        }
                    });

                    call.on('close', () => {
                        clearTimeout(connectionTimeout);
                        debugLog('📞 Participant call closed: ' + participantId);
                        removeParticipant(participantId);
                    });

                    // Store the connection attempt
                    participantConnections.set(participantId, { call, stream: null, name: participantName });

                } else {
                    debugLog('❌ Failed to create call to participant: ' + participantId);
                }
            } catch (err) {
                debugLog('❌ Error connecting to participant: ' + err.message);
            }
        }

        // Remove participant
        function removeParticipant(participantId) {
            const element = document.getElementById(`participant_${participantId}`);
            if (element) {
                element.remove();
                debugLog('✅ Removed participant UI element: ' + participantId);
            }

            participantConnections.delete(participantId);
            participantNames.delete(participantId);
            debugLog('✅ Removed participant from connections: ' + participantId);

            // Hide participants section if no more participants
            const grid = document.getElementById('participantsGrid');
            if (grid && grid.children.length === 0) {
                const participantsSection = document.querySelector('.other-participants-section');
                if (participantsSection) {
                    participantsSection.style.display = 'none';
                }
            }

            // Update status
            if (isConnected) {
                const remainingCount = participantConnections.size;
                if (remainingCount > 0) {
                    updateStatus(`🎉 Connected with expert and ${remainingCount} other participant(s)!`, 'success');
                } else {
                    updateStatus('🎉 Connected with expert! Waiting for other participants...', 'success');
                }
            }
        }

        // Handle network errors
        function handleNetworkError() {
            updateStatus('Network issue detected. Checking connection...', 'info');

            setTimeout(() => {
                if (!isConnected && connectionRetryCount < maxRetries) {
                    connectionRetryCount++;
                    debugLog(`Network retry attempt ${connectionRetryCount}/${maxRetries}`);
                    createPeerConnection();
                } else if (connectionRetryCount >= maxRetries) {
                    updateStatus('Unable to establish stable connection. Please check your internet and refresh.', 'error');
                }
            }, 5000);
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

        // End call function
        function endCall() {
            debugLog('Ending call...');

            // Clear participant discovery interval
            if (participantDiscoveryInterval) {
                clearInterval(participantDiscoveryInterval);
                participantDiscoveryInterval = null;
            }

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            if (expertCall) {
                expertCall.close();
            }

            participantConnections.forEach((connection) => {
                if (connection.call) {
                    connection.call.close();
                }
            });

            if (peer) {
                peer.destroy();
            }

            // Reset UI
            document.getElementById('videoCallInterface').style.display = 'none';
            document.getElementById('sessionInfo').style.display = 'none';
            document.getElementById('phoneEntrySection').style.display = 'block';
            document.getElementById('controlButtons').style.display = 'none';
            document.getElementById('joinBtn').disabled = false;
            document.getElementById('joinBtn').innerText = 'Connect with Expert';
            document.getElementById('phoneInput').value = '';
            document.getElementById('phoneInput').style.display = 'block';

            const participantsSection = document.querySelector('.other-participants-section');
            if (participantsSection) {
                participantsSection.remove();
            }

            participantConnections.clear();
            participantNames.clear();
            isConnected = false;
            connectionRetryCount = 0;
            lastParticipantCheck = [];

            updateStatus('Call ended. You can start a new session above.', 'info');
        }

        // Utility functions
        function updateStatus(message, type) {
            debugLog('Status (' + type + '): ' + message);

            const statusLabel = document.getElementById('<%= lblStatus.ClientID %>');
            if (statusLabel) {
                statusLabel.textContent = message;
                statusLabel.className = 'status-message ' + type;
            }
        }

        function getSessionId() {
            const urlParams = new URLSearchParams(window.location.search);
            const sessionFromUrl = urlParams.get('sessionId');
            if (sessionFromUrl) return sessionFromUrl;
            
            const sessionField = document.getElementById('<%= hdnSessionId.ClientID %>');
            if (sessionField && sessionField.value) return sessionField.value;
            
            return null;
        }

        function getCustomerPhone() {
            const phoneField = document.getElementById('<%= hdnCustomerPhone.ClientID %>') || 
                              document.getElementById('<%= hdnUserPhone.ClientID %>');
            if (phoneField && phoneField.value) return phoneField.value;

            return '';
        }

        // Cleanup on page unload
        window.onbeforeunload = function () {
            debugLog('Page unloading, cleaning up...');

            if (participantDiscoveryInterval) {
                clearInterval(participantDiscoveryInterval);
            }

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            if (peer && !peer.destroyed) {
                peer.destroy();
            }
        };
    </script>
</asp:Content>