using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class ReportCompareFCbyPeriod : System.Web.UI.Page
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
        //currentPeriod = "201312";
        int cM = DateTime.Today.Month;
        int endYear = DateTime.Today.Year;
        if (cM > 3)
            endYear++;
        string[] month = { "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar" };


        int x = Convert.ToInt32(currentPeriod.Substring(0, 4));
        int y = Convert.ToInt32(currentPeriod.Substring(4));
        int p2 = y - 2;

        FY1.Items.Clear();
        FY2.Items.Clear();
        for (int i = 2012; i <= endYear; i++)
        {
            FY1.Items.Add(new ListItem("FY " + i.ToString(), i.ToString()));
            FY2.Items.Add(new ListItem("FY " + i.ToString(), i.ToString()));
            if (i == x)
            {
                FY1.SelectedIndex = FY1.Items.Count - 1;
                if (y == 1)
                {
                    FY2.SelectedIndex = FY2.Items.Count - 2;
                    p2 = 11;
                }
                else
                    FY2.SelectedIndex = FY2.Items.Count - 1;
            }
        }

        Period1.Items.Clear();
        for (int j = 1; j <= 12; j++)
        {
            Period1.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
            Period2.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
        }
        Period1.SelectedIndex = y - 1;
        Period2.SelectedIndex = p2;
        
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        downloadReport.Visible = false;
        string msg;
        int sp = Convert.ToInt32(FY1.SelectedValue) * 100 + Convert.ToInt32(Period1.SelectedValue);
        int ep = Convert.ToInt32(FY2.SelectedValue) * 100 + Convert.ToInt32(Period2.SelectedValue);

        DataSet ds = Reports.getRportCompareP2P(sp, ep, out msg);
        DataTable dt = ds.Tables[0];// Reports.getReportComparePeriodByPeriod(sp, ep, out msg);
        if (dt.Rows.Count > 0)
            downloadReport.Visible = true;

        dt.Columns.Remove(dt.Columns["oemid"]);
        dt.Columns.Remove(dt.Columns["existsOEM"]);
        gridPlant.DataSource = ds.Tables[1];
        gridPlant.DataBind();
        grid.DataSource = dt;
        grid.DataBind();
        message.Font.Bold = true;
        message.Text = msg.Replace("\\n","<br/>");
    }
    protected void downloadReport_Click(object sender, EventArgs e)
    {
        string msg;
        int sp = Convert.ToInt32(FY1.SelectedValue) * 100 + Convert.ToInt32(Period1.SelectedValue);
        int ep = Convert.ToInt32(FY2.SelectedValue) * 100 + Convert.ToInt32(Period2.SelectedValue);

        DataSet ds = Reports.getRportCompareP2P(sp, ep, out msg);

        DataTable dt = ds.Tables[0];// Reports.getReportComparePeriodByPeriod(sp, ep, out msg);
        dt.Columns.Remove(dt.Columns["oemid"]);
        dt.Columns.Remove(dt.Columns["existsOEM"]);
        if (dt.Rows.Count > 0)
        {
            Multek.Util.DT2Excel(dt, "Compare_FC_P2P");
        }
    }
    protected void grid_DataBinding(object sender, EventArgs e)
    {
        Table tbl = (Table)grid.Controls[0];
        GridViewRow row1 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        tbl.Rows.AddAt(0, row1);
        TableRow hd = tbl.Rows[1];

        Table tbl2 = (Table)gridPlant.Controls[0];
        GridViewRow row2 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        tbl2.Rows.AddAt(0, row2);
        TableRow hd2 = tbl2.Rows[1];

        TableCell th2 = new TableHeaderCell();
        th2.Text = "";
        row2.Cells.Add(th2);


        for (int i = 0; i < 3; i++)
        {
            TableCell th = new TableHeaderCell();
            th.Text = "";
            row1.Cells.Add(th);
        }
        string[] bg = { "#000084", "#00327F" };
        int cols = hd.Cells.Count;
        int qcount = (cols - 3) / 7;
        for (int j = 0; j < qcount; j++)
        {
            TableCell th = new TableHeaderCell();
            th.ColumnSpan = 7;
            int x = 3 + j * 7;

            string xs = hd.Cells[x].Text;
            th.Text = xs.Substring(0, xs.IndexOf("_"));
            row1.Cells.Add(th);
            int L = hd.Cells[x + 1].Text.Length;
            string A = hd.Cells[x + 1].Text.Substring(L - 7);
            string B = hd.Cells[x + 2].Text.Substring(L - 7);
            hd.Cells[x].Text = "Actual<br/>Loading";
            hd.Cells[x + 1].Text = A + "<br/>Forecast";
            hd.Cells[x + 2].Text = B + "<br/>Forecast";
            hd.Cells[x + 3].Text = A + "<br/>OLS";
            hd.Cells[x + 4].Text = B + "<br/>OLS";
            hd.Cells[x + 5].Text = A + "<br/>Top Side";
            hd.Cells[x + 6].Text = B + "<br/>Top Side";

            TableHeaderCell th3 = new TableHeaderCell();
            th3.ColumnSpan = 3;
            th3.Text = th.Text;
            row2.Cells.Add(th3);
            int y = 1 + j * 3;
            hd2.Cells[y].Text = "Actual<br/>Loading";
            hd2.Cells[y + 1].Text = A + "<br/>Forecast";
            hd2.Cells[y + 2].Text = B + "<br/>Forecast";
            hd.Cells[x].BackColor = 
                hd.Cells[x + 1].BackColor = 
                hd.Cells[x + 2].BackColor = 
                hd.Cells[x + 3].BackColor =
                hd.Cells[x + 4].BackColor = 
                hd.Cells[x + 5].BackColor = 
                hd.Cells[x + 6].BackColor =
            hd2.Cells[y].BackColor = hd2.Cells[y+1].BackColor = hd2.Cells[y+2].BackColor = 
            th3.BackColor = th.BackColor = System.Drawing.ColorTranslator.FromHtml(bg[j % 2]);
        }
        /*
        foreach (TableCell cell in hd.Cells)
        {
            if (cell.Text.IndexOf("_") > 0)
                cell.Text = cell.Text.Replace("_", "<br/>");

        }*/
    }
    protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void gridPlant_DataBound(object sender, EventArgs e)
    {
    }
    protected void gridPlant_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
}
