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
/// Summary description for OEMBaan
/// </summary>
public class OEMBaan
{
    private int _BaanOEMID=0;
    public int BannOEMId { get { return _BaanOEMID; } }
    public string OEMName;
    public string Plant;
    public string GroupName;

    private string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    public OEMBaan() { }
    public OEMBaan(int oemId)
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_BaanOEMsSearching");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", oemId);
            DataTable dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            if (dt.Rows.Count == 1)
            {
                _BaanOEMID = (int)dt.Rows[0]["BaanOEMId"];
                OEMName = dt.Rows[0]["oemname"].ToString();
                Plant = dt.Rows[0]["plant"].ToString();
                GroupName = dt.Rows[0]["groupname"].ToString();
            }
        }
    }
    public void update()
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_BaanOEMupdate");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@groupname", GroupName);
            cmd.Parameters.AddWithValue("@oemid", _BaanOEMID);
            sqldb.execSqlWithCmd(ref cmd);
            cmd.Dispose();
        }
    }
    public static DataTable getGroup()
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(ConfigurationManager.ConnectionStrings["GAMconn"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_BaanOEMGroupList");
            cmd.CommandType = CommandType.StoredProcedure;
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;

    }
    public static DataTable getOEMByGroup(string group)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(ConfigurationManager.ConnectionStrings["GAMconn"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_BaanOEMsSearching");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@group", group);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    private static string removeQuota(string st)
    {
        st = st.Replace("'", "''");
        st = st.Replace("%", "");
        st = st.Replace("*", "");
        return st;
    }
    public static DataTable searchOEM(string key, string group, int top)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(ConfigurationManager.ConnectionStrings["GAMconn"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_BaanOEMsSearching");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@group", group);
            cmd.Parameters.AddWithValue("@top", top);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
}

public class OEMCus
{
    private int _OEMID = 0;
    public int OEMID { get { return _OEMID; } }
    public string CusOEM;
    public int BaanId;
    public int salesmanId;
    public bool isValid;
    public int viewSalesmanId = 0;
    private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    public OEMCus() { }
    public OEMCus(int oid)
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("select * from gam_oem where oemid=@id");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@id", oid);
            DataTable dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            if (dt.Rows.Count == 1)
            {
                _OEMID = (int)dt.Rows[0]["oemid"];
                CusOEM = dt.Rows[0]["CusOEM"].ToString();
                BaanId = (int)dt.Rows[0]["BaanId"];
                salesmanId = (int)dt.Rows[0]["SalesmanId"];
                isValid = (bool)dt.Rows[0]["isValid"];
                viewSalesmanId = (int)dt.Rows[0]["viewsalesmanId"];
            }
        }
    }
    public void Update()
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_CusOEMUpdate");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CusOEM",CusOEM);
            cmd.Parameters.AddWithValue("@BaanId", BaanId);
            cmd.Parameters.AddWithValue("@salesmanId", salesmanId);
            cmd.Parameters.AddWithValue("@OEMID", OEMID);
            cmd.Parameters.AddWithValue("@isValid", isValid);
            cmd.Parameters.AddWithValue("@viewSalesmanId", viewSalesmanId);
            sqldb.execSqlWithCmd(ref cmd);
            cmd.Dispose();
        }

    }
    public void Add()
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_CusOEMAdd");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CusOEM", CusOEM);
            cmd.Parameters.AddWithValue("@BaanId", BaanId);
            cmd.Parameters.AddWithValue("@salesmanId", salesmanId);
            cmd.Parameters.AddWithValue("@viewSalesmanId", viewSalesmanId);
            SqlParameter par1 =  cmd.Parameters.AddWithValue("@OEMID", 0);
            par1.Direction = ParameterDirection.Output;
            sqldb.execSqlWithCmd(ref cmd);
            _OEMID = Convert.ToInt32(par1.Value);
            cmd.Dispose();
        }
    }
    public static DataTable List(string key,string sales, int status)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(ConfigurationManager.ConnectionStrings["GAMconn"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_CusOEMList");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@key",key);
            cmd.Parameters.AddWithValue("@sales", sales);
            cmd.Parameters.AddWithValue("@isValid", status);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static DataTable ListBySaleId(int id)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(ConfigurationManager.ConnectionStrings["GAMconn"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_CusOEMList");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@salesId", id);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }

    public static void bkSalesAdd(int oemid, int salesman_id)
    {
        string sql = "insert into gam_oem_salesman (oem_id,baan_id,salesman_id,salesman_type)" +
            "select oemid,baanid,@sid,3 from GAM_OEM where oemid=@oemid";
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@sid", salesman_id);
            cmd.Parameters.AddWithValue("@oemid", oemid);
            sqldb.execSqlWithCmd(ref cmd);
            cmd.Dispose();
        }
    }
    public static DataTable bkSalesList(int oemid)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(ConfigurationManager.ConnectionStrings["GAMconn"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("select gam_oem_salesman.id,userName from dbo.GAM_OEM_salesman left join dbo.GAM_USERS on sysUserId = salesman_id where oem_id=@oemid");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@oemid", oemid);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static void bkSalesDel(int sysid)
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("delete GAM_OEM_salesman where id=@id");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@id", sysid);
            sqldb.execSqlWithCmd(ref cmd);
            cmd.Dispose();
        }
    }

    public static string oemType(int oemid)
    {
        string B2 = "normal";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("select right(rtrim(cusOEM),3) as oemType  from dbo.vw_gam_OEMcomb where plant='2f' and oemid=@oemid");
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.CommandType = CommandType.Text;
            DataTable dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0].ToString().ToLower() == "fpc")
                    B2 = "fpc";
                else if (dt.Rows[0][0].ToString().ToLower() == "smt")
                    B2 = "smt";
            }
        }
        return B2;
    }
}