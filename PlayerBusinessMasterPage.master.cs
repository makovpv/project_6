using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PlayerBusinessMasterPage : System.Web.UI.MasterPage
{

    public string TestName { 
        get {
            return TestName_lbl.Text;
        } 
        set {
            TestName_lbl.Text = value;
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            object TestID = Request.QueryString["TestID"];
            if (TestID != null)
            {
                using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
                {
                    TestName = dc.tests.Where(p => p.id == Convert.ToInt16(TestID)).First().name;
                    Page.Title = TestName;
                }
            }
        }
        
    }	
	
}
