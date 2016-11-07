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
        //if (Request.LogonUserIdentity.Name != "HOME\\Паша" && Request.LogonUserIdentity.Name != "User-DNS\\User")
        //    Response.Redirect ("~\\default.aspx");

        //form1.Page.Title = Request.LogonUserIdentity.Name;

        if (HttpContext.Current.User.Identity.IsAuthenticated)
            lblUserName.Text = HttpContext.Current.User.Identity.Name;

    }
}
