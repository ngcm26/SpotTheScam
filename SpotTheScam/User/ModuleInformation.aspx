<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ModuleInformation.aspx.cs" Inherits="SpotTheScam.User.ModuleInformation" MaintainScrollPositionOnPostBack="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css?family=DM+Sans:400,500,700&display=swap" rel="stylesheet" />
    <style>
        .article-container {
            max-width: 800px;
            margin: 40px auto 40px auto;
            background: #fff;
            border-radius: 18px;
            box-shadow: 0 4px 24px rgba(5,29,64,0.07);
            padding: 48px 32px 40px 32px;
            font-family: 'DM Sans', Arial, sans-serif;
        }
		.article-action-bar {
			max-width: 800px;
			margin: 24px auto 0 auto;
			display: flex;
			gap: 12px;
			justify-content: flex-end;
		}
		.action-btn {
			background: #f3f4f6;
			color: #051D40;
			border: 1px solid #e5e7eb;
			border-radius: 10px;
			padding: 10px 14px;
			font-size: 0.98rem;
			font-weight: 600;
			cursor: pointer;
			transition: background 0.2s, border-color 0.2s;
		}
		.action-btn:hover { background: #eef0f3; border-color: #d5d7db; }
        .article-title {
            color: #D36F2D;
            font-size: 2.3rem;
            font-weight: 700;
            margin-bottom: 0.3em;
            font-family: 'DM Sans', Arial, sans-serif;
            display: block;
        }
        .article-intro {
            color: #686464;
            font-size: 1.18rem;
            margin-bottom: 1.2em;
            font-family: 'DM Sans', Arial, sans-serif;
        }
        .article-byline {
            color: #686464;
            font-size: 1.02rem;
            margin-bottom: 1.5em;
            display: flex;
            justify-content: space-between;
            border-bottom: 1px solid #eee;
            padding-bottom: 0.7em;
            font-family: 'DM Sans', Arial, sans-serif;
        }
        .article-description {
            color: #051D40;
            font-size: 1.13rem;
            margin-bottom: 1.7em;
            line-height: 1.7;
            font-family: 'DM Sans', Arial, sans-serif;
        }
        .article-section-title {
            display: block;
            color: #D36F2D;
            font-size: 1.35rem;
            font-weight: 700;
            margin-top: 2.2em;
            font-family: 'DM Sans', Arial, sans-serif;
        }
        .article-section-text {
            display: block;
            color: #051D40;
            font-size: 1.13rem;
            /* margin-top: 0.7em; */
            margin-bottom: 2.2em;
            line-height: 1.7;
            font-family: 'DM Sans', Arial, sans-serif;
        }
        .article-image {
            display: block;
            margin: 32px auto 24px auto;
            max-width: 540px;
            max-height: 320px;
            width: 100%;
            height: auto;
            border-radius: 14px;
            box-shadow: 0 2px 12px rgba(5,29,64,0.08);
            object-fit: cover;
            aspect-ratio: 1.6875 / 1; /* 540/320 for consistent aspect ratio */
        }
        @media (max-width: 900px) {
            .article-container { padding: 24px 8vw 32px 8vw; }
        }
        @media (max-width: 600px) {
            .article-container { padding: 12px 2vw 18px 2vw; }
            .article-title { font-size: 1.5rem; }
        }
		.quiz-btn {
			display: block;
			width: 260px;
			margin: 14px auto 0 auto;
			background: #2563eb;
			color: #fff;
			border: none;
			border-radius: 8px;
			padding: 12px 0;
			font-size: 1.1rem;
			font-weight: 600;
			text-align: center;
			text-decoration: none;
			transition: background 0.2s;
			font-family: 'DM Sans', Arial, sans-serif;
		}
		.quiz-btn:hover { background: #1d4ed8; color: #fff; }
		/* Empty state */
		.unavailable {
			max-width: 800px;
			margin: 40px auto;
			padding: 28px 20px;
			text-align: center;
			background: #fff7ed;
			border: 1px solid #ffedd5;
			border-radius: 14px;
			font-family: 'DM Sans', Arial, sans-serif;
		}
		.unavailable h2 { color: #D36F2D; margin-bottom: 8px; }
		.unavailable p { color: #6b7280; margin-bottom: 16px; }

		/* Print styles: hide action bar/buttons */
		@media print {
			.article-action-bar, .complete-btn, .back-to-modules-btn { display: none !important; }
			.article-container { box-shadow: none; border-radius: 0; margin: 0; max-width: 100%; }
		}
    .complete-btn {
        background: #64C35C;
        color: #fff;
        border: none;
        border-radius: 8px;
        padding: 12px 0;
        font-size: 1.1rem;
        font-weight: 600;
        width: 260px;
        margin: 32px auto 0 auto;
        display: block;
        text-align: center;
        transition: background 0.2s;
    }
    .complete-btn[disabled], .complete-btn:disabled {
        background: #4ea94e;
        color: #fff;
        opacity: 0.8;
    }
    .complete-message {
        background: #64C35C;
        color: #fff;
        border-radius: 8px;
        padding: 12px 0;
        font-size: 1.1rem;
        font-weight: 600;
        width: 260px;
        margin: 32px auto 0 auto;
        display: block;
        text-align: center;
        font-family: 'DM Sans', Arial, sans-serif;
    }
    .back-to-modules-btn {
        display: block;
        width: 260px;
        margin: 18px auto 0 auto;
        background: #D36F2D;
        color: #fff;
        border: none;
        border-radius: 8px;
        padding: 12px 0;
        font-size: 1.1rem;
        font-weight: 600;
        text-align: center;
        text-decoration: none;
        transition: background 0.2s;
        font-family: 'DM Sans', Arial, sans-serif;
    }
    .back-to-modules-btn:hover {
        background: #b95a22;
        color: #fff;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<!-- Actions above the article -->
	<div class="article-action-bar">
		<asp:HyperLink ID="lnkBackToEdit" runat="server" CssClass="action-btn" Visible="false" Text="← Back to Edit" NavigateUrl="#" />
		<button type="button" class="action-btn" onclick="window.print()">Print / Save as PDF</button>
	</div>

	<asp:Panel ID="pnlContent" runat="server">
	<div class="article-container">
        <asp:Label ID="lblModuleName" runat="server" CssClass="article-title" />
        <asp:Label ID="lblIntroduction" runat="server" CssClass="article-intro" />
        <div class="article-byline">
            <span>By: <asp:Label ID="lblAuthor" runat="server" /></span>
            <span><asp:Label ID="lblDate" runat="server" /></span>
        </div>
        <asp:Label ID="lblDescription" runat="server" CssClass="article-description" />
        <asp:Label ID="lblHeader1" runat="server" CssClass="article-section-title" Visible="false" />
        <asp:Label ID="lblHeader1Text" runat="server" CssClass="article-section-text" Visible="false" />
        <asp:Image ID="img1" runat="server" CssClass="article-image" Visible="false" />
        <asp:Label ID="lblHeader2" runat="server" CssClass="article-section-title" Visible="false" />
        <asp:Label ID="lblHeader2Text" runat="server" CssClass="article-section-text" Visible="false" />
        <asp:Label ID="lblHeader3" runat="server" CssClass="article-section-title" Visible="false" />
        <asp:Label ID="lblHeader3Text" runat="server" CssClass="article-section-text" Visible="false" />
        <asp:Image ID="img2" runat="server" CssClass="article-image" Visible="false" />
        <asp:Label ID="lblHeader4" runat="server" CssClass="article-section-title" Visible="false" />
        <asp:Label ID="lblHeader4Text" runat="server" CssClass="article-section-text" Visible="false" />
        <asp:Label ID="lblHeader5" runat="server" CssClass="article-section-title" Visible="false" />
        <asp:Label ID="lblHeader5Text" runat="server" CssClass="article-section-text" Visible="false" />
        <asp:Label ID="lblCompleteMessage" runat="server" CssClass="complete-message" />
        <asp:Button ID="btnCompleteModule" runat="server" Text="Mark Module as Complete" OnClick="btnCompleteModule_Click" Visible="false" CssClass="complete-btn" />
		<asp:HyperLink ID="lnkTakeQuiz" runat="server" CssClass="quiz-btn" Visible="false" Text="Take the Quiz" NavigateUrl="#" />
        <asp:HyperLink ID="lnkBackToModules" runat="server" CssClass="back-to-modules-btn" NavigateUrl="UserModules.aspx" Text="Back to Modules" />
    </div>
	</asp:Panel>

	<asp:Panel ID="pnlUnavailable" runat="server" Visible="false">
		<div class="unavailable">
			<h2>Module unavailable</h2>
			<p>The module you are trying to view is not available or has not been published yet.</p>
			<a href="UserModules.aspx" class="back-to-modules-btn" style="margin-top: 0;">Back to Modules</a>
		</div>
	</asp:Panel>
</asp:Content>
