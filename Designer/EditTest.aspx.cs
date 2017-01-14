using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class Designer_EditTest : System.Web.UI.Page
{
    int TestID { get { return Convert.ToInt16(Request.QueryString["TestID"]); } }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.Cookies["tabIndex"] != null)
            {
                int index = int.Parse(Server.HtmlEncode(Request.Cookies["tabIndex"].Value));
                if (index > -1 && index < Tabs.Tabs.Count)
                {
                    Tabs.ActiveTabIndex = index;
                }
                else
                {
                    Tabs.ActiveTabIndex = 0;
                }
            }

            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                test tt = dc.tests.Where(tst => tst.id == TestID).FirstOrDefault();
                if (tt != null)
                {
                    cboxIsSinglePage.Checked = tt.isSinglePage;
                }
            }
        }
    }

    public void SaveProfile(object sender, EventArgs e)
    {
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //ItemLinqDataSource.DataBind();
        //Response.Redirect(Request.RawUrl); // хм, нужно просто чтобы итемы обновились в зависимости от блока.
    }
    protected void BlockLinqDataSource_Selected(object sender, LinqDataSourceStatusEventArgs e)
    {
        
    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        if (CommonData.ExistTestResults(TestID, null))
            CustomValidator1.IsValid = false;
        else
            ItemGridView.ShowFooter = true;

        // http://www.codeproject.com/Articles/417693/Insert-Update-Delete-in-ASP-NET-Gridview-DataSourc
        // http://www.codeproject.com/Articles/12291/ASP-NET-GridView-Add-a-new-record

        // http://forums.asp.net/t/889109.aspx/1

        // футер при пустом датасет
        // http://www.gotdotnet.ru/forums/4/46018/
        // http://mattberseth.com/blog/2007/07/how_to_show_header_and_footer.html
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Designer/TestBlocks.aspx?TestID=" + TestID.ToString());
    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            if (CommonData.ExistTestResults(TestID, dc))
                CustomValidator1.IsValid = false;
            else
            {
                string[] pss = boxBuffer.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                short i = 1;
                string txt_255 = "";
                foreach (string s in pss)
                {
                    if (s.Length > 255)
                        txt_255 = s.Substring(0, 254);
                    else
                        txt_255 = s;

                    dc.items.InsertOnSubmit(new item() { text = txt_255, group_id = Convert.ToInt16(BlockDropDownList.SelectedValue), item_type = 1, number = i });
                    i++;
                }
                dc.SubmitChanges();
                ItemGridView.DataBind();
            }
        }
    }

    protected void btnAddResumeItem_Click(object sender, EventArgs e)
    {
        if (ResumeGridView.Rows.Count == 0)
        {
            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            Resume_Item ri = new Resume_Item() { test_id = TestID, Resume_Item_Type = 1, OrderNumber = 1, item_text = "место для текста блока резюме" };
            dc.Resume_Items.InsertOnSubmit(ri);
            dc.SubmitChanges();
            ResumeGridView.EditIndex = 0;
        }
        else
        {
            ResumeGridView.ShowFooter = true;
        }
    }

    protected void ItemGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (CommonData.ExistTestResults(TestID, null))
        {
            CustomValidator1.IsValid = false;
            e.Cancel = true;
        }
    }
    protected void ItemGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (CommonData.ExistTestResults (TestID, null))
        {
            CustomValidator1.IsValid = false;
            e.Cancel = true;
        }
    }
    protected void CommonGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    { 
        if (e.CommandName.Equals("Insert"))
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                switch (((Control)sender).ID)
                {
                    case "ResumeGridView":
                        string itm_txt = ((TextBox)((GridView)sender).FooterRow.FindControl("tbItem_Text")).Text;
                        byte itm_type = Convert.ToByte(((DropDownList)((GridView)sender).FooterRow.FindControl("ddlItem_Type")).SelectedValue);
                        Resume_Item ri = new Resume_Item() { test_id = TestID, item_text = itm_txt, Resume_Item_Type = itm_type };
                        dc.Resume_Items.InsertOnSubmit(ri);
                        break;
                    case "ItemGridView":
                        TextBox itmText = ((GridView)sender).FooterRow.FindControl("TextBoxItemText") as TextBox;
                        TextBox itmNumber = ((GridView)sender).FooterRow.FindControl("TextBoxItemNumber") as TextBox;
                        TextBox itmDescr = ((GridView)sender).FooterRow.FindControl("TextBoxDescrText") as TextBox;
                        item itm = new item()
                        {
                            text = itmText.Text,
                            number = Convert.ToByte(itmNumber.Text),
                            group_id = Convert.ToInt16(BlockDropDownList.SelectedValue),
                            item_type = 1,
                            description = itmDescr.Text
                        };
                        dc.items.InsertOnSubmit(itm);
                        break;
                    case "ScalesGridView": break;
                }
                dc.SubmitChanges();
                ((GridView)sender).ShowFooter = false;
            }
        }
        else
            if (e.CommandName.Equals("InsertCancel"))
                ((GridView)sender).ShowFooter = false;
    }

    protected void btnNewDiagram_Click(object sender, EventArgs e)
    {
        //if (DiagramGridView.Rows.Count == 0)
        //{
            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            test_diagram dgr = new test_diagram() { name = "Новая диаграмма", test_id = TestID, diagram_type = 1 };
            dc.test_diagrams.InsertOnSubmit(dgr);
            dc.SubmitChanges();
            DiagramGridView.DataBind();
            //DiagramGridView.EditIndex = 0;
        //}
        //else
        //    DiagramGridView.ShowFooter = true;
    }
    protected void btnNewScale_Click(object sender, EventArgs e)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.tests.Where(p => p.id == TestID).First().Scales.Add(new Scale() { name = "Новая шкала", ScoreCalcType = 2 });
        dc.SubmitChanges();
        ScalesGridView.DataBind();
        
        //if (ScalesGridView.Rows.Count == 0)
        //{
        //    TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        //    Scale s = new Scale() {name = "введите наименование шкалы",  test_id = TestID, ScoreCalcType=2};
        //    //Scale s = new Scale() { test_id = TestID };
        //    dc.Scales.InsertOnSubmit(s);
        //    dc.SubmitChanges();
        //    //ScaleLinqDataSource.DataBind();

        //    //List<Scale> sl = new List<Scale>();
        //    //sl.Add(s);
        //    //ScalesGridView.DataSource = sl;
        //    //ScalesGridView.DataSourceID = null;
        //    //ScalesGridView.DataBind();


        //    //System.Collections.Specialized.ListDictionary ListDict = new System.Collections.Specialized.ListDictionary();
        //    //ListDict.Add("name", "abc");
        //    //ListDict.Add("abreviature", "def");
            
        //    //ScaleLinqDataSource.Insert(ListDict);
        //    ScalesGridView.EditIndex = 0;
        //    //GridViewRow emptyRow = base.cre CreateRo
        //}
        //else
        //    ScalesGridView.ShowFooter = true;


                        //        <EmptyDataTemplate>
                        
                        //    <asp:TextBox ID="tbName1" runat="server"></asp:TextBox>
                        //    <asp:TextBox ID="tbAbbr1" runat="server"></asp:TextBox>
                        //    <asp:LinkButton ID="LinkButton11" runat="server" CausesValidation="True" OnClick="btnScaleIns_Click"
                        //                Text="Insert"></asp:LinkButton>
                        //</EmptyDataTemplate>

    }
    protected void ScalesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox tbName = ScalesGridView.FooterRow.FindControl("tbName") as TextBox;

            //TextBox tbName1 = ScalesGridView.FooterRow.FindControl("TextBox11") as TextBox;
            //TextBox tbName2 = ScalesGridView.FooterRow.FindControl("TextBox21") as TextBox;

            TextBox tbAbbr = ScalesGridView.FooterRow.FindControl("tbAbbr") as TextBox;
            Scale scl = new Scale()
            {
                abreviature = tbAbbr.Text,
                name = tbName.Text,
                test_id = TestID
            };


            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            dc.Scales.InsertOnSubmit(scl);
            dc.SubmitChanges();

            ScalesGridView.ShowFooter = false;
        }
        else
        if (e.CommandName.Equals("InsertCancel"))
        {
            ScalesGridView.ShowFooter = false;
        }

        //else
        //    if (e.CommandName.Equals("Update"))
        //    {
        //        TextBox tbName = ScalesGridView.Rows[0].FindControl ("tbName") as TextBox;
        //        TextBox tbAbbr = ScalesGridView.Rows[0].FindControl("tbAbbr") as TextBox;

        //        //object b = (ScalesGridView.DataSource as List<Scale>)[0].abreviature;
        //        //object b2 = (ScalesGridView.DataSource as List<Scale>)[0].name;
        //        //object a = ScalesGridView.DataSourceObject;

        //        //TesterDataClassesDataContext dc = CommonData.GetDC();

        //        Scale scl = new Scale()
        //        {
        //            abreviature = tbAbbr.Text,
        //            name = tbName.Text,
        //            test_id = Convert.ToInt16(Request.QueryString["TestID"])

        //        };

        //        dc.Scales.InsertOnSubmit(scl);
        //        dc.SubmitChanges();

        //        if (ScalesGridView.DataSourceID == "")
        //            ScalesGridView.DataSourceID = "ScaleLinqDataSource";

        //    }
    }

    protected void btnScaleIns_Click(object sender, EventArgs e)
    { 
        //ScalesGridView.Rows
        //ScalesGridView.EmptyDataTemplate.
    }

    protected void btnAddKeys_Click(object sender, EventArgs e)
    {
        //Session["ScaleID"] = ScalesDropList.SelectedValue;
        //Response.Redirect("~/Designer/SelItem.aspx");
        Response.Redirect(string.Format ( "~/Designer/AddKeys.aspx?TestID={0}", TestID));
    }

    protected void btnDeleteItemScaleLink (object sender, EventArgs e)
    {
        //(sender as LinkButton).Parent.Controls.FindControl
        //Control ctrl = (sender as LinkButton).Parent.Controls[0];

        //dc.Delete_ItemScaleLink();
    }
    protected void GridView4_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {



        //object k = e.Keys["item_id"];
        //object s = e.Values["scale_id"];

        //e.
    }
    protected void GridView4_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {

    }
    protected void btnAddInterpretation_Click(object sender, EventArgs e)
    {
        interpretation new_inter = new interpretation() { test_id = TestID, text = "Введите текст интерпретации" };
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.interpretations.InsertOnSubmit(new_inter);
        dc.SubmitChanges();
        Response.Redirect(string.Format ("~/Designer/Interpretation.aspx?InterID={0}", new_inter.id));
    }

    protected void RecalcButton_Click(object sender, EventArgs e)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.Recalc_Scale_Range(Convert.ToInt16 (ScalesDropDownList.SelectedValue));
        RangeGridView.DataBind();
        //Response.Redirect(Request.RawUrl);
    }
    protected void ParamsButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format ("~/Designer/TestParams.aspx?TestID={0}", TestID));
    }
    protected void btnTestTest_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format ( "~/Player/Passport.aspx?TestID={0}&G=1", TestID));
    }

    protected void Tabs_ActiveTabChanged(object sender, EventArgs e)
    {
        
    }
    protected void Button6_Click(object sender, EventArgs e)
    {
        short? CurrNumber = 0;
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        foreach (item itm in dc.items.Where (p=> p.group_id == Convert.ToInt16 (BlockDropDownList.SelectedValue)))
        {
            if (itm.number == null)
            {
                CurrNumber++;
                itm.number = CurrNumber;
            }
            else
                CurrNumber = itm.number;
        }
        dc.SubmitChanges();
        
    }
    protected void btnCopyKF_To_Click(object sender, EventArgs e)
    {
        //if (GridView4.SelectedDataKey == null) return;

        //object key = GridView4.SelectedDataKey.Value;
        //if (key != null)
        //{
        //    string[] NumberList = NumberListTBox.Text.Split(',');
        //    foreach (string Numb in NumberList)
        //    {
        //        try
        //        {
        //            int ItmNumber = Convert.ToInt16(Numb.Trim());
        //            dc.CopyItemScaleKF_To(Convert.ToInt16 (ScalesDropList.SelectedValue), Convert.ToInt16(key), ItmNumber);
        //        }
        //        catch { }
        //    }
        //    GridView4.DataBind();
        //}
           
    }
    protected void btnAddRange_Click(object sender, EventArgs e)
    {
        string line = tbNewRange.Text;
        
        int idx1 = line.IndexOf ('(');
        int idx2 = line.IndexOf('-');

        if (idx1 > 0 && idx2 > 0)
        {
            CommonData.Add_TScore_Range (
                Convert.ToInt16 (line.Substring(0, idx1 - 1).Trim()), 
                Convert.ToInt16 (line.Substring(idx1 + 1, idx2-idx1-1).Trim()),
                Convert.ToInt16 (line.Substring(idx2 + 1, line.Length - idx2 -2).Trim())
                );

            RangeGridView.DataBind ();
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int fScaleID = Convert.ToInt16(ScalesDropDownList.SelectedValue);
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        dc.Scales.Where(p => p.id == fScaleID).First().Scale_Ranges.Add(
                new Scale_Range() {Max_Value = 1, Score=1, Scale_ID = fScaleID}
            );
        dc.SubmitChanges();
            
        RangeGridView.DataBind();

    }

    TesterDataClassesDataContext dc;

    private void DoPublishTest(bool isPublish)
    {
        dc = CommonData.GetNewDC();
        dc.tests.Where(p => p.id == TestID).First().isPublished = isPublish;
        dc.SubmitChanges();
    }

    protected void Button7_Click(object sender, EventArgs e)
    {
        DoPublishTest(true);
    }
    protected void Button8_Click(object sender, EventArgs e)
    {
        DoPublishTest(false);
    }

    void ShowKeys()
    {
        pnlKeys.Controls.Clear();
        dc = CommonData.GetNewDC();

        test aa =      dc.tests.Where(p => p.id == TestID).First();

        foreach (Scale scl in dc.tests.Where(p => p.id == TestID).First().Scales)
        {
            pnlKeys.Controls.Add(new Label() { Text = scl.name });
            pnlKeys.Controls.Add(new LiteralControl("<br/>"));
            foreach (Test_Question tq in dc.tests.Where(p => p.id == TestID).First().Test_Questions)
            {
                string key_line = CommonData.GetExistKeysString(scl.id, tq.id);
                if (key_line != ")")
                {
                    pnlKeys.Controls.Add(new Label() { Text = CommonData.GetExistKeysString(scl.id, tq.id) });
                    pnlKeys.Controls.Add(new LiteralControl("<br/>"));
                }
            }
            pnlKeys.Controls.Add(new LiteralControl("<hr/>"));
        }

        //if (ScalesDropList.SelectedValue != null && ScalesDropList.SelectedValue != "")
        //{
            
        //    foreach (Test_Question tq in dc.tests.Where(p => p.id == TestID).First().Test_Questions)
        //    {
        //        pnlKeys.Controls.Add(new Label() { Text = CommonData.GetExistKeysString(Convert.ToInt16(ScalesDropList.SelectedValue), tq.id) });
        //        pnlKeys.Controls.Add(new LiteralControl("<br/>"));
        //    }
        //}
    }

    protected void pnlKeys_Load(object sender, EventArgs e)
    {
        ShowKeys();
    }

    protected void RangeGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
            //if (e.CommandName.Equals("Update"))
            //{
            //    DropDownList ddl = RangeGridView.Rows[RangeGridView.EditIndex].FindControl("ddlParamValues") as DropDownList;
            //    object obj = ddl.SelectedValue;

            //    (RangeGridView.Rows[RangeGridView.EditIndex].FindControl("TextBox1") as TextBox).Text = obj.ToString();
            //}
    }

    protected void RangeGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        
    }
    protected void RangeGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        object a = e.RowIndex;

        //string val = (RangeGridView.Rows[e.NewEditIndex].FindControl("TextBox1") as TextBox).Text;
        //(RangeGridView.Rows[e.NewEditIndex].FindControl("ddlParamValues") as DropDownList).SelectedValue = val;
    }

    protected void Button9_Click(object sender, EventArgs e)
    {
        //string FileName = CommonData.GetResultFileName(Request.PhysicalApplicationPath, TestID);

        //string AppPath = Request.PhysicalApplicationPath;
        //string filePath = AppPath + "temp/111.csv";
        //StreamWriter sw;
        //sw = File.CreateText (filePath);
        //sw.WriteLine("abcnnnn");
        //sw.WriteLine("1212121");
        //sw.Flush();
        //sw.Close();

        //Response.AddHeader("Content-disposition", "attachment; filename=" + "111.csv");
        //Response.ContentType = "application/octet-stream";
        //Response.BinaryWrite(sw);
        //Response.End();


        //if (!System.IO.Directory.Exists(AppPath + "/Temp"))
        //    System.IO.Directory.CreateDirectory(AppPath + "/Temp");

        //string sFileName = "111"; //System.IO.Path.GetRandomFileName();
        //string sGenName = "TestResult.csv";

        ////YOu could omit these lines here as you may
        ////not want to save the textfile to the server
        ////I have just left them here to demonstrate that you could create the text file 
        //using (System.IO.StreamWriter SW = new System.IO.StreamWriter(
        //       Server.MapPath("~/Temp/" + sFileName + ".csv")))
        //{
        //    //SW.WriteLine(txtText.Text);
        //    SW.WriteLine("abcnnnn;wewew;wewew;");
        //    SW.WriteLine("1212121;3434;223;");
        //    SW.WriteLine("wwwwwww;_____;223323232;");
        //    SW.Close();
        //}

        //System.IO.FileStream fs = null;
        //fs = System.IO.File.Open(Server.MapPath("~/Temp/" +
        //         sFileName + ".csv"), System.IO.FileMode.Open);
        //byte[] btFile = new byte[fs.Length];
        //fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
        //fs.Close();

        byte[] btFile = CommonData.GetTestResult(Request.PhysicalApplicationPath, TestID, null);

        Response.AddHeader("Content-disposition", "attachment; filename=TestResult.csv");
        Response.ContentType = "application/octet-stream";
        Response.BinaryWrite(btFile);
        Response.End();
    }
    protected void btnCSV_Click(object sender, EventArgs e)
    {
        byte[] btFile = CommonData.GetMechanicData(Request.PhysicalApplicationPath, TestID);

        Response.AddHeader("Content-disposition", "attachment; filename=MechData.csv");
        Response.ContentType = "application/octet-stream";
        Response.BinaryWrite(btFile);
        Response.End();
    }
    protected void Button10_Click(object sender, EventArgs e)
    {
        CommonData.TestSubjectRecalc(TestID); // как насчет асинхронности ?
    }
    protected void ScaleRange_LDS_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {

    }
    protected void btnCopy_Click(object sender, EventArgs e)
    {
        int qq = dc.Test_Copy(TestID);
        Response.Redirect ("testlist.aspx");
    }
    protected void OnSaveSinglePage(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            test tt = dc.tests.Where(tst => tst.id == TestID).FirstOrDefault();
            if (tt != null)
            {
                tt.isSinglePage = cboxIsSinglePage.Checked;
                dc.SubmitChanges();
            }
        }
    }

}
