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
               
                <a href='Player.aspx?q=<%# Eval("RowKey") %>'>
                        <img src='<%# Eval("ThumbnailImageUrl") %>' width="300px" height="200px"/>
                    <h4><%# Eval("Title") %></h4>
                
                     </a>
            </td>
                </div>
            </ItemTemplate>

        </asp:DataList>

   
        </div>
</asp:Content>
