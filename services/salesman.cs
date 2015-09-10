using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

using System.Web.Script.Services;
using System.Web.Script.Serialization;
/// <summary>
/// Summary description for salesman
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [System.Web.Script.Services.ScriptService]
public class salesman : System.Web.Services.WebService {
    private string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    public salesman () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)] 
    public string getSale(string key)
    {
        DataTable dt;
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_salesman_find";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@Salesonly", true);
            dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            dt.Dispose();
        }
        JavaScriptSerializer jss = new JavaScriptSerializer();
        string json = jss.Serialize(dt);
        return json;
    }


    [WebMethod]
    public string getSales(string key)
    {
        string rq = "rs:{[]}";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_salesman_find";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@Salesonly", true);
            DataTable dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            rq = Multek.Util.DT2JSON(dt);
            dt.Dispose();
        }
        return rq;
    }
    [WebMethod]
    public string getUsers(string key)
    {
        string rq = "rs:{[]}";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_gam_salesman_find";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@key", key);
            DataTable dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            rq = Multek.Util.DT2JSON(dt);
        }
        return rq;
    }
    [WebMethod]
    public string getBaanOEM(string key)
    {
        DataTable dt = OEMBaan.searchOEM(key, "", 30);
        return Multek.Util.DT2JSON(dt);
    }

    [WebMethod]
    public string getForecastHistory(int oem, int period)
    {
        DataTable dt = Forecast.getForecastHistory(oem, period);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='standardTable' border='1' cellpadding='1' cellspacing='0' borderColor='#cccccc' bgcolor='#ffffff'>" +
                "<tr bgcolor='C4FFF3'><th>Submitted</th><th>Forecast Amount</th><th>Gap</th></tr>");
            foreach (DataRow row in dt.Rows)
            {
                sb.Append(string.Format("<tr><td>{0:d} [{1}]</td><td>{2}</td><td>{3}</td></tr>",
                    row["submit_date"], Multek.Util.getPeriodNBR((int)row["submit_fcst_period"]), row["fcst_amt"], row["diff"]));
            }
            sb.Append("</table>");
            return sb.ToString();
        }
        else
        {
            return "";
        }
    }
    [WebMethod]
    public string getForecastHistoryOLS(int oem, int period)
    {
        DataSet ds = Forecast.getForecastHistoryOLS(oem, period);
        if (ds.Tables[0].Rows.Count <= 0)
            return "";

        string rs = "{oem:" + oem.ToString() + ",period:" + period.ToString() + ",current:" + Multek.Util.DT2JSON(ds.Tables[0]);
        if (ds.Tables[1].Rows.Count > 0)
        {
            rs += ",log:" + Multek.Util.DT2JSON(ds.Tables[1]);
        }
        return rs + "}";
    }
    [WebMethod(EnableSession = true)]
    public string updateForecast(string oid, string amt, string period, string object_id)
    {
        
        if (HttpContext.Current.Session["usr"] != null)
        {
            nUser usr = (nUser)Session["usr"];
            return object_id +":" + Forecast.updateForecast(usr.sysUserId, Convert.ToInt32(oid), Convert.ToDouble(amt), Convert.ToInt32(period)).ToString();
             
        }
        else
            return object_id + ":0"; ;
    }
}

