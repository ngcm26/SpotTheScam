<%@ Page Title="Create Family Group" Language="C#" MasterPageFile="~/User/User.Master"
    AutoEventWireup="true" CodeBehind="CreateGroup.aspx.cs" Inherits="SpotTheScam.User.CreateGroup" %>
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

        /* layout wrapper for sidebar + content */
        .family-layout { display: grid; grid-template-columns: 240px 1fr; gap: 16px; }
        @media (max-width:1023.98px){ .family-layout { grid-template-columns: 1fr; } }
        .family-content { min-width: 0; }

        .card {
            max-width: 720px;
            margin: 28px auto;
            padding: 24px;
            background: var(--card);
            border-radius: 14px;
            border: 1px solid var(--line);
            box-shadow: 0 6px 20px rgba(0,0,0,.06);
        }

        h2 {
            margin: 0 0 4px 0;
            color: var(--ink);
            font-weight: 700;
            letter-spacing: .2px
        }

        .subtle-bar {
            height: 4px;
            background: linear-gradient(90deg,var(--brand),#f59e0b);
            border-radius: 999px;
            margin: 10px 0 18px
        }

        .form-row { margin-bottom: 16px }
        label {
            font-weight: 600;
            display: block;
            margin-bottom: 6px;
            color: var(--ink)
        }
        .help { font-size: .9rem; color: var(--sub); margin-top: 6px }

        input[type=text], textarea {
            width: 100%;
            padding: 12px 12px;
            border-radius: 10px;
            border: 1px solid var(--line);
            background: #fff;
            transition: border-color .15s, box-shadow .15s;
        }
        input[type=text]:focus, textarea:focus {
            outline: none;
            border-color: var(--brand);
            box-shadow: 0 0 0 3px var(--brand-50);
        }
        textarea { min-height: 110px; resize: vertical }

        /* Buttons */
        .btn {
            padding: 10px 16px;
            border: none;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            display: inline-block
        }
        .btn.primary {
            background: var(--brand);
            color: #fff;
            box-shadow: 0 2px 0 rgba(0,0,0,.08)
        }
        .btn.primary:hover { background: var(--brand-600) }
        .btn:disabled { opacity: .55; cursor: not-allowed }
        .btn:focus { outline: 3px solid var(--brand-50); outline-offset: 2px }

        /* Messages */
        .msg {
            padding: 12px 14px;
            border-radius: 10px;
            margin-bottom: 14px;
            font-weight: 600;
            border: 1px solid transparent
        }
        .msg.ok { background: var(--ok-bg); color: var(--ok); border-color: var(--ok-br) }
        .msg.err { background: var(--err-bg); color: var(--err); border-color: var(--err-br) }

        /* Validation */
        .val-err {
            display: block;
            margin-top: 6px;
            color: #b91c1c;
            font-weight: 600
        }
        .field-group { position: relative }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="family-layout">
        <!-- Sidebar -->
        <uc:FamilySideNav ID="SideNav" runat="server" ActiveKey="create" />

        <!-- Page content -->
        <main class="family-content">
            <div class="card">
                <h2>Create Family Group</h2>
                <div class="subtle-bar"></div>

                <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="msg">
                    <asp:Label ID="lblMsg" runat="server" />
                </asp:Panel>

                <div class="form-row">
                    <label for="<%= txtName.ClientID %>">Group Name</label>
                    <div class="field-group">
                        <asp:TextBox ID="txtName" runat="server" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server"
                            ControlToValidate="txtName" ErrorMessage="Group name is required"
                            Display="Dynamic" CssClass="val-err" />
                    </div>
                </div>

                <div class="form-row">
                    <label for="<%= txtDesc.ClientID %>">Description <span class="help">(optional, up to 250 chars)</span></label>
                    <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" MaxLength="250" />
                </div>

                <asp:Button ID="btnCreate" runat="server" Text="Create Group" CssClass="btn primary"
                    OnClick="btnCreate_Click" />
            </div>
        </main>
    </div>
</asp:Content>
