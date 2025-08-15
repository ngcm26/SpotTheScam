<%@ Page Title="My Invites" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="Invites.aspx.cs" Inherits="SpotTheScam.User.Invites" %>
<%@ Register Src="~/User/Controls/FamilySideNav.ascx" TagPrefix="uc" TagName="FamilySideNav" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --brand: #D36F2D;
            --brand-600: #c5521f;
            --brand-50: #FFF2EC;
            --ink: #1f2937;
            --sub: #6b7280;
            --card: #ffffff;
            --line: #e5e7eb;
            --ok: #0f5132;
            --ok-bg: #e8f5e9;
            --ok-br: #c8e6c9;
            --err: #842029;
            --err-bg: #ffebee;
            --err-br: #ffcdd2;
        }

        /* Layout for sidebar + content */
        .family-layout { display: grid; grid-template-columns: 240px 1fr; gap: 16px; }
        @media (max-width:1023.98px){ .family-layout { grid-template-columns: 1fr; } }
        .family-content { min-width: 0; }

        .wrap { max-width: 1060px; margin: 28px auto; padding: 0 16px }
        .card {
            background: var(--card);
            border: 1px solid var(--line);
            border-radius: 14px;
            box-shadow: 0 6px 20px rgba(0,0,0,.06);
            padding: 20px;
        }

        h2 { margin: 0; color: var(--ink); font-weight: 700; letter-spacing: .2px }
        .subtle-bar {
            height: 4px;
            background: linear-gradient(90deg,var(--brand),#f59e0b);
            border-radius: 999px;
            margin: 10px 0 16px
        }

        /* Alerts */
        .msg {
            padding: 12px 14px;
            border-radius: 10px;
            margin-bottom: 14px;
            font-weight: 600;
            border: 1px solid transparent
        }
        .ok { background: var(--ok-bg); color: var(--ok); border-color: var(--ok-br) }
        .err { background: var(--err-bg); color: var(--err); border-color: var(--err-br) }

        /* Table as cards */
        .table { width: 100%; border-collapse: separate; border-spacing: 0 10px }
        .table thead th {
            font-size: .9rem;
            text-transform: uppercase;
            letter-spacing: .04em;
            color: #374151;
            background: var(--brand-50);
            padding: 10px 12px;
            border: none
        }
        .table tr {
            background: #fff;
            border: 1px solid var(--line);
            box-shadow: 0 1px 4px rgba(0,0,0,.04)
        }
        .table td { padding: 12px; border-top: 1px solid var(--line) }
        .table tr:hover {
            box-shadow: 0 6px 16px rgba(0,0,0,.07);
            transform: translateY(-1px)
        }

        /* Buttons */
        .btn {
            border: none;
            border-radius: 10px;
            padding: 8px 14px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            font-weight: 600
        }
        .btn.sm { padding: 7px 12px; font-size: .95rem }
        .btn.primary {
            background: var(--brand);
            color: #fff;
            box-shadow: 0 2px 0 rgba(0,0,0,.08)
        }
        .btn.primary:hover { background: var(--brand-600) }
        .btn.ghost {
            background: #f3f4f6;
            color: #111827;
            border: 1px solid var(--line)
        }
        .btn.ghost:hover { background: #e5e7eb }
        .btn:focus { outline: 3px solid var(--brand-50); outline-offset: 2px }

        .muted { color: var(--sub) }
        .empty {
            padding: 20px;
            border: 1px dashed var(--line);
            border-radius: 12px;
            text-align: center;
            background: #fff
        }
        .when { color: #374151; font-variant-numeric: tabular-nums }
        .role-tag {
            padding: 6px 12px;
            border-radius: 999px;
            background: #fdece5;
            color: #7a2a10;
            font-weight: 700;
            font-size: .85rem;
            border: 1px solid #f7c8b6
        }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="family-layout">
        <!-- Sidebar -->
        <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="invites" />

        <!-- Page content -->
        <main class="family-content">
            <div class="wrap">
                <div class="card">
                    <h2>Pending Group Invites</h2>
                    <div class="subtle-bar"></div>

                    <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
                        <asp:Label ID="lblMsg" runat="server" />
                    </asp:Panel>

                    <asp:GridView ID="gvInvites" runat="server" AutoGenerateColumns="false" CssClass="table"
                        GridLines="None" ShowHeaderWhenEmpty="true"
                        OnRowCommand="gvInvites_RowCommand" OnRowDataBound="gvInvites_RowDataBound"
                        EmptyDataText="No pending invites.">
                        <Columns>
                            <asp:TemplateField HeaderText="Group">
                                <ItemTemplate>
                                    <strong><%# Eval("GroupName") %></strong><br />
                                    <span class="muted">Invitation to join this family group</span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Invited As">
                                <ItemTemplate>
                                    <span class="role-tag"><%# Eval("Role") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Invited On">
                                <ItemTemplate>
                                    <span class="when"><%# String.Format("{0:yyyy-MM-dd HH:mm}", Eval("InvitedAt")) %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkAccept" runat="server" CommandName="Accept" CommandArgument='<%# Eval("GroupId") %>'
                                        CssClass="btn sm primary" Text="Accept" />
                                    &nbsp;
                                    <asp:LinkButton ID="lnkDecline" runat="server" CommandName="Decline" CommandArgument='<%# Eval("GroupId") %>'
                                        CssClass="btn sm ghost" Text="Decline" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <EmptyDataTemplate>
                            <div class="empty">No pending invites.</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </main>
    </div>
</asp:Content>
