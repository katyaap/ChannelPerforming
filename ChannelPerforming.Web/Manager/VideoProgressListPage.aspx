<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VideoProgressListPage.aspx.cs" Inherits="ChannelPerforming.Web.Manager.VideoProgressListPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridViewVideo" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" GridLines="Vertical" HorizontalAlign="Center" Width="780px">
        <Columns>
            <asp:BoundField DataField="Title" HeaderText="Video Name" />
            <asp:BoundField DataField="MediaProgressStateType" HeaderText="State" />
        </Columns>
    </asp:GridView>
</asp:Content>
