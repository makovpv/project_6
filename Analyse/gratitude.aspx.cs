using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Analyse_gratitude : System.Web.UI.Page
{

    string SubjName
    {
        get
        {
            return Request.QueryString["s"] == null ? "" : Request.QueryString["s"];
        }
    }
    string ObjName
    {
        get
        {
            return Request.QueryString["o"] == null ? "" : Request.QueryString["o"];
        }
    }

  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser();
            if (usr != null)
            {
                if (!User.IsInRole ("HR"))
                {
                    gridGratitude.Columns.RemoveAt(4); // remove column with "Delete" button
                }
                
                using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
                {
                    user_account ua = dc.user_accounts.Where(p => p.idUser == Guid.Parse(usr.ProviderUserKey.ToString())).FirstOrDefault();
                    if (ua != null)
                    {
                        SqlUser.SelectParameters[0].DefaultValue = ua.idCompany.ToString();
                    }
                }
                tboxEndDate.Text = DateTime.Today.ToString("dd.MM.yyyy");

                if (SubjName != "")
                {
                    sqlGratitude.SelectParameters["subj_filter"].DefaultValue = SubjName;
                    sqlGratitude.SelectParameters["obj_filter"].DefaultValue = "<все>";
                }
                if (ObjName != "")
                {
                    sqlGratitude.SelectParameters["subj_filter"].DefaultValue = "<все>";
                    sqlGratitude.SelectParameters["obj_filter"].DefaultValue = ObjName;
                }
            }
            else
                Response.Redirect("~\\lk2.aspx");
        }
        else
        {
            sqlGratitude.SelectParameters["subj_filter"].DefaultValue = "<все>";
            sqlGratitude.SelectParameters["obj_filter"].DefaultValue = "<все>";
        }
    }

    protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
}