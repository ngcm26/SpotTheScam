<%@ Page Title="Webinar Management" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="StaffExpertWebinar.aspx.cs" Inherits="SpotTheScam.Staff.StaffExpertWebinar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body, .management-container, .management-container *, .section-title, .tab-btn, .form-label, .filter-dropdown {
            font-family: 'DM Sans', Arial, sans-serif !important;
        }

        .page-container {
            margin-top: 24px;
            margin-bottom: 32px;
        }

        .header-section {
            background: linear-gradient(135deg, #051D40 0%, #34495e 100%);
            color: white;
            padding: 32px 24px;
            border-radius: 16px;
            margin-bottom: 32px;
        }

        .header-title {
            font-size: 1.8em;
            font-weight: 600;
            margin: 0;
            color: white;
        }

        .header-subtitle {
            font-size: 1.08em;
            margin: 12px 0 0 0;
            opacity: 0.9;
            color: white;
        }

        .management-tabs {
            margin-bottom: 32px;
            display: flex;
            gap: 12px;
        }

        .tab-btn {
            background: #f8f9fa;
            color: #051D40;
            border: 1.5px solid #bfc5ce;
            padding: 12px 24px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 1.08em;
            cursor: pointer;
            transition: all 0.2s ease;
            outline: none;
        }

        .tab-btn.active {
            background: #051D40;
            color: white;
            border-color: #051D40;
        }

        .tab-btn:hover {
            background: #C46A1D;
            color: white;
            border-color: #C46A1D;
        }

        .management-section {
            background: white;
            border-radius: 10px;
            padding: 32px 24px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.03);
            margin-bottom: 32px;
        }

        .section-title {
            font-size: 1.3em;
            font-weight: 600;
            color: #222;
            margin-bottom: 0.2em;
        }

        .section-divider {
            border: none;
            border-top: 2px solid #222;
            margin-bottom: 24px;
            margin-top: 8px;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 32px;
        }

        .stat-card {
            background: linear-gradient(135deg, #051D40 0%, #0056b3 100%);
            color: white;
            padding: 24px;
            border-radius: 10px;
            text-align: center;
        }

        .stat-number {
            font-size: 2.2em;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .stat-label {
            font-size: 1em;
            opacity: 0.9;
            font-weight: 500;
        }

        .session-card {
            background: #f8f9fa;
            border: 1.5px solid #e9ecef;
            border-radius: 10px;
            padding: 24px;
            margin-bottom: 20px;
        }

        .session-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 16px;
        }

        .session-title {
            font-size: 1.2em;
            font-weight: 600;
            color: #051D40;
            margin: 0;
        }

        .session-date {
            color: #585252;
            font-size: 0.95em;
            font-weight: 500;
        }

        .participant-count {
            background: #28a745;
            color: white;
            padding: 6px 12px;
            border-radius: 15px;
            font-size: 0.85em;
            font-weight: 600;
        }

        .filter-section {
            display: flex;
            gap: 16px;
            margin-bottom: 24px;
            flex-wrap: wrap;
            align-items: center;
        }

        .filter-label {
            font-weight: 600;
            color: #051D40;
            font-size: 1.08em;
        }

        .filter-dropdown {
            padding: 10px 16px;
            border: 1.5px solid #bfc5ce;
            border-radius: 6px;
            background: white;
            font-size: 1.08em;
            color: #585252;
            outline: none;
            transition: border-color 0.2s;
        }

        .filter-dropdown:focus {
            border-color: #C46A1D;
        }

        .action-btn {
            background: #051D40;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 1em;
            cursor: pointer;
            transition: background-color 0.2s ease;
            margin-right: 12px;
        }

        .action-btn:hover {
            background: #C46A1D;
        }

        .action-btn.danger {
            background: #dc3545;
        }

        .action-btn.danger:hover {
            background: #c82333;
        }

        .export-btn {
            background: #28a745;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 1em;
            cursor: pointer;
            margin-left: auto;
        }

        .export-btn:hover {
            background: #218838;
        }

        .participants-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            font-family: 'DM Sans', Arial, sans-serif;
        }

        .participants-table th,
        .participants-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #e9ecef;
            font-size: 1em;
        }

        .participants-table th {
            background: #f8f9fa;
            font-weight: 600;
            color: #051D40;
        }

        .session-info-card {
            background: #fff8e1;
            border: 1.5px solid #ffc107;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 24px;
        }

        .session-info-title {
            font-size: 1.2em;
            font-weight: 600;
            color: #051D40;
            margin-bottom: 12px;
        }

        .session-info-details {
            color: #585252;
            line-height: 1.6;
        }

        @media (max-width: 768px) {
            .session-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 12px;
            }
            
            .stats-grid {
                grid-template-columns: 1fr;
            }
            
            .filter-section {
                flex-direction: column;
                align-items: flex-start;
            }

            .management-tabs {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <!-- Header Section -->
        <div class="header-section">
            <h1 class="header-title">Webinar Session Management</h1>
            <p class="header-subtitle">Monitor registrations, manage participants, and track attendance</p>
        </div>

        <!-- Management Tabs -->
        <div class="management-tabs">
            <asp:Button ID="btnOverviewTab" runat="server" CssClass="tab-btn active" 
                Text="Overview" OnClick="TabButton_Click" CommandArgument="overview" />
            <asp:Button ID="btnSessionsTab" runat="server" CssClass="tab-btn" 
                Text="Session Details" OnClick="TabButton_Click" CommandArgument="sessions" />
            <asp:Button ID="btnParticipantsTab" runat="server" CssClass="tab-btn" 
                Text="All Participants" OnClick="TabButton_Click" CommandArgument="participants" />
        </div>

        <!-- Overview Section -->
        <asp:Panel ID="pnlOverview" runat="server" CssClass="management-section">
            <h2 class="section-title">System Overview</h2>
            <hr class="section-divider" />
            
            <!-- Statistics -->
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number"><asp:Literal ID="ltTotalSessions" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Total Sessions</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number"><asp:Literal ID="ltTotalRegistrations" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Total Registrations</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number"><asp:Literal ID="ltAvailableSpots" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Available Spots</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number"><asp:Literal ID="ltUpcomingSessions" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Upcoming Sessions</div>
                </div>
            </div>

            <!-- Quick Session Overview -->
            <h3 style="color: #051D40; margin-bottom: 16px; font-weight: 600;">Session Status</h3>
            <asp:Repeater ID="rptSessionOverview" runat="server">
                <ItemTemplate>
                    <div class="session-card">
                        <div class="session-header">
                            <div>
                                <h4 class="session-title"><%# Eval("Title") %></h4>
                                <div class="session-date">📅 <%# Eval("SessionDate", "{0:MMMM dd, yyyy}") %> at <%# Eval("StartTime") %></div>
                            </div>
                            <div>
                                <span class="participant-count">
                                    <%# Eval("CurrentRegistrations") %>/<%# Eval("MaxParticipants") %> participants
                                </span>
                            </div>
                        </div>
                        <div style="color: #585252;">
                            <strong>Expert:</strong> <%# Eval("ExpertName") %><br>
                            <strong>Type:</strong> <%# Eval("SessionType") %> 
                            <%# Convert.ToInt32(Eval("PointsRequired")) > 0 ? "(" + Eval("PointsRequired") + " points)" : "" %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <!-- Session Details Section -->
        <asp:Panel ID="pnlSessions" runat="server" CssClass="management-section" Visible="false">
            <h2 class="section-title">Session Management</h2>
            <hr class="section-divider" />
            
            <div class="filter-section">
                <span class="filter-label">Filter by Session:</span>
                <asp:DropDownList ID="ddlSessionFilter" runat="server" CssClass="filter-dropdown" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlSessionFilter_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Button ID="btnExportSession" runat="server" CssClass="export-btn" 
                    Text="Export to Excel" OnClick="btnExportSession_Click" />
            </div>

            <asp:Panel ID="pnlSessionDetails" runat="server">
                <!-- Session Info -->
                <div class="session-info-card">
                    <h3 class="session-info-title">
                        <asp:Literal ID="ltSelectedSessionTitle" runat="server"></asp:Literal>
                    </h3>
                    <div class="session-info-details">
                        <strong>Date & Time:</strong> <asp:Literal ID="ltSelectedSessionDateTime" runat="server"></asp:Literal><br>
                        <strong>Expert:</strong> <asp:Literal ID="ltSelectedSessionExpert" runat="server"></asp:Literal><br>
                        <strong>Capacity:</strong> <asp:Literal ID="ltSelectedSessionCapacity" runat="server"></asp:Literal><br>
                        <strong>Current Registrations:</strong> <asp:Literal ID="ltSelectedSessionRegistrations" runat="server"></asp:Literal>
                    </div>
                </div>

                <!-- Participants List -->
                <h4 style="color: #051D40; margin: 24px 0 16px 0; font-weight: 600;">Registered Participants</h4>
                <asp:GridView ID="gvSessionParticipants" runat="server" CssClass="participants-table"
                    AutoGenerateColumns="false" EmptyDataText="No participants registered yet."
                    DataKeyNames="RegistrationId">
                    <Columns>
                        <asp:BoundField DataField="RegistrationId" HeaderText="ID" />
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Phone" HeaderText="Phone" />
                        <asp:BoundField DataField="RegistrationDate" HeaderText="Registration Date" DataFormatString="{0:MM/dd/yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Attendance">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlAttendance" runat="server" 
                                    CssClass="filter-dropdown"
                                    SelectedValue='<%# Eval("AttendanceStatus") %>'
                                    OnSelectedIndexChanged="ddlAttendance_SelectedIndexChanged"
                                    AutoPostBack="true">
                                    <asp:ListItem Value="Registered" Text="Registered" />
                                    <asp:ListItem Value="Attended" Text="Attended" />
                                    <asp:ListItem Value="No-Show" Text="No-Show" />
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnRemoveParticipant" runat="server" 
                                    CssClass="action-btn danger" Text="Remove"
                                    CommandArgument='<%# Eval("RegistrationId") %>'
                                    OnClick="btnRemoveParticipant_Click"
                                    OnClientClick="return confirm('Are you sure you want to remove this participant?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </asp:Panel>

        <!-- All Participants Section -->
        <asp:Panel ID="pnlParticipants" runat="server" CssClass="management-section" Visible="false">
            <h2 class="section-title">All Participants</h2>
            <hr class="section-divider" />
            
            <div class="filter-section">
                <span class="filter-label">Filter by Status:</span>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="filter-dropdown" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Value="All" Text="All Status" Selected="True" />
                    <asp:ListItem Value="Registered" Text="Registered" />
                    <asp:ListItem Value="Attended" Text="Attended" />
                    <asp:ListItem Value="No-Show" Text="No-Show" />
                </asp:DropDownList>
                
                <span class="filter-label">Session:</span>
                <asp:DropDownList ID="ddlParticipantSessionFilter" runat="server" CssClass="filter-dropdown" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlParticipantSessionFilter_SelectedIndexChanged">
                </asp:DropDownList>
                
                <asp:Button ID="btnExportAll" runat="server" CssClass="export-btn" 
                    Text="Export All" OnClick="btnExportAll_Click" />
            </div>

            <asp:GridView ID="gvAllParticipants" runat="server" CssClass="participants-table"
                AutoGenerateColumns="false" EmptyDataText="No participants found."
                AllowPaging="true" PageSize="20" OnPageIndexChanging="gvAllParticipants_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="RegistrationId" HeaderText="ID" />
                    <asp:BoundField DataField="SessionTitle" HeaderText="Session" />
                    <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                    <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="Phone" HeaderText="Phone" />
                    <asp:BoundField DataField="RegistrationDate" HeaderText="Registration Date" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span style="background: #051D40; color: white; padding: 4px 8px; border-radius: 12px; font-size: 0.85em; font-weight: 600;">
                                <%# Eval("AttendanceStatus") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="pager" />
            </asp:GridView>
        </asp:Panel>
    </div>
</asp:Content>