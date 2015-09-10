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


public partial class adjustmentTopSide : System.Web.UI.Page
{
    nUser usr;
    string[] k = { "Admin", "Management" };
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
            usr = (nUser)Session["usr"];
        else
            Response.Redirect("default.aspx");
        if(!k.Contains(usr.uGroup))
            Response.Redirect("default.aspx");

        if (!IsPostBack)
        {
            //ViewState["isAdmin"] = true;
            searchOEM.Attributes.Add("onkeydown", "return (event.keyCode!=13)");
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(loadpage);", true);
            setYear();
            currentPeriod.Text = Forecast.currentPeriod().ToString().Trim();
        }
    }
    private void setYear()
    {
        startFY.Items.Clear();
        endFY.Items.Clear();

        int cM = DateTime.Today.Month;
        int endYear = DateTime.Today.Year + 1;
        if (cM > 4)
            endYear++;

        for (int i = 2010; i <= endYear; i++)
        {
            startFY.Items.Add(new ListItem("FY"+i.ToString(), i.ToString()));
            endFY.Items.Add(new ListItem("FY" + i.ToString(), i.ToString()));
        }
        //endFY.SelectedIndex = startFY.SelectedIndex = startFY.Items.Count - 2;
        startPeriod.Items.Clear();
        endPeriod.Items.Clear();
        string[] month = { "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar" };
        for (int j = 1; j <= 12; j++)
        {
            startPeriod.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
            endPeriod.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
        }
        //endPeriod.SelectedIndex = 11;


        string nextPeriod = Forecast.currentPeriod().ToString().Trim();
        //nextPeriod = "201311";
        int x = Convert.ToInt32(nextPeriod.Substring(0, 4));
        startFY.Items.FindByValue(x.ToString()).Selected = true;
        int y = Convert.ToInt32(nextPeriod.Substring(4));
        startPeriod.Items.FindByValue(y.ToString()).Selected = true;
        int y2 = y + 6;
        if (y2 > 12)
        {
            y2 = y2 - 12;
            x++;
        }
        endFY.Items.FindByValue(x.ToString()).Selected = true;
        endPeriod.Items.FindByValue(y2.ToString()).Selected = true;

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        int sp = Convert.ToInt32(startFY.Text+"00") + Convert.ToInt32(startPeriod.Text);
        int ep = Convert.ToInt32(endFY.Text + "00") + Convert.ToInt32(endPeriod.Text);
        string oem = searchOEM.Text.Trim();
        int uid = (usr.isAdmin || k.Contains(usr.uGroup)) ? 0 : usr.sysUserId;
        GridView1.DataSource = Forecast.getForecastDataBySalesForAdjust(sp, ep, uid, oem);
        GridView1.DataBind();

    }
      
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        if (GridView1.Rows.Count <= 0)
            return;

        Page.RegisterStartupScript("init", "<script>loadpage()</script>");
        
        
        Table tbl = (Table)GridView1.Controls[0];
        TableRow hd = tbl.Rows[0];
        #region header title set up
        GridViewRow row1 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        row1.CssClass=  hd.CssClass = "header";

        int cols = hd.Cells.Count;
        bool[] clickEvent = new bool[cols];
        string[] fieldCss = new string[cols];
        for (int i = 0; i < cols; i++)
        {
            clickEvent[i] = false;
            fieldCss[i] = "normal";
            string xx = hd.Cells[i].Text;
            int idx = i - 2;
            TableCell th = new TableHeaderCell();
            if (i + 3 < cols && hd.Cells[i + 3].Text.IndexOf("actual") > 0)
            {
                string p = xx.Substring(0, 6);


                th.Text = Multek.Util.getPeriod(Convert.ToInt32(p), false);
                th.ColumnSpan = 5;
                TDheadertoTDheaderDIV(hd.Cells[i], "Fcst"); fieldCss[i] = "amt";
                TDheadertoTDheaderDIV(hd.Cells[i + 1], "OLS", p); fieldCss[i + 1] = "ols_over";
                hd.Cells[i + 1].CssClass = "ols_header";
                TDheadertoTDheaderDIV(hd.Cells[i + 2], "TopSide", p);fieldCss[i + 2] = "topside_over";
                hd.Cells[i + 2].CssClass = "topside_header";
                TDheadertoTDheaderDIV(hd.Cells[i + 3], "Actual"); fieldCss[i + 3] = "act";
                TDheadertoTDheaderDIV(hd.Cells[i + 4], "Gap"); fieldCss[i + 4] = "gap";
                i = i + 4;
            }
            else
            {
                if (xx.IndexOf("fcst") > 0)
                {
                    string p = xx.Substring(0, 6);
                    th.Text = Multek.Util.getPeriod(Convert.ToInt32(p), false);
                    clickEvent[i + 1] = clickEvent[i + 2] = true;
                    th.ColumnSpan = 3;
                    TDheadertoTDheaderDIV(hd.Cells[i], "Fcst"); fieldCss[i] = "amt";
                    TDheadertoTDheaderDIV(hd.Cells[i + 1], "OLS", p); fieldCss[i + 1] = "ols";
                    hd.Cells[i + 1].CssClass = "ols_header";
                    TDheadertoTDheaderDIV(hd.Cells[i + 2], "TopSide", p); fieldCss[i + 2] = "topside";
                    hd.Cells[i + 2].CssClass = "topside_header";
                    i = i + 2;
                }
                else
                {
                    th.Text = "";
                    hd.Cells[i].Text = xx;
                }
            }
            row1.Cells.Add(th);
        }
        tbl.Rows.AddAt(0, row1);
        #endregion
        string salesman = "";
        int x = 1;

        row1.Cells[0].Visible = row1.Cells[1].Visible = hd.Cells[0].Visible = hd.Cells[1].Visible = false;
        bool isTotal = false;
        foreach (GridViewRow row in GridView1.Rows)
        {
            row.Cells[0].Visible = row.Cells[1].Visible = false;
            #region salesman row
            if (salesman != row.Cells[0].Text)
            {
                salesman = row.Cells[0].Text;
                if (salesman.ToUpper() == "ALL")
                {
                    isTotal = true;
                    row.Attributes.Add("class", "footer");
                    row.Cells[2].Font.Bold = true;
                }
                else
                {
                    x++;
                    GridViewRow row2 = new GridViewRow(0, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                    row2.Attributes.Add("class", "salesmanTR");
                    TableCell td0 = new TableCell();
                    td0.Text = "";
                    row2.Cells.Add(td0);
                    TableCell td1 = new TableCell();
                    td1.Text = "0";
                    row2.Cells.Add(td1);
                    td0.Visible = td1.Visible = false;

                    TableCell th = new TableHeaderCell();
                    th.HorizontalAlign = HorizontalAlign.Left;
                    th.ColumnSpan = row.Cells.Count - 2;
                    th.Text = salesman;
                    row2.Cells.Add(th);
                    tbl.Rows.AddAt(row.RowIndex + x, row2);
                }
            }
            #endregion
            
            #region data
            if (row.RowType == DataControlRowType.DataRow)
            {
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.InnerHtml = row.Cells[1].Text;
                span.Attributes.Add("class", "hide");
                HtmlGenericControl span2 = new HtmlGenericControl("div");
                span2.InnerHtml = row.Cells[2].Text;
                row.Cells[2].Controls.Add(span);
                row.Cells[2].Controls.Add(span2);
                if (salesman.ToUpper() != "ALL")
                {
                    HtmlImage img = new HtmlImage();
                    img.Src = "http://bi.multek.com/ws/images/ask_blue.png";
                    span2.Controls.Add(img);
                    span2.Attributes.Add("onclick", "showOEMComments('" + row.Cells[1].Text + "','" + row.Cells[2].Text + "')");
                    span2.Attributes.CssStyle.Add("cursor", "pointer");
                }
                for (int i = 3; i < row.Cells.Count; i++)
                {
                    #region x
                    TableCell cell = row.Cells[i];
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.InnerHtml = Convert.ToInt32(cell.Text).ToString("#,##0");
                    cell.Text = "";
                    cell.CssClass = fieldCss[i];
                    if (fieldCss[i] == "ols" && !isTotal)
                    {
                        cell.Attributes.Add("onclick", "mod(this,'ols')");
                    }
                    if (fieldCss[i] == "topside" && !isTotal)
                    {
                        cell.Attributes.Add("onclick", "mod(this,'topside')");
                    }
                    cell.Controls.Add(div);
                    #endregion x
                }
            }
            #endregion data
            
        }
    }
    private void TDheadertoTDheaderDIV(TableCell cell, string headerText) 
    {
        cell.Text = "";
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.InnerHtml = headerText;
        div.Style.Add("width", "80px");
        cell.Controls.Add(div);
    }
    private void TDheadertoTDheaderDIV(TableCell cell, string headerText, string period)
    {
        cell.Text = "";
        HtmlGenericControl hidediv = new HtmlGenericControl("div");
        hidediv.Attributes.Add("style", "display:none");
        hidediv.InnerHtml = period;
        cell.Controls.Add(hidediv);

        HtmlGenericControl div = new HtmlGenericControl("div");
        div.InnerHtml = headerText;
        div.Style.Add("width", "80px");
        cell.Controls.Add(div);
    }
}
