<%@ Page Title="B2F forecast reports" Language="C#" MasterPageFile="~/gamReporting.master" AutoEventWireup="true" Inherits="ReportB2FView" Codebehind="ReportB2FView.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">

    <script type="text/javascript">
    function pageClear() {
        document.getElementById("report").style.display = "none";
        document.getElementById("loading").style.display = "block";
        return true;
    }
    function loadpage() {
        document.getElementById("loading").style.display = "none";
        document.getElementById("report").style.display = "block";
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Label ID="currentPeriod" runat="server" Visible="false" />
        Select B2F Forecast on: <asp:DropDownList ID="FY1" runat="server"></asp:DropDownList>
        <asp:DropDownList ID="Period1" runat="server" />
        <asp:Button ID="go" runat="server" Text="Go" onclick="go_Click" OnClientClick="return pageClear()" />
       
        <asp:Button ID="downloadResult" runat="server" Text="Download" 
        onclick="downloadResult_Click" Visible="false" />
                <div id="loading" style="display:none; text-align:center; padding:20px 0px 20px 0px; margin:0px auto 0px; width:200px;">
            <img src="http://bi.multek.com/ws/images/ajax-loader_1.gif" alt="loading" align="middle" />
        </div>
        <div id="report" style="width:auto; overflow:auto;" >
            <asp:GridView ID="resultGrid" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Vertical" >
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>
        </div>
        </ContentTemplate>
    <Triggers>
    <asp:PostBackTrigger  ControlID="downloadResult" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>

