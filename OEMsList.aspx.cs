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
using System.Text;
using System.IO;

namespace SalesForecast
{
    public partial class OEMsList : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadCusOEM();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideDisplayBox", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(hideDisplayBox);", true);
            }
        }

        private void loadCusOEM()
        {
            loadCusOEMData();
            salesman_tbx.Attributes.Add("onkeyup", "checkSales(this,'" + SalesmanID.ClientID + "')");
        }

        private void loadCusOEMData()
        {
            CusOEMList.DataSource = OEMCus.List(keyword.Text.Trim(), salesman_tbx.Text.Trim(), Convert.ToInt32(status.SelectedValue));
            CusOEMList.DataBind();
        }

        protected void searchBaanOEM_Click(object sender, EventArgs e)
        {
            loadCusOEMData();
        }
        protected void DataPager1_PreRender(object sender, EventArgs e)
        {
            loadCusOEMData();
        }

        private void genExcelByXML()
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.Charset = "";
            context.Response.AddHeader("content-disposition", "attachment;filename=OEMlist.xls");
            context.Response.ContentType = "application/vnd.ms-excel";
            StreamReader sr = new StreamReader(Context.Server.MapPath("xml/excelTemp.xml"));
            string rptxml = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            string content = "<Row>" +
                "<Cell><Data ss:Type=\"String\">Cus OEM</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">Baan OEM</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">Plant</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">Group</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">1st Salesman</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">2nd Salesman</Data></Cell></Row>";
            string rowxml = "<Row><Cell><Data ss:Type=\"String\">{0}</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">{1}</Data></Cell><Cell><Data ss:Type=\"String\">{2}</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">{3}</Data></Cell><Cell><Data ss:Type=\"String\">{4}</Data></Cell>" +
                "<Cell><Data ss:Type=\"String\">{5}</Data></Cell></Row>";

            DataTable dt = OEMCus.List(keyword.Text.Trim(), salesman_tbx.Text.Trim(), Convert.ToInt32(status.SelectedValue));
            if (dt.Rows.Count == 50000)
                content = "<Row>" +
                    "<Cell ss:StyleID=\"s71\"><Data ss:Type=\"String\">Warning: Your downloaded result has reached the limit of the number of 50000. It will probably not your expected.</Data></Cell>" +
                    "</Row>" + content;

            StringBuilder sb = new StringBuilder();
            sb.Append(content);
            foreach (DataRow row in dt.Rows)
            {
                sb.Append(string.Format(rowxml, row["cusOEM"].ToString().Trim().Replace("&", "&amp;"),
                    row["OEMName"].ToString().Trim().Replace("&", "&amp;"), row["plant"],
                    row["groupName"].ToString().Trim().Replace("&", "&amp;"),
                    row["userName"], row["vName"]));
            }
            dt.Dispose();
            rptxml = rptxml.Replace("<Row />", sb.ToString());
            Response.Write(rptxml);
            Response.End();
        }

        protected void downloadOEM_Click(object sender, EventArgs e)
        {
            genExcelByXML();
        }
    }
}