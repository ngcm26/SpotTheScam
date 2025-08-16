<%@ Page Title="Bank accounts" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="ConnectBank.aspx.cs" Inherits="SpotTheScam.User.ConnectBank" %>

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

        .page-wrap {
            background: var(--bg);
        }

        .content-wrap {
            max-width: 900px;
            margin: 0 auto;
            padding: 16px 16px 32px;
        }

        .page-title {
            margin: 2px 0 6px;
            font-weight: 800;
            letter-spacing: -.015em
        }

        .page-sub {
            color: var(--sub);
            margin-bottom: 16px
        }

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

        .btn-brand {
            background: var(--brand);
            border-color: var(--brand);
            color: #fff;
            border-radius: 10px;
            padding: 10px 14px;
            font-weight: 800
        }

            .btn-brand:hover {
                background: var(--brand-600)
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

        .lead {
            font-size: 1.05rem;
            color: var(--sub)
        }

        /* Account list */
        .acct {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 12px 16px;
            border-top: 1px solid #f1f5f9
        }

            .acct:first-child {
                border-top: 0
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
            font-size: .85rem
        }

        .acct-name {
            font-weight: 800;
            letter-spacing: -.01em
        }

        .acct-sub {
            color: var(--sub);
            font-size: .92rem
        }

        .amt {
            font-weight: 900;
            letter-spacing: -.01em
        }

        .row-right {
            text-align: right;
            min-width: 160px
        }

        .row-actions {
            margin-top: 6px
        }

        .msg-card {
            border-left: 4px solid var(--brand)
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(12,1fr);
            gap: 12px
        }

        .col-6 {
            grid-column: span 12
        }

        @media(min-width:768px) {
            .col-6 {
                grid-column: span 6
            }
        }

        .form-label {
            font-weight: 700;
            color: var(--ink)
        }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrap">
        <div class="family-layout">
            <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="connect" />

            <main class="family-content">
                <div class="content-wrap">
                    <h2 class="page-title">Bank accounts</h2>
                    <div class="page-sub">Add an account you control. You can record manual transactions anytime.</div>

                    <!-- flash -->
                    <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="card msg-card">
                        <div class="card-body">
                            <asp:Label ID="lblMsg" runat="server" />
                        </div>
                    </asp:Panel>

                    <!-- QUICK START (shown only when user has 0 accounts) -->
                    <asp:Panel ID="pnlQuickStart" runat="server" Visible="false" CssClass="section card">
                        <div class="card-body" style="display: flex; align-items: center; justify-content: space-between; gap: 12px">
                            <div>
                                <div class="page-title" style="margin: 0">Add your first account</div>
                                <div class="lead">We’ll get you started instantly so you can try the features.</div>
                            </div>
                            <asp:Button ID="btnQuickAdd" runat="server" CssClass="btn-brand" Text="Add account" OnClick="btnQuickAdd_Click" />
                        </div>
                    </asp:Panel>

                    <!-- MANUAL ADD (shown when user already has accounts) -->
                    <asp:Panel ID="pnlAddForm" runat="server" Visible="false" CssClass="section card">
                        <div class="section-header">
                            <div class="section-title">Add account</div>
                        </div>
                        <div class="card-body">
                            <div class="grid">
                                <div class="col-6">
                                    <label class="form-label">Bank</label>
                                    <asp:DropDownList ID="ddlBank" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="DBS" />
                                        <asp:ListItem Text="OCBC" />
                                        <asp:ListItem Text="UOB" />
                                        <asp:ListItem Text="Maybank" />
                                        <asp:ListItem Text="HSBC" />
                                        <asp:ListItem Text="Standard Chartered" />
                                        <asp:ListItem Text="Other" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col-6">
                                    <label class="form-label">Account nickname</label>
                                    <asp:TextBox ID="txtNickname" runat="server" CssClass="form-control" placeholder="e.g., Main, Savings" />
                                </div>
                                <div class="col-6">
                                    <label class="form-label">Last 4 digits</label>
                                    <asp:TextBox ID="txtLast4" runat="server" CssClass="form-control" MaxLength="4" placeholder="1234" />
                                </div>
                                <div class="col-6">
                                    <label class="form-label">Starting balance</label>
                                    <asp:TextBox ID="txtStartBalance" runat="server" CssClass="form-control" placeholder="0.00" />
                                </div>
                            </div>
                            <div style="margin-top: 12px; display: flex; gap: 8px">
                                <asp:Button ID="btnAdd" runat="server" CssClass="btn-brand" Text="Add account" OnClick="btnAdd_Click" />
                                <a class="btn-ghost" href="AddTransactions.aspx">Record a transaction</a>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Existing accounts -->
                    <section class="section card">
                        <div class="section-header">
                            <div class="section-title">Your accounts</div>
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="rptAccounts" runat="server" OnItemCommand="rptAccounts_ItemCommand">
                                <ItemTemplate>
                                    <div class="acct">
                                        <div>
                                            <div class="bank-chip"><%# Eval("BankName") %></div>
                                            <div class="acct-name"><%# Eval("AccountNickname") %></div>
                                            <div class="acct-sub"><%# Eval("BankName") %> — <%# Eval("AccountNumberMasked") %></div>
                                        </div>
                                        <div class="row-right">
                                            <div class="amt">$<%# String.Format("{0:N2}", Eval("Balance")) %></div>
                                            <div class="row-actions">
                                                <a class="btn-ghost" href='<%# "UserTransactionLogs.aspx?accountId=" + Eval("AccountId") %>'>View transactions</a>
                                                <asp:LinkButton ID="btnDelete" runat="server"
                                                    CssClass="btn btn-sm btn-outline-danger"
                                                    CommandName="Delete" CommandArgument='<%# Eval("AccountId") %>'
                                                    OnClientClick="return confirm('Delete this account AND all its transactions? This cannot be undone.');">
                                                    Delete
                                                </asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                                        <div class="acct">
                                            <div class="acct-sub">You don’t have any accounts yet.</div>
                                        </div>
                                    </asp:PlaceHolder>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </section>

                </div>
            </main>
        </div>
    </div>
</asp:Content>
