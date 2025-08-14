<%@ Page Title="My Transactions" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="UserTransactionLogs.aspx.cs"
    Inherits="SpotTheScam.User.UserTransactionLogs" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .wrap{max-width:1000px;margin:0 auto}
        .card{background:#fff;border:1px solid #eee;border-radius:12px;padding:20px;margin:16px 0}
        .row{display:flex;gap:12px;flex-wrap:wrap;margin-bottom:12px}
        .input,select{padding:10px;border:1px solid #d1d5db;border-radius:8px}
        .chip{display:inline-block;padding:4px 10px;border-radius:999px;font-size:.85rem}
        .chip-normal{background:#eef2ff;color:#3730a3}
        .chip-pending{background:#fff7ed;color:#92400e}
        .chip-approved{background:#ecfdf5;color:#065f46}
        .chip-denied{background:#fee2e2;color:#991b1b}
        .chip-reviewed{background:#e0f2fe;color:#075985}
        .amt-pos{color:#065f46;font-weight:700}
        .amt-neg{color:#991b1b;font-weight:700}
        table{width:100%}
        th,td{padding:10px 8px;border-bottom:1px solid #f2f2f2}
        th{text-align:left;color:#374151}
        .muted{color:#6b7280}
        a.btn{display:inline-block;padding:8px 12px;border-radius:8px;background:#2563eb;color:#fff;text-decoration:none}
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="wrap">
        <div class="card">
            <h2 style="margin:0 0 10px 0;">My Recent Transactions</h2>
            <div class="row">
                <div>
                    <span class="muted">Account</span><br />
                    <asp:DropDownList ID="ddlAccount" runat="server" CssClass="input" AutoPostBack="true"
                        OnSelectedIndexChanged="FilterChanged" />
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
                <div style="align-self:flex-end">
                    <a href="AddTransactions.aspx" class="btn">Add Transaction</a>
                </div>
            </div>

            <asp:GridView ID="gvTxns" runat="server" AutoGenerateColumns="False" GridLines="None"
                OnRowDataBound="gvTxns_RowDataBound" AllowPaging="true" PageSize="15"
                OnPageIndexChanging="gvTxns_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="WhenText" HeaderText="Date / Time" />
                    <asp:BoundField DataField="AccountNickname" HeaderText="Account" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <span runat="server" id="lblAmt"></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TransactionType" HeaderText="Type" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span runat="server" id="lblStatus" class="chip chip-normal">Normal</span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <a href='<%# "TransactionDetails.aspx?id=" + Eval("TransactionId") %>'>Details</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="muted" />
            </asp:GridView>

            <div class="muted" style="margin-top:8px">
                Showing latest 50 transactions. Filter by account/type above.
            </div>
        </div>
    </div>
</asp:Content>
