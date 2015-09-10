using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BISSOGO();
        }

    }
    private void BISSOGO()
    {
        Multek.com.multek.bi.SSOAuthen SSO = new Multek.com.multek.bi.SSOAuthen();
        if (Request.QueryString["token"] == null)
        {
            Response.Redirect(SSO.getSSOLoginPage(Server.UrlEncode(Request.Url.ToString())));
        }
        else
        {
            if (Request.QueryString["pair"] != null && Request.QueryString["token"] != null)
            {
                string a = SSO.testAuthentication(Request.QueryString["token"].ToString(), Request.QueryString["pair"].ToString());
                if (a == "failed" || a == "expired" || a == "")
                {
                    Label5.Text = "failed to access this application!";
                }
                else
                {
                    string[] Users;
                    Users = a.Trim().Split('\\');
                    if (Users.Length == 2)
                    {
                        securityChecking(Users[0], Users[1]);
                    }
                    else
                        Label5.Text = "unknow";
                }
            }
        }
        SSO.Dispose();
    }

    private void securityChecking(string userId, string domain)
    {
        nUser usr = new nUser(userId, domain);
        if (usr.isLdapUser)
        {
            if (usr.isWebAdmin())
            {
                usr.managerId = 0;
                usr.isSales = false;
                usr.isActive = usr.isAdmin = usr.isReportViewer = true;
                if (!usr.isDBuser)
                {
                    if (usr.addDBUser())
                    {
                       usr.Dispose();
                       securityChecking(userId, domain);
                       return;
                    }                    
                }
                else if (!usr.isActive)
                {
                    usr.updateDBUser();
                }
            }
            if (usr.isDBuser && usr.isActive)
            {
                Session["usr"] = usr;
                FormsAuthentication.RedirectFromLoginPage(usr.domain + "/" + usr.UID, false);
                if (Request.QueryString["ReturnUrl"] == null)
                {
                    if (Request.UrlReferrer != null)
                        Response.Redirect(Request.UrlReferrer.ToString());
                    else
                        Response.Redirect("default.aspx");
                }
                else
                    Response.Redirect(Request.QueryString["ReturnUrl"].ToString());

            }
            
        }
        else
        {
            Label5.Text = "Login Failed";
        }

    }
}
