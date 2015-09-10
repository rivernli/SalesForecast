<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="adjustmentTopSide" Title="Top Side and OLS Adjuest" Codebehind="adjustmentTopSide.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
    .EDITable { padding:0px; top:0px; left:0px; position:relative; border-color:#cccccc; border-collapse:collapse; border-spacing:1; border-style:solid;}
    .EDITable .amt {background-color:#F7F7FF}
    .EDITable .gap {background-color:#FFEDC4;}
    .EDITable .act {background-color:#FFC9FF;}
    .EDITable tr { white-space:nowrap;}
    .EDITable .header{ white-space:nowrap; background-color:#E0F0FF}
    .EDITable .footer{ white-space:nowrap; background-color:#E0F0FF}
    .EDITable .footer td {font-size:12px; font-weight:bold;}
    .EDITable .salesmanTR {background-color:#FFFCED;}
    .EDITable th { width:auto; white-space:nowrap; font-size:11px; padding:2px;}
    
    .EDITable td { width:auto; white-space:nowrap;}
    .EDITable td input { background-color:Transparent; padding:0px; margin:0px; text-align:right;
                         overflow:visible; font-size:10px; border:0px; width:30px;  color:Blue;}
    .EDITable td div{width:auto; display:block; text-align:right; white-space:nowrap; font-size:11px; padding:0px;}
    
    .normal { width:auto; white-space:nowrap;}
    .ols_header {white-space:nowrap; background-color:#B2FFB7; color:#007F46; font-weight:bold; }
    .ols_over {white-space:nowrap; background-color:#C0DBC0;}
    .ols {white-space:nowrap; background-color:#E0FFE0;}
    .topside_header {white-space:nowrap; background-color:#FAFFC6; color:#7F6A00; font-weight:bold;}
    .topside_over {white-space:nowrap; background-color:#D3CDBC;}
    .topside {white-space:nowrap; background-color:#FFF7E2;}
    
    </style>
    <script src="http://bi.multek.com/ws/utility.js" type="text/javascript"></script>
    <script src="http://bi.multek.com/ws/tableFreeze.js" type="text/javascript"></script>
    <script type="text/javascript">
    var tbbl;
    var tbblrow;
    var tbblcol;
    var cp = 0;

    function loadpage() {
        //document.getElementById("loading").style.display = "none";
        tbbl = document.getElementById("<%=GridView1.ClientID%>");
        tbblrow = tbbl.rows.length;
        tbblcol = tbbl.rows[tbblrow - 1].cells.length;
        tableFreeze("<%=GridView1.ClientID%>", "grid", 2, 1);
        cp = parseInt(document.getElementById("<%=currentPeriod.ClientID%>").innerHTML);
        tbbl.style.display = "block";
    }

    </script>

    <script type="text/javascript">
        function modReturn(obj) {
            eval("rs = " + obj);
            var tty = globalTable.rows.length - 1;
            var total = rs.gtt +"";
            globalTable.rows[tty].cells[rs.x].firstChild.innerHTML = total.addCommasToNumber();
        }
        var globalTable = null;
        var last_o = null;
        var last_v;
        function mod(o, type) {
            if (last_o == o)
                return;
            var div = o.firstChild;
            if (div == undefined || div.tagName != "DIV")
                return;
            last_o = o;
            var Mod = { "td": o,
                init: function() {
                    this.tr = this.td.parentNode; this.table = this.tr.parentNode;
                    this.x = this.td.cellIndex; this.y = this.tr.rowIndex;
                    this.oemid = this.tr.cells[0].firstChild.innerHTML;
                    this.period = this.table.rows[1].cells[this.x].firstChild.innerHTML;
                    globalTable = this.table;
                    this.working();
                },
                isNumber: function(n) {
                    n = this.toNumber(n);
                    if (n.trim() != "" && !isNaN(n))
                        return true;
                    else
                        return false;
                },
                toNumber: function(n) {
                    return n = n.ReplaceAll(",", "");
                },
                toNumberFormat: function(n) {
                    return n.addCommasToNumber();
                },
                working: function() {
                    var div = this.td.firstChild; div.style.display = "none";
                    var input = document.createElement("input");
                    input.setAttribute("type", "text");
                    input.value = Mod.toNumber(div.innerHTML);
                    input.style.width = (this.td.clientWidth - 2) + "px";
                    input.onblur = function() {
                        var v = this.value;
                        if (!Mod.isNumber(v)) {
                            this.focus();
                            return;
                        }
                        v = Mod.toNumber(v);
                        var p = this.parentNode; p.removeChild(this);
                        var div = p.firstChild;
                        if (v != Mod.toNumber(div.innerHTML)) {
                            forecastWS.updateOLS_TopSide_adjust(Mod.period, Mod.oemid, v, type, Mod.x, Mod.y, modReturn)
                            //document.getElementById("temp").value = "do ajax sync to x:" + Mod.x + " y:" + Mod.y;
                            div.innerHTML = Mod.toNumberFormat(v);
                            //div.innerHTML = "<img src='http://bi.multek.com/ws/images/loading_tiny.gif' />";
                        }
                        div.style.display = "block";
                        div.style.width = (p.clientWidth - 2) + "px";
                    }
                    input.onkeydown = function() {
                        var keyCode;
                        if (window.event) keyCode = window.event.keyCode;
                        else if (e) keyCode = e.which;
                        if ((keyCode >= 37 && keyCode <= 40) || keyCode == 9) {
                            cellMoving(this, keyCode);
                        }
                    }
                    input.onfocus = function() { this.select(); }
                    this.td.appendChild(input);
                    input.focus();
                }
            };
            Mod.init();
            return;
        }
        function cellMoving(obj, keyCode) {
            var Obj = {
                init: function(obj) {

                    this.cell = obj.parentNode; this.tr = this.cell.parentNode; this.table = this.tr.parentNode;
                    this.x = this.cell.cellIndex; this.y = this.tr.rowIndex;
                    this.css = this.cell.className, this.y2 = this.table.childNodes.length - 1; this.x2 = this.tr.childNodes.length
                },
                chkSelection: function(Right) {
                    var pass = false;
                    if (obj.createTextRange) {
                        var x = document.selection.createRange();
                        var r = x.duplicate();
                        r.moveStart('character', -obj.value.length)
                        var l = 0;
                        if (Right)
                            l = obj.value.length;
                        if (x.text.length == 0 && r.text.length == l)
                            pass = true;
                    }
                    return pass;
                },
                chkCss: function() { if (this.cell == undefined) return false; this.css = this.cell.className; if (this.css == "ols" || this.css == "topside") return true; else return false; },
                moveUp: function() {
                    if (this.y-- > 1) {
                        this.tr = this.table.rows[this.y];
                        if (this.tr.cells.length == 1 && this.y > 1)
                            this.y--; this.tr = this.table.rows[this.y];
                        this.cell = this.tr.cells[this.x];
                        if (Obj.chkCss()) { this.GO(); }
                    }
                },
                moveDown: function() {
                    if (this.y++ < this.y2 - 1) {
                        this.tr = this.table.rows[this.y];
                        if (this.tr.cells.length == 1 && this.y < this.y2 - 1)
                            this.y++; this.tr = this.table.rows[this.y];
                        this.cell = this.tr.cells[this.x];
                        if (Obj.chkCss()) { this.GO(); }
                    }
                },
                moveLeft: function() {
                    if (this.chkSelection(false)) {
                        if (this.x-- > 1) {
                            this.cell = this.tr.cells[this.x];
                            while (!Obj.chkCss() && this.x > 1) {
                                this.x--; 
                                this.cell = this.tr.cells[this.x];
                            }
                            if (Obj.chkCss()) { this.GO(); }
                        }
                    }
                },
                moveRight: function() {
                    if (this.chkSelection(true)) {
                        if (this.x++ < this.x2) {
                            this.cell = this.tr.cells[this.x];
                            while (!Obj.chkCss() && this.x < this.x2) {
                                this.x++;
                                this.cell = this.tr.cells[this.x];
                            }
                            if (this.cell.tagName == "TD" && Obj.chkCss()) { this.GO(); }
                        }
                    }
                },
                GO: function() {
                    obj.blur();
                    mod(this.cell, this.css);
                }
            }
            
            Obj.init(obj);
            switch (keyCode) {
                case 38:
                    Obj.moveUp();
                    break;
                case 40:
                    Obj.moveDown();
                    break;
                case 37:
                    Obj.moveLeft();
                    break;
                case 39:
                case 9:
                    Obj.moveRight();
                    break;
            }
        }
    </script>

  <script type="text/javascript">
      function showOEMComments(oemid, oem) {
          var oMask = document.getElementById("mask")
          if (oMask != undefined && oMask != null) {
              oMask.parentNode.removeChild(oMask);
          }
          var mybody = document.getElementsByTagName("body").item(0);          
          var mask = pg.html.appendObject({ create: "div", param: { id:"mask",cssName:"mask",
            style: {position:"absolute",left:0,top:0,width:"100%",height:"100%",backgroundColor:"#cccccc",filter:"alpha(opacity=95)",zIndex:"9999"}}
            }, mybody);

            var con = pg.html.appendObject({ create: "div", param: { style: { position: "relative", width: "300px", height: "300px", backgroundColor: "#ffffff", border: "1px #0094FF solid", padding:"3px"}} }, mask);
            var span = pg.html.appendObject({ create: "div", param: { style: { textAlign: "right"}} }, con);
            var img = pg.html.appendObject({ create: "img", param: { src: "http://bi.multek.com/ws/images/stopC2.png", style: {cursor:"pointer"}} }, span);
            img.onclick = function() {
                var oMask = document.getElementById("mask");
                if (oMask != undefined && oMask != null) { oMask.parentNode.removeChild(oMask); }
            }
            var title = pg.html.appendObject({ create: "div", param: { innerHTML: oem +" comments:", style: {position:"absolute",textAlign: "left", top: "1px", left: "1px"}} }, span);
            var POS = [mask.clientWidth, mask.clientHeight, con.clientWidth, con.clientHeight]
            var x = (POS[0] - POS[2]) / 2; if (x < 10) x = 10;
            var y = (POS[1] - POS[3]) / 2; if (y < 10) y = 10;
            con.style.top = y + "px";
            con.style.left = x + "px";


            var div = pg.html.appendObject({ create: "div", param: { style: { paddingBottm: "3px"}} }, con);
            var inpRM = pg.html.appendObject({ create: "textarea", param: { id: "inpRM", rows: 4, style: { width: "250px"}} }, div);
            inpRM.onblur = function() {
                forecastWS.setOEMComments(oemid, document.getElementById("inpRM").value, OEMcommentsCallBack);
                this.value = "";
            }
            var div2 = pg.html.appendObject({ create: "div", param: {id:"maskContent"} }, div); 
            forecastWS.getOEMComments(oemid, OEMcommentsCallBack);
            pg.html.appendObject({ create: "div", param: { innerHTML: "<img src='http://bi.multek.com/ws/images/ajax-loader_2.gif' />", style: { textAlign: "center",paddingTop:"6px"}} }, div2);
        }
        function OEMcommentsCallBack(obj) {
            if (obj.trim() == "")
                return;
            var oMask = document.getElementById("maskContent")
            oMask.innerHTML = "";
            var div = pg.html.appendObject({ create: "div", param: { style: { paddingBottm: "3px", "fontSize": "10px"}} }, oMask);
            eval("var ajax = " + obj);
            div.innerHTML = "";
            if (ajax != undefined || ajax != null) {
                for (var i = 0; i < ajax.rs.length; i++) {
                    var sdiv = pg.html.appendObject({ create: "div", param: { style: { borderBottom: "1px #ccc solid" },
                        innerHTML: ajax.rs[i].author + " - " + ajax.rs[i].comment_date
                    }
                    }, div);
                    if (ajax.rs[i].del == "1") {
                        var lb = pg.html.appendObject({ create: "lable", param: { innerHTML: "del", style: { cursor: "pointer", paddingLeft:"4px"}} }, sdiv);
                        lb.onclick = function(id, oid) {
                            return function() { forecastWS.delOEMComments(id, oid, OEMcommentsCallBack); }
                        } (ajax.rs[i].comment_id, ajax.rs[i].oem_id);
                    }
                    pg.html.appendObject({ create: "div", param: { innerHTML: ajax.rs[i].comment} }, sdiv);
                }
            }
        }
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/forecastWS.asmx" />
        </Services>
    </asp:ScriptManager>

    <asp:UpdatePanel ID="updatePanel1" runat="server">
    <ContentTemplate>
    
    
<div id="ulist" class="mainPanel"><div class="sayIcon">&nbsp;</div><div class="subPanel"></div></div>

        <asp:Label ID="temp" runat="server" Visible="false" />
        <asp:Label ID="currentPeriod" runat="server" CssClass="hide" />

                        <table class="standardTable" border="1" width="100%" bordercolor="#cccccc">
                        <tr><td width="100" bgcolor="#C4E0FF">Fiscal Year:</td>
                        <td width="*">
                            <div style="width:auto;">
                            Start:<asp:DropDownList ID="startFY" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="startPeriod" runat="server"></asp:DropDownList>
                            <asp:Label ID="toLabel" runat="server" Text="To" Visible="true" />
                            End:<asp:DropDownList ID="endFY" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="endPeriod" runat="server"></asp:DropDownList>
                            OEM:<asp:TextBox ID="searchOEM" runat="server" />
                            <asp:Button ID="Button1" runat="server" Text="Go" onclick="Button1_Click" />
                            </div>
                        </td>
                        </tr>
                        </table>
                        <div style="visibility:visible; padding:0px; overflow:hidden; display:block; width:100%; height:500px;"  id="grid">
            <asp:GridView ID="GridView1" runat="server" CssClass="EDITable" 
                                ondatabound="GridView1_DataBound"
                                ></asp:GridView>
            </div>    
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="Button1" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

