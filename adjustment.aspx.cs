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

public partial class adjustment : System.Web.UI.Page
{
    nUser Me;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
        {
            Me = (nUser)Session["usr"];
            if (!Me.isAdmin)
                Response.Redirect("default.aspx");
            //isWebAdmin = Me.isWebAdmin();
        }
        else
        {
            Response.Redirect("default.aspx");
        }
        if (!IsPostBack)
        {
            Page.RegisterStartupScript("init", "<script>onload()</script>");
            loadGrid();
        }
    }
    private void loadGrid()
    {
        int list_by = Convert.ToInt16(listedBy.SelectedValue);
        DataTable dt = Forecast.getAdjustFC(list_by);
        
        DataTable dtSales = dt.DefaultView.ToTable(true, new string[] { "salesman" });
        dtSales.TableName = "sales";
        dt.TableName = "FC";
        DataSet ds = new DataSet("ds");
        ds.Tables.Add(dt);
        ds.Tables.Add(dtSales);
        DataRelation drl = new DataRelation("SalesOEM", ds.Tables["sales"].Columns["salesman"],
            ds.Tables["FC"].Columns["salesman"], true);
        drl.Nested = true;
        ds.Relations.Add(drl);
        salesmanList.DataSource = ds.Tables["sales"];
        salesmanList.DataBind();
    }
    protected void listedBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        loadGrid();
    }
}
