<%@ Page Title="Add Transaction" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="AddTransactions.aspx.cs"
    Inherits="SpotTheScam.User.AddTransactions" %>
<%@ Register Src="~/User/Controls/FamilySideNav.ascx" TagPrefix="uc" TagName="FamilySideNav" %>

<asp:Content ID="HeadC" ContentPlaceHolderID="head" runat="server">
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
      --bad:#991b1b; --bad-bg:#fee2e2; --bad-br:#fecaca;
    }

    /* Layout for sidebar + content */
    .family-layout{display:grid;grid-template-columns:240px 1fr;gap:16px}
    @media (max-width:1023.98px){ .family-layout{grid-template-columns:1fr} }
    .family-content{min-width:0;}

    .card{
      background:var(--card); border:1px solid var(--line); border-radius:14px;
      padding:24px; max-width:820px; margin:28px auto; box-shadow:0 6px 20px rgba(0,0,0,.06);
    }
    h2{margin:0;color:var(--ink);font-weight:700;letter-spacing:.2px}
    .subtle-bar{height:4px;background:linear-gradient(90deg,var(--brand),#f59e0b);border-radius:999px;margin:10px 0 16px}

    .row{display:flex;gap:16px;flex-wrap:wrap}
    .col{flex:1 1 280px}

    .label{font-weight:600;margin:8px 0 6px;color:var(--ink)}
    .formc{
      width:100%;padding:12px;border:1px solid var(--line);border-radius:10px;background:#fff;
      transition:border-color .15s, box-shadow .15s;
    }
    .formc:focus{outline:none;border-color:var(--brand);box-shadow:0 0 0 3px var(--brand-50)}

    /* Buttons */
    .btn{display:inline-block;padding:12px 18px;border-radius:10px;border:0;font-weight:600;cursor:pointer}
    .btn:focus{outline:3px solid var(--brand-50);outline-offset:2px}
    .btn-primary,.btn.primary{background:var(--brand);color:#fff;box-shadow:0 2px 0 rgba(0,0,0,.08)}
    .btn-primary:hover,.btn.primary:hover{background:var(--brand-600)}
    .btn-secondary,.btn.secondary{background:#6b7280;color:#fff}

    /* Alerts */
    .alert{padding:12px 14px;border-radius:10px;margin-bottom:16px;font-weight:600;border:1px solid transparent}
    .alert-ok,.alert.ok{background:var(--ok-bg);color:var(--ok);border-color:var(--ok-br)}
    .alert-bad,.alert.err{background:var(--bad-bg);color:var(--bad);border-color:var(--bad-br)}

    /* Recent list */
    .list{margin-top:20px}
    .txn-list{list-style:none;margin:0;padding:0}
    .txn-item{
      display:flex;justify-content:space-between;align-items:center;gap:12px;
      padding:12px 14px;border:1px solid var(--line);border-radius:10px;background:#fff;
      box-shadow:0 1px 4px rgba(0,0,0,.04); margin-bottom:10px
    }
    .txn-item:hover{box-shadow:0 6px 16px rgba(0,0,0,.07);transform:translateY(-1px)}
    .txn-main{display:flex;flex-wrap:wrap;gap:8px;align-items:center}
    .date{font-weight:700}
    .type{padding:2px 10px;border-radius:999px;font-size:12px;border:1px solid #f7c8b6;background:#fdece5;color:#7a2a10}
    .amount{font-variant-numeric:tabular-nums;font-weight:700;color:#111827}
    .chip{display:inline-block;padding:2px 8px;border-radius:999px;font-size:12px;border:1px solid var(--line);color:#374151}
  </style>
</asp:Content>

<asp:Content ID="BodyC" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="family-layout">
    <!-- Sidebar -->
    <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="add" />

    <!-- Page content -->
    <main class="family-content">
      <div class="card">
        <h2>Add Transaction</h2>
        <div class="subtle-bar"></div>

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert">
          <asp:Label ID="lblMsg" runat="server" />
        </asp:Panel>

        <div class="row">
          <div class="col">
            <div class="label">Account</div>
            <asp:DropDownList ID="ddlAccount" runat="server" CssClass="formc"
              AutoPostBack="true" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" />
          </div>
          <div class="col">
            <div class="label">Type</div>
            <asp:DropDownList ID="ddlType" runat="server" CssClass="formc">
              <asp:ListItem Text="Withdrawal" Value="Withdrawal" Selected="True" />
              <asp:ListItem Text="Deposit" Value="Deposit" />
            </asp:DropDownList>
          </div>
        </div>

        <div class="row">
          <div class="col">
            <div class="label">Amount</div>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="formc" TextMode="Number" />
          </div>
          <div class="col">
            <div class="label">Recipient / Sender (optional)</div>
            <asp:TextBox ID="txtRecipient" runat="server" CssClass="formc" MaxLength="255" />
          </div>
        </div>

        <div class="label">Description</div>
        <asp:TextBox ID="txtDescription" runat="server" CssClass="formc" MaxLength="255" />

        <div style="margin-top:16px">
          <asp:Button ID="btnSubmit" runat="server" Text="Save Transaction"
            CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
          <a href="ConnectBank.aspx" class="btn btn-secondary">Connect Bank</a>
        </div>

        <div class="list">
          <h3 style="margin-top:24px">Recent Transactions (selected account)</h3>
          <asp:Repeater ID="rptRecent" runat="server">
            <HeaderTemplate><ul class="txn-list"></HeaderTemplate>
            <ItemTemplate>
              <li class="txn-item">
                <div class="txn-main">
                  <span class="date"><%# Eval("TransactionDate","{0:dd MMM}") %></span> —
                  <span><%# Eval("Description") %></span>
                  <span class="type"><%# Eval("TransactionType") %></span>
                  <span class="chip">Bal: $<%# string.Format("{0:N2}", Eval("BalanceAfterTransaction")) %></span>
                </div>
                <div class="amount">
                  $<%# string.Format("{0:N2}", Eval("Amount")) %>
                </div>
              </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
          </asp:Repeater>
        </div>
      </div>
    </main>
  </div>
</asp:Content>
