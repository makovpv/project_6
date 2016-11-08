using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Analyse_risk : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser();
            if (usr != null)
            {
                tboxEndDate.Text = DateTime.Today.AddDays(1).ToString("dd.MM.yyyy");
            }
            else
                Response.Redirect("~\\lk2.aspx");
        }

        

    }
}