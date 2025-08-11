<%@ Page Title="Suspicious Activities" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="SuspiciousActivities.aspx.cs" Inherits="SpotTheScam.User.SuspiciousActivities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Suspicious Activities</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <style>
        .sus-card { background: #fff; border-radius: 0.75rem; box-shadow: 0 4px 12px rgba(0,0,0,0.09); }
        .severity-red { color: #b91c1c; background: #fee2e2; font-weight: 700; }
        .severity-yellow { color: #b45309; background: #fefcbf; font-weight: 700; }
        .status-badge { font-size: 0.9em; border-radius: 0.35em; padding: 0.25em 0.7em; }
        .sus-btn { font-size:0.95rem; padding:0.5em 1.2em; border-radius:0.45em; }
        .sus-btn-safe { background:#059669; color:#fff; border:none; }
        .sus-btn-safe:hover { background:#047857; }
        .sus-btn-alert { background:#eab308; color:#fff; border:none; }
        .sus-btn-alert:hover { background:#b45309; }
        .sus-row:hover td { background: #f1f5f9 !important; }
        .sus-table th, .sus-table td { padding: 0.85rem 1rem; white-space: nowrap; vertical-align: middle; }
        .sus-table th { font-size: 1rem; background: #e5e7eb; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto p-4">
        <h1 class="text-3xl font-bold text-red-700 mb-6">Suspicious Activities</h1>
        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-5">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>
        <div class="sus-card p-6 mb-8">
            <h2 class="text-xl font-semibold mb-5 text-gray-800">Review and Action Required</h2>
            <asp:GridView ID="gvSuspicious" runat="server"
                AutoGenerateColumns="False"
                AllowPaging="True"
                PageSize="10"
                DataKeyNames="TransactionId"
                CssClass="sus-table min-w-full"
                HeaderStyle-CssClass="gv-header"
                RowStyle-CssClass="sus-row">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkHeader" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkRow" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Flag">
                        <ItemTemplate>
                            <%# 
                                (Eval("Severity").ToString() == "red")
                                ? "<i class='fas fa-flag' style='color:#dc2626;font-size:1.2em;' title='High Risk'></i>"
                                : "<i class='fas fa-flag' style='color:#facc15;font-size:1.2em;' title='Medium Risk'></i>"
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Severity">
                        <ItemTemplate>
                            <%# 
                                (Eval("Severity").ToString() == "red")
                                ? "<span class='status-badge severity-red'>High Risk</span>"
                                : "<span class='status-badge severity-yellow'>Medium Risk</span>"
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="TransactionTime" HeaderText="Time" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:BoundField DataField="TransactionType" HeaderText="Type" />
                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <%# 
                                Eval("TransactionType").ToString() == "Credit"
                                    ? "<span style='color:#059669;font-weight:bold;'>+" + Eval("Amount", "{0:C2}") + "</span>"
                                    : "<span style='color:#b91c1c;font-weight:bold;'>-" + Eval("Amount", "{0:C2}") + "</span>"
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SenderRecipient" HeaderText="Sender/Recipient" />
                </Columns>
            </asp:GridView>
            <div class="flex flex-wrap gap-4 justify-end mt-4">
                <asp:Button ID="btnBulkSafe" runat="server" Text="Mark as Safe" CssClass="sus-btn sus-btn-safe text-sm" OnClick="btnBulkSafe_Click" />
                <asp:Button ID="btnBulkAlert" runat="server" Text="Alert Trusted Contact" CssClass="sus-btn sus-btn-alert text-sm" OnClick="btnBulkAlert_Click" />
            </div>
        </div>
    </div>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        // Select/Deselect all checkboxes
        $(function () {
            var headerCheck = $('#<%= gvSuspicious.ClientID %> input[id*="chkHeader"]');
            headerCheck.on('change', function () {
                var isChecked = $(this).is(':checked');
                $('#<%= gvSuspicious.ClientID %> input[id*="chkRow"]').prop('checked', isChecked);
            });
        });
    </script>
</asp:Content>
