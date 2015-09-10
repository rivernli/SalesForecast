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
using System.IO;
using System.Text;

public partial class admini : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void download_forecast_output_Click(object sender, EventArgs e)
    {
        DataTable dt = Forecast.getForecastOutput();
        downloadOutputH(dt, "ForecastOutput_H.xls");
    }
    private void downloadOutputH(DataTable dt, String fName)
    {
        HttpContext context = HttpContext.Current;
        context.Response.Clear();
        context.Response.Charset = "";
        //context.Response.AddHeader("content-disposition", "attachment;filename=ForecastOutput.xls");
        context.Response.AddHeader("content-disposition", "attachment;filename="+ fName);
        context.Response.ContentType = "application/vnd.ms-excel";
        StreamReader sr = new StreamReader(Context.Server.MapPath("xml/excelTemp.xml"));
        string rptxml = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();


        string content = "<Row>" +
            "<Cell><Data ss:Type=\"String\">Salesman</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Cus OEM</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Baan OEM</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Plant</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">P/N</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ITEM</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Sales ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">cal ASP Period</Data>" +
            "</Cell><Cell><Data ss:Type=\"String\">max.ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">min.ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ttl.AMT.</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ttl.Qty</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ttl.SQFT</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Layer</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Tech</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Surf</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Remark</Data></Cell>";
        string rowxml = "<Row>" +
            "<Cell><Data ss:Type=\"String\">{0}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{1}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{2}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{3}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{4}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{5}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{6}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{7}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{8}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{9}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{10}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{11}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{12}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{13}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{14}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{15}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{16}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{17}</Data></Cell>";

        for (int i = 18; i < dt.Columns.Count; i++)
        {
            content += "<Cell><Data ss:Type=\"String\">"+ dt.Columns[i].ColumnName +"</Data></Cell>";
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
    protected void download_forecast_outpu_row_Click(object sender, EventArgs e)
    {
        DataTable dt = Forecast.getForecastOutputVertical();
        downloadOutputV(dt,"ForecastOutput_V.xls");
    }
    private void downloadOutputV(DataTable dt, String fName)
    {
        HttpContext context = HttpContext.Current;
        context.Response.Clear();
        context.Response.Charset = "";
        //context.Response.AddHeader("content-disposition", "attachment;filename=ForecastOutputV.xls");
        context.Response.AddHeader("content-disposition", "attachment;filename="+ fName);
        context.Response.ContentType = "application/vnd.ms-excel";
        StreamReader sr = new StreamReader(Context.Server.MapPath("xml/excelTemp.xml"));
        string rptxml = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();


        string content = "<Row>" +
            "<Cell><Data ss:Type=\"String\">Forecast Period</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">P/N</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Forecast Amount</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Salesman</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">OEMID</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">cusOEM</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">BaanOEM</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Plant</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Item</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">cal ASP period</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">sales input ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">max. ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">min. ASP</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Layer</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Tech</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Surf</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ttl. Amt</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">ttl. SQFT</Data>" +
            "</Cell><Cell><Data ss:Type=\"String\">ttl. QTY</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">Remark</Data></Cell>"+
            "</Row>";
            /*
            "<Cell><Data ss:Type=\"String\">Month</Data></Cell>"+
            "<Cell><Data ss:Type=\"String\">Quarter </Data></Cell>"+
            "</Row>";
             * */
        string rowxml = "<Row>" +
            "<Cell><Data ss:Type=\"Number\">{0}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{1}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{2}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{3}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{4}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{5}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{6}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{7}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{8}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{9}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{10}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{11}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{12}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{13}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{14}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{15}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{16}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{17}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{18}</Data></Cell>" +
            "<Cell><Data ss:Type=\"Number\">{19}</Data></Cell>" +
            "<Cell><Data ss:Type=\"String\">{20}</Data></Cell>" +
            "</Row>";
            //"<Cell><Data ss:Type=\"String\">{21}</Data></Cell><Cell><Data ss:Type=\"String\">{22}</Data></Cell></Row>";

        //DataTable dt = Forecast.getForecastOutputVertical();
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
    private string getXML()
    {
        StreamReader sr = new StreamReader(Context.Server.MapPath("xml/excelTemp.xml"));
        string rptxml = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();
        return rptxml;
    }


    protected void download_forecast_oem_cem_Click(object sender, EventArgs e)
    {
        Forecast.getCEMOEM_Forecast();
        sync_oem_cem.Text = "Updated On " + DateTime.Now.ToShortTimeString();
    }
    protected void download_forecast_outpu_row0_Click(object sender, EventArgs e)
    {
        //cell
        DataTable dt = Forecast.getCEMOEM_ForecastCells();
        downloadOutputH(dt,"ForecastOutput_H.xls");
    }
    protected void download_forecast_outpu_row1_Click(object sender, EventArgs e)
    {
        //row
        DataTable dt = Forecast.getCEMOEM_ForecastRows();
        downloadOutputV(dt, "ForecastOutput_V.xls");
    }
    protected void log_salesman_forecast_Click(object sender, EventArgs e)
    {
        string msg = Forecast.submitFirstLock_salesmanLog();
        firstLock.Text = msg;

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        downloadOutputV(Forecast.getB2F_forecastRows(), "ForecastOutput_B2F_V.xls");
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        downloadOutputH(Forecast.getB2F_forecastCells(), "ForecastOutput_B2F_H.xls");

    }

    protected void download_current_forecast_Click(object sender, EventArgs e)
    {
        DataSet ds = Forecast.getForecast(true);
        Multek.Util.DS2Excel(ds, "CurrentMonthForecast");
    }

    protected void download_new_forecast_Click(object sender, EventArgs e)
    {
        DataSet ds = Forecast.getForecast(false);
        Multek.Util.DS2Excel(ds, "Forecast");

    }
}
