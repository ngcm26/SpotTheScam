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
            grid-template-columns: 1fr auto;
            align-items: center;
            gap: 20px;
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
            padding: 15px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
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

    <!-- Bulk Actions -->
    <div class="bulk-actions">
        <strong>Actions:</strong>
        <asp:Button ID="btnStartSession" runat="server" CssClass="btn-action btn-call" 
            Text="🚀 Start Video Session" OnClick="btnStartSession_Click" />
    </div>

    <!-- Participants List -->
    <div class="participants-grid">
        <h3 style="color: var(--brand-navy); margin-bottom: 20px;">
            Registered Participants (<asp:Label ID="lblParticipantCount" runat="server"></asp:Label>)
        </h3>

        <asp:Repeater ID="rptParticipants" runat="server" OnItemCommand="rptParticipants_ItemCommand">
            <ItemTemplate>
                <div class="participant-card">
                    <div class="participant-info">
                        <h4><%# Eval("UserName") %></h4>
                        <div class="participant-details">
                            <span>📧 <%# Eval("Email") %></span>
                            <span>📱 <%# Eval("PhoneNumber") ?? "No phone provided" %></span>
                            <span>📅 Registered: <%# Eval("BookingDate", "{0:dd/MM/yyyy HH:mm}") %></span>
                            <span>🎯 Points Used: <%# Eval("PointsUsed") ?? "0" %></span>
                        </div>
                    </div>
                    <div class="participant-actions">
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

        // Auto-hide alerts after 5 seconds
        setTimeout(function() {
            var alertPanel = document.getElementById('<%= AlertPanel.ClientID %>');
            if (alertPanel && alertPanel.style.display !== 'none') {
                alertPanel.style.display = 'none';
            }
        }, 5000);
    </script>
</asp:Content>