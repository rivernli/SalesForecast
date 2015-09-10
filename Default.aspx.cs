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

public partial class Default : System.Web.UI.Page
{
    nUser usr;
    string[] k = { "Admin", "Management" };
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
            usr = (nUser)Session["usr"];
        else
            Response.Redirect("logout.aspx");

        if (!IsPostBack)
        {
            System.Web.UI.Page pf = (System.Web.UI.Page)Context.Handler;
            //ViewState["isAdmin"] = usr.isAdmin;
            msg.Text = "Welcome " + usr.userName + " to GAM/BDM forecast";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(loadpage);", true);
            currentPeriod.Text = Forecast.currentPeriod().ToString().Trim();
            setYear();
        }
    }
    private void setYear()
    {
        int x = Convert.ToInt32(currentPeriod.Text.Substring(0, 4));
        int y = Convert.ToInt32(currentPeriod.Text.Substring(4));
        y = y - 3;
        if (y < 0)
        {
            y += 12;
            x--;
        }
        int y2 = y+6;
        int x2 = x;
        if (y2 > 12)
        {
            y2 -= 12;
            x2++;
        }

        startFY.Items.Clear();
        endFY.Items.Clear();
        int cM = DateTime.Today.Month;
        int endYear = DateTime.Today.Year + 1;
        if (cM > 3)
            endYear++;

        for (int i = 2010; i <= endYear; i++)
        {
            startFY.Items.Add(new ListItem("FY " +i.ToString(), i.ToString()));
            if (i == x)
                startFY.SelectedIndex = startFY.Items.Count - 1;
            endFY.Items.Add(new ListItem("FY " + i.ToString(), i.ToString()));
            if (i == x2)
                endFY.SelectedIndex = endFY.Items.Count-1;
        }
        //endFY.SelectedIndex = startFY.SelectedIndex = startFY.Items.Count - 2;
        startPeriod.Items.Clear();
        endPeriod.Items.Clear();

        string[] month = { "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar" };
        for (int j = 1; j <= 12; j++)
        {
            startPeriod.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
            if (y == j)
                startPeriod.SelectedIndex = j - 1;
            endPeriod.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
            if (y2 == j)
                endPeriod.SelectedIndex = j - 1;
        }
        //endPeriod.SelectedIndex = 11;// (cM > 11) ? 11 : cM + 1;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Collect();
    }
    private void Collect()
    {
        int sp = Convert.ToInt32(startFY.Text + "00") + Convert.ToInt32(startPeriod.Text);
        int ep = Convert.ToInt32(endFY.Text + "00") + Convert.ToInt32(endPeriod.Text);
        bool canBeView = false;
        //if (usr.isAdmin || usr.uGroup == "Admin")
        if (k.Contains(usr.uGroup) || usr.isAdmin)
            canBeView = true;
        GridView1.DataSource = Forecast.getForecastDataBySales2(sp, ep, canBeView ? 0 : usr.sysUserId);
        GridView1.DataBind();
    }
    private class editRow
    {
        public int cell_id;
        public int position;
        public int idate;
        public string type;
        public editRow(int cid, int pos, int dat, string typ)
        {
            cell_id = cid;
            position = pos;
            idate = dat;
            type = typ.ToLower();
        }
        public bool isEditable { get {
            if (position <= 0)
                return false;
            else
            {
                if (type.ToLower() != "fcst")
                    return false;
                return true;
            }
        } }
    }

       
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        Table tbl = (Table)GridView1.Controls[0];
        TableRow hd = tbl.Rows[0];
        int cudate = Convert.ToInt32(currentPeriod.Text);
        int cols = hd.Cells.Count;
        ArrayList al = new ArrayList();
        int pos = 1;
        for (int i = 3; i < cols; i++)
        {
            string dx = hd.Cells[i].Text;
            int idate = Convert.ToInt32(dx.Substring(0, 6));
            if (idate >= cudate)
            {
                if (dx.Substring(7) == "fcst")
                {
                    al.Add(new editRow(i - 3, pos, idate, dx.Substring(7)));
                    pos++;
                }
                else
                {
                    al.Add(new editRow(i - 3, pos - 1, idate, dx.Substring(7)));
                }
            }
            else
            {
                al.Add(new editRow(i - 3, 0, idate, dx.Substring(7)));
            }
        }
        
        GridViewRow row1 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        hd.CssClass = row1.CssClass = "header";
        for (int i = 0; i < cols; i++)
        {
            string xx = hd.Cells[i].Text;
            TableCell th = new TableHeaderCell();
            if (i >=3)
            {
                int idate = Convert.ToInt32(xx.Substring(0, 6));
                th.Text = Multek.Util.getPeriodNBR(idate);
                TDheadertoTDheaderDIV(hd.Cells[i], "Forecast");

                if (idate <= cudate)
                {
                    th.ColumnSpan = 3;

                    TDheadertoTDheaderDIV(hd.Cells[i + 1], "Actual");
                    TDheadertoTDheaderDIV(hd.Cells[i + 2], "Gap");
                    if (idate == cudate)
                    {
                        TDheadertoTDheaderDIV(hd.Cells[i + 1], "Loading");
                    }
                    hd.Cells[i + 1].Attributes.Add("class", "act");
                    hd.Cells[i].Attributes.Add("class", "amt");
                    hd.Cells[i + 2].Attributes.Add("class", "gap");
                    i = i + 2;
                }
                else
                {
                    hd.Cells[i].Style.Add("background-color", "#ffffff");
                }
            }
            else
            {
                th.Text = xx;
            }
            row1.Cells.Add(th);
        }
        
        row1.Cells[2].RowSpan = 2;
        hd.Cells.Remove(hd.Cells[2]);
        tbl.Rows.AddAt(0, row1);
        row1.Cells[0].Visible = row1.Cells[1].Visible = false;
        hd.Cells[1].Visible = hd.Cells[0].Visible = false;
        
        string salesman = "";
        int x = 1;
        foreach (GridViewRow row in GridView1.Rows)
        {
            row.Cells[0].Visible = row.Cells[1].Visible = false;
           
            if (salesman != row.Cells[0].Text)
            {
                salesman = row.Cells[0].Text;
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
                TableCell th = new  TableHeaderCell();
                th.HorizontalAlign = HorizontalAlign.Left;
                th.ColumnSpan = row.Cells.Count-2;
                th.Text = salesman;
                row2.Cells.Add(th);
                tbl.Rows.AddAt(row.RowIndex+x, row2);
            }
            if (row.RowType == DataControlRowType.DataRow)
            {
                row.Cells[2].HorizontalAlign = HorizontalAlign.Right;
             
                   
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.InnerHtml = row.Cells[1].Text;
                span.Attributes.Add("class", "hide");
                
                HtmlGenericControl span2 = new HtmlGenericControl("div");
                span2.InnerHtml = row.Cells[2].Text;
                //row.Cells[2].Controls.Add(span);
                //row.Cells[2].Controls.Add(span2); 
                   
                for (int i = 3; i < row.Cells.Count; i++)
                {
                    TableCell cell = row.Cells[i];
                    cell.Attributes.Add("id", "td_" + (row.RowIndex + x + 1).ToString() + "_" + (i - 2).ToString());
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.InnerHtml = Convert.ToInt32(cell.Text).ToString("#,##0");
                    cell.Text = "";
                    cell.Controls.Add(div);

                    editRow er = (editRow)al[i-3];
                    if (er.isEditable)
                    {
                        div.Attributes.Add("id", "tbl_" + row.RowIndex + "_" + er.position);
                        //if (span2.InnerHtml.ToLower() == "total")
                        if(row.Cells[2].Text.ToLower() == "total")
                        {
                            div.Attributes.Add("ed", "0");
                        }
                        else
                        {
                            //div.Attributes.Add("oemid", span.InnerHtml);
                            div.Attributes.Add("oemid", row.Cells[1].Text);
                            div.Attributes.Add("period", er.idate.ToString());
                            div.Attributes.Add("ed", "1");
                            div.Attributes.Add("onclick", "inputFocus(this)");
                        }
                    }
                    else
                    {
                        div.Attributes.Add("ed", "0");
                        div.Attributes.Add("id", er.type.Trim() + "_" + row.RowIndex + "_" + er.position);
                        switch (er.type)
                        {
                            case "gap":
                                cell.Attributes.Add("class", "gap");//background-color:#FED6FF");
                                break;
                            case "fcst":
                                cell.Attributes.Add("class", "amt");//"background-color:#DFFFD6");
                                //div.Attributes.Add("oemid", span.InnerHtml);
                                div.Attributes.Add("oemid", row.Cells[1].Text);
                                div.Attributes.Add("period", er.idate.ToString());
                                div.Attributes.Add("onclick", "inputFocus(this)");
                                break;
                            default:
                                cell.Attributes.Add("class", "act");
                                break;
                        }
                    }
                }
            }
        }
    }
    private void TDheadertoTDheaderDIV(TableCell cell, string headerText) 
    {
        cell.Text = "";
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.InnerHtml = headerText;
        cell.Controls.Add(div);
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
}
