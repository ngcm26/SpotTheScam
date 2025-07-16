<%@ Page Title="Transaction Details" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="TransactionDetails.aspx.cs" Inherits="SpotTheScam.User.TransactionDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Transaction Details</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        .details-card {
            background: #fff;
            border-radius: 0.75rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            padding: 2rem;
            max-width: 600px;
            margin: 2rem auto;
        }
        .details-label {
            font-weight: 600;
            color: #374151;
        }
        .details-value {
            color: #1f2937;
        }
        .flag-icon {
            font-size: 1.5rem;
            vertical-align: middle;
        }
        .action-btn {
            background-color: #4f46e5;
            color: #fff;
            padding: 0.75rem 1.5rem;
            border-radius: 0.5rem;
            margin-right: 1rem;
            border: none;
            cursor: pointer;
            transition: background-color 0.2s;
        }
        .action-btn:hover {
            background-color: #4338ca;
        }
        .danger-btn {
            background-color: #e53e3e;
        }
        .danger-btn:hover {
            background-color: #c53030;
        }
        .alert {
            padding: 1rem;
            border-radius: 0.5rem;
            margin-bottom: 1rem;
            font-weight: 500;
        }
        .alert-success { background: #d1fae5; color: #065f46; }
        .alert-danger { background: #fee2e2; color: #991b1b; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="details-card">
        <h1 class="text-2xl font-bold mb-4">Transaction Details</h1>
        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-4">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>
        <table class="w-full mb-6">
            <tr>
                <td class="details-label">Flagged Status:</td>
                <td class="details-value">
                    <asp:Literal ID="ltFlaggedStatus" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Severity:</td>
                <td class="details-value">
                    <asp:Literal ID="ltSeverity" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Date:</td>
                <td class="details-value">
                    <asp:Literal ID="ltDate" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Time:</td>
                <td class="details-value">
                    <asp:Literal ID="ltTime" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Description:</td>
                <td class="details-value">
                    <asp:Literal ID="ltDescription" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Type:</td>
                <td class="details-value">
                    <asp:Literal ID="ltType" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Amount:</td>
                <td class="details-value">
                    <asp:Literal ID="ltAmount" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Sender/Recipient:</td>
                <td class="details-value">
                    <asp:Literal ID="ltSenderRecipient" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Balance After:</td>
                <td class="details-value">
                    <asp:Literal ID="ltBalanceAfter" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Bank:</td>
                <td class="details-value">
                    <asp:Literal ID="ltBankName" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="details-label">Account:</td>
                <td class="details-value">
                    <asp:Literal ID="ltAccountNickname" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <div class="flex flex-wrap gap-2">
            <asp:Button ID="btnAlertFamily" runat="server" Text="Alert Trusted Family Member" CssClass="action-btn" OnClick="btnAlertFamily_Click" />
            <asp:Button ID="btnContactBank" runat="server" Text="Contact Bank" CssClass="action-btn danger-btn" OnClick="btnContactBank_Click" />
            <asp:Button ID="btnBack" runat="server" Text="Back to Transactions" CssClass="action-btn" OnClick="btnBack_Click" />
        </div>
    </div>
</asp:Content>
