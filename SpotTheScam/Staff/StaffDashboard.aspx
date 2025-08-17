<%@ Page Title="Staff Dashboard" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="StaffDashboard.aspx.cs" Inherits="SpotTheScam.Staff.StaffDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-btn { background:#D36F2D; color:#fff; padding:8px 14px; border-radius:8px; text-decoration:none; border:none; cursor:pointer; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Shown if logged in -->
    <asp:PlaceHolder ID="phDashboard" runat="server" Visible="false">
        <div style="margin-bottom: 16px; display:flex; justify-content:space-between; align-items:center;">
            <div>
                <h1 class="mb-1" style="color:#C46A1D; font-weight:600;">Welcome, <asp:Label ID="lblStaffName" runat="server" /></h1>
                <div style="color:#344054;">Weekly insights from user scans. Use this to plan new modules and quizzes.</div>
            </div>
        </div>

        <div class="filter-bar" style="display:flex; gap:10px; align-items:center; margin-bottom:14px;">
            <label for="ddlRange">Range</label>
            <asp:DropDownList ID="ddlRange" runat="server">
                <asp:ListItem Text="Last 7 days" Value="7" Selected="True" />
                <asp:ListItem Text="Last 30 days" Value="30" />
            </asp:DropDownList>
            <asp:Button ID="btnApply" runat="server" Text="Apply" CssClass="filter-btn" OnClick="btnApply_Click" />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
        </div>

        <!-- KPI row -->
        <div class="kpi-row" style="display:grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap:14px; margin-bottom:14px;">
            <div style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <div style="color:#667085; font-size:0.9rem;">Blogs</div>
                <div style="color:#051D40; font-weight:700; font-size:1.4rem; margin-top:4px;">
                    <asp:Label ID="lblBlogs" runat="server" />
                </div>
            </div>
            <div style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <div style="color:#667085; font-size:0.9rem;">Scans (range)</div>
                <div style="color:#051D40; font-weight:700; font-size:1.4rem; margin-top:4px;">
                    <asp:Label ID="lblScans" runat="server" />
                </div>
            </div>
            <div style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <div style="color:#667085; font-size:0.9rem;">Quiz Engagement (range)</div>
                <div style="color:#051D40; font-weight:700; font-size:1.4rem; margin-top:4px;">
                    <asp:Label ID="lblQuiz" runat="server" />
                </div>
            </div>
            <div style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <div style="color:#667085; font-size:0.9rem;">Upcoming Sessions</div>
                <div style="color:#051D40; font-weight:700; font-size:1.4rem; margin-top:4px;">
                    <asp:Label ID="lblUpcomingSessions" runat="server" />
                </div>
            </div>
        </div>

        <div class="cards" style="display:grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap:14px;">
            <div class="card" style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <h3 style="margin:0 0 10px 0; color:#051D40; font-size:1.1rem;">Top Scam Types</h3>
                <asp:GridView ID="gvTopTypes" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="scam_type" HeaderText="Scam Type" />
                        <asp:BoundField DataField="cnt" HeaderText="Count" />
                    </Columns>
                </asp:GridView>
            </div>

            <div class="card" style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <h3 style="margin:0 0 10px 0; color:#051D40; font-size:1.1rem;">Top Channels</h3>
                <asp:GridView ID="gvTopChannels" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="channel" HeaderText="Channel" />
                        <asp:BoundField DataField="cnt" HeaderText="Count" />
                    </Columns>
                </asp:GridView>
            </div>

            <div class="card" style="border:1px solid #eee; border-radius:10px; padding:14px; background:#fff;">
                <h3 style="margin:0 0 10px 0; color:#051D40; font-size:1.1rem;">Opportunities (No Module Mapping Yet)</h3>
                <asp:GridView ID="gvGaps" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="scam_type" HeaderText="Scam Type" />
                        <asp:BoundField DataField="cnt" HeaderText="Count" />
                    </Columns>
                </asp:GridView>
                <asp:Label ID="lblGapsHint" runat="server" CssClass="empty" />
            </div>
        </div>
    </asp:PlaceHolder>

    <!-- Shown if NOT logged in -->
    <asp:PlaceHolder ID="phPleaseLogin" runat="server" Visible="false">
        <div class="alert alert-warning" role="alert">
            <h4 class="alert-heading">Access Denied</h4>
            <p>Please log in to view this page.</p>
            <hr>
            <a href="../User/UserLogin.aspx" class="btn btn-primary">Go to Login</a>
        </div>
    </asp:PlaceHolder>
</asp:Content>
