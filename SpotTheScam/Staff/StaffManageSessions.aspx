<%@ Page Title="Manage Expert Sessions" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="StaffManageSessions.aspx.cs" Inherits="SpotTheScam.Staff.StaffManageSessions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-header {
            background: linear-gradient(135deg, var(--brand-orange) 0%, #e67e22 100%);
            color: white;
            padding: 30px 0;
            margin: -20px -15px 30px -15px;
            border-radius: 0 0 15px 15px;
        }

        .page-title {
            font-size: 1.8rem;
            font-weight: 700;
            margin: 0;
            text-align: center;
        }

        .page-subtitle {
            text-align: center;
            margin-top: 10px;
            opacity: 0.9;
        }

        .action-buttons {
            text-align: center;
            margin-bottom: 30px;
        }

        .btn-create {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.3s;
            text-decoration: none;
            display: inline-block;
        }

        .btn-create:hover {
            background: #b45a22;
            color: white;
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

        .sessions-grid {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .session-card {
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            transition: transform 0.3s ease;
        }

        .session-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .session-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
        }

        .session-title {
            font-size: 1.2rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin: 0;
        }

        .session-status {
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            background: #d4edda;
            color: #155724;
        }

        .session-info {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 15px;
        }

        .info-item {
            display: flex;
            align-items: center;
            font-size: 0.9rem;
        }

        .info-icon {
            margin-right: 8px;
            color: var(--brand-orange);
            width: 16px;
        }

        .session-description {
            color: #666;
            font-size: 0.9rem;
            line-height: 1.4;
            margin-bottom: 15px;
        }

        .session-actions {
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

        .btn-edit {
            background: #28a745;
            color: white;
        }

        .btn-edit:hover {
            background: #218838;
            color: white;
        }

        .btn-delete {
            background: #dc3545;
            color: white;
        }

        .btn-delete:hover {
            background: #c82333;
        }

        .btn-video {
            background: var(--brand-orange);
            color: white;
        }

        .btn-video:hover {
            background: #b45a22;
            color: white;
        }

        .btn-manage-participants {
            background: #17a2b8;
            color: white;
        }

        .btn-manage-participants:hover {
            background: #138496;
            color: white;
        }

        .btn-cancel-regs {
            background: #ffc107;
            color: #212529;
        }

        .btn-cancel-regs:hover {
            background: #e0a800;
            color: #212529;
        }

        .create-form {
            background: white;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 30px;
            display: none;
        }

        .form-section {
            margin-bottom: 25px;
        }

        .form-section h4 {
            color: var(--brand-navy);
            margin-bottom: 15px;
            font-weight: 600;
            border-bottom: 2px solid #e9ecef;
            padding-bottom: 8px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 20px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group.full-width {
            grid-column: 1 / -1;
        }

        .form-label {
            display: block;
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
            font-size: 0.9rem;
        }

        .required {
            color: #dc3545;
        }

        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 0.9rem;
            transition: border-color 0.3s ease;
            box-sizing: border-box;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--brand-orange);
            box-shadow: 0 0 0 2px rgba(211, 111, 45, 0.2);
        }

        .error-message {
            color: #dc3545;
            font-size: 0.8rem;
            margin-top: 5px;
            display: block;
        }

        .form-actions {
            text-align: center;
            padding-top: 20px;
            border-top: 2px solid #e9ecef;
        }

        .btn-save {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            margin-right: 15px;
        }

        .btn-save:hover {
            background: #b45a22;
        }

        .btn-cancel {
            background: #6c757d;
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-cancel:hover {
            background: #5a6268;
        }

        .no-sessions {
            text-align: center;
            padding: 40px;
            color: #666;
        }

        @media (max-width: 768px) {
            .form-row {
                grid-template-columns: 1fr;
                gap: 15px;
            }
            
            .session-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 10px;
            }
            
            .session-info {
                grid-template-columns: 1fr;
                gap: 10px;
            }
            
            .session-actions {
                justify-content: center;
                flex-direction: column;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Page Header -->
    <div class="page-header">
        <h1 class="page-title">Manage Expert Sessions</h1>
        <p class="page-subtitle">Create and manage video consultation sessions for users</p>
    </div>

    <!-- Alert Panel -->
    <asp:Panel ID="AlertPanel" runat="server" CssClass="alert" Visible="false">
        <asp:Label ID="AlertMessage" runat="server"></asp:Label>
    </asp:Panel>

    <!-- Action Buttons -->
    <div class="action-buttons">
        <asp:Button ID="btnToggleForm" runat="server" CssClass="btn-create" 
            Text="+ Create New Session" OnClientClick="toggleCreateForm(); return false;" />
    </div>

    <!-- Create/Edit Form -->
    <div id="createForm" class="create-form">
        <div class="form-section">
            <h4>Session Details</h4>
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Session Title <span class="required">*</span></label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" 
                        placeholder="e.g., Protecting Your Online Banking" MaxLength="200"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                        ControlToValidate="txtTitle" 
                        ErrorMessage="Session title is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Session Type <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlSessionType" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="Select Session Type" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Free" Text="Free Session"></asp:ListItem>
                        <asp:ListItem Value="Premium" Text="Premium Session"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSessionType" runat="server" 
                        ControlToValidate="ddlSessionType" 
                        ErrorMessage="Session type is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            
            <div class="form-group full-width">
                <label class="form-label">Description <span class="required">*</span></label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                    placeholder="Describe what participants will learn in this session..." 
                    TextMode="MultiLine" Rows="3" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" 
                    ControlToValidate="txtDescription" 
                    ErrorMessage="Description is required" 
                    CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="form-section">
            <h4>Schedule & Capacity</h4>
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Session Date <span class="required">*</span></label>
                    <asp:TextBox ID="txtSessionDate" runat="server" CssClass="form-control" 
                        TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSessionDate" runat="server" 
                        ControlToValidate="txtSessionDate" 
                        ErrorMessage="Session date is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Topic Category</label>
                    <asp:DropDownList ID="ddlTopic" runat="server" CssClass="form-control">
                        <asp:ListItem Value="General" Text="General" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Banking" Text="Banking Security"></asp:ListItem>
                        <asp:ListItem Value="Phone" Text="Phone Scams"></asp:ListItem>
                        <asp:ListItem Value="Social" Text="Social Media Safety"></asp:ListItem>
                        <asp:ListItem Value="Email" Text="Email Scams"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Start Time <span class="required">*</span></label>
                    <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" 
                        TextMode="Time"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvStartTime" runat="server" 
                        ControlToValidate="txtStartTime" 
                        ErrorMessage="Start time is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label class="form-label">End Time <span class="required">*</span></label>
                    <asp:TextBox ID="txtEndTime" runat="server" CssClass="form-control" 
                        TextMode="Time"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEndTime" runat="server" 
                        ControlToValidate="txtEndTime" 
                        ErrorMessage="End time is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Max Participants <span class="required">*</span></label>
                    <asp:TextBox ID="txtMaxParticipants" runat="server" CssClass="form-control" 
                        placeholder="e.g., 100" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvMaxParticipants" runat="server" 
                        ControlToValidate="txtMaxParticipants" 
                        ErrorMessage="Maximum participants is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rvMaxParticipants" runat="server" 
                        ControlToValidate="txtMaxParticipants" 
                        MinimumValue="1" MaximumValue="1000" Type="Integer"
                        ErrorMessage="Must be between 1 and 1000" 
                        CssClass="error-message" Display="Dynamic"></asp:RangeValidator>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Points Required</label>
                    <asp:TextBox ID="txtPointsCost" runat="server" CssClass="form-control" 
                        placeholder="0 for free sessions" TextMode="Number" Text="0"></asp:TextBox>
                    <asp:RangeValidator ID="rvPointsCost" runat="server" 
                        ControlToValidate="txtPointsCost" 
                        MinimumValue="0" MaximumValue="1000" Type="Integer"
                        ErrorMessage="Must be between 0 and 1000" 
                        CssClass="error-message" Display="Dynamic"></asp:RangeValidator>
                </div>
            </div>
        </div>

        <div class="form-section">
            <h4>Expert Information</h4>
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Expert Name <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlExpertName" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="Select Expert" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Dr Harvey Blue" Text="Dr Harvey Blue"></asp:ListItem>
                        <asp:ListItem Value="Officer James Wilson" Text="Officer James Wilson"></asp:ListItem>
                        <asp:ListItem Value="Maria Rodriguez" Text="Maria Rodriguez"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvExpertName" runat="server" 
                        ControlToValidate="ddlExpertName" 
                        ErrorMessage="Expert name is required" 
                        CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Expert Title</label>
                    <asp:TextBox ID="txtExpertTitle" runat="server" CssClass="form-control" 
                        placeholder="e.g., Cybersecurity Specialist, 15+ years experience" MaxLength="200"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSave" runat="server" CssClass="btn-save" 
                Text="Create Session" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="btn-cancel" 
                Text="Cancel" OnClientClick="toggleCreateForm(); return false;" />
            
            <asp:HiddenField ID="hdnEditingSessionId" runat="server" />
        </div>
    </div>

    <!-- Sessions List -->
    <div class="sessions-grid">
        <asp:Repeater ID="rptSessions" runat="server" OnItemCommand="rptSessions_ItemCommand">
            <HeaderTemplate>
                <h3 style="color: var(--brand-navy); margin-bottom: 20px;">Current Sessions</h3>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="session-card">
                    <div class="session-header">
                        <h4 class="session-title"><%# Eval("SessionTitle") %></h4>
                        <span class="session-status">Available</span>
                    </div>
                    
                    <div class="session-info">
                        <div class="info-item">
                            <span class="info-icon">📅</span>
                            <span><%# Eval("SessionDate", "{0:dd/MM/yyyy}") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">🕐</span>
                            <span><%# Eval("StartTime") %> - <%# Eval("EndTime") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">👨‍💼</span>
                            <span><%# Eval("ExpertName") %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">👥</span>
                            <span><%# Eval("CurrentParticipants") %>/<%# Eval("MaxParticipants") %> participants</span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">⭐</span>
                            <span><%# Convert.ToInt32(Eval("PointsCost")) > 0 ? Eval("PointsCost") + " points" : "Free" %></span>
                        </div>
                        <div class="info-item">
                            <span class="info-icon">📝</span>
                            <span><%# Eval("SessionTopic") %></span>
                        </div>
                    </div>
                    
                    <p class="session-description"><%# Eval("SessionDescription") %></p>
                    
                    <div class="session-actions">
                        <a href='<%# "StaffVideoCall.aspx?sessionId=" + Eval("Id") %>' class="btn-action btn-video">
                            📹 Start Video Session
                        </a>

                        <!-- Manage Participants button - shows participant count -->
                        <asp:LinkButton ID="btnManageParticipants" runat="server" CssClass="btn-action btn-manage-participants" 
                            CommandName="manageparticipants" CommandArgument='<%# Eval("Id") %>'
                            Visible='<%# Convert.ToInt32(Eval("RegistrationCount")) > 0 %>'
                            CausesValidation="false">
                            👥 Manage Participants (<%# Eval("RegistrationCount") %>)
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-action btn-edit" 
                            CommandName="edit" CommandArgument='<%# Eval("Id") %>'
                            CausesValidation="false">
                            ✏️ Edit
                        </asp:LinkButton>

                        <!-- Show Cancel Registrations button only if there are registrations -->
                        <asp:LinkButton ID="btnCancelRegs" runat="server" CssClass="btn-action btn-cancel-regs" 
                            CommandName="cancelregistrations" CommandArgument='<%# Eval("Id") %>'
                            Visible='<%# Convert.ToInt32(Eval("RegistrationCount")) > 0 %>'
                            OnClientClick="return confirm('Are you sure you want to cancel all registrations for this session?');"
                            CausesValidation="false">
                            📝 Cancel Registrations (<%# Eval("RegistrationCount") %>)
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-action btn-delete" 
                            CommandName="delete" CommandArgument='<%# Eval("Id") %>' 
                            OnClientClick="return confirm('Are you sure you want to delete this session?');"
                            CausesValidation="false">
                            🗑️ Delete
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Show message when no sessions exist -->
        <asp:Panel ID="pnlNoSessions" runat="server" CssClass="no-sessions" Visible="false">
            <p><strong>No sessions created yet.</strong></p>
            <p>Click "Create New Session" to add your first expert consultation session.</p>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        function toggleCreateForm() {
            var form = document.getElementById('createForm');
            if (form.style.display === 'none' || form.style.display === '') {
                form.style.display = 'block';
                form.scrollIntoView({ behavior: 'smooth' });
            } else {
                form.style.display = 'none';
            }
        }

        // Auto-hide alerts after 5 seconds
        setTimeout(function() {
            var alertPanel = document.getElementById('<%= AlertPanel.ClientID %>');
            if (alertPanel && alertPanel.style.display !== 'none') {
                alertPanel.style.display = 'none';
            }
        }, 5000);

        // Set minimum date to today
        document.addEventListener('DOMContentLoaded', function() {
            var dateInput = document.getElementById('<%= txtSessionDate.ClientID %>');
            if (dateInput) {
                var today = new Date().toISOString().split('T')[0];
                dateInput.min = today;
            }
        });
    </script>
</asp:Content>