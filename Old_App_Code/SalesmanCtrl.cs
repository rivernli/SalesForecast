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
/// Summary description for SalesmanCtrl
/// </summary>
public class SalesmanCtrl
{
    public static string cname = "GAMconn";
	public SalesmanCtrl()
	{
        
	}
    public class Manager
    {
        private static string __conn = ConfigurationManager.ConnectionStrings[cname].ToString();
        public static DataTable gets()
        {
            DataTable dt = new DataTable();
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("sp_gam_salesmanGet");
                cmd.CommandType = CommandType.StoredProcedure;
                dt = sqldb.getDataTableWithCmd(ref cmd);
                cmd.Dispose();
            }
            return dt;
        }
    }
    public class Salesman
    {
        private static string __conn = ConfigurationManager.ConnectionStrings[cname].ToString();
        public nUser user;
        public Salesman(int id) 
        {
            user = new nUser(id);
            if (!user.isDBuser)
                user = null;
        }

        public static DataTable getsByManager(int manager_id)
        {
            DataTable dt = new DataTable();
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("sp_gam_salesmanGet");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@managerId", manager_id);
                dt = sqldb.getDataTableWithCmd(ref cmd);
                cmd.Dispose();
            }
            return dt;
        }
        public static void update(int manager_id, int sysUserId)
        {
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("[sp_gam_salesmanChangeManager]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@managerId", manager_id);
                cmd.Parameters.AddWithValue("@sysId", sysUserId);
                sqldb.execSqlWithCmd(ref cmd);
                cmd.Dispose();
            }
        }

        public static bool isCEMSales(string uid)
        {
            bool yes = false;
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("[sp_gam_isCEM_sales]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usrid", uid);
                yes = Convert.ToBoolean(sqldb.getSignalValueCmd(ref cmd));
                cmd.Dispose();
            }
            return yes;
        }
    }

}
