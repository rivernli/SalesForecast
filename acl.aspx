<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="acl" Title="User Access Control List" Codebehind="acl.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
        <script type="text/javascript">
            function show(obj) {
                var o = document.getElementById(obj);
                if (o.style.display == "none")
                    o.style.display = "block";
                else
                    o.style.display = "none";
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:ScriptManager ID="ScriptManager1" EnablePartialRendering="false" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>


        <div style="cursor:pointer" onclick="show('usage')">使用註解.</div>
        <div style="display:none;" id="usage">
            <div>admin=Yes:什麽都能做.包括這個ACL. 其它權限是不能進ACL</div>
            <div>Sales=Yes: 只能看自己和自己屬下的OEM資料. 除非group是administrator或management.但這條在marketing 不能用</div>
            <div>Report View.能看 Forecast的report. 
                能在marketing裡看東西, 如果不是sales, report viewer 或admin.在marketing是不能看到東西的.</div>
            <div>Price:在marketing裡能不能看到價錢.</div>
            <div>Group: 
                <div>Admin:能管理 config 裡的東西.除了ACL</div>
                <div>management:能看所有東西,改forecast數.包括所有OEM.但不包config</div>
                <div>其它的只是區分用戶的屬性.</div>
            </div>
        </div>


            <asp:Panel ID="searchPanel" runat="server">
                <asp:Label ID="Label2" runat="server" Text="Domain"></asp:Label>
                <asp:DropDownList ID="domainList" runat="server">
                    <asp:ListItem Text="Asia" Value="asia" />
                    <asp:ListItem Text="Europe" Value="europe" />
                    <asp:ListItem Text="Americas" Value="americas" />
                </asp:DropDownList>
             <asp:Label ID="Label1" runat="server" Text="UID/Email:"></asp:Label>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
             <asp:Button ID="Button1"
                runat="server" Text="Search" onclick="Button1_Click" />
                <asp:DropDownList ID="DropDownList1" runat="server">
                </asp:DropDownList>
                <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
                    Text="Group Search" />
                <br />
                <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
            </asp:Panel>
           <asp:Panel ID="Panel1" runat="server" Visible="False">
                             
                <table  class="standardTable" border="1" width="740" bordercolor="#cccccc">
                <tr>
                    <td width="120" bgcolor="#C4E0FF">User Id:</td><td><asp:Label ID="userLabel" runat="server" Text=""></asp:Label>
                    &nbsp;(<asp:Label ID="domainLabel" runat="server" Text=""></asp:Label>)
                    </td> 
                    <td width="120" bgcolor="#C4E0FF">Full Name:</td><td><asp:Label ID="nameLabel" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td width="120" bgcolor="#C4E0FF">Department:</td><td><asp:Label ID="departmentLabel" runat="server" Text=""></asp:Label></td>
                    <td width="120" bgcolor="#C4E0FF">Job title:</td><td><asp:Label ID="titleLabel" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td width="120" bgcolor="#C4E0FF">E-Mail:</td><td><asp:Label ID="emailLabel" runat="server" Text=""></asp:Label></td>
                    <td width="120" bgcolor="#C4E0FF">Mobile:</td><td><asp:Label ID="mobileLabel" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td width="120" bgcolor="#C4E0FF">Tel:</td><td><asp:Label ID="telLabel" runat="server" Text=""></asp:Label></td>
                    <td width="120" bgcolor="#C4E0FF">Fax:</td><td><asp:Label ID="faxLabel" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr valign="top">
                    <td width="120" bgcolor="#C4E0FF">Control:</td><td colspan="3">
                        <div>
                        <asp:CheckBox ID="isAdmin" runat="server" Text="Admin" />
                &nbsp; &nbsp;
                        <asp:CheckBox ID="isActive" runat="server" Text="Active" Checked="true" />
                        <asp:CheckBox ID="isReportViewer" runat="server" Text="Report Viewer" />
                        <asp:CheckBox ID="isSales" runat="server" Text="Sales" />
                        <asp:CheckBox ID="isPriceView" runat="server" Text="Price" />
                        <asp:TextBox ID="ManagerId" runat="server" Text="0" Visible="false" />
                            <asp:DropDownList ID="DropDownList2" runat="server">
                            </asp:DropDownList>
                        </div>
                        </td></tr>
                        <tr><td colspan="4" align="center">
                        <asp:Button ID="addUserBtn" runat="server" Text="User Add" 
                                onclick="addUserBtn_Click" OnClientClick="return confirm('add user?');" />
                            <asp:Button ID="cancelAddUserBtn" runat="server" Text="Cancel" 
                                onclick="cancelAddUserBtn_Click" />
                    </td>
                </tr>    

                </table>
             </asp:Panel>            
    
       <asp:Panel ID="list" runat="server">
           <asp:ListView ID="ListView1" runat="server" 
               onitemcanceling="ListView1_ItemCanceling" onitemcommand="ListView1_ItemCommand" 
               onitemdatabound="ListView1_ItemDataBound" onitemediting="ListView1_ItemEditing" 
               onitemupdating="ListView1_ItemUpdating" OnPagePropertiesChanging="ListView1_PagePropertiesChanging"
               >
               <LayoutTemplate>
                   <div style="background:url(http://bi.multek.com/ws/images/background.gif);">
                       <table border="1" bordercolor="#cccccc" cellpadding="1" cellspacing="0" 
                           class="standardTable" width="98%" >
                           <tr bgcolor="0CAAFF">
                               <td>Name</td>
                               <td>Acc</td>
                               <td>Department/Title</td>
                               <td>tel</td>
                               <td>email</td>
                               <td>Active</td>
                               <td><abbr title="Everything can do and view in GAM/BDM">Admin</abbr></td>
                               <td><abbr title="See for the OEM under their account">Sales</td>
                               <td><abbr title="Report & Price can be view" >Report Viewer</abbr></td>
                               <td><abbr title="Price can be view" >Price View</td>
                               <td>Group</td>
                               <td>
                               </td>
                           </tr>
                           <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                       </table>
                   </div>
               </LayoutTemplate>
               <ItemTemplate>
                   <tr bgcolor='<%# (bool)Eval("isActive")==true?"#ffffff":"#dddddd" %>'
                   onmouseover='this.style.backgroundColor="#D9FFD1"'
                   onmouseout='this.style.backgroundColor=""'>
                       <td><asp:Label ID="uid" runat="server" Text='<%# Eval("uid") %>' Visible="false"></asp:Label>
                           <asp:Label ID="isAd" runat="server" Text='<%# (bool)Eval("isAdmin") == true ? "Y" : "N"%>' Visible="false" />
                           <%# Eval("username") %></td>
                           <td><%# Eval("uid") %>/<%# Eval("domain") %></td>
                       <td><%# Eval("department") %> / <%# Eval("jobTitle") %></td>
                       <td><%# Eval("tel") %></td>
                       <td><%# Eval("emailAddress") %></td>
                       <td><%# (bool)Eval("isActive")==true?"Yes":"No" %></td>
                       <td><%# (bool)Eval("isAdmin")==true?"Yes":"No" %></td>
                       <td><%# (bool)Eval("isSales")==true?"Yes":"No" %></td>
                       <td><%# (bool)Eval("isReportViewer")==true?"Yes":"No" %></td>
                       <td><%# (bool)Eval("isPriceView")==true?"Yes":"No" %></td>
                       <td><%# Eval("uGroup") %></td>
                       <td>
                           <asp:ImageButton ID="editBtn" runat="server" CommandName="Edit" 
                               ImageUrl="http://bi.multek.com/ws/images/edit.png" AlternateText="Modify User" 
                                />
                       </td>
                   </tr>
               </ItemTemplate>
               <EditItemTemplate>
                   <tr>
                       <td>
                           <asp:Label ID="uid" runat="server" Text='<%# Eval("uid") %>' Visible="false" />
                           <asp:Label ID="isAd" runat="server" Text='<%# (bool)Eval("isAdmin") == true ? "Y" : "N"%>' Visible="false" />
                       <%# Eval("username") %></td>
                           <td><%# Eval("uid") %>/<%# Eval("domain") %></td>
                       <td><%# Eval("department") %> / <%# Eval("jobTitle") %></td>
                       <td><%# Eval("tel") %></td>
                       <td><%# Eval("emailaddress") %></td>
                       <td><asp:CheckBox ID="isActive" runat="server" Checked='<%# Eval("isActive") %>' Text="" /></td>
                       <td><asp:CheckBox ID="isAdmin" runat="server" Checked='<%# Eval("isAdmin") %>' Text="" /></td>
                       <td><asp:CheckBox ID="isSales" runat="server" Checked='<%# Eval("isSales") %>' Text="" /></td>
                       <td><asp:CheckBox ID="isReportViewer" runat="server" Checked='<%# Eval("isReportViewer") %>' Text="" /></td>
                       <td><asp:CheckBox ID="isPriceView" runat="server" Checked='<%# Eval("isPriceView") %>' Text="" /></td>
                       <td><asp:DropDownList ID="uGroupList" runat="server" />
                        <asp:Label ID="uGroup" runat="server" Text='<%# Eval("uGroup") %>' Visible="false" />
                       </td>
                       <td>
                        <asp:Label ID="dom" runat="server" Text='<%# Eval("domain") %>' Visible="false" />
                           <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Cancel" 
                               ImageUrl="http://bi.multek.com/ws/images/cancel.png" 
                               />
                           <asp:ImageButton ID="ImageButton2" runat="server" CommandName="Update" 
                               ImageUrl="http://bi.multek.com/ws/images/submit.png" 
                            />
                       </td>
                   </tr>
               </EditItemTemplate>
           </asp:ListView>
       
                                <div>
<asp:DataPager ID="DataPager1" runat="server" PageSize="12" PagedControlID="ListView1">
    <Fields>
        <asp:NumericPagerField ButtonType="Link" />
     </Fields>
</asp:DataPager>
                    </div> 
       
       </asp:Panel>    
    
    
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

