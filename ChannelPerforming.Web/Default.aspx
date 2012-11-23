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
    <asp:ListView ID="ListViewVideoList" runat="server" GroupItemCount="6">
        <LayoutTemplate>
            <table>
                <tr>
                    <table border="0" cellpadding="5">
                        <asp:PlaceHolder ID="groupPlaceHolder" runat="server"></asp:PlaceHolder>
                    </table>
                </tr>
            </table>
        </LayoutTemplate>
        <GroupTemplate>
            <tr>
                <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
            </tr>
        </GroupTemplate>
        <ItemTemplate>
            <div class="header">
            <td>
                <a href='VideoPlayer.aspx?q=<%= Eval("RowKey") %>'>
                    <img src='<%= Eval("ThumbnailImageUrl") %>' width="100px" height="100px"/>
                    <h4><%= Eval("Title") %> asd</h4>
                </a>
            </td>
                </div>
        </ItemTemplate>
    </asp:ListView>

    <asp:DataPager ID="DataVideoPager"  PagedControlID="ListViewVideoList" runat="server">
        <Fields>
            <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
            <asp:NumericPagerField />
            <asp:NextPreviousPagerField ButtonType="Link" ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
        </Fields>
    </asp:DataPager>
        </div>
</asp:Content>
