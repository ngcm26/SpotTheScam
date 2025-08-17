<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilySideNav.ascx.cs"
    Inherits="SpotTheScam.User.Controls.FamilySideNav" %>

<!-- Mobile trigger (shown <1024px) -->
<button type="button" class="family-trigger" aria-controls="family-drawer" aria-expanded="false">
    <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
        <path d="M3 6h18M3 12h18M3 18h18" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" />
    </svg>
    Family
</button>

<!-- Overlay + Drawer (mobile) / Sidebar (desktop) -->
<div id="family-overlay" class="family-overlay" hidden></div>

<nav id="family-drawer" class="family-sidenav" aria-label="Family navigation">
    <div class="family-sidenav__header">
        <span class="title">Family Hub</span>
        <button type="button" class="close" aria-label="Close">
            <svg viewBox="0 0 24 24" width="22" height="22" aria-hidden="true">
                <path d="M6 6l12 12M18 6l-12 12" stroke="currentColor" stroke-width="2" fill="none" stroke-linecap="round" />
            </svg>
        </button>
    </div>
    <!-- Group selector -->
    <div class="family-groups">
        <div class="subtle" style="margin: 6px 6px 4px;">Your groups</div>

        <asp:Repeater ID="rptGroups" runat="server">
            <ItemTemplate>
                <asp:LinkButton ID="lnkSelectGroup" runat="server"
                    CommandArgument='<%# Eval("GroupId") %>'
                    OnClick="lnkSelectGroup_Click"
                    CssClass='<%# (Convert.ToBoolean(Eval("IsActive")) ? "group-link active" : "group-link") %>'>
        <%# Eval("GroupName") %>
                </asp:LinkButton>
            </ItemTemplate>
            <FooterTemplate>
                <asp:PlaceHolder runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count==0 %>'>
                    <a class="group-link" href="/User/CreateGroup.aspx">No groups yet — create one</a>
                </asp:PlaceHolder>
            </FooterTemplate>
        </asp:Repeater>
    </div>


    <ul class="family-links">
        <li><a class='<%# ActiveClass("overview") %>' href="/User/FamilyOverview.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M3 13h6V3H3v10Zm12 8h6V3h-6v18ZM3 21h6v-6H3v6Zm12-8h6V9h-6v4Z" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            Family Overview</a></li>
        <li><a class='<%# ActiveClass("groups") %>' href="/User/MyGroups.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M12 12a4 4 0 1 0-4-4 4 4 0 0 0 4 4Zm7 8v-1a5 5 0 0 0-5-5H10a5 5 0 0 0-5 5v1" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            My Groups</a></li>

        <li><a class='<%# ActiveClass("create") %>' href="/User/CreateGroup.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M12 5v14M5 12h14" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            Create Group</a></li>

        <li><a class='<%# ActiveClass("invites") %>' href="/User/Invites.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M4 4h16v12H4zM22 20l-6-6" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            Invites</a></li>

        <li><a class='<%# ActiveClass("connect") %>' href="/User/ConnectBank.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M3 10h18M5 20h14a2 2 0 0 0 2-2V6H3v12a2 2 0 0 0 2 2Z" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            Connect Bank</a></li>

        <li><a class='<%# ActiveClass("add") %>' href="/User/AddTransactions.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M12 8v8M8 12h8M4 4h16v16H4z" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            Add Transaction</a></li>

        <li><a class='<%# ActiveClass("logs") %>' href="/User/UserTransactionLogs.aspx">
            <span class="ico">
                <svg viewBox="0 0 24 24">
                    <path d="M4 6h16M4 12h16M4 18h10" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                </svg></span>
            Transaction Logs</a></li>
    </ul>
</nav>

<style>
    :root {
        --brand: #D36F2D;
        --brand-600: #c5521f;
        --brand-50: #FFF2EC;
        --ink: #1f2937;
        --sub: #6b7280;
        --card: #fff;
        --line: #e5e7eb;
    }
    /* Layout helpers (page wrapper uses .family-layout / .family-content) */
    .family-layout {
        display: grid;
        grid-template-columns: 240px 1fr;
        gap: 16px
    }

    @media (max-width:1023.98px) {
        .family-layout {
            grid-template-columns: 1fr
        }
    }

    /* Trigger (mobile only) */
    .family-trigger {
        display: none;
        align-items: center;
        gap: 8px;
        border: 1px solid var(--line);
        background: #fff;
        border-radius: 10px;
        padding: 8px 12px;
        font-weight: 600;
        cursor: pointer
    }

        .family-trigger:hover {
            box-shadow: 0 0 0 3px var(--brand-50);
            border-color: var(--brand)
        }

    @media (max-width:1023.98px) {
        .family-trigger {
            display: inline-flex;
            margin-bottom: 10px
        }
    }

    /* Overlay for drawer */
    .family-overlay {
        position: fixed;
        inset: 0;
        background: rgba(0,0,0,.35);
        z-index: 99
    }

    /* Sidebar / Drawer */
    .family-sidenav {
        position: sticky;
        top: 16px;
        align-self: start;
        z-index: 100;
        width: 240px;
        background: var(--card);
        border: 1px solid var(--line);
        border-radius: 14px;
        box-shadow: 0 6px 20px rgba(0,0,0,.06);
        padding: 12px;
    }

    .family-sidenav__header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 8px
    }

    .family-sidenav .title {
        font-weight: 700;
        color: var(--ink)
    }

    .family-sidenav .close {
        display: none;
        border: 0;
        background: transparent;
        cursor: pointer
    }

    @media (max-width:1023.98px) {
        .family-sidenav {
            position: fixed;
            left: -280px;
            top: 0;
            height: 100vh;
            width: 260px;
            padding: 16px;
            border-radius: 0 14px 14px 0;
            transition: left .25s ease;
            z-index: 100;
            overflow: auto
        }

            .family-sidenav.open {
                left: 0
            }

            .family-sidenav .close {
                display: inline-flex
            }
    }

    /* Links */
    .family-links {
        list-style: none;
        margin: 0;
        padding: 6px
    }

        .family-links li {
            margin-bottom: 6px
        }

        .family-links a {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 10px 12px;
            border: 1px solid var(--line);
            border-radius: 10px;
            text-decoration: none;
            color: #111827;
            font-weight: 600;
            background: #fff;
        }

            .family-links a:hover {
                border-color: var(--brand);
                box-shadow: 0 0 0 3px var(--brand-50)
            }

            .family-links a.active {
                background: var(--brand);
                color: #fff;
                border-color: var(--brand)
            }

                .family-links a.active:hover {
                    background: var(--brand-600)
                }

        .family-links .ico {
            display: inline-flex;
            width: 18px;
            height: 18px;
            color: currentColor
        }

            .family-links .ico svg {
                width: 18px;
                height: 18px
            }


    .family-groups {
        padding: 6px
    }

    .group-link {
        display: block;
        padding: 8px 10px;
        border: 1px solid var(--line);
        border-radius: 8px;
        margin-bottom: 6px;
        text-decoration: none;
        background: #fff;
        color: #111827;
        font-weight: 600
    }

        .group-link:hover {
            border-color: var(--brand);
            box-shadow: 0 0 0 3px var(--brand-50)
        }

        .group-link.active {
            background: var(--brand);
            color: #fff;
            border-color: var(--brand)
        }
</style>

<script>
    (function () {
        var trigger = document.currentScript.previousElementSibling.previousElementSibling; // .family-trigger
        var overlay = document.getElementById('family-overlay');
        var drawer = document.getElementById('family-drawer');
        var closeBtn = drawer.querySelector('.close');

        function open() {
            drawer.classList.add('open'); overlay.hidden = false;
            trigger.setAttribute('aria-expanded', 'true'); drawer.focus();
        }
        function close() {
            drawer.classList.remove('open'); overlay.hidden = true;
            trigger.setAttribute('aria-expanded', 'false');
        }

        if (trigger) trigger.addEventListener('click', open);
        if (closeBtn) closeBtn.addEventListener('click', close);
        if (overlay) overlay.addEventListener('click', close);
        document.addEventListener('keydown', function (e) { if (e.key === 'Escape') close(); });
    })();
</script>
