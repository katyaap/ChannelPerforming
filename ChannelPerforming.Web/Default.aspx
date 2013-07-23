<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChannelPerforming.Web._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <style type="text/css">
        .container
        {
	        margin-left: auto;
	        margin-right: auto;
	        width: 50em;
        }

        .VideoHeader {
            margin: 5px 0px 5px;
            text-align: center;
        }

        .VideoContent {
            margin: 10px 0px 0px;
            float: left;
        }
    </style>
    <div class="container">
        <asp:DataList ID="VideoList" runat="server" Width="736px">
            <ItemTemplate>
                <div class="VideoContent">
                    <a href='ProtectionPlayer.aspx?q=<%# Eval("RowKey") %>'>
                        <h4 class="VideoHeader"><%# Eval("Title") %></h4>
                        <img src='<%# Eval("ThumbnailImageUrl") %>' width="300px" height="200px"/>
                     </a>
                </div>
           
            </ItemTemplate>

        </asp:DataList>
       
   
        </div>
</asp:Content>
