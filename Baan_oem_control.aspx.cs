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

public partial class Baan_oem_control : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            /*controledo n gamSetting Master page.
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
            loadBaanOEM();
        }
    }
    protected void BaanOEMList_ItemEditing(object sender, ListViewEditEventArgs e)
    {
        BaanOEMList.EditIndex = e.NewEditIndex;
        loadData();
    }
    protected void BaanOEMList_ItemCanceling(object sender, ListViewCancelEventArgs e)
    {
        BaanOEMList.EditIndex = -1;
        loadData();
    }
    protected void BaanOEMList_ItemUpdating(object sender, ListViewUpdateEventArgs e)
    {
        ListViewItem itm = BaanOEMList.Items[e.ItemIndex];
        string gn = ((TextBox)itm.FindControl("groupName")).Text.Trim();
        int id = Convert.ToInt32(((Label)itm.FindControl("BaanOEMId")).Text);
        OEMBaan oem = new OEMBaan(id);
        oem.GroupName = gn;
        oem.update();
        BaanOEMList.EditIndex = -1;
        loadData();
    }
    protected void DataPager1_PreRender(object sender, EventArgs e)
    {
        loadData();
    }
    private void loadBaanOEM()
    {
        loadData();
        loadGroups();
    }
    private void loadGroups()
    {
        DropDownList1.DataSource = OEMBaan.getGroup();
        DropDownList1.DataTextField = "gn";
        DropDownList1.DataValueField = "groupName";
        DropDownList1.DataBind();
        DropDownList1.Items.Insert(0, new ListItem("", ""));
    }
    private void loadData()
    {
        BaanOEMList.DataSource = OEMBaan.searchOEM(keyBaanOEM.Text.Trim(), DropDownList1.SelectedValue.Trim(), 0);
        BaanOEMList.DataBind();
    }
    protected void searchBaanOEM_Click(object sender, EventArgs e)
    {
        BaanOEMList.EditIndex = -1;
        DataPager1.SetPageProperties(0, 20, true);
    }
}
