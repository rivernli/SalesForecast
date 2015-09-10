<%@ Page Title="CEM Forecast" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="CEMForecast" Codebehind="CEMForecast.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
    <asp:UpdatePanel runat="server">
    <ContentTemplate>
    <div style="padding:2px;">
    CEM Forecast on <asp:Label ID="currentPeriod" Text="" runat="server"></asp:Label>:
        <div>
        
        <asp:ListView id="main" runat="server" ondatabound="main_DataBound" 
                onitemdatabound="main_ItemDataBound">
         <LayoutTemplate>
           <table id="Table2" style="left:0px; top:0px; background-color:#ffffff; border-color:#888888; border-collapse:collapse; border-spacing:1; border-style:solid;"
                     cellpadding="1" cellspacing="0" border="1" bordercolor="#888888" runat="server" >
                <tr bgcolor="#E1EFF2" style="background-image:url(http://bi.multek.com/ws/images/tbackground.png); background-repeat:repeat-x;">
                     <th>Cust P/N</th>
                     <th>Int. P/N</th>
                     <th>CS Code</th>
                     <th>ASP</th>
                     <th>MasterPrice</th>
                     <th colspan="2"><asp:Label ID="Label1" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label2" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label3" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label4" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label5" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label6" runat="server" Font-Bold="true"></asp:Label></th>
                </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
           </table>
         </LayoutTemplate>
         <ItemTemplate>
            <tr bgcolor="#FFF5D8" onmouseover="this.style.backgroundColor='#FFCD70'" onmouseout="this.style.backgroundColor='#FFF5D8'">
                <th colspan="5" align="left"><%# Eval("oem") %> - <%# Eval("plant") %></th>
                <th colspan="2" align="right"><%# Eval("p1","{0:C}") %></th>
                <th colspan="2" align="right"><%# Eval("p2","{0:C}") %></th>
                <th colspan="2" align="right"><%# Eval("p3","{0:C}") %></th>
                <th colspan="2" align="right"><%# Eval("p4","{0:C}") %></th>
                <th colspan="2" align="right"><%# Eval("p5","{0:C}") %></th>
                <th colspan="2" align="right"><%# Eval("p6","{0:C}") %></th>
            </tr>
                <asp:ListView ID="sub" runat="server" 
                DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("myDataRelation")%>' 
                >
                <LayoutTemplate>
                <tr id="itemPlaceholder" runat="server"></tr>
                </LayoutTemplate>
                <ItemTemplate>
                   <tr onmouseover="this.style.backgroundColor='#F8CCFF'"onmouseout="this.style.backgroundColor=''">
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"cpn\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"[\"ipn\"]") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "[\"customer_code\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"asp\"]","{0:n2}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"PriceMaster\"]", "{0:n2}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"q1\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"p1\"]", "{0:c}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"q2\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"p2\"]", "{0:c}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"q3\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"p3\"]", "{0:c}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"q4\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"p4\"]", "{0:c}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"q5\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"p5\"]", "{0:c}")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"q6\"]")%></td>
                        <td align="right"><%#DataBinder.Eval(Container.DataItem, "[\"p7\"]", "{0:c}")%></td>
                </tr>
                </ItemTemplate>
                </asp:ListView>
                
         </ItemTemplate>
        </asp:ListView>
        
                    <asp:ListView ID="list" runat="server" ondatabound="list_DataBound">
                    <LayoutTemplate>
                    <table id="Table1" style="left:0px; top:0px; background-color:#ffffff; border-color:#888888; border-collapse:collapse; border-spacing:1; border-style:solid;"
                     cellpadding="1" cellspacing="0" border="1" bordercolor="#888888" runat="server" >
                     <tr bgcolor="#688EFF">
                     <th>OEM</th>
                     <th>Plant</th>
                     <th>Customer Code</th>
                     <th>Cust P/N</th>
                     <th>Int. P/N</th>
                     <th>ASP</th>
                     <th>MasterPrice</th>
                     <th colspan="2"><asp:Label ID="Label1" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label2" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label3" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label4" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label5" runat="server" Font-Bold="true"></asp:Label></th>
                     <th colspan="2"><asp:Label ID="Label6" runat="server" Font-Bold="true"></asp:Label></th>
                     </tr>
                    <tr id="itemPlaceholder" runat="server"></tr>
                    </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr  onmouseover="this.style.backgroundColor='#F8CCFF'"onmouseout="this.style.backgroundColor=''">
                        <td><%# Eval("OEM") %></td>
                        <td><%# Eval("plant") %></td>
                        <td><%# Eval("customer_code") %></td>
                        <td><%# Eval("cpn") %></td>
                        <td><%# Eval("ipn") %></td>
                        <td><%# Eval("asp","{0:n2}") %></td>
                        <td><%# Eval("PriceMaster") %></td>
                        <td align="right"><%# Eval("q1") %></td>
                        <td align="right"><%# Eval("p1", "{0:c}")%></td>
                        <td align="right"><%# Eval("q2") %></td>
                        <td align="right"><%# Eval("p2", "{0:c}")%></td>
                        <td align="right"><%# Eval("q3") %></td>
                        <td align="right"><%# Eval("p3", "{0:c}")%></td>
                        <td align="right"><%# Eval("q4") %></td>
                        <td align="right"><%# Eval("p4", "{0:c}")%></td>
                        <td align="right"><%# Eval("q5") %></td>
                        <td align="right"><%# Eval("p5", "{0:c}")%></td>
                        <td align="right"><%# Eval("q6") %></td>
                        <td align="right"><%# Eval("p6", "{0:c}")%></td>
                        </tr>
                    </ItemTemplate>
                    <EmptyItemTemplate>
                        <tr ><td colspan="13">No part number matched.</td></tr>
                    </EmptyItemTemplate>
                    <EmptyDataTemplate>
                        <div>*** No part number matched. ***</div>
                    </EmptyDataTemplate>
                </asp:ListView>
        </div>     
        <asp:HyperLink ID="cem_upload_lnk" runat="server" NavigateUrl="~/CEMForecastUploadView.aspx" Text="Upload CEM Forecast" Visible="false" />
    </div>   
    </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>

