<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="SalesmenForecast6" Title="Salesman Forecast" Codebehind="SalesmenForecast6.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
    .EDITable{padding:0px; top:0px; left:0px; position:relative; border-color:#cccccc; border-collapse:collapse; border-spacing:1; border-style:solid;}
    .EDITable tr { white-space:nowrap;}
    .EDITable .header{ white-space:nowrap; background-color:#E0F0FF}
    .EDITable th { width:auto; white-space:nowrap; font-size:11px; padding:2px;}
    .EDITable td span { text-align:right; white-space:nowrap; width:100%;}
    .EDITable td { width:auto; white-space:nowrap; text-align:right; padding:2px 3px }
    .EDITable td .newPart { text-align:left; width:150px; overflow:hidden;padding:1px; margin:0px; color:#000000; font-size:12px; background-color:Transparent;border:1px groove #444;}/*border:0px; border-bottom:1px solid #444;*/
    .EDITable td .revenue { text-align:right; overflow:visible; width:80px;padding:1px; margin:0px; color:#000000; font-size:12px; border:0px;background-color:Transparent;}
    .EDITable td div{ width:auto; text-align:right; white-space:nowrap; font-size:11px; overflow:visible; padding:0px;}
    .DynDiv {position:absolute; background-color:#ffffff; z-index:1; width:auto; display:block;
             -moz-box-shado: 3px 3px 4px #ccc; -webkit-box-shadow: 3px 3px 4px #ccc; box-shadow: 3px 3px 4px #ccc;
            /* For IE 8 */
            -ms-filter: "progid:DXImageTransform.Microsoft.Shadow(Strength=8, Direction=135, Color='#cccccc')";
            /* For IE 5.5 - 7 */
            filter: progid:DXImageTransform.Microsoft.Shadow(Strength=8, Direction=135, Color='#cccccc');
    }
    .pj_detail {cursor:pointer; border:0px;}
    .mainPanel { width:200px; position:absolute; top:0px; left:0px; display:none; z-index:1999; width:338px;}
    .mainPanel .subPanel{ position:relative; top:0px; left:16px; padding:5px; width:260px; height:auto; background-color:#FFFCCC; border:1px #FF6A00 solid;}
    .subPanel span {padding-left:4px; padding-right:2px; background-color:#ffffff; position:relative;}
    .mainPanel .sayIcon {position:relative; top:22px; left:0px; background-image:url("http://bi.multek.com/ws/images/sayConner2.png?B"); background-repeat:no-repeat; background-position:left top; width:20px; height:12px; z-index:50}

    .pastPeriodParts {font-size:10px; color:#444444; background-color:#FFE2EE}
    .pastPeriod {font-size:11px; color:#444444; background-color:#FFE2EE}
    .loadPeriod {font-size:11px; color:#444444; background-color:#F7F7FF}
    .Btitle{ text-align:center; font-size:11px; color:#000000; background-color:#E2F7FF; font-style:italic;}
    </style>
    <script type="text/jscript" id="salesman_filter">
        var Sales = {
            dpb: "displayBox",
            selection_pos: -1,
            textObj: null,
            idObj: null,
            wip: false,
            checkSales: function(asp_object, asp_label_id) {
                this.textObj = asp_object;
                this.idObj = document.getElementById(asp_label_id);
                this.textObj.style.color = "red";
                if (!this.wip) {
                    var kc;
                    if (window.event) kc = window.event.keyCode;
                    else if (e) kc = e.which;
                    if (kc == 27) { this.textObj.value = ""; hideDisplayBox(); return; }
                    if (kc == 38 || kc == 40) {
                        var div = document.getElementById(this.dpb);
                        if (div != undefined && div.childNodes.length > 0) {
                            var max = div.childNodes.length - 1;
                            (kc == 38) ? this.selection_pos-- : this.selection_pos++;
                            if (this.selection_pos > max) this.selection_pos = 0;
                            if (this.selection_pos < 0) this.selection_pos = max;
                            for (var i = 0; i <= max; i++) {div.childNodes[i].style.backgroundColor = "";}
                            var obk = div.childNodes[this.selection_pos];
                            obk.style.backgroundColor = "#FFFCCC";
                            this.textObj.value = obk.innerHTML;
                            this.idObj.value = obk.did;
                            this.textObj.style.color = "blue";
                        }
                    } else {
                        hideDisplayBox();
                        this.wip = true;
                        this.selection_pos = -1;
                        salesman.getUsers(this.textObj.value, Sales.showValue);
                    }
                }
            },
            showValue: function(obj) {
                if (!Sales.wip)
                    return;
                var rs = eval(obj);
                var panel = pg.html.showPanel(event, Sales.dpb, "auto", "100");
                var xy = new getCumulativeOffset(Sales.textObj);
                pg.html.changeObject({ style: { left: (xy.x) + "px", top: (xy.y + 22) + "px", width: Sales.textObj.clientWidth - 4} }, panel);
                if (rs.length == 0) { hideDisplayBox(); }
                else {
                    for (var i = 0; i < rs.length; i++) {
                        var d = pg.html.appendObject({ create: "div", param: { innerHTML: rs[i].userName, did: rs[i].sysUserId,
                            style: { cursor: "pointer" }
                        }
                        }, panel);
                        d.onclick = function() {
                            Sales.textObj.value = this.innerHTML;
                            Sales.idObj.value = this.did;
                            Sales.textObj.style.color = "blue";
                            Sales.selection_pos = -1;
                            hideDisplayBox();
                        }
                    }
                }
                Sales.wip = false;
            }
        };


        function hideDisplayBox() {
            pg.html.hideDivContent(Sales.dpb);
            pg.html.hideDivContent(udfPN.subPanel);
            OLS.hide();
            pn.close();
        }
        </script>
    <script type="text/javascript" id="forecast_cell_object_moving_focus_blur_updae">
        function cellMoving(obj, keyCode) {
            var Obj = {
                init: function(obj) {
                    this.cell = obj.parentNode; this.tr = this.cell.parentNode; this.table = this.tr.parentNode;
                    this.x = this.cell.cellIndex; this.y = this.tr.rowIndex;
                    this.y2 = this.table.childNodes.length - 1; this.x2 = this.tr.childNodes.length
                },
                chkSelection: function(Right) {
                    var pass = false;
                    if (obj.createTextRange) {
                        var x = document.selection.createRange();
                        var r = x.duplicate();
                        r.moveStart('character', -obj.value.length)
                        var l = 0;
                        if (Right) l = obj.value.length;
                        if (x.text.length == 0 && r.text.length == l) pass = true;
                    }
                    return pass;
                },
                moveUp: function() {
                    if (this.y-- > 1) {
                        this.tr = this.table.rows[this.y];
                        this.cell = this.tr.cells[this.x];
                        if (Obj.checking()) { this.GO(); }
                    }
                },
                moveDown: function() {
                    if (this.y++ < this.y2 - 1) {
                        this.tr = this.table.rows[this.y];
                        this.cell = this.tr.cells[this.x];
                        if (Obj.checking()) { this.GO(); }
                    }
                },
                moveLeft: function() {
                    if (this.chkSelection(false)) {
                        if (this.x-- > 1) {
                            this.cell = this.tr.cells[this.x];
                            if (Obj.checking()) { this.GO(); }
                        }
                    }
                },
                moveRight: function() {
                    if (this.chkSelection(true)) {
                        this.x++;
                        if (this.x < this.x2) {
                            this.cell = this.tr.cells[this.x];
                            if (this.cell.tagName == "TD" && Obj.checking()) { this.GO(); }
                        }
                    }
                },
                checking: function() {
                    //checking the new cell is right. and check child object too..
                    if (this.cell == undefined) return false;
                    var tgObj = this.cell.firstChild;
                    if (tgObj != undefined && !tgObj.disabled && tgObj.tagName != undefined) {
                        return true;
                    }
                    return false;
                },
                GO: function() {
                    var tgObj = this.cell.firstChild;
                    if (tgObj.tagName == "INPUT") {
                        obj.blur();
                        tgObj.focus();
                    }
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
        function keyMove(obj) {

            var keyCode;
            if (window.event) keyCode = window.event.keyCode;
            else if (e) keyCode = e.which;
            if ((keyCode >= 37 && keyCode <= 40) || keyCode == 9) {
                cellMoving(obj, keyCode);
            }
        }

        var fcInp = {
            globalImg: "http://bi.multek.com/ws/images/",
            panelId: "fcDetail",
            object: null,
            table: null,
            partNo: "", period: "", oriValue: "",
            inputFocus: function(obj) {
                obj.select();
                obj.style.border = '1px #555 solid';
                this.object = obj; this.oriValue = obj.value;
                var tr = obj.parentNode.parentNode; this.table = tr.parentNode;
                this.period = this.table.rows[0].cells[obj.parentNode.cellIndex + 1].innerText; //first td celspan=2 so +1
                this.partNo = tr.cells[0].firstChild.firstChild.innerHTML;
                this.showDetail();
                forecastWS.getCusPartRemark(obj.getAttribute("oemid"), this.partNo, fcInp.callBackMoreDetail);
            },
            showDetail: function() {
                var pos = new getCumulativeOffset(this.object);
                pg.html.showDivContent(this.panelId);
                var pan = document.getElementById(this.panelId);
                document.getElementById("main").appendChild(pan);
                pg.html.changeObject({ style: { border: "2px #fff solid", width: "300px", whiteSpace: "normal", paddingTop: "14px",
                    left: (pos.x - 220) + "px", top: (pos.y + 17) + "px",
                    background: "#FFB27F url('" + this.globalImg + "arrowCon.png') no-repeat right top"
                }, innerHTML: ""
                }, pan);
                pan.innerHTML = "Period: " + this.period + "<br/>P/N: " + this.partNo;
            },
            callBackMoreDetail: function(obj) {
                eval("var o = " + obj);
                if (fcInp.object.getAttribute("oemid") != o.oemid || fcInp.partNo != o.pn)
                    return;
                var pan = document.getElementById(fcInp.panelId);
                pan.innerHTML += "<br/>ASP: " + (o.ASP ? o.ASP : "--") + "<br/>Remark: " + (o.remark ? o.remark : "--");
            },
            inputBlur: function(obj, period) {
                pg.html.hideDivContent(fcInp.panelId);
                if (obj != fcInp.object)
                    return;

                obj.style.border = "0px";
                obj.value = obj.value.ReplaceAll(",", "");
                if (fcInp.oriValue.ReplaceAll(",", "") != obj.value) {
                    obj.style.color = "red";
                    if (isNaN(obj.value) || obj.value.trim() == "")
                        obj.value = fcInp.oriValue;
                    else
                        forecastWS.getCusPartForecastUpdate(period, obj.getAttribute("oemid"), fcInp.partNo, obj.value, obj.id, false, fcInp.callBackUpdated);
                } else {
                    obj.value = fcInp.oriValue;
                }
            },
            callBackUpdated: function(obj) {
                var rs3 = eval(obj);
                if (rs3.error) {
                    alert(rs3.message);
                    return;
                }
                var cell = document.getElementById(rs3.cell);
                if (cell != undefined) {
                    cell.value = rs3.amt.addCommasToNumber();
                    cell.style.color = "blue";
                }
            }
        };
    </script>
    <script type="text/javascript" id="user_define_project">

        var udfPN = {
            globalImg: "http://bi.multek.com/ws/images/",
            subPanel:"__sub__panel",
            object: null, submitObject: null, panel: null,
            layer: null, tech: null, surf: null,
            _setCloseObject: function() {
                var cls = pg.html.appendObject({ create: "div", param: { style: { textAlign: "right", backgroundColor: "#C4E8FF", color: "#ffffff", padding: "2px 5px"}} }, this.panel);
                var img = pg.html.appendObject({ create: "img", param: { src: this.globalImg + "stopC2.png", style: { textAlign: "right", cursor: "pointer"}} }, cls);
                img.onmouseover = function() { this.src = udfPN.globalImg + "stopC1.png" }
                img.onmouseout = function() { this.src = udfPN.globalImg + "stopC2.png" }
                img.onclick = function() { pg.html.hideDivContent(udfPN.panel.id); }
            },
            _presetTechObject: function(parent) {
                return pg.html.appendObject({ create: "div", param: { style: { width: "auto", cursor: "default"}} }, parent);
            },
            _layerBox: function() {
                var layerBox = this._presetTechObject(this.panel);

                this.layer = pg.html.appendObject({ create: "span", param: { innerHTML: "Layer", id: "defineLayerDiv", style: { width: "auto", cursor: "pointer"}} }, layerBox);
                this.layer.onclick = function() { layer2.style.display = (layer2.style.display == "block") ? "none" : "block"; }

                var layer2 = pg.html.appendObject({ create: "div", param: { style: { position: "relative", display: "none", left: "10px", width: "auto", backgroundColor: "#C1D7FF"}} }, layerBox);
                var layerArray = ["2-8", "10-16", "18-24", "26+"];
                for (var i = 0; i < layerArray.length; i++)
                    pg.html.appendObject({ create: "div", param: { innerHTML: layerArray[i], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer"}} }, layer2);
            },
            _techBox: function() {
                var techBox = this._presetTechObject(this.panel);

                this.tech = pg.html.appendObject({ create: "span", param: { innerHTML: "Technology", id: "defineTechnologyDiv", style: { width: "auto", cursor: "pointer"}} }, techBox);
                this.tech.onclick = function() { tech2.style.display = (tech2.style.display == "block") ? "none" : "block"; }

                var tech2 = pg.html.appendObject({ create: "div", param: { style: { position: "relative", display: "none", left: "10px", width: "auto", backgroundColor: "#C1D7FF"}} }, techBox);
                var techArray = ["STD", "BVH", "HDI 1+N+1", "HDI 2+N+2", "HDI 4+N+4", "ELIC", "Embedded Coin", "Attached Coin"];
                for (var j = 0; j < techArray.length; j++)
                    pg.html.appendObject({ create: "div", param: { innerHTML: techArray[j], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer"}} }, tech2);
            },
            _surfBox: function() {
                var surfacefinishBox = this._presetTechObject(this.panel);

                this.surf = pg.html.appendObject({ create: "span", param: { innerHTML: "SurfaceFinish", id: "defineSurfaceFinishDiv", style: { width: "auto", cursor: "pointer"}} }, surfacefinishBox);
                this.surf.onclick = function() { surfacefinish2.style.display = (surfacefinish2.style.display == "block") ? "none" : "block"; }

                var surfacefinish2 = pg.html.appendObject({ create: "div", param: { style: { position: "relative", display: "none", left: "10px", width: "auto", backgroundColor: "#C1D7FF"}} }, surfacefinishBox);
                var surfArray = ["Au Plating", "Gold Finger", "Immersion Silver", "Immersion Tin", "ENIG", "OSP", "Not specified"]
                for (var k = 0; k < surfArray.length; k++)
                    pg.html.appendObject({ create: "div", param: { innerHTML: surfArray[k], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer"}} }, surfacefinish2);
            },
            _setUserDefinePart: function() {
                udfPN.submitObject.style.display = "none";

                if (udfPN.layer.innerText.indexOf("BOM") == 0) {
                    var lay = udfPN.layer.innerText.substr(3).trim();
                    var tec = udfPN.tech.innerText.substring(3).trim();
                    var sff = "SMT";
                } else if (udfPN.layer.innerHTML.indexOf("Technology") == 0) {
                    var lay = udfPN.layer.innerText.substr(10).trim();
                    var tec = udfPN.tech.innerText.substring(8).trim();
                    var sff = udfPN.surf.innerText.substring(6).trim();
                }
                else {
                    var lay = udfPN.layer.innerText.substr(5).trim();
                    var tec = udfPN.tech.innerText.substring(10).trim();
                    var sff = udfPN.surf.innerText.substring(13).trim();
                }
                //object: null, submitObject: null, panel: null,
                udfPN.object.value = "";
                if (lay + tec + sff != "") {
                    udfPN.object.value = "[" + (lay != "" ? lay : "-") + "][" + (tec != "" ? tec : "-") + "][" + (sff != "" ? sff : "-") + "]";
                    udfPN.object.style.width = "auto";
                    if (lay != "" && tec != "" && sff != "") {
                        udfPN.submitObject.style.display = "block";
                    }
                }
            },
            _selectedOne: function() {
                var p = this.parentNode;
                for (var i = 0; i < p.childNodes.length; i++) { p.childNodes[i].style.color = ""; }
                this.style.color = (this.style.color == "red") ? "" : "red";
                p.style.display = "none";
                var title = p.parentNode.firstChild;
                var title_str = title.innerHTML.split(" ");
                title.innerHTML = title_str[0] + " <b style='color:red'>" + this.innerHTML + "</b>";
                udfPN._setUserDefinePart();
            },
            callNewPN: function(obj, btn_submit_id,oemType) {
                hideDisplayBox();
                obj.value = "";
                this.object = obj;
                this.submitObject = document.getElementById(btn_submit_id);
                this.submitObject.style.display = "none";
                this.panel = pg.html.showPanel(event, this.subPanel, "auto", 200);
                pg.html.changeObject({ style: { zIndex: "100" }, innerHTML: "" }, this.panel);
                this._setCloseObject();
                if (oemType == "normal") {
                    this._layerBox();
                    this._techBox();
                    this._surfBox();
                }
                else
                {
                    var layerBox = this._presetTechObject(this.panel);
                    layerBox.style.width = "140px";
                    this.layer = pg.html.appendObject({ create: "span", param: { innerHTML: "BOM", id: "defineLayerDiv", style: { width: "auto", cursor: "pointer" } } }, layerBox);
                    this.layer.onclick = function () { layer2.style.display = (layer2.style.display == "block") ? "none" : "block"; }
                    var layer2 = pg.html.appendObject({ create: "div", param: { style: { position: "relative", display: "none", left: "10px", width: "130px", backgroundColor: "#C1D7FF" } } }, layerBox);

                    var techBox = this._presetTechObject(this.panel);
                    this.tech = pg.html.appendObject({ create: "span", param: { innerHTML: "SMT", id: "defineTechnologyDiv", style: { width: "auto", cursor: "pointer" } } }, techBox);
                    this.tech.onclick = function () { tech2.style.display = (tech2.style.display == "block") ? "none" : "block"; }
                    var tech2 = pg.html.appendObject({ create: "div", param: { style: { position: "relative", display: "none", left: "10px", width: "130px", backgroundColor: "#C1D7FF" } } }, techBox);

                    if (oemType == "smt") {
                        var layerArray = ["Purchased", "Provided"];
                        for (var i = 0; i < layerArray.length; i++)
                            pg.html.appendObject({ create: "div", param: { innerHTML: layerArray[i], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer" } } }, layer2);
                        var techArray = ["Single", "Double", "Custom"];
                        for (var j = 0; j < techArray.length; j++)
                            pg.html.appendObject({ create: "div", param: { innerHTML: techArray[j], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer" } } }, tech2);

                    } else {
                        this.layer.innerHTML = "Technology";
                        this.tech.innerHTML = "MicroVia";
                        var surfacefinishBox = this._presetTechObject(this.panel);
                        this.surf = pg.html.appendObject({ create: "span", param: { innerHTML: "Finish", id: "defineSurfaceFinishDiv", style: { width: "auto", cursor: "pointer" } } }, surfacefinishBox);
                        this.surf.onclick = function () { surfacefinish2.style.display = (surfacefinish2.style.display == "block") ? "none" : "block"; }

                        var layerArray = ["1-2", "Multi","Rigid-Flex"];
                        for (var i = 0; i < layerArray.length; i++)
                            pg.html.appendObject({ create: "div", param: { innerHTML: layerArray[i], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer" } } }, layer2);
                        var techArray = ["STD", "HDI", "ELIC"];
                        for (var j = 0; j < techArray.length; j++)
                            pg.html.appendObject({ create: "div", param: { innerHTML: techArray[j], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer" } } }, tech2);

                        var surfacefinish2 = pg.html.appendObject({ create: "div", param: { style: { position: "relative", display: "none", left: "10px", width: "auto", backgroundColor: "#C1D7FF" } } }, surfacefinishBox);
                        var surfArray = ["ENIG", "SoftAu", "DUAL"];
                        for (var k = 0; k < surfArray.length; k++)
                            pg.html.appendObject({ create: "div", param: { innerHTML: surfArray[k], onclick: this._selectedOne, style: { width: "auto", cursor: "pointer" } } }, surfacefinish2);
                    }
                }
            }
        };
    </script>
    <script type="text/javascript" id="page_event" >
        function overText(o,period)
        {
            o.style.backgroundColor = "#C7E5D8";
        }
        function outText(o)
        {
            o.style.backgroundColor = "Transparent";
        }
        var forecastPage = { "gridID": "grid", "loadingID": "loading",
            "loading": function() { hideDisplayBox(); document.getElementById(forecastPage.gridID).style.display = "none"; document.getElementById(forecastPage.loadingID).style.display = "block"; return true; },
            "loadCompleted": function() {
                hideDisplayBox();
                document.getElementById(forecastPage.loadingID).style.display = "none";
                var grid = document.getElementById(forecastPage.gridID);
                var tbl = grid.firstChild;
                if (tbl.tagName == undefined)
                    tbl = grid.getElementsByTagName("table")[0];

                if (tbl == undefined || tbl == null)
                    return;

                if (tbl.tagName != undefined) {
                    var rowCount = tbl.rows.length;
                    var cellCount = tbl.rows[0].cells.length;
                    if (rowCount > 2) {
                        var salesman = "";
                        for (var i = 1; i < rowCount; i++) {
                            var R = tbl.rows[i];
                            if (R.cells[0].innerHTML.trim() == salesman) { R.cells[0].innerHTML = ""; }
                            else if (R.cells.length == cellCount) { salesman = R.cells[0].innerHTML.trim(); }
                        }
                    }
                }
            }
        }
        
    </script>
    <script type="text/javascript" id="part_no_detail_and_OEM_comments_event">

        var pn = { obj: null, panel: null,
            "imgSrc": "http://bi.multek.com/ws/images/",
            "mouseOver": function(c) { c.src = this.imgSrc + "ask_orange.png"; return; },
            "mouseOut": function(c) { c.src = this.imgSrc + "ask_blue.png"; return; },
            "_setPanel": function() {
                this.panel = document.getElementById("ulist");
                /*
                if (this.panel == undefined || this.panel == null) {
                    this.panel = document.createElement("ulist");
                    var mybody = document.getElementsByTagName("body").item(0);
                    mybody.appendChild(this.panel);
var style = document.createElement('style');
style.type = 'text/css';
style.innerHTML = '.cssClass { color: #F00; }';
document.getElementsByTagName('head')[0].appendChild(style);
                }*/
                this.panel.style.display = "block"; this.panel.childNodes[1].innerHTML = "";
                var position = new getCumulativeOffset(this.obj);
                this.panel.style.left = (position.x + position.w) + "px"; this.panel.style.top = (position.y - 18) + "px";
                var pw = this.panel.clientWidth; var left = pw + position.x + position.w; var width = document.body.clientWidth;
                if (left > width && width > pw) {
                    left = width - (pw + 2);
                    if (left < position.x) { this.panel.style.left = (position.x + 10) + "px"; }
                    else { this.panel.style.left = left + "px"; }
                }
            },
            "check": function(_new_object) { if (_new_object == this.obj) { this.close(); this.obj = null; return false; } return true; },
            "close": function() { this.panel = document.getElementById("ulist"); this.panel.style.display = "none"; this.panel.childNodes[1].innerHTML = ""; },
            "open4AspRemarks": function(_object, _oemid, part) {
                if (!this.check(_object)) return;
                this.obj = _object; this._setPanel();
                this.ProjectRemark(this.panel, _oemid, part);
            },
            "open4Comments": function(_object, _oemid) {
                if (!this.check(_object)) return;
                this.obj = _object; this._setPanel();
                this.OEMcomments(_oemid);
            }
        };
        pn.OEMcomments = function(oid) {
            var div = pg.html.appendObject({ create: "div", param: { style: { paddingBottm: "3px"}} }, this.panel.childNodes[1]);
            pg.html.appendObject({ create: "div", param: { innerHTML: "Comments:", style: { width: "auto"}} }, div);
            var inpRM = pg.html.appendObject({ create: "textarea", param: { id: "inpRM", rows: 4, style: { width: "250px"}} }, div);
            inpRM.onblur = function() {
                forecastWS.setOEMComments(oid, document.getElementById("inpRM").value, pn.OEMcommentsCallBack);
                this.value = "";
            }
            forecastWS.getOEMComments(oid, pn.OEMcommentsCallBack);
        };
        pn.OEMcommentsCallBack = function(obj) {
            if (obj.trim() == "")
                return;
            var pan = document.getElementById("ulist").childNodes[1];
            if (pan.childNodes[1] != null)
                pan.removeChild(pan.childNodes[1]);
            var div = pg.html.appendObject({ create: "div", param: { style: { paddingBottm: "3px", "fontSize": "10px"}} }, pan);
            eval("var ajax = " + obj);
            div.innerHTML = "";
            if (ajax != undefined || ajax != null) {
                for (var i = 0; i < ajax.rs.length; i++) {
                    var sdiv = pg.html.appendObject({ create: "div", param: { style: { borderBottom: "1px #ccc solid" },
                        innerHTML: ajax.rs[i].author + " - " + ajax.rs[i].comment_date
                    }
                    }, div);
                    if (ajax.rs[i].del == "1") {
                        var lb = pg.html.appendObject({ create: "lable", param: { innerHTML: "del", style: { cursor: "pointer",paddingLeft:"4px"}} }, sdiv);
                        lb.onclick = function(id, oid) {
                             return function() { forecastWS.delOEMComments(id,oid, pn.OEMcommentsCallBack); } 
                            } (ajax.rs[i].comment_id,ajax.rs[i].oem_id);
                    }
                    pg.html.appendObject({ create: "div", param: { innerHTML: ajax.rs[i].comment} }, sdiv);
                }
            }
        };
        pn.ProjectRemark = function(pan, oid, part) {
            var div = pg.html.appendObject({ create: "div", param: { style: { paddingBottm: "3px"}} }, pan.childNodes[1]);
            pg.html.appendObject({ create: "div", param: { id: "pdName", style: { width: "auto", padding: "2px"}} }, div);
            pg.html.appendObject({ create: "label", param: { innerHTML: "ASP:"} }, div);
            var inpASP = pg.html.appendObject({ create: "input", param: { id: "inpASP", type: "text", disabled: "true", style: { width: "80px", color: "#888888"}} }, div);
            inpASP.onfocus = function() { this.style.color = "#000000"; inpASP.exValue = this.value; }
            inpASP.onblur = function() {
                if (this.value.trim() != "" && !isNaN(this.value) && this.value != this.exValue) {
                    this.style.color = "blue";
                    forecastWS.setCusPartRemark(oid, part, document.getElementById("inpRM").value, document.getElementById("inpASP").value, pn.ProjectCallBackASP);
                } else {
                    this.style.color = "#888888";
                }
            }
            pg.html.appendObject({ create: "div", param: { innerHTML: "Remark:", style: { width: "auto"}} }, div);
            var inpRM = pg.html.appendObject({ create: "textarea", param: { id: "inpRM", disabled: "true", rows: 4, style: { width: "250px", color: "#888888"}} }, div);
            inpRM.onfocus = function() { this.style.color = "#000000"; inpRM.exValue = this.value; }
            inpRM.onblur = function() {
                if (this.value != this.exValue) {
                    this.style.color = "blue";
                    forecastWS.setCusPartRemark(oid, part, document.getElementById("inpRM").value, document.getElementById("inpASP").value, pn.ProjectCallBackASP);
                } else {
                    this.style.color = "#888888";
                }
            }
            forecastWS.getCusPartRemark(oid, part, pn.ProjectCallBackASP);
            forecastWS.getCusPartActualHistory(oid, part, pn.ProjectCallHistory);
        }
        pn.ProjectCallBackASP = function(obj) {
            eval("var h = " + obj);
            var ipn = document.getElementById("inpRM"), inasp = document.getElementById("inpASP");
            ipn.value = inasp.value = "";
            ipn.disabled = inasp.disabled = false;
            if (h.remark != undefined) { ipn.value = h.remark; ipn.style.color = "#888888"; }
            document.getElementById("pdName").innerHTML = "";
            if (h.product_name != undefined) { document.getElementById("pdName").innerHTML = "Product Name:" + (h.product_name == "" ? "--" : h.product_name); }
            if (h.suggestASP) { document.getElementById("pdName").innerHTML += "<br/>Avg-ASP: $" + h.suggestASP; }
            if (h.ASP) {inasp.value = h.ASP;inasp.style.color = "#888888";}
        }
        pn.ProjectCallHistory = function(obj) {
            eval("var ajax = " + obj);
            var pan = document.getElementById("ulist").childNodes[1];
            var msg = pg.html.appendObject({ create: "div", param: { innerHTML: ((ajax.rs.length == 0) ? "No history found" : "History:<br/>")} }, pan);
            if (ajax.rs.length > 0) {
                var tbl = pg.html.appendObject({ create: "table", param: { className: "standardTable", width: "100%", cellspacing: "0", cellpadding: "0",
                    border: "1px", borderColor: "#999999"}
                }, msg);
                var row1 = tbl.insertRow(0);
                row1.insertCell(0).innerHTML = "Period";
                row1.insertCell(1).innerHTML = "Revenue";
                row1.insertCell(2).innerHTML = "Int.Part#";
                for (var i = 0; i < ajax.rs.length; i++) {
                    var row = tbl.insertRow(i + 1);
                    row.insertCell(0).innerHTML = "FY" + ajax.rs[i].iperiod.substr(0, 4) + ",P" + ajax.rs[i].iperiod.substr(4, 2);
                    row.insertCell(1).innerHTML = ajax.rs[i].amt.addCommasToNumber();
                    row.insertCell(2).innerHTML = ajax.rs[i].int_part_no;
                    row.cells[1].align="right";
                    row.cells[0].style.fontSize=row.cells[1].style.fontSize=row.cells[2].style.fontSize = "11px";
                }
                msg.appendChild(tbl);
            }
        }
    </script>
    <script type="text/javascript">
        var OLS = {
            globalImg: "http://bi.multek.com/ws/images/", panelId: "fcDetail", panel: null,
            table: null,
            period: null, oemid: null, object: null,
            show: function(td) {
                if (this.object == td) {
                    this.hide();
                    return;
                }
                this.object = td;
                var tr = td.parentNode;
                this.table = tr.parentNode;
                this.oemid = tr.cells[1].firstChild.innerHTML;
                this.period = this.table.rows[0].cells[td.cellIndex].firstChild.innerHTML;
                this.setPanel();

                if (this.period && this.oemid)
                    salesman.getForecastHistoryOLS(this.oemid, this.period, this.callShow);
                return;
            },
            callShow: function(obj) {
                if (obj == "") {
                    OLS.hide();
                    return;
                }
                eval("var O = " + obj);
                if (O.oem != OLS.oemid || O.period != OLS.period) {
                    OLS.hide();
                    return;
                }
                OLS.panel.innerHTML = "";
                var table = OLS._tableShow();
                var c = O.current.rs[0];
                OLS._addTR(table, "C " + OLS.toPeriod(c.InPeriod), c.amt, c.ols, c.topside);
                if (O.log) {
                    var log = O.log.rs;
                    for (var i = 0; i < log.length; i++) {
                        OLS._addTR(table, OLS.toPeriod(log[i].InPeriod), log[i].amt, log[i].ols, log[i].topside);
                    }
                }
                OLS.panel.innerHTML += "";
            },
            _addTR: function(table, p, a, o, t) {
                var tr = pg.html.appendObject({ create: "TR", param: { bgColor: "#ffffff"} }, table);
                pg.html.appendObject({ create: "td", param: { innerHTML: p} }, tr);
                pg.html.appendObject({ create: "td", param: { innerHTML: a.addCommasToNumber(), align: "right"} }, tr);
                pg.html.appendObject({ create: "td", param: { innerHTML: o.addCommasToNumber(), align: "right"} }, tr);
                pg.html.appendObject({ create: "td", param: { innerHTML: t.addCommasToNumber(), align: "right"} }, tr);
            },
            _tableShow: function() {
                var tb = pg.html.appendObject({ create: "table", param: { className: "standardTable", border: "1px", cellPadding: "1px",
                    cellSpacing: "1px", bgColor: "#ffffff", width: "100%"
                }
                }, OLS.panel);
                var tr = pg.html.appendObject({ create: "TR", param: { bgcolor: "#FF9D60"} }, tb);
                pg.html.appendObject({ create: "th", param: { innerHTML: "Period"} }, tr);
                pg.html.appendObject({ create: "th", param: { innerHTML: "FC Amt"} }, tr);
                pg.html.appendObject({ create: "th", param: { innerHTML: "OLS"} }, tr);
                pg.html.appendObject({ create: "th", param: { innerHTML: "Top Side"} }, tr);
                return tb;
            },
            toPeriod: function(p) {
                return "FY" + p.substring(0, 4) + "P" + p.substring(4);
            },
            setPanel: function() {
                var pos = new getCumulativeOffset(this.object);
                pg.html.showDivContent(this.panelId);
                var pan = document.getElementById(this.panelId);
                document.getElementById("main").appendChild(pan);
                pg.html.changeObject({ style: { border: "2px #fff solid", width: "300px", whiteSpace: "normal", paddingTop: "14px",
                    left: (pos.x - 220) + "px", top: (pos.y + 17) + "px",
                    background: "#FFB27F url('" + this.globalImg + "arrowCon.png') no-repeat right top"
                }, innerHTML: ""
                }, pan);
                this.panel = pan;
            },
            hide: function() {
                OLS.object = null;
                pg.html.hideDivContent(OLS.panelId); return;
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/services/forecastWS.asmx" />
            <asp:ServiceReference Path="~/services/salesman.asmx" />
        </Services>
    </asp:ScriptManager>

    <asp:UpdatePanel ID="updatePanel1" runat="server">
    <ContentTemplate>
<div id="ulist" class="mainPanel"><div class="sayIcon">&nbsp;</div><div class="subPanel"></div></div>
<div id="temp"></div>
<asp:Label ID="currentPeriod" runat="server" CssClass="hide" />
<asp:Label ID="selectedOEMID" runat='server' CssClass="hide" />
        <table class="standardTable" border="1" width="100%" bordercolor="#cccccc">
        <tr><td width="100" bgcolor="#C4E0FF">Fiscal Year:</td>
        <td width="*">
            <div style="width:auto;">
            <asp:DropDownList ID="startFY" runat="server"></asp:DropDownList>
            <asp:DropDownList ID="startPeriod" runat="server"></asp:DropDownList>

            <asp:TextBox ID="selectSalesId" runat="server" Text="0"  CssClass="hide" />
            <asp:CheckBox ID="subSales" runat="server" Text="Subordinates" Checked="false" />
            <asp:CheckBox ID="bkSales" runat="server" Text="Backup" Checked="false" />
             &nbsp;
            <asp:PlaceHolder Visible="false" runat="server" ID="adminFeature" >
            Sales:<asp:TextBox ID="selectSales" runat="server" Text="" Width="140"/>
            </asp:PlaceHolder>
            OEM:<asp:TextBox ID="selectOEM" runat="server" Text="" Width="80" />
            Plant:<asp:TextBox ID="selectPlant" runat="server" Text="" Width="30" />
            Group:<asp:TextBox ID="selectGroup" runat="server" Text="" Width="60" />
            <asp:Button ID="Button1" runat="server" Text="Go" onclick="Button1_Click" OnClientClick="return forecastPage.loading()" />
            
            <asp:Button ID="Download1" runat="server" Text="Download" OnClick="Download1_Click" />
            </div>
        </td>
        </tr>
        </table>
        <asp:Label ID="SaleSubmit" runat="server" ForeColor="Red" Font-Bold="true" />
<div id="loading" style="display:none; text-align:center; padding:20px 0px 20px 0px; margin:0px auto 0px; width:200px;">
<img src="http://bi.multek.com/ws/images/ajax-loader_1.gif" alt="loading" align="middle" />
</div>
<asp:Label ID="temp" runat="server" ForeColor="Red" Font-Bold="true"/>
        <div style="visibility:visible; padding:0px; display:block; width:100%;"  
        id="grid"><asp:ListView id="list1" runat="server" OnItemDataBound="list1_ItemDataBound" 
                onselectedindexchanging="list1_SelectedIndexChanging" >
            <LayoutTemplate>
                <table class="EDITable" cellpadding="1" cellspacing="0" border="1" bordercolor="#cccccc" runat="server" >

                <tr bgcolor="#E1EFF2"><th><asp:Label ID="l1" Font-Bold="true" Text="Sales" runat="server" /></th>
                    <th><asp:Label ID="Label13" Font-Bold="true" Text="OEM" runat="server" /></th>
                    <th><asp:Label ID="curr_1" runat="server" /></th>
                    <th><asp:Label ID="curr_2" runat="server" /></th>
                    <th><asp:Label ID="curr_3" runat="server" /></th>
                    <th><asp:Label ID="load_4" runat="server" /></th>
                    <th><asp:Label ID="load_5" runat="server" /></th>
                    <th><asp:Label ID="p1" runat="server" CssClass="hide" /><asp:Label ID="Label1" runat="server" /></th>
                    <th><asp:Label ID="p2" runat="server" CssClass="hide" /><asp:Label ID="Label2" runat="server" /></th>
                    <th><asp:Label ID="p3" runat="server" CssClass="hide" /><asp:Label ID="Label3" runat="server" /></th>
                    <th><asp:Label ID="p4" runat="server" CssClass="hide" /><asp:Label ID="Label4" runat="server" /></th>
                    <th><asp:Label ID="p5" runat="server" CssClass="hide" /><asp:Label ID="Label5" runat="server" /></th>
                    <th><asp:Label ID="p6" runat="server" CssClass="hide" /><asp:Label ID="Label6" runat="server" /></th>
                    <th><asp:Label ID="p7" runat="server" CssClass="hide" /><asp:Label ID="Label7" runat="server" /></th>
                    <th><asp:Label ID="p8" runat="server" CssClass="hide" /><asp:Label ID="Label8" runat="server" /></th>
                    <th><asp:Label ID="p9" runat="server" CssClass="hide" /><asp:Label ID="Label9" runat="server" /></th>
                    <th><asp:Label ID="p10" runat="server" CssClass="hide" /><asp:Label ID="Label10" runat="server" /></th>
                    <th><asp:Label ID="p11" runat="server" CssClass="hide" /><asp:Label ID="Label11" runat="server" /></th>
                    <th><asp:Label ID="p12" runat="server" CssClass="hide" /><asp:Label ID="Label12" runat="server" /></th>
                </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
                <tr bgcolor="#EDFBFF">
                    <td colspan="2">Total</td>
                    <td><asp:Label id="tt1" runat="server" /></td>
                    <td><asp:Label id="tt2" runat="server" /></td>
                    <td><asp:Label id="tt3" runat="server" /></td>
                    <td><asp:Label id="tt4" runat="server" /></td>
                    <td><asp:Label id="tt5" runat="server" /></td>
                    <td><asp:Label id="ttl1" runat="server" /><asp:LinkButton ID="bttl1" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl2" runat="server" /><asp:LinkButton ID="bttl2" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl3" runat="server" /><asp:LinkButton ID="bttl3" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl4" runat="server" /><asp:LinkButton ID="bttl4" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl5" runat="server" /><asp:LinkButton ID="bttl5" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl6" runat="server" /><asp:LinkButton ID="bttl6" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl7" runat="server" /><asp:LinkButton ID="bttl7" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl8" runat="server" /><asp:LinkButton ID="bttl8" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl9" runat="server" /><asp:LinkButton ID="bttl9" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl10" runat="server" /><asp:LinkButton ID="bttl10" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl11" runat="server" /><asp:LinkButton ID="bttl11" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                    <td><asp:Label id="ttl12" runat="server" /><asp:LinkButton ID="bttl12" runat="server" Text="Copy" Visible="false" onclick="bbt12_Click" /></td>
                </tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr onmouseover="this.style.backgroundColor='#F8CCFF'"
                    onmouseout="this.style.backgroundColor=''">
                    <td><div><%# Eval("userName")%></div></td>
                    <td><asp:Label ID="oemid" runat="server" Text='<%# Eval("oemid") %>' CssClass="hide" /><div><asp:LinkButton ID="selOEM" runat="server" CommandName="Select" Text='<%# Eval("cusoem") %>' OnClientClick="return forecastPage.loading()"/></div></td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[3])%></td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[4])%></td>
                    <td class="pastPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[5])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[6])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[7])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[8])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[9])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[10])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[11])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[12])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[13])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[14])%></td>
                    
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[15])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[16])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[17])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[18])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[19])%></td>
                </tr>
            </ItemTemplate>
            <SelectedItemTemplate>
                <tr style="background-color:#DBFFC6">
                    <td style="font-weight:bold;"><div><%# Eval("userName")%></div></td>
                    <td style="font-weight:bold; white-space:nowrap">
                        <asp:Label ID="oemid" runat="server" Text='<%# Eval("oemid") %>' CssClass="hide" />
                        <asp:LinkButton ID="selOEM" runat="server" CommandName="select" Text='<%# Eval("cusoem") %>' />
                        <asp:Image runat="server" ID="oemRemark" ImageUrl="http://bi.multek.com/ws/images/test.gif" ImageAlign="AbsBottom" />
                    </td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[3])%></td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[4])%></td>
                    <td class="pastPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[5])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[6])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[7])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[8])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[9])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[10])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[11])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[12])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[13])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[14])%></td>
                    
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[15])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[16])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[17])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[18])%></td>
                    <td onclick="return OLS.show(this)"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[19])%></td>
                </tr>

                <asp:ListView ID="list3" runat="server" InsertItemPosition="FirstItem" 
                oniteminserting="list3_ItemInserting" OnItemDataBound="list3_ItemDatabound">
                <LayoutTemplate>
                <tr id="itemPlaceholder" runat="server"></tr>
                </LayoutTemplate>
                <InsertItemTemplate>
                    <tr>
                        <td colspan="2" align="right"><div>add others:<asp:TextBox ID="newPN" runat="server" Width="80" CssClass="newPart" /></div></td>
                        <td colspan="3" class="Btitle" style="text-align:center"><asp:Button ID="add" Text="Add" runat="server" CommandName="Insert" />Actual</td><td colspan="2" class="Btitle" style="text-align:center">Loading</td>
                        <td colspan="12" class="Btitle" style="text-align:center">Forecast</td>
                    </tr>
                </InsertItemTemplate>
                <ItemTemplate>
                    <tr onmouseover='this.style.backgroundColor="#FFF6B5"'
                        onmouseOut='this.style.backgroundColor=""'>
                    <td colspan="2" align="right"><div><asp:Label ID="pn" runat="server" Text='<%# Eval("cus_part_no")%>' /> <asp:Image ID="img_detail" runat="server" /></div>
                    </td>
                        <td class="pastPeriodParts"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[15])%></td>
                        <td class="pastPeriodParts"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[16])%></td>
                        <td class="pastPeriodParts"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[17])%></td>
                        <td style="color:#cccccc">--</td>
                        <td style="color:#cccccc">--
                            <asp:Label ID="lab1" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[3].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab2" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[4].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab3" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[5].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab4" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[6].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab5" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[7].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab6" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[8].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab7" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[9].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab8" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[10].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab9" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[11].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab10" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[12].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab11" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[13].ColumnName %>' Visible="false" />
                            <asp:Label ID="lab12" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem).DataView.Table.Columns[14].ColumnName %>' Visible="false" />
                        </td>
                        <td><asp:TextBox CssClass="revenue" ID="ren1" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[3])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren2" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[4])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren3" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[5])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren4" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[6])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren5" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[7])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren6" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[8])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren7" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[9])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren8" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[10])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren9" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[11])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren10" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[12])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren11" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[13])%>' /></td>
                        <td><asp:TextBox CssClass="revenue" ID="ren12" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[14])%>' /></td>
                    </tr>
                </ItemTemplate>
                </asp:ListView>            
            </SelectedItemTemplate>
        </asp:ListView>
        Click when you finish (you can come back and revise figures after clicking)
        <asp:LinkButton ID="finish_forecast" runat="server" Text="Submit" 
                onclick="finish_forecast_Click" />
        </div>    

    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="Download1" />
    </Triggers>
    </asp:UpdatePanel>


</asp:Content>

