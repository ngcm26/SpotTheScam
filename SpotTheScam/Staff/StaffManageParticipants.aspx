<%@ Page Title="Manage Session Participants" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="StaffManageParticipants.aspx.cs" Inherits="SpotTheScam.Staff.StaffManageParticipants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-header {
            background: linear-gradient(135deg, var(--brand-orange) 0%, #e67e22 100%);
            color: white;
            padding: 30px 0;
            margin: -20px -15px 30px -15px;
            border-radius: 0 0 15px 15px;
        }

        .session-info-card {
            background: white;
            border-radius: 10px;
            padding: 25px;
            margin-bottom: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            border-left: 5px solid var(--brand-orange);
        }

        .participants-grid {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
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

        .participant-checkbox {
            transform: scale(1.2);
        }

        .participant-info h4 {
            color: var(--brand-navy);
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

        .btn-action {
            padding: 8px 16px;
            border: none;
            border-radius: 6px;
            font-size: 0.85rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            text-decoration: none;
            display: inline-block;
        }

        .btn-call {
            background: #28a745;
            color: white;
        }

        .btn-call:hover {
            background: #218838;
            color: white;
        }

        .btn-remove {
            background: #dc3545;
            color: white;
        }

        .btn-remove:hover {
            background: #c82333;
        }

        .btn-back {
            background: #6c757d;
            color: white;
            margin-bottom: 20px;
        }

        .btn-back:hover {
            background: #5a6268;
            color: white;
        }

        .session-link-section {
            background: #e7f3ff;
            border: 1px solid #b3d9ff;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 30px;
        }

        .session-link {
            background: white;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 10px;
            font-family: monospace;
            word-break: break-all;
            margin: 10px 0;
        }

        .copy-button {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 8px 15px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 0.9rem;
        }

        .no-participants {
            text-align: center;
            padding: 40px;
            color: #666;
        }

        .bulk-actions {
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
        }

        .bulk-actions h4 {
            color: var(--brand-navy);
            margin: 0;
            font-size: 1.1rem;
        }

        .bulk-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 12px 20px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .bulk-btn:hover {
            background: #b45a22;
        }

        .bulk-btn.secondary {
            background: #6c757d;
        }

        .bulk-btn.secondary:hover {
            background: #5a6268;
        }

        .bulk-btn.danger {
            background: #dc3545;
        }

        .bulk-btn.danger:hover {
            background: #c82333;
        }

        .selection-info {
            background: #e8f4e8;
            border: 1px solid #c3e6cb;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 20px;
            display: none;
        }

        .selection-info.show {
            display: block;
        }

        .alert {
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .alert-success {
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
        }

        .alert-error {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
        }

        .search-filter {
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
            border-color: var(--brand-orange);
            box-shadow: 0 0 0 2px rgba(211, 111, 45, 0.2);
        }

        .select-all-section {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .quick-connect-section {
            background: #e3f2fd;
            border: 1px solid #90caf9;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
        }

        .quick-connect-section h4 {
            color: var(--brand-navy);
            margin: 0 0 15px 0;
        }

        .connect-mode-buttons {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Page Header -->
    <div class="page-header">
        <h1 style="text-align: center; margin: 0; font-size: 1.8rem; font-weight: 700;">Manage Session Participants</h1>
    </div>

    <!-- Back Button -->
    <a href="StaffManageSessions.aspx" class="btn-action btn-back">← Back to Sessions</a>

    <!-- Alert Panel -->
    <asp:Panel ID="AlertPanel" runat="server" CssClass="alert" Visible="false">
        <asp:Label ID="AlertMessage" runat="server"></asp:Label>
    </asp:Panel>

    <!-- Session Information -->
    <div class="session-info-card">
        <h3 style="color: var(--brand-navy); margin-top: 0;">Session Details</h3>
        <asp:Label ID="lblSessionInfo" runat="server"></asp:Label>
    </div>

    <!-- Session Link Section -->
    <div class="session-link-section">
        <h4 style="color: var(--brand-navy); margin-top: 0;">🔗 Session Link for Staff</h4>
        <p>Use this link to start the video session:</p>
        <div class="session-link" id="sessionLink">
            <asp:Label ID="lblSessionLink" runat="server"></asp:Label>
        </div>
        <button type="button" class="copy-button" onclick="copySessionLink()">📋 Copy Link</button>
    </div>

    <!-- Quick Connect Options -->
    <div class="quick-connect-section">
        <h4>🚀 Quick Video Session Options</h4>
        <div class="connect-mode-buttons">
            <asp:Button ID="btnStartBroadcast" runat="server" CssClass="bulk-btn" 
                Text="📢 Start Broadcast Mode (All Participants)" OnClick="btnStartBroadcast_Click" />
            <asp:Button ID="btnStartGroupCall" runat="server" CssClass="bulk-btn" 
                Text="👥 Start Group Call (Selected)" OnClick="btnStartGroupCall_Click" />
            <asp:Button ID="btnStartOneOnOne" runat="server" CssClass="bulk-btn secondary" 
                Text="📞 One-on-One Mode" OnClick="btnStartOneOnOne_Click" />
        </div>
    </div>

    <!-- Selection Info Panel -->
    <div id="selectionInfo" class="selection-info">
        <strong>Selected: <span id="selectedCount">0</span> participants</strong>
        <span id="selectedNames"></span>
    </div>

    <!-- Bulk Actions -->
    <div class="bulk-actions">
        <h4>Bulk Actions:</h4>
        <asp:Button ID="btnConnectSelected" runat="server" CssClass="bulk-btn" 
            Text="📞 Connect to Selected" OnClick="btnConnectSelected_Click" />
        <asp:Button ID="btnRemoveSelected" runat="server" CssClass="bulk-btn danger" 
            Text="❌ Remove Selected" OnClick="btnRemoveSelected_Click" 
            OnClientClick="return confirm('Remove selected participants from the session?');" />
        <button type="button" class="bulk-btn secondary" onclick="refreshParticipantsList()">
            🔄 Refresh List
        </button>
    </div>

    <!-- Select All Section -->
    <div class="select-all-section">
        <label>
            <input type="checkbox" id="selectAllCheckbox" onchange="toggleSelectAll()" />
            <strong>Select All Participants</strong>
        </label>
        <span>Total: <asp:Label ID="lblParticipantCount" runat="server">0</asp:Label> participants</span>
    </div>

    <!-- Search and Filter -->
    <div class="search-filter">
        <input type="text" id="searchInput" class="search-input" 
               placeholder="🔍 Search participants by name, phone, or email..." 
               onkeyup="filterParticipants(this.value)" />
    </div>

    <!-- Participants List -->
    <div class="participants-grid">
        <h3 style="color: var(--brand-navy); margin-bottom: 20px;">
            Registered Participants
        </h3>

        <asp:Repeater ID="rptParticipants" runat="server" OnItemCommand="rptParticipants_ItemCommand">
            <ItemTemplate>
                <div class="participant-card" data-participant-id="<%# Eval("UserId") %>" 
                     data-phone="<%# Eval("PhoneNumber") %>" data-name="<%# Eval("UserName") %>" 
                     data-email="<%# Eval("Email") %>">
                    <input type="checkbox" class="participant-checkbox" 
                           onchange="updateSelection()" value="<%# Eval("UserId") %>" />
                    <div class="participant-info">
                        <h4><%# Eval("UserName") %></h4>
                        <div class="participant-details">
                            <span>📧 <%# Eval("Email") %></span>
                            <span>📱 <%# Eval("PhoneNumber") ?? "No phone provided" %></span>
                            <span>📅 Registered: <%# Eval("BookingDate", "{0:dd/MM/yyyy HH:mm}") %></span>
                            <span>🎯 Points Used: <%# Eval("PointsUsed") ?? "0" %></span>
                            <span>💬 Concerns: <%# Eval("ScamConcerns") ?? "General" %></span>
                        </div>
                    </div>
                    <div class="participant-actions">
                        <asp:Button ID="btnCallParticipant" runat="server" CssClass="btn-action btn-call" 
                            Text="📞 Connect Now" CommandName="ConnectParticipant" 
                            CommandArgument='<%# Eval("UserId") + "," + Eval("PhoneNumber") + "," + Eval("UserName") %>' />
                        <asp:Button ID="btnRemoveParticipant" runat="server" CssClass="btn-action btn-remove" 
                            Text="❌ Remove" CommandName="RemoveParticipant" 
                            CommandArgument='<%# Eval("UserId") %>'
                            OnClientClick="return confirm('Remove this participant from the session?');" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- No Participants Message -->
        <asp:Panel ID="pnlNoParticipants" runat="server" CssClass="no-participants" Visible="false">
            <p><strong>No participants registered yet.</strong></p>
            <p>Participants will appear here once they register for this session.</p>
        </asp:Panel>
    </div>

    <script>
        let selectedParticipants = [];

        function copySessionLink() {
            var linkElement = document.getElementById('sessionLink').innerText;
            navigator.clipboard.writeText(linkElement).then(function() {
                alert('Session link copied to clipboard!');
            }, function() {
                // Fallback for older browsers
                var textArea = document.createElement('textarea');
                textArea.value = linkElement;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                alert('Session link copied to clipboard!');
            });
        }

        function toggleSelectAll() {
            const selectAllCheckbox = document.getElementById('selectAllCheckbox');
            const participantCheckboxes = document.querySelectorAll('.participant-checkbox');
            
            participantCheckboxes.forEach(checkbox => {
                if (!checkbox.closest('.participant-card').style.display.includes('none')) {
                    checkbox.checked = selectAllCheckbox.checked;
                }
            });
            
            updateSelection();
        }

        function updateSelection() {
            const checkedBoxes = document.querySelectorAll('.participant-checkbox:checked');
            const selectionInfo = document.getElementById('selectionInfo');
            const selectedCount = document.getElementById('selectedCount');
            const selectedNames = document.getElementById('selectedNames');
            
            selectedParticipants = [];
            let names = [];
            
            checkedBoxes.forEach(checkbox => {
                const card = checkbox.closest('.participant-card');
                const participantId = checkbox.value;
                const name = card.getAttribute('data-name');
                const phone = card.getAttribute('data-phone');
                
                selectedParticipants.push({
                    id: participantId,
                    name: name,
                    phone: phone
                });
                names.push(name);
            });
            
            selectedCount.textContent = selectedParticipants.length;
            
            if (selectedParticipants.length > 0) {
                selectionInfo.classList.add('show');
                if (names.length <= 3) {
                    selectedNames.textContent = ': ' + names.join(', ');
                } else {
                    selectedNames.textContent = ': ' + names.slice(0, 3).join(', ') + ` and ${names.length - 3} more`;
                }
                
                // Update bulk action buttons
                updateBulkActionButtons(true);
            } else {
                selectionInfo.classList.remove('show');
                updateBulkActionButtons(false);
            }
            
            // Update select all checkbox state
            const allCheckboxes = document.querySelectorAll('.participant-checkbox');
            const visibleCheckboxes = Array.from(allCheckboxes).filter(cb => 
                !cb.closest('.participant-card').style.display.includes('none'));
            const checkedVisibleBoxes = visibleCheckboxes.filter(cb => cb.checked);
            
            const selectAllCheckbox = document.getElementById('selectAllCheckbox');
            if (checkedVisibleBoxes.length === 0) {
                selectAllCheckbox.indeterminate = false;
                selectAllCheckbox.checked = false;
            } else if (checkedVisibleBoxes.length === visibleCheckboxes.length) {
                selectAllCheckbox.indeterminate = false;
                selectAllCheckbox.checked = true;
            } else {
                selectAllCheckbox.indeterminate = true;
            }
        }

        function updateBulkActionButtons(hasSelection) {
            const connectBtn = document.getElementById('<%= btnConnectSelected.ClientID %>');
            const removeBtn = document.getElementById('<%= btnRemoveSelected.ClientID %>');
            
            if (connectBtn) {
                connectBtn.disabled = !hasSelection;
                connectBtn.style.opacity = hasSelection ? '1' : '0.5';
            }
            if (removeBtn) {
                removeBtn.disabled = !hasSelection;
                removeBtn.style.opacity = hasSelection ? '1' : '0.5';
            }
        }

        function filterParticipants(searchTerm) {
            const cards = document.querySelectorAll('.participant-card');
            const term = searchTerm.toLowerCase();

            cards.forEach(card => {
                const name = card.getAttribute('data-name').toLowerCase();
                const phone = card.getAttribute('data-phone').toLowerCase();
                const email = card.getAttribute('data-email').toLowerCase();

                if (name.includes(term) || phone.includes(term) || email.includes(term)) {
                    card.style.display = 'grid';
                } else {
                    card.style.display = 'none';
                    // Uncheck hidden cards
                    const checkbox = card.querySelector('.participant-checkbox');
                    if (checkbox) checkbox.checked = false;
                }
            });
            
            updateSelection();
        }

        function refreshParticipantsList() {
            window.location.reload();
        }

        // Store selected participant IDs in hidden field before postback
        function storeSelectedParticipants() {
            const selectedIds = selectedParticipants.map(p => p.id);
            
            // Create or update hidden field with selected participant IDs
            let hiddenField = document.getElementById('hdnSelectedParticipants');
            if (!hiddenField) {
                hiddenField = document.createElement('input');
                hiddenField.type = 'hidden';
                hiddenField.id = 'hdnSelectedParticipants';
                hiddenField.name = 'hdnSelectedParticipants';
                document.forms[0].appendChild(hiddenField);
            }
            hiddenField.value = selectedIds.join(',');
        }

        // Add event listeners to bulk action buttons
        document.addEventListener('DOMContentLoaded', function() {
            const connectBtn = document.getElementById('<%= btnConnectSelected.ClientID %>');
            const removeBtn = document.getElementById('<%= btnRemoveSelected.ClientID %>');
            
            if (connectBtn) {
                connectBtn.addEventListener('click', storeSelectedParticipants);
            }
            if (removeBtn) {
                removeBtn.addEventListener('click', storeSelectedParticipants);
            }
            
            // Initialize bulk action button states
            updateBulkActionButtons(false);
        });

        // Auto-hide alerts after 5 seconds
        setTimeout(function() {
            var alertPanel = document.getElementById('<%= AlertPanel.ClientID %>');
            if (alertPanel && alertPanel.style.display !== 'none') {
                alertPanel.style.display = 'none';
            }
        }, 5000);
    </script>
</asp:Content>