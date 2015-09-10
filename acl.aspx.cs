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

public partial class acl : System.Web.UI.Page
{
    nUser Me;
    bool isWebAdmin = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usr"] != null)
        {
            Me = (nUser)Session["usr"];
            if (!Me.isAdmin)
                Response.Redirect("default.aspx");
            isWebAdmin = Me.isWebAdmin();
        }
        else
        {
            //if(User.Identity.IsAuthenticated)
            //Me = new nUser(Page.User.Identity);
            Response.Redirect("default.aspx");
        }

        if (!IsPostBack)
        {

            ViewState["GroupSearch"] = false;
            string[] items = new string[] { "", "Management", "Admin", "GAM", "BDM", "China Sales", "HK Sales", "China CS", "HK CS", "Others", "Disabled" };
            foreach (string k in items)
            {
                DropDownList1.Items.Add(k);
            }
            DropDownList1.DataBind();
            
            DataPager1.SetPageProperties(0, DataPager1.PageSize, true);
            ListView1.SelectedIndex = -1; 
            loadData();
        }
    }
    private void loadData()
    {
        string uid = TextBox1.Text.Trim();
        if ((bool)ViewState["GroupSearch"])
            ListView1.DataSource = Me.getDbUsersByGroup(DropDownList1.SelectedValue.ToString());
        else
            ListView1.DataSource = Me.getDbUsers(uid, domainList.SelectedValue);
        ListView1.DataBind();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        ViewState["GroupSearch"] = false;
        Panel1.Visible = false;
        ListView1.Visible = searchPanel.Visible = true;
        Label3.Text = "";
        bool reload = true;
        if (TextBox1.Text.Trim() != "")
        {
            nUser usr = new nUser(TextBox1.Text, domainList.SelectedValue);
            if (usr.isLdapUser)
            {
                reload = false;
                if (usr.isDBuser)
                {
                    Label3.Text = "User " + usr.UID + " existed";
                    DataPager1.SetPageProperties(0, DataPager1.PageSize, true);
                    ListView1.SelectedIndex = -1; 
                    loadData();
                }
                else
                {
                    userLabel.Text = usr.UID;
                    nameLabel.Text = usr.userName;
                    domainLabel.Text = usr.domain;
                    departmentLabel.Text = usr.department;
                    titleLabel.Text = usr.jobTitle;
                    emailLabel.Text = usr.emailAddress;
                    mobileLabel.Text = usr.mobile;
                    telLabel.Text = usr.tel;
                    faxLabel.Text = usr.fax;
                    Panel1.Visible = true;
                    ListView1.Visible = searchPanel.Visible = false;
                    DropDownList2.Items.AddRange(DropDownList1.Items.OfType<ListItem>().ToArray()); 
                    DropDownList2.DataBind();
                    TextBox1.Text = "";
                }
            }
        }
        if (reload)
        {
            DataPager1.SetPageProperties(0, DataPager1.PageSize, true);
            ListView1.SelectedIndex = -1;
            loadData();
        }
    }
    protected void addUserBtn_Click(object sender, EventArgs e)
    {
        nUser usr = new nUser(userLabel.Text, domainLabel.Text);
        if (usr.isLdapUser && !usr.isDBuser)
        {
            usr.isActive = isActive.Checked;
            usr.isAdmin = isAdmin.Checked;
            usr.isSales = isSales.Checked;
            usr.isReportViewer = isReportViewer.Checked;
            usr.isPriceView = isPriceView.Checked;
            usr.uGroup = DropDownList2.SelectedValue.ToString();
            usr.addDBUser();
        }
        Panel1.Visible = false;
        ListView1.Visible = searchPanel.Visible = true;
        loadData();
    }
    protected void cancelAddUserBtn_Click(object sender, EventArgs e)
    {
        Panel1.Visible = false;
        ListView1.Visible = searchPanel.Visible = true;
    }
    protected void ListView1_ItemCanceling(object sender, ListViewCancelEventArgs e)
    {
        ListView1.EditIndex = -1;
        loadData();
    }
    protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "Update")
        {
            string u = ((Label)e.Item.FindControl("uid")).Text.ToString();
            string dom = ((Label)e.Item.FindControl("dom")).Text.ToString();
            nUser usr = new nUser(u, dom);
            usr.isActive = ((CheckBox)e.Item.FindControl("isActive")).Checked;
            usr.isAdmin = ((CheckBox)e.Item.FindControl("isAdmin")).Checked;
            usr.isSales = ((CheckBox)e.Item.FindControl("isSales")).Checked;
            usr.isReportViewer = ((CheckBox)e.Item.FindControl("isReportViewer")).Checked;
            usr.isPriceView = ((CheckBox)e.Item.FindControl("isPriceView")).Checked;
            usr.uGroup = ((DropDownList)e.Item.FindControl("uGroupList")).SelectedValue.ToString();
            usr.updateDBUser();
            //nLog.addLog(Me.uid, "Update User", usr.uid + " Updated", Request.Url.ToString(), Session.SessionID);
        }
    }
    protected void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            string itemUid = ((Label)e.Item.FindControl("uid")).Text;
            if (nUser.isWebmin(itemUid))
            {
                if (e.Item.FindControl("editBtn") != null)
                    e.Item.FindControl("editBtn").Visible = false;
                if (e.Item.FindControl("ImageButton2") != null)
                    e.Item.FindControl("ImageButton2").Visible = false; 
            }
            else if (((Label)e.Item.FindControl("isAd")).Text == "Y" && !isWebAdmin)
            {
                if (e.Item.FindControl("editBtn") != null)
                    ((ImageButton)e.Item.FindControl("editBtn")).Visible = false;
                else
                    ((ImageButton)e.Item.FindControl("ImageButton2")).Visible = false;
            }
            else
            {
                if (e.Item.FindControl("uGroupList") != null)
                {
                    DropDownList ddl = (DropDownList)e.Item.FindControl("uGroupList");
                    ddl.Items.AddRange(DropDownList1.Items.OfType<ListItem>().ToArray());
                    string u = ((Label)e.Item.FindControl("uGroup")).Text.Trim();
                    for (int i = 0; i < ddl.Items.Count; i++)
                    {
                        if (ddl.Items[i].Value.Trim() == u)
                        {
                            ddl.SelectedIndex = i;
                            break;
                        }
                    }
                    ddl.DataBind();
                }
            }
        }
    }

    protected void ListView1_ItemEditing(object sender, ListViewEditEventArgs e)
    {
        ListView1.EditIndex = e.NewEditIndex;
        loadData();
    }
    protected void ListView1_ItemUpdating(object sender, ListViewUpdateEventArgs e)
    {
        ListView1.EditIndex = -1;
        loadData();
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        ViewState["GroupSearch"] = true;
        Panel1.Visible = false;
        ListView1.Visible = searchPanel.Visible = true;
        Label3.Text = "";

        DataPager1.SetPageProperties(0, DataPager1.PageSize, true);
        ListView1.SelectedIndex = -1;
        loadData();
    }

    protected void ListView1_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        DataPager1.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        ListView1.SelectedIndex = -1;
        loadData();
    }
}
