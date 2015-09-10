using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for forecastWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class forecastWS : System.Web.Services.WebService {
    private string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    public forecastWS () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string getCusPartForecastByOEM(int period, int oemid)
    {
        return GETFS(period, oemid);
    }

    [WebMethod(EnableSession = true)]
    public string getCusPartForecastUpdate(int period, int oemid, string customerPart, int amt,string cellid, bool resetTable)
    {
        int salesId = 0;

        if (customerPart.IndexOf("<") >= 0 || customerPart.IndexOf(">") > 0)
        {
            return "obj = {cell:\"\",error:1,message:\"error, invalid part number!! "+ customerPart.Replace("\"","'") +"\"}";
        }
        if (HttpContext.Current.Session["usr"] != null)
        {
            nUser usr = (nUser)Session["usr"];
            salesId = usr.sysUserId;
            usr.Dispose();
        }
        int tamt = 0;
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_forecastCustomerPart_Update";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@project", customerPart);
            cmd.Parameters.AddWithValue("@amt", amt);
            cmd.Parameters.AddWithValue("@salesmanId", salesId);
            SqlParameter _tamt = cmd.Parameters.AddWithValue("@tamt", 0);
            _tamt.Direction = ParameterDirection.Output;
            db.execSqlWithCmd(ref cmd);
            tamt = Convert.ToInt32(_tamt.Value);
        }
        if (resetTable)
            return GETFS(period, oemid);
        else
            return "obj = {\"amt\":\""+ amt.ToString() +"\",\"tamt\":\"" + tamt.ToString() +"\",\"cell\":\"" + cellid +"\"}";
    }

    [WebMethod]
    public string getCusPartActualHistory(int oemid, string customerPart)
    {
        string rq = "";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_actualItemHistory_get";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@cspart", customerPart);
            DataTable dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            rq = Multek.Util.DT2JSON(dt);
        }
        return rq;
    }
    [WebMethod]
    public string getCusPartRemark(int oemid, string customerPart)
    {
        string remark = "{}";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_actualItemRemark_get";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@part_no", customerPart);
            SqlParameter _rmk = cmd.Parameters.Add(new SqlParameter("@remark", SqlDbType.NVarChar, 1000));
            _rmk.Direction = ParameterDirection.Output;
            db.execSqlWithCmd(ref cmd);
            cmd.Dispose();
            remark = _rmk.Value.ToString().Replace(System.Environment.NewLine, "\\n ");
        }
        return remark;
    }
    [WebMethod]
    public string setCusPartRemark(int oemid, string customerPart, string new_remark, float new_asp)
    {
        string remark = "{}";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_actualItemRemark_set";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@part_no", customerPart.Trim());
            cmd.Parameters.AddWithValue("@remark", new_remark.Trim());
            cmd.Parameters.AddWithValue("@asp",new_asp);
            db.execSqlWithCmd(ref cmd);
            cmd.Dispose();
            remark = getCusPartRemark(oemid, customerPart);
        }
        return remark;
    }
    private string GETFS(int period, int oemid)
    {
        string rq = "";
        string oem, baan, plant;
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "[sp_gam_ForecastCustomerPart_Get]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@periodLong", 3);

            SqlParameter _oem = cmd.Parameters.AddWithValue("@oem", "");
            _oem.Size = 100;
            SqlParameter _baan = cmd.Parameters.AddWithValue("@baanName", "");
            _baan.Size = 100;
            SqlParameter _plant = cmd.Parameters.AddWithValue("@plant", "");
            _plant.Size = 3;
            _plant.SqlDbType = SqlDbType.NVarChar;

            _oem.Direction = ParameterDirection.Output;
            _baan.Direction = ParameterDirection.Output;
            _plant.Direction = ParameterDirection.Output;
            DataSet ds = db.getDataSetCmd(ref cmd);
            DataTable dt = modDS(ds);// ds.Tables[0];


            oem = _oem.Value.ToString();
            baan = _baan.Value.ToString();
            plant = _plant.Value.ToString();

            cmd.Dispose();
            rq = Multek.Util.DT2JSON(dt);
            dt.Dispose();
        }
        return "ob = {oem:{id:\"" + oemid + "\",oem:\"" + oem + "\",baan:\"" + baan + "\",plant:\"" + plant + "\"},result:" + rq + "}";

    }
    private static DataTable modDS(DataSet ds)
    {
        DataTable dt1 = ds.Tables[0];
        DataTable dt2 = ds.Tables[1];
        DataTable fst = dt2.DefaultView.ToTable(true, new string[] { "iperiod" });
        foreach (DataRow fstr in fst.Rows)
        {
            DataColumn col = new DataColumn(fstr["iperiod"].ToString(), typeof(int));
            col.DefaultValue = 0;
            dt1.Columns.Add(col);
        }
        fst.Dispose();
        foreach (DataRow row in dt2.Select("sid > 0"))
        {
            foreach (DataRow fr in dt1.Select("cus_part_no = '" + row["project"] + "'"))
            {
                fr[row["iperiod"].ToString()] = Convert.ToInt32(row["amt"]);
            }
        }
        dt2.Dispose();
        return dt1;
    }




    
    [WebMethod]
    public string getPartForecastHistory(int period, int oemid,string project)
    {
        string sql = "select top 6 * from dbo.gam_part_forecast_log where oem_id=@oemid and project=@project and period = @period order by log_period desc";
        string rq = "";
        /*
        int tamt = 0;
        string oem, baan, plant;
         * */
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            //cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@project", project);
            DataTable dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            rq = Multek.Util.DT2JSON(dt);
            dt.Dispose();
        }
        return  rq;
    }

    [WebMethod]
    public string updateOLS_TopSide_adjust(int period, int oemid, int amt, string adjustType, int x, int y)
    {
        int gtt = 0;
        int stt = 0;
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_forecast_TopSide_adjustment");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@adjust", amt);
            cmd.Parameters.AddWithValue("@isOLS", (adjustType == "ols") ? true : false);

            SqlParameter _gtt = cmd.Parameters.AddWithValue("@grandTtl", 0);
            _gtt.Direction = ParameterDirection.Output;
            SqlParameter _stt = cmd.Parameters.AddWithValue("@salesTtl", 0);
            _stt.Direction = ParameterDirection.Output;

            db.execSqlWithCmd(ref cmd);
            gtt = Convert.ToInt32(_gtt.Value);
            stt = Convert.ToInt32(_stt.Value);
            cmd.Dispose();
        }
        return "{x:" + x.ToString() + ",y:" + y.ToString() + ",amt:" + amt.ToString() + ",gtt:" + gtt.ToString()
            + ",stt:" + stt.ToString() + "}";
    }
    [WebMethod (EnableSession = true)]
    public string setOEMComments(int oemid, string comments)
    {
        if (comments.Trim() != "")
        {
            string author = "";
            if (HttpContext.Current.Session["usr"] != null)
            {
                nUser usr = (nUser)Session["usr"];
                author = usr.userName;
                usr.Dispose();
            }
            if (author != "")
            {
                using (Multek.SqlDB db = new Multek.SqlDB(__conn))
                {
                    SqlCommand cmd = new SqlCommand("sp_gam_oem_comments_set");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@oem_id", oemid);
                    cmd.Parameters.AddWithValue("@comments", comments);
                    cmd.Parameters.AddWithValue("@author", author);
                    db.execSqlWithCmd(ref cmd);
                    cmd.Dispose();
                }
            }
        }
        return getOEMComments(oemid);
    }
    [WebMethod(EnableSession = true)]
    public string getOEMComments(int oemid)
    {
        string author = "";
        if (HttpContext.Current.Session["usr"] != null)
        {
            nUser usr = (nUser)Session["usr"];
            author = usr.userName;
            usr.Dispose();
        }
        string output = "";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_oem_comments_get");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oem_id", oemid);
            cmd.Parameters.AddWithValue("@author", author);
            DataTable dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            output = Multek.Util.DT2JSON(dt);
        }
        return output;
    }
    [WebMethod(EnableSession = true)]
    public string delOEMComments(int comment_id, int oemid)
    {
            string author = "";
            if (HttpContext.Current.Session["usr"] != null)
            {
                nUser usr = (nUser)Session["usr"];
                author = usr.userName;
                usr.Dispose();
            }
            if (author != "")
            {
                using (Multek.SqlDB db = new Multek.SqlDB(__conn))
                {
                    SqlCommand cmd = new SqlCommand("sp_gam_oem_comments_del");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@comment_id", comment_id);
                    cmd.Parameters.AddWithValue("@author", author);
                    db.execSqlWithCmd(ref cmd);
                    cmd.Dispose();
                }
            }
        return getOEMComments(oemid);
    }
}

