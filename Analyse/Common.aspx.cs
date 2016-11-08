using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;


//public class GridViewHelper
//{
//    public GridViewHelper(GridView grd, bool useFooterForGeneralSummaries, SortDirection groupSortDirection)
//    {
//        this.mGrid = grd;
//        this.useFooter = useFooterForGeneralSummaries;
//        this.groupSortDir = groupSortDirection;
//        this.mGeneralSummaries = new GridViewSummaryList();
//        this.mGroups = new GridViewGroupList();
//        this.mGrid.RowDataBound += new GridViewRowEventHandler(RowDataBoundHandler);
//    }
//}



public partial class Analyse_Common : System.Web.UI.Page
{
    int GroupID { get { return Convert.ToInt16(Request.QueryString["g"]); } }
    string ZoneName { get { return Request.QueryString["ma"]; } }
    bool IsSingleGroupRep { get { return Request.QueryString["g"] != null;} }
    int idCompany { get; set; }
    bool IsHR {get; set;}
    int? idDept { get; set; }                            

    int testid = 0;
    int fmainscaleid = 0;
    int MainScaleID 
    {
        get
        {
            if (fmainscaleid == 0)
            {
                TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
                Scale s = dc.Test_SubjectGroup_Links.Where(p => p.idGroup == GroupID).FirstOrDefault().test.Scales.Where(q => q.isMain == true).FirstOrDefault();
                if (s != null)
                {
                    fmainscaleid = s.id;
                    testid = s.test_id;
                }
                else
                    throw new Exception("не установлена главная шкала");
            }
            return fmainscaleid;
        }
    }
    int TestID
    {
        get
        {
            if (testid == 0)
            {
                using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
                {
                    Scale s;
                    if (IsSingleGroupRep)
                    {
                        Test_SubjectGroup_Link lnk = dc.Test_SubjectGroup_Links.Where(p => p.idGroup == GroupID).FirstOrDefault();
                        if (lnk != null)
                            s = lnk.test.Scales.Where(q => q.isMain == true).FirstOrDefault();
                        else
                            throw new Exception("В исследовании отсутствуют инструменты (тесты)");
                    }
                    else
                        s = dc.tests.Where(p => p.monitoring_area == ZoneName).FirstOrDefault().Scales.Where(q => q.isMain == true).FirstOrDefault();

                    if (s != null)
                    {
                        fmainscaleid = s.id;
                        testid = s.test_id;
                    }
                    else
                        throw new Exception("Не удается определить главную шкалу");
                }
            }
            return testid;
        }
    }

    DataTable tblMainScale;
    DataTable tblSummary = new DataTable();
    DataTable tblCommon = new DataTable();
    DataTable tblPersonalMainScore = new DataTable();
    DataTable tbl7 = new DataTable();
    DataTable tblMeasureCommon = new DataTable();
    DataTable tblMeasure_1 = new DataTable();

    List<rep_item_link> RepLinks;
    List<Scale_Norm> SNList;
    List<Norm_Type> NormColors;
    List<Scale> ScaleList;

    enum RepItemType
    {
        ritInfo = 3, ritContents = 5,
        ritIndicator = 10, ritRound = 20, ritMainScaleScore = 30, 
        ritByJob = 40, ritDiagramByJob = 45, ritByDept = 50, ritDiagramByDept = 55, ritDiagram = 60,
        ritSummaryAll = 70, ritFreq = 100, ritMostPopular = 105,
        ritMeasureObj = 110,
        ritPersonalMainScore = 80,
        ritPersonalMainProc = 90,
        ritRawSubjectData = 120
    }

    #region Query Texts
    // похоже, все можно свести с общему случаю (*_composite)
    const string qry1_single =
        "declare @c float " +
        "select @c=COUNT(*) from Test_Subject ts where Test_Id=@TestID and test_date is not null " +
        "SELECT ts.id, " +
        "case when ts.fio is null then 'Респондент №'+cast(ts.id as varchar(10)) else ts.fio end as fio, " +
        "ts.Test_Date, " +
        "td.Test_Value, " +
        "@ScaleID as scale_id, " +
        "RANK() over (order by td.test_value desc) as rate, " +
        "(select COUNT(*)/@c " +
        "from Test_Subject ts1 " +
        "inner join Test_Data td1 on td1.Subject_ID = ts1.id and td1.Scale_ID = td.Scale_ID " +
        "where Test_Id=ts.Test_ID and td1.Test_Value < td.test_value " +
        ") as pp, " +
        "d.name as dept_name, j.name as job_name, " +
        "dbo.relevance_for_date (ts.test_date, getdate()) as relevance, " +
        "null as number " +
        "from Test_Subject ts " +
        "left join Test_Data td on td.Subject_ID = ts.id and td.Scale_ID = @ScaleID " +
        "left join user_account ua on ua.iduser = ts.iduser " +
        "left join dept d on ua.iddept = d.id " +
        "left join job j on ua.idjob = j.id " +
        "where ts.group_id=@GroupID and ts.test_date is not null and (ua.iddept = @idDept or @HR = 1)";
    const string qry1_composite =
        "with last_cte (iduser, last_date, iddept, idjob, number) as ( " +
        "select ts.iduser, max(ts.test_date) as last_date, max(ua.iddept) iddept, max(ua.idjob) as idjob, " +
        "sum(case when ts.test_date is not null then 1 else 0 end) as [number] " +
        "from Test_Subject ts " +
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany " +
        "where ts.test_id= @TestID and /*ts.test_date is not null and*/ ua.idState in (2,6) and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser " +
        ") " +
        "SELECT DISTINCT " +
        //"ts.id,  " +
        "case when ts.fio is null then 'Респондент №'+cast(ts.id as varchar(10)) else ts.fio end as fio,  " +
        "ts.Test_Date,  " +
        "td.Test_Value,  " +
        "td.scale_id, " +
        "RANK() over (order by td.test_value desc) as rate, " +
        "(select COUNT(*)/@TotalSubjCount " +
        "from Test_Subject ts1 " +
        "inner join Test_Data td1 on td1.Subject_ID = ts1.id and td1.Scale_ID = td.Scale_ID " +
        "where Test_Id=ts.Test_ID and td1.Test_Value < td.test_value " +
        ") as pp, " +
        "d.name as dept_name, j.name as job_name, " +
        "dbo.relevance_for_date (ts.test_date, getdate()) as relevance, " +
        "last_cte.number " +
        "from test_subject ts " +
        "inner join last_cte on ts.iduser = last_cte.iduser and isnull(ts.test_date,'19000101') = isnull(last_cte.last_date,'19000101' ) " +
        "left join Test_Data td on td.Subject_ID = ts.id and td.Scale_ID = @ScaleID " +
        "left join dept d on last_cte.iddept = d.id " +
        "left join job j on last_cte.idjob = j.id " +
        "order by 7, 1";
    const string qryJob_single =
        "declare @c float " +
        "select @c=count(*) from test_data where scale_id=@ScaleID " +
        "select j.name, left (j.name, 8) as short_name, " +
        "round(avg_score,0) as avg_score, avg_pp, avg_pp*100 as avg_pp_100, rank () over (order by avg_score desc) as rate, subj_count from ( " +
        "select ua.idjob, avg(score) as avg_score, avg(pp) as avg_pp, count(*) as subj_count " +
        "from ( " +
        "select ts.iduser, td.test_value as score, " +
        "(select count (*)/@c from test_data tdd where tdd.scale_id=@ScaleID and tdd.test_value <= td.test_value) as pp  " +
        "from test_subject ts " +
        "inner join test_data td on td.subject_id=ts.id and td.scale_id=@ScaleID " +
        "where ts.group_id=@GroupID " +
        ") ii " +
        "left join user_account ua on ua.iduser=ii.idUser " +
        "where (ua.iddept = @idDept or @HR = 1) " +
        "group by ua.idjob) q " +
        "left join job j on j.id=q.idjob ";
    const string qryJob_composite =
        "with last_cte (iduser, last_date, iddept, idjob) as ( " +
        "select ts.iduser, max(ts.test_date) as last_date, max(ua.iddept) iddept, max(ua.idjob) as idjob  " +
        "from Test_Subject ts  " +
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany  " +
        "where ts.test_id= @TestID and ts.test_date is not null and ua.idState in (2,6)  and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser  " +
        ")  " +
        "select j.name, left (j.name, 8) as short_name, "+
        "round(avg_score,0) as avg_score, avg_pp, avg_pp*100 as avg_pp_100, rank () over (order by avg_score desc) as rate, subj_count from  " +
        "( " +
        "  select idjob, avg(test_value) as avg_score, avg(pp) as avg_pp, count(*) as subj_count from ( "+
        "		select  " +
        "			last_cte.idJob, td.test_value,  " +
        "			(select count (*)/@TotalSubjCount from test_data tdd where tdd.scale_id=@ScaleID and tdd.test_value <= td.test_value) as pp  " +
        "		from test_subject ts " +
        "		inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date " +
        "		inner join test_data td on td.subject_id=ts.id and td.scale_id=@ScaleID ) qq" +
        "		group by qq.idjob " +
        "		) q " +
        "left join job j on j.id=q.idjob";
    const string qryDept_single =
        "declare @c float " +
        "select @c=count(*) from test_data where scale_id=@ScaleID " +
        "select d.name, round(avg_score,0) as avg_score, avg_pp, avg_pp*100 as avg_pp_100, rank () over (order by avg_score desc) as rate, subj_count from ( " +
        "select ua.iddept, avg(score) as avg_score, avg(pp) as avg_pp, count(*) as subj_count " +
        "from ( " +
        "	select ts.iduser, td.test_value as score, " +
        "		(select count (*)/@c from test_data tdd where tdd.scale_id=@ScaleID and tdd.test_value < td.test_value) as pp  " +
        "	from test_subject ts " +
        "	inner join test_data td on td.subject_id=ts.id and td.scale_id=@ScaleID " +
        "	where ts.group_id=@GroupID " +
        "	) ii " +
        "left join user_account ua on ua.iduser=ii.idUser " +
        "where (ua.iddept = @idDept or @HR = 1) " +
        "group by ua.iddept) q " +
        "left join dept d on d.id=q.iddept";
    const string qryDept_composite =
        "with last_cte (iduser, last_date, iddept, idjob) as ( " +
        "select ts.iduser, max(ts.test_date) as last_date, max(ua.iddept) iddept, max(ua.idjob) as idjob  " +
        "from Test_Subject ts  " +
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany  " +
        "where ts.test_id= @TestID and ts.test_date is not null and ua.idState in (2,6)  and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser  " +
        ")  " +
        "select d.name, round(avg_score,0) as avg_score, avg_pp, avg_pp*100 as avg_pp_100, rank () over (order by avg_score desc) as rate, subj_count from  " +
        "( " +
        "  select iddept, avg(test_value) as avg_score, avg(pp) as avg_pp, count(*) as subj_count from ( " +
        "		select  last_cte.iddept, td.test_value,  " +
        "			(select count (*)/@TotalSubjCount from test_data tdd where tdd.scale_id=@ScaleID and tdd.test_value < td.test_value) as pp  " +
        "		from test_subject ts " +
        "		inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date " +
        "		inner join test_data td on td.subject_id=ts.id and td.scale_id=@ScaleID ) qq" +
        "		group by qq.iddept ) q " +
        "left join dept d on d.id=q.iddept";
    const string qryDiagram_single =
        //"select 1 as normtype_id, 'очень низкое (0-20)' as name, 2 as subj_count union all "+
        //"select 2 as normtype_id, 'низкое (21-40)' as name, 3 as subj_count  ";
        "select NormType_id, " +
        "nt.name+' ('+cast(cast(sn.lowrange as decimal(4,0)) as varchar(7))+'-'+cast(cast(sn.highrange as decimal(4,0)) as varchar(7)) +')' as name, " +
        "COUNT(*) subj_count " +
        "from Test_Subject ts " +
        "inner join user_account ua on ua.iduser = ts.iduser " +
        "inner join Test_Data td on td.Subject_ID = ts.id " +
        "inner join Scale_Norm sn on td.Scale_ID =sn.Scale_ID and td.Test_Value between sn.LowRange and sn.HighRange " +
        "inner join Norm_Type nt on nt.id = sn.NormType_id " +
        "where ts.group_id = @GroupID and td.Scale_ID = @ScaleID and measurenumber=1 and (ua.iddept = @idDept or @HR = 1)" +
        "group by NormType_id, nt.name, sn.lowrange, sn.highrange";
    //const string qryDiagram_composite =
    //    "with last_cte (iduser, last_date) as ( "+
    //    "select ts.iduser, max(ts.test_date) as last_date "+
    //    "from Test_Subject ts "+  
    //    "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany "+
    //    "where ts.test_id= @TestID and ts.test_date is not null and ua.idState in (2,6)  and (ua.iddept = @idDept or @HR = 1) " +  
    //    "group by ts.iduser)  "+ 
    //    "select NormType_id, "+
    //    "nt.name+' ('+cast(cast(sn.lowrange as decimal(4,0)) as varchar(7))+'-'+cast(cast(sn.highrange as decimal(4,0)) as varchar(7)) +')' as name, "+
    //    "COUNT(*) subj_count "+
    //    "from Test_Subject ts " +
    //    "inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date  "+
    //    "inner join Test_Data td on td.Subject_ID = ts.id "+
    //    "inner join Scale_Norm sn on td.Scale_ID =sn.Scale_ID and td.Test_Value between sn.LowRange and sn.HighRange "+
    //    "inner join Norm_Type nt on nt.id = sn.NormType_id "+
    //    "where td.Scale_ID = @ScaleID and measurenumber=1 "+
    //    "group by NormType_id, nt.name, sn.lowrange, sn.highrange";
    const string qry2_single =
        "declare @c float " +
        "select @c=COUNT(*) from Test_Subject ts where Test_Id=@TestID " +
        "SELECT ts.id,  " +
        "case when ts.fio is null then 'Респондент №'+cast(ts.id as varchar(10)) else ts.fio end as fio,  " +
        "ts.Test_Date,  " +
        "cast(round(td.Test_Value,0) as int) as test_value,  " +
        "s.id as scale_id, s.name as scale_name, " +
        "(select cast(round(COUNT(*)/@c *100, 0) as int) " +
        "from Test_Subject ts1 " +
        "inner join Test_Data td1 on td1.Subject_ID = ts1.id and td1.Scale_ID = td.Scale_ID " +
        "where Test_Id=ts.Test_ID and td1.Test_Value < td.test_value " +
        ") pp " +
        "from Test_Subject ts " +
        "inner join user_account ua on ua.iduser = ts.iduser "+
        "left join Test_Data td on td.Subject_ID = ts.id " +
        "inner join scales s on s.id = td.scale_id " +
        "where ts.group_id=@GroupID and (ua.iddept = @idDept or @HR = 1)" +
        "order by ts.id, s.id";
    const string qry2_composite =
        "with last_cte (iduser, last_date) as (  " +
        "select ts.iduser, max(ts.test_date) as last_date  " +
        "from Test_Subject ts  " +
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany " +
        "where ts.test_id= @TestID and ts.test_date is not null and ua.idState in (2,6) and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser)   " +
        "SELECT ts.id,  " +
        "case when ts.fio is null then 'Респондент №'+cast(ts.id as varchar(10)) else ts.fio end as fio,  " +
        "ts.Test_Date,  " +
        "cast(round(td.Test_Value,0) as int) as test_value,  " +
        "s.id as scale_id, s.name as scale_name, " +
        "(select cast(round(COUNT(*)/@TotalSubjCount *100, 0) as int) " +
		"	 from Test_Subject ts1 " +
		"	 inner join Test_Data td1 on td1.Subject_ID = ts1.id and td1.Scale_ID = td.Scale_ID " +
		"	 where Test_Id=ts.Test_ID and td1.Test_Value < td.test_value " +
		") pp " +
        "FROM test_subject ts " +
        "inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date " +
        "left join Test_Data td on td.Subject_ID = ts.id " +
        "inner join scales s on s.id = td.scale_id " +
        "order by ts.id, s.id";
    const string qryFreq_single =
        "declare @idtest int, @subjcount decimal " +
        "select top 1 @idtest = idtest from test_subjectgroup_link where idgroup=@idgroup "+
        "select @subjcount = count(*) from test_subject inner join user_account ua on ua.iduser=test_subject.iduser "+
        "where group_id = @idgroup and test_date is not null and (ua.iddept = @idDept or @HR = 1) " +

        "select sum(case when ts.id is not null and tr.selectedvalue = 1 then 1 else 0 end) as freq,  " +
        "i.id,i.text, ss.ordernumber "+
        "from test_question tq "+
        "inner join items i on i.group_id = tq.id "+
        "inner join subscales ss on ss.dimension_id=i.dimension_id "+
        "left join test_results tr on tr.item_id=i.id and tr.subscale_id=ss.id and tr.selectedvalue=1 "+
        "left join test_subject ts on ts.id = tr.subject_id "+
        "left join user_account ua on ua.iduser = ts.iduser "+

        "where tq.test_id = @idTest  "+
        "and (ts.group_id = @idGroup or ts.group_id is null) and (ua.iddept = @idDept or @HR = 1)" +
        "group by i.id, i.number, i.text, ss.id, ss.ordernumber "+
        "order by i.number, ss.ordernumber";

    const string qryRawData_single =
        "SELECT FIO, test_date, [text], header FROM ( "+
        "select FIO, test_date, txt.[text], i.[text] as header , i.id as item_id, i.number " +
        "from test_subject ts "+
        "left join test_results_txt txt on ts.id = txt.subject_id "+
        "left join items i on i.id = txt.item_id " +
        "where ts.group_id = @GroupID and test_date is not null " +
        "union all "+ // множественный выбор одной текстовой строкой
        "select FIO, test_date, "+
        "dbo.GetMultyAnswerLine (i.id, ts.id), "+
        " i.[text] as header, i.id, i.number "+
		"from test_subject ts "+
		"INNER join subscaledimension ssd on ssd.test_id = ts.test_id and ssd.dimension_type in (13,1,2)  "+ // при "left join" в такую таблицу в чисто текстовых опросниках попадает пустой столбец всегда
		"left join items i on i.dimension_id = ssd.id  "+
        "where ts.group_id = @GroupID and test_date is not null) q " +
        "order by test_date desc, FIO, number, item_id";
    

    const string qryFreq_composite =
        "with last_cte (iduser, last_date) as (  " +
        "select ts.iduser, max(ts.test_date) as last_date  "+
        "from Test_Subject ts  "+
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany " +
        "where ts.test_id= @idTest and ts.test_date is not null and ts.basesubject_id is null and ua.idState in (2,6) and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser)   "+
        "SELECT  "+
        "sum(case when last_cte.iduser is not null and tr.selectedvalue = 1 then 1 else 0 end) as freq,  " +
        "i.id,i.text, ss.ordernumber "+
        "from test_question tq "+
        "inner join items i on i.group_id = tq.id "+
        "inner join subscales ss on ss.dimension_id=i.dimension_id "+
        "left join test_results tr on tr.item_id=i.id and tr.subscale_id=ss.id and tr.selectedvalue=1 "+
        "left join test_subject ts on ts.id = tr.subject_id and ts.basesubject_id is null "+
        "left join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date "+
        "where tq.test_id = @idTest " +
        "group by i.id, i.number, i.text, ss.id, ss.ordernumber "+
        "order by i.number, ss.ordernumber";
    const string qryMostPopular_composite =
        "with last_cte (iduser, last_date) as (  " +
        "select ts.iduser, max(ts.test_date) as last_date  " +
        "from Test_Subject ts  " +
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany " +
        "where ts.test_id= @idTest and ts.test_date is not null and ua.idState in (2,6) and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser)   " +
        "SELECT q.item_id, q.subscale_id, i.text, ss.name, "+
        //"cast(round(100.0 * cnt / cast(@CompositeSubjCount as decimal),0) as int) as freq "+
        "q.cnt / cast(com.cnt as decimal) as freq " +
        "from ( "+
        "select tr.item_id, subscale_id, count(*) cnt, row_number () over (partition by tr.item_id order by count(*) desc) as rn "+
        "from test_subject ts "+
        "inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date "+
        "inner join test_results tr on tr.subject_id=ts.id "+
        "where ts.test_id = @idTest and tr.selectedvalue=1 " +
        "group by tr.item_id, subscale_id "+
        ") q "+
        "inner join items i on i.id=q.item_id and q.rn=1 "+
        "inner join subscales ss on ss.id=q.subscale_id and q.rn=1 " +
        "inner join ( "+
			"select tr.item_id, count(*) cnt "+
			"from test_subject ts "+
			"inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date "+
			"inner join test_results tr on tr.subject_id=ts.id "+
			"where ts.test_id = @idTest and tr.selectedvalue=1 "+
			"group by tr.item_id "+
		") com on com.item_id = i.id "+
        "order by i.number";

    #endregion Query Texts
    
    private SqlDataSource sqlMostPopular;
    Dictionary<string, string> RepCommonInfo = new Dictionary<string, string>();

    protected void Page_Load(object sender, EventArgs e)
    {
        InitSQL();

        if (!IsPostBack)
        {
            System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser();
            if (usr == null)
                return;
            lblUserNameTitle.Text = usr.UserName;

            if (User.IsInRole("HR") || User.IsInRole("Department manager"))
                lblRoleTitle.Text = " (HR)";
            if (User.IsInRole("Department manager"))
                lblRoleTitle.Text = " (руководитель отдела)";
            if (User.IsInRole("Admin"))
                lblRoleTitle.Text = " (admin)";

            if (IsSingleGroupRep)
                RepCommonInfo = CommonData.GetGroupCommonInfo(GroupID);
            else
            {
                RepCommonInfo = new Dictionary<string, string>();
                RepCommonInfo.Add("GroupName", string.Format("Отчет по зоне контроля '{0}'", ZoneName));
                RepCommonInfo.Add("TestName", "наименование теста");
                RepCommonInfo.Add("TestPeriod", "начало-окончание");
            }

            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                ScaleList = dc.Scales.Where(p => p.test_id == TestID).OrderBy(q => q.id).ToList();
                SNList = dc.Scale_Norms.Where(p => p.Scale_ID == MainScaleID).ToList();
                NormColors = dc.Norm_Types.ToList();

                IsHR = User.IsInRole("HR") || User.IsInRole("Admin");
                user_account current_ua = dc.user_accounts.Where(p => p.idUser == CommonData.GetCurrentUserKey()).FirstOrDefault();
                idDept = current_ua.idDept;

                if (IsSingleGroupRep)
                    RepLinks = dc.rep_item_links.Where(p => p.idGroup == GroupID).ToList();
                else
                {   // находим исследование для определения списка элементов отчета
                    idCompany = (int)current_ua.idCompany;
                    RepCommonInfo.Add("CompanyName", current_ua.Company.name);
                    RepCommonInfo.Add("SubjCount", ResearchData.CompositeGroupSubjCount(dc, TestID, idCompany).ToString());
                    RepLinks = new List<rep_item_link>();
                    foreach (rep_test_link rtl in dc.rep_test_links.Where(p => p.idTest == TestID))
                    {
                        RepLinks.Add(new rep_item_link() { idItemType = rtl.idItemType });
                    }
                }
                if (!RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritInfo))
                    RemoveReportControl(divInfo);
                if (!RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritContents))
                    RemoveReportControl(divContent);

                SetSQLCommand(dc, IsSingleGroupRep, RepLinks);
                FillCommonData(dc, ScaleList);

                // определяем макс. значение для диаграммы
                Scale scl = ScaleList.Where(p => p.isMain == true).FirstOrDefault();
                int MaxScaleValue = scl.ScoreCalcType == 3 ? 100 : Convert.ToInt32(scl.max_value); // если тип расчета "процент", то макс = 100%

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagramByJob))
                {
                    System.Web.UI.DataVisualization.Charting.Chart dgr = TesterChart.DrawBarChart(600, MaxScaleValue);
                    dgr.Series[0].XValueMember = "name";
                    dgr.Series[0].YValueMembers = "avg_score";
                    dgr.DataSourceID = "sqlDataByJob";
                    pnlBarJob.Controls.Add(dgr);
                }
                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritByJob))
                {
                    gridByJob.DataSourceID = "sqlDataByJob";
                    //gridByJob.DataBind();
                }
                else
                {
                    lblByJob.Visible = false;
                    RemoveReportControl(ref_main_test_job);
                }

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagramByDept))
                {
                    System.Web.UI.DataVisualization.Charting.Chart dgr = TesterChart.DrawBarChart(600, MaxScaleValue);
                    dgr.Series[0].XValueMember = "name";
                    dgr.Series[0].YValueMembers = "avg_score";
                    dgr.DataSourceID = "sqlDataByDept";
            
                    pnlBarDept.Controls.Add(dgr);
                }
                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritByDept))
                {
                    gridByDept.DataSourceID = "SqlDataByDept";
                    //gridByDept.DataBind();
                }
                else
                {
                    lblByDept.Visible = false;
                    RemoveReportControl(ref_main_test_dept);
                }

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritSummaryAll))
                {
                    Gridview2.DataSource = tblSummary;
                    Gridview2.DataBind();
                }
                else
                {
                    RemoveReportControl(ref_summary_all);
                    RemoveReportControl(lbl_summary_all);
                }

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritMainScaleScore))
                {
                    lblGridView1.Visible = true;
                    Gridview1.DataSource = tblMainScale;
                    Gridview1.DataBind();
                }
                else
                    RemoveReportControl(ref_main_test);

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritPersonalMainScore))
                {
                    lblGridView6.Visible = true;
                    Gridview6.DataSource = tblPersonalMainScore;
                    Gridview6.DataBind();
                }
                else
                    RemoveReportControl(ref_personal_test_all);

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritPersonalMainProc))
                {
                    lblGridView7.Visible = true;
                    Gridview7.DataSource = tbl7;
                    Gridview7.DataBind();
                }
                else
                    RemoveReportControl(ref_personal_proc_all);
                    
                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritMeasureObj)) //if (ReportID == 3) // по объектам оценки
                {
                    FillCommonMeasure(dc, ScaleList);
                    gridMeasure.DataSource = tblMeasure_1;
                    gridMeasure.DataBind();
                }
                else
                {
                    lblMeasureObj.Visible = false;
                    RemoveReportControl(ref_measure_obj);
                }

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagram))
                {
                    test_diagram td = dc.test_diagrams.Where(p => p.test_id == TestID).FirstOrDefault();
                    if (td != null)
                    {
                        if (IsSingleGroupRep)
                        {
                            if (IsHR)
                                pnlDiagramAVG.Controls.Add(TesterChart.DrawDiagram(dc, td, 600 /* (int)Chart1.Width.Value*/, GroupID, 1));
                            else
                                if (User.IsInRole("Department manager"))
                                {
                                    List<int> data = (
                                    from ts in dc.Test_Subjects
                                    join ua_ in dc.user_accounts on ts.idUser equals ua_.idUser
                                    where ts.group_id == GroupID && ua_.idDept == idDept
                                    select ts.id).ToList<int>();

                                    pnlDiagramAVG.Controls.Add(TesterChart.DrawDiagram(dc, td, 600, data));
                                }
                        }
                        else
                        {
                            List<int> SubjIDList = new List<int>();
                            foreach (DataRow dr in tblMainScale.Rows)
                            {
                                SubjIDList.Add((int)dr["id"]);
                            }
                            pnlDiagramAVG.Controls.Add(TesterChart.DrawDiagram(dc, td, 600, SubjIDList));
                        }

                        foreach (Test_Diagram_Scale tds in td.Test_Diagram_Scales) // вынести в общий код (повторяется в TestResult)
                        {
                            pnlDiagramAVG.Controls.Add(new LiteralControl(
                                string.Format("<br/>{0} - {1}",
                                new object[] { tds.Scale.abreviature, tds.Scale.name })));
                        }
                    }
                    //Chart1.Titles[0].Text = dc.Scales.Where(p => p.id == MainScaleID).FirstOrDefault().name;
                }
                else
                    RemoveReportControl(pnlDiagramAVG);

                if (!RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritRound))
                    RemoveReportControl (Chart1);
                else
                {
                    Chart1.Titles[0].Text = dc.Scales.Where(p => p.id == MainScaleID).FirstOrDefault().name;
                    Chart1.DataSourceID = "SqlDiagram";
                    Chart1.DataBind();
                }

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritIndicator))
                    RefreshGroupCircle(dc);
                else
                    RemoveReportControl(divIndicator);

                //SqlDiagram.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();

                #region Answer Frequency

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritFreq))
                {
                    // таблица по частоте выбора
                    //sqlFreq.SelectCommand = qryFreq_single;
                    //sqlFreq.SelectParameters["idGroup"].DefaultValue = GroupID.ToString();
                    DataView vFreq = (DataView)sqlFreq.Select(new DataSourceSelectArguments());
                    DataTable tblFreqRaw = vFreq.ToTable();
                    DataTable tblFreq = new DataTable();
                    tblFreq.Columns.Add(new DataColumn("text"));
                    DataRow rowFreq = null;
                    int CurrItemID = 0; //int PriorItemID = 0;

                    List<SubScale> SubScaleList = dc.Test_Questions.Where(p => p.test_id == TestID).FirstOrDefault().items.FirstOrDefault().SubScaleDimension.SubScales.ToList();
                    int AnsVarCount = SubScaleList.Count();

                  
                    //string legend = "<b>Легенда:</b><br/>";
                    //foreach (SubScale ss in SubScaleList)
                    //{
                    //    legend += string.Format("{0} - {1}<br/>", ss.OrderNumber, ss.name);
                    //}
                    //lblLegend.Text = legend;

                    // при условии что ответы нумеруются именно 1,2,3,4,...  Кто это гарантирует ?
                    //for (int i = 1; i <= AnsVarCount; i++)
                    //{
                    if (AnsVarCount == 0)
                        throw new Exception("Нулевое кол-во вариантов ответов. Определить частоту выбора невозможно");
                    else
                    {
                        foreach (SubScale ss in SubScaleList)
                        {
                            tblFreq.Columns.Add(new DataColumn("col_" + ss.OrderNumber.ToString()) { DefaultValue = "0" });

                            ////SubScaleList.Where(w => w.OrderNumber == i).First().name

                            //BoundField col = new BoundField() { DataField = "col_" + ss.OrderNumber.ToString(), HeaderText = ss.OrderNumber.ToString() };
                            //col.HeaderStyle.Width = 50;
                            //col.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                            //GridviewFreq.Columns.Add(col);

                            GridViewDataColumn gcol = new GridViewDataColumn("col_" + ss.OrderNumber.ToString(), SubScaleList.Where(w => w.OrderNumber == ss.OrderNumber).First().name);
                            ASPxGridviewFreq.Columns.Add(gcol);
                        }

                        foreach (DataRow dr in tblFreqRaw.Rows)
                        {
                            if ((int)dr["id"] != CurrItemID)
                            {
                                rowFreq = tblFreq.Rows.Add();
                                rowFreq["text"] = dr["text"].ToString();
                                CurrItemID = (int)dr["id"];
                            }

                            rowFreq["col_" + dr["ordernumber"]] = string.Format("{0}", dr["freq"]);
                            //if (rowFreq["col_" + dr["ordernumber"]] == "")
                        }

                        lblGridViewFreq.Visible = true;

                        ASPxGridviewFreq.DataSource = tblFreq;
                        ASPxGridviewFreq.DataBind();
                        //GridviewFreq.DataSource = tblFreq;
                        //GridviewFreq.DataBind();
                    }
                }
                else
                    RemoveReportControl(ref_ans_freq);

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritMostPopular) && !IsSingleGroupRep)
                {
                    GridviewMostPopular.DataSource = sqlMostPopular;
                    GridviewMostPopular.DataBind();
                }

                #endregion Answer Frequency

                if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritRawSubjectData))
                {
                    DataView vRawData = (DataView)sqlRawData.Select(new DataSourceSelectArguments());
                    DataTable tblRawData = vRawData.ToTable();
                    //tblRawData.Columns.Add(new DataColumn("text"));

                    DataTable tblRawDataLandscape = new DataTable();
                    tblRawDataLandscape.Columns.Add(new DataColumn("Subj"));
                    tblRawDataLandscape.Columns.Add(new DataColumn("TestDate"));

                    BoundField GridField = new BoundField() { DataField = "Subj", ReadOnly = true, HeaderText = "Участник" };
                    GridField.ItemStyle.Wrap = true;
                    gridRawData.Columns.Add(GridField);
                    GridField = new BoundField() { DataField = "TestDate", ReadOnly = true, HeaderText = "Дата" };
                    gridRawData.Columns.Add(GridField);

                    int SubjCount = RepLinks[0].subject_group.Test_Subjects.Count(p => p.Test_Date != null);
                    int QuestionCount = tblRawData.Rows.Count / SubjCount;
                    for (int i = 1; i <= QuestionCount; i++)
                    {
                        tblRawDataLandscape.Columns.Add(new DataColumn("Col_" + i.ToString()) { Caption = "KKK"});
                    }
                    for (int i = 1; i <= SubjCount; i++)
                    {
                        DataRow row = tblRawDataLandscape.Rows.Add();
                        row["Subj"] = tblRawData.Rows[(i - 1) * QuestionCount]["FIO"];
                        row["TestDate"] = tblRawData.Rows[(i - 1) * QuestionCount][1];
                        for (int j = 1; j <= QuestionCount; j++)
                        {
                            row["Col_"+j.ToString()] = tblRawData.Rows[(i - 1) * QuestionCount +j-1]["text"];
                            if (i == 1)
                            {
                            //    tblRawDataLandscape.Columns[j + 1].Caption = tblRawData.Rows[(i - 1) * QuestionCount + j - 1]["header"].ToString();

                                BoundField fld = new BoundField() { 
                                    DataField = "col_" + j.ToString(),
                                    HeaderText = tblRawData.Rows[(i - 1) * QuestionCount + j - 1]["header"].ToString()
                                    };
                                fld.ItemStyle.Wrap = true;
                                fld.ItemStyle.Width = new Unit("25%");
                                gridRawData.Columns.Add(fld);
                            }
                        }
                    }
                    gridRawData.DataSource = tblRawDataLandscape;
                    //gridRawData.DataBound += gridRawData_DataBound;
                    //gridRawData.Load += gridRawData_Load;
                    gridRawData.DataBind();
                    
                }
            } // using dc
            ShowGroupInfo(RepCommonInfo);
        }
    }

    private void RemoveReportControl(Control p_Control)
    {
        Control ctrl = form1.FindControl(p_Control.ID);
        if (ctrl!= null)
            ctrl.Parent.Controls.Remove (p_Control);
    }

    private void InitSQL()
    {
        sqlMeasure.SelectCommand =
            "select s.id, s.name as scale_name, lnk.id as idLink, lnk.measureobjectname,  "+
            "         cast(round(avg(td.test_value),0) as int) avg_val "+
            " from test_subjectgroup_link lnk "+
            " inner join scales s on s.test_id= lnk.idtest "+
            " left join test_subject ts on ts.group_id = lnk.idgroup and ts.idTestLink = lnk.id "+
            " left join test_data td on td.scale_id = s.id and td.subject_id= ts.id "+
            " where lnk.idgroup=@GroupID "+
            " group by s.id, s.name, lnk.id, lnk.measureobjectname";
    }


    private void ShowGroupInfo(Dictionary<string, string> aGroupInfo)
    {
        lblProjName.Text = aGroupInfo["GroupName"];
        Label1.Text = aGroupInfo["CompanyName"];
        Label3.Text = aGroupInfo["SubjCount"];
        Label4.Text = aGroupInfo["TestName"];
        Label2.Text = aGroupInfo["TestPeriod"];
    }
   
    protected void Chart1_DataBound(object sender, EventArgs e)
    {
        TesterChart.SetSectorColor(Chart1.Series[0].Points);
    }

    void RefreshGroupCircle(TesterDataClassesDataContext dc)
    {
        if (IsSingleGroupRep)
        {
            CommonData.ScaleRateInfo sss = CommonData.GetGroupScaleRate(dc, GroupID, TestID, MainScaleID, 1, User);
            divCirc.InnerText = string.Format("{0}%", Convert.ToInt16(sss.perc /*avg_score*/).ToString());
            divCirc.Style["background"] = sss.bk_color;
            divLowUser.InnerText = string.Format("{0}% людей, проходивших этот тест, набрали такой же или более низкий балл по шкале '{1}'",
                Convert.ToInt16(sss.perc).ToString(), sss.ScaleName);
        }
        // else в чем смысл индикатора для зоны контроля ?
    }

    private void SetCommonParamVal(SqlDataSource p_datasource)
    {
        p_datasource.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
        p_datasource.SelectParameters["TestID"].DefaultValue = TestID.ToString();
        p_datasource.SelectParameters.Add("GroupID", GroupID.ToString());
        p_datasource.SelectParameters.Add("idDept", idDept.ToString());
        p_datasource.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
    }

    private void SetSQLCommand(TesterDataClassesDataContext dc, bool p_IsSingleGroupRep, List<rep_item_link> p_RepLinks)
    {
        if (p_IsSingleGroupRep)
        {
            SqlDataSource1.SelectCommand = qry1_single;
            SetCommonParamVal(SqlDataSource1);

            SqlDataSource2.SelectCommand = qry2_single;
            SqlDataSource2.SelectParameters["TestID"].DefaultValue = TestID.ToString();
            SqlDataSource2.SelectParameters.Add("GroupID", GroupID.ToString());
            SqlDataSource2.SelectParameters.Add("idDept", idDept.ToString());
            SqlDataSource2.SelectParameters.Add("HR", IsHR == true ? "1" : "0");

            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritByJob) ||
                RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagramByJob))
            {
                sqlDataByJob.SelectCommand = qryJob_single;
                //sqlDataByJob.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
                sqlDataByJob.SelectParameters.Add("GroupID", GroupID.ToString());
                sqlDataByJob.SelectParameters.Add("idDept", idDept.ToString());
                sqlDataByJob.SelectParameters.Add("HR", IsHR == true ? "1" : "0");

                sqlDataByJob.SelectParameters.Add("ScaleID", MainScaleID.ToString());
                sqlDataByJob.SelectParameters.Add("TestID", TestID.ToString());
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritByDept) ||
                RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagramByDept)
                )
            {
                SqlDataByDept.SelectCommand = qryDept_single;
                //SqlDataByDept.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
                SqlDataByDept.SelectParameters.Add("GroupID", GroupID.ToString());
                SqlDataByDept.SelectParameters.Add("idDept", idDept.ToString());
                SqlDataByDept.SelectParameters.Add("HR", IsHR == true ? "1" : "0");

                SqlDataByDept.SelectParameters.Add("ScaleID", MainScaleID.ToString());
                SqlDataByDept.SelectParameters.Add("TestID", TestID.ToString());
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritRound))
            {
                //SqlDiagram.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
                //SqlDiagram.SelectParameters["TestID"].DefaultValue = TestID.ToString();
                //SqlDiagram.SelectParameters.Add("GroupID", GroupID.ToString());
                //SqlDiagram.SelectParameters.Add("idDept", idDept.ToString());
                //SqlDiagram.SelectParameters.Add("HR", IsHR == true ? "1" : "0");

                SetCommonParamVal(SqlDiagram);
                SqlDiagram.SelectCommand = qryDiagram_single;
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritFreq))
            {// таблица по частоте выбора
                sqlFreq.SelectCommand = qryFreq_single;
                sqlFreq.SelectParameters.Add("idGroup", GroupID.ToString());
                sqlFreq.SelectParameters.Add("idDept", idDept.ToString());
                sqlFreq.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritRawSubjectData))
            {// первичные данные
                sqlRawData.SelectCommand = qryRawData_single;
                sqlRawData.SelectParameters.Add("GroupID", GroupID.ToString());
            }
        }
        else // composite
        {
            int TotalSubjCount = dc.Test_Subjects.Where(p => p.Test_Id == TestID).Count();
            SqlDataSource1.SelectCommand = qry1_composite;
            SqlDataSource1.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
            SqlDataSource1.SelectParameters["TestID"].DefaultValue = TestID.ToString();
            SqlDataSource1.SelectParameters.Add("idCompany", idCompany.ToString());
            SqlDataSource1.SelectParameters.Add(new Parameter ("TotalSubjCount", DbType.Decimal, TotalSubjCount.ToString()));
            SqlDataSource1.SelectParameters.Add("idDept", idDept.ToString());
            SqlDataSource1.SelectParameters.Add("HR", IsHR == true ? "1" : "0");

            SqlDataSource2.SelectCommand = qry2_composite;
            SqlDataSource2.SelectParameters["TestID"].DefaultValue = TestID.ToString();
            SqlDataSource2.SelectParameters.Add("idCompany", idCompany.ToString());
            SqlDataSource2.SelectParameters.Add(new Parameter("TotalSubjCount", DbType.Decimal, TotalSubjCount.ToString()));
            SqlDataSource2.SelectParameters.Add("idDept", idDept.ToString());
            SqlDataSource2.SelectParameters.Add("HR", IsHR == true ? "1" : "0");

            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritByJob) ||
                RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagramByJob))
            {
                sqlDataByJob.SelectCommand = qryJob_composite;
                //sqlDataByJob.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
                sqlDataByJob.SelectParameters.Add("ScaleID", MainScaleID.ToString());
                sqlDataByJob.SelectParameters.Add("TestID", TestID.ToString());
                sqlDataByJob.SelectParameters.Add("idCompany", idCompany.ToString());
                sqlDataByJob.SelectParameters.Add(new Parameter("TotalSubjCount", DbType.Decimal, TotalSubjCount.ToString()));
                sqlDataByJob.SelectParameters.Add("idDept", idDept.ToString());
                sqlDataByJob.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritByDept) ||
                RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritDiagramByDept))
            {
                SqlDataByDept.SelectCommand = qryDept_composite;
                //SqlDataByDept.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
                SqlDataByDept.SelectParameters.Add("ScaleID", MainScaleID.ToString());
                SqlDataByDept.SelectParameters.Add("TestID", TestID.ToString());
                SqlDataByDept.SelectParameters.Add("idCompany", idCompany.ToString());
                SqlDataByDept.SelectParameters.Add(new Parameter("TotalSubjCount", DbType.Decimal, TotalSubjCount.ToString()));
                SqlDataByDept.SelectParameters.Add("idDept", idDept.ToString());
                SqlDataByDept.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritRound))
            {
                SqlDiagram.SelectCommand = TesterChart.qryDiagram_composite;
                SqlDiagram.SelectParameters["ScaleID"].DefaultValue = MainScaleID.ToString();
                SqlDiagram.SelectParameters["TestID"].DefaultValue = TestID.ToString();
                SqlDiagram.SelectParameters.Add("idCompany", idCompany.ToString());
                SqlDiagram.SelectParameters.Add("idDept", idDept.ToString());
                SqlDiagram.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritFreq))
            {// таблица по частоте выбора
                //int CompositeGroupSubjCount = ResearchData.CompositeGroupSubjCount(dc, TestID, idCompany);
                sqlFreq.SelectCommand = qryFreq_composite;
                sqlFreq.SelectParameters.Add("idCompany", idCompany.ToString());
                sqlFreq.SelectParameters.Add("idTest", TestID.ToString());
                //sqlFreq.SelectParameters.Add("CompositeSubjCount", CompositeGroupSubjCount.ToString());
                sqlFreq.SelectParameters.Add("idDept", idDept.ToString());
                sqlFreq.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
            }
            if (RepLinks.Exists(p => p.idItemType == (short)RepItemType.ritMostPopular))
            {
                sqlMostPopular = new SqlDataSource(
                            System.Configuration.ConfigurationManager.ConnectionStrings["tester_dataConnectionString"].ConnectionString,
                            qryMostPopular_composite);
                //sqlMostPopular.SelectParameters.Add("CompositeSubjCount", CompositeGroupSubjCount.ToString());
                sqlMostPopular.SelectParameters.Add("idCompany", idCompany.ToString());
                sqlMostPopular.SelectParameters.Add("idTest", TestID.ToString());
                sqlMostPopular.SelectParameters.Add("idDept", idDept.ToString());
                sqlMostPopular.SelectParameters.Add("HR", IsHR == true ? "1" : "0");
            }
        }
    }

    void FillCommonData(TesterDataClassesDataContext dc, List<Scale> pScales)
    {
        DataView view = (DataView)SqlDataSource1.Select(new DataSourceSelectArguments());
        tblMainScale = view.ToTable(); // это общая таблица. из нее формируем все остальные таблицы (ХМ, проблемы с сортировкой тогда)

        if (tblMainScale.Rows.Count > 0)
        {
            //SqlDataSource2.SelectParameters["TestID"].DefaultValue = TestID.ToString();
            DataView view_common = (DataView)SqlDataSource2.Select(new DataSourceSelectArguments());
            tblCommon = view_common.ToTable();

            tblPersonalMainScore.Columns.Add(new DataColumn("FIO"));
            tbl7.Columns.Add(new DataColumn("FIO"));

            tblSummary.Columns.Add(new DataColumn("Параметр"));
            tblSummary.Rows.Add()["Параметр"] = "Средний бал";
            tblSummary.Rows.Add()["Параметр"] = "Минимальное значение";
            tblSummary.Rows.Add()["Параметр"] = "Максимальное значение";
            tblSummary.Rows.Add()["Параметр"] = "Средний процентиль";

            foreach (Scale s in pScales)
            {
                tblSummary.Columns.Add(new DataColumn(s.name));

                string[] nw = s.name.Split (' ');
                int maxlen = 0;
                foreach (string z in nw)
                {
                    if (z.Length > maxlen) maxlen = z.Length;
                }

                string ShortName = s.name;
                if (s.abreviature != "" && maxlen > 12)
                    ShortName = s.abreviature;

                tblPersonalMainScore.Columns.Add(new DataColumn(s.name));
                BoundField GridField = new BoundField() { DataField = s.name, ReadOnly = true, HeaderText = ShortName};
                GridField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                GridField.DataFormatString = "{0:p0}";
                GridField.ItemStyle.Width = 75;
                GridField.ItemStyle.CssClass = "clsVertBT";
                //GridField.ItemStyle.Font.
                Gridview6.Columns.Add(GridField);

                tbl7.Columns.Add(new DataColumn(s.name));
                GridField = new BoundField() { DataField = s.name, ReadOnly = true, HeaderText = ShortName };
                GridField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                GridField.ItemStyle.Width = 75;
                GridField.DataFormatString = "{0:p0}";
                Gridview7.Columns.Add(GridField);

                tblSummary.Rows[0][s.name] = 0;
                tblSummary.Rows[1][s.name] = int.MaxValue;
                tblSummary.Rows[2][s.name] = int.MinValue;
                tblSummary.Rows[3][s.name] = 0;
            }

            Gridview6.Columns[1].ItemStyle.HorizontalAlign = HorizontalAlign.Center;

            int idSubj = 0;
            int DataColNumber = 1;
            int SubjCount = tblMainScale.Rows.Count; int SubjNumber = 0;
            DataRow tbl6_row = null;
            DataRow tbl7_row = null;
            //decimal AVGScore=0; int MinScore=int.MaxValue; int MaxScore=int.MinValue; decimal AVGPerc=0; // а отрицательные ?
            foreach (DataRow rr in tblCommon.Rows)
            {
                if (idSubj != (int)rr["id"])
                {
                    idSubj = (int)rr["id"];

                    tbl6_row = tblPersonalMainScore.Rows.Add();
                    tbl7_row = tbl7.Rows.Add();

                    tbl6_row["fio"] = rr["fio"].ToString();
                    tbl7_row["fio"] = rr["fio"].ToString();
                    DataColNumber = 1;
                    SubjNumber++;
                }
                tbl6_row[DataColNumber] = rr["test_value"];
                tbl7_row[DataColNumber] = rr["pp"];

                tblSummary.Rows[0][DataColNumber] = Convert.ToInt32(tblSummary.Rows[0][DataColNumber]) + Convert.ToInt32(rr["test_value"]);
                tblSummary.Rows[3][DataColNumber] = Convert.ToInt32(tblSummary.Rows[3][DataColNumber]) + Convert.ToInt32(rr["pp"]);
                if (Convert.ToInt32(tblSummary.Rows[1][DataColNumber]) > (int)rr["test_value"])
                    tblSummary.Rows[1][DataColNumber] = (int)rr["test_value"];
                if (Convert.ToInt32(tblSummary.Rows[2][DataColNumber]) < (int)rr["test_value"])
                    tblSummary.Rows[2][DataColNumber] = (int)rr["test_value"];

                DataColNumber++;
            }

            foreach (Scale s in pScales)
            {
                tblSummary.Rows[0][s.name] = Math.Round(Convert.ToInt32(tblSummary.Rows[0][s.name]) / (decimal)SubjCount);
                tblSummary.Rows[3][s.name] = Math.Round(Convert.ToInt32(tblSummary.Rows[3][s.name]) / (decimal)SubjCount);
            }
        }

        //------------------------
        //foreach (DataRow dd in tbl6.Select("scale_id=1"))
        //{ 

        //}
        //if (MinScore > (int)rr["test_value"]) MinScore = (int)rr["test_value"];
        //if (MaxScore < (int)rr["test_value"]) MaxScore = (int)rr["test_value"];
        //if (DataColNumber == ScaleList.Count)
        //{
        //    tblSummary.Rows[1][DataColNumber] = MinScore;
        //    tblSummary.Rows[2][DataColNumber] = MinScore;
        //}
        //------------------------
    }

    /// <summary>
    /// по объектам оценки
    /// </summary>
    void FillCommonMeasure(TesterDataClassesDataContext dc, List<Scale> pScales)
    { 
        tblMeasure_1.Columns.Add(new DataColumn("Шкала"));
        foreach (Scale s in pScales)
        {
            tblMeasure_1.Rows.Add()[0] = s.name;
        }
		DataView view_measure = (DataView)sqlMeasure.Select(new DataSourceSelectArguments());
        tblMeasureCommon = view_measure.ToTable();
        int idLink = 0;
        int DataRowNumber = 0;
        int DataColNumber = 0;
        foreach (DataRow rr in tblMeasureCommon.Rows)
		{
			if (idLink != (int)rr["idLink"])
            {
                idLink = (int)rr["idLink"];
                tblMeasure_1.Columns.Add(new DataColumn(rr["measureobjectname"].ToString()));
                DataRowNumber = 0;
                DataColNumber++;
            }
            tblMeasure_1.Rows[DataRowNumber][DataColNumber] = rr["avg_val"];
            DataRowNumber++;
		}
    }

    protected void Gridview1_Sorting(object sender, GridViewSortEventArgs e)
    {
        //object aa = (sender as GridView).DataSourceObject.GetViewNames();

        //Gridview1.Sort("fio",SortDirection.Descending);
        //if (view != null)
        //{
        //    view.Sort = "rank";
        //}
    }
    
    static ColorConverter cnv = new ColorConverter();

    protected void Gridview1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Header && e.Row.DataItem != null)
        {
                if (((DataRowView)(e.Row.DataItem)).Row["test_value"] != System.DBNull.Value)
                {
                    int tv = Convert.ToInt16(((DataRowView)(e.Row.DataItem)).Row["test_value"]);
                    Scale_Norm sn = SNList.Where(p => p.LowRange <= tv && p.HighRange >= tv).FirstOrDefault();

                    if (sn != null)
                    {
                        string bkClr = sn.Norm_Type.bk_color;
                        e.Row.BackColor = (Color)cnv.ConvertFromString("#" + bkClr);
                    }
                }
        }
    }
    protected void gridByJob_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Header && e.Row.DataItem != null)
        {
            if (((DataRowView)(e.Row.DataItem)).Row["avg_score"] != System.DBNull.Value)
            {
                int tv = Convert.ToInt16(((DataRowView)(e.Row.DataItem)).Row["avg_score"]);
                Scale_Norm sn = SNList.Where(p => p.LowRange <= tv && p.HighRange >= tv).FirstOrDefault();
                if (sn != null)
                {
                    string bkClr = sn.Norm_Type.bk_color;
                    e.Row.BackColor = (Color)cnv.ConvertFromString("#" + bkClr);
                }
            }
        }
    }
    protected void gridView6_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Header && e.Row.DataItem != null)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                int tv = 0;
                if (int.TryParse(e.Row.Cells[i].Text, out tv))
                {
                    Scale scl = ScaleList.Where(
                        p => p.name == ((GridView)sender).HeaderRow.Cells[i].Text || p.abreviature == ((GridView)sender).HeaderRow.Cells[i].Text
                        ).FirstOrDefault();

                    if (scl != null)
                    {
                        Scale_Norm sn = scl.Scale_Norms.Where(p => p.LowRange <= tv && p.HighRange >= tv).FirstOrDefault();
                        if (sn != null)
                        {
                            string bkClr = sn.Norm_Type.bk_color;
                            e.Row.Cells[i].BackColor = (Color)cnv.ConvertFromString("#" + bkClr);
                        }
                    }
                }
            }
        }
    }

    protected void Gridview7_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Header && e.Row.DataItem != null)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                int tv = 0;
                if (int.TryParse(e.Row.Cells[i].Text, out tv))
                {
                    string bkClr = "#";
                    if (tv <= 20) bkClr += NormColors.Where(p => p.id == 1).FirstOrDefault().bk_color;
                    else if (tv <= 40) bkClr += NormColors.Where(p => p.id == 2).FirstOrDefault().bk_color;
                    else if (tv <= 60) bkClr += NormColors.Where(p => p.id == 3).FirstOrDefault().bk_color;
                    else if (tv <= 80) bkClr += NormColors.Where(p => p.id == 4).FirstOrDefault().bk_color;
                    else bkClr += NormColors.Where(p => p.id == 2).FirstOrDefault().bk_color;

                    e.Row.Cells[i].BackColor = (Color)cnv.ConvertFromString(bkClr);
                }
            }
        }
    }
    protected void GridviewFreq_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Header && e.Row.DataItem != null)
        {
            // считаем сумму по строке
            int rowSum = 0;
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                int tv = 0;
                if (int.TryParse(e.Row.Cells[i].Text.Replace(" %", ""), out tv))
                {
                    rowSum += tv;    
                }
            }
            
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                int tv = 0;
                if (int.TryParse(e.Row.Cells[i].Text.Replace(" %",""), out tv))
                {
                    int val = (int)Math.Round (100.0 * tv / rowSum, 0);
                    e.Row.Cells[i].Text = string.Format ("{0} %", val);
                    if (val > 25)
                    {
                        e.Row.Cells[i].BackColor = Color.LightGreen;
                    }
                }
            }
        }
    }
}