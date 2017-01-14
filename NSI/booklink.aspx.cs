using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NSI_booklink : System.Web.UI.Page
{
    Int16 idBook { get { return Convert.ToInt16 (Request.QueryString["id"]); } }
    
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        { 
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                lblBook.InnerText = string.Format("Список компетенций для книги '{0}'", dc.books.Where(p => p.idBook == idBook).FirstOrDefault().title);
                user_account ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                if (ua != null && ua.idCompany != null)
                {
                    sqlBooks.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                }
            }
        }

    }

    protected void onClick(object Sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            dc.book_competence_lnks.DeleteAllOnSubmit (dc.book_competence_lnks.Where (q=> q.idBook == idBook));
            foreach (GridViewRow row in gvLink.Rows)
            {
                if ((row.Cells[0].FindControl("cbox") as CheckBox).Checked)
                { 
                    Int16 idComp = Convert.ToInt16 ( row.Cells[3].Text);
                    dc.book_competence_lnks.InsertOnSubmit(new book_competence_lnk() { idBook = idBook, idCompetence = idComp });
                }
            }
            dc.SubmitChanges();
            
        }
        Response.Redirect(@"~\nsi\books.aspx");

    }

}