<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" 
 ValidateRequest="false" Inherits="B2FForecast" Title="B2F Forecast Upload" Codebehind="B2FForecast.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
function processing_upload(){
    document.getElementById("page").style.display = "none";
    document.getElementById("pageloading").style.display = "block";
    return true;
}
function finish_upload(){
    document.getElementById("page").style.display = "block";
    document.getElementById("pageloading").style.display = "none";
}
</script>
<style type="text/css">
.mainPanel { position:absolute; top:0px; left:0px; display:none; z-index:1999; width:auto;}
.mainPanel .subPanel{ position:relative; top:0px; left:16px; padding:5px; width:auto; height:auto; background-color:#ffffff; border:1px #ccc solid;}
.subPanel span {padding-left:4px; padding-right:2px; background-color:#ffffff; position:relative;}
.mainPanel .sayIcon {position:relative; top:22px; left:0px; background-image:url("http://bi.multek.com/ws/images/sayConner.png"); background-repeat:no-repeat; background-position:left top; width:20px; height:12px; z-index:50}
.mainPanel .subPanel .standardTable td,th {font-size:11px;white-space: nowrap;}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

<div id="ulist" class="mainPanel"><div class="sayIcon">&nbsp;</div><div class="subPanel" style="cursor:pointer" onclick="closePanel()"></div></div>
    
<div id="pageloading" style="display:none; text-align:center; padding:20px 0px 20px 0px; margin:0px auto 0px; width:200px;">
<img src="http://bi.multek.com/ws/images/ajax-loader_1.gif" alt="loading" align="middle" />
</div>

<div id="page">
<asp:Label ID="tmp" runat="server" ForeColor="Red" />
<asp:Label ID="currentPeriod" runat="server" Visible="false" />
<asp:Panel ID="DisplayArea" runat="server">
    <asp:LinkButton ID="uploadAgain" runat="server" 
        Text="Clear existing data and upload again." onclick="uploadAgain_Click" />
    
    <asp:LinkButton ID="displayResult" runat="server" Text="[Display By OEM+CPN+PN]" OnClick="displayResult_click" />
    
    <asp:ListView id="ListViewDisplay" runat="server">
        <LayoutTemplate>
                <table id="Table1" style="left:0px; top:0px; background-color:#ffffff; border-color:#888888; border-collapse:collapse; border-spacing:1; border-style:solid;"
                 cellpadding="1" cellspacing="0" border="1" bordercolor="#888888" runat="server" >
                 <tr bgcolor="#688EFF">
        <td>Forecast Period</td>
        <td>Sales</td>
        <td>OEM</td>
        <td>Customer Part#</td>
        <td>Internal Part#</td>
        <td>SQFT</td>
        <td>QTY</td>
        <td>Array</td>
        <td>SMT Unit Price</td>
        <td>FPC</td>
        <td>BOM</td>
    </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
                </table>
        </LayoutTemplate>
        <ItemTemplate>
                <tr bgcolor="#FFE48C">
                    <td><asp:Label ID="period" runat="server" Text='<%# Eval("forecastPeriod") %>' /></td>
                    <td><asp:Label ID="sales" runat="server" Text='<%# Eval("sales")%>' /></td>
                    <td><asp:Label ID="oem" runat="server" Text='<%# Eval("oem")%>' /></td>
                    <td><asp:Label ID="cpn" runat="server" Text='<%# Eval("cpn") %>' /></td>
                    <td><asp:Label ID="ipn" runat="server" Text='<%# Eval("pn") %>' /></td>
                    <td><asp:Label ID="sqft" runat="server" Text='<%# Eval("sqft") %>' /></td>
                    <td><asp:Label ID="qty" runat="server" Text='<%# Eval("qty") %>' /></td>
                    <td><asp:Label ID="array" runat="server" Text='<%# Eval("array") %>' /></td>
                    <td><asp:Label ID="smt" runat="server" Text='<%# Eval("smtPrice") %>' /></td>
                    <td><asp:Label ID="fpc" runat="server" Text='<%# Eval("fpcPrice") %>' /></td>
                    <td><asp:Label ID="bom" runat="server" Text='<%# Eval("bomPrice") %>' /></td>
                </tr>
        </ItemTemplate>
    </asp:ListView>                    
</asp:Panel>
<asp:Panel ID="ResultArea" runat="server" Visible="false">

    <asp:Button ID="goDisplay" runat="server" Text="Back" 
        onclick="goDisplay_Click" />
    <asp:Button ID="downloadResult" runat="server" Text="Download" 
        onclick="downloadResult_Click" />
    <asp:GridView ID="resultGrid" runat="server" BackColor="White" 
        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" EnableModelValidation="True" OnDataBound="resultGrid_DataBound" >
        <RowStyle ForeColor="#000066" />
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
    </asp:GridView>
</asp:Panel>
<asp:Panel ID="uploadArea" runat="server" >
    <asp:Label ID="uploadMessage" runat="server" />
    <div style="padding:2px;">
    <table width="100%" class="standardTable" style="BORDER-COLLAPSE: collapse"  cellSpacing="1" cellPadding="3" width="100%" 
    align="center" border="1" bordercolor='#aaaaaa'>
    <tr bgcolor="#dddddd" style="background-image:url(http://bi.multek.com/ws/images/tbackground.png); background-repeat:repeat-x;">
        <td>Forecast Period</td>
        <td>Sales</td>
        <td>OEM</td>
        <td>Customer Part#</td>
        <td>Internal Part#</td>
        <td>SQFT</td>
        <td>QTY</td>
        <td>Array</td>
        <td>SMT Unit Price</td>
        <td>FPC Unit Price</td>
        <td>BOM Unit Price</td>
    </tr></table>
    <asp:TextBox ID="cem_content" runat="server" Height="300px"  
            TextMode="MultiLine" Width="100%" ></asp:TextBox>
    </div>
    <div style="vertical-align:baseline;">
        <asp:Button ID="Button1" runat="server" Text="Preview" onclick="Button1_Click" 
            OnClientClick="processing_upload();" />
    </div>
</asp:Panel>

<asp:Panel ID="previewArea" runat="server" Visible="False">
    <div><asp:Label ID="previewMessage" runat="server" /></div>
<asp:Button ID="confirmUpload" Text="confirm Upload to System" runat="server" 
        onclick="confirmUpload_Click" OnClientClick="return confirm('are you sure to upload the B2F forecast to system?');" />
<asp:Button ID="cancelUpload" Text="Go Back" runat="server" 
        onclick="cancelUpload_Click" />
    <asp:ListView id="listMain" runat="server">
        <LayoutTemplate>
                <table id="Table1" style="left:0px; top:0px; background-color:#ffffff; border-color:#888888; border-collapse:collapse; border-spacing:1; border-style:solid;"
                 cellpadding="1" cellspacing="0" border="1" bordercolor="#888888" runat="server" >
                 <tr bgcolor="#688EFF">
        <td>Forecast Period</td>
        <td>Sales</td>
        <td>OEM</td>
        <td>Customer Part#</td>
        <td>Internal Part#</td>
        <td>SQFT</td>
        <td>QTY</td>
        <td>Array</td>
        <td>SMT Unit Price</td>
        <td>FPC</td>
        <td>BOM</td>
    </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
                </table>
        </LayoutTemplate>
        <ItemTemplate>
                <tr bgcolor="#FFE48C">
                    <td><asp:Label ID="period" runat="server" Text='<%# Eval("period") %>' /></td>
                    <td><asp:Label ID="sales" runat="server" Text='<%# Eval("sales")%>' /></td>
                    <td><asp:Label ID="oem" runat="server" Text='<%# Eval("oem")%>' /></td>
                    <td><asp:Label ID="cpn" runat="server" Text='<%# Eval("cpn") %>' /></td>
                    <td><asp:Label ID="ipn" runat="server" Text='<%# Eval("ipn") %>' /></td>
                    <td><asp:Label ID="sqft" runat="server" Text='<%# Eval("sqft") %>' /></td>
                    <td><asp:Label ID="qty" runat="server" Text='<%# Eval("qty") %>' /></td>
                    <td><asp:Label ID="array" runat="server" Text='<%# Eval("array") %>' /></td>
                    <td><asp:Label ID="smt" runat="server" Text='<%# Eval("smt") %>' /></td>
                    <td><asp:Label ID="fpc" runat="server" Text='<%# Eval("fpc") %>' /></td>
                    <td><asp:Label ID="bom" runat="server" Text='<%# Eval("bom") %>' /></td>
                </tr>
        </ItemTemplate>
    </asp:ListView>                    


</asp:Panel>


</div>

    </ContentTemplate>
    <Triggers>
    <asp:PostBackTrigger ControlID="confirmUpload" />
    <asp:PostBackTrigger ControlID="downloadResult" />
    </Triggers>
    </asp:UpdatePanel>    
    

</asp:Content>

