<%@ Page Title="My Transactions" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="UserTransactionLogs.aspx.cs"
    Inherits="SpotTheScam.User.UserTransactionLogs" %>

<%@ Register Src="~/User/Controls/FamilySideNav.ascx" TagPrefix="uc" TagName="FamilySideNav" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --brand: #D36F2D;
            --brand-600: #c5521f;
            --brand-50: #FFF2EC;
            --ink: #1f2937;
            --sub: #6b7280;
            --card: #ffffff;
            --line: #e5e7eb;
            --ok: #065f46;
            --ok-bg: #ecfdf5;
            --warn: #92400e;
            --warn-bg: #fff7ed;
            --bad: #991b1b;
            --bad-bg: #fee2e2;
            --info: #075985;
            --info-bg: #e0f2fe;
            --muted: #6b7280;
        }

        .wrap {
            max-width: 1060px;
            margin: 0 auto;
            padding: 0 16px
        }

        .card {
            background: var(--card);
            border: 1px solid var(--line);
            border-radius: 14px;
            padding: 20px;
            margin: 18px 0;
            box-shadow: 0 6px 20px rgba(0,0,0,.06)
        }

        h2 {
            margin: 0 0 10px 0;
            color: var(--ink);
            font-weight: 700;
            letter-spacing: .2px
        }

        .subtle-bar {
            height: 4px;
            background: linear-gradient(90deg,var(--brand),#f59e0b);
            border-radius: 999px;
            margin: 6px 0 14px
        }

        .row {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
            margin-bottom: 12px
        }

        .input, select {
            padding: 10px;
            border: 1px solid var(--line);
            border-radius: 10px;
            background: #fff;
            transition: border-color .15s, box-shadow .15s
        }

            .input:focus, select:focus {
                outline: none;
                border-color: var(--brand);
                box-shadow: 0 0 0 3px var(--brand-50)
            }

        .muted {
            color: var(--muted)
        }

        a.btn {
            display: inline-block;
            padding: 10px 14px;
            border-radius: 10px;
            background: var(--brand);
            color: #fff;
            text-decoration: none;
            font-weight: 600;
            box-shadow: 0 2px 0 rgba(0,0,0,.08)
        }

            a.btn:hover {
                background: var(--brand-600)
            }

            a.btn:focus {
                outline: 3px solid var(--brand-50);
                outline-offset: 2px
            }

        /* Switch toggle */
        .switch-label {
            display: flex;
            align-items: center;
            gap: 8px;
            cursor: pointer;
            font-weight: 500;
            color: var(--ink);
        }

        .switch {
            position: relative;
            width: 42px;
            height: 24px;
            display: inline-block
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #d1d5db;
            transition: .3s;
            border-radius: 999px;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 18px;
                width: 18px;
                left: 3px;
                bottom: 3px;
                background-color: white;
                transition: .3s;
                border-radius: 50%;
            }

        input:checked + .slider {
            background-color: var(--brand);
        }

            input:checked + .slider:before {
                transform: translateX(18px);
            }

        /* Table styling */
        table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0 10px
        }

        thead th {
            text-align: left;
            color: #374151;
            font-size: .9rem;
            text-transform: uppercase;
            letter-spacing: .04em;
            background: var(--brand-50);
            padding: 10px 12px;
            border: none
        }

        tbody tr {
            background: #fff;
            border: 1px solid var(--line);
            box-shadow: 0 1px 4px rgba(0,0,0,.04)
        }

            tbody tr:hover {
                box-shadow: 0 6px 16px rgba(0,0,0,.07);
                transform: translateY(-1px)
            }

        th, td {
            padding: 12px;
            border-top: 1px solid var(--line)
        }

        /* Amounts */
        .amt-pos {
            color: var(--ok);
            font-weight: 700
        }

        .amt-neg {
            color: var(--bad);
            font-weight: 700
        }

        /* Chips */
        .chip {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: .85rem;
            font-weight: 600;
            border: 1px solid transparent
        }

        .chip-normal {
            background: #eef2ff;
            color: #3730a3;
            border-color: #e0e7ff
        }

        .chip-pending {
            background: var(--warn-bg);
            color: var(--warn);
            border-color: #fde68a
        }

        .chip-approved {
            background: var(--ok-bg);
            color: var(--ok);
            border-color: #bbf7d0
        }

        .chip-denied {
            background: var(--bad-bg);
            color: var(--bad);
            border-color: #fecaca
        }

        .chip-reviewed {
            background: var(--info-bg);
            color: var(--info);
            border-color: #bae6fd
        }

        .chip-held {
            background: #e5e7eb;
            color: #374151;
            border-color: #d1d5db
        }

        /* Severity pill */
        .sev-pill {
            padding: 4px 8px;
            border-radius: 999px;
            font-weight: 700;
            border: 1px solid transparent
        }

        .sev-red {
            background: var(--bad-bg);
            color: var(--bad);
            border-color: #fecaca
        }

        .sev-yellow {
            background: #fef3c7;
            color: var(--warn);
            border-color: #fde68a
        }

        .sev-none {
            background: #e5e7eb;
            color: #374151;
            border-color: #d1d5db
        }

        /* Pager */
        .muted a {
            color: var(--brand);
            text-decoration: none
        }

            .muted a:hover {
                text-decoration: underline
            }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="family-layout">
        <uc:FamilySideNav ID="Side" runat="server" ActiveKey="logs" />
        <main class="family-content">
            <div class="wrap">
                <div class="card">
                    <h2>My Recent Transactions</h2>
                    <div class="subtle-bar"></div>

                    <div class="row">
                        <div>
                            <span class="muted">Account</span><br />
                            <asp:DropDownList ID="ddlAccount" runat="server" CssClass="input" AutoPostBack="true"
                                OnSelectedIndexChanged="FilterChanged" />
                        </div>
                        <div style="align-self: flex-end">
                            <label class="switch-label">
                                <span class="muted">Show flagged only</span>
                                <span class="switch">
                                    <asp:CheckBox ID="chkFlaggedOnly" runat="server"
                                        AutoPostBack="true" OnCheckedChanged="FilterChanged" />
                                    <span class="slider"></span>
                                </span>
                            </label>
                        </div>
                        <div>
                            <span class="muted">Type</span><br />
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="input" AutoPostBack="true"
                                OnSelectedIndexChanged="FilterChanged">
                                <asp:ListItem Text="All" Value="" />
                                <asp:ListItem Text="Withdrawal" Value="Withdrawal" />
                                <asp:ListItem Text="Deposit" Value="Deposit" />
                            </asp:DropDownList>
                        </div>
                        <div style="align-self: flex-end">
                            <a href="AddTransactions.aspx" class="btn">Add Transaction</a>
                        </div>
                    </div>

                    <asp:GridView ID="gvTxns" runat="server" AutoGenerateColumns="False" GridLines="None"
                        CssClass="table" ShowHeaderWhenEmpty="true"
                        OnRowDataBound="gvTxns_RowDataBound" AllowPaging="true" PageSize="15"
                        OnPageIndexChanging="gvTxns_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="WhenText" HeaderText="Date / Time" />
                            <asp:BoundField DataField="AccountNickname" HeaderText="Account" />
                            <asp:BoundField DataField="Description" HeaderText="Description" />
                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate><span runat="server" id="lblAmt"></span></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="TransactionType" HeaderText="Type" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span runat="server" id="lblStatus" class="chip chip-normal">Normal</span>
                                    <span runat="server" id="lblHeld" class="chip chip-held" style="display: none">Held</span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Severity">
                                <ItemTemplate>
                                    <span id="lblSev" runat="server" class="sev-pill sev-none" title='<%# Eval("Notes") %>'>
                                        <%# string.IsNullOrEmpty(Eval("Severity") as string) ? "—" : Eval("Severity") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="muted" />
                    </asp:GridView>

                    <div class="muted" style="margin-top: 8px">
                        Showing latest 50 transactions. Filter by account/type above.
                    </div>
                </div>
            </div>
        </main>
    </div>
</asp:Content>
