<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="adjustment" Title="Forecast Adjustment" Codebehind="adjustment.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .EDITable{padding:0px; top:0px; left:0px; position:relative; border-color:#cccccc; border-collapse:collapse; border-spacing:1; border-style:solid;}
    .EDITable tr { white-space:nowrap;}
    .EDITable .header{ white-space:nowrap; background-color:#E0F0FF}
    .EDITable td {white-space:nowrap;padding:2px 3px }
    
    .keyParent { background-color:#F4E2FF}
    .keyChild {}
    .keyInput {cursor:pointer;}
</style>
<script type="text/javascript" id="format">
function onload(){
    if(window.location.hash != "" && window.location.hash!="#"){
        var tbl = document.getElementById("<%=salesmanList.ClientID%>_Table1");
        var sales = decodeURI(window.location.hash.substr(1,100)).trim();
        //alert(sales);
        for(var i =0 ; i < tbl.rows.length; i ++){
            if(tbl.rows[i].cells.length == 1 && tbl.rows[i].cells[0].firstChild.innerHTML == sales)
                collapseOEM(tbl.rows[i].cells[0].firstChild);
        }
        window.location.hash = "";
    }
}
function adjust(td){
    var aj = td.innerHTML;
    td.innerHTML = "";
    var inp = pg.html.appendObject({create:"input",param:{type:"text",value:aj,style:{width:"96px",border:"1px #ccc solid",textAlign:"right",fontSize:"11px"}}},td);
    inp.focus();
    td.onclick = null;
    inp.onblur = outAdjust
}
function outAdjust(){
    var td = this.parentNode;
    var val =this.value;
    td.innerHTML = val;    
    td.onclick = function(){adjust(this);}
}
function collapseOEM(obj){

        var tr = obj.parentNode.parentNode;
        var n = tr.rowIndex+1;
        var tbl = tr.parentNode;
        var tblLen = tbl.rows.length;
        var visiabl = tbl.rows[n].style.display=="none"?"":"none";
        var g=true;
        while(g){
            tbl.rows[n].style.display = visiabl;
            if(n+1 >= tblLen){
                g = false;
            }else if(tbl.rows[n+1].className != "keyChild")//cells.length != tbl.rows[n].cells.length)
            {
                g=false;
            }
            n++;
        }
    //}
}
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/salesman.asmx" />
        </Services>
    </asp:ScriptManager>

    <asp:UpdatePanel ID="updatePanel1" runat="server">
    <ContentTemplate>
        <asp:DropDownList ID="listedBy" runat="server" AutoPostBack="true" 
            onselectedindexchanged="listedBy_SelectedIndexChanged" >
            <asp:ListItem Value="1" Text="Salesman" />
            <asp:ListItem Value="0"  Text="plant" />
        </asp:DropDownList>
        
        <asp:ListView ID="salesmanList" runat="server">
        <LayoutTemplate>
            <table id="Table1" class="EDITable" cellpadding="1" cellspacing="0" border="1" bordercolor="#cccccc" runat="server" >
                <tr bgcolor="#E1EFF2">
                    <th rowspan="2" width="180px"><asp:Label ID="salesman" Font-Bold="true" Text="Sales/OEM" runat="server" /></th>
                    <th colspan="2"><asp:Label ID="period_1" runat="server" Text="next period" /></th>
                    <th colspan="2"><asp:Label ID="period_2" runat="server" Text="next+1 period" /></th>
                    <th colspan="2"><asp:Label ID="period_3" runat="server" Text="next+2 period"/></th>
                </tr>
                <tr bgcolor="#E1EFF2">
                    <th width="100px">Forecast</th><th width="100px">Adjust</th>
                    <th width="100px">Forecast</th><th width="100px">Adjust</th>
                    <th width="100px">Forecast</th><th width="100px">Adjust</th>
                </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr class="keyParent" onmouseover="this.style.backgroundColor='#F8CCFF'" onmouseout="this.style.backgroundColor='#F4E2FF'">
                 <td colspan="7"><span style="font-size:14px;cursor:pointer" onclick="collapseOEM(this)"><%# Eval("salesman")%></span></td>
            </tr>
            <asp:ListView ID="oemList" runat="server"  DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("SalesOEM") %>'>
            <LayoutTemplate>
                <tr id="itemPlaceholder" runat="server"></tr>
            </LayoutTemplate>
            <ItemTemplate>
            <tr class="keyChild" style="display:none" onmouseover="this.style.backgroundColor='#FFEEC9'" onmouseout="this.style.backgroundColor=''">
                <td><span id="oemid" style="display:none"><%#DataBinder.Eval(Container.DataItem,"[\"oemid\"]") %></span><%#DataBinder.Eval(Container.DataItem,"[\"cusOEM\"]") %></td>
                <td align="right"><%#string.Format("{0:N0}",DataBinder.Eval(Container.DataItem,"[3]")) %></td>
                <td align="right" onclick="adjust(this)" class="keyInput"><%#string.Format("{0:N0}",DataBinder.Eval(Container.DataItem,"[4]")) %></td>
                <td align="right"><%#string.Format("{0:N0}",DataBinder.Eval(Container.DataItem,"[5]")) %></td>
                <td align="right" onclick="adjust(this)" class="keyInput"><%#string.Format("{0:N0}",DataBinder.Eval(Container.DataItem,"[6]")) %></td>
                <td align="right"><%#string.Format("{0:N0}",DataBinder.Eval(Container.DataItem,"[7]")) %></td>
                <td align="right" onclick="adjust(this)" class="keyInput"><%#string.Format("{0:N0}", DataBinder.Eval(Container.DataItem, "[8]"))%></td>
            </tr>
            </ItemTemplate>
            </asp:ListView>
        </ItemTemplate>
        </asp:ListView>
    </ContentTemplate>
    </asp:UpdatePanel>
   
</asp:Content>

