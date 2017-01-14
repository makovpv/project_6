using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NSI_books : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                sqlBooks.SelectParameters["idCompany"].DefaultValue = ua.idCompany.ToString();
                lblCompany.InnerHtml = string.Format("Библиотека компании '{0}'", ua.Company.name);
            }
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
            dc.books.InsertOnSubmit(new book()
            {
                idCompany = (int)ua.idCompany,
                title = "новая книга",
                author = "",
                pages = 1
            });
            dc.SubmitChanges();
            GridView1.DataBind();
        }
    }

}