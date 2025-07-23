<%@ Page Title="Register for Webinar" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserWebinarRegistration.aspx.cs" Inherits="SpotTheScam.User.UserWebinarRegistration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .header-section {
            background: linear-gradient(135deg, var(--brand-orange) 0%, #e67e22 100%);
            color: white;
            padding: 40px 0;
            margin-bottom: 40px;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
        }

        .header-title {
            font-size: 1.8rem;
            font-weight: 700;
            margin: 0;
            color: white;
            text-align: center;
        }

        .header-subtitle {
            font-size: 1rem;
            margin: 10px 0 0 0;
            color: white;
            text-align: center;
            opacity: 0.9;
        }

        .points-badge {
            background: #FFC107;
            color: #000;
            padding: 5px 15px;
            border-radius: 15px;
            font-weight: 600;
            font-size: 0.9rem;
            position: absolute;
            top: 20px;
            right: 20px;
        }

        .page-container {
            padding-bottom: 60px;
        }

        .registration-card {
            background: white;
            border-radius: 15px;
            box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
            margin: 0 auto;
            max-width: 900px;
            overflow: hidden;
        }

        .session-info {
            background: #f8f9fa;
            padding: 25px;
            border-left: 4px solid var(--brand-orange);
        }

        .session-title {
            font-size: 1.3rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 10px;
            display: flex;
            align-items: center;
        }

        .session-title::before {
            content: "🔵";
            margin-right: 10px;
        }

        .session-description {
            color: #666;
            font-size: 0.95rem;
            line-height: 1.5;
            margin-bottom: 20px;
        }

        .session-details {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 20px;
        }

        .detail-item {
            display: flex;
            align-items: center;
            font-size: 0.9rem;
            color: #666;
        }

        .detail-icon {
            margin-right: 8px;
            font-size: 1rem;
        }

        .expert-info {
            display: flex;
            align-items: center;
            margin-top: 15px;
        }

        .expert-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            margin-right: 10px;
            object-fit: cover;
        }

        .expert-name {
            font-weight: 600;
            color: var(--brand-navy);
            margin: 0;
            font-size: 0.9rem;
        }

        .expert-title {
            color: #666;
            font-size: 0.8rem;
            margin: 0;
        }

        .form-section {
            padding: 30px;
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

        .form-control.error {
            border-color: #dc3545;
            background-color: #fff5f5;
        }

        .error-message {
            color: #dc3545;
            font-size: 0.8rem;
            margin-top: 5px;
            display: block;
        }

        .checkbox-group {
            margin: 20px 0;
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
        }

        .checkbox-item input[type="checkbox"] {
            margin-right: 10px;
            transform: scale(1.1);
        }

        .checkbox-item label {
            font-size: 0.9rem;
            color: #666;
            cursor: pointer;
        }

        .register-btn {
            background: var(--brand-orange);
            color: white;
            border: none;
            padding: 15px 40px;
            border-radius: 25px;
            font-weight: 600;
            font-size: 1rem;
            cursor: pointer;
            transition: background-color 0.3s ease;
            display: block;
            margin: 30px auto 0;
            min-width: 200px;
        }

        .register-btn:hover {
            background: #b45a22;
        }

        .register-btn:disabled {
            background: #ccc;
            cursor: not-allowed;
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

        @media (max-width: 768px) {
            .form-row {
                grid-template-columns: 1fr;
                gap: 15px;
            }
            
            .session-details {
                grid-template-columns: 1fr;
                gap: 10px;
            }
            
            .form-section {
                padding: 20px;
            }
            
            .points-badge {
                position: static;
                display: inline-block;
                margin-top: 10px;
            }
            
            .page-container {
                padding-bottom: 40px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <!-- Header Section -->
        <div class="header-section">
            <div class="container">
                <div class="points-badge">
                    <span>Current Points: <asp:Label ID="lblCurrentPoints" runat="server" Text="0" /> ⭐</span>
                </div>
                <h1 class="header-title">Register for expert sessions</h1>
                <p class="header-subtitle">Secure your spot with our cybersecurity experts</p>
            </div>
        </div>

        <div class="container">
            <div class="registration-card">
                <!-- Session Information -->
                <div class="session-info">
                    <h2 class="session-title">
                        <asp:Literal ID="ltSessionTitle" runat="server">Protecting Your Online Banking</asp:Literal>
                    </h2>
                    <p class="session-description">
                        <asp:Literal ID="ltSessionDescription" runat="server">
                            Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.
                        </asp:Literal>
                    </p>
                    
                    <div class="session-details">
                        <div class="detail-item">
                            <span class="detail-icon">📅</span>
                            <span><asp:Literal ID="ltSessionDate" runat="server">June 15, 2025</asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">🕐</span>
                            <span><asp:Literal ID="ltSessionTime" runat="server">2:00 PM - 3:00 PM</asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">👥</span>
                            <span><asp:Literal ID="ltParticipants" runat="server">Up to 100 Participants</asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-icon">⭐</span>
                            <span><asp:Literal ID="ltSessionType" runat="server">Free Session</asp:Literal></span>
                        </div>
                    </div>
                    
                    <div class="expert-info">
                        <asp:Image ID="imgExpert" runat="server" CssClass="expert-avatar" 
                            ImageUrl="/Images/expert2.jpg" AlternateText="Expert" />
                        <div>
                            <div class="expert-name">Expert: <asp:Literal ID="ltExpertName" runat="server">Dr Harvey Blue</asp:Literal></div>
                            <div class="expert-title"><asp:Literal ID="ltExpertTitle" runat="server">Cybersecurity Specialist, 15+ years experience</asp:Literal></div>
                        </div>
                    </div>
                </div>

                <!-- Registration Form -->
                <div class="form-section">
                    <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert">
                        <asp:Literal ID="ltAlertMessage" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">First Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                placeholder="Enter your first name" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                                ControlToValidate="txtFirstName" 
                                ErrorMessage="First name is required" 
                                CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        
                        <div class="form-group">
                            <label class="form-label">Last Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" 
                                placeholder="Enter your last name" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                                ControlToValidate="txtLastName" 
                                ErrorMessage="Last name is required" 
                                CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Email Address <span class="required">*</span></label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                placeholder="Enter your email address" TextMode="Email" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                ControlToValidate="txtEmail" 
                                ErrorMessage="Email address is required" 
                                CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                ControlToValidate="txtEmail" 
                                ErrorMessage="Please enter a valid email address" 
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" 
                                CssClass="error-message" Display="Dynamic"></asp:RegularExpressionValidator>
                        </div>
                        
                        <div class="form-group">
                            <label class="form-label">Phone Number <span class="required">*</span></label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" 
                                placeholder="Enter your phone number" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPhone" runat="server" 
                                ControlToValidate="txtPhone" 
                                ErrorMessage="Phone number is required" 
                                CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Main Security Concerns (Optional)</label>
                        <asp:DropDownList ID="ddlSecurityConcerns" runat="server" CssClass="form-control">
                            <asp:ListItem Value="" Text="Main Security Concerns (Optional)" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="Banking" Text="Online Banking Security"></asp:ListItem>
                            <asp:ListItem Value="Phone" Text="Phone/SMS Scams"></asp:ListItem>
                            <asp:ListItem Value="Email" Text="Email Phishing"></asp:ListItem>
                            <asp:ListItem Value="Social" Text="Social Media Safety"></asp:ListItem>
                            <asp:ListItem Value="Shopping" Text="Online Shopping Safety"></asp:ListItem>
                            <asp:ListItem Value="Identity" Text="Identity Theft Protection"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="checkbox-group">
                        <label class="form-label">How do you usually access your bank account?</label>
                        <div class="checkbox-item">
                            <asp:CheckBox ID="chkMobileBanking" runat="server" />
                            <label for="<%= chkMobileBanking.ClientID %>">Mobile banking app</label>
                        </div>
                        <div class="checkbox-item">
                            <asp:CheckBox ID="chkOnlineBanking" runat="server" />
                            <label for="<%= chkOnlineBanking.ClientID %>">Online banking website</label>
                        </div>
                        <div class="checkbox-item">
                            <asp:CheckBox ID="chkATM" runat="server" />
                            <label for="<%= chkATM.ClientID %>">ATM machines</label>
                        </div>
                        <div class="checkbox-item">
                            <asp:CheckBox ID="chkPhoneBanking" runat="server" />
                            <label for="<%= chkPhoneBanking.ClientID %>">Phone banking</label>
                        </div>
                        <div class="checkbox-item">
                            <asp:CheckBox ID="chkBranchVisits" runat="server" />
                            <label for="<%= chkBranchVisits.ClientID %>">Branch visits</label>
                        </div>
                    </div>

                    <asp:Button ID="btnRegister" runat="server" CssClass="register-btn" 
                        Text="Register Now - Free" OnClick="btnRegister_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>