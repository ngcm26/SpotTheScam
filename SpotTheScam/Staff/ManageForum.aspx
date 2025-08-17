<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="ManageForum.aspx.cs" Inherits="SpotTheScam.Staff.ManageForum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .manage-forum-container {
            max-width: 1100px;
            margin: 30px auto;
            background: #ffffff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
        }

        .manage-forum-header {
            font-size: 1.6rem;
            font-weight: 600;
            margin-bottom: 20px;
        }

        .gridview-styled {
            border: none;
            width: 100%;
        }

            .gridview-styled th {
                background-color: #f8f9fa;
                color: #333;
                padding: 12px;
                border-bottom: 2px solid #dee2e6;
                text-align: left;
            }

            .gridview-styled td {
                padding: 10px;
                border-bottom: 1px solid #dee2e6;
                vertical-align: middle;
            }

            .gridview-styled tr:hover {
                background-color: #f1f1f1;
            }

        .btn-sm {
            padding: 5px 10px;
            font-size: 0.85rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="manage-forum-container">
        <div class="manage-forum-header">Manage Forum Discussions</div>

        <asp:GridView ID="gv_forum" runat="server" AutoGenerateColumns="False"
    CssClass="gridview-styled table table-hover"
    Width="100%"
    DataKeyNames="DiscussionId"
    OnRowDeleting="gv_forum_RowDeleting"
    OnSelectedIndexChanged="gv_forum_SelectedIndexChanged"
    OnRowEditing="gv_forum_RowEditing"
    OnRowCancelingEdit="gv_forum_RowCancelingEdit"
    OnRowUpdating="gv_forum_RowUpdating">

    <Columns>
        <asp:BoundField DataField="DiscussionId" HeaderText="Forum ID" ReadOnly="true" />
        <asp:BoundField DataField="Username" HeaderText="Username" ReadOnly="true" />
        <asp:BoundField DataField="Title" HeaderText="Discussion Title" />
        <asp:BoundField DataField="Description" HeaderText="Description" />
        <asp:BoundField DataField="CreatedAt" HeaderText="Created At" ReadOnly="true" />

        <asp:CommandField ShowSelectButton="True" SelectText="View" />
        <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
        <asp:CommandField ShowEditButton="True" EditText="Edit" UpdateText="Save" CancelText="Cancel" />
    </Columns>
</asp:GridView>
    </div>
</asp:Content>
