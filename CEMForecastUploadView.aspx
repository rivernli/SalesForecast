<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" 
 ValidateRequest="false" Inherits="CEMForecastUploadView" Title="CEM Forecast Upload" Codebehind="CEMForecastUploadView.aspx.cs" %>

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
<script type="text/javascript">
function closePanel(){
    var pan = document.getElementById("ulist");
    pan.style.display = "none";
    pan.childNodes[1].innerHTML = "";
}
function retHistory(history){
    pan.childNodes[1].innerHTML = history;
}
function openPanel(obj,code){
    pan = document.getElementById("ulist");
    var currentPosition = new getCumulativeOffset(obj);
    pan.childNodes[1].innerHTML = "loading....";
    pan.style.display = "block";
    pan.style.left = (2+ currentPosition.x + currentPosition.w)+ "px";
    pan.style.top = (currentPosition.y-15) + "px";
    PageMethods.getPartNoHistory(obj.innerHTML,code,retHistory);
    var endLeft = pan.clientWidth + currentPosition.x + currentPosition.w;
    var docWidth = document.body.clientWidth;
    if(endLeft > docWidth && docWidth > pan.clientWidth)
    {
            var left = docWidth - (pan.clientWidth + 2);
            if(left < currentPosition.x)
                pan.style.left = (currentPosition.x + 10)+"px";
            else
                pan.style.left = left + "px";
    }
}
</script>

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
<asp:Label ID="tmp" runat="server" />
<asp:Panel ID="uploadArea" runat="server" >
    <div style="padding:2px;">
    Upload CEM Forecast:
    <table width="100%" style="BORDER-COLLAPSE: collapse"  cellSpacing="1" cellPadding="3" width="100%"
    align="center" border=1 bordercolor='#aaaaaa'>
    <tr bgcolor="#dddddd" style="background-image:url(http://bi.multek.com/ws/images/tbackground.png); background-repeat:repeat-x;"><td>Customer Code</td><td>Part Number</td>
    <td><asp:Label ID="Label1" runat="server" /></td>
    <td><asp:Label ID="Label2" runat="server" /></td>
    <td><asp:Label ID="Label3" runat="server" /></td>
    <td><asp:Label ID="Label4" runat="server" /></td>
    <td><asp:Label ID="Label5" runat="server" /></td>
    <td><asp:Label ID="Label6" runat="server" /></td>
    </tr></table>
    <asp:Label ID="periodString" runat="server" Visible="false" />
    <asp:TextBox ID="cem_content" runat="server" Height="300px"  
            TextMode="MultiLine" Width="100%" ></asp:TextBox>
    </div>
    <div style="vertical-align:baseline;">
        <asp:Button ID="Button1" runat="server" Text="Preview" onclick="Button1_Click" 
            OnClientClick="processing_upload();" />
    </div>
</asp:Panel>

<asp:Panel ID="previewArea" runat="server" Visible="False">
<div>Upload Preview</div>

    <asp:ListView id="listMain" runat="server">
        <LayoutTemplate>
                <table id="Table1" style="left:0px; top:0px; background-color:#ffffff; border-color:#888888; border-collapse:collapse; border-spacing:1; border-style:solid;"
                 cellpadding="1" cellspacing="0" border="1" bordercolor="#888888" runat="server" >
                 <tr bgcolor="#688EFF"><td>CS Code/OEM</td>
                 <td>CSPN/CEMPN</td>
                 <td>Plant</td>
                 <td>Ship Date</td>
                 <td>PriceMaster(Avg)</td>
                 <td>ASP</td>
                 <td>Project</td>
                 <td>P1</td><td>P2</td><td>P3</td><td>P4</td><td>P5</td><td>P6</td>
                 </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
                </table>
        </LayoutTemplate>
        <ItemTemplate>
                <tr bgcolor="#FFE48C">
                    <td><%# Eval("code") %></td>
                    <td colspan="6"><%# Eval("pn")%></td>
                    <td><%# Eval("q1") %></td>
                    <td><%# Eval("q2") %></td>
                    <td><%# Eval("q3") %></td>
                    <td><%# Eval("q4") %></td>
                    <td><%# Eval("q5") %></td>
                    <td><%# Eval("q6") %></td>
                </tr>
                <asp:ListView ID="listSub" runat="server" DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("dateRelated") %>'>
                    <LayoutTemplate>
                        <tr id="itemPlaceholder" runat="server"></tr>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr  onmouseover="this.style.backgroundColor='#F8CCFF'"onmouseout="this.style.backgroundColor=''">
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"oem\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"cpn\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"plant\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "[\"lastDate\"]", "{0:yyyy-MM-dd}")%></td>
                        <td>$<%#DataBinder.Eval(Container.DataItem, "[\"priceMaster\"]", "{0:N2}")%>
                        (<%#DataBinder.Eval(Container.DataItem, "[\"guessPrice\"]","{0:N2}")%>)
                        </td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"asp\"]","{0:N2}") %></td>
                        <td><span onclick='<%#DataBinder.Eval(Container.DataItem,"[\"customer_code\"]","openPanel(this,\"{0}\");") %>' style="cursor:pointer"><%#DataBinder.Eval(Container.DataItem,"[\"ipn\"]") %></span></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"q1\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"q2\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"q3\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"q4\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"q5\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"q6\"]") %></td>
                        </tr>
                    </ItemTemplate>
                    <EmptyItemTemplate>
                        <tr ><td colspan="13">No part number matched.</td></tr>
                    </EmptyItemTemplate>
                    <EmptyDataTemplate>
                        <tr bgcolor="#cccccc"><td colspan="13" align="center" style="font-size:8px;">*** No part number matched. ***</td></tr>
                    </EmptyDataTemplate>
                </asp:ListView>
        </ItemTemplate>
    </asp:ListView>                    

<asp:Button ID="downloadUpload" Text="Download Excel" runat="server" 
        onclick="downloadUpload_Click" />
<asp:Button ID="confirmUpload" Text="Confirm" runat="server" OnClientClick="processing_upload()" 
        onclick="confirmUpload_Click" />
      
<asp:Button ID="cancelUpload" Text="Cancel" runat="server" 
        onclick="cancelUpload_Click" />
</asp:Panel>

<div><a href="CEMForecast.aspx">Go Back</a></div>

</div>

    </ContentTemplate>
    <Triggers>
    <asp:PostBackTrigger ControlID="downloadUpload" />
    </Triggers>
    </asp:UpdatePanel>    
    

</asp:Content>

