    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Analyse_IdeaList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser();
            if (usr != null)
            {
                using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
                {
                    user_account ua = dc.user_accounts.Where(p => p.idUser == Guid.Parse(usr.ProviderUserKey.ToString())).FirstOrDefault();
                    if (ua != null)
                    {
                        sqlIdea.SelectParameters[0].DefaultValue = ua.idCompany.ToString();
                        SqlDept.SelectParameters[0].DefaultValue = ua.idCompany.ToString();
                        SqlUser.SelectParameters[0].DefaultValue = ua.idCompany.ToString();
                    }
                }
                tboxEndDate.Text = DateTime.Today.ToString("dd.MM.yyyy");

                int yr = DateTime.Today.Year;
                //DateTime dt = DateTime.Parse ((DateTime.Today.Year).ToString()+"1201");
                //for (int )

                ddlQuartImplement.Items.Add(new ListItem("не имеет значения", "19000101"));

                ddlQuartImplement.Items.Add(new ListItem (string.Format ("IV квартал {0} года", yr), string.Format("{0}1001", yr)));
                ddlQuartImplement.Items.Add(new ListItem (string.Format ("III квартал {0} года", yr), string.Format("{0}0701", yr)));
                ddlQuartImplement.Items.Add(new ListItem(string.Format("II квартал {0} года", yr), string.Format("{0}0401", yr)));
                ddlQuartImplement.Items.Add(new ListItem(string.Format("I квартал {0} года", yr), string.Format("{0}0101", yr)));
                
                ddlQuartImplement.Items.Add(new ListItem(string.Format("IV квартал {0} года", yr-1), string.Format("{0}1001", yr-1)));
                ddlQuartImplement.Items.Add(new ListItem(string.Format("III квартал {0} года", yr - 1), string.Format("{0}0701", yr-1)));
                ddlQuartImplement.Items.Add(new ListItem(string.Format("II квартал {0} года", yr - 1), string.Format("{0}0401", yr-1)));
                ddlQuartImplement.Items.Add(new ListItem(string.Format("I квартал {0} года", yr - 1), string.Format("{0}0101", yr-1)));
            }
            else
                Response.Redirect("~\\lk2.aspx");
        }
    }
}