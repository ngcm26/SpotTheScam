<%@ Page Title="Accept Invite" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="AcceptInvite.aspx.cs" Inherits="SpotTheScam.User.AcceptInvite" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <style>
    .wrap { max-width: 900px; margin: 24px auto; padding: 0 12px; }
    .card { background: #fff; border: 1px solid #eee; border-radius: 14px; box-shadow: 0 2px 10px rgba(0,0,0,.06); padding: 22px; }
    .row { display:flex; gap:16px; flex-wrap:wrap; }
    .btn { display:inline-block; padding:10px 16px; border-radius:10px; border:1px solid #ddd; text-decoration:none; cursor:pointer; }
    .btn-primary { background:#D36F2D; color:#fff; border-color:#D36F2D; }
    .btn-light { background:#fff; color:#333; }
    .muted { color:#666; }
    .banner { display:flex; gap:16px; align-items:center; }
    .icon-ok, .icon-err {
      width:48px; height:48px; border-radius:50%; display:grid; place-items:center; 
      border:1px solid;
    }
    .icon-ok { background:#EAF7EF; border-color:#D6F0DE; }
    .icon-err { background:#FDECEC; border-color:#F8D3D3; }
    .icon-ok svg { width:24px; height:24px; color:#2E7D32; }
    .icon-err svg { width:24px; height:24px; color:#C62828; }
    .spacer { height:8px; }
  </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="wrap">

    <!-- SUCCESS -->
    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="card">
      <div class="banner">
        <div class="icon-ok">
          <!-- check icon -->
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
            <path d="M9 16.2l-3.5-3.5 1.4-1.4L9 13.4l7.1-7.1 1.4 1.4z"/>
          </svg>
        </div>
        <div>
          <h2 style="margin:0">You’re in!</h2>
          <div class="muted">
            Your membership is now active for
            <strong><asp:Literal ID="litGroupName" runat="server" /></strong>.
          </div>
        </div>
      </div>

      <div class="spacer"></div>
      <div class="row">
        <asp:HyperLink ID="hlGoGroup" runat="server" CssClass="btn btn-primary" Text="Go to this group" />
        <asp:HyperLink ID="hlMyGroups" runat="server" CssClass="btn btn-light" Text="My Groups" NavigateUrl="~/User/MyGroups.aspx" />
        <asp:HyperLink ID="hlFamilyHub" runat="server" CssClass="btn btn-light" Text="Family Hub" NavigateUrl="~/User/MyGroups.aspx" />
      </div>
    </asp:Panel>

    <!-- ERROR / EXPIRED / INVALID -->
    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="card">
      <div class="banner">
        <div class="icon-err">
          <!-- x icon -->
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
            <path d="M18.3 5.71L12 12.01 5.7 5.7 4.29 7.11 10.59 13.4 4.29 19.7 5.7 21.11 12 14.82 18.29 21.1 19.7 19.69 13.41 13.4 19.7 7.11z"/>
          </svg>
        </div>
        <div>
          <h2 style="margin:0">We couldn’t activate your invite</h2>
          <div class="muted"><asp:Literal ID="litError" runat="server" /></div>
        </div>
      </div>

      <div class="spacer"></div>
      <div class="row">
        <asp:HyperLink ID="hlBack" runat="server" CssClass="btn btn-light" Text="Back to My Groups" NavigateUrl="~/User/MyGroups.aspx" />
      </div>
    </asp:Panel>

  </div>
</asp:Content>
