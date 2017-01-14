using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Designer_TestBlocks : System.Web.UI.Page
{
    int BlockID { get { return Convert.ToInt16(Request.QueryString["BlockID"]); } }
    int TestID { get { return Convert.ToInt16 ( ViewState["TestID"]); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            Test_Question tq = dc.Test_Questions.Where(p => p.id == BlockID).FirstOrDefault();

            tbName.Text = tq.text;
            tbNomer.Text = Convert.ToString (tq.number);
            cbxIsShuffledItm.Checked = tq.isShuffledItem;
            cbxIsShuffledAns.Checked = tq.isShuffledAns;
            cbxIsTimeRestricted.Checked = tq.isTimeRestrict;
            txtBoxInstruction.Text = tq.instruction;
            txtBoxComment.Text = tq.comment;
            ViewState["TestID"] = tq.test_id;

            hlGotoTest.NavigateUrl = string.Format("~/Designer/EditTest.aspx?TestID={0}", TestID);
            hlGotoBlocks.NavigateUrl = string.Format("~/Designer/TestBlocks.aspx?TestID={0}", TestID);
        }
    }
    protected void ajaxFileUpload_OnUploadComplete(object sender, AjaxControlToolkit.AjaxFileUploadEventArgs e)
    {
        if (e.ContentType.Contains("jpg") || e.ContentType.Contains("gif")
            || e.ContentType.Contains("png") || e.ContentType.Contains("jpeg"))
        {
            Session["fileContentType_" + e.FileId] = e.ContentType;
            Session["fileContents_" + e.FileId] = e.GetContents();
        }

        // Set PostedUrl to preview the uploaded file.         
        e.PostedUrl = string.Format("?preview=1&fileId={0}", e.FileId);
    }
    protected void BlockLinqDataSource_Selected(object sender, LinqDataSourceStatusEventArgs e)
    {
        //txtBoxInstruction.Text = GridView1.da
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //TesterDataClassesDataContext dc = CommonData.GetDC();
        //dc.
        
        //txtBoxInstruction.Text = GridView1.SelectedRow.DataItem
    }
    protected void BlockLinqDataSource_DataBinding(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        int BlockID = Convert.ToInt16(Request.QueryString["BlockID"]);

        Test_Question tq = dc.Test_Questions.Where(p => p.id == BlockID).FirstOrDefault();
        tq.text = tbName.Text;
        if (tbNomer.Text == "")
            tq.number = null;
        else
            tq.number = Convert.ToByte(tbNomer.Text);

        tq.isShuffledItem = cbxIsShuffledItm.Checked;
        tq.isShuffledAns = cbxIsShuffledAns.Checked;
        tq.isTimeRestrict = cbxIsTimeRestricted.Checked;

        tq.comment = txtBoxComment.Text;
        tq.instruction = txtBoxInstruction.Text;
        dc.SubmitChanges();
        //Response.Redirect();
        Response.Redirect("~/Designer/TestBlocks.aspx?TestID=" + tq.test_id.ToString());
    }

    protected void Button2_Click(object sender, EventArgs e)
    {

        int TestID = Convert.ToInt16(ViewState["TestID"]);
        Response.Redirect("~/Designer/TestBlocks.aspx?TestID=" + TestID.ToString());
    }
}