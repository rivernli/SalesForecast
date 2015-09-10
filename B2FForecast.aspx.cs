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
using System.Xml;
using System.Data.SqlClient;
using System.Text;
using System.IO;

public partial class B2FForecast : System.Web.UI.Page
{
    private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    private DataTable dt;
    nUser Me;
    string[] k = { "Admin", "Management" };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
        {
            Me = (nUser)Session["usr"];
            bool pass = false;
            if (Me.isAdmin || k.Contains(Me.uGroup))
                pass = true;
            if(Me.UID.ToLower() == "mcnhwong" || Me.UID.ToLower() == "mcnthuan")
                pass = true;

            if (!pass)
                Response.Redirect("default.aspx");
        }
        else
        {
            Response.Redirect("default.aspx");
        }

        if (!IsPostBack)
        {
            currentPeriod.Text = Multek.Util.getPeriodNBR(Forecast.currentPeriod());
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(finish_upload);", true);
            loadExistsData();
            uploadMessage.Text = "Uplaod B2F forecast for " + currentPeriod.Text + ".";
            previewMessage.Text = "Preview B2F forecast for " + currentPeriod.Text + ".";
        }
    }

    private void loadExistsData()
    {
        displayNormal();
        DataTable dt = new DataTable();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            string sql = "sp_gam_b2f_display_data";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        ListViewDisplay.DataSource = dt;
        ListViewDisplay.DataBind();
        if (dt.Rows.Count > 0)
        {
            uploadAgain.Text = "Clear existing " + currentPeriod.Text + " forecast and upload again.";

        }
        else
        {
            //uploadAgain.Text = "Upload B2F forecast for " + currentPeriod.Text +".";
            displayUpload();
        }
    }
    private void displayNormal()
    {
        ResultArea.Visible = previewArea.Visible = uploadArea.Visible = false;
        DisplayArea.Visible = true;
    }
    private void displayUpload()
    {
        uploadArea.Visible = true;
        ResultArea.Visible = DisplayArea.Visible = false;
        cem_content.Text = "";
        listMain.DataSource = new DataTable();
        listMain.DataBind();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        tmp.Text = "";
        confirmUpload.Enabled = false;
        if (cem_content.Text.Trim() != "")
        {
            string txt = Server.HtmlEncode(cem_content.Text.Trim());
            setContent2DT(txt);
            uploadArea.Visible = false;
            previewArea.Visible = confirmUpload.Enabled = true;
            if (listMain.Items.Count == 0)
            {
                tmp.Text += "<br />Upload Content error";
                confirmUpload.Enabled = false;
            }
        }
        else
            tmp.Text = "Blank Content uploaded";
    }

    private Hashtable Month2Period()
    {
        Hashtable m2p = new Hashtable();
        m2p.Add("Jan", "10");
        m2p.Add("Feb", "11");
        m2p.Add("Mar", "12");
        m2p.Add("Apr", "01");
        m2p.Add("May", "02");
        m2p.Add("Jun", "03");
        m2p.Add("Jul", "04");
        m2p.Add("Aug", "05");
        m2p.Add("Sep", "06");
        m2p.Add("Oct", "07");
        m2p.Add("Nov", "08");
        m2p.Add("Dec", "09");
        return m2p;
    }
    private void setContent2DT(string content)
    {

        string[] lines = content.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        string[] header = lines[0].Split(new char[] { '\t' });
        tmp.Text = "";
        if (header.Length != 11)
        {
            tmp.Text = "fields not match to requirements";
            listMain.DataSource = new DataTable();
            listMain.DataBind();
            return;
        }
        Hashtable m2p = Month2Period();
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("period", System.Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("sales", System.Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("oem", System.Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("cpn", System.Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("ipn", System.Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("sqft", System.Type.GetType("System.Double")));
        dt.Columns.Add(new DataColumn("qty", System.Type.GetType("System.Int32")));
        dt.Columns.Add(new DataColumn("array", System.Type.GetType("System.Int32")));
        dt.Columns.Add(new DataColumn("smt", System.Type.GetType("System.Double")));
        dt.Columns.Add(new DataColumn("fpc", System.Type.GetType("System.Double")));
        dt.Columns.Add(new DataColumn("bom", System.Type.GetType("System.Double")));

        int initPos = 0;
        int ttlPos = lines.Length;
        if (lines[0].ToLower().IndexOf("forecast period") >= 0)
            initPos = 1;

        //foreach (string line in lines)
        for(int i= initPos; i < ttlPos; i++)
        {
            string[] item = lines[i].Split(new char[] { '\t' });
            /*
            int INT;
            if (int.TryParse(item[0].Trim(), out INT))
            {
            }
            */
            string[] p = item[0].Trim().Split(new char[] { '-' });
            string period = item[0];
            if (m2p.Contains(p[0]))
            {
                string ps = m2p[p[0]].ToString();
                if (Convert.ToInt32(ps) < 10)
                    p[1] = (Convert.ToInt32(p[1]) + 1).ToString();
                // if(p[1].length ==1) p[1] = "0" + p[1];
                period = "20" + p[1] + m2p[p[0]].ToString();// +item[0];
               
            }

            if (item.Length ==11)
            {
                //tmp.Text += item[0].Trim() +"<br/>";
                DataRow row = dt.NewRow();
                row[0] = period;// item[0].Trim();
                row[1] = item[1].Trim();
                row[2] = item[2].Trim();
                row[3] = item[3].Trim();
                row[4] = item[4].Trim();
                row[5] = getDouble(item[5].Trim());
                row[6] = getInt(item[6].Trim());
                row[7] = getInt(item[7].Trim());
                row[8] = getDouble(item[8].Trim());
                row[9] = getDouble(item[9].Trim());
                row[10] = getDouble(item[10].Trim());
                dt.Rows.Add(row);
            }
        }
        listMain.DataSource = dt;
        listMain.DataBind();

    }
    private int getInt(string s)
    {
        int i = 0;
        int.TryParse(s, out i);
        return i;
    }
    private double getDouble(string s)
    {
        double d = 0;
        double.TryParse(s, out d);
        return d;
    }
    protected void cancelUpload_Click(object sender, EventArgs e)
    {
        uploadArea.Visible = true;
        previewArea.Visible = false;
        tmp.Text = "";
    }
    protected void confirmUpload_Click(object sender, EventArgs e)
    {
        tmp.Text = "";
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            string sql = "sp_gam_b2f_overwrite";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            sqldb.execSqlWithCmd(ref cmd);
            //cmd.Dispose();

            cmd = new SqlCommand("sp_gam_b2f_add");
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (ListViewDataItem item in listMain.Items)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fcPeriod", ((Label)item.FindControl("period")).Text);
                cmd.Parameters.AddWithValue("@sales", ((Label)item.FindControl("sales")).Text);
                cmd.Parameters.AddWithValue("@oem", ((Label)item.FindControl("oem")).Text);
                cmd.Parameters.AddWithValue("@cpn", ((Label)item.FindControl("cpn")).Text);
                cmd.Parameters.AddWithValue("@pn", ((Label)item.FindControl("ipn")).Text);
                cmd.Parameters.AddWithValue("@sqft", Convert.ToSingle(((Label)item.FindControl("sqft")).Text));
                cmd.Parameters.AddWithValue("@qty", ((Label)item.FindControl("qty")).Text);
                cmd.Parameters.AddWithValue("@array", ((Label)item.FindControl("array")).Text);
                cmd.Parameters.AddWithValue("@smt", ((Label)item.FindControl("smt")).Text);
                cmd.Parameters.AddWithValue("@fpc", ((Label)item.FindControl("fpc")).Text);
                cmd.Parameters.AddWithValue("@bom", ((Label)item.FindControl("bom")).Text);
                sqldb.execSqlWithCmd(ref cmd);
            }
            cmd.Connection.Dispose();
            cmd.Dispose();
        }
        loadExistsData();
    }
    protected void displayResult_click(object sender, EventArgs e)
    {
        DisplayArea.Visible = false;
        ResultArea.Visible = true;
        DataTable dt = getResult();
        if (dt.Rows.Count > 0)
        {
            resultGrid.DataSource = dt;
            resultGrid.DataBind();
        }
    }
    protected void uploadAgain_Click(object sender, EventArgs e)
    {
        tmp.Text = "";
        displayUpload();
    }
    protected void goDisplay_Click(object sender, EventArgs e)
    {
        displayNormal();
    }
    private DataTable getResult()
    {
        string msg = "";
        DataTable dt = new DataTable();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            string sql = "sp_gam_b2f_report";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter _msg = cmd.Parameters.AddWithValue("@msg", "");
            _msg.Size = 200;
            _msg.Direction = ParameterDirection.Output;
            db.execSqlWithCmd(ref cmd);
            msg = _msg.Value.ToString();
            dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    protected void downloadResult_Click(object sender, EventArgs e)
    {
        DataTable dt = getResult();
        if (dt.Rows.Count > 0)
        {
            Multek.Util.DT2Excel(dt, "B2F_forecast_result");
            dt.Dispose();
        }
    }

    protected void resultGrid_DataBound(object sender, EventArgs e)
    {
        Table tbl = (Table)resultGrid.Controls[0];
        TableRow hd = tbl.Rows[0];
        #region header title set up
        GridViewRow row1 = new GridViewRow(1, -1, DataControlRowType.Header, DataControlRowState.Normal);
        row1.CssClass = hd.CssClass = "header";
        string[] HK = new string[] {"SQFT","QTY","Array","SMT","BOD","FPC"};
        int cols = hd.Cells.Count;
        for (int i = 0; i < cols; i++)
        {
            string xx = hd.Cells[i].Text;
            int idx = i - 2;
            TableCell th = new TableHeaderCell();

            if (xx.IndexOf("_sqft") > 0)
            {
                string p = xx.Substring(0, 6);
                th.Text = Multek.Util.getPeriod(Convert.ToInt32(p), false);
                th.ColumnSpan = 6;
                for(var j=0; j < HK.Length; j++)
                    hd.Cells[i+j].Text = HK[j];
                i = i + 5;
            }
            else
            {
                th.Text = "";
                hd.Cells[i].Text = xx;
            }
            row1.Cells.Add(th);
        }
        tbl.Rows.AddAt(0, row1);
        #endregion
    }

    private void TDheadertoTDheaderDIV(TableCell cell, string headerText)
    {
        cell.Text = "";
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.InnerHtml = headerText;
        //div.Style.Add("width", "80px");
        cell.Controls.Add(div);
    }
}
