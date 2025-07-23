<%@ Page Title="Scam Recovery Guidance" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ScamRecoveryGuidance.aspx.cs" Inherits="SpotTheScam.User.ScamRecoveryGuidance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Back button container */
        .back-button-section {
            max-width: 900px;
            margin: 0 auto;
            padding: 1.5rem 1.5rem 0 1.5rem;
        }

        /* Immediate actions container */
        .immediate-actions-section {
            max-width: 900px;
            margin: 0 auto;
            padding: 0 1.5rem 1.5rem 1.5rem;
        }

        /* Recovery guidelines container - wider */
        .recovery-guidelines-section {
            max-width: 1100px;
            margin: 0 auto;
            padding: 0 1.5rem 2rem 1.5rem;
        }

        .back-btn {
            background-color: #D36F2D;
            color: white;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            margin-bottom: 1.5rem;
        }

        .back-btn:hover {
            background-color: #B85A24;
            color: white;
            text-decoration: none;
        }

        /* Immediate Actions Section - Made more compact */
        .immediate-actions {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            border-radius: 8px;
            padding: 1.25rem;
            margin-bottom: 1.5rem;
        }

        .immediate-actions h3 {
            color: #721c24;
            font-size: 0.95rem;
            font-weight: 600;
            margin-bottom: 0.875rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .immediate-actions .alert-icon {
            color: #dc3545;
            font-size: 1.1rem;
        }

        .action-item {
            display: flex;
            align-items: flex-start;
            gap: 0.625rem;
            margin-bottom: 0.75rem;
            padding: 0.625rem;
            background-color: rgba(255, 255, 255, 0.7);
            border-radius: 6px;
        }

        .action-item:last-child {
            margin-bottom: 0;
        }

        .action-number {
            background-color: #dc3545;
            color: white;
            width: 22px;
            height: 22px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 0.8rem;
            font-weight: 600;
            flex-shrink: 0;
        }

        .action-content h4 {
            font-size: 0.825rem;
            font-weight: 600;
            margin-bottom: 0.2rem;
            color: #721c24;
        }

        .action-content p {
            font-size: 0.75rem;
            color: #721c24;
            margin: 0;
            line-height: 1.3;
        }

        /* Recovery Guidelines Section - Made wider */
        .recovery-section {
            background-color: white;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 1.5rem;
            margin-bottom: 2rem;
        }

        .recovery-section h3 {
            font-size: 1rem;
            font-weight: 600;
            color: #051D40;
            margin-bottom: 1.25rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .section-icon {
            width: 28px;
            height: 28px;
            background-color: #007bff;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 0.9rem;
        }

        .guidelines-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.25rem;
        }

        .csa-btn {
            background-color: #17a2b8;
            color: white;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 500;
            text-decoration: none;
        }

        .csa-btn:hover {
            background-color: #138496;
            color: white;
            text-decoration: none;
        }

        .step-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .step-card {
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 1.5rem;
            background-color: #f8f9fa;
        }

        .step-header {
            display: flex;
            align-items: center;
            gap: 0.625rem;
            margin-bottom: 0.875rem;
        }

        .step-number {
            background-color: #007bff;
            color: white;
            width: 26px;
            height: 26px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 0.8rem;
            font-weight: 600;
        }

        .step-title {
            font-size: 0.9rem;
            font-weight: 600;
            color: #051D40;
            margin: 0;
        }

        .step-description {
            font-size: 0.8rem;
            color: #666;
            margin-bottom: 0.875rem;
            line-height: 1.3;
        }

        .step-action {
            background-color: #e7f3ff;
            border-left: 3px solid #007bff;
            padding: 0.625rem;
            border-radius: 0 6px 6px 0;
        }

        .step-action strong {
            color: #007bff;
            font-size: 0.8rem;
        }

        .step-action p {
            font-size: 0.75rem;
            color: #666;
            margin: 0.2rem 0 0 0;
        }

        /* Center step */
        .center-step {
            grid-column: 1 / -1;
            justify-self: center;
            max-width: 450px;
        }

        /* Timeline Section - Full width */
        .timeline-section {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 8px;
            padding: 2rem;
            margin-bottom: 2rem;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            box-sizing: border-box;
        }

        .timeline-section .timeline-content {
            max-width: 900px;
            margin: 0 auto;
        }

        .timeline-section h3 {
            color: #856404;
            font-size: 1rem;
            font-weight: 600;
            margin-bottom: 1.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .timeline-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 1rem;
        }

        .timeline-item {
            background-color: rgba(255, 255, 255, 0.8);
            border-radius: 8px;
            padding: 1rem;
            text-align: center;
        }

        .timeline-number {
            background-color: #ffc107;
            color: #856404;
            width: 32px;
            height: 32px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 0.875rem;
            font-weight: 600;
            margin: 0 auto 0.5rem auto;
        }

        .timeline-time {
            font-size: 0.75rem;
            font-weight: 600;
            color: #856404;
            margin-bottom: 0.5rem;
        }

        .timeline-title {
            font-size: 0.85rem;
            font-weight: 600;
            color: #051D40;
            margin-bottom: 0.5rem;
        }

        .timeline-desc {
            font-size: 0.75rem;
            color: #666;
            line-height: 1.3;
        }

        .view-timeline-btn {
            background-color: #17a2b8;
            color: white;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            margin-top: 1rem;
        }

        .view-timeline-btn:hover {
            background-color: #138496;
            color: white;
            text-decoration: none;
        }

        /* Protection Section - separate container */
        .protection-section-container {
            max-width: 1000px;
            margin: 0 auto;
            padding: 0 1.5rem 2rem 1.5rem;
        }

        /* Protection Section - Made wider with bigger fonts */
        .protection-section {
            background-color: white;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 2.5rem;
            margin-bottom: 2rem;
        }

        .protection-section h3 {
            font-size: 1.3rem;
            font-weight: 600;
            color: #051D40;
            margin-bottom: 1.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .protection-icon {
            width: 36px;
            height: 36px;
            background-color: #17a2b8;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 1.1rem;
        }

        .protection-tips {
            display: flex;
            justify-content: space-between;
            gap: 2.5rem;
        }

        .tips-column {
            flex: 1;
        }

        .tips-box {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 8px;
            padding: 2rem;
            margin-bottom: 1.5rem;
        }

        .tips-box h4 {
            color: #856404;
            font-size: 1rem;
            font-weight: 600;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .tips-box ul {
            list-style: none;
            padding: 0;
            margin: 0;
        }

        .tips-box li {
            font-size: 0.9rem;
            color: #856404;
            margin-bottom: 0.6rem;
            padding-left: 1rem;
            position: relative;
            line-height: 1.4;
        }

        .tips-box li:before {
            content: "•";
            position: absolute;
            left: 0;
            color: #856404;
        }

        .security-box {
            background-color: #d1ecf1;
            border: 1px solid #bee5eb;
            border-radius: 8px;
            padding: 2rem;
        }

        .security-box h4 {
            color: #0c5460;
            font-size: 1rem;
            font-weight: 600;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .security-box ul {
            list-style: none;
            padding: 0;
            margin: 0;
        }

        .security-box li {
            font-size: 0.9rem;
            color: #0c5460;
            margin-bottom: 0.6rem;
            padding-left: 1rem;
            position: relative;
            line-height: 1.4;
        }

        .security-box li:before {
            content: "•";
            position: absolute;
            left: 0;
            color: #0c5460;
        }

        .prevention-tips-btn {
            background-color: #17a2b8;
            color: white;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            margin-top: 1rem;
        }

        .prevention-tips-btn:hover {
            background-color: #138496;
            color: white;
            text-decoration: none;
        }

        @media (max-width: 768px) {
            .back-button-section,
            .immediate-actions-section,
            .recovery-guidelines-section,
            .protection-section-container {
                padding-left: 1rem;
                padding-right: 1rem;
            }

            .step-grid {
                grid-template-columns: 1fr;
            }
            
            .timeline-grid {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .protection-tips {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Back Button Section -->
    <div class="back-button-section">
        <a href="RecoverySupport.aspx" class="back-btn">Back to Scam Types</a>
    </div>

    <!-- Immediate Actions Section -->
    <div class="immediate-actions-section">
        <div class="immediate-actions">
            <h3>
                <span class="alert-icon">⚠️</span>
                IMMEDIATE ACTIONS - Do This Right Now
            </h3>
            
            <div class="action-item">
                <div class="action-number">1</div>
                <div class="action-content">
                    <h4>STOP ALL COMMUNICATIONS - Log off website and stop payments</h4>
                    <p>Immediately close all contact with the scammer and stop any ongoing transactions.</p>
                </div>
            </div>

            <div class="action-item">
                <div class="action-number">2</div>
                <div class="action-content">
                    <h4>CONTACT BANK/POLICE IMMEDIATELY - Report within 24 hours to bank</h4>
                    <p>Time is critical. Contact your bank and local police as soon as possible.</p>
                </div>
            </div>

            <div class="action-item">
                <div class="action-number">3</div>
                <div class="action-content">
                    <h4>DOCUMENT EVERYTHING - Screenshots, emails, text communications</h4>
                    <p>Save all evidence including messages, receipts, and transaction records.</p>
                </div>
            </div>

            <div class="action-item">
                <div class="action-number">4</div>
                <div class="action-content">
                    <h4>BLOCK CONTACT - Block their phone/email and report online accounts</h4>
                    <p>Prevent further contact by blocking all communication channels.</p>
                </div>
            </div>
        </div>
    </div>

    <!-- Complete Recovery Guidelines Section -->
    <div class="recovery-guidelines-section">
        <div class="recovery-section">
            <div class="guidelines-header">
                <h3>
                    <span class="section-icon">📋</span>
                    Complete Recovery Guidelines
                </h3>
                <a href="#" class="csa-btn">CSA Guidelines</a>
            </div>

            <div class="step-grid">
                <!-- Step 1 -->
                <div class="step-card">
                    <div class="step-header">
                        <div class="step-number">1</div>
                        <h4 class="step-title">Contact All Financial Institutions</h4>
                    </div>
                    <p class="step-description">Reach out to every bank, credit card company, and financial service provider immediately.</p>
                    <div class="step-action">
                        <strong>Action:</strong> Call during business hours, have account numbers ready, request fraud protection
                        <p>Consider freezing accounts temporarily</p>
                    </div>
                </div>

                <!-- Step 2 -->
                <div class="step-card">
                    <div class="step-header">
                        <div class="step-number">2</div>
                        <h4 class="step-title">Document Everything</h4>
                    </div>
                    <p class="step-description">Create a paper trail of all scam-related communications and transactions.</p>
                    <div class="step-action">
                        <strong>Action:</strong> Screenshot messages, print emails, save transaction receipts, write down contact information
                        <p>Keep detailed records of all interactions</p>
                    </div>
                </div>

                <!-- Step 3 -->
                <div class="step-card">
                    <div class="step-header">
                        <div class="step-number">3</div>
                        <h4 class="step-title">Monitor Your Credit</h4>
                    </div>
                    <p class="step-description">Check for new accounts or credit inquiries you didn't make or authorize.</p>
                    <div class="step-action">
                        <strong>Action:</strong> Get free credit reports, set up fraud alerts, consider credit freeze
                        <p>Review statements monthly</p>
                    </div>
                </div>

                <!-- Step 4 -->
                <div class="step-card">
                    <div class="step-header">
                        <div class="step-number">4</div>
                        <h4 class="step-title">Secure Your Accounts</h4>
                    </div>
                    <p class="step-description">Change passwords and enable additional security on all accounts.</p>
                    <div class="step-action">
                        <strong>Action:</strong> Use different passwords, enable 2-factor authentication, review account permissions
                        <p>Update security on all online accounts</p>
                    </div>
                </div>

                <!-- Step 5 - Center -->
                <div class="step-card center-step">
                    <div class="step-header">
                        <div class="step-number">5</div>
                        <h4 class="step-title">File Official Reports</h4>
                    </div>
                    <p class="step-description">Report to authorities and law enforcement for your protection.</p>
                    <div class="step-action">
                        <strong>Action:</strong> Police report, MAS complaint (if investment), CSA scam alert
                        <p>Official documentation helps recovery</p>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Recovery Timeline Section -->
    <div class="timeline-section">
        <div class="timeline-content">
            <h3>
                <span class="alert-icon">📅</span>
                Recovery Timeline
            </h3>
            
            <div class="timeline-grid">
                <div class="timeline-item">
                    <div class="timeline-number">1</div>
                    <div class="timeline-time">First 24 Hours</div>
                    <div class="timeline-title">Crisis Response</div>
                    <div class="timeline-desc">Contact banks, change passwords, file police report, secure accounts</div>
                </div>

                <div class="timeline-item">
                    <div class="timeline-number">2</div>
                    <div class="timeline-time">Week 1</div>
                    <div class="timeline-title">Documentation & Monitoring</div>
                    <div class="timeline-desc">Gather evidence, contact all financial institutions, set up account monitoring</div>
                </div>

                <div class="timeline-item">
                    <div class="timeline-number">3</div>
                    <div class="timeline-time">Weeks 2-4</div>
                    <div class="timeline-title">Follow-up & Recovery</div>
                    <div class="timeline-desc">Work with banks on refunds, legal consultation if needed, emotional support</div>
                </div>

                <div class="timeline-item">
                    <div class="timeline-number">4</div>
                    <div class="timeline-time">Ongoing</div>
                    <div class="timeline-title">Protection & Prevention</div>
                    <div class="timeline-desc">Regular monitoring, updated security measures, scam awareness education</div>
                </div>
            </div>
            
            <a href="#" class="view-timeline-btn">View Full Timeline</a>
        </div>
    </div>

    <!-- Protection Section -->
    <div class="protection-section-container">
        <div class="protection-section">
            <h3>
                <span class="protection-icon">🛡️</span>
                Protect Yourself Going Forward
            </h3>
            
            <div class="protection-tips">
                <div class="tips-column">
                    <div class="tips-box">
                        <h4>⚠️ Banks will NEVER ask for:</h4>
                        <ul>
                            <li>Your full password or PIN over the phone</li>
                            <li>You to transfer money to "safe" accounts</li>
                            <li>Remote access to your computer</li>
                            <li>Personal details via email or text</li>
                            <li>You to withdraw cash for "verification"</li>
                        </ul>
                    </div>
                </div>
                
                <div class="tips-column">
                    <div class="security-box">
                        <h4>🔒 Security Recommendations:</h4>
                        <ul>
                            <li>Install ScamShield: Get Netflix official app blocks scam calls</li>
                            <li>Enable Account Alerts: Get notified of all transactions</li>
                            <li>Use Strong Passwords: Different passwords for each account</li>
                            <li>Verify Independently: Call bank using official numbers</li>
                            <li>Trust Your Instincts: If it feels wrong, it probably is</li>
                        </ul>
                    </div>
                </div>
            </div>
            
            <a href="#" class="prevention-tips-btn">Prevention Tips</a>
        </div>
    </div>
</asp:Content>