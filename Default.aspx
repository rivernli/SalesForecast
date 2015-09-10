<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="Default" Title="Default View" Codebehind="Default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
    .EDITable { padding:0px; top:0px; left:0px; position:relative; border-color:#cccccc; border-collapse:collapse; border-spacing:1; border-style:solid;}
    .EDITable .amt {background-color:#DFFFD6}
    .EDITable .gap {background-color:#FFEDC4;}
    .EDITable .act {background-color:#FFC9FF;}
    .EDITable tr { white-space:nowrap;}
    .EDITable .header{ white-space:nowrap; background-color:#E0F0FF}
    .EDITable .salesmanTR {background-color:#FFFCED;}
    .EDITable th { width:auto; white-space:nowrap; font-size:11px; padding:2px;}
    
    .EDITable td { width:auto; white-space:nowrap;}
    .EDITable td input_old {text-align:right; width:80px; border:0px; color:#000000; white-space:nowrap;}
    
    .EDITable td input { background-color:Transparent; padding:0px; margin:0px; text-align:right; color:#000000; overflow:visible; font-size:11px; border:0px; width:30px;}
    .EDITable td div{ width:auto; text-align:right; white-space:nowrap; font-size:11px; overflow:visible; padding:0px;}
    </style>
    <script src="http://bi.multek.com/ws/tableFreeze.js" type="text/javascript"></script>
   
    <script type="text/javascript">
    var tbbl;
    var tbblrow;
    var tbblcol;
    var cp=0;
    function loadpage()
    {
        document.getElementById("loading").style.display = "none";
        tbbl = document.getElementById("<%=GridView1.ClientID%>");
        tbblrow = tbbl.rows.length;
        tbblcol = tbbl.rows[tbblrow-1].cells.length;
        tableFreeze("<%=GridView1.ClientID%>","grid",2,1); 
        cp = parseInt(document.getElementById("<%=currentPeriod.ClientID%>").innerHTML);
    }
    </script>
    <script type="text/javascript">
    function messaging(obj,msg){
        var left, top;
        left = (obj.offsetWidth)?obj.offsetWidth+3:3;
        top = 0;
        var div = obj;
        var p = document.getElementById("grid");
        if (obj.offsetParent) {
            do {
                if(obj == p)
                    break;
                left += obj.offsetLeft;
                top  += obj.offsetTop;
            } while (obj = obj.offsetParent);
        }
        var msgDiv = document.getElementById("msgDiv");
        if(msgDiv == undefined || msgDiv == null){
            msgDiv = document.createElement("div");
            msgDiv.id = "msgDiv";
        }
        p.appendChild(msgDiv);
        msgDiv.innerHTML = msg;
        msgDiv.style.position = "relative";
        msgDiv.style.top = top;
        msgDiv.style.left = left;
        msgDiv.style.display = "none";
        msgDiv.style.zIndex = 1;
        if(div.period && div.oemid)
            salesman.getForecastHistory(div.oemid,div.period,showHistory); 
    }
    function showHistory(obj){
            var msgDiv = document.getElementById("msgDiv");
            if(msgDiv && obj != ""){
                msgDiv.innerHTML = obj;
                msgDiv.style.width = msgDiv.firstChild.offsetWidth;
                msgDiv.style.display = "block";
                var grid = document.getElementById("grid");
                var totalHeight = grid.offsetHeight + grid.scrollTop;
                if(grid.offsetWidth < grid.scrollWidth)
                    totalHeight  -= 15;
                var objTop = parseInt(msgDiv.style.top);
                var objHeight = msgDiv.offsetHeight;
                var diffHeight = objHeight + objTop - totalHeight;
                if(diffHeight > 0 )
                    msgDiv.style.top = objTop - diffHeight - 3;

                var totalWidth = grid.offsetWidth + grid.scrollLeft;
                if(grid.offsetHeight < grid.scrollHeight)
                    totalWidth  -= 15;
                var objLeft = parseInt(msgDiv.style.left);
                var objWidth = msgDiv.firstChild.offsetWidth;
                var diffWidth = objWidth + objLeft - totalWidth;
                if(diffWidth >0)
                    msgDiv.style.left = objLeft - objWidth;// - inputField.parentNode.offsetWidth - 3;
            }else{
                msgDiv.style.display = "none";
            }
    }
    
    function inputFocus(DIV){
            messaging(DIV,"history...");
    }
    function pageClear(){
        var x  = document.getElementById("grid");
        for(var i=x.childNodes.length-1; i >=0 ; i--)
        {
            x.removeChild(x.childNodes[i]);
        }
        document.getElementById("loading").style.display = "block";
        return true;        
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:ScriptManager ID="ScriptManager1" EnablePartialRendering="true" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/salesman.asmx" />
        </Services>
    </asp:ScriptManager>

    <asp:UpdatePanel ID="updatePanel1" runat="server">
    <ContentTemplate>
        <asp:Label ID="msg" runat="server" />
        <asp:Label ID="currentPeriod" runat="server" CssClass="hide" />

                        <table class="standardTable" border="1" width="100%" bordercolor="#cccccc">
                        <tr><td width="100" bgcolor="#C4E0FF">Fiscal Year:</td>
                        <td width="*">
                            <div style="width:500px;">
                            Start <asp:DropDownList ID="startFY" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="startPeriod" runat="server"></asp:DropDownList>
                            <asp:Label ID="toLabel" runat="server" Text="To" Visible="true" />
                            End
                            <asp:DropDownList ID="endFY" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="endPeriod" runat="server"></asp:DropDownList>
                            <asp:Button ID="Button1" runat="server" Text="Go" onclick="Button1_Click" OnClientClick="return pageClear()" />
                            </div>
                        </td>
                        </tr>
                        </table>
<div id="loading" style="display:none; text-align:center; padding:20px 0px 20px 0px; margin:0px auto 0px; width:200px;">
<img src="http://bi.multek.com/ws/images/ajax-loader_1.gif" alt="loading" align="middle" />
</div>
                        <div style="visibility:visible; padding:0px; overflow:hidden; display:block; width:100%; height:500px;"  id="grid">
            <asp:GridView ID="GridView1" runat="server" CssClass="EDITable" 
                                ondatabound="GridView1_DataBound"
                                onrowdatabound="GridView1_RowDataBound" ></asp:GridView>
            </div>    
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

