<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="ModuleInformation.aspx.cs" Inherits="SpotTheScam.User.ModuleInformation" %>
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
        .article-title {
            color: #D36F2D;
            font-size: 2.3rem;
            font-weight: 700;
            margin-bottom: 0.3em;
            font-family: 'DM Sans', Arial, sans-serif;
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
    </div>
</asp:Content>
