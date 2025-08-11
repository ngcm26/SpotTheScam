<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="UserTransactionLogs.aspx.cs" Inherits="SpotTheScam.User.UserTransactionLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Transaction Logs</title>
    <link href="https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.6.0/js/bootstrap.bundle.min.js"></script>

    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #F4F6F8;
        }

        .container {
            max-width: 1200px;
        }

        .card {
            background: #fff;
            border-radius: 0.75rem;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            overflow-x: auto;
        }
        /* Alerts */
        .alert {
            padding: 1rem;
            border-radius: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.75rem;
        }

        .alert-success {
            background-color: #d1fae5;
            color: #065f46;
        }

        .alert-error {
            background-color: #fee2e2;
            color: #991b1b;
        }

        .alert-icon {
            font-size: 1.25rem;
        }

        /* Form Inputs */
        .form-input {
            padding: 0.75rem;
            border: 1px solid #d1d5db;
            border-radius: 0.5rem;
            width: 100%;
        }

        /* --- START: TABLE ALIGNMENT FIX --- */

        /* Use the table's ID for a high-specificity selector */
        #ContentPlaceHolder1_gvTransactions {
            /* This overrides the inline style and is the MOST important change */
            table-layout: auto !important;
            /* Ensure border consistency */
            border-collapse: collapse;
        }

            /* Standardize padding and box model for ALL cells */
            #ContentPlaceHolder1_gvTransactions th,
            #ContentPlaceHolder1_gvTransactions td {
                white-space: nowrap; /* Prevents text from wrapping */
                box-sizing: border-box; /* Ensures padding is included in the width */
            }

                /* Give the last column extra padding so it doesn't get cut off */
                #ContentPlaceHolder1_gvTransactions th:last-child,
                #ContentPlaceHolder1_gvTransactions td:last-child {
                    padding-right: 2rem !important;
                }

/* --- END: TABLE ALIGNMENT FIX --- */
        /* GridView Headers & Rows */
        .gv-header th, .gv-row td {
            white-space: nowrap !important;
            padding: 0.75rem 1.15rem;
            vertical-align: middle;
            font-size: 0.98rem;
            border-bottom: 1px solid #e5e7eb;
        }

        .gv-header th {
            background-color: #FBECC3; /* Light brand orange */
            color: #D36F2D; /* Primary orange text */
            font-weight: 700;
            border-bottom: 2px solid #FBECC3;
        }

            .gv-row td:last-child, .gv-header th:last-child {
                border-right: none;
            }
        .gv-row:nth-child(even) {
            background-color: #F4F6F8; /* Soft off-white, matches landing page */
        }

        .gv-row:nth-child(odd) {
            background-color: #fff;
        }
        /* Spacer row for visible gaps between rows */
        .gv-row-spacer td {
            height: 0.65rem !important;
            padding: 0 !important;
            background: transparent !important;
            border: none !important;
        }

        /* Table min width for horizontal scroll */
        /* Find and replace this entire class definition */
        .min-w-full {
            min-width: 1100px;
            width: 100%;
            table-layout: auto; /* This fixes the clipped text by auto-sizing columns */
            border-collapse: collapse; /* Ensures borders and spacing are consistent */
        }

        /* Description Column Truncation */
        .truncate-description {
            max-width: 210px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        /* Add this new rule anywhere inside the <style> block */
        .gv-header th:last-child,
        .gv-row td:last-child {
            padding-right: 2rem; /* Adds more space to the very end of the row */
        }

        /* Actions */
        .gv-actions a {
            color: #4f46e5;
            text-decoration: none;
            margin-right: 0.5rem;
        }

            .gv-actions a:hover {
                text-decoration: underline;
            }

        input[type="submit"].btn-primary,
        input[type="submit"].btn-secondary {
            width: auto !important;
            display: inline-block !important;
            min-width: unset !important;
            max-width: 180px; /* Optional: just in case */
            padding: 0.3rem 0.95rem;
            font-size: 0.96rem;
            border-radius: 0.4rem;
            box-sizing: border-box;
        }


        .btn-primary {
            background: #D36F2D;
            color: #fff;
            border: none;
        }

            .btn-primary:hover {
                background: #b95916;
            }

        .btn-secondary {
            background: #051D40;
            color: #fff;
            border: none;
        }

            .btn-secondary:hover {
                background: #16335B;
            }

        /* Row hover for clickable transactions */
        .transaction-row:hover td {
            background: #eef2ff !important;
            cursor: pointer;
        }

        /* Account Carousel Styling */
        .account-card-container {
            display: flex;
            overflow-x: hidden;
            scroll-behavior: smooth;
            -webkit-overflow-scrolling: touch;
        }

        .account-card {
            flex: 0 0 100%;
            padding: 1.5rem;
            border-radius: 0.75rem;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            margin-right: 1rem;
        }

            .account-card:last-child {
                margin-right: 0;
            }

            .account-card h3 {
                font-size: 1.5rem;
                font-weight: bold;
                margin-bottom: 0.5rem;
            }

            .account-card p {
                font-size: 1rem;
                margin-bottom: 0.25rem;
            }

            .account-card .balance {
                font-size: 2rem;
                font-weight: bold;
                margin-top: 1rem;
            }

        /* Carousel Dots */
        .pagination-dots {
            display: flex;
            justify-content: center;
            margin-top: 1rem;
        }

        .pagination-dot {
            height: 10px;
            width: 10px;
            background-color: #d1d5db;
            border-radius: 50%;
            display: inline-block;
            margin: 0 5px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

            .pagination-dot.active {
                background-color: #4f46e5;
            }

        @media (max-width: 900px) {
            .hide-on-mobile {
                display: none !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mx-auto p-4">
        <h1 class="text-3xl font-bold text-gray-800 mb-6">Your Transaction Logs</h1>

        <a href="SuspiciousActivities.aspx" class="btn btn-secondary mb-6 mr-3 inline-block">
            <i class="fas fa-exclamation-triangle mr-2"></i>View Suspicious Activities
        </a>

        <asp:Panel ID="AlertPanel" runat="server" Visible="false" CssClass="alert mb-6">
            <asp:Label ID="AlertMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Account Overview Section -->
        <div class="card p-6 mb-6">
            <div class="flex justify-between items-center mb-4">
                <h2 class="text-2xl font-semibold text-gray-700">Account Overview</h2>
                <asp:LinkButton ID="lnkViewAllAccounts" runat="server" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="text-indigo-600 hover:underline">View All Accounts</asp:LinkButton>
            </div>
            <asp:Panel ID="pnlAccountOverview" runat="server" Visible="false">
                <div class="mb-4">
                    <p class="text-gray-600 text-lg">Total Balance Across All Accounts:</p>
                    <p class="text-4xl font-bold text-indigo-700">
                        <asp:Literal ID="ltTotalBalance" runat="server"></asp:Literal>
                    </p>
                </div>
                <div class="relative flex items-center">
                    <asp:LinkButton ID="btnPrevAccount" runat="server" OnClick="btnPrevAccount_Click" CssClass="absolute left-0 z-10 p-2 rounded-full bg-gray-200 hover:bg-gray-300 focus:outline-none -ml-4">
                        <i class="fas fa-chevron-left text-gray-700"></i>
                    </asp:LinkButton>
                    <asp:Panel ID="pnlAccountCards" runat="server" CssClass="account-card-container flex-grow">
                        <asp:Repeater ID="rptAccounts" runat="server">
                            <ItemTemplate>
                                <div class="account-card flex-shrink-0 w-full">
                                    <h3 class="font-semibold text-white"><%# Eval("AccountNickname") %></h3>
                                    <p class="text-sm text-gray-200"><%# Eval("BankName") %> - <%# Eval("AccountType") %></p>
                                    <p class="text-sm text-gray-200">Account No: <%# Eval("AccountNumber") %></p>
                                    <p class="balance text-white"><%# Eval("Balance", "{0:C2}") %></p>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <asp:LinkButton ID="btnNextAccount" runat="server" OnClick="btnNextAccount_Click" CssClass="absolute right-0 z-10 p-2 rounded-full bg-gray-200 hover:bg-gray-300 focus:outline-none -mr-4">
                        <i class="fas fa-chevron-right text-gray-700"></i>
                    </asp:LinkButton>
                </div>
                <div id="accountDots" runat="server" class="pagination-dots"></div>
            </asp:Panel>
            <asp:Panel ID="pnlNoAccounts" runat="server" Visible="false" CssClass="text-center py-8">
                <p class="text-gray-500 text-lg mb-4">You haven't linked any bank accounts yet.</p>
                <asp:Button ID="btnAddFirstAccount" runat="server" Text="Add Your First Account" PostBackUrl="~/User/UserBankAccounts.aspx" CssClass="btn-primary" />
            </asp:Panel>
        </div>

        <!-- Transaction Filters -->
        <div class="card p-6 mb-6">
            <h2 class="text-2xl font-semibold text-gray-700 mb-4">Filter Transactions</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-4">
                <div>
                    <label for="<%=txtStartDate.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Start Date:</label>
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-input"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=txtEndDate.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">End Date:</label>
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-input"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=ddlTransactionType.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Transaction Type:</label>
                    <asp:DropDownList ID="ddlTransactionType" runat="server" CssClass="form-input">
                        <asp:ListItem Text="All Types" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Credit" Value="Credit"></asp:ListItem>
                        <asp:ListItem Text="Debit" Value="Debit"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="lg:col-span-2">
                    <label for="<%=txtSearch.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Search Description/Recipient:</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-input" placeholder="e.g., Groceries, John Doe"></asp:TextBox>
                </div>
                <div>
                    <label for="<%=ddlAccount.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Select Account:</label>
                    <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-input"></asp:DropDownList>
                </div>
                <div>
                    <label for="<%=txtMinAmount.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Min Amount:</label>
                    <asp:TextBox ID="txtMinAmount" runat="server" CssClass="form-input" TextMode="Number"></asp:TextBox>
                    <asp:CompareValidator ID="cvMinAmount" runat="server" ControlToValidate="txtMinAmount" Operator="DataTypeCheck" Type="Currency" ErrorMessage="*Must be a valid number" Display="Dynamic" CssClass="text-red-500 text-xs mt-1"></asp:CompareValidator>
                </div>
                <div>
                    <label for="<%=txtMaxAmount.ClientID%>" class="block text-gray-700 text-sm font-bold mb-2">Max Amount:</label>
                    <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="form-input" TextMode="Number"></asp:TextBox>
                    <asp:CompareValidator ID="cvMaxAmount" runat="server" ControlToValidate="txtMaxAmount" Operator="DataTypeCheck" Type="Currency" ErrorMessage="*Must be a valid number" Display="Dynamic" CssClass="text-red-500 text-xs mt-1"></asp:CompareValidator>
                    <asp:CompareValidator ID="cvAmountRange" runat="server" ControlToValidate="txtMaxAmount" ControlToCompare="txtMinAmount" Operator="GreaterThanEqual" Type="Currency" ErrorMessage="*Max amount must be >= Min amount" Display="Dynamic" CssClass="text-red-500 text-xs mt-1"></asp:CompareValidator>
                </div>
            </div>
            <div class="flex flex-wrap gap-4 justify-end">
                <asp:Button ID="btnApplyFilters" runat="server" Text="Apply Filters" OnClick="btnApplyFilters_Click" CssClass="btn-primary" />
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" OnClick="btnClearFilters_Click" CssClass="btn-secondary" />
                <asp:Button ID="btnExportPdf" runat="server" Text="Export to PDF" OnClick="btnExportPdf_Click" CssClass="btn-secondary" />
                <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" OnClick="btnExportExcel_Click" CssClass="btn-secondary" />
            </div>
        </div>

        <h2 class="text-2xl font-semibold text-gray-700 mb-4">All Transactions</h2>
        <div class="card" style="overflow-x:auto; max-width:100vw; padding:0;">
            <div style="padding: 1.5rem;">

                <asp:GridView ID="gvTransactions" runat="server"
                    CssClass="my-grid min-w-full"
                    Style="width: 100%; table-layout: fixed;"
                    AutoGenerateColumns="False"
                    AllowPaging="True"
                    PageSize="10"
                    DataKeyNames="TransactionId"
                    OnPageIndexChanging="gvTransactions_PageIndexChanging"
                    OnRowDataBound="gvTransactions_RowDataBound"
                    HeaderStyle-CssClass="gv-header"
                    RowStyle-CssClass="gv-row">

                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkHeader" runat="server" AutoPostBack="false" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkRow" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <%# 
                (Eval("IsFlagged") != DBNull.Value && Convert.ToBoolean(Eval("IsFlagged")))
                    ? $"<span class='inline-block px-2 py-1 text-xs font-semibold rounded {(Eval("Severity").ToString() == "red" ? "bg-red-100 text-red-700" : "bg-yellow-100 text-yellow-800")}'>{(Eval("Severity").ToString() == "red" ? "High Risk" : "Medium Risk")}</span>"
                    : "<span class='inline-block px-2 py-1 text-xs font-semibold rounded bg-gray-100 text-gray-500'>Normal</span>"
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="TransactionTime" HeaderText="Time" />

                    <asp:BoundField DataField="Description" HeaderText="Description">
                        <ItemStyle CssClass="truncate-description" />
                    </asp:BoundField>

                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <%# 
                Eval("TransactionType").ToString() == "Credit"
                    ? "<span class='inline-block bg-green-100 text-green-700 text-xs font-semibold px-2 py-1 rounded'>Credit</span>"
                    : "<span class='inline-block bg-red-100 text-red-700 text-xs font-semibold px-2 py-1 rounded'>Debit</span>" 
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <%# 
                Eval("TransactionType").ToString() == "Credit"
                    ? $"<span class='text-green-700 font-semibold'>+{Eval("Amount", "{0:C2}")}</span>"
                    : $"<span class='text-red-700 font-semibold'>-{Eval("Amount", "{0:C2}")}</span>"
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="SenderRecipient" HeaderText="Sender/Recipient" />
                </Columns>

                <EmptyDataTemplate>
                    <p class="text-gray-500 text-center py-4">No transactions found for the selected filters.</p>
                </EmptyDataTemplate>
                <PagerStyle CssClass="pagination-style" />
            </asp:GridView>
            <div class="flex justify-center gap-6 mt-5">
                <asp:Button ID="btnBulkFlag" runat="server" Text="Flag Selected" CssClass="btn-primary px-4 py-2 text-sm" OnClick="btnBulkFlag_Click" />
                <asp:Button ID="btnBulkUnflag" runat="server" Text="Unflag Selected" CssClass="btn-secondary px-4 py-2 text-sm" OnClick="btnBulkUnflag_Click" />
            </div>



        </div>

        <div class="modal fade" id="transactionModal" tabindex="-1" role="dialog" aria-labelledby="transactionModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="transactionModalLabel">Transaction Details</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;</button>
                    </div>
                    <div class="modal-body" id="modalTransactionDetails">
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Scripts -->
    <script type="text/javascript">
        // Show Transaction Modal on Row Click
        function showTransactionModal(transactionId) {
            $.ajax({
                type: "POST",
                url: "UserTransactionLogs.aspx/GetTransactionDetails",
                data: JSON.stringify({ id: transactionId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $('#modalTransactionDetails').html(response.d);
                    $('#transactionModal').modal('show');
                }
            });
        }

        // Make table row clickable for details
        $(document).on('click', '.transaction-row', function (e) {
            if (
                $(e.target).is('input[type="checkbox"]') ||
                $(e.target).is('button') ||
                $(e.target).closest('button').length > 0 ||
                $(e.target).is('a')
            ) return;
            var txnId = $(this).attr('data-id');
            if (txnId) {
                showTransactionModal(txnId);
            }
        });

        // Select All Rows
        $(function () {
            var headerCheck = $('#<%= gvTransactions.ClientID %> input[id*="chkHeader"]');
            headerCheck.on('change', function () {
                var isChecked = $(this).is(':checked');
                $('#<%= gvTransactions.ClientID %> input[id*="chkRow"]').prop('checked', isChecked);
            });
        });
    </script>
</asp:Content>
