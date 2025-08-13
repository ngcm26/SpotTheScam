<%@ Page Title="New Transaction" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="AddManualTransaction.aspx.cs" Inherits="SpotTheScam.User.AddManualTransaction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%-- Add Font Awesome for icons --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

    <style>
        .transaction-card {
            max-width: 600px;
            margin: 2rem auto;
            background-color: #ffffff;
            padding: 2rem;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            border: 1px solid #e0e0e0;
        }

        .transaction-header {
            text-align: center;
            margin-bottom: 2rem;
        }

            .transaction-header h2 {
                font-size: 1.8rem;
                color: #333;
                font-weight: 600;
            }

            .transaction-header p {
                color: #777;
            }

        .input-group-prepend .input-group-text {
            background-color: #f8f9fa;
            border-right: 0;
            width: 42px; /* Fixed width for alignment */
            justify-content: center;
        }

        .form-control {
            height: 48px;
            border-left: 0;
        }
        
        /* Style adjustments for dropdown */
        select.form-control {
            border-left: 1px solid #ced4da;
        }
        
        .btn-submit-transaction {
            width: 100%;
            padding: 12px;
            font-size: 1.1rem;
            font-weight: bold;
            background-color: #007bff;
            border: none;
        }
        
        .validation-summary {
            margin-bottom: 1rem;
            border-left: 5px solid #dc3545; /* Red border */
            padding: 1rem;
            background-color: #f8d7da;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="transaction-card">
        <div class="transaction-header">
            <h2><i class="fas fa-paper-plane" style="color:#007bff;"></i> Make a Transaction</h2>
            <p>Simulate a new payment or transfer.</p>
        </div>

        <%-- Panel for success/error messages --%>
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert" role="alert">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>
        
        <%-- Validation Summary to show all errors in one place --%>
        <asp:ValidationSummary ID="valSummary" runat="server" 
            HeaderText="Please correct the following errors:"
            CssClass="validation-summary text-danger" 
            DisplayMode="BulletList" />

        <%-- From Account --%>
        <div class="form-group">
            <label for="<%= ddlMyAccounts.ClientID %>">From Account</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="fas fa-university"></i></span>
                </div>
                <asp:DropDownList ID="ddlMyAccounts" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
             <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlMyAccounts" 
                ErrorMessage="Please select an account." ForeColor="Red" Display="Dynamic" 
                InitialValue="" CssClass="ml-1" />
        </div>

        <%-- Amount --%>
        <div class="form-group">
            <label for="<%= txtAmount.ClientID %>">Amount</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><b>S$</b></span>
                </div>
                <asp:TextBox ID="txtAmount" runat="server" TextMode="Number" step="0.01" CssClass="form-control" placeholder="e.g., 50.00"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmount" ErrorMessage="Amount is required." ForeColor="Red" Display="Dynamic" CssClass="ml-1" />
            <asp:CompareValidator runat="server" ControlToValidate="txtAmount" Operator="GreaterThan" ValueToCompare="0" Type="Currency" ErrorMessage="Amount must be greater than 0." ForeColor="Red" Display="Dynamic" CssClass="ml-1" />
        </div>

        <%-- Description --%>
        <div class="form-group">
            <label for="<%= txtDescription.ClientID %>">Description / Recipient</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="fas fa-comment-dots"></i></span>
                </div>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="e.g., Online Shopping"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" ErrorMessage="Description is required." ForeColor="Red" Display="Dynamic" CssClass="ml-1" />
        </div>

        <br />

        <asp:Button ID="btnSubmitTransaction" runat="server" Text="Submit Transaction" OnClick="btnSubmitTransaction_Click" CssClass="btn btn-primary btn-submit-transaction" />
    </div>
</asp:Content>