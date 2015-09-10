<%@ Page Language="C#" MasterPageFile="~/gamSetting.master" AutoEventWireup="true" Inherits="salesmanRelation" Title="Salesman staff structure" Codebehind="salesmanRelation.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/salesman.asmx" />
        </Services>
    </asp:ScriptManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
    <asp:Label ID="temp" runat="server" />
        <table><tr valign="top">
        <td><asp:TreeView ID="TreeView1" runat="server" NodeStyle-ImageUrl="images/list_users.gif" 
                onselectednodechanged="TreeView1_SelectedNodeChanged">
            <SelectedNodeStyle BackColor="#CCFFFF" />
            </asp:TreeView></td>
        <td>
            <asp:Panel ID="salesDetal" runat="server" Visible="false">
            <div style="padding:5px; border:1px #ccc solid; width:320px; height:240px; position:relative; top:30px;">
                <div style="font-weight:bold"><asp:Label id="userName" runat="server"  Text="" Font-Bold="true"/>
                <asp:ImageButton ID="ImageButton2" runat="server" 
                                        ImageUrl="images/will_move.png" AlternateText="Apps Move" 
                        onclick="ImageButton2_Click" />
                </div>
                <hr style="size:1px;" />
                    <asp:Label ID="title" runat="server" Text="" /><br />
                    <asp:Label ID="department" runat="server" Text="" />                
                    <asp:Label ID="userId" runat="server" Text="" Visible="false" />
                    <div style="padding-top:6px;">OEM list
                    <asp:GridView ID="oemlist" runat="server" AutoGenerateColumns="False" 
                            CellPadding="4" ForeColor="#333333" GridLines="None" >
                        <RowStyle BackColor="#EFF3FB" />
                        <Columns>
                            <asp:BoundField DataField="CusOEM" HeaderText="Name" />
                            <asp:BoundField DataField="OEMName" HeaderText="BaaN" />
                            <asp:BoundField DataField="plant" HeaderText="Plant" />
                        </Columns>
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <EditRowStyle BackColor="#2461BF" />
                        <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>
            </div>
            </asp:Panel>
        </td>
        </tr></table>    
    </ContentTemplate>
    </asp:UpdatePanel>
    
    
</asp:Content>

