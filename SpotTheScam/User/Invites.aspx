<%@ Page Title="My Invites" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="Invites.aspx.cs" Inherits="SpotTheScam.User.Invites" %>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="wrap">
    <div class="card">
      <h2>Pending Group Invites</h2>

      <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
        <asp:Label ID="lblMsg" runat="server" />
      </asp:Panel>

      <asp:GridView ID="gvInvites" runat="server" AutoGenerateColumns="false" CssClass="table"
          OnRowCommand="gvInvites_RowCommand" OnRowDataBound="gvInvites_RowDataBound" EmptyDataText="No pending invites.">
        <Columns>
          <asp:BoundField DataField="GroupName" HeaderText="Group" />
          <asp:BoundField DataField="Role" HeaderText="Invited As" />
          <asp:BoundField DataField="InvitedAt" HeaderText="Invited On" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
          <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
              <asp:LinkButton ID="lnkAccept" runat="server" CommandName="Accept" CommandArgument='<%# Eval("GroupId") %>' Text="Accept" />
              &nbsp;&nbsp;
              <asp:LinkButton ID="lnkDecline" runat="server" CommandName="Decline" CommandArgument='<%# Eval("GroupId") %>' Text="Decline" />
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>
    </div>
  </div>
</asp:Content>
