using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Group_SubjAdd : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser();
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        user_account ua = dc.user_accounts.Where(p => p.idUser == Guid.Parse(usr.ProviderUserKey.ToString())).FirstOrDefault();
        lblUserNameTitle.Text = usr.UserName;
        string lblRole_Text = "";
        if (User.IsInRole("Department manager"))
            lblRole_Text += " руководитель отдела ";
        if (User.IsInRole("Admin"))
            lblRole_Text += " admin ";
        if (User.IsInRole("HR") || User.IsInRole("Department manager"))
            lblRole_Text += " HR ";
        lblRoleTitle.Text = " (" + lblRole_Text + ")";


        if (!IsPostBack)
        {
            if (Request.UrlReferrer != null)
            {
                Button2.PostBackUrl = Request.UrlReferrer.AbsoluteUri;
            }
        }
    }

    int GroupID { get { return Convert.ToInt16(Request.QueryString["g"]); } }

    protected void Button1_Click(object sender, EventArgs e)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();

        subject_group sg = dc.subject_groups.Where (p=> p.id == GroupID).FirstOrDefault();
        Test_SubjectGroup_Link lnk = sg.Test_SubjectGroup_Links.FirstOrDefault();
        if (lnk!=null)
        {
            int FirstTestID = lnk.idTest;

            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            { 
                if ((GridView1.Rows[i].FindControl("cbChecked") as CheckBox).Checked)
                {
                    dc.Test_Subjects.InsertOnSubmit(new Test_Subject()
                    {
                        group_id = GroupID,
                        Test_Id = FirstTestID,
                        Nick_Name = GridView1.Rows[i].Cells[1].Text,
                        fio = GridView1.Rows[i].Cells[1].Text, ////// !! 
                        idUser = Guid.Parse(GridView1.DataKeys[i].Value.ToString())
                    });

                    // для тестов с параметрами важны возраст и пол !! (см. comments ниже)

                    //subj.idUser = UAcc.idUser;
                    //subj.Gender = UAcc.gender;
                    //if (UAcc.birth_year == null)
                    //    subj.Age = null;
                    //else
                    //{
                    //    int aaa = Convert.ToInt16(DateTime.Now.Year) - Convert.ToInt16(UAcc.birth_year);
                    //    subj.Age = Convert.ToInt16(aaa);
                    //}

                }
            }

            // from textbox
            if (tboxSubjAdd.Text.Trim() != "")
            {
                //    int idCompany = dc.subject_groups.Where (p=> p.id == GroupID).fir
                string[] NewSubjList = tboxSubjAdd.Text.Trim().Split(',');
                foreach (string NewSubjNick in NewSubjList)
                {
                    user_account ua = CommonData.CreateNewUserAccount(NewSubjNick.Trim(), Convert.ToInt16(sg.idCompany), dc);
                    if (ua != null)
                    {
                        Test_Subject ts = new Test_Subject();
                        ts.idUser = ua.idUser;
                        ts.group_id = sg.id;
                        ts.Nick_Name = ua.login_name;
                        ts.fio = ua.login_name;
                        ts.Test_Id = FirstTestID;

                        dc.Test_Subjects.InsertOnSubmit(ts);
                    }
                }
            }

            dc.SubmitChanges();
        }
        
        if (Button2.PostBackUrl != null)
            Response.Redirect(Button2.PostBackUrl);

    }

    //добавить всех "активных" сотрудников в исследование.
    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in GridView1.Rows)
        {
            Control ctr = gvr.FindControl("cbChecked");
            if (ctr != null && ctr is CheckBox)
                ((CheckBox)ctr).Checked = true;
        }
    }
}