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

        .phone-entry-container h2 {
            color: white;
            margin-bottom: 15px;
            font-size: 1.8rem;
            font-weight: 600;
        }

        .phone-entry-container p {
            color: white;
            margin-bottom: 25px;
            opacity: 0.9;
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
            backdrop-filter: blur(10px);
        }

        .phone-input::placeholder {
            color: rgba(255,255,255,0.7);
        }

        .phone-input:focus {
            outline: none;
            border-color: white;
            background: rgba(255,255,255,0.2);
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
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
        }

        .join-btn:disabled {
            background: rgba(255,255,255,0.5);
            cursor: not-allowed;
            transform: none;
        }

        .video-container {
            display: none;
            gap: 20px;
            padding: 20px;
            justify-content: center;
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
            border-radius: 10px;
            background: #000;
        }

        .session-info {
            margin-bottom: 20px;
            padding: 25px;
            background: #f8f9fa;
            border-radius: 15px;
            display: none;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
        }

        .session-info h2 {
            color: #051D40;
            margin-bottom: 20px;
            font-size: 1.5rem;
            font-weight: 600;
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

        .customer-details {
            line-height: 1.8;
            text-align: left;
        }

        .customer-details p {
            margin: 12px 0;
            color: #333;
        }

        .customer-details strong {
            color: #D36F2D;
            min-width: 140px;
            display: inline-block;
            font-weight: 600;
        }

        .end-call-btn {
            background-color: #dc3545;
            color: white;
            padding: 12px 25px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 500;
            margin-top: 15px;
            transition: background-color 0.3s;
        }

        .end-call-btn:hover {
            background-color: #c82333;
        }

        .debug-info {
            background: #f8f9fa;
            padding: 15px;
            margin: 15px 0;
            border-radius: 8px;
            font-family: monospace;
            font-size: 12px;
            max-height: 200px;
            overflow-y: auto;
            text-align: left;
            border-left: 4px solid #D36F2D;
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
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        @media (max-width: 768px) {
            .phone-input {
                max-width: 100%;
            }
            
            .video-container {
                flex-direction: column;
            }
            
            .customer-details strong {
                min-width: auto;
                display: block;
                margin-bottom: 5px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main-container">
        <asp:HiddenField ID="hdnSessionId" runat="server" />
        <asp:HiddenField ID="hdnCustomerPhone" runat="server" />
        
        <!-- Phone Entry Section -->
        <div id="phoneEntrySection" class="phone-entry-container">
            <h2>Join Expert Consultation</h2>
            <p>Enter your phone number to connect with our scam prevention expert</p>
            <div class="expert-consultation-badge">🛡️ Expert Video Consultation</div>
            <input type="tel" id="phoneInput" class="phone-input" placeholder="Enter your phone number" />
            <br />
            <button id="joinBtn" class="join-btn" onclick="joinSession()">Connect with Expert</button>
            <div id="phoneStatus" class="status-message" style="display: none;"></div>
        </div>

        <!-- Session Info (shown after joining) -->
        <div id="sessionInfo" class="session-info">
            <h2>Expert Consultation Session</h2>
            <div class="expert-consultation-badge">🎯 Live with Scam Prevention Expert</div>
            <asp:Label ID="lblSessionInfo" runat="server" />
        </div>

        <!-- Video Call Interface -->
        <div id="videoCallInterface" class="video-container">
            <div class="video-wrapper">
                <h3>Your Video</h3>
                <video id="localVideo" autoplay muted playsinline></video>
            </div>
            <div class="video-wrapper">
                <h3>Expert Video</h3>
                <video id="remoteVideo" autoplay playsinline></video>
            </div>
        </div>

        <div class="control-panel">
            <asp:Label ID="lblStatus" runat="server" CssClass="status-message info" />
            <div style="display: none;">
                <button id="endCallBtn" class="end-call-btn" onclick="endCall()">End Call</button>
            </div>
            <div id="debugInfo" class="debug-info" style="display: none;"></div>
        </div>
    </div>

    <script type="text/javascript">
        let peer;
        let currentCall;
        let localStream;
        let isConnectionActive = false;
        let sessionId = null;
        let customerPhone = null;

        function updateDebugInfo(message) {
            const debugDiv = document.getElementById('debugInfo');
            debugDiv.style.display = 'block';
            const timestamp = new Date().toLocaleTimeString();
            debugDiv.innerHTML += `${timestamp}: ${message}<br>`;
            debugDiv.scrollTop = debugDiv.scrollHeight;
        }

        function showStatus(message, type = 'info') {
            const statusElement = document.getElementById('<%= lblStatus.ClientID %>');
            statusElement.innerText = message;
            statusElement.className = `status-message ${type}`;

            const phoneStatus = document.getElementById('phoneStatus');
            phoneStatus.innerText = message;
            phoneStatus.className = `status-message ${type}`;
            phoneStatus.style.display = 'block';
        }

        async function joinSession() {
            const phoneInput = document.getElementById('phoneInput');
            const phone = phoneInput.value.trim();

            if (!phone) {
                showStatus('Please enter your phone number', 'error');
                return;
            }

            // Validate and format phone number
            const formattedPhone = formatPhoneNumber(phone);
            if (!formattedPhone) {
                showStatus('Please enter a valid phone number', 'error');
                return;
            }

            customerPhone = formattedPhone;
            document.getElementById('<%= hdnCustomerPhone.ClientID %>').value = customerPhone;

            // Disable join button
            document.getElementById('joinBtn').disabled = true;
            showStatus('Looking for your expert consultation...', 'info');

            try {
                // Check if session exists with this phone number
                const sessionData = await checkSessionExists(customerPhone);
                if (sessionData.success) {
                    sessionId = sessionData.sessionId;
                    document.getElementById('<%= hdnSessionId.ClientID %>').value = sessionId;
                    
                    // Hide phone entry, show session info
                    document.getElementById('phoneEntrySection').style.display = 'none';
                    document.getElementById('sessionInfo').style.display = 'block';
                    
                    // Load session details
                    loadSessionDetails(sessionData);
                    
                    // Initialize video call
                    await initializeCall();
                } else {
                    showStatus(sessionData.message || 'No active expert session found with this phone number', 'error');
                    document.getElementById('joinBtn').disabled = false;
                }
            } catch (error) {
                updateDebugInfo(`Error joining session: ${error.message}`);
                showStatus('Error connecting to session. Please try again.', 'error');
                document.getElementById('joinBtn').disabled = false;
            }
        }

        function formatPhoneNumber(phone) {
            // Remove all non-digit characters
            const cleaned = phone.replace(/\D/g, '');
            
            // Check for valid length (assuming 8-15 digits)
            if (cleaned.length < 8 || cleaned.length > 15) {
                return null;
            }
            
            // Format as needed (this example assumes Singapore format)
            if (cleaned.length === 8) {
                return `+65${cleaned}`;
            } else if (cleaned.startsWith('65') && cleaned.length === 10) {
                return `+${cleaned}`;
            } else if (cleaned.startsWith('0')) {
                return `+65${cleaned.substring(1)}`;
            }
            
            return `+${cleaned}`;
        }

        async function checkSessionExists(phone) {
            try {
                const response = await fetch('VideoCall.aspx/CheckSession', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ phoneNumber: phone })
                });
                
                const data = await response.json();
                return JSON.parse(data.d);
            } catch (error) {
                updateDebugInfo(`Check session error: ${error.message}`);
                return { success: false, message: 'Connection error' };
            }
        }

        function loadSessionDetails(sessionData) {
            const sessionInfo = `
                <div class='customer-details'>
                    <p><strong>Session ID:</strong> ${sessionData.sessionId}</p>
                    <p><strong>Expert:</strong> ${sessionData.staffName || 'Scam Prevention Specialist'}</p>
                    <p><strong>Session Date:</strong> ${sessionData.sessionDate || 'Today'}</p>
                    <p><strong>Session Time:</strong> ${sessionData.sessionTime || 'Current'}</p>
                    <p><strong>Consultation Type:</strong> Expert Video Consultation</p>
                    <p><strong>Focus Area:</strong> Scam Prevention & Safety Education</p>
                </div>`;
                
            document.getElementById('<%= lblSessionInfo.ClientID %>').innerHTML = sessionInfo;
        }

        async function initializeCall() {
            try {
                updateDebugInfo('Starting customer initialization...');

                localStream = await navigator.mediaDevices.getUserMedia({
                    video: true,
                    audio: true
                });

                updateDebugInfo('Got media stream');
                document.getElementById('localVideo').srcObject = localStream;

                // Create peer ID using phone number
                const peerId = `customer_${customerPhone.replace(/[^0-9]/g, '')}`;
                updateDebugInfo(`Customer Peer ID: ${peerId}`);

                peer = new Peer(peerId, {
                    host: 'localhost',
                    port: 3001,
                    path: '/',
                    debug: 3
                });

                peer.on('open', (id) => {
                    updateDebugInfo(`Peer connection opened with ID: ${id}`);
                    showStatus('Connected. Waiting for expert to join...', 'success');
                    isConnectionActive = true;

                    // Show video interface
                    document.getElementById('videoCallInterface').style.display = 'flex';
                    document.getElementById('endCallBtn').parentElement.style.display = 'block';
                });

                peer.on('call', (call) => {
                    updateDebugInfo('Receiving call from staff');
                    call.answer(localStream);
                    setupCall(call);
                });

                peer.on('error', (err) => {
                    updateDebugInfo(`Peer error: ${err.type} - ${err.message}`);
                    isConnectionActive = false;
                    let message = 'Connection error: ';

                    switch (err.type) {
                        case 'invalid-id':
                            message += 'Please refresh and try again';
                            break;
                        case 'peer-unavailable':
                            message += 'Expert not available yet';
                            break;
                        case 'network':
                            message += 'Network connection issue';
                            break;
                        default:
                            message += err.message;
                    }

                    showStatus(message, 'error');
                });

                peer.on('disconnected', () => {
                    updateDebugInfo('Peer disconnected, attempting reconnection...');
                    isConnectionActive = false;
                    showStatus('Connection lost. Attempting to reconnect...', 'info');
                    peer.reconnect();
                });

            } catch (err) {
                updateDebugInfo(`Setup error: ${err.message}`);
                showStatus('Error accessing camera/microphone. Please ensure they are connected and permitted.', 'error');
            }
        }

        function setupCall(call) {
            currentCall = call;
            updateDebugInfo('Setting up call');

            call.on('stream', (remoteStream) => {
                updateDebugInfo('Received remote stream from expert');
                document.getElementById('remoteVideo').srcObject = remoteStream;
                showStatus('Connected with your scam prevention expert! 🛡️', 'success');
            });

            call.on('close', () => {
                updateDebugInfo('Call closed');
                document.getElementById('remoteVideo').srcObject = null;
                showStatus('Consultation ended by expert', 'info');
            });

            call.on('error', (err) => {
                updateDebugInfo(`Call error: ${err.message}`);
                showStatus('Call error: ' + err.message, 'error');
            });
        }

        function endCall() {
            if (currentCall) {
                updateDebugInfo('Ending current call');
                currentCall.close();
                currentCall = null;
            }

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            if (peer && isConnectionActive) {
                peer.destroy();
            }

            document.getElementById('remoteVideo').srcObject = null;
            showStatus('Consultation ended', 'info');

            // Optionally redirect back to webinar page
            setTimeout(() => {
                window.location.href = 'UserWebinarSessionListing.aspx';
            }, 2000);
        }

        window.onbeforeunload = function () {
            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }
            if (currentCall) {
                currentCall.close();
            }
            if (peer && isConnectionActive) {
                peer.destroy();
            }
        };

        // Auto-focus phone input when page loads
        window.onload = function () {
            document.getElementById('phoneInput').focus();
        };
    </script>
</asp:Content>