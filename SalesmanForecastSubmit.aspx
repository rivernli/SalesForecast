<%@ Page Language="C#" MasterPageFile="~/gamReporting.master" AutoEventWireup="true" Inherits="SalesmanForecastSubmit" Title="Salesman Submitted" Codebehind="SalesmanForecastSubmit.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    Salesman Submitted Forecast!
    <asp:GridView ID="grid1" runat="server" AutoGenerateColumns="False" 
        BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
        CellPadding="3" GridLines="Vertical" Width="100%">
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="userName" 
                DataNavigateUrlFormatString="adjustmentTopSide.aspx#{0}" DataTextField="userName" 
                HeaderText="Salesman" />
            <asp:BoundField DataField="userName" HeaderText="Salesman" />
            <asp:BoundField DataField="emailAddress" HeaderText="Email" />
            <asp:BoundField DataField="period" HeaderText="Period" />
            <asp:BoundField DataField="submit_time" HeaderText="Submitted Time" />
        </Columns>
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
</asp:Content>

