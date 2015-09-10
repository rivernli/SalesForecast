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

public partial class salesmanRelation : System.Web.UI.Page
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
            loadTree();
        }
    }
    private void loadTree()
    {
        if (TreeView1.Nodes.Count > 0)
            TreeView1.Nodes.Remove(TreeView1.Nodes[0]);
        /*
        TreeNode blk = new TreeNode("<span style='font-weight:bold; font-size:14px;'>Salesman Relation Control</span>", "-1");
        TreeView1.Nodes.Add(blk);*/
        TreeNode root = new TreeNode("<span>Globel Account Manager<span>", "0");
        root.ImageUrl = "images/worldLink.png";
        TreeView1.Nodes.Add(root);
        reloadSubTree(root,false);
        root.Expand();
        TreeView1.DataBind();
    }
    private void reloadSubTree(TreeNode node, bool exp)
    {
        node.ChildNodes.Clear();
        int id = Convert.ToInt32(node.Value);
        DataTable dt = SalesmanCtrl.Salesman.getsByManager(id);
        foreach (DataRow row in dt.Rows)
        {
            TreeNode n = new TreeNode(row["userName"].ToString(), row["sysUserId"].ToString());
            node.ChildNodes.Add(n);
            reloadSubTree(n,false);
            if (exp)
                node.Expand();
            else
                node.Collapse();
        }
    }
    private void NodeMove(TreeNode node)
    {
        temp.Text = "yes move..";
        ImageButton2.ImageUrl = "images/will_move.png";
        if (ViewState["node"] != null)
        {
            string oldpath = ViewState["node"].ToString();
            ViewState["node"] = null;
            if (node.ValuePath.IndexOf(oldpath, 0) == 0)
            {
                temp.Text = "Cannot move parent to child";
            }
            else
            {
                temp.Text = "yes starting move.";
                string newpath = node.ValuePath;
                TreeNode mNode = TreeView1.FindNode(oldpath);
                SalesmanCtrl.Salesman.update(Convert.ToInt32(node.Value), Convert.ToInt32(mNode.Value));
                reloadSubTree(mNode.Parent, true);
                TreeNode nNode = TreeView1.FindNode(newpath);
                nNode.Select();
                reloadSubTree(nNode, true);
            }
        }
    }
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        temp.Text = "";
        TreeNode node = TreeView1.SelectedNode;
        if (ImageButton2.ImageUrl.IndexOf("ready_move", 0) >= 0)
            NodeMove(node);
        
        salesDetal.Visible = false;
        int id = Convert.ToInt32(node.Value);
        if (id > 0)
        {
            salesDetal.Visible = true;
            SalesmanCtrl.Salesman sales = new SalesmanCtrl.Salesman(id);
            userName.Text = sales.user.userName;
            department.Text = sales.user.department;
            title.Text = sales.user.jobTitle;
            userId.Text = id.ToString();
            oemlist.DataSource = OEMCus.ListBySaleId(sales.user.sysUserId);
            oemlist.DataBind();
        }
    }

    protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
    {
        if (ImageButton2.ImageUrl.IndexOf("will_move", 0) >= 0)
        {
            ViewState["node"] = TreeView1.SelectedNode.ValuePath;
            ImageButton2.ImageUrl = "images/ready_move.png";
        }
        else
        {
            ViewState["node"] = null;
            ImageButton2.ImageUrl = "images/will_move.png";
        }

    }
}
