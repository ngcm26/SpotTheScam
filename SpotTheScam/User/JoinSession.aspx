<%@ Page Title="Join Session" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="JoinSession.aspx.cs" Inherits="SpotTheScam.JoinSession" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .join-container {
            max-width: 800px;
            margin: 40px auto;
            padding: 0 20px;
        }

        .session-card {
            background: white;
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            text-align: center;
        }

        .session-icon {
            font-size: 4rem;
            margin-bottom: 20px;
        }

        .session-title {
            font-size: 1.8rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 10px;
        }

        .session-subtitle {
            color: #666;
            margin-bottom: 30px;
        }

        .session-details {
            background: #f8f9fa;
            border-radius: 10px;
            padding: 20px;
            margin: 20px 0;
            text-align: left;
        }

        .detail-item {
            display: flex;
            margin-bottom: 10px;
            align-items: center;
        }

        .detail-icon {
            margin-right: 10px;
            color: var(--brand-orange);
            width: 20px;
        }

        .btn-join-large {
            background: linear-gradient(135deg, #4caf50, #45a049);
            color: white;
            padding: 15px 40px;
            border: none;
            border-radius: 10px;
            font-size: 1.2rem;
            font-weight: 600;
            cursor: pointer;
            margin: 20px 10px;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn-join-large:hover {
            background: linear-gradient(135deg, #45a049, #4caf50);
            transform: translateY(-2px);
            color: white;
        }

        .btn-secondary {
            background: #6c757d;
            color: white;
            padding: 10px 25px;
            border: none;
            border-radius: 8px;
            font-weight: 500;
            cursor: pointer;
            margin: 10px;
            text-decoration: none;
            display: inline-block;
        }

        .btn-secondary:hover {
            background: #5a6268;
            color: white;
        }

        .alert {
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .alert-error {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
        }

        .alert-warning {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
        }

        .alert-info {
            background-color: #d1ecf1;
            border: 1px solid #bee5eb;
            color: #0c5460;
        }

        .pre-session-checklist {
            background: #e8f5e8;
            border: 1px solid #c3e6cb;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
            text-align: left;
        }

        .checklist-item {
            margin: 10px 0;
            display: flex;
            align-items: center;
        }

        .checklist-item input[type="checkbox"] {
            margin-right: 10px;
            transform: scale(1.2);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="join-container">
        <!-- Alert Panel -->
        <asp:Panel ID="AlertPanel" runat="server" CssClass="alert" Visible="false">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Main Session Card -->
        <div class="session-card">
            <div class="session-icon">🎥</div>
            
            <h1 class="session-title">
                <asp:Label ID="lblSessionTitle" runat="server"></asp:Label>
            </h1>
            
            <p class="session-subtitle">
                Expert Video Consultation Session
            </p>

            <!-- Session Details -->
            <div class="session-details">
                <h4 style="margin-top: 0; color: var(--brand-navy);">Session Information</h4>
                <asp:Label ID="lblSessionDetails" runat="server"></asp:Label>
            </div>

            <!-- Pre-Session Checklist -->
            <div class="pre-session-checklist">
                <h4 style="margin-top: 0; color: var(--brand-navy);">📋 Before You Join</h4>
                <div class="checklist-item">
                    <input type="checkbox" id="checkCamera">
                    <label for="checkCamera">Camera is working and positioned correctly</label>
                </div>
                <div class="checklist-item">
                    <input type="checkbox" id="checkMicrophone">
                    <label for="checkMicrophone">Microphone is working and not muted</label>
                </div>
                <div class="checklist-item">
                    <input type="checkbox" id="checkInternet">
                    <label for="checkInternet">Stable internet connection</label>
                </div>
                <div class="checklist-item">
                    <input type="checkbox" id="checkQuiet">
                    <label for="checkQuiet">In a quiet environment</label>
                </div>
            </div>

            <!-- Join Actions -->
            <asp:Panel ID="pnlJoinActions" runat="server">
                <asp:Button ID="btnJoinSession" runat="server" CssClass="btn-join-large" 
                    Text="🚀 Join Video Session" OnClick="btnJoinSession_Click" />
                <br/>
                <a href="UserMySessions.aspx" class="btn-secondary">← Back to My Sessions</a>
            </asp:Panel>

            <!-- Error Panel -->
            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <div style="color: #dc3545; margin: 20px 0;">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </div>
                <a href="UserMySessions.aspx" class="btn-secondary">← Back to My Sessions</a>
            </asp:Panel>
        </div>
    </div>

    <script>
        // Check if all checklist items are checked before allowing join
        document.addEventListener('DOMContentLoaded', function() {
            var checkboxes = document.querySelectorAll('.checklist-item input[type="checkbox"]');
            var joinButton = document.getElementById('<%= btnJoinSession.ClientID %>');
            
            function updateJoinButton() {
                var allChecked = Array.from(checkboxes).every(cb => cb.checked);
                if (joinButton) {
                    joinButton.disabled = !allChecked;
                    if (!allChecked) {
                        joinButton.style.opacity = '0.5';
                        joinButton.title = 'Please complete the checklist above';
                    } else {
                        joinButton.style.opacity = '1';
                        joinButton.title = '';
                    }
                }
            }
            
            checkboxes.forEach(function(checkbox) {
                checkbox.addEventListener('change', updateJoinButton);
            });
            
            updateJoinButton(); // Initial check
        });
    </script>
</asp:Content>
