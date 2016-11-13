using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class Group_idea : System.Web.UI.Page
{
    int ID { get { return Convert.ToInt32(Request.QueryString["id"]); } }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                idea subj_idea = dc.ideas.Where(p => p.id == ID).FirstOrDefault();
                
                lFIO.Text = subj_idea.Test_Subject.fio;
                lDate.Text = subj_idea.Test_Subject.Test_Date.Value.ToShortDateString ();
                dept user_dept = subj_idea.Test_Subject.user_account.dept;
                if (user_dept != null)
                    lDept.Text = subj_idea.Test_Subject.user_account.dept.name;
                else lDept.Text = "не указан";

                tbResume.Text = subj_idea.resume;
                ddlState.SelectedValue = subj_idea.idState.ToString();
                //lnkFiles.idIdea = ID;
                ReloadFileList();

                sqlSubjResults.SelectParameters["SubjID"].DefaultValue = subj_idea.idSubject.HasValue ? ((int)subj_idea.idSubject.Value).ToString() : "0";

                lnkEditor.Visible = subj_idea.idState == (byte)enIdeaState.istRevision && subj_idea.Test_Subject.idUser == CommonData.GetCurrentUserKey();
                if (lnkEditor.Visible)
                    lnkEditor.NavigateUrl = string.Format("~\\group\\ansall.aspx?id={0}", subj_idea.idSubject);
            }
        }
        
    }

    public enum enIdeaState { istImplemented = 6, istRevision = 4}

    protected void btnSaveClick(object sender, EventArgs e)
    {
        //if (lnkFiles.UploadControl.HasFile)
        //{
        //    lnkFiles.UploadControl.SaveAs(Server.MapPath (string.Format (@"~\Data\Ideas\{0}_{1}", ID, lnkFiles.UploadControl.FileName)));
        //}
        
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            idea ide = dc.ideas.Where(i => i.id == ID).FirstOrDefault();
            if (ide != null)
            {
                if ((enIdeaState)ide.idState != enIdeaState.istImplemented &&
                    ddlState.SelectedValue == ((byte)enIdeaState.istImplemented).ToString())
                {
                    ide.implement_date = DateTime.Now; // устанавливаем дату реализации
                }
                else
                    if ((enIdeaState)ide.idState == enIdeaState.istImplemented &&
                        ddlState.SelectedValue != ((byte)enIdeaState.istImplemented).ToString())
                    {
                        ide.implement_date = null; // очищаем дату реализации
                    }

                ide.idState = Convert.ToByte ( ddlState.SelectedValue);
                ide.resume = tbResume.Text;

                dc.SubmitChanges();
            }
            Response.Redirect("~\\lk2.aspx");
        }
    }
    ////////////////////////////////////////////////////////////
    private void ReloadFileList()
    {
        DirectoryInfo di = new DirectoryInfo(Server.MapPath(@"~\data\ideas\"));
        FileInfo[] files = di.GetFiles(this.ID.ToString() + "_*.*");
        rptFiles.DataSource = files;
        rptFiles.DataBind();
    }
    
    protected void CommandBtn_Click(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "Upload")
        {
            if (uplControl.HasFile)
            {
                uplControl.SaveAs(Server.MapPath(string.Format(@"~\Data\Ideas\{0}_{1}", this.ID.ToString(), uplControl.FileName)));
                ReloadFileList();
            }
        }
        else
            if (e.CommandArgument != null)
            {
                string fname = e.CommandArgument.ToString();

                if (e.CommandName == "Delete")
                {
                    File.Delete(Server.MapPath(@"~\data\ideas\" + fname));
                    //idIdea = Convert.ToInt32(fname.Substring(0, fname.IndexOf('_')));
                    ReloadFileList();
                }
                else if (e.CommandName == "Download")
                {
                    string ContentType = "text/html";
                    switch ((new FileInfo(e.CommandArgument.ToString())).Extension.ToLower())
                    {
                        case ".png":
                            ContentType = "image/png";
                            break;
                        case ".jpeg":
                            ContentType = "image/jpeg";
                            break;
                        case ".pdf":
                            ContentType = "application/pdf";
                            break;
                    }

                    System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.ClearContent();
                    response.Clear();
                    response.ContentType = ContentType; // "text/plain";
                    response.AddHeader("Content-Disposition", "attachment; filename=" + fname + ";");
                    response.TransmitFile(Server.MapPath(@"~\data\ideas\" + fname));
                    response.Flush();
                    response.End();
                }
            }
    }

}




