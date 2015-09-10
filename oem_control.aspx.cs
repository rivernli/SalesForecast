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

public partial class oem_control : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            /* controledo n gamSetting Master page.
            if (Session["usr"] != null)
            {
                nUser Me = (nUser)Session["usr"];
                if (!Me.isAdmin)
                    Response.Redirect("default.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
            */
            loadCusOEM();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "hideDisplayBox", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(hideDisplayBox);", true);
        }
    }

    private void loadCusOEM()
    {
        loadCusOEMData();
        salesman_tbx.Attributes.Add("onkeyup", "checkSales(this,'"+ SalesmanID.ClientID +"')");
    }

    private void loadCusOEMData()
    {
        CusOEMList.DataSource = OEMCus.List(keyword.Text.Trim(),salesman_tbx.Text.Trim(),Convert.ToInt32(status.SelectedValue));
        CusOEMList.DataBind();
    }
    private void resetCusOEMData()
    {
        CusOEMList.EditIndex = -1;
        CusOEMList.InsertItemPosition = InsertItemPosition.None;
        //keyword.Text = "";
        //salesman_tbx.Text = "";
        loadCusOEMData();
        ViewSalesmanID.Text = BaanOEMID.Text = SalesmanID.Text = "0";
    }
    protected void add_new_Click(object sender, EventArgs e)
    {
        CusOEMList.SelectedIndex = CusOEMList.EditIndex = -1;
        CusOEMList.InsertItemPosition = InsertItemPosition.FirstItem;
        loadCusOEMData();
        /*
        TextBox slm = (TextBox)CusOEMList.InsertItem.FindControl("salesman");
        slm.Attributes.Add("onkeyup", "checkSales(this,'"+ SalesmanID.ClientID +"')");
        TextBox baanoem = (TextBox)CusOEMList.InsertItem.FindControl("baanOEM");
        baanoem.Attributes.Add("onkeyup", "checkBaanOEM(this)");*/

    }
    protected void CusOEMList_ItemCanceling(object sender, ListViewCancelEventArgs e)
    {
        resetCusOEMData();
    }
    protected void CusOEMList_ItemUpdating(object sender, ListViewUpdateEventArgs e)
    {
        int id = Convert.ToInt32(((Label)CusOEMList.EditItem.FindControl("id_oem")).Text);
        if (id > 0)
        {
            OEMCus oem = new OEMCus(id);
            oem.BaanId = Convert.ToInt32(BaanOEMID.Text);
            oem.salesmanId = Convert.ToInt32(SalesmanID.Text);
            oem.CusOEM = ((TextBox)CusOEMList.EditItem.FindControl("cusOEM")).Text.Trim();
            oem.viewSalesmanId = Convert.ToInt32(ViewSalesmanID.Text);
            oem.isValid = ((CheckBox)CusOEMList.EditItem.FindControl("isValid")).Checked;
            if (oem.CusOEM != "")
            {
                oem.Update();
            }
        }
        loadCusOEMData();
        resetCusOEMData();
    }
    protected void CusOEMList_ItemEditing(object sender, ListViewEditEventArgs e)
    {
        CusOEMList.EditIndex = e.NewEditIndex;
        CusOEMList.InsertItemPosition = InsertItemPosition.None;
        loadCusOEMData();
        /*
        BaanOEMID.Text = ((Label)CusOEMList.EditItem.FindControl("id_baanOEM")).Text;
        SalesmanID.Text = ((Label)CusOEMList.EditItem.FindControl("id_salesman")).Text;
        TextBox slm = (TextBox)CusOEMList.EditItem.FindControl("salesman");
        slm.Attributes.Add("onkeyup", "checkSales(this)");
        TextBox baanoem = (TextBox)CusOEMList.EditItem.FindControl("baanOEM");
        baanoem.Attributes.Add("onkeyup", "checkBaanOEM(this)");*/
    }
    protected void CusOEMList_ItemInserting(object sender, ListViewInsertEventArgs e)
    {
        OEMCus oem = new OEMCus();
        oem.BaanId = Convert.ToInt32(BaanOEMID.Text);
        oem.salesmanId = Convert.ToInt32(SalesmanID.Text);
        oem.CusOEM = ((TextBox)e.Item.FindControl("cusOEM")).Text.Trim();
        oem.viewSalesmanId = Convert.ToInt32(ViewSalesmanID.Text);
        oem.isValid = ((CheckBox)e.Item.FindControl("isValid")).Checked;
        if (oem.CusOEM != "")
        {
            oem.Add();
        }
        resetCusOEMData();
    }
    protected void searchBaanOEM_Click(object sender, EventArgs e)
    {
        loadCusOEMData();
    }
    protected void DataPager1_PreRender(object sender, EventArgs e)
    {
        loadCusOEMData();
        if (CusOEMList.EditIndex >= 0)
        {
            BaanOEMID.Text = ((Label)CusOEMList.EditItem.FindControl("id_baanOEM")).Text;
            SalesmanID.Text = ((Label)CusOEMList.EditItem.FindControl("id_salesman")).Text;
            TextBox slm = (TextBox)CusOEMList.EditItem.FindControl("salesman");
            slm.Attributes.Add("onkeyup", "checkSales(this,'"+ SalesmanID.ClientID +"')");
            TextBox baanoem = (TextBox)CusOEMList.EditItem.FindControl("baanOEM");
            baanoem.Attributes.Add("onkeyup", "checkBaanOEM(this)");

            ViewSalesmanID.Text = ((Label)CusOEMList.EditItem.FindControl("id_vsalesman")).Text;
            TextBox vlm = (TextBox)CusOEMList.EditItem.FindControl("vsalesman");
            vlm.Attributes.Add("onkeyup", "checkSales(this,'" + ViewSalesmanID.ClientID +"')");

            /******load backup sales list******/

            int id_oem = Convert.ToInt32(((Label)CusOEMList.EditItem.FindControl("id_oem")).Text);
            TextBox bksalesid = (TextBox)CusOEMList.EditItem.FindControl("bksalesmanId");
            TextBox bksales = (TextBox)CusOEMList.EditItem.FindControl("bksalesman");
            bksales.Attributes.Add("onkeyup", "checkSales(this,'" + bksalesid.ClientID + "')");
            ListView gv = (ListView)CusOEMList.EditItem.FindControl("bklist");
            gv.DataSource = OEMCus.bkSalesList(id_oem);
            gv.DataBind();
        }
        else if (CusOEMList.InsertItemPosition == InsertItemPosition.FirstItem)
        {
            TextBox slm = (TextBox)CusOEMList.InsertItem.FindControl("salesman");
            slm.Attributes.Add("onkeyup", "checkSales(this,'" + SalesmanID.ClientID + "')");
            TextBox baanoem = (TextBox)CusOEMList.InsertItem.FindControl("baanOEM");
            baanoem.Attributes.Add("onkeyup", "checkBaanOEM(this)");
            TextBox vlm = (TextBox)CusOEMList.InsertItem.FindControl("vsalesman");
            vlm.Attributes.Add("onkeyup", "checkSales(this,'" + ViewSalesmanID.ClientID + "')");
        }
    }
    protected void bkList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "del")
        {
            int idx = Convert.ToInt32(((Label)e.Item.FindControl("id")).Text);
            OEMCus.bkSalesDel(idx);
        }
    }
    protected void CusOEMList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "addBKSales")
        {

            int id_oem = Convert.ToInt32(((Label)CusOEMList.EditItem.FindControl("id_oem")).Text);
            int sid = 0;// Convert.ToInt32(((TextBox)CusOEMList.EditItem.FindControl("bksalesmanId")).Text);
            Int32.TryParse(((TextBox)CusOEMList.EditItem.FindControl("bksalesmanId")).Text,out sid);
            if(sid > 0)
                OEMCus.bkSalesAdd(id_oem, sid);
        }
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
