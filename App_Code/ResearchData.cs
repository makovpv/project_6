using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Web.Security;

/// <summary>
/// Summary description for ResearchData
/// </summary>
public class ResearchData
{
	public ResearchData()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    /// <summary>
    /// последнее по дате исследование для заданного теста и компании
    /// </summary>
    /// <param name="idTest">ID теста</param>
    /// <param name="idCompany">ID компании</param>
    /// <returns></returns>
    public static int LastGroupID(TesterDataClassesDataContext dc, int idTest, int idCompany)
    {
            return
            dc.ExecuteQuery<int>(
            "select top 1 sg.id, lnk.idtest " +
            "from test_subjectgroup_link lnk " +
            "inner join subject_group sg on sg.id = lnk.idGroup " +
            "where lnk.idTest = {0} and sg.idcompany = {1} " +
            "order by sg.start_date desc",
            new object[] { idTest, idCompany }
            ).FirstOrDefault();
    }
    /// <summary>
    /// кол-во респондентов в композитной группе по зоне мониторинга
    /// </summary>
    public static int CompositeGroupSubjCount (TesterDataClassesDataContext dc, int idTest, int idCompany)
    {
        return
            dc.ExecuteQuery<int>(
            "select count (distinct ua.iduser) "+
            "from test_subject ts "+
            "inner join subject_group sg on sg.id = ts.group_id "+
            "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = {1} "+
            "where ts.test_id = {0} and ts.test_date is not null and ua.idState in (2,6)",
            new object[] { idTest, idCompany }
            ).FirstOrDefault();
    }

    /// <summary>
    /// заполнение исследования респондентами
    /// </summary>
    /// <param name="p_group">исследование для заполнения</param>
    /// <param name="SelfAssessment">заполнить субъектами и объектами оценки для "самооценки с контролем"</param>
    public static void FillTestSubject(subject_group p_group, bool SelfAssessment = false)
    {
        Test_SubjectGroup_Link test_lnk = p_group.Test_SubjectGroup_Links.FirstOrDefault();
        if (test_lnk == null)
            throw new Exception("В исследовании отсутствует инструмент. Добавьте хотя бы один тест в исследование");
        else
        {
            var qqs5 = (from ua in p_group.Company.user_accounts
                        select ua).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4 && q.idState != null);
            foreach (user_account usr in qqs5)
            {
                Test_Subject ts = new Test_Subject();
                ts.idUser = usr.idUser;
                ts.Nick_Name = usr.login_name;
                ts.Test_Id = test_lnk.idTest;
                ts.MailWasSent = false; // is need ?
                ts.MeasureNumber = 1; // is need ?
                p_group.Test_Subjects.Add(ts);

                Test_SubjectGroup_Link lnk = new Test_SubjectGroup_Link();
                lnk.idTest = test_lnk.idTest;
                lnk.isObjectRequired = false;
                lnk.MeasureObjectName = usr.fio;
                lnk.test_measure_links.Add(new test_measure_link() { idUser = usr.idUser }); // объект оценки - сам субъект
                p_group.Test_SubjectGroup_Links.Add(lnk);
            }

            if (SelfAssessment)
            {
                string[] DeptMngList = System.Web.Security.Roles.GetUsersInRole("Department manager");
                foreach (user_account usr in qqs5)
                {
                    if (DeptMngList.Contains(usr.login_name)) // нач.отдела
                    {
                        foreach (Test_SubjectGroup_Link lnk in p_group.Test_SubjectGroup_Links)
                        { // объекты измерения (оценки)
                            if (lnk.MeasureObjectName != usr.fio)
                            {
                                if (qqs5.Where(p => p.idDept == usr.idDept && p.fio == lnk.MeasureObjectName).FirstOrDefault() != null)
                                {
                                    lnk.test_measure_links.Add(new test_measure_link() { idUser = usr.idUser });
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void AddNewEmployee(subject_group sg)
    {
        Test_SubjectGroup_Link test_lnk = sg.Test_SubjectGroup_Links.FirstOrDefault();
        if (test_lnk == null)
            throw new Exception("В исследовании отсутствует инструмент. Добавьте хотя бы один тест в исследование");
        else
            foreach (user_account ua in sg.Company.user_accounts.Where(p => p.idState != 1 && p.idState != 3 && p.idState != 4 && p.idState != null))
            {
                if (sg.Test_Subjects.Count(q => q.idUser == ua.idUser) == 0)
                { 
                    Test_Subject ts = new Test_Subject();
                    ts.idUser = ua.idUser;
                    ts.Nick_Name = ua.login_name;
                    ts.Test_Id = test_lnk.idTest;
                    ts.MailWasSent = false; 
                    ts.MeasureNumber = 1;

                    sg.Test_Subjects.Add(ts);
                }
            }
    }

    public static void SendMailFor(IQueryable<Test_Subject> SubjList, string MailTemplate, out string log)
    {
        log = "";
        string rootURL = HttpContext.Current.Request.Url.Host;
        SmtpClient smtp = new SmtpClient("mail.testplayer.org");
        smtp.Credentials = new NetworkCredential("info@testplayer.org", "qwe!@#^%$"); //IMPORANT:  Your smtp login email MUST be same as your FROM address. 

        foreach (Test_Subject subj in SubjList)
        {
            MailMessage mail = new MailMessage();
            string SubjEmail = "";
            try
            {
                mail.From = new MailAddress("info@testplayer.org"); //IMPORTANT: This must be same as your smtp authentication address.

                MembershipUser member = Membership.GetUser (subj.idUser);
                SubjEmail = member.Email;

                if (SubjEmail.Contains("gmail.com"))
                {
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                }
                else
                {
                    smtp.Port = 25;
                    smtp.EnableSsl = false;
                }

                mail.To.Add(SubjEmail); 
                //set the content 
                mail.Subject = "Приглашение пройти тестирование на ulleum.com";

                string IndividualLink = string.Format("{0}/player/testtrack.aspx?s={1}", rootURL, subj.id);
                mail.Body = MailTemplate.Replace("#link#", IndividualLink).Replace ("#fio#", subj.fio) ;
                //mail.Body = tboxMailTemplate.Text.Replace("#enddate#", sg.stop_date.ToString());
                //mail.Body = tboxMailTemplate.Text.Replace("#name#", sg.name);

                smtp.Send(mail);
                // надо еще MailWasSent потом проставлять = true !
                log += string.Format("приглашение успешно отправлено на {0}<br/>", SubjEmail);

                subj.MailWasSent = true;
            }
            catch (Exception ex)
            {
                log += string.Format(string.Format("{0} - {1}<br/>", SubjEmail, ex.Message));
            }
        }
    }

    public static void SaveResearch(
        int p_id, string p_Name, int p_idCompany, 
        string p_StartDate, string p_StopDate, 
        bool p_isForYearPlan,
        bool p_isSubjAutoAdd,
        bool p_isEmpAutoNew,
        bool p_isAnonymous,
        byte p_Freq,
        byte p_SubjFillType
        )
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            subject_group sg = dc.subject_groups.Where(p => p.id == p_id).FirstOrDefault();
            sg.name = p_Name;
            sg.idCompany = p_idCompany;

            DateTime sdate;
            if (DateTime.TryParse(p_StopDate, out sdate))
                sg.stop_date = sdate;
            else sg.stop_date = null;

            if (DateTime.TryParse(p_StartDate, out sdate))
                sg.start_date = sdate;
            else sg.start_date = null;

            sg.isForYearPlan = p_isForYearPlan;
            sg.isAutoSubjAdd = p_isSubjAutoAdd;
            sg.isAutoEmpNew  = p_isEmpAutoNew;
            sg.isAnonymous = p_isAnonymous;

            schedule schdl = sg.schedules.FirstOrDefault();
            if (schdl != null)
            {
                schdl.idFreq = p_Freq; 
                schdl.idFillType = p_SubjFillType; 
            }
            dc.SubmitChanges();
        }
    }

    /// <summary>
    /// генерируем "отложенные" ответы для вопросов
    /// </summary>
    /// <param name="sg">исследование</param>
    public static void GenerateDelayedAnswer(subject_group sg)
    { 
        foreach (Test_SubjectGroup_Link sgl in sg.Test_SubjectGroup_Links)
        {
            foreach (SubScaleDimension ssd in sgl.test.SubScaleDimensions.Where (p=> p.dimension_type == (byte)DimensionType.dtEMP))
            {
                CommonData.GenerateAnswerWithEMP(ssd, sg.Company);
            }
            foreach (SubScaleDimension ssd in sgl.test.SubScaleDimensions.Where(p => p.dimension_type == (byte)DimensionType.dtCompetence))
            {
                CommonData.GenerateAnswerWithCompetence(ssd, sg.Company);
            }
            foreach (SubScaleDimension ssd in sgl.test.SubScaleDimensions.Where(p => p.dimension_type == (byte)DimensionType.dtBook))
            {
                CommonData.GenerateAnswerWithBook(ssd, sg.Company);
            }
        }
        
    }
}