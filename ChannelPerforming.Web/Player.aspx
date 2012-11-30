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
        .comment {
            
        }
        .comment .CommentFrom {
            
        }

        .Control { }

        .Comments {

            margin:25px 0px 20px;
        }

        .CommentContent { }

        .Contents {
            margin:20px 0px 0px;
        }

        .Content { }

        .Contents label {
            margin:5px 0px 0px;
        }
    </style>

     <div class="container">
        <div class="header">
           <%= Title %>
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
                        <a href="<%= MediaUrl %>">Dowland</a>
                    </div>
                </div>
            </video>
        </div>
        <div class="footer">
            <%= Description %>
        </div>
         <div class="comment">
             <label>Comments</label> 
             <div class="Comments">
                 <asp:DataList ID="DataListComment" runat="server" Width="662px">
                     <ItemTemplate>
                 <div class="Contents">
                     <label>
                         <%# Eval("UserName") %>
                     </label>
                    <div class="Content">
                        <%# Eval("Contnet") %>
                    </div>
                 </div>
                     </ItemTemplate>
                 </asp:DataList>
             </div>
             <div class="CommentFrom">
                 <div class="Control">
                     <label>Name</label>
                     <asp:TextBox ID="TextBoxCommentName" runat="server" Width="425px"></asp:TextBox>    
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBoxCommentName" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="CommentGrup"></asp:RequiredFieldValidator>
                 </div>
                 <div class="Control">
                     <label>Email</label>
                     <asp:TextBox ID="TextBoxCommentEmail" TextMode="Email" runat="server" Width="425px"></asp:TextBox>    
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="TextBoxCommentEmail" runat="server" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="CommentGrup"></asp:RequiredFieldValidator>
                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBoxCommentEmail" ErrorMessage="*" ForeColor="#FF3300" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="CommentGrup"></asp:RegularExpressionValidator>
                 </div>
                 <div class="Control">
                     <label>Comment</label>
                     <asp:TextBox ID="TextBoxCommentContent" TextMode="MultiLine" runat="server" Height="130px" Width="425px"></asp:TextBox>    
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBoxCommentContent" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="CommentGrup"></asp:RequiredFieldValidator>
                 </div>
                 <div class="Control">
                     <asp:Button ID="ButtonSend" runat="server" Text="Send" OnClick="ButtonSend_Click" ValidationGroup="CommentGrup" />
                     <div>
                         <asp:Label ID="LabelCommentResult" runat="server" Text=""></asp:Label>
                     </div>
                 </div>
             </div>
         </div>
    </div>
    <asp:HiddenField ID="HiddenFieldVideo" runat="server" />
</asp:Content>
