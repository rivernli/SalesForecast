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
/// Summary description for Forecast
/// </summary>
public class Forecast
{
    private static string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();

	public Forecast()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    private static DataSet getDS(int startPeriod, int endPeriod, int salesman)
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_ForecastData_getBySalesId");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@startPeriod", startPeriod);
            cmd.Parameters.AddWithValue("@endPeriod", endPeriod);
            cmd.Parameters.AddWithValue("@salesman", salesman);
            ds = sqldb.getDataSetCmd(ref cmd);
            cmd.Dispose();
        }
        return ds;
    }
    private static DataSet getDS(int startPeriod, int endPeriod, int salesman, string uid)
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_ForecastData_getBySalesId");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@startPeriod", startPeriod);
            cmd.Parameters.AddWithValue("@endPeriod", endPeriod);
            cmd.Parameters.AddWithValue("@salesman", salesman);
            cmd.Parameters.AddWithValue("@uid", uid);
            ds = sqldb.getDataSetCmd(ref cmd);
            cmd.Dispose();
        }
        return ds;
    }
    
    private static DataSet getDSbyAny(int startPeriod, int endPeriod, int salesman, string oem, string plant, string group)
    {
        return getDSbyAny(startPeriod, endPeriod, salesman, oem, plant, group, true, true);
    }
    private static DataSet getDSbyAny(int startPeriod, int endPeriod, int salesman, string oem, string plant, string group, bool bk1, bool bk2)
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("[sp_gam_ForecastData_getAllCriteria]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@startPeriod", startPeriod);
            cmd.Parameters.AddWithValue("@endPeriod", endPeriod);
            cmd.Parameters.AddWithValue("@salesman", salesman);
            cmd.Parameters.AddWithValue("@oemgroup", group);
            cmd.Parameters.AddWithValue("@oem", oem);
            cmd.Parameters.AddWithValue("@plant", plant);
            cmd.Parameters.AddWithValue("@subSales", false);
            cmd.Parameters.AddWithValue("@bkSales", false);
            ds = sqldb.getDataSetCmd(ref cmd);
            cmd.Dispose();
        }
        return ds;
    }


    private static DataTable TableVtoH_Adjust(DataSet ds, int cp)
    {
        DataTable dt2 = ds.Tables[1].DefaultView.ToTable(true, new string[] { "userName", "OEMID", "CusOEM" });
        DataRow trow = dt2.NewRow();
        trow["OEMID"] = "0";
        trow["CusOEM"] = "TOTAL";
        trow["userName"] = "All";
        dt2.Rows.Add(trow);
        int pv = 0;
        foreach (DataRow r in ds.Tables[0].Rows)
        {
            pv = (int)r["iperiod"];
            DataColumn colm = new DataColumn(pv.ToString() + " fcst", typeof(int));
            colm.DefaultValue = 0;
            dt2.Columns.Add(colm);

            DataColumn colmols = new DataColumn(pv.ToString() + " ols", typeof(int));
            colmols.DefaultValue = 0;
            dt2.Columns.Add(colmols);

            DataColumn colmtop = new DataColumn(pv.ToString() + " topside", typeof(int));
            colmtop.DefaultValue = 0;
            dt2.Columns.Add(colmtop);

            if (pv <= cp)
            {
                DataColumn colm3 = new DataColumn(pv.ToString() + " actual", typeof(int));
                colm3.DefaultValue = 0;
                dt2.Columns.Add(colm3);
                DataColumn colmG = new DataColumn(pv.ToString() + " gap", typeof(int));
                colmG.DefaultValue = 0;
                dt2.Columns.Add(colmG);
            }
        }
        return dt2;
    }

    private static DataTable TableVtoH2(DataSet ds, int cp)
    {
        DataTable dt2 = ds.Tables[1].DefaultView.ToTable(true, new string[] { "userName","OEMID", "CusOEM" });
        DataRow trow = dt2.NewRow();
        trow["OEMID"] = "0";
        trow["CusOEM"] = "TOTAL";
        trow["userName"] = "All";
        dt2.Rows.Add(trow);
        int pv = 0;
        foreach (DataRow r in ds.Tables[0].Rows)
        {
            pv = (int)r["iperiod"];
            DataColumn colm = new DataColumn(r["iperiod"].ToString() + " fcst", typeof(double));
            colm.DefaultValue = 0;
            dt2.Columns.Add(colm);
            if (pv <= cp)
            {
                DataColumn colm3 = new DataColumn(r["iperiod"].ToString() + " actual", typeof(double));
                colm3.DefaultValue = 0;
                dt2.Columns.Add(colm3);
                DataColumn colmG = new DataColumn(r["iperiod"].ToString() + " gap", typeof(double));
                colmG.DefaultValue = 0;
                dt2.Columns.Add(colmG);
            }
        }
        return dt2;
    }

    private static DataTable TableVtoH3(DataSet ds, int cp)
    {
        DataTable dt2 = ds.Tables[1].DefaultView.ToTable(true, new string[] { "userName", "OEMID", "CusOEM" });
        int pv = 0;
        foreach (DataRow r in ds.Tables[0].Rows)
        {
            pv = (int)r["iperiod"];
            DataColumn colm = new DataColumn(r["iperiod"].ToString() + " fcst", typeof(double));
            colm.DefaultValue = 0;
            dt2.Columns.Add(colm);
            if (pv <= cp)
            {
                DataColumn colm3 = new DataColumn(r["iperiod"].ToString() + " actual", typeof(double));
                colm3.DefaultValue = 0;
                dt2.Columns.Add(colm3);
                DataColumn colmG = new DataColumn(r["iperiod"].ToString() + " gap", typeof(double));
                colmG.DefaultValue = 0;
                dt2.Columns.Add(colmG);
            }
        }
        return dt2;
    }
    public static DataTable getForecastOnly(int startPeriod, int endPeriod, int salesman, string oemName, string plant, string group, bool subSales, bool bkSales)
    {
        DataTable dt2 = new DataTable();

        DataSet ds = getDSbyAny(startPeriod, endPeriod, salesman, oemName, plant, group, subSales, bkSales);
        if (ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
        {
            int cp = currentPeriod();
            int pv = 0;
            //dt2 = TableVtoH2b(ds, cp);
            //dt2 = TableVtoH2a(ds, cp);
            //dt2 = TableVtoH2(ds, cp);
            dt2 = TableVtoH3(ds, cp);
            int coln = dt2.Columns.Count - 1;
            for(int x = coln;  x >0; x--)
            {
                DataColumn col = dt2.Columns[x];
                if (col.ColumnName.IndexOf("actual") >= 0)
                    dt2.Columns.Remove(col);
                if (col.ColumnName.IndexOf("gap") >= 0)
                    dt2.Columns.Remove(col);
            }
                DataTable tdCV = ds.Tables[2];
                for (int j = 0; j < tdCV.Rows.Count; j++)
                {
                    string n = (j + 1).ToString() + "_view";
                    if (tdCV.Rows.Count > j)
                        n = tdCV.Rows[j][0].ToString() + " view";
                    DataColumn colm = new DataColumn(n, typeof(double));
                    colm.DefaultValue = 0;
                    dt2.Columns.Add(colm);
                    colm.SetOrdinal(j + 3);
                }
            foreach (DataRow oem in dt2.Rows)
            {
                foreach (DataRow fcstRow in ds.Tables[1].Select("OEMID=" + oem[1].ToString()))
                {
                    pv = Convert.ToInt32(fcstRow["fiscal_period"]);
                    if (pv > 0)
                        oem[pv.ToString() + " fcst"] = fcstRow["fcst_amt"].ToString();
                }
                foreach (DataRow vRow in ds.Tables[3].Select("OEMID=" + oem[1].ToString()))
                {
                    oem[vRow["fiscal_period"] + " view"] = vRow["actual_amt"].ToString();
                }

            }
            dt2.Columns.Remove(dt2.Columns[dt2.Columns.Count - 1]);
            /*
            int lr = dt2.Rows.Count - 1;
            for (int i = 3; i < dt2.Columns.Count; i++)
            {
                string o = dt2.Compute("sum([" + dt2.Columns[i].ColumnName + "])", null).ToString();
                dt2.Rows[lr][i] = Convert.ToInt32(o); ;
            }
             */ 

        }
        ds.Dispose();
        return dt2;

    }
    public static DataTable getForecastDataBySalesByPart(int startPeriod, int endPeriod, int salesman,string oemName,string plant,string group)
    {
        DataSet ds = getDSbyAny(startPeriod, endPeriod, salesman, oemName, plant, group);
        DataTable dt2 = new DataTable();
        if (ds.Tables[0].Rows.Count > 0)
        {
            int cp = currentPeriod();
            int pv = 0;
            dt2 = TableVtoH2(ds, cp);
            foreach (DataRow oem in dt2.Rows)
            {
                foreach (DataRow fcstRow in ds.Tables[1].Select("OEMID=" + oem[1].ToString()))
                {
                    pv = Convert.ToInt32(fcstRow["fiscal_period"]);
                    if (pv > 0)
                    {
                        oem[pv.ToString() + " fcst"] = fcstRow["fcst_amt"].ToString();
                        if (pv <= cp)
                        {
                            oem[pv.ToString() + " actual"] = fcstRow["actual_amt"].ToString();
                            //oem[pv.ToString() + " gap"] = (Convert.ToInt32(fcstRow["fcst_amt"]) - Convert.ToInt32(fcstRow["actual_amt"])).ToString();
                            oem[pv.ToString() + " gap"] = (Convert.ToInt32(fcstRow["actual_amt"]) - Convert.ToInt32(fcstRow["fcst_amt"])).ToString();
                        }
                    }
                }
            }
            int lr = dt2.Rows.Count - 1;
            for (int i = 3; i < dt2.Columns.Count; i++)
            {
                string o = dt2.Compute("sum([" + dt2.Columns[i].ColumnName + "])", null).ToString();
                dt2.Rows[lr][i] = Convert.ToInt32(o); ;
            }
        }
        ds.Dispose();
        return dt2;
    }

    public static DataTable getAdjustFC(int list_by)
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_FC_adjustment");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@bySales", list_by);
            ds = sqldb.getDataSetCmd(ref cmd);
            cmd.Dispose();
        }
        DataTable dt3 = new DataTable();
        if (ds.Tables[0].Rows.Count > 0)
        {
            dt3 = ds.Tables[1].DefaultView.ToTable(true, new string[] { "salesman", "OEMID", "CusOEM" });
            DataRow row = dt3.NewRow();
            row["salesman"] = "All";
            row["OEMID"] = "0";
            row["cusOEM"] = "TOTAL";
            dt3.Rows.Add(row);
            foreach (DataRow rx in ds.Tables[0].Rows)
            {
                DataColumn colFC = new DataColumn(rx["iperiod"].ToString() + "_FC", typeof(double));
                DataColumn colAD = new DataColumn(rx["iperiod"].ToString() + "_AD", typeof(double));
                colFC.DefaultValue = colAD.DefaultValue = 0;
                dt3.Columns.Add(colFC);
                dt3.Columns.Add(colAD);
            }
            foreach (DataRow roem in dt3.Rows)
            {
                foreach(DataRow rf in ds.Tables[1].Select("OEMID="+ roem[1].ToString() +" and fiscal_period >0 "))
                {
                    roem[rf["fiscal_period"] + "_FC"] = rf["fcst_amt"].ToString();
                    roem[rf["fiscal_period"] + "_AD"] = rf["adjust_amt"].ToString();
                }
            }
            int line = dt3.Rows.Count - 1;
            for (int i = 3; i < dt3.Columns.Count; i++)
            {
                string o = dt3.Compute("sum([" + dt3.Columns[i].ColumnName + "])", null).ToString();
                dt3.Rows[line][i] = Convert.ToInt32(o); ;
            }
        }
        ds.Dispose();
        return dt3;
    }
    public static DataTable getForecastDataBySalesForAdjust(int startPeriod, int endPeriod, int salesman, string OEM)
    {
        DataSet ds = getDSbyAny(startPeriod, endPeriod, salesman, OEM, "", "");
        //DataSet ds = getDS(startPeriod, endPeriod, salesman);
        DataTable dt2 = new DataTable();
        if (ds.Tables[0].Rows.Count > 0)
        {
            int cp = currentPeriod();
            int pv = 0;
            dt2 = TableVtoH_Adjust(ds, cp);
            foreach (DataRow oem in dt2.Rows)
            {
                foreach (DataRow fcstRow in ds.Tables[1].Select("OEMID=" + oem[1].ToString()))
                {
                    pv = Convert.ToInt32(fcstRow["fiscal_period"]);
                    if (pv > 0)
                    {
                        oem[pv.ToString() + " fcst"] = fcstRow["fcst_amt"].ToString();
                        oem[pv.ToString() + " ols"] = fcstRow["ols_adjust"].ToString();
                        oem[pv.ToString() + " topside"] = fcstRow["topside_adjust"].ToString();
                        if (pv <= cp)
                        {
                            oem[pv.ToString() + " actual"] = fcstRow["actual_amt"].ToString();
                            oem[pv.ToString() + " gap"] = (Convert.ToInt32(fcstRow["actual_amt"]) - Convert.ToInt32(fcstRow["fcst_amt"])).ToString();
                        }
                    }
                }
            }
            int lr = dt2.Rows.Count - 1;
            for (int i = 3; i < dt2.Columns.Count; i++)
            {
                string o = dt2.Compute("sum([" + dt2.Columns[i].ColumnName + "])", null).ToString();
                dt2.Rows[lr][i] = Convert.ToInt32(o); ;
            }
        }
        ds.Dispose();
        return dt2;
    }
    public static DataTable getForecastDataBySales2(int startPeriod, int endPeriod, int salesman)
    {
        DataSet ds = getDS(startPeriod, endPeriod, salesman);
        DataTable dt2 = new DataTable();
        if (ds.Tables[0].Rows.Count > 0)
        {
            int cp = currentPeriod();
            int pv = 0;
            dt2 = TableVtoH2(ds, cp);
            foreach (DataRow oem in dt2.Rows)
            {
                foreach (DataRow fcstRow in ds.Tables[1].Select("OEMID=" + oem[1].ToString()))
                {
                    pv = Convert.ToInt32(fcstRow["fiscal_period"]);
                    if (pv > 0)
                    {
                        oem[pv.ToString() + " fcst"] = fcstRow["fcst_amt"].ToString();
                        if (pv <= cp)
                        {
                            oem[pv.ToString() + " actual"] = fcstRow["actual_amt"].ToString();
                            oem[pv.ToString() + " gap"] = (Convert.ToInt32(fcstRow["actual_amt"]) - Convert.ToInt32(fcstRow["fcst_amt"])).ToString();
                        }
                    }
                }
            }
            int lr = dt2.Rows.Count-1;
            for (int i = 3; i < dt2.Columns.Count; i++)
            {
                string o = dt2.Compute("sum([" + dt2.Columns[i].ColumnName + "])",null).ToString();
                dt2.Rows[lr][i] = Convert.ToInt32(o);;
            }
        }
        ds.Dispose();
        return dt2;
    }

    private static DataTable TableVtoH(DataSet ds, int cp)
    {
        DataTable dt2 = ds.Tables[1].DefaultView.ToTable(true, new string[] { "OEMID", "CusOEM" });
        DataRow trow = dt2.NewRow();
        trow["OEMID"] = "0";
        trow["CusOEM"] = "TOTAL";
        dt2.Rows.Add(trow);
        int pv = 0;
        foreach (DataRow r in ds.Tables[0].Rows)
        {
            pv = (int)r["iperiod"];
            DataColumn colm = new DataColumn(r["iperiod"].ToString() + " fcst", typeof(double));
            colm.DefaultValue = 0;
            dt2.Columns.Add(colm);
            if (pv <= cp)
            {
                DataColumn colm3 = new DataColumn(r["iperiod"].ToString() + " actual", typeof(double));
                colm3.DefaultValue = 0;
                dt2.Columns.Add(colm3);
                DataColumn colmG = new DataColumn(r["iperiod"].ToString() + " gap", typeof(double));
                colmG.DefaultValue = 0;
                dt2.Columns.Add(colmG);
            }
        }
        return dt2;
    }
    public static DataTable getForecastDataBySales(int startPeriod, int endPeriod, int salesman)
    {
        DataSet ds = getDS(startPeriod, endPeriod, salesman);
        DataTable dt2 = new DataTable();
        if (ds.Tables[0].Rows.Count > 0)
        {
            int cp = currentPeriod();
            int pv = 0;
            dt2 = TableVtoH(ds, cp);
            foreach (DataRow oem in dt2.Rows)
            {
                foreach (DataRow fcstRow in ds.Tables[1].Select("OEMID=" + oem[0].ToString()))
                {
                    pv = Convert.ToInt32(fcstRow["fiscal_period"]);
                    if (pv > 0)
                    {
                        oem[pv.ToString() + " fcst"] = fcstRow["fcst_amt"].ToString();
                        if (pv <= cp)
                        {
                            oem[pv.ToString() + " actual"] = fcstRow["actual_amt"].ToString();
                            oem[pv.ToString() + " gap"] = (Convert.ToInt32(fcstRow["fcst_amt"]) - Convert.ToInt32(fcstRow["actual_amt"])).ToString();
                        }
                    }
                }
            }
            int lr = dt2.Rows.Count - 1;
            for (int i = 3; i < dt2.Columns.Count; i++)
            {
                string o = dt2.Compute("sum([" + dt2.Columns[i].ColumnName + "])", null).ToString();
                dt2.Rows[lr][i] = Convert.ToInt32(o); ;
            }
        }
        ds.Dispose();
        return dt2;
    }

    public static void copyPreviousPreiodForecastData(int period, int salesman)
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_SalesFCST_copy_previous_preiod");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@sid", salesman);
            cmd.Parameters.AddWithValue("@period", period);
            sqldb.execSqlWithCmd(ref cmd);
            cmd.Dispose();
        }
    }
    public static int currentPeriod()
    {
        int cd = 0;
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            string sql;// = "sp_currentPeriodAdd";
            sql = "sp_currentPeriod";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@addNumber", -1);
            cd = Convert.ToInt32(sqldb.getSignalValueCmd(ref cmd));
            cmd.Dispose();
        }
        return cd;
    }
    public static int currentPeriod(int p)
    {
        int cd = 0;
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            string sql = "sp_currentPeriodAdd";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@addNumber", -1);
            cd = Convert.ToInt32(sqldb.getSignalValueCmd(ref cmd));
            cmd.Dispose();
        }
        return cd;
    }
    public static int currentPeriodAdd(int num)
    {
        int cd = 0;
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_currentPeriodAdd");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@addNumber", num);
            cd = Convert.ToInt32(sqldb.getSignalValueCmd(ref cmd));
            cmd.Dispose();
        }
        return cd;
    }
    public static double updateForecast(int uid,int oid,double amt,int period)
    {
        double rs = 0;
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            //del_sp_gam_ForecastData_add
            SqlCommand cmd = new SqlCommand("sp_gam_ForecastData_add");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@uid",uid);
            cmd.Parameters.AddWithValue("@oid",oid);
            cmd.Parameters.AddWithValue("@amt",amt);
            cmd.Parameters.AddWithValue("@period",period);
            //rs = Convert.ToDouble(sqldb.getSignalValueCmd(ref cmd));
            cmd.Dispose();
        }
        return rs;
    }
    public static DataTable getCEM_OEMS()
    {
        DataTable dt = new  DataTable();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            dt = db.getDataTable("sp_gam_getCEM_OEMs");
        }
        return dt;
    }
    public static int updateforecastPN(int period, int oemid, string customerPart, int amt, int salesId)
    {
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
        return tamt;
    }
    public static DataTable getForecastHistory(int oem_id, int forecast_period)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_ForecastData_History");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oem_id", oem_id);
            cmd.Parameters.AddWithValue("@fiscal_period", forecast_period);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static DataSet getForecastHistoryOLS(int oem_id, int forecast_period)
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_forecastData_History_OLS");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@oemid", oem_id);
            cmd.Parameters.AddWithValue("@period", forecast_period);
            ds = db.getDataSetCmd(ref cmd);
            cmd.Dispose();
        }
        return ds;
        //exec sp_gam_forecastData_History_OLS @oemid,@period
    }
    public static DataTable getForecastOutput_forSalesDL(int startPeriod, int endPeriod, int salesman, string oemName, string plant, string group, bool subSales, bool bkSales)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_forecast_output_for_salesDL");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@startPeriod", startPeriod);
            cmd.Parameters.AddWithValue("@endPeriod", endPeriod);
            cmd.Parameters.AddWithValue("@salesman", salesman);
            cmd.Parameters.AddWithValue("@oemgroup", group);
            cmd.Parameters.AddWithValue("@oem", oemName);
            cmd.Parameters.AddWithValue("@plant", plant);
            cmd.Parameters.AddWithValue("@subSales", subSales);
            cmd.Parameters.AddWithValue("@bkSales", bkSales);
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static DataTable getForecastOutput()
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_forecast_output");
            cmd.CommandType = CommandType.StoredProcedure;
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static DataTable getForecastOutputVertical()
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_forecast_output_vertical");
            cmd.CommandType = CommandType.StoredProcedure;
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static void getCEMOEM_Forecast()
    {
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_cem_forecast_mix_result");
            cmd.CommandType = CommandType.StoredProcedure;
            sqldb.execSqlWithCmd(ref cmd);
            cmd.Dispose();
        }
    }
    public static DataTable getCEMOEM_ForecastCells()
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_cem_forecast_mix_result_download");
            cmd.CommandType = CommandType.StoredProcedure;
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    public static DataTable getCEMOEM_ForecastRows()
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB sqldb = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_cem_forecast_mix_result_download_vertical");
            cmd.CommandType = CommandType.StoredProcedure;
            dt = sqldb.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }
    private static DataTable getDT(string sp)
    {
        DataTable dt = new DataTable();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand(sp);
            cmd.CommandType = CommandType.StoredProcedure;
            dt = db.getDataTableWithCmd(ref cmd);
            cmd.Dispose();
        }
        return dt;
    }

    public static DataSet getForecast(bool isCurrent)
    {
        DataSet ds = new DataSet();
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_forecast_download_all");
            cmd.Parameters.AddWithValue("@isCurrent", isCurrent);
            cmd.CommandType = CommandType.StoredProcedure;
            ds = db.getDataSetCmd(ref cmd);
            cmd.Dispose();
        }
        return ds;
    }
    public static DataTable getB2F_forecastCells()
    {
        return getDT("sp_gam_b2f_forecast_result");
    }
    public static DataTable getB2F_forecastRows()
    {
        return getDT("sp_gam_b2f_forecast_result_vertical");
    }

    public static string submitFirstLock_salesmanLog()
    {
        string msg = "";
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand("sp_gam_AutoSave_Forecast_1SalesLog");
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter _msg = cmd.Parameters.AddWithValue("@msg", "");
            _msg.Size = 100;
            _msg.Direction = ParameterDirection.Output;
            db.execSqlWithCmd(ref cmd);
            msg = _msg.Value.ToString();
        }
        return msg;
    }
}
