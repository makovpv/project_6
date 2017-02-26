using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraCharts;
using DevExpress.Web;
using DevExpress.Web.ASPxRoundPanel;
using System.Web.UI.DataVisualization.Charting;
using DevExpress.Web.ASPxGridView;
using System.Data;
using System.Collections;


public partial class lk2 : System.Web.UI.Page
{

    bool isForBusiness
    {
        get
        {
            string appset = System.Configuration.ConfigurationManager.AppSettings.Get("isForBusiness");
            return appset == null ? false : appset == "true";
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (isForBusiness)
        {
            //if (!IsPostBack) 
            ScriptManager.RegisterStartupScript(this, this.GetType(), "InitMenu", "InitMenu();", true);
            this.Page.MasterPageFile = "~/MainBusinessMasterPage.master";
        }
    }

    int? idCompany;
    protected void Page_Load(object sender, EventArgs e)
    {
        InitSQL();
        System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser();
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        user_account ua = dc.user_accounts.Where(p => p.idUser == Guid.Parse(usr.ProviderUserKey.ToString())).FirstOrDefault();
        if (ua != null) idCompany = ua.idCompany;
        bool canViewAllDept = User.IsInRole("HR") || User.IsInRole("Admin");
        
        if (!IsPostBack)
        {

            lblUserName.Text = usr.UserName;
            lblUserNameTitle.Text = usr.UserName;

            if (ua != null)
            {
                lblCompanyTitle.Text = ua.Company != null ? ua.Company.name : "";
                
                Subject_lds.SelectParameters["iduser"].DefaultValue = usr.ProviderUserKey.ToString();
                SubjectAuto_lds.SelectParameters["iduser"].DefaultValue = usr.ProviderUserKey.ToString();

                string idCompanyStr = ua.idCompany != null ? ua.idCompany.ToString() : "0";
                sqlUsers.SelectParameters["idCompany"].DefaultValue = idCompanyStr;
                SqlHRNowResearch.SelectParameters["idcompany"].DefaultValue = idCompanyStr;
                SqlHRNowResearchAll.SelectParameters["idcompany"].DefaultValue = idCompanyStr;
                SqlHRClosedResearch.SelectParameters["idcompany"].DefaultValue = idCompanyStr;
                SqlHRClosedResearchAll.SelectParameters["idcompany"].DefaultValue = idCompanyStr;
                SqlHRConstResearchAll.SelectParameters["idcompany"].DefaultValue = idCompanyStr;

                sqlMySubject.SelectParameters["iduser"].DefaultValue = usr.ProviderUserKey.ToString();

                lblCompany.Text = ua.Company.name;
                lblDept.Text = ua.dept != null ? ua.dept.name : "<не указан>";
                lblFIO.Text = ua.fio;
                lblJob.Text = ua.Job != null ? ua.Job.name : "<не указана>";
                lblStatus.Text = ua.user_state != null ? ua.user_state.name : "<не указан>";


                liCompanyPage.Visible = User.IsInRole("HR") || User.IsInRole("Department manager");
                liEmployess.Visible = liCompanyPage.Visible;
                liHRProcess.Visible = User.IsInRole("HR");
                if (User.IsInRole("HR") || User.IsInRole("Department manager"))
                {
                    lblRole.Text += " HR ";
                    sqlCompanyDept.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    SqlHRMonth.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    SqlCurrExamination.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    SqlHRPlanResearch.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    SqlHRPlanResearchAll.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();

                    //SqlPastExamination.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    sqlActiveTreats.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    sqlActiveTreats.SelectParameters["iddept"].DefaultValue = ua.idDept.ToString();
                    sqlPotential.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    sqlPotential.SelectParameters["iddept"].DefaultValue = ua.idDept.ToString();
                    sqlMonitoring.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                    SqlMetrics.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();

                    CommonData.CompanyEmpCountInfo EmpCount = CommonData.GetEmpCount(dc, (int)ua.idCompany);
                    lblStaffCount.Text = EmpCount.StaffCount.ToString();
                    //lblKandidatCount.Text = EmpCount.CandidateCount.ToString();
                    //lblIspitatCount.Text = EmpCount.ProbationCount.ToString();
                    //lblVacancyCount.Text = EmpCount.VacancyCount.ToString();
                }
                if (User.IsInRole("Expert"))
                {
                    NewIdeas_lds.SelectParameters["idcompany"].DefaultValue = ua.idCompany.ToString();
                }
                if (User.IsInRole("Department manager"))
                {
                    lblRole.Text += " руководитель отдела ";
                    ApprovedSubject_lds.SelectParameters["idUser"].DefaultValue = ua.idUser.ToString();
                }
                if (User.IsInRole("Admin"))
                {
                    lblRole.Text += " admin ";
                }
                if (lblRole.Text == "") lblRole.Text = "сотрудник";

                lblRoleTitle.Text = " (" + lblRole.Text+ ")";

                
                sqlActiveTreats.SelectParameters["isHR"].DefaultValue = canViewAllDept.ToString();
                sqlPotential.SelectParameters["isHR"].DefaultValue = canViewAllDept.ToString();


                
                DrawEmpDiagrams(dc, ua.idCompany); // рисуем диаграммы по сотрудникам
            }

            GetHRMonthData(DateTime.Now.Year.ToString());
            HRLabelYear.Text = DateTime.Now.Year.ToString() + " год";

        }

        //DataView rez = (DataView)sqlActiveTreats.Select(new DataSourceSelectArguments());
        //if (rez != null)
        //    DrawCompanySubTable(rez, companyActiveTable, "blue", "red");

        //rez = (DataView)sqlPotential.Select(new DataSourceSelectArguments());
        //if (rez != null)
        //    DrawCompanySubTable(rez, companyPotencTable, "brown", "green");

        DataView rez = (DataView)SqlMetrics.Select(new DataSourceSelectArguments());
        if (rez != null)
            DrawCompanySubTable(rez, companyMetricTable, "magenta", "red");

        DrawIndicators(dc, ua, canViewAllDept); // indicators

        
        InitMenuHints();
        InitYearChart();
    }

    /// <summary>
    /// отрисовка единичного индикатора
    /// </summary>
    void DrawSingleIndicator(TesterDataClassesDataContext p_dc, indicator ind, user_account p_ua, 
        bool p_canViewAllDept, Control tc)
    {
        Image img = new Image();
        switch (ind.idType)
        {
            case 10: // индикатор по основной шкале
                ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc);    
                //if (ind.Scale != null)
                //{
                //    TesterChart.CalcAndDrawIndicator(p_dc, img, Server.MapPath(@"~\Images\Indicator"), ind, p_ua.idUser, p_canViewAllDept);
                //    if (img.ImageUrl != "")
                //    {
                //        tc.Controls.Add(new Label() { Text = ind.name, Width = 65 });
                //        tc.Controls.Add(img);
                //    }
                //}
                break;
            case 20:
                if (ind.Scale != null) // круговая диаграмма
                {
                    System.Web.UI.DataVisualization.Charting.Chart dgr = new System.Web.UI.DataVisualization.Charting.Chart()
                    {
                        ID = "ind_diagr",
                        BackColor = System.Drawing.Color.Transparent
                    };

                    dgr.DataBound += dgr_DataBound;

                    compIndicatorData.Controls.Add(dgr);
                    TesterChart.FillSmallDoughnut(dgr, p_ua.idCompany, ind.Scale, ind.name, p_ua.idDept, p_canViewAllDept);
                }
                break;
            case 60: // столбчатая диаграмма
                if (ind.test_diagram != null)
                {
                    List<int> SubjIDList =
                    (
                        from uacc in p_dc.user_accounts
                        join ts in p_dc.Test_Subjects on uacc.idUser equals ts.idUser
                        where uacc.idCompany == p_ua.idCompany && (p_canViewAllDept || uacc.idDept == p_ua.idDept) && ts.Test_Id == ind.test_diagram.test_id
                        select ts.id).ToList();
                    tc.Controls.Add(TesterChart.DrawDiagram(p_dc, ind.test_diagram, 600, SubjIDList));
                }
                break;
            case 100: // счетчик ссылок (упоминаний)
                ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc);    
                //if (ind.idGroup != null)
                //{
                //    TesterChart.CalcAndDrawIndicator(p_dc, img, Server.MapPath(@"~\Images\Indicator"), ind, p_ua.idUser, p_canViewAllDept);
                //    if (img.ImageUrl != "")
                //    {
                //        tc.Controls.Add(new Label() { Text = ind.name, Width = 65, CssClass = "clsIndexDescr" });
                //        tc.Controls.Add(img);
                //        string LastAns = CommonData.LastTextAnswer(p_dc, (int)ind.idGroup, p_ua.fio);
                //        tc.Controls.Add(new Label() { Text = LastAns, Width = 150 });
                //    }
                //}
                break;
            case 80: // счетчик прохождений
                ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc);
                //if (ind.idGroup != null)
                //{
                //    TesterChart.CalcAndDrawIndicator(p_dc, null, null, ind, p_ua.idUser, p_canViewAllDept);
                //    if (img.ImageUrl != "")
                //    {
                //        tc.Controls.Add(new Label() { Text = ind.name, Width = 65, CssClass = "clsIndexDescr" });
                //        tc.Controls.Add(img);
                //    }
                //}
                break;
            case 120: //context.
                if (ind.isPersonal)
                {
                    TesterChart.PrintAchivements(p_dc, ind, p_ua.fio, tc);
                    //foreach (string achivement_text in
                    //    (
                    //    from lnk in p_dc.Test_SubjectGroup_Links
                    //    join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
                    //    join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
                    //    join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
                    //    join txt in p_dc.Test_Results_Txts on tr.Subject_ID equals txt.subject_id
                    //    where lnk.idGroup == ind.idGroup && tr.SelectedValue == 1 && ss.name == p_ua.fio
                    //    select txt.text)
                    //    )
                    //{

                    //    if (achivement_text != null)
                    //    {
                    //        tc.Controls.Add(new Label() { Text = achivement_text, CssClass = "clsAchivementLabel" });
                    //        tc.Controls.Add(new LiteralControl("<br/>"));
                    //    }
                    //}
                }
                break;
            case 130: // idea (not individual)
            case 190: // risks (group indicator)
            case 170: // gratitude (not individual)
                ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc);
                break;
            case 140: // integral
            case 160: // plan
                //ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc, Server.MapPath(@"~\Images\Indicator"));
                ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc);
                break;
            default:
                ObjectIndicator.BuildAndPut(p_dc, ind, p_ua, tc);
                break;
        } 
    }
    private void DrawIndicators(TesterDataClassesDataContext p_dc, user_account p_ua, bool p_canViewAllDept)
    {
        if (p_ua != null)
        {
            string prior_header = "";
            foreach (indicator ind in p_dc.indicators.Where(p => p.idCompany == p_ua.idCompany && p.isPersonal == false).OrderBy(q => q.category_header))
            {// group indicators
                if (prior_header != ind.category_header)
                {
                    compIndicatorData.Controls.Add(new LiteralControl("<hr/><br/>"));
                    prior_header = ind.category_header;
                }
                DrawSingleIndicator(p_dc, ind, p_ua, p_canViewAllDept, compIndicatorData);
            }

            prior_header = ""; // personal indicators
            Panel prior_Group = null;
            foreach (indicator ind in p_dc.indicators.Where(p => p.idCompany == p_ua.idCompany && p.isPersonal == true).OrderBy(q => q.category_header))
            {
                Panel pnlGroup = null;
                if (prior_header != ind.category_header)
                {
                    if (prior_Group != null && prior_Group.Controls.Count == 3)
                    {// удаляем заголовок группы индикаторов, если нет ни одного индикатора
                        compPersonalIndicatorData.Controls.Remove(prior_Group);
                    }
                    pnlGroup = new Panel();
                    compPersonalIndicatorData.Controls.Add(pnlGroup);

                    pnlGroup.Controls.Add(new LiteralControl("<br/><br/>"));
                    pnlGroup.Controls.Add(new Label() { Text = ind.category_header, CssClass = "clsIndicatorGroup" });
                    pnlGroup.Controls.Add(new LiteralControl("<hr/>"));
                    prior_header = ind.category_header;
                    prior_Group = pnlGroup;
                }

                Control pnl = pnlGroup == null ? (Control)compPersonalIndicatorData : pnlGroup;
                if (ind.link_URL != null)
                {
                    if (!ind.link_URL.StartsWith("http"))
                    {
                        ind.link_URL = "http://" + ind.link_URL;
                    }
                    pnl.Controls.Add(new HyperLink()
                    {
                        NavigateUrl = ind.link_URL,
                        Text = ind.link_title == null || ind.link_title == "" ? ind.link_URL : ind.link_title
                    });
                    pnl.Controls.Add(new Label() { CssClass = "clsIndexDescr" });
                }

                DrawSingleIndicator(p_dc, ind, p_ua, p_canViewAllDept, pnl);
                //DrawSingleIndicator(p_dc, ind, p_ua, p_canViewAllDept, (Control)compPersonalIndicatorData);
                if (ind.subject_group != null)
                {
                    if (ind.repeat_lnk) // ссылка на повторное прохождение
                    {
                        Test_Subject NextTimeSubject = ind.subject_group.Test_Subjects.Where(ts => ts.idUser == p_ua.idUser && ts.Test_Date == null).FirstOrDefault();
                        if (NextTimeSubject != null)
                        {
                            pnl.Controls.Add( new HyperLink() {   
                                NavigateUrl = string.Format("~\\Player\\testtrack.aspx?s={0}", NextTimeSubject.id),
                                Text = "Пройти повторно..."});
                        }
                    }
                    if (ind.full_report_lnk) // ссылка на отчет
                    {
                        Test_Subject LastTimeSubject = ind.subject_group.Test_Subjects.Where(ts => ts.idUser == p_ua.idUser && ts.Test_Date != null).OrderByDescending(obd=> obd.Test_Date).FirstOrDefault();
                        if (LastTimeSubject != null)
                        {
                            pnl.Controls.Add(new HyperLink()
                            {
                                NavigateUrl = string.Format("~\\Player\\testresult.aspx?SubjId={0}", LastTimeSubject.id),
                                Text = "Отчет"
                            });
                        }
                    }


                    compPersonalIndicatorData.Controls.Add(new LiteralControl("<br/>"));
                }
            }
            if (prior_Group != null && prior_Group.Controls.Count == 3)
            {// удаляем заголовок группы индикаторов, если нет ни одного индикатора
                compPersonalIndicatorData.Controls.Remove(prior_Group);
            }
        }
    }

    /// <summary>
    /// Инициализации меню и их событий
    /// </summary>
    private void InitMenuHints()
    {
        //RemoveEvents(ImageButtonCompany);
        //ImageButtonCompany.Attributes.Add("onmouseover", "return companyHint(true);");
        //ImageButtonCompany.Attributes.Add("onmouseout", "return companyHint(false);");

        //RemoveEvents(ImageButtonHR);
        //ImageButtonHR.Attributes.Add("onmouseover", "return hrHint(true);");
        //ImageButtonHR.Attributes.Add("onmouseout", "return hrHint(false);");

        //RemoveEvents(ImageButtonSotr);
        //ImageButtonSotr.Attributes.Add("onmouseover", "return sotrHint(true);");
        //ImageButtonSotr.Attributes.Add("onmouseout", "return sotrHint(false);");

        //RemoveEvents(ImageButtonUnFinishedTest);
        //ImageButtonUnFinishedTest.Attributes.Add("onmouseover", "return unFinishHint(true);");
        //ImageButtonUnFinishedTest.Attributes.Add("onmouseout", "return unFinishHint(false);");

        //RemoveEvents(ImageButtonAccount);
        //ImageButtonAccount.Attributes.Add("onmouseover", "return accountHint(true);");
        //ImageButtonAccount.Attributes.Add("onmouseout", "return accountHint(false);");
    }

    /// <summary>
    /// очистка евентов у пунктов меню
    /// </summary>
    private void RemoveEvents(ImageButton b)
    {
        b.Attributes.Remove("onmouseover");
        b.Attributes.Remove("onmouseout");
    }

    private void DrawEmpDiagrams(TesterDataClassesDataContext p_dc, int? p_idCompany)
    {
        var DeptSource =
        from a in p_dc.user_accounts
        join d in p_dc.depts on a.idDept equals d.id
        where a.idCompany == p_idCompany && (a.idState != 1 && a.idState != 3 && a.idState != 4)
        group d by d.name
            into gr
            select new
            {
                DeptName = gr.Key,
                EmpCount = gr.Select(x => x.name).Count()
            };

        ChartCompanyDept.DataSource = DeptSource;

        var JobSource =
        from a in p_dc.user_accounts
        join j in p_dc.Jobs on a.idJob equals j.id
        where a.idCompany == p_idCompany && (a.idState != 1 && a.idState != 3 && a.idState != 4)
        group j by j.name
            into gr
            select new
            {
                JobName = gr.Key,
                EmpCount = gr.Select(x => x.name).Count()
            };

        //ChartCompanyJob.DataSource = JobSource;
    }

    private void InitSQL()
    {
        sqlUsers.SelectCommand = 
            " select case isnull(fio,'') when '' then ua.login_name else fio end as fio, "+
            " d.name as dept_name, j.name as job_name, ua.iduser, us.name as state_name "+
            " from user_account ua"+
            " left join dept d on d.id=ua.iddept and d.idcompany=@idcompany"+
            " left join job j on j.id=ua.idjob"+
            " left join user_state us on us.id = ua.idstate"+
            " where ua.idcompany=@idcompany"+
            " order by 1";

        sqlUsers.DeleteCommand = "exec dbo.user_delete @iduser";

        sqlCompanyDept.SelectCommand = 
            " select d.name as dept_name, count(*) as emp_count "+
            " from dept d left join user_account ua on ua.iddept = d.id and ua.idstate in (2,6)"+
            " where d.idcompany=@idcompany group by d.id, d.name";

        sqlMySubject.SelectCommand = 
            "select t.name, "+
            " convert (varchar(20),  ts.Test_Date, 104) as test_date_txt,"+
            " ts.Test_Date as test_date,"+
            " ts.id as SubjID, ua.fio"+
            " from test_subject ts "+
            " inner join user_account ua on ua.login_name=ts.nick_name"+
            " inner join test t on t.id = ts.Test_Id"+
            " where ts.test_date is not null and ua.iduser=@iduser " +
            " order by 3 desc";

        SqlCurrExamination.SelectCommand =
            " select id, name, case when stop_date is not null then '(окончание '+convert (varchar(10), stop_date, 104)+')' else null end as stop_date FROM (" +
            "            SELECT sg.id,sg.name, " +
            "            sum(case when ts.test_date is null then 0 else 1 end) as passed_count," +
            "            count(*) as total_count," +
            "            stop_date" +
            "            FROM [Subject_group] sg" +
            "            left join test_subject ts on ts.group_id=sg.id and basesubject_id is null" +
            "            where idCompany = @idCompany and getdate() between isnull(start_date, '19000101') and isnull(stop_date,'20700101')" +
            " group by sg.id,sg.name, sg.stop_date) q" +
            " WHERE passed_count < total_count ORDER BY q.stop_date";


        Subject_lds.SelectCommand = 
            " SELECT ts.ID, sg.name as groupname"+
            " FROM [Test_Subject] ts "+
            " left join subject_group sg on sg.id=ts.group_id" +
            " where test_date is null and ts.idUser = @iduser and sg.isAutoSubjAdd = 0 and isnull(sg.stop_date,'20700101') > getdate()";
        SubjectAuto_lds.SelectCommand =
            " SELECT ts.ID, sg.name as groupname"+
            " FROM [Test_Subject] ts "+
            " left join subject_group sg on sg.id=ts.group_id" +
            " where test_date is null and ts.idUser = @iduser and sg.isAutoSubjAdd = 1 and isnull(sg.stop_date,'20700101') > getdate()";
        NewIdeas_lds.SelectCommand =
            " select idea.id, ts.fio, test_date"+
            " from idea"+
            " join test_subject ts on ts.id = idea.idSubject"+
            " join user_account ua on ua.iduser = ts.iduser"+
            " where idea.idState = 0 and ua.idcompany = @idcompany";
        ApprovedSubject_lds.SelectCommand =
            " select tsa.id, idSubject, ts.fio" +
            " from test_subject_approved tsa" +
            " join Test_Subject ts on ts.id = tsa.idSubject" +
            " where isApproved = 0 and ApprovedByUser = @idUser";

        sqlMonitoring.SelectCommand = 
            " select distinct test.monitoring_area"+
            " from test" +
            " inner join test_subjectgroup_link lnk on lnk.idtest=test.id"+
            " inner join subject_group sg on sg.id = lnk.idGroup"+
            " inner join test_subject ts on ts.test_id= test.id and ts.test_date is not null and ts.group_id = sg.id"+
            " where isnull(monitoring_area,'') <> '' and sg.idcompany = @idCompany and test.ispublished = 1";

        SqlHRNowResearch.SelectCommand =
            "select id, name, " +
            " name+' (' +cast(passed_count as varchar(5)) +'/' +cast(total_count as varchar(5)) name_and_count, " +
            " cast (case passed_count when 0 then 1 else 0 end as bit) isempty, " +
            " cast (case passed_count when 0 then 0 else 1 end as bit) isnotempty, " +
            " convert (varchar(10), stop_date, 104) as stop_date FROM (" +
            " SELECT sg.id,sg.name, " +
            " sum(case when ts.test_date is null then 0 else 1 end) as passed_count," +
            " count(*) as total_count," +
            " stop_date" +
            " FROM [Subject_group] sg" +
            " left join test_subject ts on ts.group_id=sg.id and basesubject_id is null" +
            //" where UserKey = @UsrKey " +
            " where sg.idcompany = @idcompany and isAutoSubjAdd = 0" +
            " and (sg.[start_date] < @EndDate and isnull(sg.[stop_date],'20500101') >= @StartDate) " +
            " and sg.isForYearPlan = 1" +
            " group by sg.id,sg.name, sg.stop_date) q" +
            " WHERE passed_count < total_count ORDER BY q.stop_date";

        SqlHRPlanResearch.SelectCommand =
            " select id, name, convert (varchar(10),start_date,104) as start_date from subject_group" +
            " where idcompany = @idcompany " +
            " and isnull(start_date, '19000101') >= @StartDate and isnull(start_date, '19000101') < @EndDate " +
            " ORDER BY subject_group.start_date";

        SqlHRPlanResearchAll.SelectCommand =
            " select id, name, convert (varchar(10),start_date,104) as start_date from subject_group" +
            " where idcompany = @idcompany " +
            " and isnull(start_date, '19000101') >= getdate() "+
            //"and isnull(start_date, '19000101') < '20151231' " +
            " ORDER BY subject_group.start_date";

        SqlHRClosedResearch.SelectCommand =
            " select id, name, " +
            " name+' (' +cast(passed_count as varchar(5)) +'/' +cast(total_count as varchar(5)) name_and_count, " +
            " cast (case passed_count when 0 then 1 else 0 end as bit) isempty, " +
            " cast (case passed_count when 0 then 0 else 1 end as bit) isnotempty, " +
            " convert (varchar(10), stop_date, 104) as stop_date FROM (" +
            " SELECT sg.id,sg.name, " +
            " sum(case when ts.test_date is null then 0 else 1 end) as passed_count," +
            " count(*) as total_count," +
            " stop_date" +
            " FROM [Subject_group] sg" +
            " left join test_subject ts on ts.group_id=sg.id and basesubject_id is null" +
            //" where UserKey = @UsrKey " +
            " where sg.idcompany = @idcompany" +
            " group by sg.id,sg.name, sg.stop_date) q" +
            " WHERE (isnull(stop_date,'20500101') >= @StartDate and isnull(stop_date,'20500101') < @EndDate) "+
            //" or (passed_count = total_count and total_count > 1)"+
            " ORDER BY q.stop_date";
        
        SqlHRClosedResearchAll.SelectCommand =
            " select id, name, " +
            " name+' (' +cast(passed_count as varchar(5)) +'/' +cast(total_count as varchar(5)) +')' name_and_count, " +
            " cast (case passed_count when 0 then 1 else 0 end as bit) isempty, " +
            " cast (case passed_count when 0 then 0 else 1 end as bit) isnotempty, " +
            " convert (varchar(10), stop_date, 104) as stop_date FROM (" +
            " SELECT sg.id,sg.name, " +
            " sum(case when ts.test_date is null then 0 else 1 end) as passed_count," +
            " count(*) as total_count," +
            " stop_date" +
            " FROM [Subject_group] sg" +
            " left join test_subject ts on ts.group_id=sg.id and basesubject_id is null" +
            //" where UserKey = @UsrKey " +
            " where sg.idcompany = @idcompany" +
            " group by sg.id,sg.name, sg.stop_date) q" +
            " WHERE ( "+
            //isnull(stop_date,'20500101') >= '20150101' and 
            " isnull(stop_date,'20500101') < getdate()) " +
            //" or (passed_count = total_count and total_count > 1)" +
            " ORDER BY q.stop_date";

        SqlHRNowResearchAll.SelectCommand =
            " select id, name, " +
            " name+' (' +cast(passed_count as varchar(5)) +'/' +cast(total_count as varchar(5))+')' name_and_count, " +
            " cast (case passed_count when 0 then 1 else 0 end as bit) isempty, " +
            " cast (case passed_count when 0 then 0 else 1 end as bit) isnotempty, " +
            " convert (varchar(10), stop_date, 104) as stop_date FROM (" +
            " SELECT sg.id,sg.name, " +
            " sum(case when ts.test_date is null then 0 else 1 end) as passed_count," +
            " count(*) as total_count," +
            " stop_date, start_date " +
            " FROM [Subject_group] sg" +
            " left join test_subject ts on ts.group_id=sg.id and basesubject_id is null" +
            //" where UserKey = @UsrKey " +
            " where sg.idcompany = @idcompany and sg.isAutoSubjAdd = 0" +
            " group by sg.id,sg.name, sg.stop_date, sg.start_date) q" +
            //" WHERE (isnull(stop_date,'20500101') >= '20150101' and isnull(stop_date,'20500101') < '20151231') " +
            //" or (passed_count = total_count and total_count > 1)" +
            " WHERE getdate() between isnull(start_date, '19000101') and isnull(stop_date,'20500101') " +
            " ORDER BY q.stop_date";

        SqlHRConstResearchAll.SelectCommand =
            " select id, name, " +
            " name+' (' +cast(passed_count as varchar(5)) +'/' +cast(total_count as varchar(5))+')' name_and_count, " +
            " cast (case passed_count when 0 then 1 else 0 end as bit) isempty, " +
            " cast (case passed_count when 0 then 0 else 1 end as bit) isnotempty, " +
            " convert (varchar(10), stop_date, 104) as stop_date FROM (" +
            " SELECT sg.id,sg.name, " +
            " sum(case when ts.test_date is null then 0 else 1 end) as passed_count," +
            " count(*) as total_count," +
            " stop_date, start_date " +
            " FROM [Subject_group] sg" +
            " left join test_subject ts on ts.group_id=sg.id and basesubject_id is null" +
            " where sg.idcompany = @idcompany and sg.isAutoSubjAdd = 1" +
            " group by sg.id,sg.name, sg.stop_date, sg.start_date) q" +
            " WHERE getdate() between isnull(start_date, '19000101') and isnull(stop_date,'20500101') " +
            " ORDER BY q.stop_date";
        
        sqlPotential.SelectCommand =
            "with last_cte (iduser, test_id, max_date)" +
            " as (" +
                " select ts.iduser, ts.test_id, max(ts.test_date) max_date" +
                " from subject_group sg" +
                " inner join test_subject ts on ts.group_id  = sg.id and ts.test_date is not null" +
                " where idcompany = @idCompany" +
                " group by ts.iduser, ts.test_id" +
            " )" +
                " select q.id, emp_count, ii.[text], t.name as test_name, case when q.diff<=1 then 1 else 0 end as isnew, 0 as ismetric " +
                " from (" +
                " select itp.id, count(*) emp_count, min (datediff(dd,ts.test_date,getdate())) diff " +
                " from test_subject ts" +
                " inner join last_cte on ts.iduser=last_cte.iduser and ts.test_id = last_cte.test_id and ts.test_date = last_cte.max_date" +
                " inner join interpretation itp on itp.test_id = ts.test_id and itp.idInterKind = 2" +
                " left join user_account ua on ts.iduser = ua.iduser" +
                " where ua.idstate not in (1,3,4) and dbo.IsInterpretationFor (itp.id, ts.id) = 1 and (ua.iddept = @iddept or @isHR=1)" +
                " group by itp.id" +
            " ) q" +
            " inner join interpretation ii on q.id=ii.id"+
            " inner join test t on t.id=ii.test_id " + 
            " order by 4";

        sqlActiveTreats.SelectCommand =
            " with last_cte (iduser, test_id, max_date)" +
            " as (" +
                " select ts.iduser, ts.test_id, max(ts.test_date) max_date" +
                " from subject_group sg" +
                " inner join test_subject ts on ts.group_id  = sg.id and ts.test_date is not null" +
                " where idcompany = @idCompany" +
                " group by ts.iduser, ts.test_id" +
            " )" +
            " select q.id, emp_count, ii.[text], t.name as test_name, case when q.diff<=1 then 1 else 0 end as isnew, 0 as ismetric " +
            " from (" +
                " select itp.id, count(*) emp_count, min (datediff(dd,ts.test_date,getdate())) diff " +
                " from test_subject ts" +
                " inner join last_cte on ts.iduser=last_cte.iduser and ts.test_id = last_cte.test_id and ts.test_date = last_cte.max_date" +
                " inner join interpretation itp on itp.test_id = ts.test_id and itp.idInterKind = 1" +
                " left join user_account ua on ts.iduser = ua.iduser" +
                " where ua.idstate not in (1,3,4) and dbo.IsInterpretationFor (itp.id, ts.id) = 1 and (ua.iddept = @iddept or @isHR=1)" +
                " group by itp.id" +
            " ) q" +
            " inner join interpretation ii on q.id=ii.id " +
            " inner join test t on t.id=ii.test_id " +
            " order by 4";

        SqlMetrics.SelectCommand =
            "set dateformat 'dmy' "+
            "select md.idmetric as id, md.metric_name as test_name, md.description as text, 1 as ismetric, count(*) as emp_count " +
            "from dbo.metricdeviation (@idcompany, null) md " +
            "group by md.idmetric, md.metric_name, md.description";
            
            //"select m.idMetric as id, m.name test_name, m.description as text, 1 as ismetric, count (*) as emp_count " +
            //"from metric m "+
            //"join Test_Data td on m.idScale = td.Scale_ID "+
            //"join test_subject ts on ts.id = td.subject_id "+
            //"join user_account ua on ua.iduser = ts.iduser " +
            //"where m.idCompany = @idcompany and td.Test_Value < m.index_value and m.condition = '<' " +
            //" and ts.actual = 1 "+
            //" and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null) "+
            //" and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null) "+
            //"group by m.idMetric, m.name, m.description "+
            //"UNION ALL " + // not exists
            //"select q.id, q.test_name, q.text, 1 as ismetric, count(*) as emp_count "+
            //"from ( "+
            //"SELECT m.idMetric as id, m.name test_name, m.description as text, 1 as ismetric "+
            //"FROM metric m  "+
            //"inner JOIN Test_Subject ts ON ts.test_id = m.idtest  "+
            //"inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = m.idcompany "+
            //"WHERE m.idcompany = @idCompany and m.condition = 'NE'  and m.index_value = 1 "+
            //"and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null)   "+
            //"and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)  "+
            //"GROUP BY m.idMetric, m.name, m.description, ts.fio "+
            //"having count (ts.test_date) = 0) q "+
            //"group by q.id, q.test_name, q.text";

    }



    #region Бублики
    private void InitYearChart()
    {
        this.ChartData.Click -= new ImageMapEventHandler(Chart1_Click);
        this.ChartData.Click += new ImageMapEventHandler(Chart1_Click);

        // direct using of PostBackValue
        foreach (System.Web.UI.DataVisualization.Charting.Series series in this.ChartData.Series)
            series.PostBackValue = "series:" + series.Name + ",#INDEX";

        // transfer of click coordinates. getCoordinates is a javascript function.
        this.ChartData.Attributes["onclick"] = ClientScript.GetPostBackEventReference(this.ChartData, "@").Replace("'@'", "'chart:'+_getCoord(event)");

        // set position to relative in order to get proper coordinates.
        this.ChartData.Style[HtmlTextWriterStyle.Position] = "relative";
        this.ClientScript.RegisterClientScriptBlock(
            typeof(Chart),
            "Chart",
            @"function _getCoord(event){if(typeof(event.x)=='undefined'){return event.layerX+','+event.layerY;}return event.x+','+event.y;}",
            true);


        ChartData.AntiAliasing = AntiAliasingStyles.All;
        ChartMonth.AntiAliasing = AntiAliasingStyles.All;
        ChartMonth.Series["MonthSeries"]["DoughnutRadius"] = "20";
        ChartData.Series["DataSeries"]["DoughnutRadius"] = "80";
        

        System.Web.UI.DataVisualization.Charting.Series sdPoints = ChartData.Series[0];
        for (int i = 0; i < 12; i++)
            sdPoints.Points[i]["Exploded"] = "true";

        //sdPoints = ChartMonth.Series[0];
        //for (int i = 0; i < 12; i++)
        //    sdPoints.Points[i]["Exploded"] = "true";


        ShowMonthInfo(DateTime.Now.Year, DateTime.Now.Month, false);
    }

    private void GetHRMonthData(string year)
    {

        if (idCompany == null)
            return;

        for (int i = 1; i < 13; i++)
        {
            string sql =
                "  declare @curr_per int " +
                "  set @curr_per = dbo.DateToPeriodID('" + year + (i < 10 ? "0" + i.ToString() : i.ToString()) + "01') " +
                "  select   " +
                "  ( " +
                //     -- текущие 
                "      select 'Тек: ' + cast(isnull(count(id), 0) as varchar(10)) " +
                "      from subject_group " +
                "      where idcompany = " + idCompany.ToString() + 
                "      and dbo.DateToPeriodID(start_date) = @curr_per and stop_date is null  " +
                "  ) teck " +
                "  , ( " +
                //     -- завершенные
                "      select 'Зав: ' + cast(isnull(count(id), 0) as varchar(10)) " +
                "      from subject_group " +
                "      where idcompany = " + idCompany.ToString() +
                "      and dbo.DateToPeriodID(start_date) = @curr_per and stop_date is not null  " +
                "  ) zav " +
                "  , ( " +
                //     --запланированные " +
                "      select 'Зап: ' + cast(isnull(count(id), 0) as varchar(10)) " +
                "      from subject_group " +
                "      where idcompany = " + idCompany.ToString() +
                "      and dbo.DateToPeriodID(start_date) > @curr_per   " +
                "  ) zap ";

            
            SqlHRMonth.SelectCommand = sql;
            DataView dw = (DataView)SqlHRMonth.Select(new DataSourceSelectArguments());

            if (dw == null)
                return;

            foreach (DataRow dataRow in dw.ToTable().Rows)
            {

                System.Web.UI.DataVisualization.Charting.Series sdPoints = ChartData.Series[0];
                (sdPoints.Points[i - 1] as DataPoint).Label =
                    (dataRow["teck"] != null ? dataRow["teck"].ToString() : "") + "\n" +
                    (dataRow["zap"] != null ? dataRow["zav"].ToString() : "") + "\n" +
                    (dataRow["zav"] != null ? dataRow["zap"].ToString() : "");
                break;
            }
        }
    }

    protected void Chart1_Click(object sender, ImageMapEventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "tableHRShow", "tableHRShow();", true);
        
        System.Web.UI.DataVisualization.Charting.Series sdPoints = ChartData.Series[0];

        string[] input = e.PostBackValue.Split(':');
        if (input.Length == 2)
        {
            string[] seriesData = input[1].Split(',');
            if (input[0].Equals("series"))
            {
                for (int i = 1; i < 12; i++)
                    sdPoints.Points[i]["Exploded"] = "true";

                int selPoint = Convert.ToInt32(seriesData[1]);
                sdPoints.Points[selPoint]["Exploded"] = "false";


                int month = 0;
                if (selPoint >= 9)
                    month = selPoint - 8;
                else
                    month = selPoint + 4;

                ShowMonthInfo(DateTime.Now.Year, month);
            }
        }
    }

    private void ShowMonthInfo(int year, int month, bool isShow = true)
    {
        DateTime startDateD = (new DateTime(year, month, 01));
        DateTime endDateD = startDateD.AddMonths(1);

        string startDate = startDateD.Year.ToString() + (month.ToString().Length == 1 ? "0" + month.ToString() : month.ToString()) + "01";
        string endDate = endDateD.Year.ToString() + (endDateD.Month.ToString().Length == 1 ? "0" + endDateD.Month.ToString() : endDateD.Month.ToString()) + 
            (endDateD.Day.ToString().Length == 1 ? "0" + endDateD.Day.ToString() : endDateD.Day.ToString());

        SqlHRNowResearch.SelectParameters["StartDate"].DefaultValue = startDate;
        SqlHRNowResearch.SelectParameters["EndDate"].DefaultValue = endDate;
        //SqlHRNowResearch.SelectParameters["idcompany"].DefaultValue = idCompany;
        RepeaterHRNowResearch.DataBind();


        SqlHRPlanResearch.SelectParameters["StartDate"].DefaultValue = startDate;
        SqlHRPlanResearch.SelectParameters["EndDate"].DefaultValue = endDate;
        RepeaterHRPlanResearch.DataBind();

        SqlHRClosedResearch.SelectParameters["StartDate"].DefaultValue = startDate;
        SqlHRClosedResearch.SelectParameters["EndDate"].DefaultValue = endDate;
        RepeaterHRClosedResearch.DataBind();


        System.Web.UI.DataVisualization.Charting.Series sdPoints = ChartData.Series[0];

        Label lbl = infoPanel.FindControl("infoPanelHeader") as Label;
        if (lbl != null)
        {
            
            int i = month + 4;
            if (month >= 4)
                i = month - 4;
            else
                i = month + 8;
            lbl.Text = sdPoints.Points[i].ToolTip + " " + year.ToString();
            sdPoints.Points[i]["Exploded"] = "false";
        }

        //TableInfo.Visible = true;

        //if (month >= 1 && month <= 6)
            //TableInfo.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Left;
        //else
        TableInfo.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Right;

        if (isShow)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "HRInfoPanelShow", "HRInfoPanelShow();", true);
            GetHRMonthData(DateTime.Now.Year.ToString());
        }
    }
    #endregion


    private void DrawCompanySubTable(DataView data, Table parentTable, string labelColor, string itemColor)
    {
        parentTable.Rows.Clear();
        TableRow parentTableRow = new TableRow() { Width = new Unit(100, UnitType.Percentage) };
        parentTableRow.Cells.Add(new TableCell() { Width = new Unit(50, UnitType.Percentage), HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Top });
        parentTableRow.Cells.Add(new TableCell() { Width = new Unit(50, UnitType.Percentage), HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Top });

        Table t1 = new Table() { Width = new Unit(100, UnitType.Percentage) };
        Table t2 = new Table() { Width = new Unit(100, UnitType.Percentage) };
        

        int i = 0;
        int rowIndex1 = -1;
        int rowIndex2 = -1;
        string test_name = "";

        foreach (DataRow dataRow in data.ToTable().Rows)
        {
            if (dataRow["test_name"] == null)
                continue;

            if (test_name != dataRow["test_name"].ToString())
            {
                test_name = dataRow["test_name"].ToString();

                DataRow[] innserData = data.ToTable().Select("test_name = '" + test_name + "'");

                if (i % 3 == 0)
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell() { VerticalAlign = VerticalAlign.Top });
                    t1.Rows.Add(row);
                    rowIndex1++;
                    t1.Rows[rowIndex1].Cells[0].Controls.Add(CreateCompanySubTable(innserData, test_name, labelColor, itemColor));
                }
                else
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell() { VerticalAlign = VerticalAlign.Top });
                    t2.Rows.Add(row);
                    rowIndex2++;
                    t2.Rows[rowIndex2].Cells[0].Controls.Add(CreateCompanySubTable(innserData, test_name, labelColor, itemColor));
                }
                i++;
            }
            
        }

        parentTable.Rows.Add(parentTableRow);
        parentTable.Rows[0].Cells[0].Controls.Add(t1);
        parentTable.Rows[0].Cells[1].Controls.Add(t2);
    }

    private Table CreateCompanySubTable(DataRow[] data, string header, string labelColor, string icoColor)
    {
        string headerShort = header;
        if (headerShort.Length > 60)
            headerShort = headerShort.Substring(0, 57) + "...";

        Table table = new Table() { CssClass = "companySubTable" };

        TableRow row = new TableRow();

        for (int r = 0; r <= 3; r++)
        {
            row = new TableRow();
            row.Cells.Add(new TableCell() { Width = new Unit(100, UnitType.Percentage) });
            table.Rows.Add(row);
        }

        //Label label = new Label() { Text = headerShort, CssClass = "companySubTableLabel", ToolTip = header };
        //table.Rows[1].Cells[0].Controls.Add(label);

        // данные
        int i = 2;
        foreach (DataRow dataRow in data)
        {
            if (dataRow["emp_count"] == null || dataRow["id"] == null || dataRow["text"] == null)
                continue;

            row = new TableRow();
            row.Cells.Add(new TableCell() { Width = new Unit(100, UnitType.Percentage) });
            table.Rows.Add(row);

            Image ico = new Image() { ImageUrl = "Images/people_ico_" + icoColor + ".png" };
            table.Rows[i].Cells[0].Controls.Add(ico);

            HyperLink link = new HyperLink() { 
                CssClass = "companySubTableLink",
                NavigateUrl = (dataRow["ismetric"] == null || dataRow["ismetric"].ToString() == "0") ? "~/Analyse/ThreatDetail.aspx?i=" + dataRow["id"].ToString() : "~/Metric/Detail.aspx?id=" + dataRow["id"].ToString()
            };
            table.Rows[i].Cells[0].Controls.Add(link);

            Label linkLabelN = new Label() { Text = dataRow["emp_count"].ToString(), CssClass = "companySubTableLinkLabel" };
            link.Controls.Add(linkLabelN);
            Label linkLabelT = new Label() { Text = dataRow["text"].ToString(), CssClass = "companySubTableLinkLabelText" };
            link.Controls.Add(linkLabelT);

            if (dataRow.Table.Columns.Contains("isNew") && Convert.ToBoolean(dataRow["isNew"]))
            {
                Image icoNew = new Image() { ImageUrl = "Images/Attention_green.png" };
                table.Rows[i].Cells[0].Controls.Add(icoNew);
            }

            i++;
        }

        return table;
    }


    void dgr_DataBound(object sender, EventArgs e)
    {
        TesterChart.SetSectorColor(((System.Web.UI.DataVisualization.Charting.Chart)sender).Series[0].Points);
	}

    protected void btnNewResearch_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format(@"~\group\edit.aspx?g={0}", CommonData.CreateNewResearch()));
    }


    protected void DeleteClosedResearch_Command(object sender, CommandEventArgs e)
    {
        DeleteResearch(Convert.ToInt32(e.CommandArgument));
        RepeaterHRClosedResearch.DataBind();
        Repeater3.DataBind();
    }

    protected void DeletePlanResearch_Command(object sender, CommandEventArgs e)
    {
        DeleteResearch(Convert.ToInt32(e.CommandArgument));
        RepeaterHRPlanResearch.DataBind();
        Repeater2.DataBind();
    }

    protected void DeleteCurrentResearch_Command(object sender, CommandEventArgs e)
    {
        DeleteResearch( Convert.ToInt32 (e.CommandArgument));
        RepeaterHRNowResearch.DataBind();
        Repeater1.DataBind();
    }

    private static void DeleteResearch(int p_idGroup)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            subject_group sg = dc.subject_groups.Where(p => p.id == p_idGroup).FirstOrDefault();
            if (sg != null)
            {
                foreach (Test_SubjectGroup_Link lnk in sg.Test_SubjectGroup_Links)
                {
                    dc.Test_SubjectGroup_Links.DeleteOnSubmit(lnk);
                }
                foreach (Test_Subject ts in sg.Test_Subjects)
                {
                    dc.Delete_Subject(ts.id);
                }

                dc.subject_groups.DeleteOnSubmit(sg);
                dc.SubmitChanges();
            }
        }
    }
}
