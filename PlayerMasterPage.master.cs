using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MyMasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            object TestID = Request.QueryString["TestID"];
            if (TestID != null)
            {
                TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
                TestName_lbl.Text = dc.tests.Where(p => p.id == Convert.ToInt16(TestID)).First().name;
                Page.Title = TestName_lbl.Text;
            }
        }
        
    }
}
