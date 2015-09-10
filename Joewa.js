 function PostHttpRequest(url,sendDoc,resultFunction){
	    var HttpRequest = createXMLObject();
	    if(HttpRequest){
		    HttpRequest.onreadystatechange = function(){
    		    if(HttpRequest.readyState==4){if(HttpRequest.status==200){
				    eval(resultFunction +'(HttpRequest)');
			    }}
		    }
		    HttpRequest.open("POST",url,true);
		    HttpRequest.setRequestHeader("Content-Type","text/xml; charset=utf-8");
		    HttpRequest.send(sendDoc);
	    }
    }
    function createXMLObject(){
		var xObj;
		if(window.XMLHttpRequest) {
			try {
				xObj = new XMLHttpRequest();
			} catch(e) {
				xObj = false;
			}
		} else if(window.ActiveXObject) {
			try {
				xObj = new ActiveXObject("Msxml2.XMLHTTP");
			} catch(e) {
				try {
					xObj = new ActiveXObject("Microsoft.XMLHTTP");
				} catch(e) {
					xObj = false;
				}
			}
		}
		return xObj;
	}
	
	
	var joewa = {}
	joewa.RegexpMatch = function(testingString,patten)
	{
	    var output = testingString.match(patten);
        if(output !=null)
        {
            return output[1];
        }
        return "";
	}
	joewa.JoinKeyAndElement = function(ArrayList)
	{   var RS = "";
	    for(var key in ArrayList)
	    {
	        try{
	            RS += key + escape(document.getElementById(ArrayList[key]).value);
	        }catch(Error){
	            RS += "";
	        }
	    }
	    return RS;
	}
	joewa.requestStringArray = function(loc)
	{
		var query = loc.search.substring(1);
		var vars = query.split("&");
		var ary = new Array();
		for (var i=0;i<vars.length;i++) {
			var pair = vars[i].split("=");
			ary[pair[0]] =part[1];
		}
		return ary;
	}
	
	
	var training = {}
	training.goBackModulePage = function()
	{
	    var q = window.location.search.substring(1); 
	    window.location.href = "default.aspx#"+ q.replace(/&/g,";")+";";
	}
