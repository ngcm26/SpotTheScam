<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="StaffVideoCall.aspx.cs" Inherits="SpotTheScam.Staff.StaffVideoCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/peerjs/1.4.7/peerjs.min.js"></script>
    <style>
        .main-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        .phone-lookup-container {
            background: #f8f9fa;
            padding: 30px;
            border-radius: 15px;
            text-align: center;
            margin-bottom: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .phone-input {
            width: 100%;
            max-width: 300px;
            padding: 15px;
            font-size: 18px;
            border: 2px solid #ddd;
            border-radius: 8px;
            margin: 15px 0;
            text-align: center;
        }

        .phone-input:focus {
            outline: none;
            border-color: #D36F2D;
        }

        .connect-btn {
            background-color: #D36F2D;
            color: white;
            padding: 15px 30px;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.3s;
            margin: 0 10px;
        }

        .connect-btn:hover {
            background-color: #b45a22;
        }

        .connect-btn:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }

        .waiting-customers {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
        }

        .customer-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px;
            margin: 10px 0;
            background: white;
            border-radius: 8px;
            border-left: 4px solid #D36F2D;
        }

        .customer-info {
            flex-grow: 1;
        }

        .customer-phone {
            font-weight: bold;
            font-size: 16px;
            color: #333;
        }

        .customer-waiting-time {
            font-size: 14px;
            color: #666;
        }

        .quick-connect-btn {
            background-color: #28a745;
            color: white;
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .quick-connect-btn:hover {
            background-color: #218838;
        }

        .video-container {
            display: none;
            gap: 20px;
            padding: 20px;
            justify-content: center;
            margin-bottom: 20px;
        }

        .video-wrapper {
            background: #f5f5f5;
            padding: 15px;
            border-radius: 8px;
            text-align: center;
            flex: 1;
            max-width: 500px;
        }

        video {
            width: 100%;
            max-width: 480px;
            border-radius: 8px;
            background: #000;
        }

        .session-info {
            margin-bottom: 20px;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 8px;
            display: none;
        }

        .control-panel {
            text-align: center;
            margin: 20px 0;
            padding: 15px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .btn-control {
            padding: 10px 20px;
            margin: 0 10px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            background-color: #D36F2D;
            color: white;
            font-weight: 500;
            transition: background-color 0.3s;
        }

        .btn-control:hover {
            background-color: #b45a22;
        }

        .status-message {
            padding: 10px;
            margin: 10px 0;
            border-radius: 4px;
            text-align: center;
            font-weight: 500;
            display: block;
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
            line-height: 1.6;
        }

        .customer-details p {
            margin: 10px 0;
        }

        .customer-details strong {
            color: #666;
            min-width: 120px;
            display: inline-block;
        }

        h3 {
            color: #333;
            margin-bottom: 15px;
        }

        .debug-info {
            background: #f8f9fa;
            padding: 10px;
            margin: 10px 0;
            border-radius: 4px;
            font-family: monospace;
            font-size: 12px;
            max-height: 200px;
            overflow-y: auto;
            text-align: left;
        }

        .refresh-btn {
            background-color: #6c757d;
            color: white;
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            margin-left: 10px;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main-container">
        <asp:HiddenField ID="hdnSessionId" runat="server" />
        <asp:HiddenField ID="hdnCustomerPhone" runat="server" />
        
        <!-- Phone Lookup Section -->
        <div id="phoneLookupSection" class="phone-lookup-container">
            <h2>Connect with Webinar Participants</h2>
            <p>Enter a participant's phone number to start a video consultation</p>
            <input type="tel" id="phoneInput" class="phone-input" placeholder="Enter participant's phone number" />
            <br />
            <button id="connectBtn" class="connect-btn" onclick="connectToCustomer()">Connect to Participant</button>
            <button id="refreshBtn" class="refresh-btn" onclick="refreshWaitingList()">Refresh List</button>
            <div id="phoneStatus" class="status-message" style="display: none;"></div>
        </div>

        <!-- Waiting Customers Section -->
        <div id="waitingCustomersSection" class="waiting-customers">
            <h3>Participants Currently Online</h3>
            <div id="waitingList">
                <!-- This will be populated dynamically -->
                <div class="customer-item" style="display: none;">
                    <div class="customer-info">
                        <div class="customer-phone">No participants currently waiting</div>
                        <div class="customer-waiting-time">Check back in a few minutes</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Session Info (shown after connecting) -->
        <div id="sessionInfo" class="session-info">
            <h2>Expert Consultation Session</h2>
            <asp:Label ID="lblCustomerInfo" runat="server" />
        </div>

        <!-- Video Call Interface -->
        <div id="videoCallInterface" class="video-container">
            <div class="video-wrapper">
                <h3>Your Video</h3>
                <video id="localVideo" autoplay muted playsinline></video>
            </div>
            <div class="video-wrapper">
                <h3>Participant Video</h3>
                <video id="remoteVideo" autoplay playsinline></video>
            </div>
        </div>

        <div class="control-panel">
            <button type="button" class="btn-control" onclick="startCall()" id="startCallBtn" style="display: none;">Start Call</button>
            <button type="button" class="btn-control" onclick="endCall()" id="endCallBtn" style="display: none;">End Consultation</button>
            <asp:Label ID="lblStatus" runat="server" CssClass="status-message" />
            <div id="debugInfo" class="debug-info" style="display: none;"></div>
        </div>
    </div>

    <script type="text/javascript">
        let peer;
        let currentCall;
        let localStream;
        let isConnectionActive = false;
        let currentCustomerPhone = null;
        let sessionId = null;
        let refreshInterval;

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

        async function connectToCustomer() {
            const phoneInput = document.getElementById('phoneInput');
            const phone = phoneInput.value.trim();

            if (!phone) {
                showStatus('Please enter a participant phone number', 'error');
                return;
            }

            // Format phone number
            const formattedPhone = formatPhoneNumber(phone);
            if (!formattedPhone) {
                showStatus('Please enter a valid phone number', 'error');
                return;
            }

            currentCustomerPhone = formattedPhone;
            document.getElementById('<%= hdnCustomerPhone.ClientID %>').value = currentCustomerPhone;

            // Disable connect button
            document.getElementById('connectBtn').disabled = true;
            showStatus('Looking for participant session...', 'info');

            try {
                // Check if customer session exists
                const sessionData = await checkCustomerSession(currentCustomerPhone);
                if (sessionData.success) {
                    sessionId = sessionData.sessionId;
                    document.getElementById('<%= hdnSessionId.ClientID %>').value = sessionId;
                    
                    // Hide phone lookup, show session info
                    document.getElementById('phoneLookupSection').style.display = 'none';
                    document.getElementById('waitingCustomersSection').style.display = 'none';
                    document.getElementById('sessionInfo').style.display = 'block';
                    
                    // Load session details
                    loadSessionDetails(sessionData);
                    
                    // Initialize video call
                    await initializeCall();
                } else {
                    showStatus(sessionData.message || 'No active participant session found with this phone number', 'error');
                    document.getElementById('connectBtn').disabled = false;
                }
            } catch (error) {
                updateDebugInfo(`Error connecting to participant: ${error.message}`);
                showStatus('Error connecting to participant session. Please try again.', 'error');
                document.getElementById('connectBtn').disabled = false;
            }
        }

        function formatPhoneNumber(phone) {
            // Remove all non-digit characters
            const cleaned = phone.replace(/\D/g, '');
            
            // Check for valid length
            if (cleaned.length < 8 || cleaned.length > 15) {
                return null;
            }
            
            // Format as needed (Singapore format)
            if (cleaned.length === 8) {
                return `+65${cleaned}`;
            } else if (cleaned.startsWith('65') && cleaned.length === 10) {
                return `+${cleaned}`;
            } else if (cleaned.startsWith('0')) {
                return `+65${cleaned.substring(1)}`;
            }
            
            return `+${cleaned}`;
        }

        async function checkCustomerSession(phone) {
            try {
                const response = await fetch('StaffVideoCall.aspx/CheckCustomerSession', {
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
                    <p><strong>Participant:</strong> ${sessionData.customerName || 'Webinar Participant'}</p>
                    <p><strong>Phone Number:</strong> ${sessionData.customerPhone}</p>
                    <p><strong>Session Date:</strong> ${sessionData.sessionDate || new Date().toLocaleDateString()}</p>
                    <p><strong>Session Time:</strong> ${sessionData.sessionTime || new Date().toLocaleTimeString()}</p>
                    <p><strong>Type:</strong> Expert Video Consultation</p>
                    <p><strong>Concerns:</strong> ${sessionData.concerns || 'General scam prevention inquiry'}</p>
                </div>`;
                
            document.getElementById('<%= lblCustomerInfo.ClientID %>').innerHTML = sessionInfo;
        }

        async function initializeCall() {
            try {
                updateDebugInfo('Starting expert initialization...');

                localStream = await navigator.mediaDevices.getUserMedia({
                    video: true,
                    audio: true
                });

                updateDebugInfo('Got media stream');
                document.getElementById('localVideo').srcObject = localStream;

                // Create peer ID using expert identifier
                const peerId = `expert_${currentCustomerPhone.replace(/[^0-9]/g, '')}`;
                updateDebugInfo(`Expert Peer ID: ${peerId}`);

                peer = new Peer(peerId, {
                    host: 'localhost',
                    port: 3001,
                    path: '/',
                    debug: 3
                });

                peer.on('open', (id) => {
                    updateDebugInfo(`Peer connection opened with ID: ${id}`);
                    showStatus('Connected. Ready to start consultation.', 'success');
                    isConnectionActive = true;

                    // Show video interface and controls
                    document.getElementById('videoCallInterface').style.display = 'flex';
                    document.getElementById('startCallBtn').style.display = 'inline-block';
                    document.getElementById('endCallBtn').style.display = 'inline-block';
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
                            message += 'Participant not connected yet';
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

        function startCall() {
            if (!currentCustomerPhone) {
                showStatus('No participant phone number available', 'error');
                return;
            }

            const customerId = `customer_${currentCustomerPhone.replace(/[^0-9]/g, '')}`;
            updateDebugInfo(`Attempting to call participant: ${customerId}`);

            if (peer && localStream && isConnectionActive) {
                showStatus('Calling participant...', 'info');

                const call = peer.call(customerId, localStream);
                setupCall(call);
            } else {
                updateDebugInfo('Cannot start call - connection not ready');
                showStatus('Cannot start call - please refresh the page', 'error');
            }
        }

        function setupCall(call) {
            currentCall = call;
            updateDebugInfo('Setting up call object');

            call.on('stream', (remoteStream) => {
                updateDebugInfo('Received participant stream');
                document.getElementById('remoteVideo').srcObject = remoteStream;
                showStatus('Connected with participant - consultation in progress', 'success');

                // Hide start button, show end button
                document.getElementById('startCallBtn').style.display = 'none';
                document.getElementById('endCallBtn').style.display = 'inline-block';
            });

            call.on('close', () => {
                updateDebugInfo('Call closed');
                document.getElementById('remoteVideo').srcObject = null;
                showStatus('Consultation ended', 'info');
                resetInterface();
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

            resetInterface();
        }

        function resetInterface() {
            // Show phone lookup section again
            document.getElementById('phoneLookupSection').style.display = 'block';
            document.getElementById('waitingCustomersSection').style.display = 'block';
            document.getElementById('sessionInfo').style.display = 'none';
            document.getElementById('videoCallInterface').style.display = 'none';
            document.getElementById('startCallBtn').style.display = 'none';
            document.getElementById('endCallBtn').style.display = 'none';

            // Reset form
            document.getElementById('phoneInput').value = '';
            document.getElementById('connectBtn').disabled = false;

            // Clear variables
            currentCustomerPhone = null;
            sessionId = null;

            // Restart waiting list refresh
            startWaitingListRefresh();
        }

        async function refreshWaitingList() {
            try {
                const response = await fetch('StaffVideoCall.aspx/GetWaitingParticipants', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                const data = await response.json();
                const participants = JSON.parse(data.d);

                const waitingList = document.getElementById('waitingList');
                waitingList.innerHTML = '';

                if (participants && participants.length > 0) {
                    participants.forEach(participant => {
                        const participantDiv = document.createElement('div');
                        participantDiv.className = 'customer-item';
                        participantDiv.innerHTML = `
                            <div class="customer-info">
                                <div class="customer-phone">${participant.phone}</div>
                                <div class="customer-waiting-time">Waiting since ${participant.waitingTime}</div>
                                <div style="font-size: 12px; color: #666;">
                                    Concerns: ${participant.concerns || 'General inquiry'}
                                </div>
                            </div>
                            <button class="quick-connect-btn" onclick="quickConnect('${participant.phone}')">
                                Connect Now
                            </button>
                        `;
                        waitingList.appendChild(participantDiv);
                    });
                } else {
                    waitingList.innerHTML = `
                        <div class="customer-item">
                            <div class="customer-info">
                                <div class="customer-phone">No participants currently waiting</div>
                                <div class="customer-waiting-time">Check back in a few minutes</div>
                            </div>
                        </div>
                    `;
                }
            } catch (error) {
                updateDebugInfo(`Error refreshing waiting list: ${error.message}`);
            }
        }

        function quickConnect(phone) {
            document.getElementById('phoneInput').value = phone;
            connectToCustomer();
        }

        function startWaitingListRefresh() {
            // Refresh waiting list every 30 seconds
            if (refreshInterval) {
                clearInterval(refreshInterval);
            }

            refreshInterval = setInterval(() => {
                if (document.getElementById('phoneLookupSection').style.display !== 'none') {
                    refreshWaitingList();
                }
            }, 30000);

            // Initial refresh
            refreshWaitingList();
        }

        window.onload = function () {
            startWaitingListRefresh();
            document.getElementById('phoneInput').focus();
        };

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
            if (refreshInterval) {
                clearInterval(refreshInterval);
            }
        };
    </script>
</asp:Content>