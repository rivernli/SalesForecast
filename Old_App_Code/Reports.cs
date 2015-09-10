using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
/// <summary>
/// Summary description for Reports
/// </summary>
public class Reports
{
    private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();

	public Reports()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static DataSet getRportCompareP2P(int p1, int p2, out string message)
    {
        message = "";
        DataSet ds = new DataSet();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_CompareForecastOEM");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@basePeriod", p1);
            cmd.Parameters.AddWithValue("@comparePeriod", p2);
            SqlParameter _msg = cmd.Parameters.AddWithValue("@message", "");
            _msg.Size = 1000;
            _msg.Direction = ParameterDirection.Output;
            ds = db.getDataSetCmd(ref cmd);
            message = _msg.Value.ToString();
            cmd.Dispose();
        }
        return ds;
    }
    public static DataSet getRportFCP2P(int p1, int p2, out string message)
    {
        message = "";
        DataSet ds = new DataSet();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("[sp_gam_CompareForecastOEM_delta]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@basePeriod", p1);
            cmd.Parameters.AddWithValue("@comparePeriod", p2);
            SqlParameter _msg = cmd.Parameters.AddWithValue("@message", "");
            _msg.Size = 1000;
            _msg.Direction = ParameterDirection.Output;
            ds = db.getDataSetCmd(ref cmd);
            message = _msg.Value.ToString();
            cmd.Dispose();
        }
        return ds;
    }
    public static DataTable getReportComparePeriodByPeriod(int p1, int p2, out string message)
    {
        message = "";
        DataTable dt = new DataTable();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_CompareForecastOEM");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@basePeriod", p1);
            cmd.Parameters.AddWithValue("@comparePeriod", p2);
            SqlParameter _msg = cmd.Parameters.AddWithValue("@message", "");
            _msg.Size = 1000;
            _msg.Direction = ParameterDirection.Output;
            dt = db.getDataTableWithCmd(ref cmd);
            message = _msg.Value.ToString();
            cmd.Dispose();
        }
        return dt;
    }
    public static DataTable getB2FResult(int period)
    {
        string msg = "";
        DataTable dt = new DataTable();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            string sql = "sp_gam_b2f_report";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@up", period);
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
}
