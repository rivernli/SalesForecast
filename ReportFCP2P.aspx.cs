using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class ReportFCP2P : System.Web.UI.Page
{
    private int sp { get { return Convert.ToInt32(FY1.SelectedValue) * 100 + Convert.ToInt32(Period1.SelectedValue); } }
    private int ep { get { return Convert.ToInt32(FY2.SelectedValue) * 100 + Convert.ToInt32(Period2.SelectedValue); } }
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
        loadingData();
    }
    private void loadingData()
    {
        downloadReport.Visible = false;
        string msg;
        DataSet ds = Reports.getRportFCP2P(sp, ep, out msg);
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count > 0)
            downloadReport.Visible = true;

        dt.Columns.Remove(dt.Columns["oemid"]);
        dt.Columns.Remove(dt.Columns["cusOEM"]);
        dt.Columns.Remove(dt.Columns["existsOEM"]);
        //dt.Columns.Remove(dt.Columns["userName"]);
        loadData2Grid(ds.Tables[1], gridPlant);
        loadData2Grid(ds.Tables[2], gridOEM);
        loadData2Grid(ds.Tables[3], gridSales);
        loadData2Grid(dt, grid);

        message.Font.Bold = true;
        message.Text = msg.Replace("\\n", "<br/>");

    }
    private void loadData2Grid(DataTable dt, GridView gv)
    {
        gv.Columns.Clear();
        foreach (DataColumn col in dt.Columns)
        {
            BoundField field = new BoundField();
            field.DataField = field.HeaderText = col.ColumnName;
            if (Multek.Util.IsNumeric(col))
            {
                if(col.ColumnName.ToLower().IndexOf("varp") > 0)
                    field.DataFormatString = "{0:P2}";
                else
                    field.DataFormatString = "{0:N0}";
                field.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                field.ItemStyle.Wrap = false;
            }
            gv.Columns.Add(field);
        }
        gv.DataSource = dt;
        gv.DataBind();
    }
    protected void downloadReport_Click(object sender, EventArgs e)
    {
        string msg;
        DataSet ds = Reports.getRportFCP2P(sp, ep, out msg);
        if (ds.Tables[0].Rows.Count > 0)
        {
            ds.Tables[0].Columns.Remove(ds.Tables[0].Columns["oemid"]);
            ds.Tables[0].Columns.Remove(ds.Tables[0].Columns["cusOEM"]);
            ds.Tables[0].Columns.Remove(ds.Tables[0].Columns["existsOEM"]);
            Multek.Util.DS2Excel(ds, "Forecast_P2P_var_result");
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

        Table tbl3 = (Table)gridOEM.Controls[0];
        GridViewRow row3 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        tbl3.Rows.AddAt(0, row3);
        TableRow hd3 = tbl3.Rows[1];
        
        Table tbl4 = (Table)gridSales.Controls[0];
        GridViewRow row4 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        tbl4.Rows.AddAt(0, row4);
        TableRow hd4 = tbl4.Rows[1];
        

        TableCell th2 = new TableHeaderCell();
        th2.Text = "";
        row2.Cells.Add(th2);

        TableCell th3 = new TableHeaderCell();
        th3.Text = "";
        row3.Cells.Add(th3);
        
        TableCell th4 = new TableHeaderCell();
        th4.Text = "";
        row4.Cells.Add(th4);
        
        for (int i = 0; i < 4; i++)
        {
            TableCell th = new TableHeaderCell();
            th.Text = "";
            row1.Cells.Add(th);
        }

        string[] bg = { "#000084", "#00327F" };
        int cols = hd.Cells.Count;
        int qcount = (cols - 4) / 5;
        for (int j = 0; j < qcount; j++)
        {
            TableCell th = new TableHeaderCell();
            th.ColumnSpan = 5;
            int x = 4 + j * 5;

            string xs = hd.Cells[x].Text;
            th.Text = xs.Substring(0, xs.IndexOf("_"));
            row1.Cells.Add(th);

            int L = hd.Cells[x + 1].Text.Length;
            string A = hd.Cells[x + 1].Text.Substring(L - 7);
            string B = hd.Cells[x + 2].Text.Substring(L - 7);

            hd.Cells[x].Text = "Actual<br/>Loading";
            hd.Cells[x + 1].Text = A + "<br/>Forecast";
            hd.Cells[x + 2].Text = B + "<br/>Forecast";
            hd.Cells[x + 3].Text = "Var";
            hd.Cells[x + 4].Text = "Var%";

            th.BackColor = 
            hd.Cells[x].BackColor =
                hd.Cells[x + 1].BackColor =
                hd.Cells[x + 2].BackColor =
                hd.Cells[x + 3].BackColor =
                hd.Cells[x + 4].BackColor =
             System.Drawing.ColorTranslator.FromHtml(bg[j % 2]);

            setRow(th.Text, row2, hd2, j, A, B);
            setRow(th.Text, row3, hd3, j, A, B);
            setRow(th.Text, row4, hd4, j, A, B);
        }
    }
    
    private void setRow(string title, GridViewRow row, TableRow tr, int j, string A, string B)
    {
        string[] bg = { "#000084", "#00327F" };

        TableHeaderCell th = new TableHeaderCell();
        th.ColumnSpan = 5;
        th.Text = title;
        row.Cells.Add(th);

        int y = 1 + j * 5;
        tr.Cells[y].Text = "Actual<br/>Loading";
        tr.Cells[y + 1].Text = A + "<br/>Forecast";
        tr.Cells[y + 2].Text = B + "<br/>Forecast";
        tr.Cells[y + 3].Text = "Var";
        tr.Cells[y + 4].Text = "Var%";

        tr.Cells[y].BackColor =
            tr.Cells[y + 1].BackColor =
            tr.Cells[y + 2].BackColor =
            tr.Cells[y + 3].BackColor =
            tr.Cells[y + 4].BackColor =
            th.BackColor = System.Drawing.ColorTranslator.FromHtml(bg[j % 2]);

    }
    protected void showPlant_Click(object sender, EventArgs e)
    {
        showButtonPanel(showPlant);
    }
    private void showButtonPanel(LinkButton btn)
    {
        showSales.Enabled = showPlant.Enabled = showAll.Enabled = showOEMs.Enabled = true;
        btn.Enabled = false;
        Plants.Visible = !showPlant.Enabled;
        OEMs.Visible = !showOEMs.Enabled;
        all.Visible = !showAll.Enabled;
        Sales.Visible = !showSales.Enabled;
        loadingData();
    }
    protected void showOEMs_Click(object sender, EventArgs e)
    {
        showButtonPanel(showOEMs);
    }
    protected void showSales_Click(object sender, EventArgs e)
    {
        showButtonPanel(showSales);
    }
    protected void showAll_Click(object sender, EventArgs e)
    {
        showButtonPanel(showAll);
    }
}
