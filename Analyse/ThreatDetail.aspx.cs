using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Analyse_ThreatDetail : System.Web.UI.Page
{
    int idInterpretation { get { return Convert.ToInt16(Request.QueryString["i"]); } }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        InitSQL();

        if (!IsPostBack)
        {
            System.Web.Security.MembershipUser usrLoged = System.Web.Security.Membership.GetUser();

            if (usrLoged == null)
                return;

            lblUserNameTitle.Text = usrLoged.UserName;
            if (User.IsInRole("HR") || User.IsInRole("Department manager"))
                lblRoleTitle.Text = " (HR)";
            if (User.IsInRole("Department manager"))
                lblRoleTitle.Text = " (руководитель отдела)";
            if (User.IsInRole("Admin"))
                lblRoleTitle.Text = " (admin)";


            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                sqlActiveTreatsEmp.SelectParameters["idTest"].DefaultValue = 
                    dc.interpretations.Where(p => p.id == idInterpretation).FirstOrDefault().test_id.ToString();
                user_account ua = dc.user_accounts.Where (p=> p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                if (ua!= null)
                {
                    sqlActiveTreatsEmp.SelectParameters["idCompany"].DefaultValue = ua.idCompany.Value.ToString();
					
					sqlActiveTreatsEmp.SelectParameters["idDept"].DefaultValue = ua.idDept.Value.ToString();
					if (User.IsInRole("HR") || User.IsInRole("Admin") )
						sqlActiveTreatsEmp.SelectParameters["isHR"].DefaultValue = "true";
					else
						sqlActiveTreatsEmp.SelectParameters["isHR"].DefaultValue = "false";
                }

                lblInterKind.Text = string.Format("Список сотрудников, которые '{0}'",
                    dc.interpretations.Where(p => p.id == idInterpretation).FirstOrDefault().text
                    );

                //switch (dc.interpretations.Where (p=> p.id == idInterpretation).FirstOrDefault().idInterKind)
                //{
                //    case 1:
                //        lblInterKind.Text = "Список сотрудников по 'активной угрозе'";
                //        break;
                //    case 2:
                //        lblInterKind.Text = "Список сотрудников имеющих потенциал";
                //        break;
                //}

            }
            
            
        }

    }

    private void InitSQL()
    {
        sqlActiveTreatsEmp.SelectCommand =
            " with last_cte (iduser, test_id, max_date) "+
            " as ( "+
            " select ts.iduser, ts.test_id, max(ts.test_date) max_date "+
            " from subject_group sg "+
            " inner join test_subject ts on ts.group_id  = sg.id and ts.test_date is not null "+
            " where idcompany = @idcompany and ts.test_id = @idTest "+
            " group by ts.iduser, ts.test_id "+
            " ) "+
            " select ts.fio, ts.group_id, sgg.name as research_name, d.name as dept_name, j.name as job_name, ts.test_date, ts.id, "+
            " dbo.relevance_for_date (ts.test_date, getdate()) as relevance "+
            " from test_subject ts "+
            " inner join last_cte on ts.iduser=last_cte.iduser and ts.test_id = last_cte.test_id and ts.test_date = last_cte.max_date "+
            " inner join subject_group sgg on sgg.id= ts.group_id "+
            " left join user_account ua on ua.iduser = ts.iduser "+
            " left join dept d on d.id = ua.iddept "+
            " left join job j on j.id = ua.idjob "+
            " where (ua.iddept=@iddept or @isHR=1) and ua.idstate not in (1,3,4) and dbo.IsInterpretationFor (@idInterpetation, ts.id) = 1";
    }
}