<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="ProtectionPlayer.aspx.cs" Inherits="ChannelPerforming.Web.ProtectionPlayer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    
    <style type="text/css">
        .container {
            margin-left: auto;
            margin-right: auto;
            width: 50em;
        }
        
        #silverlightControlHost {
	        height: 100%;
	        text-align:center;
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

        .Control {
        }

        .Comments {
            margin: 25px 0px 20px;
        }

        .CommentContent {
        }

        .Contents {
            margin: 20px 0px 0px;
        }

        .Content {
        }

        .Contents label {
            margin: 5px 0px 0px;
        }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
    <div class="container">
        <div class="header">
            <%= Title %>
        </div>
        <div class="leanback-player-video">
            <div id="silverlightControlHost">
                <object data="data:application/x-silverlight-2," height="100%" type="application/x-silverlight-2" width="100%">
                    <param name="source" value="ClientBin/ChannelPerforming.ProtectionPlayer.xap" />
                    <param name="minRuntimeVersion" value="4.0.50401.0" />
                    <param name="autoUpgrade" value="true" />
                    <param name="InitParams" value="mediaurl=<%= MediaUrl %>" />
                </object>

                <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>z
            </div>
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
