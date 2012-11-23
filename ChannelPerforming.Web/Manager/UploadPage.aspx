<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadPage.aspx.cs" Inherits="ChannelPerforming.Web.Manager.UploadPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table class="auto">
        <tr>
            <td>
                <asp:Label ID="LabelVideoName" runat="server" Text="Video Name"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="TextBoxVideoName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="TextBoxVideoName" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LabelVideoDescription" runat="server" Text="Video Description"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="TextBoxVideoDescription" runat="server" TextMode="MultiLine" Height="88px" Width="364px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LabelTag" runat="server" Text="Tags"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="TextBoxTags" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:FileUpload ID="VideoUpload" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="ButtonSave" runat="server" Text="Save" OnClick="ButtonSave_Click" />
            </td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>
