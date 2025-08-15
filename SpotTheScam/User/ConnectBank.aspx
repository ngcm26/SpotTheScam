<%@ Page Title="Connect Bank" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="ConnectBank.aspx.cs" Inherits="SpotTheScam.User.ConnectBank" %>
<%@ Register Src="~/User/Controls/FamilySideNav.ascx" TagPrefix="uc" TagName="FamilySideNav" %>

<asp:Content ID="headC" ContentPlaceHolderID="head" runat="server">
  <style>
    :root{
      --brand:#D36F2D;
      --brand-600:#c5521f;
      --brand-50:#FFF2EC;
      --ink:#1f2937;
      --sub:#6b7280;
      --card:#ffffff;
      --line:#e5e7eb;
      --ok:#0f5132; --ok-bg:#e8f5e9; --ok-br:#c8e6c9;
      --warn:#92400e; --warn-bg:#fef3c7; --warn-br:#fde68a;
      --danger:#b91c1c; --danger-bg:#fee2e2; --danger-br:#fecaca;
    }

    /* Layout for sidebar + content */
    .family-layout{display:grid;grid-template-columns:240px 1fr;gap:16px}
    @media (max-width:1023.98px){ .family-layout{grid-template-columns:1fr} }
    .family-content{min-width:0;}

    .card{
      background:var(--card); border:1px solid var(--line); border-radius:14px;
      padding:24px; max-width:760px; margin:28px auto; box-shadow:0 6px 20px rgba(0,0,0,.06);
    }
    h2{margin:0;color:var(--ink);font-weight:700;letter-spacing:.2px}
    .subtle-bar{height:4px;background:linear-gradient(90deg,var(--brand),#f59e0b);border-radius:999px;margin:10px 0 16px}
    p.muted{color:var(--sub);margin-top:0}

    /* Buttons */
    .btn{display:inline-block;padding:12px 18px;border-radius:10px;border:0;font-weight:600;cursor:pointer}
    .btn:focus{outline:3px solid var(--brand-50);outline-offset:2px}
    .btn.primary{background:var(--brand);color:#fff;box-shadow:0 2px 0 rgba(0,0,0,.08)}
    .btn.primary:hover{background:var(--brand-600)}
    .btn.danger{background:var(--danger);color:#fff}
    .btn.danger:hover{filter:brightness(.95)}

    /* Back-compat for existing classes */
    .btn-primary{background:var(--brand);color:#fff}
    .btn-danger{background:var(--danger);color:#fff}

    .stack>*{margin-right:12px;margin-bottom:12px}

    /* Alerts */
    .alert{padding:12px 14px;border-radius:10px;margin-bottom:16px;font-weight:600;border:1px solid transparent}
    .alert.ok{background:var(--ok-bg);color:var(--ok);border-color:var(--ok-br)}
    .alert.warn{background:var(--warn-bg);color:var(--warn);border-color:var(--warn-br)}
    .alert.err{background:var(--danger-bg);color:var(--danger);border-color:var(--danger-br)}

    hr{border:none;border-top:1px solid var(--line);margin:18px 0}

    /* Accounts list */
    .acct-list{list-style:none;padding:0;margin:0}
    .acct-item{
      display:flex;justify-content:space-between;align-items:center;
      padding:12px 14px;border:1px solid var(--line);border-radius:10px;background:#fff;
      box-shadow:0 1px 4px rgba(0,0,0,.04); margin-bottom:10px
    }
    .acct-item:hover{box-shadow:0 6px 16px rgba(0,0,0,.07);transform:translateY(-1px)}
    .acct-meta{color:var(--sub);font-size:.95rem}
    .balance{font-variant-numeric:tabular-nums;font-weight:700;color:#111827}
  </style>
</asp:Content>

<asp:Content ID="bodyC" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="family-layout">
    <!-- Sidebar -->
    <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="connect" />

    <!-- Page content -->
    <main class="family-content">
      <div class="card">
        <h2>Connect a Mock Bank</h2>
        <div class="subtle-bar"></div>
        <p class="muted">One click will create 2 realistic accounts and seed recent transactions for your user only.</p>

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert">
          <asp:Label ID="lblMsg" runat="server" />
        </asp:Panel>

        <div class="stack">
          <asp:Button ID="btnSeed" runat="server" Text="Create Mock Accounts"
              CssClass="btn primary" OnClick="btnSeed_Click" />
          <asp:Button ID="btnWipe" runat="server" Text="Remove My Mock Data"
              CssClass="btn danger" OnClick="btnWipe_Click" />
        </div>

        <hr />
        <h4>Your current accounts</h4>
        <asp:Repeater ID="rptAccounts" runat="server">
          <HeaderTemplate><ul class="acct-list"></HeaderTemplate>
          <ItemTemplate>
            <li class="acct-item">
              <div>
                <strong><%# Eval("AccountNickname") %></strong>
                <div class="acct-meta">
                  <%# Eval("BankName") %> — <%# Eval("AccountNumberMasked") %>
                </div>
              </div>
              <div class="balance">
                $<%# string.Format("{0:N2}", Eval("Balance")) %>
              </div>
            </li>
          </ItemTemplate>
          <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
      </div>
    </main>
  </div>
</asp:Content>
