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

public partial class CEMForecastUploadView : System.Web.UI.Page
{
    private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    nUser Me;
    string[] k = { "Admin", "Management" };

    bool isWebAdmin = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
        {
            Me = (nUser)Session["usr"];

            if (k.Contains(Me.uGroup) || Me.isAdmin)
            {
                isWebAdmin = true;
            }            
            
            if (!isWebAdmin)
                Response.Redirect("default.aspx");
        }
        else
        {
            Response.Redirect("default.aspx");
        }
        if (!IsPostBack)
        {
            /*
            periodString.Text = Forecast.currentPeriodAdd(1).ToString() + "," +
                Forecast.currentPeriodAdd(2).ToString() + "," +
                Forecast.currentPeriodAdd(3).ToString() + "," +
                Forecast.currentPeriodAdd(4).ToString() + "," +
                Forecast.currentPeriodAdd(5).ToString() + "," +
                Forecast.currentPeriodAdd(6).ToString();

            Label1.Text = Util.getPeriodNBR(Forecast.currentPeriodAdd(1));
            Label2.Text = Util.getPeriodNBR(Forecast.currentPeriodAdd(2));
            Label3.Text = Util.getPeriodNBR(Forecast.currentPeriodAdd(3));
            Label4.Text = Util.getPeriodNBR(Forecast.currentPeriodAdd(4));
            Label5.Text = Util.getPeriodNBR(Forecast.currentPeriodAdd(5));
            Label6.Text = Util.getPeriodNBR(Forecast.currentPeriodAdd(6));
             */
            Label1.Text = Forecast.currentPeriodAdd(1).ToString();
            Label2.Text = Forecast.currentPeriodAdd(2).ToString();
            Label3.Text = Forecast.currentPeriodAdd(3).ToString();
            Label4.Text = Forecast.currentPeriodAdd(4).ToString();
            Label5.Text = Forecast.currentPeriodAdd(5).ToString();
            Label6.Text = Forecast.currentPeriodAdd(6).ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(finish_upload);", true);
        }
    }

    private void ContentToDTSame(string content)
    {
        string[] lines = content.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        string[] header = lines[0].Split(new char[] { '\t' });
        tmp.Text = "";
        if (header.Length < 8)
        {
            tmp.Text = "fields not match to requirements";

            listMain.DataSource = new DataTable();
            listMain.DataBind();
            return;
        }
        int[] period = { Convert.ToInt32(Label1.Text), Convert.ToInt32(Label2.Text), Convert.ToInt32(Label3.Text),
                       Convert.ToInt32(Label4.Text),Convert.ToInt32(Label5.Text),Convert.ToInt32(Label6.Text)};

        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("code", System.Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("pn", System.Type.GetType("System.String")));
        for (int i = 0; i < period.Length; i++)
        {
            dt.Columns.Add(new DataColumn(period[i].ToString(), System.Type.GetType("System.Int32")));
        }
        foreach (string line in lines)
        {
            string[] item = line.Split(new char[] { '\t' });
            DataRow row = dt.NewRow();
            row[0] = item[0].Trim();
            row[1] = item[1].Trim();
            for (int i = 2; i < 8; i++)
            {
                if (i >= item.Length)
                    row[i] = 0;
                else
                {
                    int qty = 0;
                    int.TryParse(item[i], out qty);
                    //row[i] = item[i] == "" ? 0 : Convert.ToInt32(item[i]);
                    row[i] = qty;
                }
            }
            dt.Rows.Add(row);
        }
        ViewState["DT"] = dt;
        ViewState["period"] = period;
        DataTableBulkCopyToSQL2(dt, period);
    }
    private void DataTableBulkCopyToSQL2(DataTable dt, int[] period)
    {
        string guid = Guid.NewGuid().ToString();
        DataColumn keyStamp = new DataColumn("keyStamp", System.Type.GetType("System.String"));
        keyStamp.DefaultValue = guid;
        dt.Columns.Add(keyStamp);

        DataSet ds = new DataSet("ds");

        using (SqlConnection sqlconn = new SqlConnection(__conn))
        {
            sqlconn.Open();
            using (SqlTransaction sqltran = sqlconn.BeginTransaction())
            {
                SqlBulkCopy bcp = new SqlBulkCopy(sqlconn, SqlBulkCopyOptions.Default, sqltran);
                bcp.DestinationTableName = "cemFCtable2";
                bcp.ColumnMappings.Add(0, 0);
                bcp.ColumnMappings.Add(1, 1);
                bcp.ColumnMappings.Add(2, 2);
                bcp.ColumnMappings.Add(3, 3);
                bcp.ColumnMappings.Add(4, 4);
                bcp.ColumnMappings.Add(5, 5);
                bcp.ColumnMappings.Add(6, 6);
                bcp.ColumnMappings.Add(7, 7);
                bcp.ColumnMappings.Add(8, 8);
                bcp.BulkCopyTimeout = 10000;
                bcp.NotifyAfter = 10;
                try
                {
                    bcp.WriteToServer(dt);
                    string sql = "sp_gam_cem_forecast_uploadView";
                    SqlCommand cmd = new SqlCommand(sql, sqlconn, sqltran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyStamp", guid);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    da.Dispose();
                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    tmp.Text = ex.ToString();
                }
                finally
                {
                    //dt = null;
                    bcp.Close();
                }
            }
            sqlconn.Close();
        }
        DataRelation dl = new DataRelation("dateRelated", ds.Tables[0].Columns["id"],ds.Tables[1].Columns["id"], false);
        dl.Nested = true;
        ds.Relations.Add(dl);
        listMain.DataSource = ds.Tables[0];
        listMain.DataBind();
        //ViewState["data"] = ds;
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        tmp.Text = "";
        downloadUpload.Enabled= confirmUpload.Enabled = false;
        if (cem_content.Text.Trim() != "")
        {
            string txt = Server.HtmlEncode(cem_content.Text.Trim());
            ContentToDTSame(txt);
            uploadArea.Visible = false;
            downloadUpload.Enabled = previewArea.Visible = confirmUpload.Enabled = true;
            if (listMain.Items.Count == 0)
            {
                tmp.Text += "<br />Upload Content error";
                downloadUpload.Enabled = confirmUpload.Enabled = false;
            }
        }
        else
            tmp.Text = "Blank Content uploaded";
    }
    protected void cancelUpload_Click(object sender, EventArgs e)
    {
        uploadArea.Visible = true;
        previewArea.Visible = false;
    }

    
    protected void confirmUpload_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)ViewState["DT"];
        dt.Columns.Remove("keyStamp");

        string guid = Guid.NewGuid().ToString();
        DataColumn keyStamp = new DataColumn("keyStamp", System.Type.GetType("System.String"));
        keyStamp.DefaultValue = guid;
        dt.Columns.Add(keyStamp);

        using (SqlConnection sqlconn = new SqlConnection(__conn))
        {
            sqlconn.Open();
            using (SqlTransaction sqltran = sqlconn.BeginTransaction())
            {
                SqlBulkCopy bcp = new SqlBulkCopy(sqlconn, SqlBulkCopyOptions.Default, sqltran);
                bcp.DestinationTableName = "cemFCtable2";
                bcp.ColumnMappings.Add(0, 0);
                bcp.ColumnMappings.Add(1, 1);
                bcp.ColumnMappings.Add(2, 2);
                bcp.ColumnMappings.Add(3, 3);
                bcp.ColumnMappings.Add(4, 4);
                bcp.ColumnMappings.Add(5, 5);
                bcp.ColumnMappings.Add(6, 6);
                bcp.ColumnMappings.Add(7, 7);
                bcp.ColumnMappings.Add(8, 8);
                bcp.BulkCopyTimeout = 10000;
                bcp.NotifyAfter = 10;
                try
                {
                    bcp.WriteToServer(dt);
                    string sql = "sp_gam_cem_forecast_uploadConfirm";
                    SqlCommand cmd = new SqlCommand(sql, sqlconn, sqltran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyStamp", guid);
                    cmd.ExecuteNonQuery();
                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    tmp.Text = ex.ToString();
                }
                finally
                {
                    bcp.Close();
                }
            }
            sqlconn.Close();
        }
        Response.Redirect("CEMForecast.aspx");
    }


    private DataTable DataTableBulkCopyToSQL3()
    {
        DataTable dt = (DataTable)ViewState["DT"];
        dt.Columns.Remove("keyStamp");
        int[] period = (int[])ViewState["period"];
        DataTable nDT = new DataTable();

        string guid = Guid.NewGuid().ToString();
        DataColumn keyStamp = new DataColumn("keyStamp", System.Type.GetType("System.String"));
        keyStamp.DefaultValue = guid;
        dt.Columns.Add(keyStamp);

        DataSet ds = new DataSet("ds");

        using (SqlConnection sqlconn = new SqlConnection(__conn))
        {
            sqlconn.Open();
            using (SqlTransaction sqltran = sqlconn.BeginTransaction())
            {
                SqlBulkCopy bcp = new SqlBulkCopy(sqlconn, SqlBulkCopyOptions.Default, sqltran);
                bcp.DestinationTableName = "cemFCtable2";
                bcp.ColumnMappings.Add(0, 0);
                bcp.ColumnMappings.Add(1, 1);
                bcp.ColumnMappings.Add(2, 2);
                bcp.ColumnMappings.Add(3, 3);
                bcp.ColumnMappings.Add(4, 4);
                bcp.ColumnMappings.Add(5, 5);
                bcp.ColumnMappings.Add(6, 6);
                bcp.ColumnMappings.Add(7, 7);
                bcp.ColumnMappings.Add(8, 8);
                bcp.BulkCopyTimeout = 10000;
                bcp.NotifyAfter = 10;
                try
                {
                    bcp.WriteToServer(dt);
                    string sql = "sp_gam_cem_forecast_uploadDownload";
                    SqlCommand cmd = new SqlCommand(sql, sqlconn, sqltran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keyStamp", guid);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    da.Dispose();
                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    tmp.Text = ex.ToString();
                }
                finally
                {
                    bcp.Close();
                }
            }
            sqlconn.Close();
        }
        if(ds.Tables.Count >0)
            nDT = ds.Tables[0];
        ds.Dispose();
        return nDT;
    }
    private void downloadUploaded()
    {
        if (ViewState["DT"]!= null)
        {
            DataTable dt = DataTableBulkCopyToSQL3();
            string file = "cem_forecast_upload_" + Guid.NewGuid().ToString();
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.Charset = "";
            context.Response.AddHeader("content-disposition", "attachment;filename="+ file +".xls");
            context.Response.ContentType = "application/vnd.ms-excel";
            StreamReader sr = new StreamReader(Context.Server.MapPath("xml/excelTemp.xml"));
            string rptxml = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();

            string content = "<Row>" +
                 "<Cell><Data ss:Type=\"String\">ID</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">"+ Label1.Text +"</Data></Cell><Cell><Data ss:Type=\"String\">" + Label2.Text + "</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">" + Label3.Text + "</Data></Cell><Cell><Data ss:Type=\"String\">" + Label4.Text + "</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">" + Label5.Text + "</Data></Cell><Cell><Data ss:Type=\"String\">" + Label6.Text + "</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">customer Code</Data></Cell><Cell><Data ss:Type=\"String\">OEM</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">Plant</Data></Cell><Cell><Data ss:Type=\"String\">forecast PN</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">result Customer PN</Data></Cell><Cell><Data ss:Type=\"String\">Project</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">ASP(guess)</Data></Cell><Cell><Data ss:Type=\"String\">Price(guess)</Data></Cell>" +
                 "<Cell><Data ss:Type=\"String\">Price Master</Data></Cell><Cell><Data ss:Type=\"String\">Type</Data></Cell></Row>";

            string rowxml = "<Row><Cell><Data ss:Type=\"Number\">{0}</Data></Cell><Cell><Data ss:Type=\"Number\">{1}</Data></Cell>" +
                "<Cell><Data ss:Type=\"Number\">{2}</Data></Cell><Cell><Data ss:Type=\"Number\">{3}</Data></Cell>" +
                "<Cell><Data ss:Type=\"Number\">{4}</Data></Cell><Cell><Data ss:Type=\"Number\">{5}</Data></Cell>" +
                "<Cell><Data ss:Type=\"Number\">{6}</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">{7}</Data></Cell><Cell><Data ss:Type=\"String\">{8}</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">{9}</Data></Cell><Cell><Data ss:Type=\"String\">{10}</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">{11}</Data></Cell><Cell><Data ss:Type=\"String\">{12}</Data></Cell>" +
                "<Cell><Data ss:Type=\"Number\">{13}</Data></Cell><Cell><Data ss:Type=\"Number\">{14}</Data></Cell>"+
                "<Cell><Data ss:Type=\"Number\">{16}</Data></Cell><Cell><Data ss:Type=\"String\">{15}</Data></Cell></Row>";


            StringBuilder sb = new StringBuilder();
            sb.Append(content);
            foreach (DataRow row in dt.Rows)
            {
                sb.Append(string.Format(rowxml, row.ItemArray));
            }
            dt.Dispose();
            rptxml = rptxml.Replace("<Row />", sb.ToString());
            Response.Write(rptxml);
            Response.End();
        }
    }

    [System.Web.Services.WebMethod]
    public static string getPartNoHistory(string iPN,string code)
    {
        //string result = "<span>No user found!</span>";
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            string sql = "select top 10 OEM,plant,cus_part_no,qty,price,amount,sqft,asp = case when amount=0 or sqft=0 then 0 else amount/sqft end,"+
                "Convert(datetime, gamDate, 112) as lastDate from dbo.cemFC_ship_and_mss where int_part_no=@ipn and customer_code=@code order by gamDate desc";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@ipn", iPN);
            cmd.Parameters.AddWithValue("@code", code);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Connection.Dispose();
            cmd.Dispose();
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("<table class='standardTable' border='1' cellpadding='1' cellspacing='0' borderColor='#cccccc' width='250' bgcolor='#FFFBBC'>"+
            "<tr bgcolor='#C6976B'><th>Part#</th><th>Qty</th><th>Price</th><th>Amount</th><th>Sqft</th><th>ASP</th><th>Date</th></tr>");
        foreach (DataRow row in dt.Rows)
        {
            //sb.Append(String.Format("<tr><td nowrap=true>{0}</td><td align='right'>{1:N0}</td><td>{2:N3}</td><td>{3:d}</tr>", row[2], row[3],row[4],row[8]));
            sb.Append(String.Format("<tr bgcolor='#C5EACB'><td>{0}</td><td align='right'>{1:N0}</td><td align='right'>{2:N2}</td><td align='right'>{3:N2}</td><td align='right'>{4:N4}</td><td align='right'>{5:N2}</td><td>{6:dd MMM yyyy}</tr>", 
                row[2], row[3], row[4],row[5],row[6],row[7], row[8]));
        }
        sb.Append("</table>");
        dt.Dispose();
        return sb.ToString();
    }

    protected void downloadUpload_Click(object sender, EventArgs e)
    {
        downloadUploaded();
    }
}
