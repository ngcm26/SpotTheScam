<%@ Page Title="" Language="C#" MasterPageFile="~/Staff/Staff.Master" AutoEventWireup="true" CodeBehind="EditModule.aspx.cs" Inherits="SpotTheScam.EditModule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Edit Module</title>
    <style>
        body, .add-module-container, .add-module-container *, .form-label, .form-input, .form-select, .cover-image-label, .module-info-title {
            font-family: 'DM Sans', Arial, sans-serif !important;
        }
        .add-module-container {
            display: flex;
            gap: 48px;
            align-items: flex-start;
            margin-top: 24px;
            margin-bottom: 32px;
        }
        .cover-image-preview {
            width: 320px;
            height: 320px;
            background: #dbdbdb;
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 12px;
            overflow: hidden;
        }
        .cover-image-label {
            text-align: center;
            color: #051D40;
            font-size: 1.15em;
            font-weight: 500;
            margin-top: 0.5em;
        }
        .module-info-section {
            flex: 1;
            min-width: 320px;
        }
        .module-info-title {
            font-size: 1.35em;
            color: #222;
            font-weight: 600;
            margin-bottom: 0.2em;
        }
        .module-info-divider {
            border: none;
            border-top: 2px solid #222;
            margin-bottom: 1.2em;
            margin-top: 0.2em;
        }
        .form-label {
            color: #051D40;
            font-weight: 600;
            margin-bottom: 0.2em;
            display: block;
        }
        .form-input, .form-select, .form-textarea {
            width: 100%;
            background: #fff;
            border: 1.5px solid #bfc5ce;
            border-radius: 6px;
            padding: 10px 16px;
            font-size: 1.08em;
            margin-bottom: 1.1em;
            color: #585252;
            font-weight: 400;
            outline: none;
            transition: border-color 0.2s;
            resize: vertical;
        }
        .form-input:focus, .form-select:focus, .form-textarea:focus {
            border-color: #C46A1D;
        }
        .form-actions {
            margin-top: 18px;
        }
        .module-details-section {
            background: #fff;
            border-radius: 10px;
            padding: 32px 24px 24px 24px;
            margin-bottom: 32px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.03);
            max-width: 900px;
        }
        .module-details-title {
            font-size: 1.3em;
            font-weight: 600;
            color: #222;
            margin-bottom: 0.2em;
        }
        .module-details-divider {
            border: none;
            border-top: 2px solid #222;
            margin-bottom: 1.2em;
            margin-top: 0.2em;
        }
        .module-details-row {
            display: flex;
            gap: 16px;
            margin-bottom: 0.8em;
        }
        .module-details-row > div {
            flex: 1;
        }
        .rfv-error, .rfv-error span {
            font-family: 'DM Sans', Arial, sans-serif !important;
            font-size: 1em !important;
            margin: 2px 0 6px 0 !important;
            padding: 0 !important;
            display: block;
        }
        .current-image {
            width: 100%;
            height: 100%;
            object-fit: cover;
            border-radius: 12px;
            border: 1px solid #ddd;
        }
        .module-info-image {
            width: 120px;
            height: 120px;
            object-fit: cover;
            border-radius: 8px;
            border: 1px solid #ddd;
            display: block;
            margin-bottom: 8px;
        }
        .form-actions {
            margin-top: 18px;
            margin-bottom: 32px;
            display: flex;
            gap: 12px;
        }
        .btn {
            padding: 10px 24px;
            border-radius: 6px;
            font-weight: 500;
            font-size: 1em;
            border: none;
            cursor: pointer;
            transition: all 0.2s;
        }
        .btn-primary {
            background: #C46A1D;
            color: white;
        }
        .btn-primary:hover {
            background: #a85a1a;
        }
        .btn-secondary {
            background: #6c757d;
            color: white;
        }
        .btn-secondary:hover {
            background: #5a6268;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Main form, now stretches edge to edge -->
    <div class="add-module-container">
        <!-- Left: Cover Image Preview -->
        <div style="display: flex; flex-direction: column; align-items: center;">
            <div class="cover-image-preview">
                <asp:Image ID="imgCurrentCover" runat="server" CssClass="current-image" Visible="false" />
                <span id="noCoverImage" runat="server" style="color: #bcbcbc; font-size: 1.2em;" visible="false">Cover image</span>
            </div>
            <asp:FileUpload ID="fuCoverImage" runat="server" CssClass="form-input" style="display:none;" />
            <label for="<%= fuCoverImage.ClientID %>" class="cover-image-label" style="cursor:pointer;">Change cover image</label>
        </div>
        <!-- Right: Module Information -->
        <div class="module-info-section">
            <div class="module-info-title">Module</div>
            <hr class="module-info-divider" />
            <label for="<%= txtModuleName.ClientID %>" class="form-label">Module name:</label>
            <asp:TextBox ID="txtModuleName" runat="server" CssClass="form-input" />
            <asp:RequiredFieldValidator ID="rfvModuleName" runat="server" ControlToValidate="txtModuleName" ErrorMessage="Module name is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
            <label for="<%= txtAuthor.ClientID %>" class="form-label">Author:</label>
            <asp:TextBox ID="txtAuthor" runat="server" CssClass="form-input" />
            <label for="<%= ddlStatus.ClientID %>" class="form-label">Status:</label>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                <asp:ListItem Text="-- Select Status --" Value="" />
                <asp:ListItem Text="Draft" Value="draft" />
                <asp:ListItem Text="Published" Value="published" />
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatus" InitialValue="" ErrorMessage="Please select a status: Draft or Published" ForeColor="Red" Display="Dynamic" CssClass="rfv-error" />
            <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false" />
            <asp:Label ID="lblMessage" runat="server" CssClass="error-message" Visible="false" />
        </div>
    </div>
    <!-- Module Details Section (now edge to edge, no container) -->
    <div>
        <div class="module-details-title">Module Information</div>
        <hr class="module-details-divider" />
        <label for="txtDescription" class="form-label">Description:</label>
        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="3" placeholder="Enter module description..." />
        <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Description is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtIntroduction" class="form-label">Introduction:</label>
        <asp:TextBox ID="txtIntroduction" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="3" placeholder="Enter introduction text..." />
        <asp:RequiredFieldValidator ID="rfvIntroduction" runat="server" ControlToValidate="txtIntroduction" ErrorMessage="Introduction is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader1" class="form-label">Header 1:</label>
        <asp:TextBox ID="txtHeader1" runat="server" CssClass="form-input" placeholder="Enter header text..." />
        <asp:RequiredFieldValidator ID="rfvHeader1" runat="server" ControlToValidate="txtHeader1" ErrorMessage="Header 1 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader1Text" class="form-label">Text:</label>
        <asp:TextBox ID="txtHeader1Text" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="2" placeholder="Enter content..." />
        <asp:RequiredFieldValidator ID="rfvHeader1Text" runat="server" ControlToValidate="txtHeader1Text" ErrorMessage="Text for Header 1 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader2" class="form-label">Header 2:</label>
        <asp:TextBox ID="txtHeader2" runat="server" CssClass="form-input" placeholder="Enter header text..." />
        <asp:RequiredFieldValidator ID="rfvHeader2" runat="server" ControlToValidate="txtHeader2" ErrorMessage="Header 2 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader2Text" class="form-label">Text:</label>
        <asp:TextBox ID="txtHeader2Text" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="2" placeholder="Enter content..." />
        <asp:RequiredFieldValidator ID="rfvHeader2Text" runat="server" ControlToValidate="txtHeader2Text" ErrorMessage="Text for Header 2 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader3" class="form-label">Header 3:</label>
        <asp:TextBox ID="txtHeader3" runat="server" CssClass="form-input" placeholder="Enter header text..." />
        <asp:RequiredFieldValidator ID="rfvHeader3" runat="server" ControlToValidate="txtHeader3" ErrorMessage="Header 3 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader3Text" class="form-label">Text:</label>
        <asp:TextBox ID="txtHeader3Text" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="2" placeholder="Enter content..." />
        <asp:RequiredFieldValidator ID="rfvHeader3Text" runat="server" ControlToValidate="txtHeader3Text" ErrorMessage="Text for Header 3 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader4" class="form-label">Header 4:</label>
        <asp:TextBox ID="txtHeader4" runat="server" CssClass="form-input" placeholder="Enter header text..." />
        <asp:RequiredFieldValidator ID="rfvHeader4" runat="server" ControlToValidate="txtHeader4" ErrorMessage="Header 4 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader4Text" class="form-label">Text:</label>
        <asp:TextBox ID="txtHeader4Text" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="2" placeholder="Enter content..." />
        <asp:RequiredFieldValidator ID="rfvHeader4Text" runat="server" ControlToValidate="txtHeader4Text" ErrorMessage="Text for Header 4 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader5" class="form-label">Header 5:</label>
        <asp:TextBox ID="txtHeader5" runat="server" CssClass="form-input" placeholder="Enter header text..." />
        <asp:RequiredFieldValidator ID="rfvHeader5" runat="server" ControlToValidate="txtHeader5" ErrorMessage="Header 5 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="txtHeader5Text" class="form-label">Text:</label>
        <asp:TextBox ID="txtHeader5Text" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="2" placeholder="Enter content..." />
        <asp:RequiredFieldValidator ID="rfvHeader5Text" runat="server" ControlToValidate="txtHeader5Text" ErrorMessage="Text for Header 5 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="fuImage1" class="form-label">Image 1:</label>
        <asp:Image ID="imgCurrentImage1" runat="server" CssClass="module-info-image" Visible="false" />
        <asp:FileUpload ID="fuImage1" runat="server" CssClass="form-input" />
        <label for="fuImage2" class="form-label">Image 2:</label>
        <asp:Image ID="imgCurrentImage2" runat="server" CssClass="module-info-image" Visible="false" />
        <asp:FileUpload ID="fuImage2" runat="server" CssClass="form-input" />
        <div class="form-actions">
            <asp:Button ID="btnUpdate" runat="server" Text="Update Module" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>
