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

        .join-btn:disabled {
            background: rgba(255,255,255,0.5);
            cursor: not-allowed;
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

        /* Multi-participant styles */
        .other-participants-section {
            background: white;
            border-radius: 15px;
            padding: 20px;
            margin-top: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .participants-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .participant-video-card {
            background: #f8f9fa;
            border: 2px solid #e9ecef;
            border-radius: 12px;
            padding: 12px;
        }

        .participant-video-card video {
            width: 100%;
            height: 150px;
            border-radius: 8px;
            background: #000;
            object-fit: cover;
        }

        @media (max-width: 768px) {
            .video-container {
                flex-direction: column;
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
                <video id="localVideo" autoplay="true" muted="true" playsinline="true"></video>
            </div>
            <div class="video-wrapper">
                <h3>Expert Video</h3>
                <video id="remoteVideo" autoplay="true" playsinline="true"></video>
            </div>
        </div>

        <!-- Control Panel -->
        <div class="control-panel">
            <asp:Label ID="lblStatus" runat="server" CssClass="status-message info" 
                Text="Enter your phone number above to join the expert consultation" />
            <div>
                <button id="endCallBtn" class="join-btn" type="button" onclick="endCall()" style="display: none; background: #dc3545;">End Call</button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Simplified User Video Call System
        let peer;
        let localStream;
        let currentCall;
        let expertCall;
        let isConnected = false;
        let sessionId = null;
        let customerPhone = '';
        let participantConnections = new Map();

        // Debug function
        function debugLog(message) {
            console.log('🔍 DEBUG:', message);
            updateStatus(message, 'info');
        }

        // Page load initialization
        window.onload = function () {
            debugLog('User video call page loaded');

            // Check for auto-join parameters
            sessionId = getSessionId();
            customerPhone = getCustomerPhone();

            debugLog('Session ID: ' + sessionId + ', Phone: ' + customerPhone);

            if (sessionId && customerPhone) {
                debugLog('Auto-joining session');
                document.getElementById('phoneInput').value = customerPhone;
                setTimeout(() => joinSession(), 1000);
            }
        };

        // Join session
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
                // Check session
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

                    // Store values
                    document.getElementById('<%= hdnSessionId.ClientID %>').value = sessionId;
                    document.getElementById('<%= hdnCustomerPhone.ClientID %>').value = phone;
                    document.getElementById('<%= hdnUserPhone.ClientID %>').value = phone;

                    // Hide phone entry, show session info
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

        // Initialize video
        async function initializeVideo() {
            try {
                debugLog('Getting user media...');

                // Get camera and microphone
                localStream = await navigator.mediaDevices.getUserMedia({
                    video: true,
                    audio: true
                });

                // Show local video
                const localVideo = document.getElementById('localVideo');
                if (localVideo) {
                    localVideo.srcObject = localStream;
                    debugLog('Local video stream set');
                }

                // Show video interface
                document.getElementById('videoCallInterface').style.display = 'flex';
                document.getElementById('endCallBtn').style.display = 'inline-block';

                debugLog('Creating peer connection...');
                await createPeerConnection();

            } catch (err) {
                debugLog('Media error: ' + err.message);
                updateStatus('Camera/microphone access required. Please allow permissions and refresh.', 'error');
            }
        }

        // Create peer connection
        async function createPeerConnection() {
            try {
                const cleanPhone = customerPhone.replace(/[^0-9]/g, '');
                const peerId = `customer_${cleanPhone}`;

                debugLog('Creating peer with ID: ' + peerId);

                peer = new Peer(peerId, {
                    host: '0.peerjs.com',
                    port: 443,
                    path: '/',
                    secure: true,
                    debug: 1
                });

                peer.on('open', (id) => {
                    debugLog('Peer connected with ID: ' + id);
                    updateStatus('Connected! Waiting for expert to join...', 'success');

                    // Try to connect to expert
                    setTimeout(() => connectToExpert(), 2000);
                });

                peer.on('call', (call) => {
                    debugLog('Incoming call from: ' + call.peer);
                    handleIncomingCall(call);
                });

                peer.on('error', (err) => {
                    debugLog('Peer error: ' + err.type + ' - ' + err.message);
                    updateStatus('Connection error: ' + err.message, 'error');
                });

                peer.on('disconnected', () => {
                    debugLog('Peer disconnected');
                    updateStatus('Connection lost. Attempting to reconnect...', 'info');
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
                return;
            }

            const expertId = `expert_session_${sessionId}`;
            debugLog('Attempting to call expert: ' + expertId);

            try {
                const call = peer.call(expertId, localStream);

                if (call) {
                    expertCall = call;

                    call.on('stream', (remoteStream) => {
                        debugLog('Received expert video stream');
                        handleExpertStream(remoteStream);
                    });

                    call.on('error', (err) => {
                        debugLog('Expert call error: ' + err.message);
                        updateStatus('Expert not available. Retrying...', 'info');
                        setTimeout(() => connectToExpert(), 5000);
                    });

                    call.on('close', () => {
                        debugLog('Expert call ended');
                        handleExpertDisconnect();
                    });
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

            // Answer the call
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
                isConnected = true;
                updateStatus('🎉 Connected with expert! Video consultation is active.', 'success');

                // Start looking for other participants
                setTimeout(() => discoverParticipants(), 3000);
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

            // Try to reconnect
            setTimeout(() => connectToExpert(), 3000);
        }

        // Handle participant stream
        function handleParticipantStream(participantId, stream, call) {
            debugLog('Adding participant: ' + participantId);

            participantConnections.set(participantId, { call, stream });
            addParticipantToUI(participantId, stream);
        }

        // Add participant to UI
        function addParticipantToUI(participantId, stream) {
            // Create participants section if it doesn't exist
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
            }

            // Add participant video
            const grid = document.getElementById('participantsGrid');
            const participantDiv = document.createElement('div');
            participantDiv.className = 'participant-video-card';
            participantDiv.id = `participant_${participantId}`;

            const participantName = participantId.replace('customer_', 'Participant ');
            participantDiv.innerHTML = `
                <div style="margin-bottom: 10px;">
                    <h4 style="margin: 0; color: #051D40;">👤 ${participantName}</h4>
                </div>
                <video autoplay="true" playsinline="true"></video>
            `;

            grid.appendChild(participantDiv);

            // Set video stream
            const video = participantDiv.querySelector('video');
            if (video) {
                video.srcObject = stream;
            }

            debugLog('Participant added to UI: ' + participantName);
        }

        // Discover other participants
        async function discoverParticipants() {
            if (!sessionId || !isConnected) return;

            try {
                debugLog('Discovering other participants...');

                const response = await fetch('/Staff/StaffVideoCall.aspx/GetSessionParticipants', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ sessionId: parseInt(sessionId) })
                });

                const data = await response.json();
                const result = JSON.parse(data.d);

                if (result.success && result.participants) {
                    const myPhone = customerPhone.replace(/[^0-9]/g, '');

                    result.participants.forEach(participant => {
                        const cleanPhone = participant.phone.replace(/[^0-9]/g, '');

                        if (cleanPhone !== myPhone) {
                            const participantId = `customer_${cleanPhone}`;

                            if (!participantConnections.has(participantId)) {
                                setTimeout(() => {
                                    connectToParticipant(participantId);
                                }, Math.random() * 2000);
                            }
                        }
                    });
                }
            } catch (error) {
                debugLog('Error discovering participants: ' + error.message);
            }
        }

        // Connect to participant
        function connectToParticipant(participantId) {
            if (!peer || !localStream) return;

            debugLog('Connecting to participant: ' + participantId);

            try {
                const call = peer.call(participantId, localStream);

                if (call) {
                    call.on('stream', (remoteStream) => {
                        debugLog('Connected to participant: ' + participantId);
                        handleParticipantStream(participantId, remoteStream, call);
                    });

                    call.on('error', (err) => {
                        debugLog('Participant call error: ' + err.message);
                    });
                }
            } catch (err) {
                debugLog('Error connecting to participant: ' + err.message);
            }
        }

        // Remove participant
        function removeParticipant(participantId) {
            const element = document.getElementById(`participant_${participantId}`);
            if (element) {
                element.remove();
            }
            participantConnections.delete(participantId);
            debugLog('Removed participant: ' + participantId);
        }

        // End call
        function endCall() {
            debugLog('Ending call...');

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            if (expertCall) {
                expertCall.close();
            }

            participantConnections.forEach((connection) => {
                connection.call.close();
            });

            if (peer) {
                peer.destroy();
            }

            // Reset UI
            document.getElementById('videoCallInterface').style.display = 'none';
            document.getElementById('sessionInfo').style.display = 'none';
            document.getElementById('phoneEntrySection').style.display = 'block';
            document.getElementById('joinBtn').disabled = false;
            document.getElementById('joinBtn').innerText = 'Connect with Expert';
            document.getElementById('phoneInput').value = '';

            updateStatus('Call ended. You can start a new session above.', 'info');
        }

        // Utility functions
        function updateStatus(message, type) {
            console.log('📱 Status (' + type + '): ' + message);

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

        // Periodic participant discovery
        setInterval(() => {
            if (isConnected && sessionId) {
                discoverParticipants();
            }
        }, 10000);

        // Cleanup on page unload
        window.onbeforeunload = function () {
            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }
        };
    </script>
</asp:Content>