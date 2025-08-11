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
        // Enhanced JavaScript for User VideoCall.aspx - Replace the existing script section

        let peer;
        let currentCall;
        let localStream;
        let isConnectionActive = false;
        let sessionId = null;
        let customerPhone = null;

        function updateDebugInfo(message) {
            const debugDiv = document.getElementById('debugInfo');
            if (debugDiv) {
                debugDiv.style.display = 'block';
                const timestamp = new Date().toLocaleTimeString();
                debugDiv.innerHTML += `${timestamp}: ${message}<br>`;
                debugDiv.scrollTop = debugDiv.scrollHeight;
            }
            console.log('🔧 DEBUG:', message);
        }

        function showStatus(message, type = 'info') {
            // Update main status label
            const statusElement = document.getElementById('<%= lblStatus.ClientID %>');
            if (statusElement) {
                statusElement.innerText = message;
                statusElement.className = `status-message ${type}`;
            }

            // Update phone status
            const phoneStatus = document.getElementById('phoneStatus');
            if (phoneStatus) {
                phoneStatus.innerText = message;
                phoneStatus.className = `status-message ${type}`;
                phoneStatus.style.display = 'block';
            }

            console.log(`📱 Customer status (${type}):`, message);
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
            const hdnCustomerPhone = document.getElementById('<%= hdnCustomerPhone.ClientID %>');
    if (hdnCustomerPhone) {
        hdnCustomerPhone.value = customerPhone;
    }

    // Disable join button
    const joinBtn = document.getElementById('joinBtn');
    if (joinBtn) {
        joinBtn.disabled = true;
    }

    showStatus('Looking for your expert consultation...', 'info');
    updateDebugInfo(`Checking session for phone: ${customerPhone}`);

    try {
        // Check if session exists with this phone number
        const sessionData = await checkSessionExists(customerPhone);
        if (sessionData.success) {
            sessionId = sessionData.sessionId;
            const hdnSessionId = document.getElementById('<%= hdnSessionId.ClientID %>');
            if (hdnSessionId) {
                hdnSessionId.value = sessionId;
            }
            
            updateDebugInfo(`Session found: ${sessionId}`);
            
            // FIXED: Notify staff immediately when participant joins
            await notifyStaffParticipantJoined();
            
            // Hide phone entry, show session info
            const phoneEntrySection = document.getElementById('phoneEntrySection');
            const sessionInfoDiv = document.getElementById('sessionInfo');
            
            if (phoneEntrySection) phoneEntrySection.style.display = 'none';
            if (sessionInfoDiv) sessionInfoDiv.style.display = 'block';
            
            // Load session details
            loadSessionDetails(sessionData);
            
            // Initialize video call
            await initializeCall();
        } else {
            showStatus(sessionData.message || 'No active expert session found with this phone number', 'error');
            if (joinBtn) {
                joinBtn.disabled = false;
            }
        }
    } catch (error) {
        updateDebugInfo(`Error joining session: ${error.message}`);
        showStatus('Error connecting to session. Please try again.', 'error');
        if (joinBtn) {
            joinBtn.disabled = false;
        }
    }
}

// FIXED: Notify staff when participant joins
async function notifyStaffParticipantJoined() {
    if (!sessionId || !customerPhone) return;
    
    try {
        const response = await fetch('VideoCall.aspx/NotifyStaffParticipantJoined', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ 
                sessionId: sessionId, 
                phoneNumber: customerPhone,
                participantName: 'Participant'
            })
        });
        
        const data = await response.json();
        const result = JSON.parse(data.d);
        
        if (result.success) {
            updateDebugInfo('✅ Staff notified of participant connection');
        }
    } catch (error) {
        updateDebugInfo(`⚠️ Failed to notify staff: ${error.message}`);
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
        updateDebugInfo(`Making request to check session for phone: ${phone}`);
        
        const response = await fetch('VideoCall.aspx/CheckSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ phoneNumber: phone })
        });
        
        const data = await response.json();
        const result = JSON.parse(data.d);
        
        updateDebugInfo(`Session check result: ${JSON.stringify(result)}`);
        return result;
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
            <p><strong>Status:</strong> <span style="color: #28a745; font-weight: 600;">✅ Connected & Ready</span></p>
        </div>`;
        
    const lblSessionInfo = document.getElementById('<%= lblSessionInfo.ClientID %>');
    if (lblSessionInfo) {
        lblSessionInfo.innerHTML = sessionInfo;
    }

    updateDebugInfo('Session details loaded and displayed');
}

async function initializeCall() {
    try {
        updateDebugInfo('Starting customer initialization...');

        // Get user media first
        localStream = await navigator.mediaDevices.getUserMedia({
            video: true,
            audio: true
        });

        updateDebugInfo('Got media stream successfully');

        const localVideo = document.getElementById('localVideo');
        if (localVideo) {
            localVideo.srcObject = localStream;
        }

        // Create peer ID using phone number
        const peerId = `customer_${customerPhone.replace(/[^0-9]/g, '')}`;
        updateDebugInfo(`Creating customer peer with ID: ${peerId}`);

        peer = new Peer(peerId, {
            host: 'localhost',
            port: 3001,
            path: '/',
            debug: 3
        });

        peer.on('open', (id) => {
            updateDebugInfo(`Peer connection opened with ID: ${id}`);
            showStatus('🟢 Connected! Waiting for expert to join...', 'success');
            isConnectionActive = true;

            // Show video interface
            const videoCallInterface = document.getElementById('videoCallInterface');
            const endCallBtnParent = document.getElementById('endCallBtn')?.parentElement;

            if (videoCallInterface) {
                videoCallInterface.style.display = 'flex';
            }
            if (endCallBtnParent) {
                endCallBtnParent.style.display = 'block';
            }
        });

        peer.on('call', (call) => {
            updateDebugInfo('📞 Receiving call from expert');
            call.answer(localStream);
            setupCall(call);
            
            // Update status to show expert connection
            showStatus('🎯 Expert connected! Session in progress...', 'success');
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
                    message += 'Expert not available yet. Please wait...';
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

            // Try to reconnect
            setTimeout(() => {
                if (peer && !peer.destroyed) {
                    peer.reconnect();
                }
            }, 2000);
        });

    } catch (err) {
        updateDebugInfo(`Setup error: ${err.message}`);
        showStatus('Error accessing camera/microphone. Please ensure they are connected and permitted.', 'error');
    }
}

function setupCall(call) {
    currentCall = call;
    updateDebugInfo('Setting up call with expert');

    call.on('stream', (remoteStream) => {
        updateDebugInfo('Received remote stream from expert');

        const remoteVideo = document.getElementById('remoteVideo');
        if (remoteVideo) {
            remoteVideo.srcObject = remoteStream;
        }

        showStatus('🛡️ Connected with your scam prevention expert!', 'success');
        updateDebugInfo('✅ Video call established successfully');
    });

    call.on('close', () => {
        updateDebugInfo('Call ended by expert');

        const remoteVideo = document.getElementById('remoteVideo');
        if (remoteVideo) {
            remoteVideo.srcObject = null;
        }

        showStatus('📋 Consultation completed. Thank you!', 'info');
        
        // Show session summary or feedback form
        setTimeout(() => {
            showSessionSummary();
        }, 2000);
    });

    call.on('error', (err) => {
        updateDebugInfo(`Call error: ${err.message}`);
        showStatus('Call error: ' + err.message, 'error');
    });
}

// FIXED: Show session summary when call ends
function showSessionSummary() {
    const summaryHTML = `
        <div style="text-align: center; padding: 20px; background: #f8f9fa; border-radius: 10px; margin: 20px 0;">
            <h3 style="color: #28a745; margin-bottom: 15px;">✅ Session Completed</h3>
            <p>Thank you for participating in our expert consultation session!</p>
            <p>We hope you found the session helpful for your scam prevention knowledge.</p>
            <div style="margin-top: 20px;">
                <button onclick="window.location.href='UserMySessions.aspx'" style="background: #D36F2D; color: white; padding: 12px 25px; border: none; border-radius: 8px; margin: 5px; cursor: pointer;">
                    📅 View My Sessions
                </button>
                <button onclick="window.location.href='UserHome.aspx'" style="background: #28a745; color: white; padding: 12px 25px; border: none; border-radius: 8px; margin: 5px; cursor: pointer;">
                    🏠 Back to Home
                </button>
            </div>
        </div>
    `;
    
            const statusElement = document.getElementById('<%= lblStatus.ClientID %>');
            if (statusElement) {
                statusElement.innerHTML = summaryHTML;
                statusElement.className = 'status-message success';
            }
        }

        function endCall() {
            updateDebugInfo('User initiated call end');

            if (currentCall) {
                currentCall.close();
                currentCall = null;
            }

            if (localStream) {
                localStream.getTracks().forEach(track => track.stop());
            }

            if (peer && isConnectionActive) {
                peer.destroy();
            }

            const remoteVideo = document.getElementById('remoteVideo');
            if (remoteVideo) {
                remoteVideo.srcObject = null;
            }

            showStatus('Consultation ended', 'info');

            // Show session summary
            setTimeout(() => {
                showSessionSummary();
            }, 1000);
        }

        // Cleanup on page unload
        window.onbeforeunload = function () {
            updateDebugInfo('Page unloading - cleaning up resources');

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
            console.log('🔧 Customer VideoCall page loading...');

            const phoneInput = document.getElementById('phoneInput');
            if (phoneInput) {
                phoneInput.focus();

                // Add enter key support
                phoneInput.addEventListener('keypress', function (e) {
                    if (e.key === 'Enter') {
                        joinSession();
                    }
                });
            }

            updateDebugInfo('Customer video call page initialized');
        };
    </script>
</asp:Content>