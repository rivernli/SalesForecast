using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;

public partial class gamReporting : System.Web.UI.MasterPage
{
    string[] k = { "Admin", "Management" };
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Session["usr"] != null)
            {
                nUser Me = (nUser)Session["usr"];
                if(!k.Contains(Me.uGroup) && !Me.isAdmin && !Me.isReportViewer)
                    Response.Redirect("default.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }

        }
    }
}
