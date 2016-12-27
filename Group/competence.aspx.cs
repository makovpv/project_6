using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Group_competence : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                sqlCompetence.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
            }
            
            
        }

    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
            dc.competences.InsertOnSubmit(new competence()
            {
                idCompany = (int)ua.idCompany,
                name = "новая компетенция"
            });
            dc.SubmitChanges();
            GridView1.DataBind();
        }
    }
}