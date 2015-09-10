<%@ Page Language="C#" MasterPageFile="~/gamReporting.master" AutoEventWireup="true" 
    Inherits="admini" Title="Admin Page" Codebehind="admini.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head2" Runat="Server">
    <style type="text/css">
        #default_page_panel {width:auto; max-width:882px; height:100%; margin:10px auto;  }
        #default_page_panel .dppA { float:left; width:200px; height:200px; text-align:center; padding:10px; display:block;}
        .dppB, .dppB2 { width:100%;height:100%; background-color:#FFF0A8; overflow:auto; transition: 0.6s; transform-style: preserve-3d;}
        .dppB:hover {background-color:#FFD800;-moz-transform:rotate(3deg);
-webkit-transform:rotate(4deg);
-o-transform:rotate(4deg);
-ms-transform:rotate(4deg);
transform:rotate(4deg);
box-shadow:4px 4px 3px rgba(20%,20%,40%,0.5);
        }
        .dppB2:hover {background-color:#FFD800;-moz-transform:rotate(3deg);
-webkit-transform:rotate(-3deg);
-o-transform:rotate(-3deg);
-ms-transform:rotate(-3deg);
transform:rotate(-3deg);
box-shadow:4px 4px 3px rgba(20%,20%,40%,0.5);        
        }
        .dppB3 { width:100%;height:100%; background-color:#dddddd; overflow:auto; opacity:0.4; filter:alpha(opacity=40); transition: 1s;transform-style: preserve-3d;}
        .dppB3:hover {background-color:#cccccc;}
        .dppC {top:50%; width:100%; height:50px; position:relative; margin-top:-25px; font-size:24px; font-weight:bold;}

    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
<div>


        <div id="default_page_panel">

            <div class="dppA"><div class="dppB"><div class="dppC">
                <asp:LinkButton ID="firstLock" runat="server" Text="Log Salesman (1st lock)" onclick="log_salesman_forecast_Click" />
            </div></div></div>
            
            <div class="dppA"><div class="dppB"><div class="dppC">
                <asp:LinkButton ID="download_current_forecast" runat="server" Text="Current Forecast" OnClick="download_current_forecast_Click" />
             </div></div></div>

            <div class="dppA"><div class="dppB"><div class="dppC">
                <asp:LinkButton ID="download_new_forecast" runat="server" Text="Download Forecast" OnClick="download_new_forecast_Click" />
            </div></div></div>
        
            <div class="dppA"><div class="dppB3"><div class="dppC">
                Forecast Output<br /> (Flat View)<br />
                <asp:Button ID="download_forecast_output" Enabled="false" runat="server" Text="download" onclick="download_forecast_output_Click" />
            </div></div></div>

            <div class="dppA"><div class="dppB3"><div class="dppC">
                Forecast Output<br /> (Row View)<br /><asp:Button ID="download_forecast_outpu_row"  Enabled="false" runat="server" Text="download" onclick="download_forecast_outpu_row_Click" />
            </div></div></div>

            <div class="dppA"><div class="dppB3"><div class="dppC">
                <asp:LinkButton ID="sync_oem_cem" Text="Sync OEM+CEM forecst" runat="server" Enabled="false" onclick="download_forecast_oem_cem_Click" />
            </div></div></div>

            <div class="dppA"><div class="dppB3"><div class="dppC">
                <asp:LinkButton ID="cells_fc" Text="Download FC by Cells" runat="server" Enabled="false" onclick="download_forecast_outpu_row0_Click" />
            </div></div></div>
            
            <div class="dppA"><div class="dppB3"><div class="dppC">
                <asp:LinkButton ID="LinkButton1" Text="Download FC by Rows" runat="server" Enabled="false" onclick="download_forecast_outpu_row1_Click" />
            </div></div></div>
            
            <div class="dppA"><div class="dppB3"><div class="dppC">
                <asp:LinkButton ID="b2fcell" runat="server" Text="B2F Cells" OnClick="Button2_Click" Enabled="false" />
            </div></div></div>

            <div class="dppA"><div class="dppB3"><div class="dppC">
                <asp:LinkButton ID="b2frow" runat="server" Text="B2F Rows" OnClick="Button1_Click" Enabled="false" />
            </div></div></div>

    </div>

</div>
</asp:Content>

