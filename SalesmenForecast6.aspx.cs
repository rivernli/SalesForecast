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
using System.Data.SqlClient;
using System.IO;
using System.Text;

public partial class SalesmenForecast6 : System.Web.UI.Page
{
    nUser usr;
    string[] k = { "Admin", "Management" };
    private string __conn = ConfigurationManager.ConnectionStrings["GAMconn"].ToString();
    private void loadSumittion(int salesId)
    {
        if (salesId > 0)
        {
            string time = nUser.submit_finish_forecase_get(salesId);
            if (time != "")
                SaleSubmit.Text = "Your last submission time: " + time;
        }
        else
        {
            SaleSubmit.Text = "";
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        temp.Text = "";
        
        if (Session["usr"] != null)
            usr = (nUser)Session["usr"];
        else
            Response.Redirect("default.aspx");

        if (!IsPostBack)
        {
            if (k.Contains(usr.uGroup) || usr.isAdmin)
            {
                adminFeature.Visible = true;
            }
            loadSumittion(usr.sysUserId);
            currentPeriod.Text = Forecast.currentPeriod().ToString().Trim();
            setYear();
            selectSales.Attributes.Add("onkeyup", "Sales.checkSales(this,'" + selectSalesId.ClientID + "')");
            Button1.Attributes.Add("onclick", "hideDisplayBox()");
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(forecastPage.loadCompleted);", true);
        }
    }
    private void setYear()
    {
        //string nextPeriod = Forecast.currentPeriodAdd(1).ToString().Trim();
        string nextPeriod = currentPeriod.Text;
        int x = Convert.ToInt32(nextPeriod.Substring(0, 4));
        int y = Convert.ToInt32(nextPeriod.Substring(4));
        startFY.Items.Clear();
        int cM = DateTime.Today.Month;
        int endYear = DateTime.Today.Year + 1;
        if (cM > 3)
            endYear++;

        for (int i = 2010; i <= endYear; i++)
        {
            startFY.Items.Add(new ListItem("FY "+i.ToString(), i.ToString()));
            if (i == x)
                startFY.SelectedIndex = startFY.Items.Count - 1;
        }

        startPeriod.Items.Clear();
        string[] month = { "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar" };
        for (int j = 1; j <= 12; j++)
        {
            startPeriod.Items.Add(new ListItem(month[j - 1] + " (P" + j.ToString() + ")", j.ToString()));
        } 
        startPeriod.SelectedIndex = y - 1;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        list1.SelectedIndex = -1;
        loadList();
    }
    protected void Download1_Click(object sender, EventArgs e)
    {
        downloadOEM();
        //downloadMaster2();
    }
    private void loadList()
    {
        int sp = Convert.ToInt32(startFY.Text + "00") + Convert.ToInt32(startPeriod.Text);
        int ep = sp + 100;
        int saleId = usr.sysUserId;
        string oem = "";
        string plant = "";
        string group = "";
        if(k.Contains(usr.uGroup) || usr.isAdmin)
        {
            saleId = 0;
            if (selectSalesId.Text.Trim() != "" && selectSales.Text.Trim() != "")
                int.TryParse(selectSalesId.Text, out saleId);

            loadSumittion(saleId);
        }
        oem = selectOEM.Text.Trim();
        plant = selectPlant.Text.Trim();
        group = selectGroup.Text.Trim();

        DataTable dt = Forecast.getForecastOnly(sp, ep, saleId, oem, plant, group, subSales.Checked, bkSales.Checked);
        list1.DataSource = dt;
        list1.DataBind();
        if (dt.Rows.Count <= 0)
            temp.Text = "No record found. please input correct criteria.";

        for (int j = 1; j < 4; j++)
        {
            Label lb = (Label)list1.FindControl("curr_" + j.ToString());
            if (lb != null)
            {
                string[] xp = dt.Columns[j + 2].ColumnName.Split(new char[] { ' ' });
                if(xp.Count() > 1)
                    lb.Text = "Actual<br/>" + Multek.Util.getPeriod(Convert.ToInt32(xp[0]));
            }
        }
        for(int j =4; j<6;j++)
        {
            Label lb = (Label)list1.FindControl("load_" + j.ToString());
            if (lb != null)
            {
                string[] xp = dt.Columns[j + 2].ColumnName.Split(new char[] { ' ' });
                if (xp.Count() > 1)
                    lb.Text = "Loading<br/>" + Multek.Util.getPeriod(Convert.ToInt32(xp[0]));
            }
        }

        for (int i = 1; i <= 12; i++)
        {
            Label lb = (Label)list1.FindControl("Label" + i.ToString());
            if (lb != null)
            {
                string[] dp = dt.Columns[i + 7].ColumnName.Split(new char[] {' '});
                lb.Text = Multek.Util.getPeriod(Convert.ToInt32(dp[0]));
                lb.Font.Bold = true;
                ((Label)list1.FindControl("p" + i.ToString() )).Text = dp[0];
            }
        }
        if (dt.Rows.Count > 0)
        {
            
            for (int i = 3; i < 8; i++)
            {
                string label = "tt" + (i - 2).ToString();
                ((Label)list1.FindControl(label)).Text = Convert.ToInt32(dt.Compute("sum([" + dt.Columns[i].ColumnName + "])", null)).ToString("#,##0"); ;
            }
            bool copy = true;

            for(int i=8; i < dt.Columns.Count;i++){
                string label = "ttl" + (i - 7).ToString();
                Label lb = (Label)list1.FindControl(label);
                string cln = dt.Columns[i].ColumnName;
                int num = Convert.ToInt32(dt.Compute("sum([" + cln + "])", null));
                lb.Text = num.ToString("#,##0");
                LinkButton btn = (LinkButton)list1.FindControl("b" + label);
                lb.Visible = true;
                btn.Visible = false;
                if (saleId > 0 && num == 0 && copy)
                {
                    if (Convert.ToInt32(cln.Substring(0, 6)) >= (Convert.ToInt32(currentPeriod.Text) + 100))
                    {
                        lb.Text = cln.Substring(0, 6) +","+ saleId.ToString();
                        btn.Text = "<-Copy";
                        btn.Visible = true;
                        lb.Visible = false;
                        copy = false;
                    }
                }
            }
        }
    }

    protected void list1_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.FindControl("selOEM") != null)
        {
            LinkButton lbt = (LinkButton)e.Item.FindControl("selOEM");
            if (lbt.Text.ToLower() == "total")
            {
                lbt.Enabled = false;
                lbt.Attributes.Remove("onclick");
            }
        }
        if (e.Item.FindControl("oemRemark") != null)
        {
            Image img = (Image)e.Item.FindControl("oemRemark");
            Label oid = (Label)e.Item.FindControl("oemid");
            img.Attributes.Add("onclick", "pn.open4Comments(this,"+ oid.Text.ToString() +")");
            img.Attributes.CssStyle.Add("cursor", "pointer");
        }
        if (e.Item.FindControl("list3") != null)
        {
            ListView dl2 = (ListView)e.Item.FindControl("list3");
            Label oid = (Label)e.Item.FindControl("oemid");
            int oemid = Convert.ToInt32(oid.Text);
            selectedOEMID.Text = oemid.ToString();

            int sp = Convert.ToInt32(startFY.Text + "00") + Convert.ToInt32(startPeriod.Text);
            DataTable dt = GETFS(sp, oemid);
            dl2.DataSource = dt;
            dl2.DataBind();
            if (dl2.InsertItem.FindControl("add") != null)
            {
                string isB2f = OEMCus.oemType(oemid);
                Button im = (Button)dl2.InsertItem.FindControl("add");
                string cid = im.ClientID;
                im.Attributes.Add("style", "display:none");
                TextBox newPartNo = (TextBox)dl2.InsertItem.FindControl("newPN");
                if (newPartNo != null)
                {
                    newPartNo.Attributes.Add("readonly", "readonly");
                    newPartNo.Attributes.Add("onclick", "udfPN.callNewPN(this,'" + cid + "','"+ isB2f +"')");
                    newPartNo.Attributes.Add("onmouseover", "overText(this)");
                    newPartNo.Attributes.Add("onmouseout", "outText(this)");
                }
            }
        }
    }

    public DataTable GETFS(int period, int oemid)
    {
        
        DataTable dt = new DataTable();
        string oem, baan, plant;
        using (Multek.SqlDB db = new Multek.SqlDB(__conn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "[sp_gam_ForecastCustomerPart_Get]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@oemid", oemid);
            cmd.Parameters.AddWithValue("@periodLong", 3);
            cmd.Parameters.AddWithValue("@totalPeriod", 11);

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

            oem = _oem.Value.ToString();
            baan = _baan.Value.ToString();
            plant = _plant.Value.ToString();

            bool isCEM = false;
            DataTable cems = Forecast.getCEM_OEMS();
            //temp.Text = cems.Rows[0][0].ToString();
            foreach(DataRow row in cems.Rows)
            {
                if (row[0].ToString().Trim().ToUpper() == baan.Trim().ToUpper())
                {
                    isCEM = true;
                    break;
                }
            }
            dt = modDS(ds);
            if (isCEM)
            {
                for(int i = dt.Rows.Count-1; i >=0; i--)
                {
                    DataRow row = dt.Rows[i];
                    if (row["cus_part_no"].ToString().IndexOf("[") < 0)
                        dt.Rows.Remove(row);
                }
            }
            cmd.Dispose();
        }
        return dt;
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

        /*new table3 for current -3 period*/
        DataTable dtx = ds.Tables[2];
        foreach (DataRow cpx in dtx.DefaultView.ToTable(true, new string[] { "iperiod" }).Rows)
        {
            DataColumn clx = new DataColumn("cp_"+ cpx["iperiod"].ToString(), typeof(int));
            clx.DefaultValue = 0;
            dt1.Columns.Add(clx);
        }
        /*end of new table3 for current -3 period*/

        foreach (DataRow row in dt2.Select("sid > 0"))
        {
            foreach (DataRow fr in dt1.Select("cus_part_no = '" + row["project"] + "'"))
            {
                fr[row["iperiod"].ToString()] = Convert.ToInt32(row["amt"]);
            }
        }
        dt2.Dispose();
        foreach (DataRow rowx in dtx.Select("samt > 0"))
        {
            foreach(DataRow fx in dt1.Select("cus_part_no = '"+ rowx["cus_part_no"] +"'"))
                fx["cp_"+ rowx["iperiod"].ToString()] = Convert.ToInt32(rowx["samt"]);
        }
        return dt1;
    }

    protected void list3_ItemInserting(object sender, ListViewInsertEventArgs e)
    {
        if (e.Item.FindControl("newPN") != null)
        {
            string pn = ((TextBox)e.Item.FindControl("newPN")).Text.Trim();
            if (pn != "")
            {
                if (pn.IndexOf("[") != 0 || pn.IndexOf("<") >= 0 || pn.IndexOf(">") >= 0 || pn.IndexOf("][") < 1)
                {
                    temp.Text = "Error!!!! pleases DON'T input the invalid PN!!!!";
                    return;
                }
                int oemid = Convert.ToInt32(selectedOEMID.Text);// Convert.ToInt32(((Label)list1.SelectedItem.FindControl("oemid")).Text);
                int sp = Convert.ToInt32(startFY.Text + "00") + Convert.ToInt32(startPeriod.Text);
                Forecast.updateforecastPN(sp, oemid, pn, 0, usr.sysUserId);
            }
        }
        loadList();
    }
    protected void list3_ItemDatabound(object source, ListViewItemEventArgs e)
    {
        if(e.Item.ItemType == ListViewItemType.DataItem)
        {
            if (e.Item.FindControl("pn") != null)
            {
                string pn = ((Label)e.Item.FindControl("pn")).Text;
                int oemid = Convert.ToInt32(selectedOEMID.Text);
                Image img = (Image)e.Item.FindControl("img_detail");
                img.ImageUrl = "http://bi.multek.com/ws/images/ask_blue.png";
                img.AlternateText = "Part number detail";
                img.CssClass = "pj_detail";
                img.Attributes.Add("onmouseover", "return pn.mouseOver(this)");
                img.Attributes.Add("onmouseout", "return pn.mouseOut(this)");
                img.Attributes.Add("onclick", "pn.open4AspRemarks(this," + oemid + ",'" + pn + "')");

                setForecastTextBox(e.Item, "ren1", "lab1", Convert.ToInt32(currentPeriod.Text),pn,oemid);
                setForecastTextBox(e.Item, "ren2", "lab2", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren3", "lab3", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren4", "lab4", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren5", "lab5", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren6", "lab6", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren7", "lab7", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren8", "lab8", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren9", "lab9", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren10", "lab10", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren11", "lab11", Convert.ToInt32(currentPeriod.Text), pn, oemid);
                setForecastTextBox(e.Item, "ren12", "lab12", Convert.ToInt32(currentPeriod.Text), pn, oemid);
            }
        }
    }
    private void setForecastTextBox(ListViewItem item, string boxId, string plabel, int curPeriod, string pn, int oemid)
    {
        if (item.FindControl(boxId) != null)
        {
            TextBox txtbox = (TextBox)item.FindControl(boxId);
            int period = Convert.ToInt32(((Label)item.FindControl(plabel)).Text);
            txtbox.Attributes.Add("onmouseover", "overText(this," + period + ")");
            txtbox.Attributes.Add("onmouseout", "outText(this)");
            if (period >= curPeriod)
            {
                txtbox.Attributes.Add("oemid", oemid.ToString());
                //txtbox.Attributes.Add("pn", pn);
                //txtbox.Attributes.Add("ov", txtbox.Text);
                //txtbox.Attributes.Add("onblur", "blurToUpdate(this," + oemid + "," + period + ",'" + pn + "')");
                txtbox.Attributes.Add("onblur", "fcInp.inputBlur(this," + period + ")");
                txtbox.Attributes.Add("onfocus", "fcInp.inputFocus(this)");
                txtbox.Attributes.Add("onkeydown", "keyMove(this)");
            }
            else
            {
                txtbox.Enabled = false;
            }

        }
    }
    protected void list1_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
    {
        if (list1.SelectedIndex == e.NewSelectedIndex)
            list1.SelectedIndex = -1;
        else
            list1.SelectedIndex = e.NewSelectedIndex;
        selectedOEMID.Text = "";
        loadList();
    }
    protected void finish_forecast_Click(object sender, EventArgs e)
    {
        nUser.submit_finish_forecast(usr.sysUserId, Convert.ToInt32(currentPeriod.Text));
        finish_forecast.Enabled = false;
        loadSumittion(usr.sysUserId);
    }

    private void downloadMaster2()
    {
        int sp = Convert.ToInt32(startFY.Text + "00") + Convert.ToInt32(startPeriod.Text);
        int ep = sp + 100;
        int saleId = usr.sysUserId;
        string oem = "";
        string plant = "";
        string group = "";

        if (k.Contains(usr.uGroup) || usr.isAdmin)
        {
            saleId = 0;
            if (selectSalesId.Text.Trim() != "" && selectSales.Text.Trim() != "")
                int.TryParse(selectSalesId.Text, out saleId);
        }
        oem = selectOEM.Text.Trim();
        plant = selectPlant.Text.Trim();
        group = selectGroup.Text.Trim();


        HttpContext context = HttpContext.Current;
        context.Response.Clear();
        context.Response.Charset = "";
        context.Response.AddHeader("content-disposition", "attachment;filename=ForecastOutput.xls");
        context.Response.ContentType = "application/vnd.ms-excel";
        StreamReader sr = new StreamReader(Context.Server.MapPath("xml/excelTemp.xml"));
        string rptxml = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();


        string content = "<Row>" +
            "<Cell><Data ss:Type=\"String\">Cus OEM</Data></Cell><Cell><Data ss:Type=\"String\">Baan OEM</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Plant</Data></Cell><Cell><Data ss:Type=\"String\">P/N</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ITEM</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">Sales ASP</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">ASP</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">cal ASP Period</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">max.ASP</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">min.ASP</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">ttl.AMT.</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">ttl.Qty</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">ttl.SQFT</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">Layer</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">Tech.</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">Surf</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">Remark</Data></Cell>";

        string rowxml = "<Row><Cell><Data ss:Type=\"String\">{0}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{1}</Data></Cell><Cell><Data ss:Type=\"String\">{2}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{3}</Data></Cell><Cell><Data ss:Type=\"String\">{4}</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{5}</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{6}</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{7}</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{8}</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{9}</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{10}</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{11}</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"Number\">{12}</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">{13}</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">{14}</Data></Cell><Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">{15}</Data></Cell>" +
            "<Cell ss:StyleID=\"s77\"><Data ss:Type=\"String\">{16}</Data></Cell>";

        DataTable dt = Forecast.getForecastOutput_forSalesDL(sp, ep, saleId, oem, plant, group, subSales.Checked, bkSales.Checked);
        for (int i = 17; i < dt.Columns.Count; i++)
        {
            content += "<Cell><Data ss:Type=\"String\">" + dt.Columns[i].ColumnName + "</Data></Cell>";
            rowxml += "<Cell><Data ss:Type=\"Number\">{" + i.ToString() + "}</Data></Cell>";
        }
        content += "</Row>";
        rowxml += "</Row>";

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

    private void downloadOEM()
    {
        int sp = Convert.ToInt32(startFY.Text + "00") + Convert.ToInt32(startPeriod.Text);
        int ep = sp + 100;
        int saleId = usr.sysUserId;
        string oem = "";
        string plant = "";
        string group = "";
        if (k.Contains(usr.uGroup) || usr.isAdmin)
        {
            saleId = 0;
            if (selectSalesId.Text.Trim() != "" && selectSales.Text.Trim() != "")
                int.TryParse(selectSalesId.Text, out saleId);

            loadSumittion(saleId);
        }
        oem = selectOEM.Text.Trim();
        plant = selectPlant.Text.Trim();
        group = selectGroup.Text.Trim();

        DataTable dt = Forecast.getForecastOnly(sp, ep, saleId, oem, plant, group, subSales.Checked, bkSales.Checked);
        if (dt.Rows.Count > 0)
        {
            int j;
            for (j = 3; j < 20; j++)
            {
                DataColumn dc = dt.Columns[j];
                string[] name = dc.ColumnName.Split(new char[] { ' ' });
                if (name.Count() > 0)
                {
                    string p = Multek.Util.getPeriod(Convert.ToInt32(name[0]));
                    if (j < 6)
                        p = "Actual " + p;
                    else if (j < 8)
                        p = "Loading " + p;
                    dc.ColumnName = p;
                }
            }
            dt.Columns.Remove(dt.Columns[1]);

            dt.Columns["userName"].ColumnName = "Salesman";
            dt.Columns["cusoem"].ColumnName = "OEM";
            Multek.Util.DT2Excel(dt, "output");
        }
        else
        {
            temp.Text = "No record found. please input correct criteria.";
        }
    }
    protected void bbt12_Click(object sender, EventArgs e)
    {
        LinkButton btn = ((LinkButton)sender);
        string[] data = ((Label)list1.FindControl(btn.ID.Substring(1))).Text.Split(new char[]{','});
        int sid = 0;
        int period = 1000;
        int.TryParse(data[1], out sid);
        int.TryParse(data[0], out period);
        Forecast.copyPreviousPreiodForecastData(period, sid);
        loadList();
    }
}
