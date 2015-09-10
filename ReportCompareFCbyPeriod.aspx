<%@ Page Title="Forecast Period vs Forecast Period reports" Language="C#" MasterPageFile="~/gamReporting.master" AutoEventWireup="true" Inherits="ReportCompareFCbyPeriod" Codebehind="ReportCompareFCbyPeriod.aspx.cs" %>

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
    
        Report (1)FY period forecast VS (2)FY period forecast:
        <div>
        <asp:Label ID="currentPeriod" runat="server" Visible="false" />
        First Period: <asp:DropDownList ID="FY1" runat="server"></asp:DropDownList>
        <asp:DropDownList ID="Period1" runat="server"></asp:DropDownList>

        &nbsp;VS&nbsp;

        Second Period: <asp:DropDownList ID="FY2" runat="server"></asp:DropDownList>
        <asp:DropDownList ID="Period2" runat="server"></asp:DropDownList>
        <asp:Button ID="Button1" runat="server" Text="Go" onclick="Button1_Click" OnClientClick="return pageClear()" />
        </div>
        
        <div id="loading" style="display:none; text-align:center; padding:20px 0px 20px 0px; margin:0px auto 0px; width:200px;">
            <img src="http://bi.multek.com/ws/images/ajax-loader_1.gif" alt="loading" align="middle" />
        </div>
        <div id="report" style="width:auto; overflow:auto;" >
            <asp:Label ID="message" runat="server" />
            <div><asp:Button ID="downloadReport" runat="server" Text="Download Reports" OnClick="downloadReport_Click" Visible="false" /></div>
            <asp:GridView ID="gridPlant" runat="server" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                ondatabound="gridPlant_DataBound" onrowdatabound="gridPlant_RowDataBound" >
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>     
            <br />      
            <asp:GridView ID="grid" runat="server" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                ondatabound="grid_DataBinding" onrowdatabound="grid_RowDataBound" >
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
    <asp:PostBackTrigger  ControlID="downloadReport" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>

