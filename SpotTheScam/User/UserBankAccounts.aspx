<%@ Page Title="My Bank Accounts" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserBankAccounts.aspx.cs" Inherits="SpotTheScam.User.UserBankAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2>My Bank Accounts</h2>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert" EnableViewState="false">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <div class="card mb-4">
            <div class="card-header">
                Add New Bank Account
            </div>
            <div class="card-body">
                <div class="form-group">
                    <asp:Label ID="lblBankName" runat="server" Text="Bank Name:"></asp:Label>
                    <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBankName" runat="server" ControlToValidate="txtBankName"
                        ErrorMessage="Bank Name is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblAccountType" runat="server" Text="Account Type:"></asp:Label>
                    <asp:TextBox ID="txtAccountType" runat="server" CssClass="form-control" Text="Savings"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number (Last 4 digits or Full if secure):"></asp:Label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber"
                        ErrorMessage="Account Number is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblAccountNickname" runat="server" Text="Account Nickname (e.g., 'My Main Savings'):"></asp:Label>
                    <asp:TextBox ID="txtAccountNickname" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblBalance" runat="server" Text="Initial Balance:"></asp:Label>
                    <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBalance" runat="server" ControlToValidate="txtBalance"
                        ErrorMessage="Balance is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvBalance" runat="server" ControlToValidate="txtBalance"
                        Type="Double" Operator="DataTypeCheck" ErrorMessage="Please enter a valid number for balance."
                        ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                </div>
                <asp:Button ID="btnAddAccount" runat="server" Text="Add Account" CssClass="btn btn-primary" OnClick="btnAddAccount_Click" />
            </div>
        </div>

        <h3>Existing Accounts</h3>
        <asp:GridView ID="gvBankAccounts" runat="server" AutoGenerateColumns="False" DataKeyNames="AccountId"
            CssClass="table table-striped table-bordered" OnRowEditing="gvBankAccounts_RowEditing"
            OnRowCancelingEdit="gvBankAccounts_RowCancelingEdit" OnRowUpdating="gvBankAccounts_RowUpdating"
            OnRowDeleting="gvBankAccounts_RowDeleting">
            <Columns>
                <asp:BoundField DataField="BankName" HeaderText="Bank Name" />
                <asp:BoundField DataField="AccountType" HeaderText="Type" />
                <asp:BoundField DataField="AccountNickname" HeaderText="Nickname" />
                <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" />
                <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:C}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" Text="Edit" CssClass="btn btn-sm btn-info me-2"></asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this account?');" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="btn btn-sm btn-success me-2"></asp:LinkButton>
                        <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="btn btn-sm btn-secondary"></asp:LinkButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>