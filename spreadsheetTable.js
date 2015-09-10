function sps(tableId,cupd)
{
    this.table = document.getElementById(tableId);
    this.isValid = false;
    this.tableColNo = 0;
    this.tableRowNo = 0;
    if(this.table != undefined)
    {
        this.isValid=true;
        if(this.table.parentNode)
            this.table.parentNode.style.visibility = "visible";
        this.tableRowNo = this.table.rows.length;
        this.tableColNo = this.table.rows[this.tableRowNo-1].cells.length;
    }
    this.currentPeriod=cupd;
    this.hideColumn = function(col){
        var tbl = this.table;
        for(var i = 0 ; i < this.tableRowNo; i++)
        {
            tbl.rows[i].cells[col].style.display = "none";
            tbl.rows[i].cells[col].style.width = "0px";
        }
        tbl.rows[0].style.backgroundColor = "#dddddd";
    }
    this.NumberCol = function(col){
        for(var i = 2; i < this.tableRowNo; i++){
            var cell = this.table.rows[i].cells[col];
            if(cell != undefined){
                var num = cell.innerHTML;
                cell.innerHTML = "";
                var div = pg.html.appendObject({"create":"div","param":{"innerHTML":num.addCommasToNumber(),"id":"tbl_"+i+"_"+col,
                                    "oriValue":num,"oem":this.table.rows[i].cells[0].innerText}},cell);
                if(i < this.tableRowNo-1){
                    div.onclick = inputFocus;
                }
            }
        }
    }
    var inputField;
    function inputBlur(){
        var div = this.parentNode;
        var val = this.value;
        div.removeChild(inputField);
        div.innerHTML = val;
        inputField = null;
    }
    function inputFocus(){
        if(inputField){
            var val = inputField.value;
            var p = inputField.parentNode;
            if(p != this){
                p.removeChild(inputField);
                inputField = null;
                p.innerHTML = val;
                //alert(p.parentNode.clientWidth);
                inputBuild(this);
            }
        }else
            inputBuild(this);
    }
    function inputBuild(div){
        inputField = null;
        inputField = document.createElement("input");
        inputField.onblur = inputBlur;
        inputField.type = "text";
        inputField.value = div.innerHTML;
        inputField.style.width = div.clientWidth-4;
        div.innerHTML = "";
        div.appendChild(inputField);
        inputField.focus();
    }
}

function spreadSheet(cupd,tableId)
{
    var sheet = new sps(tableId,cupd);
    if(sheet.isValid){
        //sheet.hideColumn(0);
        for(var i=1; i < sheet.tableColNo; i++)
            sheet.NumberCol(i);
    }
}

        if(document.getElementById(tableId))
        {
            sheet.table = document.getElementById(tableId);
            sheet.currentPeriod = cupd;

            document.getElementById("grid").style.visibility = "hidden";
            var num_cols = gv.rows[0].cells.length;
            var num_rows = gv.rows.length;
            hideFirstCol(gv,num_rows);
            for(var i = 2; i < num_cols; i++)
            {
                var heads = gv.rows[0].cells[i].innerHTML.split(" ");
                var p = heads[0];
                var isInput=false;
                gv.rows[0].cells[i].innerHTML = periodName(heads[0]);
                var cls = "amt";
                if(p> cupd)
                    isInput = true;
                else if(p == cupd)
                {
                    if(heads[1].indexOf("actual") >=0){
                        gv.rows[0].cells[i].innerHTML ="LOAD";
                        cls = "act";
                    }if(heads[1].indexOf("gap") >=0){
                        gv.rows[0].cells[i].innerHTML ="GAP";
                        cls = "gap";
                    }else if(new Date().getDate() < 14 && heads[1].indexOf("fcst") >=0)
                        isInput = true;
                }else
                {
                    if(heads[1].indexOf("fcst") <0){
                        gv.rows[0].cells[i].innerHTML = heads[1].trim().toUpperCase();
                        cls = "act";
                        if(heads[1].indexOf("gap") >=0)
                            cls = "gap";
                    }
                }
                for(var k = 1; k < num_rows; k++)
                {
                        var cell = gv.rows[k].cells[i];
                        cell.className = cls;//"amt";
                        var num = cell.innerText;
                        if(isInput && k < num_rows-1)
                        {
                            cell.innerHTML = "";
                            var inp = pg.html.appendObject({"create":"input","param":{"type":"text","value":num.addCommasToNumber(),"key":p,"id":"tbl_"+k+"_"+i,
                                "oriValue":num,"bkColor":"#000000","oem":gv.rows[k].cells[0].innerText}},cell);
                            inp.style.width = cell.clientWidth-4;
                            inp.onfocus = inputFocus;
                            inp.onblur = inputBlur;
                            inp.onkeyup = arrowCheck;
                        }else
                            cell.innerHTML = num.addCommasToNumber();
                }
            }
            document.getElementById("grid").style.visibility = "visible";
        }
