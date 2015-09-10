using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ReportB2FView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string cp = Forecast.currentPeriod().ToString().Trim();
            currentPeriod.Text = cp;
            loadPeriodList(cp);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(loadpage);", true);

        }
    }
    private void loadPeriodList(string currentPeriod)
    {
        int cM = DateTime.Today.Month;
        int endYear = DateTime.Today.Year;
        if (cM > 3)
            endYear++;
        string[] month = { "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar" };


        int x = Convert.ToInt32(currentPeriod.Substring(0, 4));
        int y = Convert.ToInt32(currentPeriod.Substring(4));

        FY1.Items.Clear();
        for (int i = 2014; i <= endYear; i++)
        {
            FY1.Items.Add(new ListItem("FY " + i.ToString(), i.ToString()));
            if (i == x)
            {
                FY1.SelectedIndex = FY1.Items.Count - 1;
            }
        }

        Period1.Items.Clear();
        for (int j = 1; j <= 12; j++)
        {
            Period1.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
        }
        Period1.SelectedIndex = y - 1;
    }

    protected void go_Click(object sender, EventArgs e)
    {

        int sp = Convert.ToInt32(FY1.SelectedValue) * 100 + Convert.ToInt32(Period1.SelectedValue);
        DataTable dt = Reports.getB2FResult(sp);
        if (dt.Rows.Count > 0)
        {
            resultGrid.Visible = downloadResult.Visible = true;
            resultGrid.DataSource = dt;
            resultGrid.DataBind();
        }
        else
        {
            resultGrid.Visible = downloadResult.Visible = false;
        }
        dt.Dispose();

    }
    protected void downloadResult_Click(object sender, EventArgs e)
    {
                int sp = Convert.ToInt32(FY1.SelectedValue) * 100 + Convert.ToInt32(Period1.SelectedValue);
        DataTable dt = Reports.getB2FResult(sp);
        if (dt.Rows.Count > 0)
        {
            Multek.Util.DT2Excel(dt, "B2F_result");
            dt.Dispose();
        }
    }
}
