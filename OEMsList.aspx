<%@ Page Title="OEM List." Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="OEMsList.aspx.cs" Inherits="SalesForecast.OEMsList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
<script type="text/javascript">

    var getCumulativeOffset = function (obj) {
        var left, top;
        left = top = 0;
        if (obj.offsetParent) {
            do {
                left += obj.offsetLeft;
                top += obj.offsetTop;
            } while (obj = obj.offsetParent);
        }
        return {
            x: left,
            y: top
        };
    };

    var me = null;
    var inprogress = false;
    var serverId = null;
    var dpb = "displayBox";
   
function checkSales(o, client) {
    me = o;
    serverId = client;
    document.getElementById(serverId).value = "0";
    me.style.color = "red";

    hideDisplayBox();


    if (!inprogress) {
        inprogress = true;
        salesman.getUsers(o.value, showValue);
    }
}
function showValue(obj) {
    if (!inprogress)
        return;
    var result = eval(obj);
    var panel = pg.html.showPanel(event, dpb, "auto", "100");
    var xy = new getCumulativeOffset(me);

    panel.style.left = xy.x + "px";
    panel.style.top = (xy.y + 22) + "px";
    panel.style.width = (me.clientWidth - 4) + "px";
    if (result.length == 0)
    { hideDisplayBox(); }
    else
    {
        for (var i = 0 ; i < result.length; i++) {
            if (i > 20)
                break;
            var div = document.createElement("div");
            div.innerHTML = result[i].userName;
            div.did = result[i].sysUserId;
            div.style.cursor = "pointer";
            div.onclick = function () {
                me.value = this.innerHTML;
                document.getElementById(serverId).value = this.did;
                me.style.color = "blue";
                serverId = null;
                hideDisplayBox();
            }
            panel.appendChild(div);
        }
    }
    inprogress = false;
}
function hideDisplayBox() {
    var p = document.getElementById(dpb);
    if (p != null) { p.parentNode.removeChild(p); }
}

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/salesman.asmx" />
        </Services>
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="CustomOEM_Panel" runat="server" >
        <asp:TextBox ID="BaanOEMID" runat="server" Text="0" CssClass="hide"/>
        <asp:TextBox ID="SalesmanID" runat="server" Text="0" CssClass="hide" />
        <asp:TextBox ID="ViewSalesmanID" runat="server" Text="0" CssClass="hide" />
            Search:<asp:TextBox ID="keyword" runat="server" Text="" />
            Salesman:<asp:TextBox ID="salesman_tbx" runat="server" Text="" />
            Status:<asp:DropDownList ID="status" runat="server">
            <asp:ListItem Text="" Value="-1" Selected="True" />
            <asp:ListItem Text="Active" Value="1" />
            <asp:ListItem Text="Inactive" Value="0" />
            </asp:DropDownList>
            <asp:Button ID="searchBaanOEM" runat="server" Text="Search" 
                onclick="searchBaanOEM_Click" />
            <asp:Button ID="downloadOEM" runat="server" Text="Download" 
                onclick="downloadOEM_Click" /> 
                           
            <asp:ListView ID="CusOEMList" runat="server">
            <LayoutTemplate>
                <div>
                <table class="standardTable" cellspacing="0" cellpadding="1" border="1" bordercolor="#cccccc" width="100%">
                <tr bgcolor="#FFBC7A">
                    <td>Cus OEM</td>
                    <td>Baan OEM</td>
                    <td>Plant</td>
                    <td>Group</td>
                    <td>1stSalesman</td>
                    <td>2ndSalesman</td>
                </tr>
                <tr ID="itemPlaceholder" runat="server" />
                </table>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <tr bgcolor='<%# (bool)Eval("isValid")==true?"#ffffff":"#dddddd" %>'
                 onmouseover="this.style.backgroundColor='#D3FFF7';" onmouseout="this.style.backgroundColor='';">
                    <td><%# Eval("CusOEM") %></td>
                    <td><%# Eval("OEMName") %></td>
                    <td><%# Eval("Plant") %></td>
                    <td><%# Eval("GroupName") %></td>
                    <td><%# Eval("userName") %></td>
                    <td><%# Eval("vName") %></td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div>Nothing found!</div>
            </EmptyDataTemplate>
            </asp:ListView>

            <asp:DataPager ID="DataPager1" runat="server" PagedControlID="CusOEMList" 
                    PageSize="20" onprerender="DataPager1_PreRender" >
                <Fields>
                    <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" 
                        ShowLastPageButton="True" />
                </Fields>
            </asp:DataPager>
        </asp:Panel>  

    
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="downloadOEM" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>
