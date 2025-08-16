<%@ Page Title="Family Overview" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="FamilyOverview.aspx.cs" Inherits="SpotTheScam.User.FamilyOverview" %>

<%@ Register Src="~/User/Controls/FamilySideNav.ascx" TagPrefix="uc" TagName="FamilySideNav" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --brand: #D36F2D;
            --brand-600: #c0551f;
            --ink: #0f172a;
            --sub: #64748b;
            --line: #e5e7eb;
            --card: #fff;
            --bg: #fafafa;
        }
        /* Page container */
        .page-wrap {
            background: var(--bg);
        }

        .content-wrap {
            max-width: 1120px;
            margin: 0 auto;
            padding: 16px 16px 32px;
        }

        @media (min-width:1400px) {
            .content-wrap {
                max-width: 1200px;
            }
        }

        /* Headings */
        .page-title {
            margin: 2px 0 6px;
            font-weight: 800;
            letter-spacing: -.015em
        }

        .page-sub {
            color: var(--sub);
            margin-bottom: 16px
        }

        /* Cards + elevation */
        .card {
            background: var(--card);
            border: 1px solid var(--line);
            border-radius: 14px;
            box-shadow: 0 6px 18px rgba(15,23,42,.06);
        }

        .card-body {
            padding: 16px
        }

        .section {
            margin-bottom: 18px
        }

        .section-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 12px 16px 0
        }

        .section-title {
            font-size: 1.05rem;
            font-weight: 700
        }

        /* KPI grid */
        .kpi-grid {
            display: grid;
            grid-template-columns: repeat(12,1fr);
            gap: 12px;
        }

        .kpi {
            grid-column: span 12;
        }

        @media (min-width:576px) {
            .kpi {
                grid-column: span 6;
            }
        }

        @media (min-width:992px) {
            .kpi {
                grid-column: span 3;
            }
        }

        .kpi .kpi-body {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 16px
        }

        .kpi .ico {
            width: 34px;
            height: 34px;
            border-radius: 10px;
            display: grid;
            place-items: center;
            background: #fff6f1;
            color: var(--brand);
            border: 1px solid #ffe4d6
        }

        .kpi .val {
            font-size: 1.8rem;
            font-weight: 800;
            letter-spacing: -.01em
        }

        .kpi .delta {
            color: var(--sub);
            font-size: .92rem;
            margin-left: 6px
        }

        .kpi .lbl {
            color: var(--sub);
            font-size: .92rem;
            margin-top: 2px
        }

        /* List rows (Risk feed, Approvals) */
        .list {
            padding: 4px 0
        }

        .rowline {
            display: flex;
            justify-content: space-between;
            gap: 12px;
            padding: 12px 16px;
            border-top: 1px solid #f1f5f9
        }

            .rowline:first-child {
                border-top: 0
            }

        .row-left .top {
            font-weight: 700
        }

        .row-left .sub {
            color: var(--sub);
            font-size: .92rem
        }

        .row-right {
            text-align: right;
            min-width: 140px
        }

        .amt-neg {
            font-weight: 800
        }

        .chip {
            display: inline-block;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: .85rem;
            line-height: 1.6;
            border: 1px solid #eee
        }

        .chip-red {
            background: #fdeaea;
            color: #8b1b1b;
            border-color: #f2c1c1
        }

        .chip-held {
            background: #fff7e6;
            color: #7a4a00;
            border-color: #ffd591
        }

        .chip-muted {
            background: #f1f5f9;
            color: #0f172a;
            border-color: #e2e8f0
        }

        /* Buttons */
        .btn-brand {
            background: var(--brand);
            border-color: var(--brand);
            color: #fff;
            border-radius: 10px;
            padding: 8px 12px;
            font-weight: 600
        }

            .btn-brand:hover {
                background: var(--brand-600);
                border-color: var(--brand-600)
            }

        .btn-ghost {
            border: 1px solid var(--line);
            background: #fff;
            border-radius: 10px;
            padding: 7px 12px;
            font-weight: 600;
            color: #0f172a
        }

            .btn-ghost:hover {
                border-color: var(--brand);
                box-shadow: 0 0 0 3px rgba(211,111,45,.12)
            }

        /* Accounts grid */
        .acct-grid {
            display: grid;
            grid-template-columns: repeat(12,1fr);
            gap: 12px;
        }

        .acct {
            grid-column: span 12;
        }

        @media (min-width:768px) {
            .acct {
                grid-column: span 6;
            }
        }

        @media (min-width:1200px) {
            .acct {
                grid-column: span 4;
            }
        }

        .acct .kpi-body {
            padding: 16px
        }

        /* Safety table-ish */
        .safe-head, .safe-row {
            display: grid;
            grid-template-columns: 3fr 3fr 3fr 3fr;
            gap: 8px;
            padding: 10px 16px
        }

        .safe-head {
            color: var(--sub);
            font-weight: 700
        }

        .safe-row {
            border-top: 1px solid #f1f5f9
        }

        .safe-actions a {
            color: var(--brand);
            text-decoration: none;
            font-weight: 700
        }


        /* --- Accounts snapshot polish --- */
        .account-card {
            background: #fff;
            border: 1px solid var(--line);
            border-radius: 16px;
            box-shadow: 0 10px 28px rgba(15,23,42,.06);
            padding: 16px;
            transition: transform .15s ease, box-shadow .2s ease, border-color .2s ease;
        }

            .account-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 14px 34px rgba(15,23,42,.10);
                border-color: #e2e8f0;
            }

        .account-top {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 8px;
        }

        .bank-chip {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 4px 10px;
            border-radius: 999px;
            background: #fff6f1;
            color: var(--brand);
            border: 1px solid #ffe4d6;
            font-weight: 700;
            font-size: .85rem;
        }

            .bank-chip svg {
                width: 16px;
                height: 16px
            }

        .account-name {
            font-weight: 800;
            letter-spacing: -.01em;
            margin-top: 8px
        }

        .account-mask {
            color: var(--sub);
            font-size: .92rem
        }

        .balance-wrap {
            margin-top: 10px
        }

        .balance-label {
            color: var(--sub);
            font-size: .92rem;
        }

        .balance-value {
            font-size: 1.6rem;
            font-weight: 900;
            letter-spacing: -.015em;
            margin-top: 2px
        }

        .card-actions {
            margin-top: 12px;
            display: flex;
            gap: 8px;
            flex-wrap: wrap
        }

        .btn-ghost {
            border: 1px solid var(--line);
            background: #fff;
            border-radius: 10px;
            padding: 7px 12px;
            font-weight: 700;
        }

            .btn-ghost:hover {
                border-color: var(--brand);
                box-shadow: 0 0 0 3px rgba(211,111,45,.12)
            }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrap">
        <div class="family-layout">
            <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="overview" />

            <main class="family-content">
                <div class="content-wrap">
                    <h2 class="page-title">Family Overview</h2>
                    <div class="page-sub">Overview for your family group</div>

                    <!-- No group selected -->
                    <asp:Panel ID="pnlNoGroupSelected" runat="server" Visible="false" CssClass="section card">
                        <div class="card-body">
                            <div class="rowline" style="border-top: 0">
                                <div class="row-left">
                                    <div class="top">No family group selected</div>
                                    <div class="sub">Choose a group to see alerts, approvals, accounts and safety settings.</div>
                                </div>
                                <div class="row-right">
                                    <a href="MyGroups.aspx" class="btn-brand">Choose a group</a>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Main content when group is selected -->
                    <asp:Panel ID="pnlHasGroup" runat="server" Visible="false">

                        <!-- KPIs -->
                        <section class="section">
                            <div class="kpi-grid">
                                <!-- Red alerts -->
                                <div class="kpi card">
                                    <div class="kpi-body">
                                        <div class="ico">
                                            <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
                                                <path d="M12 9v4m0 4h.01M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0Z"
                                                    fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                            </svg>
                                        </div>
                                        <div>
                                            <div class="val">
                                                <asp:Literal ID="litKpiRed24h" runat="server" />
                                            </div>
                                            <div class="lbl">High-risk alerts (24h)</div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Pending approvals -->
                                <div class="kpi card">
                                    <div class="kpi-body">
                                        <div class="ico">
                                            <svg viewBox="0 0 24 24" width="18" height="18">
                                                <path d="M3 6h18M3 12h12M3 18h8" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                            </svg>
                                        </div>
                                        <div>
                                            <div class="val">
                                                <asp:Literal ID="litKpiPending" runat="server" />
                                            </div>
                                            <div class="lbl">Pending approvals</div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Outflow -->
                                <div class="kpi card">
                                    <div class="kpi-body">
                                        <div class="ico">
                                            <svg viewBox="0 0 24 24" width="18" height="18">
                                                <path d="M6 4v16M6 8l4-4 4 4M18 20h-8" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                            </svg>
                                        </div>
                                        <div>
                                            <div class="val">
                                                <asp:Literal ID="litKpiOutflow7d" runat="server" />
                                                <span class="delta">(<asp:Literal ID="litKpiOutflowDelta" runat="server" />)</span>
                                            </div>
                                            <div class="lbl">Outflow (last 7 days)</div>
                                        </div>
                                    </div>
                                </div>

                                <!-- New payees -->
                                <div class="kpi card">
                                    <div class="kpi-body">
                                        <div class="ico">
                                            <svg viewBox="0 0 24 24" width="18" height="18">
                                                <path d="M12 12a4 4 0 1 0-4-4 4 4 0 0 0 4 4Zm7 8v-1a5 5 0 0 0-5-5H10a5 5 0 0 0-5 5v1" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                            </svg>
                                        </div>
                                        <div>
                                            <div class="val">
                                                <asp:Literal ID="litKpiNewPayees7d" runat="server" />
                                            </div>
                                            <div class="lbl">New payees (7 days)</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </section>

                        <!-- Risk feed -->
                        <section class="section card">
                            <div class="section-header">
                                <div class="section-title">Risk feed</div>
                            </div>
                            <div class="card-body list">
                                <asp:Repeater ID="rptRiskFeed" runat="server">
                                    <ItemTemplate>
                                        <div class="rowline">
                                            <div class="row-left">
                                                <div class="top"><%# Eval("MemberName") %> • <%# Eval("AccountNickname") %> (<%# Eval("AccountNumberMasked") %>)</div>
                                                <div class="sub">
                                                    <%# Eval("TransactionDate","{0:yyyy-MM-dd}") %> <%# Eval("TransactionTime") %> •
                                                    <%# Eval("SenderRecipient") %> — <%# Eval("Description") %>
                                                </div>
                                            </div>
                                            <div class="row-right">
                                                <div class="amt-neg">-$<%# String.Format("{0:N2}", Eval("Amount")) %></div>
                                                <div>
                                                    <%# (Eval("IsHeld").ToString()=="True")
                                                        ? "<span class='chip chip-held'>Held</span>"
                                                        : (string.IsNullOrEmpty(Eval("Severity") as string) ? "<span class='chip chip-muted'>Flagged</span>"
                                                          : "<span class='chip chip-red'>"+ Eval("Severity") +"</span>") %>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                                            <div class="rowline">
                                                <div class="row-left">
                                                    <div class="sub">No recent high-risk activity.</div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </section>

                        <!-- Approvals queue (read-only here) -->
                        <section class="section card">
                            <div class="section-header">
                                <div class="section-title">Approvals queue</div>
                                <a href="Approvals.aspx" class="btn-brand">View all</a>
                            </div>
                            <div class="card-body list">
                                <asp:Repeater ID="rptApprovals" runat="server">
                                    <ItemTemplate>
                                        <div class="rowline">
                                            <div class="row-left">
                                                <div class="top"><%# Eval("MemberName") %> • <%# Eval("AccountNickname") %> (<%# Eval("AccountNumberMasked") %>)</div>
                                                <div class="sub">
                                                    <%# Eval("TransactionDate","{0:yyyy-MM-dd}") %> <%# Eval("TransactionTime") %> •
                                                    <%# Eval("SenderRecipient") %>
                                                </div>
                                            </div>
                                            <div class="row-right">
                                                <div class="amt-neg">-$<%# String.Format("{0:N2}", Eval("Amount")) %></div>
                                                <div class="chip chip-held"><%# String.IsNullOrEmpty(Eval("ReviewStatus") as string) ? "Pending" : Eval("ReviewStatus") %></div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                                            <div class="rowline">
                                                <div class="row-left">
                                                    <div class="sub">No approvals pending.</div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </section>

                        <!-- Accounts snapshot -->
                        <section class="section card">
                            <div class="section-header">
                                <div class="section-title">Accounts snapshot</div>
                            </div>
                            <div class="card-body">
                                <div class="acct-grid">
                                    <asp:Repeater ID="rptAccounts" runat="server">
                                        <ItemTemplate>
                                            <div class="acct">
                                                <div class="account-card">
                                                    <div class="account-top">
                                                        <span class="bank-chip">
                                                            <!-- card/bank icon -->
                                                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                                                <path d="M3 10h18M5 20h14a2 2 0 0 0 2-2V6H3v12a2 2 0 0 0 2 2Z" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                                            </svg>
                                                            <%# Eval("BankName") %>
                                                        </span>
                                                    </div>

                                                    <div class="account-name"><%# Eval("AccountNickname") %></div>
                                                    <div class="account-mask"><%# Eval("MemberName") %> • <%# Eval("AccountNumberMasked") %></div>

                                                    <div class="balance-wrap">
                                                        <div class="balance-label">Balance</div>
                                                        <div class="balance-value">$<%# String.Format("{0:N2}", Eval("Balance")) %></div>
                                                    </div>

                                                    <div class="card-actions">
                                                        <a class="btn-ghost"
                                                            href='<%# "UserTransactionLogs.aspx?accountId=" + Eval("AccountId") %>'>View transactions
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>

                                        <FooterTemplate>
                                            <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                                                <div class="rowline">
                                                    <div class="row-left">
                                                        <div class="sub">No accounts yet.</div>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </section>

                        <!-- Safety status -->
                        <section class="section card">
                            <div class="section-header">
                                <div class="section-title">Safety status</div>
                            </div>
                            <div class="card-body">
                                <asp:Repeater ID="rptSafety" runat="server">
                                    <HeaderTemplate>
                                        <div class="safe-head">
                                            <div>Primary</div>
                                            <div>Single txn limit</div>
                                            <div>Daily max</div>
                                            <div>Time window</div>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="safe-row">
                                            <div><%# Eval("MemberName") %></div>
                                            <div>$<%# String.Format("{0:N2}", Eval("SingleTransactionLimit")) %></div>
                                            <div>$<%# String.Format("{0:N2}", Eval("MaxDailyTransfer")) %></div>
                                            <div class="safe-actions">
                                                <%# Eval("TimeWindow") %> &nbsp;
                                                <a href='<%# "ManageGroup.aspx?groupId=" + (System.Web.HttpContext.Current.Session["CurrentGroupId"] ?? "") %>'
                                                    class="btn btn-sm btn-link" style="text-decoration: none; color: #D36F2D;">Manage</a>

                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                                            <div class="rowline">
                                                <div class="row-left">
                                                    <div class="sub">No safety limits set yet.</div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </section>

                    </asp:Panel>
                </div>
            </main>
        </div>
    </div>
</asp:Content>
