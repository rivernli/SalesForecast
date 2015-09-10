<%@ Page Language="C#" MasterPageFile="~/gamSetting.master" AutoEventWireup="true" Inherits="oem_control" Title="OEM Control" Codebehind="oem_control.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">

<script type="text/javascript">

var getCumulativeOffset = function (obj) {
    var left, top;
    left = top = 0;
    if (obj.offsetParent) {
        do {
            left += obj.offsetLeft;
            top  += obj.offsetTop;
        } while (obj = obj.offsetParent);
    }
    return {
        x : left,
        y : top
    };
};

var me=null;
var inprogress = false;
var serverId=null;
var dpb = "displayBox";
function checkBaanOEM(o){
    me = o;
    document.getElementById("<%=BaanOEMID.ClientID%>").value = "0";
    me.style.color = "red";
    hideDisplayBox();
    if(!inprogress)
    {
        inprogress=true;
        salesman.getBaanOEM(o.value,showBaanOEM);
    }
}
function showBaanOEM(obj)
{
    if(!inprogress)
        return;
    var panel = pg.html.showPanel(null,dpb,"auto","100");
    var xy = new getCumulativeOffset(me);
    panel.style.left = xy.x + "px";
    panel.style.top = (xy.y + 22) + "px";
    panel.style.width = (me.clientWidth - 4) + "px";
    var result = eval(obj);
    if(result.length == 0)
    {hideDisplayBox();}
    else{
        for(var i=0 ; i < result.length; i++)
        {
            var div = document.createElement("div");
            div.innerHTML = result[i].OEMName + " ("+ result[i].Plant+")";
            div.did = result[i].BaanOEMID;
            div.style.cursor = "pointer";
            div.onclick = function()
            {
                me.value = this.innerText;
                document.getElementById("<%=BaanOEMID.ClientID%>").value = this.did;
                me.style.color = "blue";
                hideDisplayBox();
            }
            panel.appendChild(div);
        }
    }
    inprogress=false;
}
function checkSales(o, client){
    me = o;
    serverId = client;
    document.getElementById(serverId).value = "0";
    me.style.color = "red";

    hideDisplayBox();


    if (!inprogress)
    {
        inprogress=true;
        salesman.getUsers(o.value,showValue);
    }
}
function showValue(obj)
{
    if(!inprogress)
        return;
    var result = eval(obj); 
    var panel = pg.html.showPanel(event,dpb,"auto","100");
    var xy = new getCumulativeOffset(me);

    panel.style.left = xy.x + "px";
    panel.style.top = (xy.y + 22) + "px";
    panel.style.width = (me.clientWidth - 4) + "px";
    if(result.length ==0)
    {hideDisplayBox();}
    else
    {
        for(var i=0 ; i < result.length; i++)
        {
            var div = document.createElement("div");
            div.innerHTML = result[i].userName;
            div.did = result[i].sysUserId;
            div.style.cursor = "pointer";
            div.onclick = function()
            {
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
function hideDisplayBox()
{
    var p = document.getElementById(dpb);
    if(p !=null){p.parentNode.removeChild(p);}
}

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
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
                           
            <asp:ListView ID="CusOEMList" runat="server" 
                onitemcanceling="CusOEMList_ItemCanceling" 
                onitemediting="CusOEMList_ItemEditing" 
                onitemupdating="CusOEMList_ItemUpdating" 
                oniteminserting="CusOEMList_ItemInserting" 
                onitemcommand="CusOEMList_ItemCommand">
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
                    <td align="center">
                    <asp:LinkButton runat="server" id="add_new" onclick="add_new_Click" >
                        <img src="http://bi.multek.com/ws/images/add_new.png" alt="New" border="0" />
                    </asp:LinkButton>
                    </td>
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
                    <td align="center">
                    <asp:LinkButton ID="editBtn" CommandName="Edit" runat="server" >
                        <img src="http://bi.multek.com/ws/images/edit.png" alt="Edit" style="border:0" border="0" />
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <InsertItemTemplate>
                <tr>
                    <td>
                    <asp:TextBox ID="cusOEM" runat="server" />
                    <asp:CheckBox ID="isValid" runat="server" Checked="true" />
                    </td>
                    <td><asp:TextBox ID="baanOEM" runat="server" /></td>
                    <td></td>
                    <td></td>
                    <td>
                    <asp:TextBox ID="salesman" runat="server" />
                    </td>
                    <td><asp:TextBox ID="vsalesman" runat="server" /></td>
                    <td align="center">
                        <asp:LinkButton ID="ImageButton1" CommandName="Cancel" runat="server">
                            <img src="http://bi.multek.com/ws/images/cancel.png" alt="Cancel" border="0" />
                        </asp:LinkButton>
                        <asp:LinkButton ID="ImageButton2" CommandName="Insert" runat="server">
                            <img src="http://bi.multek.com/ws/images/submit.png" alt="Save" border="0" />
                        </asp:LinkButton>
                    </td>
                </tr>            
            </InsertItemTemplate>
            <EditItemTemplate>
                <tr>
                    <td>
                    <asp:TextBox ID="cusOEM" runat="server" Text='<%# Eval("CusOEM") %>' />
                    <asp:CheckBox ID="isValid" runat="server" Checked='<%# Eval("isValid") %>' />
                    </td>
                    <td><asp:TextBox ID="baanOEM" runat="server" Text='<%# string.Format( "{0} ({1})",Eval("OEMName"),Eval("Plant")) %>' />
                    </td>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:TextBox ID="salesman" runat="server" Text='<%# Eval("userName") %>' />
                        <asp:Label ID="id_baanOEM" runat="server" Text='<%# Eval("BaanID") %>' Visible="false" />
                        <asp:Label ID="id_salesman" runat="server" Text='<%# Eval("SalesmanId") %>' Visible="false" />
                        <asp:Label ID="id_oem" runat="server" Text='<%# Eval("OEMID") %>' Visible="false"/><br />
                    <asp:TextBox ID="bksalesman" runat="server" /><asp:TextBox ID="bksalesmanId" runat="server" CssClass="hide" />
                    <asp:LinkButton ID="bkAdd" runat="server" CommandName="addBKSales" >
                        <img src="images/c.gif" Alt="Add Backup sales" border="0" />
                    </asp:LinkButton>
                    <asp:ListView ID="bkList" runat="server" OnItemCommand="bkList_ItemCommand">
                    <LayoutTemplate><div>backup salesman:</div><span ID="itemPlaceholder" runat="server" /></LayoutTemplate>
                    <ItemTemplate>
                        <span style="padding-right:4px;"><%# Eval("userName") %>
                        <asp:Label ID="id" runat="server" Visible="false" Text='<%# Eval("id") %>' />
                        <asp:LinkButton ID="delBK" runat="server" CommandName="del" >
                            <img src="images/o.gif" Alt="Del Backup Sales" border="0" />
                        </asp:LinkButton> ,</span>
                    </ItemTemplate>
                    </asp:ListView>
                    
                    </td>
                    <td>
                        <asp:TextBox ID="vsalesman" runat="server" Text='<%# Eval("vName") %>' />
                        <asp:Label ID="id_vsalesman" runat="server" Text='<%# Eval("ViewSalesmanId") %>' Visible="false" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="ImageButton1" CommandName="Cancel" runat="server" >
                            <img src="images/cancel.png" alt="Cancel" border="0" />
                        </asp:LinkButton>
                        <asp:LinkButton ID="ImageButton2" CommandName="Update" runat="server" >
                            <img src="images/submit.png" alt="Save" border="0" />
                        </asp:LinkButton>
                    </td>
                </tr> 
            </EditItemTemplate>
            <EmptyDataTemplate>
                    <asp:LinkButton runat="server" id="add_new" onclick="add_new_Click" >
                        <img src="http://bi.multek.com/ws/images/add_new.png" alt="New" border="0" />
                    </asp:LinkButton>

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

