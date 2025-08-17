<%@ Page Title="Approvals" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="Approvals.aspx.cs" Inherits="SpotTheScam.User.Approvals" %>

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

        /* Cards */
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

        /* Filters */
        .filters .form-label {
            font-weight: 700;
            color: var(--ink)
        }

        .filters .row {
            padding: 8px 16px 4px
        }

        /* List rows */
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
            min-width: 180px
        }

        .amount-neg {
            font-weight: 900;
            letter-spacing: -.01em;
            color: #b91c1c
        }

        /* Chips */
        .chip {
            display: inline-block;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: .85rem;
            line-height: 1.6;
            border: 1px solid #eee
        }

        .chip-held {
            background: #fff7e6;
            color: #7a4a00;
            border-color: #ffd591
        }

        .chip-approved {
            background: #e6fffb;
            color: #006d75;
            border-color: #b5f5ec
        }

        .chip-denied {
            background: #fff1f0;
            color: #a8071a;
            border-color: #ffa39e
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
            font-weight: 700
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
            font-weight: 700;
            color: #0f172a
        }

            .btn-ghost:hover {
                border-color: var(--brand);
                box-shadow: 0 0 0 3px rgba(211,111,45,.12)
            }

        /* Message strip */
        .msg-card {
            border-left: 4px solid var(--brand);
        }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrap">
        <div class="family-layout">
            <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="approvals" />

            <main class="family-content">
                <div class="content-wrap">
                    <h2 class="page-title">Approvals</h2>
                    <div class="page-sub">Review held transactions for your family group.</div>

                    <!-- Flash message -->
                    <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="card msg-card">
                        <div class="card-body">
                            <asp:Label ID="lblMsg" runat="server" />
                        </div>
                    </asp:Panel>

                    <!-- No group -->
                    <asp:Panel ID="pnlNoGroup" runat="server" Visible="false" CssClass="card section">
                        <div class="card-body" style="display: flex; justify-content: space-between; align-items: center; gap: 12px">
                            <div>
                                <div class="top" style="font-weight: 800">No family group selected</div>
                                <div class="page-sub" style="margin: 0">Choose a group to see approvals.</div>
                            </div>
                            <a href="MyGroups.aspx" class="btn-brand">Choose a group</a>
                        </div>
                    </asp:Panel>

                    <!-- Main -->
                    <asp:Panel ID="pnlMain" runat="server" Visible="false">

                        <!-- Filters -->
                        <section class="section card filters">
                            <div class="section-header">
                                <div class="section-title">Filters</div>
                            </div>
                            <div class="card-body">
                                <div class="row g-2 align-items-end">
                                    <div class="col-12 col-md-3">
                                        <label class="form-label">Member</label>
                                        <asp:DropDownList ID="ddlMember" runat="server" CssClass="form-select" />
                                    </div>
                                    <div class="col-6 col-md-2">
                                        <label class="form-label">Status</label>
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="Pending" Value="Pending" Selected="True" />
                                            <asp:ListItem Text="Approved" Value="Approved" />
                                            <asp:ListItem Text="Denied" Value="Denied" />
                                            <asp:ListItem Text="All" Value="All" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-6 col-md-2">
                                        <label class="form-label">From</label>
                                        <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date" />
                                    </div>
                                    <div class="col-6 col-md-2">
                                        <label class="form-label">To</label>
                                        <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date" />
                                    </div>
                                    <div class="col-6 col-md-3" style="display: flex; gap: 8px; align-items: end">
                                        <asp:Button ID="btnFilter" runat="server" CssClass="btn-brand" Text="Apply filters" OnClick="btnFilter_Click" />
                                        <asp:HiddenField ID="hfCanAct" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </section>

                        <!-- Approvals list -->
                        <section class="section card">
                            <div class="section-header">
                                <div class="section-title">Held transactions</div>
                                <a href="FamilyOverview.aspx" class="btn-ghost">Back to overview</a>
                            </div>

                            <div class="card-body list">
                                <asp:Repeater ID="rptApprovals" runat="server"
                                    OnItemCommand="rptApprovals_ItemCommand"
                                    OnItemDataBound="rptApprovals_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="rowline">
                                            <div class="row-left" style="flex: 1">
                                                <div class="top">
                                                    <%# Eval("MemberName") %> • <%# Eval("AccountNickname") %> (<%# Eval("AccountNumberMasked") %>)
                                                </div>
                                                <div class="sub">
                                                    <%# Eval("TransactionDate","{0:yyyy-MM-dd}") %> <%# Eval("TransactionTime") %> •
                                                    <%# Eval("SenderRecipient") %> — <%# Eval("Description") %>
                                                </div>
                                            </div>

                                            <div class="row-right">
                                                <div class="amount-neg">-$<%# String.Format("{0:N2}", Eval("Amount")) %></div>
                                                <div class="mb-2"><%# GetStatusChip(Container.DataItem) %></div>

                                                <!-- Action buttons (visibility handled in ItemDataBound) -->
                                                <asp:LinkButton ID="btnApprove" runat="server"
                                                    CommandName="Approve" CommandArgument='<%# Eval("TransactionId") %>'
                                                    CssClass="btn btn-sm btn-success me-1"
                                                    Visible="false" CausesValidation="false" UseSubmitBehavior="false"
                                                    OnClientClick="return confirm('Approve this transaction?');">
                                                    Approve
                                                </asp:LinkButton>

                                                <asp:LinkButton ID="btnDeny" runat="server"
                                                    CommandName="Deny" CommandArgument='<%# Eval("TransactionId") %>'
                                                    CssClass="btn btn-sm btn-outline-danger"
                                                    Visible="false" CausesValidation="false" UseSubmitBehavior="false"
                                                    OnClientClick="return confirm('Deny this transaction?');">
                                                    Deny
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </ItemTemplate>

                                    <FooterTemplate>
                                        <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                                            <div class="rowline">
                                                <div class="row-left">
                                                    <div class="sub">No transactions match your filters.</div>
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
