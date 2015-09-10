using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class CEMForecast : System.Web.UI.Page
{
    nUser Me;
    string[] k = { "Admin", "Management" };
    private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
        {
            Me = (nUser)Session["usr"];

            bool ch = false;
            if(SalesmanCtrl.Salesman.isCEMSales(Me.UID))
                ch = true;

            if(k.Contains(Me.uGroup) || Me.isAdmin)
            {
                ch = true;
                cem_upload_lnk.Visible = true;
            }
            if(!ch)
                Response.Redirect("default.aspx");
        }
        else
        {
            Response.Redirect("default.aspx");
        }

        if (!IsPostBack)
        {
            currentPeriod.Text = Multek.Util.getPeriodNBR(Forecast.currentPeriod());
            loadData();
        }
    }

    private void loadData()
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            string sql = "sp_gam_cem_forecast_view";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.Text;
            ds = sqldb.getDataSetCmd(ref cmd);
            cmd.Connection.Dispose();
            cmd.Dispose();
        }


        DataColumn[] OEM;
        DataColumn[] DATA;

        OEM = new DataColumn[] { ds.Tables[1].Columns["oem"], ds.Tables[1].Columns["plant"]};
        DATA = new DataColumn[] { ds.Tables[0].Columns["oem"], ds.Tables[0].Columns["plant"]};

        DataRelation drl = new DataRelation("myDataRelation", OEM, DATA,true);
        drl.Nested = true;
        ds.Relations.Add(drl);
        main.DataSource = ds.Tables[1];
        main.DataBind();
    }
    protected void list_DataBound(object sender, EventArgs e)
    {
        if (list.Items.Count > 0)
        {
            for (int i = 1; i <= 6; i++)
            {
                ((Label)list.FindControl("Label" + i.ToString())).Text = Multek.Util.getPeriodNBR(Forecast.currentPeriodAdd(i));
            }
        }
    }
    protected void main_DataBound(object sender, EventArgs e)
    {
        if (main.Items.Count > 0)
        {
            for (int i = 1; i <= 6; i++)
            {
                ((Label)main.FindControl("Label" + i.ToString())).Text = Multek.Util.getPeriodNBR(Forecast.currentPeriodAdd(i));
            }
        }

    }
    protected void main_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        //((Label)e.Item.FindControl("p1")).Text = "hello";
    }
}
