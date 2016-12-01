using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Drawing.Imaging;
using System.IO;

/// <summary>
/// класс для работы с диаграммами
/// </summary>
public class TesterChart
{
	public TesterChart()
	{
		//
		// TODO: Add constructor logic here
		//
    }

    public static Chart DrawBubbleDiagram(TesterDataClassesDataContext p_dc, test_diagram p_TestDiagram, int p_Width,
        int? p_GroupID, int? p_MeasureNumber)
    {
        Chart dgr = new Chart() { Width = p_Width, BackColor = Color.Transparent };
        Series s0 = new Series() { ChartType = SeriesChartType.Bubble, IsXValueIndexed = false };
        int x_pos = 0;
        foreach (Test_Diagram_Scale tds in p_TestDiagram.Test_Diagram_Scales.OrderBy(p => p.OrderNumber))
        {
            //Test_Data td;
            x_pos++;
            IQueryable<Test_Data> iqtd = p_dc.Test_Datas.Where(p => p.Test_Subject.group_id == p_GroupID && p.Scale_ID == tds.scale_id && p.Test_Subject.MeasureNumber == p_MeasureNumber);
            if (iqtd.Any())
            {
                decimal sum_val = iqtd.Sum(q => q.Test_Value);
                s0.Points.Add(new DataPoint(x_pos, new double[2] {1, (double)sum_val}) {AxisLabel = tds.Scale.abreviature });
            }
        }
        
        dgr.Series.Add(s0);
        return dgr;
    }

    /// <summary>
    /// столбчатая диаграмма (по отделам/должностям)
    /// </summary>
    public static Chart DrawBarChart(int p_width, int p_MaxValue)
    {
        Chart dgr = new Chart() { Width = p_width, BackColor = Color.Transparent };
        Series sr = NewBarSeries ("bar");
        sr.Color = Color.LightSeaGreen;

        dgr.Series.Add(sr);
        ChartArea area = new ChartArea("chart area") { BackColor = Color.Transparent };
        area.AxisX.MajorGrid.Enabled = false;
        area.AxisY.MajorGrid.LineColor = Color.LightGray;
        dgr.ChartAreas.Add(area);
        dgr.ChartAreas[0].AxisY.Maximum = p_MaxValue;

        return dgr;
    }

    private static Series NewBarSeries(string p_name)
    {
        return new Series(p_name)
        {
            IsValueShownAsLabel = true,
            Color = System.Drawing.Color.FromArgb(
                int.Parse("15", System.Globalization.NumberStyles.AllowHexSpecifier),
                int.Parse("35", System.Globalization.NumberStyles.AllowHexSpecifier),
                int.Parse("55", System.Globalization.NumberStyles.AllowHexSpecifier)
            ),

            IsXValueIndexed = false,
            ChartType = SeriesChartType.Column,
            LabelFormat = "{0}"
        };
    }
   
    private static Chart DrawDiagram(TesterDataClassesDataContext p_dc, test_diagram p_TestDiagram, int p_Width, 
        Test_Subject p_Subject, Test_Subject p_Subject2, 
        int? p_GroupID, int? p_MeasureNumber,
        List<int> p_SubjIDList
        )
    {
        Chart dgr;
        if (p_TestDiagram.diagram_type == 4)
        {
            // надо по респонденту конкретному !!
            dgr = DrawBubbleDiagram(p_dc, p_TestDiagram, p_Width, p_GroupID, p_MeasureNumber);
        }
        else
        {
            dgr = new Chart() { Width = p_Width, BackColor = Color.Transparent };
            dgr.Titles.Add(new Title() { Font = new System.Drawing.Font("Verdana", 10, FontStyle.Italic), Text = p_TestDiagram.name });
            //dgr.Titles.Add(new Title(p_TestDiagram.name, null, new System.Drawing.Font("Arial", 10.0), System.Drawing.Color.Black));

            Series s0 = NewBarSeries (p_Subject != null ? p_Subject.fio != null ? p_Subject.fio : p_Subject.Nick_Name : "Среднее по группе");

            Series s_Low = new Series("Нижн. норма");
            s_Low.IsXValueIndexed = false;
            s_Low.ChartType = SeriesChartType.Line;
            s_Low.BorderDashStyle = ChartDashStyle.Dash;
            s_Low.Color = System.Drawing.Color.Red;

            Series s_High = new Series("Верхн. норма") { BorderDashStyle = ChartDashStyle.Dash };
            //s_High.LegendText = "";
            s_High.IsXValueIndexed = false;
            s_High.ChartType = SeriesChartType.Line;
            s_High.Color = System.Drawing.Color.Red;
            ///TODO ширину линии побольше сделать

            Series s2 = new Series(" ") { IsValueShownAsLabel = true, LabelFormat = "{0}" }; // "сравнить с"
            if (p_Subject != null && p_Subject2 != null)
                s2.LegendText = p_Subject2.Nick_Name;
            else
                if (p_GroupID != null && p_MeasureNumber == null)
                    s2.LegendText = "второй замер";

            int x_pos = 0;
            foreach (Test_Diagram_Scale tds in p_TestDiagram.Test_Diagram_Scales.OrderBy(p => p.OrderNumber))
            {
                Test_Data td;  //в базовую функцию передавать уже готвый набор(ы)  IQueryable<Test_Data>
                x_pos++;
                if (p_GroupID == null && p_SubjIDList == null) // по респонденту
                {
                    td = p_dc.Test_Datas.Where(p => p.Subject_ID == p_Subject.id && p.Scale_ID == tds.scale_id).FirstOrDefault();
                    if (td != null)
                        s0.Points.Add(new DataPoint(x_pos, (double)td.Test_Value) { AxisLabel = tds.Scale.abreviature });

                    if (p_Subject2 != null)
                    {
                        Test_Data td2 = p_Subject2.Test_Datas.Where(p => p.Scale_ID == tds.scale_id).FirstOrDefault();
                        if (td2 != null)
                            s2.Points.Add(new DataPoint(x_pos, (double)td2.Test_Value) { AxisLabel = tds.Scale.abreviature });
                    }
                }
                else // по группе
                {
                    if (p_GroupID != null)
                    {
                        if (p_MeasureNumber != null) // по заданному замеру
                        {
                            IQueryable<Test_Data> iqtd = p_dc.Test_Datas.Where(p => p.Test_Subject.group_id == p_GroupID && p.Scale_ID == tds.scale_id && p.Test_Subject.MeasureNumber == p_MeasureNumber);
                            if (iqtd.Any())
                            {
                                decimal avg_val = iqtd.Average(q => q.Test_Value);
                                s0.Points.Add(new DataPoint(x_pos, (double)avg_val) { AxisLabel = tds.Scale.abreviature });
                            }
                        }
                        else // сравнение двух замеров
                        {
                            IQueryable<Test_Data> iqtd = p_dc.Test_Datas.Where(p => p.Test_Subject.group_id == p_GroupID && p.Scale_ID == tds.scale_id && p.Test_Subject.MeasureNumber == 1);
                            if (iqtd.Any())
                            {
                                decimal avg_val = iqtd.Average(q => q.Test_Value);
                                s0.Points.Add(new DataPoint(x_pos, (double)avg_val) { AxisLabel = tds.Scale.abreviature });
                            }

                            iqtd = p_dc.Test_Datas.Where(p => p.Test_Subject.group_id == p_GroupID && p.Scale_ID == tds.scale_id && p.Test_Subject.MeasureNumber == 2);
                            if (iqtd.Any())
                            {
                                decimal avg_val = iqtd.Average(q => q.Test_Value);
                                s2.Points.Add(new DataPoint(x_pos, (double)avg_val) { AxisLabel = tds.Scale.abreviature });
                            }
                        }
                    }
                    else
                        if (p_SubjIDList != null) // по зоне мониторинга
                        {
                            IQueryable<Test_Data> iqtd = p_dc.Test_Datas.Where(p => p_SubjIDList.Contains(p.Subject_ID) && p.Scale_ID == tds.scale_id);
                            if (iqtd.Any())
                            {
                                decimal avg_val = iqtd.Average(q => q.Test_Value);
                                s0.Points.Add(new DataPoint(x_pos, (double)avg_val) { AxisLabel = tds.Scale.abreviature });
                            }
                        }
                }

                s_Low.Points.AddXY(x_pos, tds.Scale.AVG_Value - tds.Scale.Standard_Dev);
                s_High.Points.AddXY(x_pos, tds.Scale.AVG_Value + tds.Scale.Standard_Dev);
            }

            dgr.Series.Add(s0);
            if (p_Subject2 != null || (p_Subject == null && p_MeasureNumber == null))
                dgr.Series.Add(s2);

            dgr.Series.Add(s_Low);
            dgr.Series.Add(s_High);

            dgr.ChartAreas.Add(new ChartArea("chart area")
            {
                BackColor = System.Drawing.Color.Transparent
                //FromArgb(
                //    int.Parse("CC", System.Globalization.NumberStyles.AllowHexSpecifier),
                //    int.Parse("CC", System.Globalization.NumberStyles.AllowHexSpecifier),
                //    int.Parse("FF", System.Globalization.NumberStyles.AllowHexSpecifier)
                //)
            });
            dgr.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //dgr.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            dgr.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;

            double MaxYValue = Convert.ToDouble(p_TestDiagram.Test_Diagram_Scales.Max(p => p.Scale.ScoreCalcType == 3 ? 100 : p.Scale.max_value));
            double MinYValue = Convert.ToDouble(p_TestDiagram.Test_Diagram_Scales.Min(p => p.Scale.ScoreCalcType == 3 ? p.Scale.min_value / p.Scale.max_value * (decimal)100.0 : p.Scale.min_value));
            if (MaxYValue != 0)
                dgr.ChartAreas[0].AxisY.Maximum = MaxYValue;

            if (MinYValue != 0)
                dgr.ChartAreas[0].AxisY.Minimum = MinYValue;

            dgr.Legends.Add(new Legend());
            //dgr.Legends.Add(new Legend()); // вторая легенда со шкалами и набранными баломи
        }

        return dgr;
 	}
    /// <summary>
    /// по группе
    /// </summary>
    public static Chart DrawDiagram(TesterDataClassesDataContext p_dc, test_diagram p_TestDiagram, int p_Width, int p_GroupID, int? p_MeasureNumber)
    {
        if (p_dc == null) p_dc = new TesterDataClassesDataContext();
        return DrawDiagram(p_dc, p_TestDiagram, p_Width, null, null, p_GroupID, p_MeasureNumber, null);
    }
    /// <summary>
    /// по респонденту(ам)
    /// </summary>
    public static Chart DrawDiagram(TesterDataClassesDataContext p_dc, test_diagram p_TestDiagram, int p_Width, Test_Subject p_Subject,
        Test_Subject p_Subject2)
    {
        return DrawDiagram(p_dc, p_TestDiagram, p_Width, p_Subject, p_Subject2, null, 1, null);
    }
    /// <summary>
    /// по зоне мониторинга /набору респондентов
    /// </summary>
    public static Chart DrawDiagram(TesterDataClassesDataContext p_dc, test_diagram p_TestDiagram, int p_Width,
        List<int> p_SubjIDList)
    {
        return DrawDiagram(p_dc, p_TestDiagram, p_Width, null, null, null, 1, p_SubjIDList);
    }

    /// <summary>
    /// заполнение пузырьковой диаграммы данными
    /// </summary>
    public static void FillBubblesDiagram (Chart p_Diagram, Panel p_ScalesInfoPanel, DataRowCollection p_rows)
    {
        foreach (DataRow row in p_rows)
        {
            int idx = p_Diagram.Series[0].Points.AddXY(row["abreviature"], 0, row["avg_val"]);
            if (Convert.ToInt16(row["avg_val"]) >= 5)
                p_Diagram.Series[0].Points[idx].Color = Color.IndianRed;
            else
                if (Convert.ToInt16(row["avg_val"]) <= 1)
                    p_Diagram.Series[0].Points[idx].Color = Color.LightGray;
                else
                    p_Diagram.Series[0].Points[idx].Color = Color.LightBlue;

            p_Diagram.Series[0].Points[idx].Label = Convert.ToInt16(row["avg_val"]).ToString();
            p_Diagram.Series[0].Points[idx].ToolTip = row["name"].ToString();

            // расшифровка снизу
            p_ScalesInfoPanel.Controls.Add(new LiteralControl(
                string.Format("<br/><b>{0} - {1}</b><br/>{2}<br>",
                new object[] { row["abreviature"], row["name"], row["descript"] })));

        }
    }

    public const string qryDiagram_composite =
        "with last_cte (iduser, last_date) as ( " +
        "select ts.iduser, max(ts.test_date) as last_date " +
        "from Test_Subject ts " +
        "inner join user_account ua on ua.iduser = ts.iduser and ua.idcompany = @idCompany " +
        "where ts.test_id= @TestID and ts.test_date is not null and ua.idState in (2,6)  and (ua.iddept = @idDept or @HR = 1) " +
        "group by ts.iduser)  " +
        "select NormType_id, " +
        "nt.name+' ('+cast(cast(sn.lowrange as decimal(4,0)) as varchar(7))+'-'+cast(cast(sn.highrange as decimal(4,0)) as varchar(7)) +')' as name, " +
        "COUNT(*) subj_count " +
        "from Test_Subject ts " +
        "inner join last_cte on ts.iduser = last_cte.iduser and ts.test_date = last_cte.last_date  " +
        "inner join Test_Data td on td.Subject_ID = ts.id " +
        "inner join Scale_Norm sn on td.Scale_ID =sn.Scale_ID and td.Test_Value between sn.LowRange and sn.HighRange " +
        "inner join Norm_Type nt on nt.id = sn.NormType_id " +
        "where td.Scale_ID = @ScaleID and measurenumber=1 " +
        "group by NormType_id, nt.name, sn.lowrange, sn.highrange";

	public static void FillSmallDoughnut (Chart p_Diagram, int? p_idCompany, Scale p_Scale, string p_header, int? p_idDept, bool p_IsHR)
	{
        p_Diagram.ChartAreas.Add(new ChartArea() { BackColor = Color.Transparent });
        p_Diagram.Height = 400;
        p_Diagram.Width = 680;
        p_Diagram.Titles.Add(p_header);

        Series sr = p_Diagram.Series.Add("series_1");
        sr.ChartType = SeriesChartType.Doughnut;
        sr.XValueMember = "name";
        sr.YValueMembers = "subj_count";
        sr.IsValueShownAsLabel = true; // XValueIndexed = false;
        sr.CustomProperties = "PieLabelStyle=Outside";

        SqlDataSource sql = new SqlDataSource(
                            System.Configuration.ConfigurationManager.ConnectionStrings["tester_dataConnectionString"].ConnectionString,
                            qryDiagram_composite);
        sql.SelectParameters.Add("ScaleID", p_Scale.id.ToString());
        sql.SelectParameters.Add("idCompany", p_idCompany.ToString());
        sql.SelectParameters.Add("TestID", p_Scale.test_id.ToString());
        sql.SelectParameters.Add("idDept", p_idDept.ToString());
        sql.SelectParameters.Add("HR", p_IsHR == true ? "1" : "0");

        p_Diagram.DataSource = sql; 
        //p_Diagram.DataBind();

	}
    /// <summary>
    /// отрисовка индикатора по шкале
    /// </summary>
    public static Label DrawScaleIndicator(CommonData.ScaleRateInfo p_ScaleInfo, string p_ImageFolderPath, System.Web.UI.WebControls.Image p_image)
    {
        Label rez = new Label();
        if (p_ScaleInfo.avg_score != 0)
        {        
            Bitmap Indicator = new Bitmap(120, 100);
            Graphics gr = Graphics.FromImage(Indicator);
            //-------------------------------------------------------------------------//
            Brush b1 = new SolidBrush(ColorTranslator.FromHtml("#FFC000"));
            gr.FillEllipse(b1, new Rectangle(10, 0, 100, 100));
            string InnerText = Convert.ToInt32 (p_ScaleInfo.avg_score).ToString();

            rez.Text = InnerText;
            rez.ForeColor = ColorTranslator.FromHtml("#FFC000");

            Brush b2 = new SolidBrush(Color.White);
            float FntSize = 24;
            gr.DrawString(InnerText, new Font("Arial", FntSize), b2, new PointF(40, 40));
            //-------------------------------------------------------------------------//
            if (!Directory.Exists(p_ImageFolderPath))
                Directory.CreateDirectory(p_ImageFolderPath);

            DeleteOldImages(p_ImageFolderPath);

            string FileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            Indicator.Save(string.Format("{0}\\ind_{1}.png", p_ImageFolderPath, FileName), ImageFormat.Png);
            // может имена файлов должны быть уникальными (рандом). иначе может быть кэширование картинок и нерелевантные значение
            p_image.ImageUrl = string.Format("Images\\Indicator\\ind_{1}.png", p_ImageFolderPath, FileName);
        }
        return rez;
    }
    /// <summary>
    /// вычисление значений и отрисовка индикатора (image)
    /// </summary>
    public static Label CalcAndDrawIndicator(TesterDataClassesDataContext p_dc, System.Web.UI.WebControls.Image p_image, 
        string p_ImageFolderPath, indicator p_indicator, Guid p_idUser, bool? canViewAllDept)
    {
        Label rez = new Label();
        Bitmap Indicator = new Bitmap(120, 100);
        Graphics gr = Graphics.FromImage(Indicator);

        CommonData.ScaleRateInfo sss = new CommonData.ScaleRateInfo() {bk_color = "#d3d3d3", perc = 0, avg_score = 0 };
        string InnerText = "";
        switch (p_indicator.idType)
        {
            case 80: // счетчик прохождений
                Brush b1 = new SolidBrush(ColorTranslator.FromHtml("#FFC000"));
                gr.FillEllipse(b1, new Rectangle(10, 0, 100, 100));
                int cnt = (
                    from ts in p_dc.Test_Subjects
                    where ts.group_id == p_indicator.idGroup && ts.Test_Date != null && ts.idUser == p_idUser
                    select ts.id).Count();
                InnerText = cnt.ToString();
                sss.avg_score = cnt; //для совместимости
                break;
            case 100: // счетчик упоминаний ("благодарности")
                Brush b100 = new SolidBrush(ColorTranslator.FromHtml("#FFC000"));
                gr.FillEllipse(b100, new Rectangle(10, 0, 100, 100));
                user_account ua = p_dc.user_accounts.Where (
                    p=> p.idUser == p_idUser
                    ).FirstOrDefault();

                int link_count = (
                    from lnk in p_dc.Test_SubjectGroup_Links
                    join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
                    join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
                    join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
                    where lnk.idGroup == p_indicator.idGroup && tr.SelectedValue == 1 && ss.name == ua.fio
                    select tr.id).Count();

                InnerText = link_count.ToString();
                sss.avg_score = link_count;
                //sss.info = "dfdfdfd";
                break;
            default:
                if (!p_indicator.isPersonal)
                    sss = CommonData.GetCompanyScaleRate(p_dc, p_indicator, canViewAllDept);
                else
                {
                    if (p_indicator.idScale != null && p_indicator.idGroup != null)
                    {
                        Test_Subject ts = p_indicator.subject_group.Test_Subjects.Where(p => p.idUser == p_idUser)
                            .OrderByDescending(o=> o.Test_Date).FirstOrDefault();
                        if (ts != null)
                        {
                            int idSubj = ts.id;
                            sss = CommonData.GetSubjectScaleRate(p_dc, idSubj, Convert.ToInt32(p_indicator.idScale));
                        }
                    }
                }

                Brush b22 = new SolidBrush(ColorTranslator.FromHtml(sss.bk_color));
                gr.FillEllipse(b22, new Rectangle(10, 0, 100, 100));
                if (sss.avg_score != null)
                    InnerText = Math.Round((decimal)sss.avg_score).ToString();
                else InnerText = "0";

                break;
        }

        Brush b2 = new SolidBrush(Color.White);
        //gr.DrawString(sss.perc.ToString()+" %", new Font("Arial", 16), b2, new RectangleF (0,60, 100, 30));
        float FntSize = p_indicator.idType == 80 ? 24 : 16;
        gr.DrawString(InnerText, new Font("Arial", FntSize), b2, new PointF(50, 40));

        if (p_indicator.idType != 80)
        {
            Norm_Type nt = p_dc.Norm_Types.Where(p => p.id == sss.NormType).FirstOrDefault();
            if (nt != null)
            {
                gr.DrawString(nt.name, new Font("Arial", 10), b2, new PointF(nt.name.Length > 10 ? 20 : 26, 60));
                InnerText = InnerText + " (" + nt.name + ")";
            }

        }

        if (p_ImageFolderPath != null)
        {
            if (!Directory.Exists(p_ImageFolderPath))
                Directory.CreateDirectory(p_ImageFolderPath);
            DeleteOldImages(p_ImageFolderPath);

            if (sss.avg_score != 0 || p_indicator.idType == 100)
            {
                string FileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                Indicator.Save(string.Format("{0}\\ind_{1}.png", p_ImageFolderPath, FileName), ImageFormat.Png);
                // может имена файлов должны быть уникальными (рандом). иначе может быть кэширование картинок и нерелевантные значение
                p_image.ImageUrl = string.Format("Images\\Indicator\\ind_{1}.png", p_ImageFolderPath, FileName);
            }
        }

        rez.Text = InnerText;
        rez.Font.Size = 34;
        rez.ForeColor = ColorTranslator.FromHtml(sss.bk_color);
        return rez;
    }

    struct str2 { public string text; public short? number; public DateTime? dt;}
    public static void PrintAchivements(TesterDataClassesDataContext p_dc, indicator p_ind, string p_fio, Control p_ctrl )
    {
        p_ctrl.Controls.Add(new Label() { Text = p_ind.name, CssClass = "clsIndicatorName" });
        p_ctrl.Controls.Add(new LiteralControl("<br/>"));
        foreach (str2 line in
            (from lnk in p_dc.Test_SubjectGroup_Links
             join ssd in p_dc.SubScaleDimensions on lnk.idTest equals ssd.test_id
             join ss in p_dc.SubScales on ssd.id equals ss.Dimension_ID
             join tr in p_dc.Test_Results on ss.id equals tr.SubScale_ID
             join txt in p_dc.Test_Results_Txts on tr.Subject_ID equals txt.subject_id
             join i in p_dc.items on txt.item_id equals i.id
             join ts in p_dc.Test_Subjects on txt.subject_id equals ts.id
             where lnk.idGroup == p_ind.idGroup && tr.SelectedValue == 1 && ss.name == p_fio
             select new str2() { text = txt.text, number = i.number, dt = ts.Test_Date }).OrderBy(o => o.dt).ThenBy(t=> t.number))
        {
            if (line.text != null)
            {
                p_ctrl.Controls.Add(new Label() { Text = line.text, CssClass = "clsAchivementLabel" });
                p_ctrl.Controls.Add(new LiteralControl("<br/>"));
            }
        }
    }

    private static void DeleteOldImages(string p_ImageFolderPath)
    {
        DirectoryInfo di = new DirectoryInfo(p_ImageFolderPath);
        DateTime yest = DateTime.Today.AddDays (-1);

        foreach (FileSystemInfo fsi in di.GetFileSystemInfos ("ind_*.png").Where (p=> p.CreationTime <  yest))
        {
            fsi.Delete();
        }
   
    }

    static ColorConverter cnv = new ColorConverter();

    public static void SetSectorColor(System.Web.UI.DataVisualization.Charting.DataPointCollection p_Points)
    {
        if (p_Points.Count > 0)
        {
            foreach (System.Web.UI.DataVisualization.Charting.DataPoint pnt in p_Points)
            {
                // яркий светофор
                if (pnt.AxisLabel.StartsWith("очень низкое")) pnt.Color = (Color)cnv.ConvertFromString("#FF0000");
                else if (pnt.AxisLabel.StartsWith("низкое")) pnt.Color = (Color)cnv.ConvertFromString("#FFC000");
                else if (pnt.AxisLabel.StartsWith("нормальное")) pnt.Color = (Color)cnv.ConvertFromString("#FFFF00");
                else if (pnt.AxisLabel.StartsWith("высокое")) pnt.Color = (Color)cnv.ConvertFromString("#92D050");
                else if (pnt.AxisLabel.StartsWith("очень высокое")) pnt.Color = (Color)cnv.ConvertFromString("#00B050");

            }
        }
    }


}