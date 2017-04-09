using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Analyse_QAssessment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                sql_QA.SelectParameters["idCompany"].DefaultValue = ua.idCompany.ToString();
            }
        }
    }
}