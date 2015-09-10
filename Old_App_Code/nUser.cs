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
using System.DirectoryServices;

    public class nUser : IDisposable
    {
        private int _sysUserId=0;
        public int sysUserId { get { return _sysUserId; } }
        private string _UID;
        public string UID { get { return _UID; } }
        public string domain;
        public string sysId { get { return domain +"/" + _UID; } }
        public string userName;
        public string uGroup="";
        public int managerId;
        public bool isReportViewer = false;
        public bool isSales = false;
        public bool isAdmin = false;
        public bool isPriceView = false;
        public bool isActive = false;


        public string emailAddress;
        public string jobTitle;
        public string fax;
        public string tel;
        public string mobile;
        public string department;
        public string ipphone;

        private bool _isDbUser = false;
        private bool _isLdapUser = false;
        public bool isDBuser { get { return _isDbUser; } }
        public bool isLdapUser { get { return _isLdapUser; } }

        public string msg;

        private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
        public nUser(string user_id, string domainName)
        {
            _isLdapUser = _isDbUser = false;
            domain = "asia";
            if (domainName != "")
                domain = domainName;
            Multek.LDAP ldap = new Multek.LDAP(@"LDAP://DC=" + domain + ",DC=ad,DC=flextronics,DC=com");
            if (ldap.findUser(user_id,domain))
            {
                _isLdapUser = true;
                _UID = ldap.uid;
                userName = ldap.name;
                emailAddress = ldap.email;
                jobTitle = ldap.title;
                fax = ldap.fax;
                tel = ldap.tel;
                mobile = ldap.mobile;
                ipphone = ldap.ipPhone;
                department = ldap.department; 
                DataTable dt = getDbUser(_UID, domain);
                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    _isDbUser = true;
                    _sysUserId = (int)row["sysUserId"];
                    managerId = (int)row["managerId"];
                    isReportViewer = (bool)row["isReportViewer"];
                    isSales = (bool)row["isSales"];
                    isActive = (bool)row["isActive"];
                    isAdmin = (bool)row["isAdmin"];
                    isPriceView = (bool)row["isPriceView"];
                    uGroup = row["uGroup"].ToString();
                }
            }
            else
            {
                using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
                {
                    SqlCommand cmd = new SqlCommand("sp_GAM_UserUpdate");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", user_id);
                    cmd.Parameters.AddWithValue("@domain", domain);
                    cmd.Parameters.AddWithValue("@username", "unknow");
                    cmd.Parameters.AddWithValue("@isReportViewer", false);
                    cmd.Parameters.AddWithValue("@isSales", false);
                    cmd.Parameters.AddWithValue("@isActive", false);
                    cmd.Parameters.AddWithValue("@isAdmin", false);
                    cmd.Parameters.AddWithValue("@isPriceView", false);
                    cmd.Parameters.AddWithValue("@group", "Disabled");
                    sqldb.execSqlWithCmd(ref cmd);
                    cmd.Dispose();
                }
                _UID = domain = "";
                msg = "User not found.";
            }
            ldap.Dispose();
        }

        //for salesman usage.
        public nUser(int system_id) 
        { 
            DataTable dt;
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("sp_gam_salesmanGet");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@salesmanId", system_id);
                dt = sqldb.getDataTableWithCmd(ref cmd);
            }
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                _isDbUser = true;
                _sysUserId = (int)row["sysUserId"];
                managerId = (int)row["managerId"];
                isReportViewer = (bool)row["isReportViewer"];
                department = row["department"].ToString();
                jobTitle = row["jobTitle"].ToString();
                isSales = (bool)row["isSales"];
                isActive = (bool)row["isActive"];
                isAdmin = (bool)row["isAdmin"];
                isPriceView = (bool)row["isPriceView"];
                userName = row["userName"].ToString();
            }
        }
        private DataTable getDbUser(string uid, string domain)
        {
            DataTable dt = new DataTable();
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("sp_GAM_UsersGet");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.Parameters.AddWithValue("@domain", domain);
                dt = sqldb.getDataTableWithCmd(ref cmd);
            }
            return dt;
        }
        public DataTable getDbUsers(string uid, string domain)
        {
            return getDbUser(uid, domain);
        }
        public DataTable getDbUsersByGroup(string group)
        {
            DataTable dt = new DataTable();
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand("sp_GAM_UsersGetGroup");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@group", group);
                dt = sqldb.getDataTableWithCmd(ref cmd);
            }
            return dt;
        }
        public void Dispose() { }

        public bool isWebAdmin()
        {
            bool pass = false;
            string webconfig_uid = _UID.ToUpper();
            string userking ="multekadmins";
            if (userking.IndexOf(webconfig_uid) >= 0)
                pass = true;
            else
                pass= false;
            return pass;
        }
        public bool addDBUser()
        {
            if (_isLdapUser && !_isDbUser)
            {
                using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
                {
                    SqlCommand cmd = new SqlCommand("sp_GAM_UserAdd");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", UID);
                    cmd.Parameters.AddWithValue("@domain", domain);
                    cmd.Parameters.AddWithValue("@username", userName);
                    cmd.Parameters.AddWithValue("@managerId", managerId);
                    cmd.Parameters.AddWithValue("@isReportViewer", isReportViewer);
                    cmd.Parameters.AddWithValue("@isSales", isSales);
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@isAdmin", isAdmin);
                    cmd.Parameters.AddWithValue("@emailAddress", emailAddress);
                    cmd.Parameters.AddWithValue("@department", department);
                    cmd.Parameters.AddWithValue("@jobTitle", jobTitle);
                    cmd.Parameters.AddWithValue("@fax", fax);
                    cmd.Parameters.AddWithValue("@tel", tel);
                    cmd.Parameters.AddWithValue("@isPriceView", isPriceView);
                    cmd.Parameters.AddWithValue("@group", uGroup);
                    sqldb.execSqlWithCmd(ref cmd);
                    cmd.Dispose();
                }
            }
            return true;
        }
        public bool updateDBUser()
        {
            if (_isLdapUser && _isDbUser)
            {
                using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
                {
                    SqlCommand cmd = new SqlCommand("sp_GAM_UserUpdate");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", UID);
                    cmd.Parameters.AddWithValue("@domain", domain);
                    cmd.Parameters.AddWithValue("@username", userName);
                    cmd.Parameters.AddWithValue("@isReportViewer", isReportViewer);
                    cmd.Parameters.AddWithValue("@isSales", isSales);
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@isAdmin", isAdmin);
                    cmd.Parameters.AddWithValue("@isPriceView", isPriceView);
                    cmd.Parameters.AddWithValue("@group", uGroup);
                    sqldb.execSqlWithCmd(ref cmd);
                    cmd.Dispose();
                }
            }
            return true;
        }

        public static bool isWebmin(string uid)
        {
            string webconfig_uid = uid.ToUpper();
            string userQu = "multekadmins";
            if (userQu.IndexOf(webconfig_uid) >= 0)
                return true;

            return false;
        }
        public static void submit_finish_forecast(int salesman_id, int period)
        {
            string sql = "insert into gam_salesman_submit (sales_id,period,submit_time) values (@salesman_id,@period,getdate())";
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@salesman_id", salesman_id);
                cmd.Parameters.AddWithValue("@period", period);
                sqldb.execSqlWithCmd(ref cmd);
                cmd.Dispose();
            }
        }
        public static string submit_finish_forecase_get(int salesman_id)
        {
            string ls = "";
            string sql = "select top 1 submit_time from gam_salesman_submit where sales_id= @salesman_id order by submit_time desc";
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@salesman_id", salesman_id);
                object o = sqldb.getSignalValueCmd(ref cmd);
                cmd.Dispose();
                if (o != null)
                    ls = Convert.ToDateTime(o).ToString("d MMM,yyyy h:mm tt");//.ToShortDateString();
            }
            return ls;
        }
        public static DataTable view_finish_forecast(int period, string salesman)
        {
            salesman = salesman.Trim();
            string sql = "select * from vw_gam_salesman_finished_forecast where period=@period order by submit_time desc";
            DataTable dt = new DataTable();
            using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@period", period);
                dt = sqldb.getDataTableWithCmd(ref cmd);
                cmd.Dispose();
            }
            return dt;            
        }

    }

