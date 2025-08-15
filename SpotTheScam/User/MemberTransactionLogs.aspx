<%@ Page Title="Member Transactions" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="MemberTransactionLogs.aspx.cs"
    Inherits="SpotTheScam.User.MemberTransactionLogs" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap {
            max-width: 1000px;
            margin: 24px auto;
            padding: 0 12px
        }

        .card {
            background: #fff;
            border: 1px solid #eee;
            border-radius: 12px;
            padding: 18px;
            margin-bottom: 18px
        }

        table {
            width: 100%
        }

        th, td {
            padding: 10px 8px;
            border-bottom: 1px solid #f2f2f2;
            text-align: left
        }

        .muted {
            color: #6b7280
        }

        .btn {
            display: inline-block;
            padding: 8px 12px;
            border-radius: 8px;
            background: #2563eb;
            color: #fff;
            text-decoration: none
        }

        .sev-pill {
            padding: 4px 8px;
            border-radius: 999px;
            font-weight: 600
        }

        .sev-red {
            background: #fee2e2;
            color: #991b1b
        }

        .sev-yellow {
            background: #fef3c7;
            color: #92400e
        }

        .sev-none {
            background: #e5e7eb;
            color: #374151
        }

        .chip {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: .85rem
        }

        .chip-normal {
            background: #eef2ff;
            color: #3730a3
        }

        .chip-pending {
            background: #fff7ed;
            color: #92400e
        }

        .chip-approved {
            background: #ecfdf5;
            color: #065f46
        }

        .chip-denied {
            background: #fee2e2;
            color: #991b1b
        }

        .chip-reviewed {
            background: #e0f2fe;
            color: #075985
        }

        .amt-pos {
            color: #065f46;
            font-weight: 700
        }

        .amt-neg {
            color: #991b1b;
            font-weight: 700
        }

        .msg {
            padding: 10px 12px;
            border-radius: 8px;
            margin-bottom: 12px;
            font-weight: 600
        }

        .ok {
            background: #e8f5e9;
            color: #1b5e20;
            border: 1px solid #c8e6c9
        }

        .err {
            background: #ffebee;
            color: #b71c1c;
            border: 1px solid #ffcdd2
        }

        .chip-held {
            background: #e5e7eb;
            color: #374151;
        }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="wrap">
        <div class="card">
            <h2>Transactions —
                <asp:Label ID="lblMember" runat="server" />
                (<asp:Label ID="lblAccount" runat="server" />)</h2>
            <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
                <asp:Label ID="lblMsg" runat="server" />
            </asp:Panel>

            <div style="margin-bottom: 10px">
                <asp:CheckBox ID="chkFlaggedOnly" runat="server" Text="Show flagged only" AutoPostBack="true" OnCheckedChanged="FilterChanged" />
            </div>

            <asp:GridView ID="gvTxns" runat="server" AutoGenerateColumns="False" GridLines="None"
                OnRowDataBound="gvTxns_RowDataBound" AllowPaging="true" PageSize="15"
                OnPageIndexChanging="gvTxns_PageIndexChanging" OnRowCommand="gvTxns_RowCommand">
                <Columns>
                    <asp:BoundField DataField="WhenText" HeaderText="Date / Time" />
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
                            <span id="lblSev" runat="server" class="sev-pill" title='<%# Eval("Notes") %>'>
                                <%# string.IsNullOrEmpty(Eval("Severity") as string) ? "—" : Eval("Severity") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnApprove" runat="server" CommandName="Approve"
                                CommandArgument='<%# Eval("TransactionId") %>' Text="Approve" CssClass="btn btn-approve" />
                            &nbsp;
        <asp:LinkButton ID="btnDeny" runat="server" CommandName="Deny"
            CommandArgument='<%# Eval("TransactionId") %>' Text="Deny" CssClass="btn btn-deny"
            OnClientClick="return confirm('Deny this transfer?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div style="margin-top: 16px">
                <asp:HyperLink ID="hlBack" runat="server" CssClass="btn" Text="Back to accounts" />
            </div>

        </div>
    </div>
</asp:Content>
