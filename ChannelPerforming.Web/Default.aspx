<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChannelPerforming.Web._Default" %>

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
        .header td {
            text-decoration: none;
            font-style: italic;
            text-align: center;
            margin: 2px 3px;
        }

    </style>
    <div class="container">
        <asp:DataList ID="VideoList" runat="server">
            <ItemTemplate>
                <td>
               
                    <img src='<%= Eval("ThumbnailImageUrl") %>' width="100px" height="100px"/>
                    <h4><%= Eval("Title") %> asd</h4>
                <a href='VideoPlayer.aspx?q=<%= Eval("RowKey") %>'>
                     </a>
            </td>
                </div>
            </ItemTemplate>

        </asp:DataList>

   
        </div>
</asp:Content>
