<%@ Page Title="Forecast Period vs Forecast Period reports" Language="C#" MasterPageFile="~/gamReporting.master" AutoEventWireup="true" Inherits="ReportFCP2P" Codebehind="ReportFCP2P.aspx.cs" %>

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
            <div><asp:Button ID="downloadReport" runat="server" Text="Download Reports" OnClick="downloadReport_Click" Visible="false" />
                
            </div>
            <div id="chart" />
            
           <asp:LinkButton ID="showPlant" runat="server" Text="Plant" Enabled="false" 
                    onclick="showPlant_Click" />
           | <asp:LinkButton ID="showOEMs" runat="server" Text="OEM" onclick="showOEMs_Click" />
           | <asp:LinkButton ID="showSales" runat="server" Text="Salesman" onclick="showSales_Click" />
           | <asp:LinkButton ID="showAll" runat="server" Text="All" onclick="showAll_Click"  />
           
           <asp:Panel ID="Sales" runat="server" Visible="true"> 
            <asp:GridView ID="gridSales" runat="server" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                AutoGenerateColumns="False" >
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <Columns>
                    <asp:BoundField DataField="userName" HeaderText="Salesman" />
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>     
            </asp:Panel>           
           <asp:Panel ID="Plants" runat="server" Visible="true"> 
            <asp:GridView ID="gridPlant" runat="server" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                AutoGenerateColumns="False" >
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <Columns>
                    <asp:BoundField DataField="plant" HeaderText="Plant" />
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>     
            </asp:Panel>
            <asp:Panel ID="OEMs" runat="server" Visible="false" >
            <asp:GridView ID="gridOEM" runat="server" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                AutoGenerateColumns="False" >
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <Columns>
                    <asp:BoundField DataField="oemName" HeaderText="OEM" />
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>     
            </asp:Panel>
            <asp:Panel ID="all" runat="server" Visible="false" >
            <asp:GridView ID="grid" runat="server" BackColor="White" BorderColor="#999999" 
                BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" 
                ondatabound="grid_DataBinding" 
                AutoGenerateColumns="False" >
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <Columns>
                    <asp:BoundField DataField="oemName" HeaderText="OEM" />
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView>
            </asp:Panel>
        </div>
        
        </ContentTemplate>
    <Triggers>
    <asp:PostBackTrigger  ControlID="downloadReport" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>

