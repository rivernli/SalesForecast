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
using System.Xml;
using System.Text.RegularExpressions;

public partial class MasterPage : System.Web.UI.MasterPage
{

    nUser Me;
    string[] k = { "Admin", "Management" };
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool isWA = false;
            defindMenu();
            if (Session["usr"] != null)
            {
                Me = (nUser)Session["usr"];
                isWA = Me.isAdmin;
            }
            else
                Response.Redirect("logout.aspx");

            if (!isWA)
            {
                config_acl.Visible = 
                config_view.Visible = 
                reporting_view.Visible = 
                adjustment_view.Visible = false;

                if (k.Contains(Me.uGroup))
                {
                    report_sales.Visible =
                    report_download.Visible = 
                    config_view.Visible = 
                    reporting_view.Visible = 
                    adjustment_view.Visible = true;
                    if (Me.uGroup != "Admin")
                        config_view.Visible =  false;
                }
                else
                {
                    reporting_view.Visible = Me.isReportViewer;
                    report_download.Visible = report_sales.Visible = false;
                }
            }
        }

    }
    private void defindMenu()
    {
        string p = Parent.Page.Request.Path.ToLower();
        MatchCollection matchs = Regex.Matches(p, "/(?<key>[a-z0-9_]*).aspx");
        if (matchs.Count > 0)
        {
            GroupCollection gc = matchs[0].Groups;
            p = gc["key"].Value;
        }

        string s = "selected";
        switch (p)
        {
            case "default":
                home.Attributes["class"] = s;
                break;
            case "oem_control":
                configuration.Text = "Config " + config_oem.Text;
                config_oem.Font.Italic = true;
                configuration.Attributes["class"] = s;
                break;
            case "baan_oem_control":
                configuration.Text = config_baan.Text;
                config_baan.Font.Italic = true;
                configuration.Attributes["class"] = s;
                break;
            case "salesmanrelation":
                configuration.Text = "Conf. " + config_salesman.Text;
                config_salesman.Font.Italic = true;
                configuration.Attributes["class"] = s;
                break;
            case "acl":
                configuration.Text = "Config " + config_acl.Text;
                config_acl.Font.Italic = true;
                configuration.Attributes["class"] = s;
                break;
            case "salesmenforecast6":
                forecast.Text = forecast_sales.Text;
                forecast_sales.Font.Italic = true;
                forecast.Attributes["class"] = s;
                break;
            case "cemforecast":
            case "cemforecastuploadview":
                forecast.Text = forecast_cem.Text;
                forecast_cem.Font.Italic = true;
                forecast.Attributes["class"] = s;
                break;
            case "adjustmenttopside":
                adjustment.Attributes["class"] = s;
                break;

            case "reportfcp2p":
                reporting.Text = report_forecast.Text;
                report_forecast.Font.Italic = true;
                reporting.Attributes["class"] = s;
                break;
            case "reportb2fview":
                reporting.Text = report_b2f.Text;
                report_b2f.Font.Italic = true;
                reporting.Attributes["class"] = s;
                break;
            case "admini":
                reporting.Text = report_download.Text;
                report_download.Font.Italic = true;
                reporting.Attributes["class"] = s;
                break;
            case "salesmanforecastsubmit":
                reporting.Text = report_sales.Text;
                report_sales.Font.Italic = true;
                reporting.Attributes["class"] = s;
                break;
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Session.Abandon();
        FormsAuthentication.SignOut();
        Response.Redirect("default.aspx");

    }
}
