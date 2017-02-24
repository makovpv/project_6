using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Metric_list : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                sqlMetrics.SelectParameters["idCompany"].DefaultValue = ua.idCompany.ToString();
                lblCompany.InnerHtml = string.Format("Метрики компании '{0}'", ua.Company.name);
            }
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
            dc.metrics.InsertOnSubmit(new metric()
            {
                idCompany = (int)ua.idCompany,
                name = "Новая метрика",
                DateCreate = DateTime.Now
                
            });
            dc.SubmitChanges();
            GridView1.DataBind();
        }
    }


}