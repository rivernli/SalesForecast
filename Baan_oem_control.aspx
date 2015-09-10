<%@ Page Language="C#" MasterPageFile="~/gamSetting.master" AutoEventWireup="true" Inherits="Baan_oem_control" Title="Baan OEM list" Codebehind="Baan_oem_control.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Services>
                    <asp:ServiceReference Path="~/services/salesman.asmx" />
                </Services>
            </asp:ScriptManager>
                        <asp:Panel ID="BaanOEM_Panel" runat="server">
            Search: 
                            <asp:TextBox ID="keyBaanOEM" runat="server" Text="" />
            Group: 
                            <asp:DropDownList ID="DropDownList1" runat="server">
                            </asp:DropDownList>
                            <asp:Button ID="searchBaanOEM" runat="server" Text="Search" 
                onclick="searchBaanOEM_Click" />
                            <asp:ListView ID="BaanOEMList" runat="server" 
            onitemediting="BaanOEMList_ItemEditing" 
            onitemcanceling="BaanOEMList_ItemCanceling" 
            onitemupdating="BaanOEMList_ItemUpdating">
                                <LayoutTemplate>
                                    <div>
                                        <table class="standardTable" cellspacing="0" cellpadding="1" border="1" bordercolor="#cccccc" width="100%">
                                            <tr bgcolor="#FFBC7A">
                                                <td>
                                                    Baan OEM</td>
                                                <td>
                                                    Plant</td>
                                                <td>
                                                    Group</td>
                                                <td align="center">
                                                </td>
                                            </tr>
                                            <tr ID="itemPlaceholder" runat="server" />
                                            </table>
                                        </div>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr valign="top" onmouseover="this.style.backgroundColor='#D3FFF7';" onmouseout="this.style.backgroundColor='';">
                                            <td>
                                                <%# Eval("OEMName") %></td>
                                            <td>
                                                <%# Eval("Plant") %></td>
                                            <td>
                                                <%# Eval("GroupName") %></td>
                                            <td align="center">
                                                <asp:ImageButton ID="editBtn" CommandName="Edit" runat="server" 
                    ImageUrl="http://bi.multek.com/ws/images/edit.png" AlternateText="Modify" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <tr valign="top" bgcolor='#A8D9FF'>
                                            <td>
                                                <%# Eval("OEMName") %><asp:Label Visible="false" ID="BaanOEMId" runat="server" Text='<%# Eval("BaanOEMId") %>' />
                                            </td>
                                            <td>
                                                <%# Eval("Plant") %></td>
                                            <td align="right">
                                                <asp:TextBox ID="groupName" runat="server" Text='<%# Bind("GroupName") %>' />
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="ImageButton1" CommandName="Cancel" runat="server" 
                    ImageUrl="http://bi.multek.com/ws/images/cancel.png"  AlternateText="Cancel"/>
                                                <asp:ImageButton ID="ImageButton2" CommandName="Update" runat="server" 
                    ImageUrl="http://bi.multek.com/ws/images/submit.png"  AlternateText="Save"/>
                                            </td>
                                        </tr>
                                    </EditItemTemplate>
                                    <EmptyItemTemplate>
                                    </EmptyItemTemplate>
                                </asp:ListView>
                                <asp:DataPager ID="DataPager1" runat="server" PagedControlID="BaanOEMList" 
                PageSize="20" onprerender="DataPager1_PreRender">
                                    <Fields>
                                        <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" 
                            ShowLastPageButton="True" />
                                    </Fields>
                                </asp:DataPager>
                            </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
</asp:Content>

