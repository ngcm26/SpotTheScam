 <%@ Page Title="Pending Family Group Invites" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="FamilyGroupInvites.aspx.cs" Inherits="SpotTheScam.User.FamilyGroupInvites" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Pending Family Group Invites</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto p-6 max-w-xl">
        <div class="main-card mb-6">
            <h2 class="text-2xl font-semibold text-gray-800 mb-4">
                Pending Family Group Invitations
            </h2>
            <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-4">
                <asp:Label ID="AlertMessage" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Repeater ID="rptInvites" runat="server" OnItemCommand="rptInvites_ItemCommand">
                <HeaderTemplate>
                    <div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="bg-white rounded-lg shadow mb-4 flex items-center justify-between px-4 py-3">
                        <div>
                            <span class="text-lg font-semibold text-gray-800"><i class="fas fa-users mr-2 text-indigo-500"></i><%# Eval("GroupName") %></span>
                        </div>
                        <div>
                            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-success" CommandName="Approve" CommandArgument='<%# Eval("GroupId") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoInvites" runat="server" Visible="false" CssClass="text-center py-4">
                <span class="text-gray-500">You have no pending group invitations.</span>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
