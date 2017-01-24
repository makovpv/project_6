using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Net;



public enum DimensionType
{
    dtSingleChoise = 1, dtMultiSelect = 2, dtOpenAnswer = 3,
    dtGender = 8, dtBirthYear = 9, dtRange = 10, dtDate = 11, dtNumber = 12,
    dtEMP = 13, dtCompetence = 14, dtBook = 15
}
//select 7, 'ранжирование',	1, 0 union all
//select 5, 'попарное ранжирование',	1, 0 union all
//select 6, 'шкалирование',	1, 0 union all

/// <summary>
/// Summary description for CommonData
/// </summary>
public class CommonData
{
    public CommonData()
    {
        //
        // TODO: Add constructor logic here
        //

    }

    public static TesterDataClassesDataContext GetNewDC()
    {
        // !!! . Не пытайтесь повторно использовать экземпляры DataContext. Каждый DataContext сохраняет состояние (включая кэш идентификации) для одного определенного сеанса редактирования/запроса. Для получения новых экземпляров на основе текущего состояния базы данных используйте новый DataContext.
        // http://msdn.microsoft.com/ru-ru/library/vstudio/bb386929%28v=vs.100%29.aspx


        //if (dc == null)
        //{
            //TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            //dc.LoadOptions.LoadWith<test>(
            //dc.DeferredLoadingEnabled = false; // !!
        //}
            return new TesterDataClassesDataContext();
    }

    class item_short_info { // может использовать тип из TesterDataClasses.dbml ?
        public int id {get; set;}
        public int dimension_id { get; set; }
    }
    class key_short_info
    {
        public short item_number { get; set; }
        public decimal kf { get; set; }
    }

    public static void SetItemScaleLinks (int aScaleID, int aBlockID, int[] arItemNumber, decimal[] arKF)
    {
        TesterDataClassesDataContext fdc = new TesterDataClassesDataContext();
        //string conn_str = System.Configuration.ConfigurationManager.ConnectionStrings["tester_dataConnectionString"].ConnectionString;
        //System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(conn_str);
        System.Data.Common.DbConnection conn = fdc.Connection;

        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        try
        {
            System.Data.Common.DbTransaction tran = conn.BeginTransaction();
            fdc.Transaction = tran; // может использовать new TesterDataClassesDataContext(); ??
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into ItemScale_link (scale_id, item_id, subscale_id, kf, dimension_id) ");
                sb.Append("select {0}, {1}, id, {2}, dimension_id from subscales where dimension_id={3} and ordernumber={4}");
                foreach (int ItemNumber in arItemNumber)
                {
                    IEnumerable<item_short_info> res = fdc.ExecuteQuery<item_short_info> ("select id, dimension_id from items where group_id = {0} and [number] = {1}",
                                        new object[] { aBlockID, ItemNumber });
                    item_short_info ItemInfo = res.First();

                    fdc.ExecuteCommand("delete from itemscale_link where item_id={0} and scale_id={1}", 
                        new object[] {ItemInfo.id, aScaleID});

                    for (int i=1; i<= arKF.Length; i++)
                    {
                        fdc.ExecuteCommand(sb.ToString(), new object[] {aScaleID,  ItemInfo.id, arKF[i-1], ItemInfo.dimension_id, i});
                    }
                }
                tran.Commit();
            }
            catch 
            {
                tran.Rollback();
            }
        }
        finally
        {
            //conn.Close();
        }
    }

    public static string GetExistKeysString(int p_ScaleID, int p_GroupID)
    {
        string res = "";

        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        
        key_short_info[] keys = 
            dc.ExecuteQuery<key_short_info>
                ("select i.number as item_number, lnk.kf from itemscale_link lnk "+
                 "inner join items i on i.id = lnk.item_id " +
                 "inner join SubScales ss on ss.id = lnk.subscale_id "+
                 "where scale_id = {0} and i.group_id = {1} order by i.number, ss.OrderNumber",
                 new object[] { p_ScaleID, p_GroupID }).ToArray<key_short_info>();

        if (keys.Length > 0)
        {
            int CurrItemNumer = Convert.ToInt16(keys[0].item_number);
            res += CurrItemNumer.ToString() + " (";
            foreach (key_short_info kk in keys)
            {
                string kf_str = "";
                if (kk.kf == Math.Round(kk.kf, 0))
                    kf_str = Convert.ToInt16(kk.kf).ToString();
                else
                    kf_str = kk.kf.ToString();

                if (CurrItemNumer == Convert.ToInt16 ( kk.item_number))
                    res += kf_str + ", ";
                else
                {
                    CurrItemNumer = Convert.ToInt16 ( kk.item_number);
                    res += ") --- " + CurrItemNumer.ToString() +" ("+kf_str+", ";
                }
            }
        }

        return res+")";
    }

    /// <summary>
    /// расстановка шкал для диаграммы
    /// </summary>
    public static void SetDiagramScales(short p_Diagram_ID, int[] arCheckedScales, byte[] arOrderNumbers)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.ExecuteCommand("delete Test_Diagram_Scales where diagram_id = {0}", p_Diagram_ID);

        for (int i = 0; i <= arCheckedScales.Length - 1; i++)
        {
            if (arCheckedScales[i] != 0)
            {
                dc.ExecuteCommand("insert into Test_Diagram_Scales (diagram_id, scale_id, orderNumber) values ({0}, {1}, {2})",
                        new object[] { p_Diagram_ID, arCheckedScales[i], arOrderNumbers[i]});
            }
        }

            //foreach (int scaleID in arCheckedScales)
            //    if (scaleID != 0)
            //        dc.ExecuteCommand("insert into Test_Diagram_Scales (diagram_id, scale_id, orderNumber) values ({0}, {1}, {2})",
            //            new object[] { p_Diagram_ID, scaleID, });
    }

    /// <summary>
    /// формирование скрипта с данными текста
    /// </summary>
    public static string GetDataScript(int p_TestID, List<Test_SubjectGroup_Link> p_MeasureList, string TestImgDirPath, 
        string p_GroupInstruction, bool isAnonymous)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("var AA;");
        sb.AppendLine("var QQ = new Array();");
        sb.AppendLine("var DD = new Array();");
        sb.AppendLine("var BB = new Array();");
        sb.AppendLine("var MM = new Array();");

        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            test Tst = dc.tests.Where(p => p.id == p_TestID).FirstOrDefault();

            //if (Tst.instruction!=null && Tst.instruction!="")
            sb.AppendFormat("var inst='{0}'", Tst.instruction != null ? Tst.instruction.Replace("\n", "\\n") : "");
            sb.AppendLine();
            sb.AppendFormat("var g_inst='{0}'", p_GroupInstruction != null ? p_GroupInstruction.Replace("\n", "\\n").Replace("\r", "\\r") : "");
            sb.AppendLine();
            sb.AppendFormat("TestID={0}", p_TestID);
            sb.AppendLine();
            if (p_MeasureList != null)
                if (p_MeasureList.Count == 0)
                    throw new Exception("нет доступных объектов оценки");
                else
                {
                    foreach (Test_SubjectGroup_Link lnk in p_MeasureList)
                    {
                        sb.AppendLine("MM.push({TestID:" + lnk.idTest.ToString() +
                            ",lnkID:" + lnk.id.ToString() +
                            ",txt:'" + lnk.MeasureObjectName +
                            "',isReq:" + (lnk.isObjectRequired ? "1" : "0") +
                            ",isDone:false})");
                    }
                }
            else
                sb.AppendFormat("MM.Push({TestID:{0},lnkID:'',txt:'',isReq:1})", p_TestID);
            //IEnumerable<SubScaleDimension> aDimensions =  dc.ExecuteQuery<SubScaleDimension>("select * from subscaledimension where test_id={0}", new object[] { p_TestID });
            foreach (SubScaleDimension dim in Tst.SubScaleDimensions)
            {
                sb.AppendLine("AA=new Array();");
                dim.SubScales.Load(); // HM...
                foreach (SubScale ss in dim.SubScales.OrderBy(o=> o.OrderNumber))
                {
                    if (dim.dimension_type == 5) // ранжирование
                    {
                        int idx = ss.name.IndexOf('(');
                        if (idx > 0)
                        {
                            sb.AppendLine("AA.push({text:'" + ss.name.Substring(0, idx - 1) + "',hnt:'" + ss.name.Substring(idx, ss.name.Length - idx) + "',id:" + ss.id.ToString() + ",num:" + ss.OrderNumber.ToString() + "});");
                        }
                        else
                            sb.AppendLine("AA.push({text:'" + ss.name + "',id:" + ss.id.ToString() + ",num:" + ss.OrderNumber.ToString() + "});");
                    }
                    else
                        sb.AppendLine("AA.push({text:'" + ss.name + "',id:" + ss.id.ToString() + ",num:" + ss.OrderNumber.ToString() + "});");
                }

                if (dim.dimension_type == 6) // шкалирование
                {
                    string val = string.Format("id:{0},type:{1},mode:{2},min:{3},max:{4},step:{5},aa:AA",
                        dim.id, dim.dimension_type, dim.dimension_mode,
                        dim.min_value == null ? 0 : dim.min_value,
                        dim.max_value == null ? 0 : dim.max_value,
                        Convert.ToInt16(dim.step_value));
                    sb.AppendLine("DD.push({" + val + "})");
                }
                else
                {
                    string val = string.Format("id:{0},type:{1},mode:{2},aa:AA",
                        dim.id, dim.dimension_type, dim.dimension_mode);
                    sb.AppendLine("DD.push({" + val + "})");
                }
            }

            //IEnumerable<Test_Question> aGroups = dc.ExecuteQuery<Test_Question> ("select id, text, instruction, comment from test_question where test_id = {0} order by number ", new object[] { p_TestID });
            foreach (Test_Question tq in Tst.Test_Questions)
            {
                string BlockLine = string.Format("id:{0},txt:'{1}',instr:'{2}',com:'{3}',ri:{4},ra:{5},tr:{6}", tq.id, tq.text,
                    tq.instruction == null ? "" : tq.instruction.Replace("\n", "\\n"),
                    tq.comment == null ? "" : tq.comment.Replace("\n", "\\n"),
                    tq.isShuffledItem ? 1 : 0,
                    tq.isShuffledAns ? 1 : 0,
                    tq.isTimeRestrict ? 1 : 0
                    );
                sb.AppendLine("BB.push({" + BlockLine + "})");

                IEnumerable<item> aItems;
                if (tq.isShuffledItem)
                    aItems = dc.ExecuteQuery<item>(
                            "select id, text, dimension_id, number from items where group_id={0}",
                            new object[] { tq.id }
                        ).OrderBy(p => Guid.NewGuid());
                else
                    aItems = dc.ExecuteQuery<item>(
                            "select id, text, dimension_id, number from items where group_id={0} order by number",
                            new object[] { tq.id });

                foreach (item itm in aItems)
                {
                    if (itm.dimension_id != null)
                    {
                        string imgType = GetQuestImageExtension(TestImgDirPath, itm.id);

                        string QLine = "QQ.push({text:'" + (itm.text == null ? "" : itm.text.Replace("'", "\"")) + "',id:" + itm.id.ToString() +
                            ",img:'" + imgType + "'" +
                            ",dim:" + itm.dimension_id.ToString() +
                            ",blk:" + tq.id.ToString() +
                            ",nn:" + (itm.number == null ? "0" : itm.number.ToString()) + "});";
                        sb.AppendLine(QLine);
                    }
                }
            }

            sb.AppendFormat("var anon={0}", isAnonymous ? 1 : 0);
            
        }
        return sb.ToString();
    }

    public static string GetQuestImageExtension(string TestImgDirPath, int itemID)
    {
        string imgType = null;
        //string s = string.Format("~\\Images\\{0}\\{1}.jpg", tq.test_id, itm.id);
        //string s = "d:\\Projects\\Tester\\WebTester\\Images\\13\\100.JPG ";
        //string s = string.Format("{0}\\{1}.*", TestImgDirPath, itm.id);
        try
        {
            if (Directory.Exists(TestImgDirPath))
            {
                string[] files = Directory.GetFiles(TestImgDirPath, itemID.ToString() + ".*");
                string ImgFile = files.FirstOrDefault();
                if (ImgFile != null)
                    imgType = Path.GetExtension(ImgFile);
            }
        }
        catch { }
        return imgType;
    }

    /// <summary>
    /// добавление произвольного диапазона для расчета Т-балла
    /// </summary>
    public static void Add_TScore_Range(int TScore, int LowLevel, int HighLevel)
    {
        //GetDC().ExecuteCommand("exec dbo.Add_TScore_Range {0}, {1}, {3}");
    }
    /// <summary>
    /// генерация вариантов ответов, как перечень действующих на тек.момент сотрудников. 
    /// (запись в БД коммитить нужно самостоятельно)
    /// </summary>
    public static void GenerateAnswerWithEMP(SubScaleDimension ssd, Company cmp)
    { 
        if (ssd.dimension_type == (byte)DimensionType.dtEMP) // "действующие сотрудники"
        {
            var qqs = (from ua in cmp.user_accounts
                        select ua).Where(q => q.idState != 1 && q.idState != 3 && q.idState != 4 && q.idState!= null).OrderBy(o=> o.fio);
            byte OrderNum = 0; // кол-во сотрудников может быть более 255!
            foreach (user_account ua in qqs)
            {
                OrderNum++;
                if (ssd.SubScales.FirstOrDefault(f => f.name == ua.fio) == null)
                {
                    ssd.SubScales.Add(new SubScale()
                    {
                        name = ua.fio == null ? ua.login_name : ua.fio,
                        OrderNumber = OrderNum
                    });
                }
            }
        }
    }
    public static void GenerateAnswerWithBook(SubScaleDimension ssd, Company cmp)
    {
        if (ssd.dimension_type == (byte)DimensionType.dtBook) 
        {
            var qqs = (from bk in cmp.books
                       select bk).OrderBy(o => o.title);
            byte OrderNum = 0; 
            foreach (book bk in qqs)
            {
                OrderNum++;
                if (ssd.SubScales.FirstOrDefault(f => f.name == bk.title) == null)
                {
                    ssd.SubScales.Add(new SubScale()
                    {
                        name = bk.title,
                        OrderNumber = OrderNum
                    });
                }
            }
        }
    }
    public static void GenerateAnswerWithCompetence(SubScaleDimension ssd, Company cmp)
    {
        if (ssd.dimension_type == (byte)DimensionType.dtCompetence)
        {
            var qqs = (from c in cmp.competences
                       select c).OrderBy(o => o.name);
            byte OrderNum = 0;
            foreach (competence c in qqs)
            {
                OrderNum++;
                if (ssd.SubScales.FirstOrDefault(f => f.name == c.name) == null)
                {
                    ssd.SubScales.Add(new SubScale()
                    {
                        name = c.name,
                        OrderNumber = OrderNum
                    });
                }
            }
        }
    }

    #region Dynamic Text
    public static void SetPassportTemplate(string TemplateText, short p_TemplateID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.ExecuteCommand("update passport_template set template_text = {0} where id={1}", new object[] { TemplateText, p_TemplateID });
    }
    public static string GetPassportTemplate(TesterDataClassesDataContext p_dc, short p_TemplateID)
    {
        //TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return p_dc.ExecuteQuery<string>("select template_text from passport_template where id = {0}", 
            new object[] { p_TemplateID }).First();
    }

    public static string GetResumeDynamicText(Resume_Item p_ResumeItem, Test_Subject p_Subject)
    {
        string res = "";
        if (p_ResumeItem.Passport_Template == null)
            res = p_ResumeItem.item_text;
        else
            res = p_ResumeItem.Passport_Template.template_text;

        if (res != null)
        {
            foreach (Scale scl in p_ResumeItem.test.Scales) // перенести в ReplaceDynamicText ?
            {
                string abr = scl.abreviature == null ? "" : scl.abreviature;
                int idx = res.ToUpper().IndexOf("[" + abr.ToUpper() + "]");
                if (idx != -1)
                {
                    Test_Data td = scl.Test_Datas.Where(p => p.Subject_ID == p_Subject.id).FirstOrDefault();
                    if (td != null)
                        res = res.Replace("[" + abr + "]",
                            (td.Test_Value).ToString("###0.###"));
                }
            }

            res = ReplaceDynamicText(res, p_ResumeItem.test, p_ResumeItem.test_diagram, p_Subject);
        }

        return res;
    }

    // возможно стоит перенести саму текстовку значений в БД (test_caregory.Description)
    private static string[] TestCategoryText = new string[4] { 
        "Этот тест относится к популярным методикам. Его результаты интересны и просты для понимания. Вместе с тем они могут быть поверхностны и ошибочны и не совсем обоснованы с научной точки зрения", 
        "Это тест разработан специалистами и имеет под собой научные основания. Однако, его тестовые нормы и качество заданий давно не пересматривались. Поэтому рекомендуем относиться к полученным результатам довольно критически.", 
        "Этот тест только находится в стадии создания. Разработчки пока еще только ищут лучшие варианты вопросов, определяют шкалы, формируют тестовые нормы. Очень важно, чтобы Вы проходили подобные тесты, так как это дает разработчикам ценную информацию и позволяет улучшить качество теста и его психометрические свойства.",
        "Этот тест относится к классу психометрических стандартизированных тестов. Его результаты научно обоснованы и проверены практикой. Чтобы правильно понять полученные данные и сделать выводы, рекомендуем обратиться к опытному психодиагносту." 
    };

    private static string GetTValuesVarStr(Dictionary<decimal,string> p_Dict)
    {
        string res = "";

        if (p_Dict.Count == 1)
            res = string.Format("{0} баллов ", p_Dict.Keys.ToArray()[0].ToString("##0.###"));
        else
        {
            foreach (short kk in p_Dict.Keys.ToArray())
            {
                res += string.Format("{0} - {1} баллов, ", p_Dict[kk], kk.ToString("##0.###"));
            }
        }
        return res;
    }

    struct ScaleNormRanges { public decimal? r1; public decimal? r2; public decimal? r3; public decimal? r4;}

    static ScaleNormRanges GetScaleNormRanges (Scale p_Scale)
    {
        ScaleNormRanges res;

        if (p_Scale.Scale_Norms.Count > 0)
        {
            res.r1 = p_Scale.Scale_Norms.Where(p => p.NormType_id == 1).FirstOrDefault().HighRange;
            res.r2 = p_Scale.Scale_Norms.Where(p => p.NormType_id == 2).FirstOrDefault().HighRange;
            res.r3 = p_Scale.Scale_Norms.Where(p => p.NormType_id == 3).FirstOrDefault().HighRange;
            res.r4 = p_Scale.Scale_Norms.Where(p => p.NormType_id == 4).FirstOrDefault().HighRange;
        }
        else
        {
            res.r1 = Convert.ToDecimal(p_Scale.AVG_Value - 2 * p_Scale.Standard_Dev);
            res.r2 = Convert.ToDecimal(p_Scale.AVG_Value - p_Scale.Standard_Dev);
            res.r3 = Convert.ToDecimal(p_Scale.AVG_Value + p_Scale.Standard_Dev);
            res.r4 = Convert.ToDecimal(p_Scale.AVG_Value + 2 * p_Scale.Standard_Dev);
        }
        return res;
    }

    private static string GetScaleDiagnosticRange(Scale p_Scale,decimal p_TestValue)
    {
 	    string res;
        ScaleNormRanges snr = GetScaleNormRanges (p_Scale);
        if (p_TestValue < snr.r1) res = "очень низким";
        else if (p_TestValue < snr.r2) res = "низким";
        else if (p_TestValue <= snr.r3) res = "нормальным";
        else if (p_TestValue < snr.r4) res = "высоким";
        else res = "очень высоким";

        return res;
    }

    /// <summary>
    /// с учетом типа расчета "проценты"
    /// </summary>
    private static decimal GetScaleSmartMaxValue(Scale p_Scale)
    {
        if (p_Scale.ScoreCalcType == 3)
            return 100;
        else
            return Convert.ToDecimal (p_Scale.max_value);
    }
    /// <summary>
    /// с учетом типа расчета "проценты"
    /// </summary>
    private static decimal GetScaleSmartMinValue(Scale p_Scale)
    {
        if (p_Scale.ScoreCalcType == 3)
            return Convert.ToDecimal ((decimal)100.0 * p_Scale.min_value / p_Scale.max_value);
        else
            return Convert.ToInt16(p_Scale.min_value);
    }

    private static string ReplaceDynamicText(string InputText, test tt, test_diagram p_Diagram, Test_Subject p_Subject)
    {
        Dictionary<string, string> VarDict = new Dictionary<string, string>();
        VarDict.Add("[Автор]", tt.author);
        VarDict.Add("[ВерсияТеста]", tt.version_number);
        VarDict.Add("[Аббревиатура]", tt.abbreviature);
        VarDict.Add("[ГодПубликации]", Convert.ToString(tt.publish_year));
        VarDict.Add("[Издатель]", tt.publisher);
        VarDict.Add("[ИсторияРазработки]", tt.develop_history);
        VarDict.Add("[ЗадачиИсследования]", tt.psy_task);
        VarDict.Add("[ТеоретическийКонструкт]", tt.Theory_Construct_Info);
        VarDict.Add("[Надежность]", tt.reliability_info);
        VarDict.Add("[Валидность]", tt.validation_info);
        VarDict.Add("[ИндексСоцЗначимости]", tt.social_advisability_idx);
        VarDict.Add("[ТестовыеНормы]", tt.test_norms);

        VarDict.Add("[Инструкция]", tt.instruction);
        VarDict.Add("[МетодРекомендации]", tt.mthd_recomendation);
        VarDict.Add("[ОграниченияИспользования]", tt.use_restriction);
        VarDict.Add("[КвалификационныеТребования]", tt.qualification_demand);
        VarDict.Add("[ОсобенностиИТВерсии]", tt.ITVersionSpecific);
        VarDict.Add("[Наименование]", tt.name);
        VarDict.Add("[Тэги]", tt.Tags);
        VarDict.Add("[ПредметДиагностики]", tt.diagnostic_subj);
        VarDict.Add("[КраткоеОписаниеТеста]", tt.Short_Description);

        if (tt.category_id != null)
            VarDict.Add("[Категория]", TestCategoryText[Convert.ToByte(tt.category_id) - 1]);
        else
            VarDict.Add("[Категория]", "");

        int ItemCount = 0;
        foreach (Test_Question tq in tt.Test_Questions)
        {
            ItemCount += tq.items.Count;
        }
        VarDict.Add("[КолВопросов]", ItemCount.ToString());

        VarDict.Add("[КолСубъектов]", tt.Test_Subjects.Count.ToString());

        VarDict.Add("[ЧислоШкал]", tt.Scales.Count.ToString());
        VarDict.Add("[ЧислоБлоков]", tt.Test_Questions.Count.ToString());

        string tmpStr = "";
        string ScalesWithDescr = "";
        foreach (Scale scl in tt.Scales) 
        {
            tmpStr+=", "+scl.name;
            if (!string.IsNullOrEmpty(scl.description))
                ScalesWithDescr += string.Format("<p><b>{0} ({1}).</b> {2}", scl.abreviature, scl.name, scl.description);

            try
            {
                VarDict.Add(string.Format("[{0}_Описание]", scl.abreviature), scl.description); // одинаковые аббревиатуры ?
            }
            catch { }
        }
        if (tmpStr.Length>0) 
            tmpStr=tmpStr.Remove(0,2);
        VarDict.Add("[НазванияВсехШкал]", tmpStr);
        VarDict.Add("[ШкалыСОписанием]", ScalesWithDescr);
        
        tmpStr = "";
        foreach (Test_Question tq in tt.Test_Questions) { tmpStr += "," + tq.text; }
        if (tmpStr.Length > 0) tmpStr = tmpStr.Remove(0);
        VarDict.Add("[НазванияВсехБлоков]", tmpStr);

        if (InputText.IndexOf("[СреднееВремяПрохождения]") != -1)
        {
            var res = (from t in tt.Test_Subjects
                        select t).Average(p => System.Data.Linq.SqlClient.SqlMethods.DateDiffMinute (p.Test_Date_Start, p.Test_Date));
            //VarDict.Add("[СреднееВремяПрохождения]", "<пока_не_реализовано>");
            VarDict.Add("[СреднееВремяПрохождения]", Convert.ToInt16 (res).ToString());
        }

        string ScalesWithScore = "";
        if (p_Subject != null)
            foreach (Test_Data td in p_Subject.Test_Datas)
            {
                if (td.Scale.description !=null && td.Scale.description != "")
                {
                    ScalesWithScore += string.Format("<p><b>{0} ({1}). {2} баллов.</b> {3}", td.Scale.abreviature, td.Scale.name, td.Test_Value.ToString("###0.###"), td.Scale.description); // +"\r\n";
                }
            }
        VarDict.Add("[БаллыПоШкаламСОписанием]", ScalesWithScore); // переменная не нужна. убрать (проверить что не используется)

        // по диаграмме переменные
        if (p_Diagram != null)
        {
            VarDict.Add("[ЧислоШкалВключенныхВДиаграмму]", p_Diagram.Test_Diagram_Scales.Count.ToString());
            Test_Data[] ptd = (
                    from td in p_Subject.Test_Datas
                     join dd in p_Diagram.Test_Diagram_Scales on td.Scale_ID equals dd.scale_id
                     select td).OrderByDescending (q=> q.Test_Value).ToArray();

            if (ptd.Length > 1)
            {
                VarDict.Add("[ДиаграммыОднаШкалаСВысшимБаллом]", ptd[0].Scale.name);
                VarDict.Add("[ДиаграммыОднаШкалаСНизшимБаллом]", ptd[ptd.Length - 1].Scale.name);
                VarDict.Add("[ДиаграммыДвеШкалыСВысшимБаллом]", string.Format("{0} и {1}", ptd[0].Scale.name, ptd[1].Scale.name));
                VarDict.Add("[ДиаграммыДвеШкалыСНизшимБаллом]", string.Format("{0} и {1}", ptd[ptd.Length - 1].Scale.name, ptd[ptd.Length - 2].Scale.name));
            }
            else
                if (ptd.Length == 1)
                {
                    VarDict.Add("[ДиаграммыДвеШкалыСВысшимБаллом]", ptd[0].Scale.name);
                    VarDict.Add("[ДиаграммыОднаШкалаСВысшимБаллом]", ptd[0].Scale.name);
                }

            string NormScore = "";
            string HiScore = "";
            string LowScore = "";
            string ExtraLowScore = "";
            string ExtraHiScore = "";

            Dictionary<string, string> NormRanges = new Dictionary<string, string>();
            Dictionary<decimal, string> MinTValues = new Dictionary<decimal, string>();
            Dictionary<decimal, string> MaxTValues = new Dictionary<decimal, string>();

            foreach (Test_Diagram_Scale tds in p_Diagram.Test_Diagram_Scales)
            {
                ScaleNormRanges snr = GetScaleNormRanges (tds.Scale);
                if (snr.r2 != null || snr.r3 != null)
                {
                    string NormRangeKey = string.Format("от {0} до {1}", snr.r2, snr.r3);
                    if (NormRanges.ContainsKey(NormRangeKey))
                        NormRanges[NormRangeKey] += ", " + tds.Scale.name;
                    else NormRanges.Add(NormRangeKey, "по шкале " + tds.Scale.name);

                    Test_Data td = p_Subject.Test_Datas.Where(p => p.Scale_ID == tds.scale_id).FirstOrDefault();
                    if (td != null)
                    {
                        decimal TestScore = td.Test_Value;
                        if (TestScore < snr.r1)
                            ExtraLowScore += string.Format(", {0} ({1}, {2} баллов)", tds.Scale.name, tds.Scale.abreviature, TestScore.ToString("###0.###"));
                        else if (TestScore < snr.r2)
                            LowScore += string.Format(", {0} ({1}, {2} баллов)", tds.Scale.name, tds.Scale.abreviature, TestScore.ToString("###0.###"));
                        else if (TestScore <= snr.r3)
                            NormScore += string.Format(", {0} ({1}, {2} баллов)", tds.Scale.name, tds.Scale.abreviature, TestScore.ToString("###0.###"));
                        else if (TestScore <= snr.r4)
                            HiScore += string.Format(", {0} ({1}, {2} баллов)", tds.Scale.name, tds.Scale.abreviature, TestScore.ToString("###0.###"));
                        else
                            ExtraHiScore += string.Format(", {0} ({1}, {2} баллов)", tds.Scale.name, tds.Scale.abreviature, TestScore.ToString("###0.###"));
                    }
                }

                if (MinTValues.ContainsKey(GetScaleSmartMinValue (tds.Scale)))
                    MinTValues[GetScaleSmartMinValue(tds.Scale)] += ", " + tds.Scale.name;
                else MinTValues.Add(GetScaleSmartMinValue(tds.Scale), "по шкале " + tds.Scale.name);

                if (MaxTValues.ContainsKey(GetScaleSmartMaxValue(tds.Scale)))
                    MaxTValues[GetScaleSmartMaxValue(tds.Scale)] += ", " + tds.Scale.name;
                else MaxTValues.Add(GetScaleSmartMaxValue(tds.Scale), "по шкале " + tds.Scale.name);
            }

            VarDict.Add("[ШкалыВНорме]", NormScore);
            VarDict.Add("[ШкалыВышеНормы]", HiScore);
            VarDict.Add("[ШкалыНижеНормы]", LowScore);
            VarDict.Add("[ШкалыГораздоНижеНормы]", ExtraLowScore);
            VarDict.Add("[ШкалыГораздоВышеНормы]", ExtraHiScore);

            string NormRangesStr = "";
            if (NormRanges.Count == 1)
                NormRangesStr = NormRanges.Keys.ToArray()[0];
            else
                foreach (string kk in NormRanges.Keys.ToArray())
                {
                    NormRangesStr += string.Format("{0} ({1}), ", NormRanges[kk], kk);
                }

            VarDict.Add("[ДиапазонНормыПоШкалам]", NormRangesStr);
            VarDict.Add("[МинБаллПоШкаламДиаграммы]", GetTValuesVarStr(MinTValues));
            VarDict.Add("[МаксБаллПоШкаламДиаграммы]", GetTValuesVarStr(MaxTValues));

            if (p_Diagram.Test_Diagram_Scales.Count == 1)
            {
                Scale SingleScale = p_Diagram.Test_Diagram_Scales[0].Scale;
                Test_Data td = p_Subject.Test_Datas.Where(p => p.Scale_ID == SingleScale.id).FirstOrDefault();
                if (td != null) 
                {
                    VarDict.Add("[РезультатПоШкалеДиаграммы]", td.Test_Value.ToString("##0.###"));
                    VarDict.Add("[ДиагностическийДиапазонПоШкалеДиаграмме]", 
                        GetScaleDiagnosticRange(SingleScale, td.Test_Value));

                    int SubjAmount =SingleScale.Test_Datas.Count;
                    if (SubjAmount > 1)
                    {
                        int SubjGreater = SingleScale.Test_Datas.Where (p=> p.Test_Value > td.Test_Value).Count();
                        int SubjLess = SingleScale.Test_Datas.Where (p=> p.Test_Value < td.Test_Value).Count();
                        VarDict.Add("[ПроцентРеспСРезультатомВыше]", Convert.ToString(Convert.ToInt16(100 *SubjGreater / (SubjAmount-1))));
                        VarDict.Add("[ПроцентРеспСРезультатомНиже]", Convert.ToString(Convert.ToInt16(100 *SubjLess / (SubjAmount-1))));
                    }
                }
            }
        }

        //string pattern = @"\{.*\[.*\].*\}";
        //string pattern = @"\{.*\[";
        //string pattern = @"\D+";

        string Result = "";

        //string[] zz = Regex.Split (TemplateText, pattern);
        string[] zz = InputText.Split('{');
        foreach (string txt_part in zz)
        {
            if (txt_part.IndexOf('}') != -1)
            {
                string VarName = Regex.Match(txt_part, @"\[.*\]").Value;
                if (VarDict.ContainsKey(VarName))
                {
                    if (VarDict[VarName] != "")
                    {
                        Result += txt_part.Replace(VarName, VarDict[VarName]).Replace("}", "");
                    }
                }
                else
                    Result += txt_part.Substring(txt_part.IndexOf('}') + 1);
            }
            else
                Result += txt_part;
        }

        //VarDict.Count


        foreach (string key in VarDict.Keys)
        {
            Result = Result.Replace(key, VarDict[key]);
        }

        //bool aa = rex.IsMatch("sddssvff {zxzxxcc [Абв] forever}");

        //MatchCollection mc = Regex.Matches (TemplateText, pattern);

        //

        //TemplateText.

        //int idxFin = TemplateText.IndexOf('}');


        //test tt = new test() { id = p_TestID };

        //System.Data.Linq.EntitySet<test> es = new System.Data.Linq.EntitySet<test>();
        //es.Load();

        //this._interpretations = new EntitySet<interpretation>(new Action<interpretation>(this.attach_interpretations), new Action<interpretation>(this.detach_interpretations));


        //test tt = GetDC().tests.Where(p => p.id == p_TestID).First();
        //item // число итемов ??
        //return TemplateText.Replace(
        //    "[Автор]", tt.author).Replace(
        //    "[ВерсияТеста]", tt.version_number).Replace(
        //    "[Аббревиатура]", tt.abbreviature).Replace(

        //    "[ГодПубликации]", Convert.ToString(tt.publish_year)).Replace(
        //    "[Издатель]", Convert.ToString(tt.publisher)).Replace(
        //    "[ИсторияРазработки]", tt.develop_history).Replace(
        //    "[ЗадачиИсследования]", tt.psy_task).Replace(
        //    "[ТеоретическийКонструкт]", tt.Theory_Construct_Info).Replace(
        //    "[Надежность]", tt.reliability_info).Replace(
        //    "[Валидность]", tt.validation_info).Replace(
        //    "[ИндексСоцЗначимости]", tt.social_advisability_idx).Replace(
        //    "[ТестовыеНормы]", tt.test_norms).Replace(

        //    "[ЧислоШкал]", tt.Scales.Count.ToString()).Replace(
        //    "[ЧислоБлоков]", tt.Test_Questions.Count.ToString()).Replace(

        //    "[Инструкция]", tt.instruction).Replace(
        //    "[МетодРекомендации]", tt.mthd_recomendation).Replace(
        //    "[ОграниченияИспользования]", tt.use_restriction).Replace(
        //    "[КвалификационныеТребования]", tt.qualification_demand).Replace(
        //    "[ОсобенностиИТВерсии]", tt.ITVersionSpecific).Replace(

        //    "[Наименование]", tt.name);


        return Result;

    }

    public static string GetPassportInfo(int p_TestID)
    {
        TesterDataClassesDataContext dc = GetNewDC();
        string TemplateText = GetPassportTemplate(dc, 1); // templateID = 1 пока временно, потом сделать выбор из шаблонов
        test tt = dc.ExecuteQuery<test>("select * from test where id={0}", new object[] { p_TestID }).First();
        return ReplaceDynamicText(TemplateText, tt, null, null);
    } 
    #endregion

    public static byte[] GetTestResult(string AppPath, int aTestID, int? aSubjGroupID)
    {
        if (!System.IO.Directory.Exists(AppPath + "/Temp"))
            System.IO.Directory.CreateDirectory(AppPath + "/Temp");

        string sFileName = string.Format ("{0}Temp\\{1}.csv", AppPath, aTestID); //System.IO.Path.GetRandomFileName();
        //string sGenName = "Friendly.csv";

        //YOu could omit these lines here as you may
        //not want to save the textfile to the server
        //I have just left them here to demonstrate that you could create the text file 
        using (System.IO.StreamWriter SW = new System.IO.StreamWriter(sFileName, false, Encoding.UTF8))
        {
            string Header = "subject;gender;age;start;finish"; 
            
            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();

            //var result = (from ssd in dc.SubScaleDimensions
            //              join itm in dc.items on ssd.id equals itm.dimension_id
            //              join ss in dc.SubScales on itm.dimension_id equals ss.Dimension_ID into sss
            //              from a in sss.DefaultIfEmpty()
            //              where ssd.test_id == aTestID
            //              select new {itm.number, a.OrderNumber, a.name, ssd.dimension_type}
            //                  ).ToList();

            //for (int z=0; z<=10; z++)
            //{
            //    Header += string.Format(";{0}{1}", result[z].number, result[z].name);
            //}

            //---------------------------------------------------------------

            //IEnumerable<QryRes> res =
            foreach (string hs in dc.ExecuteQuery<string>(
                "select '['+cast (tq.number as varchar(9))+'] '+ cast(q.item_number as varchar(9)) + isnull(' ('+cast(q.var_number as varchar(9)) +')','') " +
                "from Test_Question tq " +
                "inner join	" +
                "( " +
                "	select i.group_id, i.number as item_number, ss.OrderNumber as var_number, i.id as item_id, ss.id as var_id " +
                "	from items i" +
                "	inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (2,5)" +
                "	inner join SubScales ss on ss.Dimension_ID = i.dimension_id " +
                "	UNION ALL" +
                "	select i.group_id, i.number, null, i.id, null" +
                "	from items i " +
                "	inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (1,6,3)" +
//                "	inner join SubScales ss on ss.Dimension_ID = i.dimension_id" +
                "	) q on q.group_id = tq.id " +
                "where tq.test_id = {0} " +
                "order by tq.number, tq.id, q.item_number, q.item_id, q.var_number, q.var_id",
                new object[] { aTestID })
            )
            {
                Header += ";" + hs;
            }
            SW.WriteLine(Header);

            List<Test_Subject> ts_list;
            if (aSubjGroupID == null)
                ts_list = dc.Test_Subjects.Where(p => p.Test_Id == aTestID).ToList();
            else
                ts_list = dc.Test_Subjects.Where(p => p.Test_Id == aTestID && p.group_id == aSubjGroupID).ToList();

            foreach (Test_Subject ts in ts_list)
            {
                string line = string.Format ("{0};{1};{2};{3};{4}", 
                    new object[] {ts.Nick_Name, ts.Gender, ts.Age, ts.Test_Date_Start, ts.Test_Date});

                foreach (string ans_line in dc.ExecuteQuery<string>("select answer from dbo.SubjectAnswers({0})", new object[] { ts.id }))
                {
                    line += ";" + ans_line;
                }
                SW.WriteLine(line);

            }

            SW.Close();
        }

        System.IO.FileStream fs = null;
        fs = System.IO.File.Open(sFileName, System.IO.FileMode.Open);
        byte[] btFile = new byte[fs.Length];
        fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
        fs.Close();

        return btFile;
    }

    class ItemInfo
    {
        public byte bl_number {get;set;}
        public short itm_number { get; set; }
        public string itm_text;
        public byte ans_number;
        public string ans_text;
        public int idItem;
        public int idAns;
    }
    public static byte[] GetMechanicData(string AppPath, int aTestID)
    {
        const string queryString =  //string.Format (
        "select isnull(tq.number,0) as bl_number , i.number as itm_number," +
        "i.[text] as itm_text, ss.ordernumber as ans_number, ss.name as ans_text " +
        ",i.id as idItem , ss.id as idAns " +
        "from test_question tq " +
        "inner join items i on i.group_id=tq.id " +
        "inner join SubScaleDimension ssd on ssd.id = i.dimension_id and ssd.dimension_type in (1,6,3) " +
        "inner join SubScales ss on ss.Dimension_ID = i.dimension_id " +
        "where tq.test_id = {0} " +
        "order by tq.number, i.number, ss.ordernumber";

        if (!System.IO.Directory.Exists(AppPath + "/Temp"))
            System.IO.Directory.CreateDirectory(AppPath + "/Temp");

        string sFileName = string.Format ("{0}Temp\\{1}.csv", AppPath, aTestID);
        using (System.IO.StreamWriter SW = new System.IO.StreamWriter(sFileName, false, Encoding.UTF8))
        {
            //            string Header = "subject;gender;age;start;finish"; 
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                //ItemInfo[] lst = dc.ExecuteQuery<ItemInfo>(queryString, new object[TestID]).ToArray<ItemInfo>();
                //List<ItemInfo> lst3 = dc.ExecuteQuery<ItemInfo>(queryString, new object[]{}).ToList<ItemInfo>();

                //using (SqlConnection connection = new SqlConnection(new TesterDataClassesDataContext().Connection.ConnectionString))
                //{
                //    SqlCommand command = new SqlCommand(queryString, connection);
                //    connection.Open();
                //    SqlDataReader reader = command.ExecuteReader();
                //    try
                //    {
                //        //string

                //        //while (reader.Read())
                //        //{
                //        //    Console.WriteLine(String.Format("{0}, {1}",
                //        //        reader["OrderID"], reader["CustomerID"]));
                //        //}
                //    }
                //    finally
                //    {
                //        connection.Close();
                //    }
                //}

                //SqlDataAdapter adapter = new SqlDataAdapter("select 12 aa, 23 bb union select 111, 222",  dc.Connection.ConnectionString);
                //DataSet MyData = new DataSet();
                //adapter.Fill(MyData);

                ItemInfo[] lst = dc.ExecuteQuery<ItemInfo>(queryString, new object[] { aTestID }).ToArray();
                List<Test_Subject> ts_list = dc.Test_Subjects.Where(p => p.Test_Id == aTestID).OrderBy(q=> q.Nick_Name).ToList();
                string Line1 = ";;;;";
                string Line2 = ";;;;";
                string Line3 = "Респондент;Дата прохождения;Компания;Должность;Исследование";
                //string PriorIetmText = "";
                foreach (ItemInfo ii in lst)
                {
                    Line1 += string.Format(";{0}", ii.bl_number);
                    //if (ii.itm_text != PriorIetmText)
                    //{
                        Line2 += string.Format(";{0}.{1}", ii.itm_number, ii.itm_text.Replace(';', '~'));
                        //PriorIetmText = ii.itm_text;
                    //}
                    //else Line2 += ";";
                    Line3 += string.Format(";{0}.{1}", ii.ans_number, ii.ans_text.Replace(';','~'));
                }
                SW.WriteLine(Line1);
                SW.WriteLine(Line2);
                SW.WriteLine(Line3);
                foreach (Test_Subject ts in ts_list)
                {
                    string Line4 = string.Format ("{0};{1};{2};{3};{4}",
                        ts.Nick_Name.Replace(';', '~'), 
                        ts.Test_Date,
                        ts.user_account != null ? ts.user_account.Company != null ? ts.user_account.Company.name.Replace(';','~') : null : null,
                        ts.user_account != null ? ts.user_account.Job != null ? ts.user_account.Job.name.Replace(';', '~') : null : null,
                        ts.Test_SubjectGroup_Link != null ? ts.Test_SubjectGroup_Link.subject_group.name.Replace(';', '~') : null);
                    Test_Result[] subj_tr =dc.Test_Results.Where(p => p.Subject_ID == ts.id).ToArray();
                    foreach (ItemInfo ii in lst)
                    {
                        if (subj_tr.Any(p => p.item_id == ii.idItem && p.SubScale_ID == ii.idAns))
                            Line4 += ";1";
                        else Line4 += ";0";
                    }
                    SW.WriteLine(Line4);
                }
            }
            SW.Close();
        }

        System.IO.FileStream fs = null;
        fs = System.IO.File.Open(sFileName, System.IO.FileMode.Open);
        byte[] btFile = new byte[fs.Length];
        fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
        fs.Close();

        return btFile;
    }

    public static int GetUSPLevel(string p_Nick, string p_Tag)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return
        dc.ExecuteQuery<int>("select COUNT(*)/10 from test_subject ts " +
            "inner join test t on t.id = ts.test_id "    +
            "inner join Test_Question tq on tq.test_id = ts.Test_Id " +
            "inner join items i on i.group_id = tq.id " +
            "where Nick_Name = {0} and t.tags like {1}",
            p_Nick, p_Tag).FirstOrDefault();
        
    }

    public static void TestSubjectRecalc(int TestID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        foreach (Test_Subject ts in dc.tests.Where(p => p.id == TestID).FirstOrDefault().Test_Subjects.Where(q=> q.BaseSubject_id == null))
        {
            dc.CalcTestValues(ts.id);
        }
    }

    public static string GetSubjGroupName(int p_GroupID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return dc.ExecuteQuery<string>("select name from subject_group where id = {0}", p_GroupID).FirstOrDefault();
    }
    public static string GetTestName(int p_TestID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return dc.ExecuteQuery<string>("select name from test where id = {0}", p_TestID).FirstOrDefault();
    }

    public static string GetSubjectName(int p_SubjectID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return dc.ExecuteQuery<string>("select nick_name from test_subject where id = {0}", p_SubjectID).FirstOrDefault();
    }

    /// <summary>
    /// определение ссылок на еще не пройденные, но похожие тесты
    /// </summary>
    public static List<NamedID> GetMoreTestLink(int p_SubjectID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return dc.ExecuteQuery<NamedID>("exec dbo.GetMoreTestFor {0}", p_SubjectID).ToList<NamedID>();
    }

    public static string getScaleName(int ScaleID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        Scale scl = dc.Scales.Where(p => p.id == ScaleID).FirstOrDefault();
        if (scl != null)
            return scl.name;
        else return null;
    }

    public static Dictionary<string,string> GetGroupCommonInfo (int GroupID)
    {
        Dictionary<string, string> res = new Dictionary<string, string>();
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();

        subject_group gr = dc.subject_groups.Where(p => p.id == GroupID).FirstOrDefault();
        res.Add("GroupName", gr.name);
        res.Add("CompanyName", gr.Company!=null ? gr.Company.name:"");

        res.Add("SubjCount", dc.Test_Subjects.Count(p => p.group_id == GroupID && p.MeasureNumber == 1 && p.Test_Date!= null).ToString());
        try
        {
            string MinDate = dc.Test_Subjects.Where(p => p.group_id == GroupID && p.Test_Date != null).Min(q => q.Test_Date).Value.ToShortDateString();
            string MaxDate = dc.Test_Subjects.Where(p => p.group_id == GroupID && p.Test_Date != null).Max(q => q.Test_Date).Value.ToShortDateString();
            //string MaxDate = dc.Test_Subjects.Max(p=> p.group_id == GroupID && p.Test_Date != null).ToString();
            //if (MinDate!=null && MaxDate!= null)
            res.Add("TestPeriod", string.Format("{0} - {1}", MinDate, MaxDate));
        }
        catch
        {
            res.Add("TestPeriod", "определить невозможно");
        }
        
        string GroupTestList="";
        foreach (Test_SubjectGroup_Link lnk in gr.Test_SubjectGroup_Links)
        {
            GroupTestList +=lnk.test.name;
        }
        res.Add("TestName", GroupTestList);

        return res;
    }
    public static string GetGroupSubjCount(int GroupID, int TestID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return dc.Test_Subjects.Count(p => p.group_id == GroupID && p.Test_Id == TestID && p.MeasureNumber==1).ToString();
    }

    public static Test_Subject GetSubjectInfo(int SubjectID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        return dc.Test_Subjects.Where(p => p.id == SubjectID).FirstOrDefault();
    }

    /// <summary>
    /// генерация набора нетипичных границ тестовых норм для шкалы
    /// </summary>
    /// <param name="ScaleID">код шкалы</param>
    public static void CreateScaleRangeNorm(int ScaleID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.ExecuteCommand("if not exists (select 1 from Scale_Norm where scale_id = {0}) " +
            "insert into Scale_Norm (normtype_id, scale_id) " +
            "select id, {0} from Norm_Type",
            ScaleID);
    }

    public static void DeleteScaleRangeNorm(int ScaleID)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.ExecuteCommand("delete from Scale_Norm where scale_id = {0}", ScaleID);
    }

    /// <summary>
    /// определение id возрастного диапазона
    /// </summary>
    public static int? GetAgeRangeID(int aParamID, short val)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        int age = DateTime.UtcNow.Year - val;

        int? result = null;
        Param_Value pv = dc.Param_Values.Where(p => p.Param_ID == aParamID && (p.ivalue_1 <= age && p.ivalue_2 >= age)).FirstOrDefault();
        if (pv != null)
            result = pv.id;
        else
        {
            byte? MinVal = dc.Param_Values.Where(p => p.Param_ID == aParamID).Min(p => p.ivalue_1).Value;
            byte? MaxVal = dc.Param_Values.Where(p => p.Param_ID == aParamID).Max(p => p.ivalue_2).Value;
            if (age < MinVal)
                result = dc.Param_Values.Where(p => p.Param_ID == aParamID && p.ivalue_1 == MinVal).FirstOrDefault().id;
            if (age > MaxVal)
                result = dc.Param_Values.Where(p => p.Param_ID == aParamID && p.ivalue_2 == MaxVal).FirstOrDefault().id;
            
        }
        return result;
    }

    /// <summary>
    /// ID первого теста для проекта группы исследования
    /// </summary>
    public static int GetGroupTestID(TesterDataClassesDataContext p_dc, int pGroupID)
    {
        if (p_dc == null) p_dc = GetNewDC();
        int id = p_dc.subject_groups.Where(p => p.id == pGroupID).FirstOrDefault().project.id;
        return (p_dc.ProjectTest_Links.Where(p => p.Project_ID == id).FirstOrDefault().Test_ID);
    }

    public class CompanyEmpCountInfo
    {
        public int StaffCount { get; set; } // работники в штате
        public int CandidateCount { get; set; } // кандидаты
        public int ProbationCount { get; set; } // испытат. срок
        public int VacancyCount { get; set; } // кол-во позиций по вакансиям
    }
    public static CompanyEmpCountInfo GetEmpCount (TesterDataClassesDataContext p_dc, int p_idCompany)
    {
        CompanyEmpCountInfo res = p_dc.ExecuteQuery<CompanyEmpCountInfo>(
            "select "+
	        "sum (case idstate when 2 then 1 when 6 then 1 else 0 end) as StaffCount, "+
	        "sum (case idstate when 1 then 1 else 0 end) as CandidateCount, "+
            "sum (case idstate when 6 then 1 else 0 end) as ProbationCount " +
            "from user_account where idcompany = {0}", new object[] {p_idCompany}).FirstOrDefault();

        res.VacancyCount = p_dc.ExecuteQuery<int>(
            "select count (*) from (select distinct iddept, idjob from user_account where idcompany = 2 and idstate = 1) q", 
            new object[] { p_idCompany }).FirstOrDefault();

        return res;
    }

    #region ScaleRate
    public class ScaleRateInfo {
         
        public string bk_color { get; set; } 
        public int? NormType {get;set;}
        public double? perc { get; set; }
        public double? avg_score { get; set; }
        public string ScaleName { get; set; }
        //public string info { get; set; }
        
        //public string get_perc_category () { 
        //    int rr = Convert.ToInt16 (perc) / 20;
        //    switch (rr)
        //    {
        //        case 0: return "оч. низкое";
        //        case 1: return "низкое";
        //        case 2: return "среднее";
        //        case 3: return "высокое";
        //        case 4: return "оч. высокое";
        //        default: return "-";
        //    }
        //}
    }
    public static ScaleRateInfo GetGroupScaleRate(TesterDataClassesDataContext pdc, int pGroupID, int pTestID, int pScaleID, int pMeasureNum,
        System.Security.Principal.IPrincipal pCurrentUser)
    {
        bool isHR = pCurrentUser.IsInRole("HR") || pCurrentUser.IsInRole("Admin");
        user_account current_ua = pdc.user_accounts.Where(p => p.idUser == GetCurrentUserKey()).FirstOrDefault();
        int? idDept = current_ua.idDept;   
        
        ScaleRateInfo res = pdc.ExecuteQuery<ScaleRateInfo>(
            "declare @testid int, @scaleid int, @groupid int, @measure int, @iddept int, @hr bit " +
            "set @testid={0} set @scaleid={1} set @groupid={2} set @measure={3} set @iddept={4} set @hr={5} " +
            "declare @avg_score float, @all int, @lower int, @bk_color nvarchar(8), @normtype int " +
            "SELECT @avg_score = avg(test_value) " +
            "from test_data td " +
            "join test_subject ts on td.subject_id=ts.id " +
            "join user_account ua on ua.iduser=ts.iduser " +
            "where ts.test_id=@testid and td.scale_id=@scaleid and group_id=@groupid and measurenumber=@measure and (ua.iddept=@iddept or @hr=1) " +
            "SELECT @all = count(*) from test_subject where test_id=@testid and test_date is not null " +
            "SELECT @lower = count(ts.id) " +
            "from test_data td " +
            "join test_subject ts on td.subject_id=ts.id " +
            "where ts.test_id=@testid and td.scale_id=@scaleid and td.test_value <= @avg_score " +

            "select top 1 @bk_color = bk_color, @normtype = sn.normtype_ID from scale_norm sn " +
            "join norm_type nt on nt.id=sn.normtype_ID "+
            "where scale_id=@scaleid and @avg_score between lowrange and highrange " +

            "select @bk_color as bk_color, @normtype as normtype, cast(@lower as float)/cast(@all as float) *100.0 as perc, " + 
            "@avg_score as avg_score, name as ScaleName from scales where id = @scaleid",
            //"select 20 rr",
            new object[] { pTestID, pScaleID, pGroupID, pMeasureNum, idDept, isHR == true ? 1:0 }).FirstOrDefault();

        // яркий светофор
        switch (res.NormType)
        {
            case 1: res.bk_color = "#FF0000"; break;
            case 2: res.bk_color = "#FFC000"; break;
            case 3: res.bk_color = "#FFD100"; break;
            case 4: res.bk_color = "#92D050"; break;
            case 5: res.bk_color = "#00B050"; break;
            default: res.bk_color = "#d3d3d3"; break;
        }

        return res;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pdc"></param>
    /// <param name="pIndicator"></param>
    /// <param name="canViewAllDept">Может ли ТЕКУЩИЙ пользователь просматривать другие отделы (т.е. имеет роль HR или Admin)</param>
    /// <returns></returns>
    public static ScaleRateInfo GetCompanyScaleRate(TesterDataClassesDataContext pdc, indicator pIndicator,
        bool? canViewAllDept
        //, System.Security.Principal.IPrincipal pCurrentUser
        )
    {
        //System.Web.Security.Membership.GetUser().
            
        //    Membership.is GetUser().is
        ////CommonData.GetCurrentUserKey();
        ////System.Security.me
        //    bool canViewAllDept = User.IsInRole("HR") || User.IsInRole("Admin");
        
        //bool isHR = pCurrentUser.IsInRole("HR") || pCurrentUser.IsInRole("Admin");

        user_account current_ua = pdc.user_accounts.Where(p => p.idUser == GetCurrentUserKey()).FirstOrDefault();
        int? idDept = current_ua.idDept;   

        ScaleRateInfo res = pdc.ExecuteQuery<ScaleRateInfo>(
            "declare @idScale int, @idtest int, @avg_score float, @bk_color nvarchar(8), @normtype int, @all int, @lower int "+
            "set @idScale = {0} "+
            "select @idtest = test_id from scales where id=@idscale "+

            ";WITH last_cte (iduser, last_date) as ( "+
            "select ts.iduser, max(ts.test_date) as last_date "+
            "from Test_Subject ts "+
            "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = {1} "+
            "where ts.test_id= @idtest and ts.test_date is not null and ua.idState in (2,6) and (ua.iddept={2} or 1={3}) "+
            "group by ts.iduser "+
            ") "+
            "SELECT @avg_score = avg(td.test_value) "+
            "from test_subject ts "+
            "inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date "+
            "inner join test_data td on td.subject_id = ts.id "+
            "where td.scale_id = @idscale "+

            "select @all = count(*) from test_subject where test_id=@idtest and test_date is not null "+
            "select @lower = count(ts.id) "+
            "from test_data td "+
            "join test_subject ts on td.subject_id=ts.id "+
            "where ts.test_id=@idtest and td.scale_id=@idscale and td.test_value <= @avg_score "+

            "select top 1 @bk_color = bk_color, @normtype = sn.normtype_ID from scale_norm sn "+
            "join norm_type nt on nt.id=sn.normtype_ID "+
            "where scale_id=@idscale and @avg_score between lowrange and highrange "+

            "select @bk_color as bk_color, @normtype as normtype, @avg_score as avg_score, "+
            "cast(@lower as float)/cast(@all as float) *100.0 as perc ",
            new object[] { pIndicator.idScale, pIndicator.idCompany, idDept, canViewAllDept == true ? 1 : 0 }).FirstOrDefault();

        // яркий светофор
        switch (res.NormType)
        {
            case 1: res.bk_color = "#FF0000"; break;
            case 2: res.bk_color = "#FFC000"; break;
            case 3: res.bk_color = "#FFD100"; break; //FFFF00
            case 4: res.bk_color = "#92D050"; break;
            case 5: res.bk_color = "#00B050"; break;
            default: res.bk_color = "#d3d3d3"; break;
        }

        return res;
    }
    public static ScaleRateInfo GetSubjectScaleRate(TesterDataClassesDataContext pdc, int pSujectID, int pScaleID)
    {
        ScaleRateInfo res = pdc.ExecuteQuery<ScaleRateInfo>(
            "declare @testid int, @scaleid int, @subjectid int " +
            "set @subjectid={0} set @scaleid={1}  " +
            "declare @score float, @all int, @lower int, @bk_color nvarchar(8), @normtype int " +
            
            "select @score = test_value, @testid= ts.test_id " +
            "from test_data td " +
            "inner join test_subject ts on ts.id=td.subject_id " +
            "where td.subject_id=@subjectid and td.scale_id=@scaleid " +
            
            "select @all = count(*) from test_subject where test_id=@testid and test_date is not null " +
            
            "select @lower = count(ts.id) " +
            "from test_data td " +
            "join test_subject ts on td.subject_id=ts.id " +
            "where ts.test_id=@testid and td.scale_id=@scaleid and td.test_value <= @score " +

            "select top 1 @bk_color = bk_color, @normtype = sn.normtype_ID from scale_norm sn " +
            "join norm_type nt on nt.id=sn.normtype_ID " +
            "where scale_id=@scaleid and @score between lowrange and highrange " +

            "select @bk_color as bk_color, @normtype as normtype, " +
            "case when @all = 0 then 0 else cast(@lower as float)/cast(@all as float) *100.0 end as perc, " +
            "@score as avg_score",
            new object[] { pSujectID, pScaleID}).FirstOrDefault();

        // яркий светофор
        switch (res.NormType)
        {
            case 1: res.bk_color = "#FF0000"; break;
            case 2: res.bk_color = "#FFC000"; break;
            case 3: res.bk_color = "#FFD100"; break; //FFFF00
            case 4: res.bk_color = "#92D050"; break;
            case 5: res.bk_color = "#00B050"; break;
            default: res.bk_color = "#d3d3d3"; break;
        }

        return res;
    }
    #endregion ScaleRate

    public static Guid GetCurrentUserKey () 
    {
        return (Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
    }

    /// <summary>
    /// создание пустого дефолтного исследования
    /// </summary>
    /// <returns>код созданного ислледования</returns>
    public static int CreateNewResearch()
    {
        const string MailTemplate =
            "Добрый день! Приглашаем Вас пройти исследование '#name#' " +
            "Тестирование будет доступно для Вас до #enddate#. " +
            "Чтобы начать исследование нажмите на эту ссылку #link# " +
            " "+
            "С уважением, " +
            "Система оценки и развития сотрудника ulleum.com";

        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            subject_group sg = new subject_group()
            {
                name = "Новое исследование",
                UserKey = GetCurrentUserKey(),
                create_date = DateTime.Now,
                mail_template = MailTemplate,
                idCompany = dc.user_accounts.Where(p => p.idUser == GetCurrentUserKey()).FirstOrDefault().idCompany,
                start_date = DateTime.Today,
                idReport = 1 // стандартный групповой отчет
            };
            dc.subject_groups.InsertOnSubmit(sg);
            dc.SubmitChanges();
            return sg.id;
        }
    }

    public static user_account CreateNewUserAccount(string aNickName, int aCompanyID, TesterDataClassesDataContext dc)
    {
        user_account UAcc = dc.user_accounts.Where(q => q.login_name == aNickName).FirstOrDefault();
        if (UAcc != null)
            return UAcc; // учетка уже заведена
        else
        { // заводим новую учетку
            System.Web.Security.MembershipCreateStatus state;
            System.Web.Security.MembershipUser NewUser = System.Web.Security.Membership.CreateUser(aNickName, "12345678!", aNickName, "qwerty", "123456", true, out state);
            //NewUser.ProviderUserKey
            if (state == System.Web.Security.MembershipCreateStatus.Success)
            {
                user_account NewAcc = new user_account();
                NewAcc.idUser = Guid.Parse(NewUser.ProviderUserKey.ToString());
                NewAcc.login_name = aNickName;
                NewAcc.idCompany = aCompanyID;
                dc.user_accounts.InsertOnSubmit(NewAcc);
                return (NewAcc);
            }
            else
                return null;
        }
    }
    /// <summary>
    /// существуют ли результаты прохождения по данному инструменту
    /// </summary>
    /// <param name="idTest">код инструмента (теста)</param>
    /// <returns>существуют или нет</returns>
    public static bool ExistTestResults(int idTest, TesterDataClassesDataContext p_dc)
    {
        TesterDataClassesDataContext fdc = p_dc == null ? new TesterDataClassesDataContext() : p_dc;
        if (fdc.Test_Results.Any(p => p.item.Test_Question.test_id == idTest))
            return true;
        else
            return false;
    }

    //public static System.Web.Security.MembershipUser  ()
    //{
    //    System.Web.Security.MembershipUser usr;
    //    if (HttpContext.Current.User.Identity.IsAuthenticated)
    //    {
    //        //string UserName = HttpContext.Current.User.Identity.Name;
    //        usr = System.Web.Security.Membership.GetUser();
    //    }



    //    System.Security.Principal.
    //    System.Web.Security.MembershipUser usr = System.Web.Security.Membership.GetUser(User.Identity.Name);
    //    return (Guid)usr.ProviderUserKey; 
    //} }
    //}
    
    //struct res2v { public string aa; public string bb;}
    public static string LastTextAnswer(TesterDataClassesDataContext p_dc, int p_idGroup, string p_FIO)
    {
        string LastTextAns = "...";
        try
        {
            // определяем самую позднюю благодарность
            var maxID = (
                        from lnk in p_dc.Test_SubjectGroup_Links
                        join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
                        join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
                        join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
                        where lnk.idGroup == p_idGroup && tr.SelectedValue == 1 && ss.name == p_FIO
                        select tr.id).Max();

            LastTextAns = (
                from tr in p_dc.Test_Results
                join txt in p_dc.Test_Results_Txts on tr.Subject_ID equals txt.subject_id
                join ts in p_dc.Test_Subjects on tr.Subject_ID equals ts.id
                where tr.id == maxID
                select string.Format("{0} [{1}]", txt.text, ts.fio)).FirstOrDefault();
        }
        catch { }
        return LastTextAns;
    }
}

public class NamedID
{
    public int id;
    public string name;
    public NamedID() { }
}