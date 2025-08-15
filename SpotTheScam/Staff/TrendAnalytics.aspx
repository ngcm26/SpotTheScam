<%@ Page Title="Trend Radar" Language="C#" MasterPageFile="Staff.Master" AutoEventWireup="true" CodeBehind="TrendAnalytics.aspx.cs" Inherits="SpotTheScam.Staff.TrendAnalytics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Trend Radar</title>
    <style>
        .filter-bar { display:flex; gap:10px; align-items:center; margin-bottom:14px; }
        .filter-btn { background:#D36F2D; color:#fff; padding:8px 14px; border-radius:8px; text-decoration:none; border:none; cursor:pointer; }
        .cards { display:grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap:14px; }
        .card { border:1px solid #eee; border-radius:10px; padding:14px; background:#fff; }
        .card h3 { margin:0 0 10px 0; color:#051D40; font-size:1.1rem; }
        .empty { color:#777; font-style:italic; }
        .pill { display:inline-block; padding:2px 10px; border-radius:999px; background:#F4F6F8; color:#051D40; font-weight:600; }
        .table { width:100%; }
        .table th, .table td { padding:8px; border-bottom:1px solid #f0f0f0; }
        .table th { color:#6b7280; font-weight:600; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-bottom: 10px; display:flex; justify-content:space-between; align-items:center;">
        <div>
            <h1 style="color: #C46A1D; font-weight: 600; margin-bottom: 0.2em; font-size: 2.1rem;">Trend Radar</h1>
            <div style="color: #344054; font-size: 1.05rem;">Weekly insights from user scans. Use this to plan new modules and quizzes.</div>
        </div>
    </div>

    <div class="filter-bar">
        <label for="ddlRange">Range</label>
        <asp:DropDownList ID="ddlRange" runat="server">
            <asp:ListItem Text="Last 7 days" Value="7" Selected="True" />
            <asp:ListItem Text="Last 30 days" Value="30" />
        </asp:DropDownList>
        <asp:Button ID="btnApply" runat="server" Text="Apply" CssClass="filter-btn" OnClick="btnApply_Click" />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    </div>

    <div class="cards">
        <div class="card">
            <h3>Top Scam Types</h3>
            <asp:GridView ID="gvTopTypes" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:BoundField DataField="scam_type" HeaderText="Scam Type" />
                    <asp:BoundField DataField="cnt" HeaderText="Count" />
                </Columns>
            </asp:GridView>
        </div>

        <div class="card">
            <h3>Top Channels</h3>
            <asp:GridView ID="gvTopChannels" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:BoundField DataField="channel" HeaderText="Channel" />
                    <asp:BoundField DataField="cnt" HeaderText="Count" />
                </Columns>
            </asp:GridView>
        </div>

        <div class="card">
            <h3>Opportunities (No Module Mapping Yet)</h3>
            <asp:GridView ID="gvGaps" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:BoundField DataField="scam_type" HeaderText="Scam Type" />
                    <asp:BoundField DataField="cnt" HeaderText="Count" />
                </Columns>
            </asp:GridView>
            <asp:Label ID="lblGapsHint" runat="server" CssClass="empty" />
        </div>
    </div>
</asp:Content>


