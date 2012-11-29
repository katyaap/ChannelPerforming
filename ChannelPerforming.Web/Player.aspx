<%@ Page  Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Player.aspx.cs" Inherits="ChannelPerforming.Web.Player" %>
<%@ Import namespace="ChannelPerforming.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/leanback/css.player/leanbackPlayer.default.css" rel="stylesheet" />
    <script src="Content/leanback/js.player/leanbackPlayer.pack.js"></script>
    <script src="Content/leanback/js.player/leanbackPlayer.en.js"></script>
    <style type="text/css">
        .container {
            margin-left: auto;
            margin-right: auto;
            width: 50em;
        }

        .header {
            text-align: center;
            margin: 10px 20px;
        }

        .header a {
            text-decoration: none;
         }

        .footer {
            text-align: left;
            font-style: italic;
            margin: 10px 20px;
        }
    </style>

     <div class="container">
        <div class="header">
            <a href="VideoPlayer.aspx?q=<%= RowKey %>">
                <%= Title %>
            </a>
        </div>
        <div class="leanback-player-video">
            <!-- HTML5 Video Element -->
            <video width="640" height="360" preload="auto" autoplay controls poster="<%= ThumbnailImageUrl %>">
                <!-- HTML5 Video Source(s) -->
                <source src="<%= MediaUrl %>" type='video/mp4; codecs="avc1.42E01E, mp4a.40.2"' />

                <!-- HTML-Fallback -->
                <div class="leanback-player-html-fallback" style="width: 640px; height: 360px;">
                    <img src="<%= ThumbnailImageUrl %>" width="640" height="360" alt="Poster Image"
                        title="No HTML5-Video playback capabilities found. Please download the video(s) below." />
                    <div>
                        <strong>Download Video:</strong>
                        <a href="<%= ThumbnailImageUrl %>">.mp4</a>
                    </div>
                </div>
            </video>
        </div>
        <div class="footer">
            <%= Description %>
        </div>
    </div>
</asp:Content>
