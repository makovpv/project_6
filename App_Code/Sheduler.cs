using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// мониторинг и автоматическое создание запланированных исследований по расписанию
/// </summary>
public class Sheduler
{
    long TimerInterval {get;set;}    
    System.Threading.Timer SchdlTimer;
    
    private void do_it(object p_state)
    {
        writeLog("********* do it *********");
        
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            FixMetricDeviations(dc);
            CheckForSchedule(dc);
            CheckForNewEmployee(dc);
            
        }
        SchdlTimer.Change(TimerInterval, System.Threading.Timeout.Infinite);
        writeLog("timer changed");
    }

    private static void writeLog(string p_text)
    {
        using (StreamWriter sw = File.AppendText(string.Format(@"{0}\Data\log.txt", HttpRuntime.AppDomainAppPath)))
        {
            sw.WriteLine(p_text);
        }
    }

    /// <summary>
    /// срез отклонений по метрикам на текущую дату
    /// </summary>
    private void FixMetricDeviations(TesterDataClassesDataContext dc)
    {
        writeLog("void FixMetricDeviations");
        
        try
        {
            writeLog(string.Format ("exec at {0}", DateTime.Now.ToString()));

            dc.ExecuteCommand(
                "DECLARE @idcomp int, @d datetime " +
                "set @d = cast (getdate() as date) " +
                "if not exists (select top 1 1 from metric_hist where mdate = @d) begin " +
                
                "DECLARE ccc CURSOR FAST_FORWARD FOR SELECT id FROM Company " +
                "OPEN ccc " +
                "FETCH NEXT FROM ccc INTO @idcomp " +
                "WHILE @@FETCH_STATUS = 0 BEGIN " +
                "	INSERT INTO dbo.metric_hist	(idmetric, mdate, mNumber, idDept) " +
                "	SELECT  " +
                "		md.idMetric,  " +
                "		@d, " +
                "		COUNT(*) AS number, " +
                "		idDept   " +
                "	FROM dbo.MetricDeviation(@idcomp, null) md " +
                "	GROUP BY md.idMetric, md.idDept " +

                "	FETCH NEXT FROM ccc INTO @idcomp   " +
                "END " +
                "CLOSE ccc " +
                "DEALLOCATE ccc " +

                "end");

            writeLog("exec is finished");

        }
        catch (Exception ex)
        {
            writeLog(ex.Message);
        }
    }

    private void CheckForNewEmployee(TesterDataClassesDataContext p_dc)
    {
        foreach (subject_group sg in p_dc.subject_groups.Where(
            p => p.isAutoEmpNew == true && (p.stop_date == null || p.stop_date > DateTime.Today)))
        {
            ResearchData.AddNewEmployee(sg);
        }
        p_dc.SubmitChanges();
    }

    private void CheckForSchedule(TesterDataClassesDataContext p_dc)
    {
        foreach (schedule sch in p_dc.schedules.Where(p => p.idFreq!= 0 && p.subject_group.start_date <= DateTime.Today))
        //foreach (schedule sch in dc.schedules.Where(p => 1==1))
        {
            DateTime DateForCheck = GetDateForNextResearch(sch.idFreq, (DateTime)sch.subject_group.start_date);
            if (DateForCheck <= DateTime.Today)
            {
                GenerateNewResearch(p_dc, sch);
            }
        }
    }

    public Sheduler(long p_interval)
	{
        TimerInterval = p_interval;
        SchdlTimer = new System.Threading.Timer(do_it, null, TimerInterval, System.Threading.Timeout.Infinite);
	}

    DateTime GetDateForNextResearch(byte p_inc_type, DateTime p_date)
    {
        DateTime NextDate;
        switch (p_inc_type)
        {
            case 1:
                NextDate = p_date.AddMonths(1);
                break;
            case 2:
                NextDate = p_date.AddMonths(3);
                break;
            case 3:
                NextDate = p_date.AddMonths(6);
                break;
            case 4:
                NextDate = p_date.AddYears(1);
                break;
            default:
                NextDate = p_date;
                //NextDate = DateTime.Parse("19000101");
                break;
        }
        return NextDate;
    }

    /// <summary>
    /// создание нового исследования по шаблону
    /// </summary>
    void GenerateNewResearch(TesterDataClassesDataContext p_dc, schedule p_schedule)
    {
        subject_group sg_new = new subject_group()
        {
            name = p_schedule.subject_group.name,
            UserKey = p_schedule.subject_group.UserKey,
            create_date = DateTime.Now,
            mail_template = p_schedule.subject_group.mail_template,
            idCompany = p_schedule.subject_group.idCompany,
            start_date = GetDateForNextResearch(p_schedule.idFreq, (DateTime)p_schedule.subject_group.start_date),
            idReport = p_schedule.subject_group.idReport
        };

        int idTest = p_schedule.subject_group.Test_SubjectGroup_Links.FirstOrDefault().idTest; // рассматриваем только случай с одним тестом в исследовании ?

        // нужен ли этот блок (пустой объект измерения)?
        Test_SubjectGroup_Link lnkTest = new Test_SubjectGroup_Link();
        lnkTest.idTest = idTest;
        lnkTest.OrdNumber = 1;
        lnkTest.isObjectRequired = false; 
        sg_new.Test_SubjectGroup_Links.Add(lnkTest);

        //параметры отчета (набор галочек)

        // респонденты
        switch (p_schedule.idFillType)
        { 
            case 0: // по персонам
                var qqs1 = (from ts in p_schedule.subject_group.Test_Subjects
                         join ua in p_dc.user_accounts on ts.idUser equals ua.idUser
                         select ua).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4);

                foreach (user_account usr in qqs1)
                {
                    Test_Subject ts = new Test_Subject ();
                    ts.idUser = usr.idUser;
                    ts.Nick_Name = usr.login_name;
                    ts.Test_Id = idTest;
                    ts.MailWasSent = false; // is need ?
                    ts.MeasureNumber = 1; // is need ?
                    sg_new.Test_Subjects.Add (ts);
                }
                break;
            case 1: // по отделу
                var qqs2 = (from ts in p_schedule.subject_group.Test_Subjects
                         join ua in p_dc.user_accounts on ts.idUser equals ua.idUser
                         join ua_act in p_dc.user_accounts on ua.idDept equals ua_act.idDept
                          select ua_act).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4).Distinct();

                foreach (user_account usr in qqs2)
                {
                    Test_Subject ts = new Test_Subject ();
                    ts.idUser = usr.idUser;
                    ts.Nick_Name = usr.login_name;
                    ts.Test_Id = idTest;
                    ts.MailWasSent = false; // is need ?
                    ts.MeasureNumber = 1; // is need ?
                    sg_new.Test_Subjects.Add (ts);
                }

                break;
            case 2: // по должности
                var qqs3 = (from ts in p_schedule.subject_group.Test_Subjects
                            join ua in p_dc.user_accounts on ts.idUser equals ua.idUser
                            join ua_act in p_dc.user_accounts on new {ua.idJob, ua.idCompany } equals new {ua_act.idJob, ua_act.idCompany } 
                            select ua_act).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4).Distinct();

                foreach (user_account usr in qqs3)
                {
                    Test_Subject ts = new Test_Subject ();
                    ts.idUser = usr.idUser;
                    ts.Nick_Name = usr.login_name;
                    ts.Test_Id = idTest;
                    ts.MailWasSent = false; // is need ?
                    ts.MeasureNumber = 1; // is need ?
                    sg_new.Test_Subjects.Add (ts);
                }
                break;
            case 3: // по статусу
                var qqs4 = (from ts in p_schedule.subject_group.Test_Subjects
                            join ua in p_dc.user_accounts on ts.idUser equals ua.idUser
                            join ua_act in p_dc.user_accounts on new {ua.idState, ua.idCompany } equals new {ua_act.idState, ua_act.idCompany }
                            select ua_act).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4).Distinct();

                foreach (user_account usr in qqs4)
                {
                    Test_Subject ts = new Test_Subject();
                    ts.idUser = usr.idUser;
                    ts.Nick_Name = usr.login_name;
                    ts.Test_Id = idTest;
                    ts.MailWasSent = false; // is need ?
                    ts.MeasureNumber = 1; // is need ?
                    sg_new.Test_Subjects.Add(ts);
                }
                break;
            case 4: // самооценка с контролем начальника
                ResearchData.FillTestSubject(p_schedule.subject_group, true);

                //var qqs5 = (from ua in p_schedule.subject_group.Company.user_accounts
                //            select ua).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4);

                //foreach (user_account usr in qqs5)
                //{
                //    Test_Subject ts = new Test_Subject ();
                //    ts.idUser = usr.idUser;
                //    ts.Nick_Name = usr.login_name;
                //    ts.Test_Id = idTest;
                //    ts.MailWasSent = false; // is need ?
                //    ts.MeasureNumber = 1; // is need ?
                //    sg_new.Test_Subjects.Add (ts);

                //    Test_SubjectGroup_Link lnk = new Test_SubjectGroup_Link();
                //    lnk.idTest = idTest;
                //    lnkTest.isObjectRequired = false;
                //    lnk.MeasureObjectName = usr.fio;
                //    lnk.test_measure_links.Add (new test_measure_link() {idUser = usr.idUser}); // объект оценки - сам субъект
                //    sg_new.Test_SubjectGroup_Links.Add(lnk);

                //}
                //string[] DeptMngList = System.Web.Security.Roles.GetUsersInRole ("Department manager");
                    
                //foreach (user_account usr in qqs5)
                //{ 
                //    if (DeptMngList.Contains (usr.login_name)) // нач.отдела
                //    {
                //        foreach (Test_SubjectGroup_Link lnk in sg_new.Test_SubjectGroup_Links)
                //        { // объекты измерения (оценки)
                //            if (lnk.MeasureObjectName != usr.fio)
                //            {
                //                if (qqs5.Where (p=> p.idDept == usr.idDept && p.fio == lnk.MeasureObjectName).FirstOrDefault() != null)
                //                {
                //                    lnk.test_measure_links.Add (new test_measure_link() {idUser = usr.idUser});
                //                }
                //            }
                //        }
                //    }                        
                //}
                break;
            case 6: // все действующие сотрудники компании
                ResearchData.FillTestSubject(p_schedule.subject_group, false);
                break;
        }

        p_dc.subject_groups.InsertOnSubmit(sg_new);
        p_schedule.subject_group = sg_new;
        p_dc.SubmitChanges();

    }
    
    /// <summary>
    /// создание нового расписания
    /// </summary>
    /// <param name="p_dc">контекст</param>
    /// <param name="p_idBase">базовое исследование</param>
    /// <param name="p_idFreq">частота повторения</param>
    /// <param name="p_idFillType">способ заполнения</param>
    public static void NewSchedule (TesterDataClassesDataContext p_dc, int p_idBase, byte p_idFreq, byte p_idFillType)
    {
        p_dc.schedules.InsertOnSubmit(new schedule() { idBaseGroup = p_idBase, idFreq = p_idFreq, idFillType = p_idFillType });
        p_dc.SubmitChanges();
    }
}