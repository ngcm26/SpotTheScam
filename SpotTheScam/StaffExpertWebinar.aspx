<%@ Page Title="Expert Webinar Sessions" Language="C#" MasterPageFile="~/Staff.Master" AutoEventWireup="true" CodeBehind="StaffExpertWebinar.aspx.cs" Inherits="SpotTheScam.StaffExpertWebinar" UnobtrusiveValidationMode="None" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .container {
            max-width: 1200px;
            margin: 30px auto;
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .form-section {
            background-color: #f9f9f9;
            padding: 25px;
            border-radius: 8px;
            margin-bottom: 30px;
        }
        
        .form-group {
            margin-bottom: 15px;
        }
        
        label {
            display: inline-block;
            width: 120px;
            font-weight: bold;
            color: #333;
        }
        
        input[type="date"], input[type="time"], .btn {
            padding: 8px 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 14px;
        }
        
        .btn-add {
            background-color: #e67e22;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            margin-left: 10px;
        }
        
        .btn-add:hover {
            background-color: #d35400;
        }
        
        .btn-delete {
            background-color: #e74c3c;
            color: white;
            border: none;
            padding: 5px 15px;
            border-radius: 3px;
            cursor: pointer;
            font-size: 12px;
        }
        
        .btn-delete:hover {
            background-color: #c0392b;
        }
        
        .sessions-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        
        .sessions-table th, .sessions-table td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: left;
        }
        
        .sessions-table th {
            background-color: #f8f8f8;
            font-weight: bold;
        }
        
        .sessions-table tr:nth-child(even) {
            background-color: #f9f9f9;
        }
        
        .status-available {
            color: #28a745;
            font-weight: bold;
        }
        
        .status-booked {
            color: #007bff;
            font-weight: bold;
        }
        
        .alert {
            padding: 15px;
            margin-bottom: 20px;
            border: 1px solid transparent;
            border-radius: 4px;
        }
        
        .alert-success {
            color: #155724;
            background-color: #d4edda;
            border-color: #c3e6cb;
        }
        
        .alert-error {
            color: #721c24;
            background-color: #f8d7da;
            border-color: #f5c6cb;
        }
        
        h2 {
            color: #e67e22;
            border-bottom: 2px solid #e67e22;
            padding-bottom: 10px;
        }
        
        h3 {
            color: #e67e22;
            margin-top: 0;
        }
        
        .hidden {
            display: none;
        }
        
        body {
            background-color: #f4f1e8;
        }
    </style>

    <div class="container">
        <h2>Manage Expert Webinar Sessions</h2>
        
        <!-- Add Time Slot Form -->
        <div class="form-section">
            <h3>Add Available Time Slots</h3>
            
            <!-- Alert Panel -->
            <asp:Panel ID="AlertPanel" runat="server" CssClass="hidden">
                <asp:Label ID="AlertMessage" runat="server"></asp:Label>
            </asp:Panel>
            
            <div class="form-group">
                <label for="SessionDate">Date:</label>
                <asp:TextBox ID="SessionDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="DateValidator" runat="server" 
                                            ControlToValidate="SessionDate" 
                                            ErrorMessage="Date is required" 
                                            ForeColor="Red" 
                                            ValidationGroup="AddSession">
                </asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                <label for="StartTime">Start Time:</label>
                <asp:TextBox ID="StartTime" runat="server" TextMode="Time" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="StartTimeValidator" runat="server" 
                                            ControlToValidate="StartTime" 
                                            ErrorMessage="Start time is required" 
                                            ForeColor="Red" 
                                            ValidationGroup="AddSession">
                </asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                <label for="EndTime">End Time:</label>
                <asp:TextBox ID="EndTime" runat="server" TextMode="Time" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="EndTimeValidator" runat="server" 
                                            ControlToValidate="EndTime" 
                                            ErrorMessage="End time is required" 
                                            ForeColor="Red" 
                                            ValidationGroup="AddSession">
                </asp:RequiredFieldValidator>
            </div>
            
            <asp:Button ID="AddTimeSlotButton" runat="server" 
                       Text="Add Time Slot" 
                       CssClass="btn-add" 
                       OnClick="AddTimeSlotButton_Click" 
                       ValidationGroup="AddSession" />
        </div>
        
        <!-- Sessions Table -->
        <div>
            <h3>Available Time Slots</h3>
            <asp:GridView ID="SessionsGridView" runat="server" 
                         CssClass="sessions-table" 
                         AutoGenerateColumns="False" 
                         OnRowCommand="SessionsGridView_RowCommand"
                         DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="SessionDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:hh\:mm}" />
                    <asp:BoundField DataField="EndTime" HeaderText="End Time" DataFormatString="{0:hh\:mm}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# Eval("Status").ToString() == "Available" ? "status-available" : "status-booked" %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" NullDisplayText="-" />
                    <asp:BoundField DataField="CustomerPhone" HeaderText="Phone Number" NullDisplayText="-" />
                    <asp:BoundField DataField="ScamConcerns" HeaderText="Scam Concerns" NullDisplayText="-" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="DeleteButton" runat="server" 
                                       Text="Delete" 
                                       CssClass="btn-delete" 
                                       CommandName="DeleteSession" 
                                       CommandArgument='<%# Eval("Id") %>'
                                       OnClientClick="return confirm('Are you sure you want to delete this time slot?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 20px; color: #666;">
                        No sessions available. Add some time slots above.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>