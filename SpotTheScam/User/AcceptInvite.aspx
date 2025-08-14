<%@ Page Title="Accept Invite" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="AcceptInvite.aspx.cs" Inherits="SpotTheScam.User.AcceptInvite" %>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="wrap">
    <div class="card">
      <asp:Panel ID="pOk"  runat="server" Visible="false" CssClass="msg ok"><asp:Label ID="lblOk"  runat="server" /></asp:Panel>
      <asp:Panel ID="pErr" runat="server" Visible="false" CssClass="msg err"><asp:Label ID="lblErr" runat="server" /></asp:Panel>
    </div>
  </div>
</asp:Content>
