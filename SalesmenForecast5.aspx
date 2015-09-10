<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="SalesmenForecast5" Title="Salesman Forecast" Codebehind="SalesmenForecast5.aspx.cs" %>

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
    <script type="text/jscript" id="filter">
        var me=null;
        var inprogress = false;
        var serverId=null;
        var dpb = "displayBox";

        var selId=-1;
        function checkSales(o, client){
            me = o;
            serverId = client;
            document.getElementById(serverId).value = "0";
            me.style.color = "red";
            if(!inprogress)
            {
                var keyCode;
		        if(window.event) keyCode = window.event.keyCode;
		        else if(e) keyCode = e.which;
		        if(keyCode==27){
		            me.value = "";
		            hideDisplayBox();
		            return;
		        }
		        if(keyCode == 38 || keyCode == 40)
		        {  
		            var div = document.getElementById(dpb);
		            if(div != undefined && div.childNodes.length > 0){
		                var max = div.childNodes.length-1;
		                (keyCode==38)?selId--:selId++;
		                if(selId > max)
		                    selId = 0;
		                if(selId <0)
		                    selId = max;

                        for(var i=0; i <= max; i++){
                            div.childNodes[i].style.backgroundColor = "";
                        }		                    
		                    var obk = div.childNodes[selId];
		                    obk.style.backgroundColor = "#FFFCCC";
                            me.value = obk.innerHTML;
                            document.getElementById(serverId).value = obk.did;
                            me.style.color = "blue";
                            serverId = null;
		            }
		        }else{
                    hideDisplayBox();
                    inprogress=true;
                    selId=-1;
                    salesman.getUsers(o.value,showValue);
                }
            }
        }    
        function showValue(obj)
        {
            if(!inprogress)
                return;
            var result = eval(obj); 
            var panel = pg.html.showPanel(event,dpb,"auto","100");
            var xy = new getCumulativeOffset(me);
            panel.style.left = (xy.x)+"px";
            panel.style.top = (xy.y + 22) +"px";
            panel.style.width = me.clientWidth - 4;
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
            /*
            var p = document.getElementById(dpb);
            if(p !=null){p.parentNode.removeChild(p);}
            */
            pg.html.hideDivContent(dpb);
            pg.html.hideDivContent(subPanel);
            closePanel();
        }
        
        function keyMove(obj){
            var keyCode;
		    if(window.event) keyCode = window.event.keyCode;
		    else if(e) keyCode = e.which;
		    if((keyCode >= 37 && keyCode <= 40) || keyCode == 9)
		    {  
                var td = obj.parentNode;
                var tr = td.parentNode;
var cellid = td.cellIndex;
var rowid = tr.rowIndex;
var totalCells = tr.cells.length;
var tby = tr.parentNode;
var totalRows = tby.rows.length;
var tgObj = null;
		        switch(keyCode)
		        {
		            
		            case 38:
		                if(cellid > 1 && cellid < totalRows){
		                    var ntr = tby.rows[rowid-1];
		                    if(ntr.cells.length == totalCells){
		                        tgObj = ntr.cells[cellid].firstChild;
                                if(tgObj != undefined && !tgObj.disabled){
                                    tgObj.focus();                           
                                }
		                    }
		                }
                        break;
                    case 40:
		                if(cellid > 1 && cellid < totalRows){
		                    var ntr = tby.rows[rowid+1];
		                    if(ntr.cells.length == totalCells){
		                        tgObj = ntr.cells[cellid].firstChild;
                                if(tgObj != undefined && !tgObj.disabled){
                                    tgObj.focus();                           
                                }
		                    }
		                }
                        break;
                    case 37:
                    case 9:
                        if(chkML(obj)){
                            if(cellid >2 && tr.cells[cellid-1] != undefined){
                                tgObj = tr.cells[cellid-1].firstChild;
                                if(tgObj != undefined && !tgObj.disabled){
                                    tgObj.focus();                           
                                }
                            }
                        }
                        break;
                        
                    case 39:
                        if(chkMR(obj) && cellid <totalCells ){
                                tgObj = tr.cells[cellid+1].firstChild;
                                if(tgObj != undefined && !tgObj.disabled){
                                    tgObj.focus();                           
                                }
    		            }
    		            break;
    		            
		        }
		        
		    }
        }

    function chkMR(obj){
        var right = false;
        if(obj.createTextRange){
            var sel = document.selection.createRange();
            if(sel.text.length == 0 && getSelectionEnd(obj) == obj.value.length)
                right = true;
        }
        return right;
    }
    function chkML(obj){
        var left = false;
        if(obj.createTextRange){
            var sel = document.selection.createRange();
            if(sel.text.length == 0 && getSelectionEnd(obj) == 0)
                left = true;
        }
        return left;
    }
    function getSelectionStart(o) 
    {
	    if (o.createTextRange) {
		    var r = document.selection.createRange().duplicate()
		    r.moveEnd('character', o.value.length)
		    if (r.text == '') return o.value.length
		    return o.value.lastIndexOf(r.text)
	    } else return o.selectionStart
    }
    function getSelectionEnd(o) 
    {
	    if (o.createTextRange) {
		    var r = document.selection.createRange().duplicate()
		    r.moveStart('character', -o.value.length)
		    return r.text.length
	    } else return o.selectionEnd
    }

    </script>
    <script type="text/javascript">
    var subPanel = "__sub__panel";
    var npj;
    var npjimg;
    
    
    var fcIpDetail = null;
    function inputDetail(obj){
        fcIpDetail = obj;
        pg.html.showDivContent("fcDetail");
        var pan = document.getElementById("fcDetail");
        //var pan = showPanel(event,"fcDetail","auto","auto");

        pan.style.background = "#FFB27F url('http://bi.multek.com/ws/images/arrowCon.png') no-repeat right top";
        pan.style.border = "2px #fff solid";
        /*
        pan.style.borderBottom = "4px #666 solid";
        pan.style.borderRight = "4px #666 solid";
        */
        pan.style.paddingTop = "14px";
        pan.style.width = "300px";
        pan.style.whiteSpace = "normal";
        pan.innerHTML = "";
        var currentPosition = new getCumulativeOffset(obj);
        pan.style.left = (currentPosition.x -220)+ "px";
        pan.style.top = (currentPosition.y+ 17) + "px";
        var tb = obj.parentNode.parentNode.parentNode;
        var title = tb.rows[0].cells[obj.parentNode.cellIndex + 1].innerText;
        pan.innerHTML = "Period: "+title +"<br/>P/N: "+ obj.getAttribute("pn");// +".."+obj.getAttribute("oemid");
        forecastWS.getCusPartRemark(obj.getAttribute("oemid"),obj.getAttribute("pn"),moreDetail);
    }
    function moreDetail(obj){
        eval("var ox = " + obj);
        if(fcIpDetail.getAttribute("oemid") != ox.oemid || fcIpDetail.getAttribute("pn") != ox.pn)
        return;
        
        var pan = document.getElementById("fcDetail");
        pan.innerHTML += "<br/>ASP: " + (ox.ASP?ox.ASP:"--") +
        "<br/>Remark: " + (ox.remark?ox.remark:"--"); //Not yet updated
        
    }
    
function focusToInput(obj){
    obj.select();
    obj.style.border = '1px #555 solid';
    inputDetail(obj); //show forecast parts detail
}
function blurToUpdate(obj,period)
{
    pg.html.hideDivContent("fcDetail");//hide forecast parts detail
    obj.style.border = '0px';
    obj.value = obj.value.ReplaceAll(",","");
    if(obj.getAttribute("ov").ReplaceAll(",","") != obj.value)
    {
        obj.style.color = "red"; 
        if(isNaN(obj.value) || obj.value.trim() == "")
            obj.value = obj.getAttribute("ov");
        else
            forecastWS.getCusPartForecastUpdate(period,obj.getAttribute("oemid"),obj.getAttribute("pn"),obj.value,obj.id,false,retValue3);
    }else{
        obj.value = obj.getAttribute("ov");
    }
}
/*
function blurToUpdate(obj,oemid,period,pn,oriValue)
{
    pg.html.hideDivContent("fcDetail");//hide forecast parts detail
    obj.style.border = '0px';
    obj.value = obj.value.ReplaceAll(",","");
    if(obj.getAttribute("ov").ReplaceAll(",","") != obj.value)
    {
        obj.style.color = "red"; 
        if(isNaN(obj.value) || obj.value.trim() == "")
            obj.value = obj.getAttribute("ov");
        else
            forecastWS.getCusPartForecastUpdate(period,oemid,pn,obj.value,obj.id,false,retValue3);
    }else{
        obj.value = obj.getAttribute("ov");
    }
}
*/
function focusToDisplay(obj)
{
    //pg.html.showDivContent("altDisp",this.period,event);
}
function retValue3(obj){
    var rs3 = eval(obj);
    var cell = document.getElementById(rs3.cell);
    if(cell != undefined){
        cell.ov = cell.value = rs3.amt.addCommasToNumber();
        cell.style.color = "blue";
    }
}
    function callNewPJ(obj,imid)
    {
        hideDisplayBox();
        npj = obj;
        npjimg = document.getElementById(imid);
        obj.value = "";
        npjimg.style.display = "none";
        var nt = pg.html.showPanel(event,subPanel,"auto",200);
        nt.style.zIndex = 100;                
        nt.innerHTML = "";
        var clsimg = addCloseImage(nt);
        clsimg.onclick = closeNPJ;
        technologySelection(nt);

    }
            function addCloseImage(parent){
                    var cls = pg.html.appendObject({create:"div",param:{style:{textAlign:"right",backgroundColor:"#C4E8FF",color:"#ffffff",padding:"2px 5px"}}},parent);
                    var img = pg.html.appendObject({create:"img",param:{src:"http://bi.multek.com/ws/images/stopC2.png",style:{textAlign:"right",cursor:"pointer"}}},cls);
                    img.onmouseover = function(){this.src = "http://bi.multek.com/ws/images/stopC1.png"}
                    img.onmouseout = function(){ this.src = "http://bi.multek.com/ws/images/stopC2.png"}
                    return img;
            }
            function closeNPJ(){ 
                pg.html.hideDivContent(subPanel);
            }
            function technologySelection(div){
                var dkey  = pg.html.appendObject({create:"div",param:{style:{width:"auto",cursor:"default"}}},div);
                var layer = pg.html.appendObject({create:"div",param:{style:{width:"auto",cursor:"default"}}},div);
                var layer1 = pg.html.appendObject({create:"span",param:{innerHTML:"Layer",id:"defineLayerDiv",style:{width:"auto",cursor:"pointer"}}},layer);
                layer1.onclick = function(){
                    if(layer2.style.display == "block")
                        layer2.style.display = "none";
                    else
                        layer2.style.display = "block";
                }
                var layer2 = pg.html.appendObject({create:"div",param:{style:{position:"relative",display:"none",left:"10px",width:"auto",backgroundColor:"#C1D7FF"}}},layer);
                pg.html.appendObject({create:"div",param:{innerHTML:"2-8",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},layer2);
                pg.html.appendObject({create:"div",param:{innerHTML:"10-16",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},layer2);
                pg.html.appendObject({create:"div",param:{innerHTML:"18-24",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},layer2);
                pg.html.appendObject({create:"div",param:{innerHTML:"26+",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},layer2);
               
                var tech = pg.html.appendObject({create:"div",param:{style:{width:"auto",cursor:"default"}}},div);
                var tech1 = pg.html.appendObject({create:"span",param:{innerHTML:"Technology",id:"defineTechnologyDiv",style:{width:"auto",cursor:"pointer"}}},tech);
                tech1.onclick = function(){
                    if(tech2.style.display == "block")
                        tech2.style.display = "none";
                    else
                        tech2.style.display = "block";
                }
                var tech2 = pg.html.appendObject({create:"div",param:{style:{position:"relative",display:"none",left:"10px",width:"auto",backgroundColor:"#C1D7FF"}}},tech);
                pg.html.appendObject({create:"div",param:{innerHTML:"STD",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"BVH",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"HDI 1+N+1",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"HDI 2+N+2",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"HDI 3+N+3",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"HDI 4+N+4",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"ELIC",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"Embedded Coin",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                pg.html.appendObject({create:"div",param:{innerHTML:"Attached Coin",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},tech2);
                
                var surfacefinish =pg.html.appendObject({create:"div",param:{style:{width:"auto",cursor:"default"}}},div);
                var surfacefinish1 = pg.html.appendObject({create:"span",param:{innerHTML:"SurfaceFinish",id:"defineSurfaceFinishDiv",style:{width:"auto",cursor:"pointer"}}},surfacefinish);
                surfacefinish1.onclick = function(){
                    if(surfacefinish2.style.display == "block")
                        surfacefinish2.style.display = "none";
                    else
                        surfacefinish2.style.display = "block";
                }
                var surfacefinish2 = pg.html.appendObject({create:"div",param:{style:{position:"relative",display:"none",left:"10px",width:"auto",backgroundColor:"#C1D7FF"}}},surfacefinish);
                pg.html.appendObject({create:"div",param:{innerHTML:"Au Plating",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                //pg.html.appendObject({create:"div",param:{innerHTML:"Hard Gold Plating",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                pg.html.appendObject({create:"div",param:{innerHTML:"Gold Finger",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                pg.html.appendObject({create:"div",param:{innerHTML:"Immersion Silver",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                pg.html.appendObject({create:"div",param:{innerHTML:"Immersion Tin",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                pg.html.appendObject({create:"div",param:{innerHTML:"ENIG",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                pg.html.appendObject({create:"div",param:{innerHTML:"OSP",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
                pg.html.appendObject({create:"div",param:{innerHTML:"Not specified",onclick:divSelectedOne,style:{width:"auto",cursor:"pointer"}}},surfacefinish2);
            }
            function divSelectedOne(){
                var bc = 'red';
                if(this.style.color == bc)
                    bc = '';
                var n = this.parentNode;
                for(var i=0; i < n.childNodes.length;i++){
                    n.childNodes[i].style.color = '';
                }
                this.style.color = bc;
                this.parentNode.style.display = "none";
                var ppa = this.parentNode.parentNode.firstChild;
                var ppastr = ppa.innerHTML.split(" ");
                ppa.innerHTML = ppastr[0] +" <b style='color:red'>"+ this.innerHTML+"</b>";
                defineTech();
            }
            function defineTech(){
                var lay = document.getElementById("defineLayerDiv").innerText.substr(5).trim();//Layer
                var tec = document.getElementById("defineTechnologyDiv").innerText.substr(10).trim();//Technology
                var sff = document.getElementById("defineSurfaceFinishDiv").innerText.substr(13).trim();//SurfaceFinish
                npj.value = "";
                if(lay+tec+sff != ""){
                    npj.value = (lay!=""?"["+lay+"]":"[-]") +(tec!=""?"["+ tec+"]":"[-]") + (sff!=""?"["+ sff +"]":"[-]");
                    npj.style.width = "auto";
                    if(lay != "" && tec != "" && sff != "")
                        npjimg.style.display = "block";
                    else
                        npjimg.style.display = "none";
                }                
            }
        function overText(o,period)
        {
            o.style.backgroundColor = "#C7E5D8";
            //pg.html.showDivContent("altDisp",period,event);
        }
        function outText(o)
        {
            o.style.backgroundColor = "Transparent";
            //pg.html.hideDivContent("altDisp");
        }
        
        
        function pageReload(){
            document.getElementById("loading").style.display = "block";
            document.getElementById("grid").style.display = "none";
            return true;
        }
        function pageReloaded(){
            document.getElementById("loading").style.display = "none";
            var dv = document.getElementById("grid");
            dv.style.display = "block";
            hideDisplayBox();
            
            var tbl = dv.firstChild;
            if(tbl.tagName != undefined){
                var rows = tbl.rows.length;
                if(rows >2){
                    var sales="";
                    var cells = tbl.rows[1].cells.length;
                    for(var i = 1; i < rows-1;i ++){
                        var row = tbl.rows[i];
                        if(sales == row.cells[0].innerHTML.trim()){
                            tbl.rows[i].cells[0].innerHTML = "";
                        }else if(cells == row.cells.length){
                            sales = row.cells[0].innerHTML;
                        }
                    }
                }
            }
        }
    </script>
    <script type="text/javascript" id="page_event">
    function pj_over(img){
        img.src = "http://bi.multek.com/ws/images/ask_orange.png";
        return;
    }
    function pj_out(img){
        img.src = "http://bi.multek.com/ws/images/ask_blue.png";
        return;
    }
    var openedObj = null;
    function pj_open(obj,oid,pn){
        if(openedObj == obj){
            closePanel();
            openedObj = null;
            return;
        }
        openedObj = obj;
        var pan = document.getElementById("ulist");
        pan.style.display = "block";
        pan.childNodes[1].innerHTML = "";
        
        var currentPosition = new getCumulativeOffset(obj);
        pan.style.left = (currentPosition.x + currentPosition.w)+ "px";
        pan.style.top = (currentPosition.y -18) + "px";
        var endLeft = pan.clientWidth + currentPosition.x + currentPosition.w;
        var docWidth = document.body.clientWidth;
        if(endLeft > docWidth && docWidth > pan.clientWidth)
        {
            var left = docWidth - (pan.clientWidth + 2);
            if(left < currentPosition.x)
                pan.style.left = (currentPosition.x + 10)+"px";
            else
                pan.style.left = left + "px";
        }

        var div = pg.html.appendObject({create:"div",param:{style:{paddingBottm:"3px"}}},pan.childNodes[1]);
        pg.html.appendObject({create:"div",param:{id:"pdName",style:{width:"auto",padding:"2px"}}},div);
        pg.html.appendObject({create:"label",param:{innerHTML:"ASP:"}},div);

        var inpASP = pg.html.appendObject({create:"input",param:{id:"inpASP",type:"text",disabled:"true",style:{width:"80px",color:"#888888"}}},div);
        inpASP.onfocus = function(){this.style.color="#000000"; inpASP.exValue = this.value;}
        inpASP.onblur = function(){
        
            if(this.value.trim() != "" && !isNaN(this.value) && this.value != this.exValue){
                this.style.color = "blue";
                forecastWS.setCusPartRemark(oid,pn,document.getElementById("inpRM").value,document.getElementById("inpASP").value,rtn2);
            }else{
                this.style.color = "#888888";
            }
        }

        pg.html.appendObject({create:"div",param:{innerHTML:"Remark:",style:{width:"auto"}}},div);
        var inpRM = pg.html.appendObject({create:"textarea",param:{id:"inpRM",disabled:"true",rows:4,style:{width:"250px",color:"#888888"}}},div);
        inpRM.onfocus = function(){this.style.color = "#000000";inpRM.exValue = this.value;}
        inpRM.onblur = function(){
            if(this.value != this.exValue){     
                this.style.color = "blue";
                forecastWS.setCusPartRemark(oid,pn,document.getElementById("inpRM").value,document.getElementById("inpASP").value,rtn2);
            }else{
                this.style.color = "#888888";
            }
        }
        forecastWS.getCusPartRemark(oid,pn,rtn2);
        forecastWS.getCusPartActualHistory(oid,pn,rtn);
    }
    function rtn2(obj){
        eval("var h = " + obj);
        var ipn = document.getElementById("inpRM"), inasp = document.getElementById("inpASP");
        ipn.value = inasp.value = "";
        ipn.disabled = inasp.disabled = false;
        
        if(h.remark != undefined){
            ipn.value = h.remark;
            ipn.style.color = "#888888";
        }
        document.getElementById("pdName").innerHTML = "";
        if(h.product_name != undefined)
            document.getElementById("pdName").innerHTML = "Product Name:"+ (h.product_name==""?"--":h.product_name);
        if(h.suggestASP){
            document.getElementById("pdName").innerHTML += "<br/>Avg-ASP: $"+ h.suggestASP;
        }
        if(h.ASP){
            inasp.value = h.ASP;
            inasp.style.color = "#888888";
        }
       
    }
    function rtn(obj){
        eval("var ajax = "+ obj);
        var pan = document.getElementById("ulist").childNodes[1];
        var msg = document.createElement("div");
        pan.appendChild(msg);
        if(ajax.rs.length == 0){
            msg.innerHTML = "No history found";
        }else{
            msg.innerHTML = "History:<br/>";
            var tbl = document.createElement("table");
            tbl.className = "standardTable";
            tbl.width = "100%";
            tbl.cellspacing="0";
            tbl.cellpadding="0";
            tbl.border = "1px";
            tbl.borderColor = "#999999";
            var row1 = tbl.insertRow(0);
            var c1 = row1.insertCell(0);
            var c2 = row1.insertCell(1);
            var c3 = row1.insertCell(2);
            c1.innerHTML = "Period";
            c2.innerHTML = "Revenue";
            c3.innerHTML = "Int.Part#";
            for(var i=0;i < ajax.rs.length; i++){
                var row = tbl.insertRow(i+1);
                var cp = row.insertCell(0);
                var cr = row.insertCell(1);
                var cn = row.insertCell(2);
                cp.innerHTML = "FY"+ ajax.rs[i].iperiod.substr(0,4) +",P"+ ajax.rs[i].iperiod.substr(4,2);
                cr.innerHTML = ajax.rs[i].amt.addCommasToNumber();
                cr.align = "right";
                cn.innerHTML = ajax.rs[i].int_part_no;
                cp.style.fontSize = cr.style.fontSize = cn.style.fontSize = "11px";
            }
            msg.appendChild(tbl);
        }
    }
    function closePanel(){
        var pan = document.getElementById("ulist");
        pan.style.display = "none";
        pan.childNodes[1].innerHTML = "";
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
            <asp:Button ID="Button1" runat="server" Text="Go" onclick="Button1_Click" OnClientClick="return pageReload()" />
            
            <asp:Button ID="Download1" runat="server" Text="Download" OnClick="Download1_Click" />
            </div>
        </td>
        </tr>
        </table>
        <asp:Label ID="SaleSubmit" runat="server" ForeColor="Red" Font-Bold="true" />
<div id="loading" style="display:none; text-align:center; padding:20px 0px 20px 0px; margin:0px auto 0px; width:200px;">
<img src="http://bi.multek.com/ws/images/ajax-loader_1.gif" alt="loading" align="middle" />
</div>
<asp:Label ID="temp" runat="server" />
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
                    <th><asp:Label ID="Label1" runat="server" /></th>
                    <th><asp:Label ID="Label2" runat="server" /></th>
                    <th><asp:Label ID="Label3" runat="server" /></th>
                    <th><asp:Label ID="Label4" runat="server" /></th>
                    <th><asp:Label ID="Label5" runat="server" /></th>
                    <th><asp:Label ID="Label6" runat="server" /></th>
                    <th><asp:Label ID="Label7" runat="server" /></th>
                    <th><asp:Label ID="Label8" runat="server" /></th>
                    <th><asp:Label ID="Label9" runat="server" /></th>
                    <th><asp:Label ID="Label10" runat="server" /></th>
                    <th><asp:Label ID="Label11" runat="server" /></th>
                    <th><asp:Label ID="Label12" runat="server" /></th>
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
                    <td><%# Eval("userName")%></td>
                    <td><asp:LinkButton ID="selOEM" runat="server" CommandName="Select" Text='<%# Eval("cusoem") %>' OnClientClick="return pageReload()"/></td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[3])%></td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[4])%></td>
                    <td class="pastPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[5])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[6])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[7])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[8])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[9])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[10])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[11])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[12])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[13])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[14])%></td>
                    
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[15])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[16])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[17])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[18])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[19])%></td>
                </tr>
            </ItemTemplate>
            <SelectedItemTemplate>
                <tr style="background-color:#DBFFC6">
                    <td style="font-weight:bold;"><%# Eval("userName")%></td>
                    <td style="font-weight:bold; white-space:nowrap">
                        <asp:LinkButton ID="selOEM" runat="server" CommandName="select" Text='<%# Eval("cusoem") %>' />
                        <asp:Label ID="oemid" runat="server" Text='<%# Eval("oemid") %>' Visible="false" />
                        <asp:Image runat="server" ID="oemRemark" ImageUrl="http://bi.multek.com/ws/images/test.gif" ImageAlign="AbsBottom" />
                    </td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[3])%></td>
                    <td class="pastPeriod"><%#string.Format("{0:N0}", ((System.Data.DataRowView)Container.DataItem)[4])%></td>
                    <td class="pastPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[5])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[6])%></td>
                    <td class="loadPeriod"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[7])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[8])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[9])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[10])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[11])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[12])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[13])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[14])%></td>
                    
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[15])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[16])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[17])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[18])%></td>
                    <td><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[19])%></td>
                </tr>

                <asp:ListView ID="list3" runat="server" InsertItemPosition="FirstItem" 
                oniteminserting="list3_ItemInserting" OnItemDataBound="list3_ItemDatabound">
                <LayoutTemplate>
                <tr id="itemPlaceholder" runat="server"></tr>
                </LayoutTemplate>
                <InsertItemTemplate>
                    <tr>
                        <td colspan="2" align="right">add others:<asp:TextBox ID="newPN" runat="server" Width="80" CssClass="newPart" /></td>

                        <td colspan="3" class="Btitle" style="text-align:center"><asp:Button ID="add" Text="Add" runat="server" CommandName="Insert" />Actual</td><td colspan="2" class="Btitle" style="text-align:center">Loading</td>
                        <td colspan="12" class="Btitle" style="text-align:center">Forecast</td>
                    </tr>
                </InsertItemTemplate>
                <ItemTemplate>
                    <tr onmouseover='this.style.backgroundColor="#FFF6B5"'
                        onmouseOut='this.style.backgroundColor=""'>
                    <td colspan="2" align="right"><asp:Label ID="pn" runat="server" Text='<%# Eval("cus_part_no")%>' />
                    <asp:Image ID="img_detail" runat="server" />
                    </td>
                        <td class="pastPeriodParts"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[15])%></td>
                        <td class="pastPeriodParts"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[16])%></td>
                        <td class="pastPeriodParts"><%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[17])%></td>
                        <td class="loadPeriod"></td><td class="loadPeriod"></td>
                        <td>
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
                            <asp:TextBox CssClass="revenue" ID="ren1" runat="server" Text='<%# string.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)[3])%>' />
                        </td>
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

