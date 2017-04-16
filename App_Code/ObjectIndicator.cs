using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;

/// <summary>
/// Summary description for ObjectIndicator
/// </summary>
public class ObjectIndicator
{
	public ObjectIndicator() {}

    struct ps1 { 
        public string item_name; 
        public string txt_answer; 
        public short? OrderNumber; 
        public byte DimensionType;
        public int ID;
    }
    struct dp {
        public int OrdNumber;
        public string name;
        public int ssID;
        public int? subjID;
        public int? LeaderCode;
        public decimal? CompletePercent;
    }
    struct dpSummary
    {
        public int OrdNumber;
        public string name;
        public int ssID;
        public int? Number;
        public int? LeaderCode;
        public string LeaderName;
        public decimal? PercentIPR;
    }
    struct InfoByDept
    {
        public int? idDept;
        public int Number;
    }
    class dept_value_info 
    {
        public int idDept;
        public string name;
        public int number;
    }
    struct InfoByUserName
    {
        public string Name;
        //public int id;
        public int Number;
    }
    struct FieldDataSet
    {
        public decimal Value;
        public string Name;
        public int Order;
    }
    //struct DevCountByDate
    //{
    //    public DateTime mDate;
    //    public int DevCount;
    //}

    /// <summary>
    /// </summary>
    /// <param name="p_isSubLevel">для корректного определения относительного пути к папке с картинками</param>
    public static void BuildAndPut(TesterDataClassesDataContext p_dc, indicator p_indicator, user_account p_ua, 
        Control p_container, string p_ImageFolderPath, bool p_isSubLevel = false)
    {
        switch (p_indicator.idType) 
        {

            case 140: // integral
                try
                {
                    subject_group sg = p_indicator.subject_group;
                    Test_Subject subj = sg.Test_Subjects.Where(p => p.idUser == p_ua.idUser).OrderByDescending(q => q.Test_Date).FirstOrDefault();

                    if (subj != null)
                    {
                        p_container.Controls.Add(new LiteralControl("<hr/>"));
                        p_container.Controls.Add(new Label() { Text = p_indicator.name, CssClass = "clsIndicatorName" });
                        p_container.Controls.Add(new LiteralControl("<br/>"));

                        foreach (Scale scl in sg.Test_SubjectGroup_Links[0].test.Scales.OrderByDescending(o => o.isMain))
                        {
                            if (scl.isMain)
                            {
                                CommonData.ScaleRateInfo info = CommonData.GetSubjectScaleRate(p_dc, subj.id, scl.id);
                                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                                Label valLabel = TesterChart.DrawScaleIndicator(info, p_ImageFolderPath, img);
                                valLabel.Text = scl.name + " - " + valLabel.Text;
                                valLabel.CssClass = "testerChartLabel";

                                if (p_isSubLevel) img.ImageUrl = "..\\" + img.ImageUrl;
                                p_container.Controls.Add(valLabel);
                                //p_container.Controls.Add(img);
                                p_container.Controls.Add(new LiteralControl("<br/>"));
                            }
                            else
                            {
                                Test_Data td = subj.Test_Datas.Where(p => p.Scale_ID == scl.id).FirstOrDefault();
                                p_container.Controls.Add(new Label()
                                    {
                                        Text = string.Format("{0} - {1}",
                                           scl.name,
                                           td==null ? "нет данных" : Convert.ToInt32(td.Test_Value).ToString()),
                                        CssClass = "clsMiscLines"
                                    });
                                p_container.Controls.Add(new LiteralControl("<br/>"));
                            }
                        }
                        p_container.Controls.Add(new LiteralControl("<hr/>"));
                    }
                }
                catch
                {
                    throw new Exception("проблемы при отрисовке интегрального индикатора");
                }
                break;
        }
    }

    public static void BuildAndPut(TesterDataClassesDataContext p_dc, indicator p_indicator, user_account p_ua,
        Control p_container)
    {
        BuildAndPut(p_dc, p_indicator, p_ua, null, p_container);
    }
    /// <summary>
    /// построение и размещение индикатора без картинки
    /// </summary>
    public static void BuildAndPut(TesterDataClassesDataContext p_dc, indicator p_indicator, user_account p_ua, 
        bool? p_canViewAllDept,
        Control p_container)
    {
        try
        {
            int QuartNumber = (DateTime.Today.Month +2) / 3;
            DateTime QuartBegin = new DateTime(DateTime.Today.Year, (QuartNumber-1) * 3 + 1, 1);
            DateTime QuartEnd = QuartBegin.AddMonths(3);

            switch (p_indicator.idType)
            {
                case 10: // индикатор по основной шкале
                    if (p_indicator.Scale != null)
                    {
                        Label valLabel = TesterChart.CalcAndDrawIndicator(p_dc, null, null, p_indicator, p_ua.idUser, null);
                        valLabel.CssClass = "testerChartLabel";
                        p_container.Controls.Add(new Label() { Text = p_indicator.name, Width = 165, CssClass = "clsIndexDescr" });
                        p_container.Controls.Add(valLabel);
                        //p_container.Controls.Add(new LiteralControl("<br/>"));
                    }
                    break;
                case 130: // идеи
                    //p_container.Controls.Add(new LiteralControl("<hr/>"));
                    p_container.Controls.Add(new Label() { 
                        Text = string.Format("{0} за {1} квартал {2} года", p_indicator.name, QuartNumber, DateTime.Today.Year), 
                        CssClass = "clsIndicatorName" });

                    Dictionary<string, int> IdeaByState = new Dictionary<string, int>();
                    int TotalIdeaCount = 0;

                    foreach (string StateName in (
                        from ii in p_dc.ideas
                        join ts in p_dc.Test_Subjects on ii.idSubject equals ts.id
                        join ist in p_dc.idea_states on ii.idState equals ist.idState
                        join ua in p_dc.user_accounts on ts.idUser equals ua.idUser
                        where (ts.idUser == p_ua.idUser || p_indicator.isPersonal == false)
                            && ua.idCompany == p_ua.idCompany
                            && ii.idState != 8 // "не оценивается"
                            && ts.Test_Date >= QuartBegin && ts.Test_Date < QuartEnd
                        select ist.name))
                    {
                        TotalIdeaCount++;
                        if (IdeaByState.ContainsKey(StateName))
                            IdeaByState[StateName]++;
                        else
                            IdeaByState.Add(StateName, 1);
                    }
                    p_container.Controls.Add(new LiteralControl("<p>"));
                    p_container.Controls.Add(new LiteralControl(string.Format ("Всего идей за {0} квартал {1} г.: ", QuartNumber, DateTime.Today.Year)));

                    Label lbl = new Label() { Text = TotalIdeaCount.ToString() + "<br/>", CssClass = "AppreciateLink"};
                    lbl.Font.Size = 34; // hard
                    p_container.Controls.Add(lbl);

                    foreach (string key in IdeaByState.Keys)
                    {
                        p_container.Controls.Add(new LiteralControl(string.Format("{0}: {1}<br/>", key, IdeaByState[key])));
                    }

                    if (p_indicator.isPersonal)
                    {// персональный

                        foreach (Test_Subject tsubj in (
                            from ig in p_dc.idea_generators
                            join ts in p_dc.Test_Subjects on ig.idTest equals ts.Test_Id
                            join sg in p_dc.subject_groups on ts.group_id equals sg.id
                            where ts.idUser == p_ua.idUser && ts.Test_Date == null
                                && (sg.stop_date == null || sg.stop_date > DateTime.Today)
                                && sg.start_date <= DateTime.Today
                            select ts))
                        {
                            p_container.Controls.Add(new LiteralControl("<br/>"));
                            p_container.Controls.Add(new HyperLink()
                            {
                                NavigateUrl = string.Format("~\\Player\\testtrack.aspx?s={0}", tsubj.id),
                                Text = tsubj.subject_group.name
                            });
                        }
                    }
                    else
                    {// в разрезе отделов
                        foreach (InfoByDept ibd in (
                            from ii in p_dc.ideas
                            join ts in p_dc.Test_Subjects on ii.idSubject equals ts.id
                            join ua in p_dc.user_accounts on ts.idUser equals ua.idUser
                            where (ts.idUser == p_ua.idUser || p_indicator.isPersonal == false)
                                && ua.idCompany == p_ua.idCompany
                                && ts.Test_Date >= QuartBegin
                            group ua by ua.idDept
                                into grp
                                select new InfoByDept
                                {
                                    idDept = grp.Key,
                                    Number = grp.Select(x => x.idUser).Count()
                                }))
                        {
                            dept one_dept = p_dc.depts.Where(d => d.id == ibd.idDept).FirstOrDefault();
                            int userNumber = one_dept.user_accounts.Where(ua => ua.idState != 1 && ua.idState != 3 && ua.idState != 4).Count();

                            if (userNumber > 0)
                            {
                                p_container.Controls.Add(new LiteralControl(string.Format("<br/>{0}: {1} идей ({2}% сотрудников)",
                                    one_dept.name, ibd.Number, 100 * ibd.Number / userNumber)));
                            }
                        }
                    }
                    p_container.Controls.Add(new LiteralControl("</p>"));
                    p_container.Controls.Add(new HyperLink() { NavigateUrl = "~\\Analyse\\IdeaList.aspx", Text = "База идей" });

                    break;
                case 160:  // план развития
                    DevelopmentPlan(p_dc, p_indicator, p_container, p_ua);
                    break;
                case 170: // благодарности
                    if (p_indicator.isPersonal)
                    {
                        int cntActive = (
                            from ts in p_dc.Test_Subjects
                            where ts.group_id == 1111 && ts.Test_Date != null && ts.idUser == p_ua.idUser
                            select ts.id).Count();
                        //user_account ua = p_dc.user_accounts.Where (
                        //    p=> p.idUser == p_idUser
                        //    ).FirstOrDefault();

                        int cntPassive = (
                            from lnk in p_dc.Test_SubjectGroup_Links
                            join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
                            join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
                            join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
                            where lnk.idGroup == 1111 && tr.SelectedValue == 1 && ss.name == p_ua.fio
                            select tr.id).Count();

                        p_container.Controls.Add(new LiteralControl("<br/>"));
                        p_container.Controls.Add(new Label() { Text = "Вы поблагодарили:"});
                        //Label vlbl = TesterChart.CalcAndDrawIndicator(p_dc, null, null, p_indicator, p_ua.idUser, p_canViewAllDept);
                        HyperLink hlnk = new HyperLink()
                        {
                            Text = cntActive.ToString(),
                            NavigateUrl = string.Format("~\\Analyse\\gratitude.aspx?s={0}", p_ua.fio),
                            CssClass = "AppreciateLink"
                        };
                        hlnk.Font.Size = 34; //hard
                        p_container.Controls.Add(hlnk);
                        p_container.Controls.Add(new Label() { CssClass = "clsIndexDescr" });
                        p_container.Controls.Add(new Label() { Text = "Вас поблагодарили:" });
                        hlnk = new HyperLink()
                        {
                            Text = cntPassive.ToString(),
                            NavigateUrl = string.Format("~\\Analyse\\gratitude.aspx?o={0}", p_ua.fio),
                            CssClass = "AppreciateLink"
                        };
                        hlnk.Font.Size = 34;
                        p_container.Controls.Add(hlnk);

                        p_container.Controls.Add(new Label() { CssClass = "clsIndexDescr" });
                        p_container.Controls.Add(new HyperLink() { Text = "База благодарностей", NavigateUrl = "~\\Analyse\\gratitude.aspx" });
                        p_container.Controls.Add(new Label() { CssClass = "clsIndexDescr" });
                    }
                    else // групповой индикатор
                    {
                        p_container.Controls.Add(new Label() {
                            Text = string.Format("{0} за {1} квартал {2} года", p_indicator.name, QuartNumber, DateTime.Today.Year), 
                            CssClass = "clsIndicatorName" });

                        int QuartGratitudeNumber = (
                            from lnk in p_dc.Test_SubjectGroup_Links
                            join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
                            join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
                            join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
                            join ts in p_dc.Test_Subjects on tr.Subject_ID equals ts.id
                            where lnk.idGroup == 1111 && tr.SelectedValue == 1
                                && ts.Test_Date >= QuartBegin
                            select 1).Count();
                        p_container.Controls.Add(new LiteralControl(string.Format("<p>Всего благодарностей за квартал {0}<br/>", QuartGratitudeNumber)));

                        p_container.Controls.Add(new LiteralControl("Больше всех благодарят сотрудников:"));
                        foreach (InfoByUserName aa in (
                            from lnk in p_dc.Test_SubjectGroup_Links
                            join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
                            join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
                            join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
                            join ts in p_dc.Test_Subjects on tr.Subject_ID equals ts.id
                            where lnk.idGroup == 1111 && tr.SelectedValue == 1
                                && ts.Test_Date >= QuartBegin // начало квартала
                            group ss by ss.name into g
                            select new InfoByUserName { Name = g.Key, Number = g.Count() }
                            ).OrderByDescending(oo => oo.Number).Take(5))
                        {
                            p_container.Controls.Add(new LiteralControl(string.Format("<br/>-{0} ({1})", aa.Name, aa.Number)));
                        }
                        p_container.Controls.Add(new LiteralControl("</p>"));
                        p_container.Controls.Add(new HyperLink() { Text = "База благодарностей", NavigateUrl = "~\\Analyse\\gratitude.aspx" });
                    }
                    break;
                case 180: // квартальная оценка 
                    QuartAssessment(p_dc, p_indicator, p_container, QuartBegin, p_ua);
                    break;
                case 190: // риски
                    int RiskCount = (
                    from ts in p_dc.Test_Subjects
                    where ts.group_id == 2126 && ts.Test_Date >= QuartBegin
                    select ts.id).Count();

                    p_container.Controls.Add(new Label() {
                        Text = string.Format("{0} за {1} квартал {2} года", p_indicator.name, QuartNumber, DateTime.Today.Year), 
                        CssClass = "clsIndicatorName" });
                    p_container.Controls.Add(new LiteralControl("<p>"));
                    p_container.Controls.Add(new LiteralControl("Рисков и происшествий за квартал: " + RiskCount.ToString()));
                    p_container.Controls.Add(new LiteralControl("</p>"));
                    p_container.Controls.Add(new HyperLink() { Text = "База рисков", NavigateUrl = "~\\Analyse\\risk.aspx" });
                    
                    break;
                case 200: // отклонения
                    MetricDeviation (p_dc, p_indicator, p_container);
                    break;
                case 80: // счетчик прохождений
                    if (p_indicator.idGroup != null)
                    {
                        Label valLabel = TesterChart.CalcAndDrawIndicator(p_dc, null, null, p_indicator, p_ua.idUser, p_canViewAllDept);
                        valLabel.CssClass = "testerChartLabel";
                        //if (img.ImageUrl != "")
                        //{
                        //img.ImageUrl = "..\\" + img.ImageUrl;
                        p_container.Controls.Add(new Label() { Text = p_indicator.name, Width = 165, CssClass = "clsIndexDescr" });
                        //tc.Controls.Add(img);
                        p_container.Controls.Add(valLabel);
                        //p_container.Controls.Add(new LiteralControl("<br/>"));

                        //}

                    }
                    break;
                case 100: // счетчик упоминаний
                    if (p_indicator.idGroup != null)
                    {
                        Label valLabel = TesterChart.CalcAndDrawIndicator(p_dc, null, null, p_indicator, p_ua.idUser, p_canViewAllDept);
                        valLabel.CssClass = "testerChartLabel";



                        p_container.Controls.Add(new Label() { Text = p_indicator.name, Width = 165, CssClass = "clsIndexDescr" });
                        p_container.Controls.Add(valLabel);

                        string LastAns = CommonData.LastTextAnswer(p_dc, (int)p_indicator.idGroup, p_ua.fio);
                        p_container.Controls.Add(new Label() { Text = LastAns, Width = 150, CssClass = "clsIndexDescr" });

                    }
                    break;
            }
        }
        catch (Exception exc)
        {
            // write to log please
        }
    }


    
    
    /// <summary>
    /// отклонения по метрикам (в разрезе отделов)
    /// </summary>
    private static void MetricDeviation(TesterDataClassesDataContext p_dc, indicator p_indicator, Control p_container)
    {
        p_container.Controls.Add(new Label() { Text = p_indicator.name, CssClass = "clsIndicatorName" });
        p_container.Controls.Add(new LiteralControl("<br/>"));

        Panel ComboPanel = new Panel();
        //ComboPanel.Style.Add("display", "inline-block");
        p_container.Controls.Add(ComboPanel);
        //System.Web.UI.HtmlControls.HtmlGenericControl dv = new System.Web.UI.HtmlControls.HtmlGenericControl();
        
        Panel pnlList = new Panel();
        pnlList.Style.Add("display", "inline-block");
        pnlList.Style.Add("vertical-align", "top");
        int TotalDeviation = 0;
        foreach (dept_value_info dvi in p_dc.ExecuteQuery<dept_value_info>(
                    //"declare @dt3m datetime set @dt3m=dateadd(mm,-3,getdate()) " +
                    //"select iddept, name, sum(number) as number from ( " + // суммарный
                    //"select dept.id as iddept, dept.name, count (*) as number " +
                    //"from metric m " +
                    //"join Test_Data td on m.idScale = td.Scale_ID " +
                    //"join test_subject ts on ts.id = td.subject_id " +
                    //"join user_account ua on ua.iduser = ts.iduser " +
                    //"left join dept on dept.id = ua.iddept " +
                    //"where m.idCompany = {0} and td.Test_Value < m.index_value and m.condition = '<' " +
                    //" and ts.actual = 1 " +
                    //" and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null) " +
                    //" and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null) " +
                    //"group by dept.id, dept.name " +

                    //"UNION ALL " + // not exists
                    //"select q.iddept, dept.name, sum(emp_count) as emp_count " +
                    //"from ( " +
                    //"SELECT m.idMetric as id, ua.iddept, count (*) emp_count " +
                    //"FROM metric m  " +
                    //"inner JOIN Test_Subject ts ON ts.test_id = m.idtest  " +
                    //"inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = m.idcompany " +
                    //"WHERE m.idcompany = {1} and ts.test_date >= @dt3M "+
                    //"and m.condition = 'NE'  and m.index_value = 1 " +
                    //"and ua.idjob in (select idjob from metric_subj_filter where idmetric = m.idmetric and idjob is not null)   " +
                    //"and ua.idstate in (select idstate from metric_subj_filter where idmetric = m.idmetric and idstate is not null)  " +
                    //"GROUP BY m.idMetric, ts.iduser, ua.iddept " + // группировку по отделу выше на уровень, если будет искажать картину 
                    //"having count (ts.test_date) = 0) q " +

                    //"left join dept on dept.id = q.iddept " +
                    //"group by q.iddept, dept.name) qq "+
                    //"group by qq.name, qq.iddept",
                    
                    "set dateformat 'dmy' " +
                    "select md.iddept, dept.name, count (*) as number "+
                    "from dbo.MetricDeviation ({0}, null) md " +
                    "left join dept on dept.id = md.idDept "+
                    "group by md.iddept, dept.name",

                    new object[] { p_indicator.idCompany }))
        {
            if (dvi.number > 0)
            {
                pnlList.Controls.Add(new LiteralControl(string.Format("<br/>{0}: ", dvi.name)));
                pnlList.Controls.Add(new HyperLink() { Text = dvi.number.ToString() + " отклонений", NavigateUrl = "~\\metric\\devbydept.aspx?id=" + dvi.idDept.ToString() });
                TotalDeviation += dvi.number;
            }
        }
        if (TotalDeviation != 0)
        {
            pnlList.Controls.AddAt(0, new LiteralControl(string.Format("<br/><br/>Всего {0} отклонений<br/><br/>", TotalDeviation)));
        }
        ComboPanel.Controls.Add(pnlList);

        Panel pnlDiagram = new Panel();
        pnlDiagram.Style.Add ("display", "inline-block");
        DrawMetricDevHistory(p_dc, pnlDiagram, p_indicator);
        ComboPanel.Controls.Add(pnlDiagram);

    }

    private static void DrawMetricDevHistory(TesterDataClassesDataContext p_dc, Control p_container, indicator p_indicator)
    {

        Chart dgr = new Chart() { Height = 200, BackColor = Color.Transparent};
        Series sr = new Series();
        foreach (var dcnt in (
            from mh in p_dc.metric_hists
            join d in p_dc.depts on mh.idDept equals d.id
            where d.idCompany == p_indicator.idCompany
            group mh by mh.mDate into g
            select new {mDate = g.Key, mNumber = g.Sum(x=>x.mNumber)}).OrderBy(ob=> ob.mDate).ToList()
            )
        {
            sr.Points.Add(new DataPoint(dcnt.mDate.ToOADate(), dcnt.mNumber) { 
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 15
            });
        }

        sr.ChartType = SeriesChartType.FastLine;
        sr.Color = Color.Red;
        sr.BorderWidth = 3;

        dgr.Series.Add(sr);
        dgr.ChartAreas.Add(new ChartArea("chart area")
        {
            BackColor = System.Drawing.Color.Transparent
        });
        dgr.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
        dgr.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
        

        p_container.Controls.Add(dgr);

    }
    /// <summary>
    /// план развития
    /// </summary>
    private static void DevelopmentPlan (TesterDataClassesDataContext p_dc, indicator p_indicator, Control p_container, user_account p_ua)
    {
        if (p_indicator.isPersonal)
        {// индивидуальный
            Test_Subject tsubj = p_dc.Test_Subjects.Where(
                    ts => ts.idUser == p_ua.idUser && ts.group_id == p_indicator.idGroup && ts.Test_Date != null
                ).OrderByDescending(o => o.Test_Date).FirstOrDefault();
            if (tsubj != null)
            {
                p_container.Controls.Add(new Label() { Text = p_indicator.name, CssClass = "clsIndicatorName" });
                p_container.Controls.Add(new HyperLink() { Text = "Редактировать", NavigateUrl = "~\\Group\\AnsAll.aspx?id=" + tsubj.id.ToString() });

                string priorItemName = "";
                foreach (ps1 line in (
                    from txt in p_dc.Test_Results_Txts
                    join i in p_dc.items on txt.item_id equals i.id
                    join ssd in p_dc.SubScaleDimensions on i.dimension_id equals ssd.id
                    where txt.subject_id == tsubj.id
                    select new ps1()
                    {
                        item_name = i.text,
                        txt_answer = txt.text,
                        OrderNumber = i.number,
                        DimensionType = ssd.dimension_type,
                        ID = txt.id
                    }
                    ).Union(
                    from rs in p_dc.Test_Results
                    join ss in p_dc.SubScales on rs.SubScale_ID equals ss.id
                    join i in p_dc.items on rs.item_id equals i.id
                    join ssd in p_dc.SubScaleDimensions on i.dimension_id equals ssd.id
                    where rs.Subject_ID == tsubj.id && rs.SelectedValue == 1
                    select new ps1()
                    {
                        item_name = i.text,
                        txt_answer = ss.name,
                        OrderNumber = i.number,
                        DimensionType = ssd.dimension_type,
                        ID = rs.id
                    }
                    ).OrderBy(q => q.OrderNumber))
                {
                    if (line.item_name != priorItemName)
                    {
                        p_container.Controls.Add(new LiteralControl("<br/><br/>"));
                        p_container.Controls.Add(new Label() { Text = line.item_name });
                        p_container.Controls.Add(new LiteralControl("<br/>"));
                    }
                    else
                        p_container.Controls.Add(new LiteralControl("; "));
                    
                    p_container.Controls.Add(new Label() { Text = line.txt_answer, CssClass = "clsMyPlanAnswer" });

                    priorItemName = line.item_name;
                }
            }
        }
        else // групповой
        {
            // общее кол-во, полностью заполнивших
            int CompleteNumber = 0;
            const int IPR = 1115; /*hard*/
            const int idItemForPercentage = 13559; /*hard. исключаем процент реализации ИПР*/
            
            int TextItemCount = (
                from sg in p_dc.subject_groups
                join lnk in p_dc.Test_SubjectGroup_Links on sg.id equals lnk.idGroup
                join blk in p_dc.Test_Questions on lnk.idTest equals blk.test_id
                join itm in p_dc.items on blk.id equals itm.group_id
                join dm in p_dc.SubScaleDimensions on itm.dimension_id equals dm.id
                where sg.id == IPR && (dm.dimension_type == 3 || dm.dimension_type == 11) // открытый вопрос или дата
                select 1
            ).Count();
            int NonTextItemCount = (
                from sg in p_dc.subject_groups
                join lnk in p_dc.Test_SubjectGroup_Links on sg.id equals lnk.idGroup
                join blk in p_dc.Test_Questions on lnk.idTest equals blk.test_id
                join itm in p_dc.items on blk.id equals itm.group_id
                join dm in p_dc.SubScaleDimensions on itm.dimension_id equals dm.id
                where sg.id == IPR && dm.dimension_type != 3 && dm.dimension_type != 11 && itm.id != idItemForPercentage 
                select 1
            ).Count();

            foreach (Test_Subject ts in p_dc.Test_Subjects.Where(w => w.group_id == IPR && w.Test_Date != null))
            {
                if ((p_dc.Test_Results_Txts.Where(ww => ww.subject_id == ts.id && ww.text != "").Count() >= TextItemCount) &&
                   (p_dc.Test_Results.Where(ww => ww.Subject_ID == ts.id && ww.item_id != idItemForPercentage).Select(s => s.item_id).Distinct().Count() >= NonTextItemCount))
                {
                    CompleteNumber++;
                }
            }
            //по каждой компетенции - лидер и кол-во выбравших эту компетенцию
            var n =
                from ss in p_dc.SubScales
                join i in p_dc.items on ss.Dimension_ID equals i.dimension_id
                join tr in p_dc.Test_Results on new { jpItemID = i.id, jpDimID = (int?)ss.id } equals new { jpItemID = tr.item_id, jpDimID = tr.SubScale_ID }
                join rd in p_dc.Raw_Datas on new { tr.Subject_ID, scl = (int)1999} equals new { rd.Subject_ID, scl = rd.Scale_ID } into outer
                where i.id == 13346 /*hard*/
                from ou in outer.DefaultIfEmpty()

                join trr in p_dc.Test_Results on new { jpSubjID = ou.Subject_ID, jpFixItemID = 13348/*hard*/} equals new { jpSubjID = trr.Subject_ID, jpFixItemID = trr.item_id } into outer2
                from ou2 in outer2.DefaultIfEmpty()
                select new dp() { name = ss.name, subjID = ou.Subject_ID, OrdNumber = ss.OrderNumber, ssID = ss.id, LeaderCode = ou2.SubScale_ID, CompletePercent = ou.Raw_Value };

            List<dpSummary> qq = n.GroupBy (gg=> new { gg.ssID, gg.name } ).Select (
                dd=> new dpSummary {
                    ssID = dd.Key.ssID, 
                    name = dd.Key.name,
                    Number = dd.Select(x=> x.subjID).Distinct().Count(cn=> cn.Value!=null),
                    PercentIPR = dd.Average(y => y.CompletePercent),
                }).ToList();
            

            p_container.Controls.Add(new Label() { Text = p_indicator.name, CssClass = "clsIndicatorName" });
            p_container.Controls.Add(new Label() { Text = string.Format("<br/><br/>полностью заполнили {0} человек<br/>", CompleteNumber) });

            int k = Convert.ToInt16(Math.Round((
                from ts in p_dc.Test_Subjects
                join rd in p_dc.Raw_Datas on ts.id equals rd.Subject_ID
                where ts.group_id == IPR && ts.Test_Date != null && rd.Scale_ID == 1999 // hard x hard
                select rd.Raw_Value).Average()));
            p_container.Controls.Add(new Label() { Text = string.Format("средний показатель реализации ИПР - {0}%<br/>", k) });

            foreach (dpSummary smr in qq)
            {
                var ldc =
                    n.Where(w => w.ssID == smr.ssID).GroupBy(g => g.LeaderCode).Select(s => new { kk = s.Key, cnt = s.Count() }).OrderByDescending(ob => ob.cnt).FirstOrDefault();
                
                string LeaderName = ldc.kk == null ? "" : p_dc.SubScales.Where(ww => ww.id == ldc.kk).FirstOrDefault().name;

                p_container.Controls.Add(new Label() { Text = string.Format ("<br/>{0} <b>развивают {1} сотр.</b> (лидер {2}). Реализация ИПР - {3}%", 
                    smr.name, smr.Number, LeaderName, Convert.ToInt16 (Math.Round( Convert.ToDecimal (smr.PercentIPR)))) });
            }
            

                
        }
    }
    
    /// <summary>
    /// квартальная оценка
    /// </summary>
    private static void QuartAssessment(TesterDataClassesDataContext p_dc, indicator p_indicator, Control p_container, DateTime QuartBegin,
        user_account p_ua)
    {
        int PriorQuartNumber = (DateTime.Today.AddMonths(-3).Month + 2) / 3;
        int PriorQuartYear = DateTime.Today.AddMonths(-3).Year;
        p_container.Controls.Add(new Label()
        {
            Text = string.Format("{0} за {1} квартал {2} года", p_indicator.name, PriorQuartNumber, PriorQuartYear), 
            CssClass = "clsIndicatorName"
        });
        
        if (!p_indicator.isPersonal)
        {
            //процент утвержденных самооценок за квартал
            int total = 0; int approved = 0;
            foreach (bool tsa in
                from apr in p_dc.test_subject_approveds
                join ts in p_dc.Test_Subjects on apr.idSubject equals ts.id
                where ts.Test_Date >= QuartBegin
                select apr.isApproved)
            {
                if (tsa) approved++;
                total++;
            }
            if (total != 0)
                p_container.Controls.Add(new LiteralControl(string.Format("<br/><br/>{0}% утвержденных самооценок за квартал<br/>", approved * 100 / total)));

            //2. среднее по каждой шкале по всем сотрудникам , с точностью до десятых
            foreach (FieldDataSet sas in (
                from td in p_dc.Test_Datas
                join ts in p_dc.Test_Subjects on td.Subject_ID equals ts.id
                join tsa in p_dc.test_subject_approveds on ts.id equals tsa.idSubject
                join ig in p_dc.idea_generators on ts.Test_Id equals ig.idTest
                where ig.idGeneratorType == 2 && ts.Test_Date >= QuartBegin && tsa.isApproved
                group td by td.Scale into g
                select new FieldDataSet { Name = g.Key.name, Value = g.Average(a => a.Test_Value), Order = g.Key.isMain ? 1 : 0 }).OrderByDescending(o => o.Order))
            {
                p_container.Controls.Add(new LiteralControl(string.Format("<br/>{0} - {1}", sas.Name, sas.Value.ToString("0.0"))));
            }
            p_container.Controls.Add(new LiteralControl("<br/>"));
            //3. процент по каждому уровню оценки
            Dictionary<string, decimal> dd =
            (from tr in p_dc.Test_Results
             join ts in p_dc.Test_Subjects on tr.Subject_ID equals ts.id
             join tsa in p_dc.test_subject_approveds on ts.id equals tsa.idSubject
             join ig in p_dc.idea_generators on ts.Test_Id equals ig.idTest
             join ss in p_dc.SubScales on tr.SubScale_ID equals ss.id
             where ig.idGeneratorType == 2 && ts.Test_Date >= QuartBegin && tsa.isApproved
             group tr by new { tr.SubScale.name, tr.SubScale.OrderNumber } into g
             select new FieldDataSet { Name = g.Key.name, Value = (decimal)g.Sum(s => s.SelectedValue), Order = g.Key.OrderNumber }).OrderBy(o => o.Order
             ).ToDictionary(d => d.Name, d => d.Value);

            total = decimal.ToInt32(dd.Sum(s => s.Value));
            if (total != 0)
            {
                foreach (var n in dd)
                {
                    p_container.Controls.Add(new LiteralControl(string.Format("<br/>{0} - {1}%", n.Key, (n.Value * 100 / total).ToString("0.0"))));
                }
            }
        }
        else // индивидуальный
        {
            foreach (FieldDataSet fds in (
                from td in p_dc.Test_Datas
                join ts in p_dc.Test_Subjects on td.Subject_ID equals ts.id
                join ig in p_dc.idea_generators on ts.Test_Id equals ig.idTest
                where ts.idUser == p_ua.idUser && ig.idGeneratorType == 2 && ts.Test_Date >= QuartBegin
                select new FieldDataSet { Name = td.Scale.name, Value = td.Test_Value, Order = td.Scale.isMain ? 1 : 0 }).OrderByDescending(o => o.Order))
            {
                p_container.Controls.Add(new LiteralControl(string.Format("<br/>{0} - {1}", fds.Name, fds.Value.ToString("0.0"))));
            }

            p_container.Controls.Add(new LiteralControl("<br/><br/>"));
            string Recomendation = (from tsa in p_dc.test_subject_approveds
                                    join ts in p_dc.Test_Subjects on tsa.idSubject equals ts.id
                                    where tsa.isApproved == true && ts.idUser == p_ua.idUser && ts.Test_Date >= QuartBegin
                                    select tsa.commentary).Take(1).FirstOrDefault();
            
            p_container.Controls.Add(new LiteralControl(Recomendation));
        }
    }
}