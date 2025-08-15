<%@ Page Title="Add New Module" Language="C#" MasterPageFile="Staff.master" AutoEventWireup="true" CodeBehind="StaffAddModules.aspx.cs" Inherits="SpotTheScam.Staff.StaffAddModules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Add New Module</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/cropperjs/1.6.2/cropper.min.css" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/cropperjs/1.6.2/cropper.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
            position: relative;
        }
        .cover-image-preview img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            border-radius: 16px;
        }
        .cover-image-preview .placeholder-text {
            color: #bcbcbc;
            font-size: 1.2em;
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
        }
        .cover-image-label {
            text-align: center;
            color: #051D40;
            font-size: 1.15em;
            font-weight: 500;
            margin-top: 0.5em;
            cursor: pointer;
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
    </style>
    
    <script type="text/javascript">
        var cropperInstance = null;
        var cropperModal = null;
        var cropperImage = null;

        function openCropper(dataUrl) {
            cropperModal = document.getElementById('cropperModal');
            cropperImage = document.getElementById('cropperImage');
            cropperImage.src = dataUrl;
            cropperModal.style.display = 'flex';
            // Aspect ratio ~2:1 to match ~320x160 display on user modules grid
            cropperInstance = new Cropper(cropperImage, {
                aspectRatio: 2,
                viewMode: 1,
                autoCropArea: 1,
                background: true,
                movable: false,
                zoomable: true,
                scalable: false,
                responsive: true
            });
        }

        function closeCropper() {
            if (cropperInstance) {
                cropperInstance.destroy();
                cropperInstance = null;
            }
            if (cropperModal) {
                cropperModal.style.display = 'none';
            }
        }

        function applyCrop() {
            if (!cropperInstance) return;
            var canvas = cropperInstance.getCroppedCanvas({ width: 1200, height: 600 });
            var dataUrl = canvas.toDataURL('image/jpeg', 0.9);

            // Update preview box
            var preview = document.getElementById('coverImagePreview');
            preview.innerHTML = '';
            var img = document.createElement('img');
            img.style.width = '100%';
            img.style.height = '100%';
            img.style.objectFit = 'cover';
            img.style.borderRadius = '16px';
            img.src = dataUrl;
            preview.appendChild(img);

            // Persist in hidden field for server upload
            document.getElementById('<%= hdnCoverImageBase64.ClientID %>').value = dataUrl;

            closeCropper();

            // Postback to persist server-side and update preview to server URL
            setTimeout(function() { __doPostBack('<%= btnUploadCover.UniqueID %>', ''); }, 0);
        }

        function previewCoverImage(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function(e) {
                    // Open cropper modal instead of immediately previewing
                    openCropper(e.target.result);
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
        
        function removeCoverImage() {
            var preview = document.getElementById('coverImagePreview');
            var placeholder = document.getElementById('coverImagePlaceholder');
            var fileInput = document.getElementById('<%= fuCoverImage.ClientID %>');
            
            // Clear preview
            preview.innerHTML = '<span id="coverImagePlaceholder" class="placeholder-text">Cover image</span>';
            
            // Clear file input
            fileInput.value = '';
        }
        
        function previewImage1(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function(e) {
                    var preview = document.getElementById('image1Preview');
                    if (preview) {
                        preview.innerHTML = '<div style="position: relative; display: inline-block; margin-top: 10px;"><img src="' + e.target.result + '" style="max-width: 200px; max-height: 150px; border-radius: 8px;" /><button type="button" onclick="removeImage1()" style="position: absolute; top: 4px; right: 4px; background: rgba(255,0,0,0.8); color: white; border: none; border-radius: 50%; width: 20px; height: 20px; cursor: pointer; font-size: 10px; font-weight: bold;">✕</button></div>';
                    }
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
        
        function removeImage1() {
            var preview = document.getElementById('image1Preview');
            var fileInput = document.getElementById('<%= fuImage1.ClientID %>');
            
            // Clear preview
            preview.innerHTML = '';
            
            // Clear file input
            fileInput.value = '';
        }
        
        function previewImage2(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function(e) {
                    var preview = document.getElementById('image2Preview');
                    if (preview) {
                        preview.innerHTML = '<div style="position: relative; display: inline-block; margin-top: 10px;"><img src="' + e.target.result + '" style="max-width: 200px; max-height: 150px; border-radius: 8px;" /><button type="button" onclick="removeImage2()" style="position: absolute; top: 4px; right: 4px; background: rgba(255,0,0,0.8); color: white; border: none; border-radius: 50%; width: 20px; height: 20px; cursor: pointer; font-size: 10px; font-weight: bold;">✕</button></div>';
                    }
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
        
        function removeImage2() {
            var preview = document.getElementById('image2Preview');
            var fileInput = document.getElementById('<%= fuImage2.ClientID %>');
            
            // Clear preview
            preview.innerHTML = '';
            
            // Clear file input
            fileInput.value = '';
        }
    </script>
    <!-- Main form, now stretches edge to edge -->
    <div class="add-module-container">
        <!-- Left: Cover Image Preview -->
        <div style="display: flex; flex-direction: column; align-items: center;">
            <div class="cover-image-preview" id="coverImagePreview">
                <span id="coverImagePlaceholder" class="placeholder-text">Cover image</span>
            </div>
            <asp:FileUpload ID="fuCoverImage" runat="server" CssClass="form-input" style="display:none;" onchange="previewCoverImage(this);" />
            <asp:HiddenField ID="hdnCoverImagePath" runat="server" />
            <asp:HiddenField ID="hdnCoverImageBase64" runat="server" />
            <asp:LinkButton ID="btnUploadCover" runat="server" OnClick="btnUploadCover_Click" style="display:none;" />
            <label for="<%= fuCoverImage.ClientID %>" class="cover-image-label" style="cursor:pointer;">Change cover image</label>
            <asp:RequiredFieldValidator ID="rfvCoverImage" runat="server" ControlToValidate="hdnCoverImagePath" ErrorMessage="Cover image is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
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
            <asp:RequiredFieldValidator ID="rfvAuthor" runat="server" ControlToValidate="txtAuthor" ErrorMessage="Author is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />

            <label for="<%= ddlStatus.ClientID %>" class="form-label">Status:</label>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                <asp:ListItem Text="-- Select Status --" Value="" />
                <asp:ListItem Text="Draft" Value="draft" />
                <asp:ListItem Text="Published" Value="published" />
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatus" InitialValue="" ErrorMessage="Please select a status: Draft or Published" ForeColor="Red" Display="Dynamic" CssClass="rfv-error" />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
        </div>
    </div>

    <!-- Simple Cropper Modal -->
    <div id="cropperModal" style="display:none; position:fixed; inset:0; background: rgba(0,0,0,0.6); z-index: 9999; align-items:center; justify-content:center;">
        <div style="background:#fff; padding:16px; border-radius:12px; max-width:90vw; max-height:85vh; width:900px;">
            <div style="width:100%; height:60vh; overflow:hidden;"><img id="cropperImage" alt="Crop image" style="max-width:100%; display:block;" /></div>
            <div style="display:flex; justify-content:flex-end; gap:8px; margin-top:12px;">
                <button type="button" class="btn btn-secondary" onclick="closeCropper()">Cancel</button>
                <button type="button" class="btn btn-primary" onclick="applyCrop()">Apply Crop</button>
            </div>
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
        <asp:FileUpload ID="fuImage1" runat="server" CssClass="form-input" onchange="previewImage1(this);" />
        <div id="image1Preview"></div>
        <asp:RequiredFieldValidator ID="rfvImage1" runat="server" ControlToValidate="fuImage1" ErrorMessage="Image 1 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <label for="fuImage2" class="form-label">Image 2:</label>
        <asp:FileUpload ID="fuImage2" runat="server" CssClass="form-input" onchange="previewImage2(this);" />
        <div id="image2Preview"></div>
        <asp:RequiredFieldValidator ID="rfvImage2" runat="server" ControlToValidate="fuImage2" ErrorMessage="Image 2 is required." Display="Dynamic" ForeColor="Red" CssClass="rfv-error" />
        <div class="form-actions" style="display:flex; gap:8px; align-items:center;">
            <asp:Button ID="btnAddModule" runat="server" Text="Add Module" OnClick="btnAddModule_Click" CssClass="btn btn-primary" />
            <asp:HyperLink ID="lnkPreviewNew" runat="server" CssClass="btn btn-secondary" Text="Preview" Visible="false" NavigateUrl="#" />
        </div>
    </div>
</asp:Content>
