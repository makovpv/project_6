using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Player_AnsAll : System.Web.UI.Page
{

    int? idsubj;
    int? idSubj 
    { get 
        { 
            string res = Request.QueryString["id"];
            if (res != null)
                return Convert.ToInt32(res);
            else
            {
                return idsubj;
            }
        }
        set {
            idsubj = value;
        }
    }
    int? idApprove
    {
        get
        {
            string res = Request.QueryString["ap"];
            if (res != null)
                return Convert.ToInt32(res);
            else
                return null;
        }
    }


    
    protected void Page_Load(object sender, EventArgs e)
    {
       if (!IsPostBack)
        {
            CreateControls();
        }
        else
        {
            SaveResults();
            Response.Redirect ("~\\lk2.aspx");
            //PreviousPage.ResolveUrl(
        }
    }

    private void SaveResults()
    {
        string[] keys = Request.Form.AllKeys.Where(pp => pp.StartsWith("ctl00$ContentPlaceHolder1$spCtr_")).ToArray();
        List<Player.AnswerInfo> ItemAnsList = Player.GetUserAnswers_JS_SinglePage(keys, Request.Form);
        keys = Request.Form.AllKeys.Where(pp => pp.StartsWith("ctl00$ContentPlaceHolder1$txt_")).ToArray();
        List<Player.AnswerInfo> TextAnswerList =Player.GetUserText_JS(keys, Request.Form);

        keys = Request.Form.AllKeys.Where(pp => pp.StartsWith("ctl00$ContentPlaceHolder1$dt_")).ToArray();
        List<Player.AnswerInfo> DateAnswerList = Player.GetUserText_JS(keys, Request.Form);
        TextAnswerList.AddRange(DateAnswerList);

        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            if (idApprove != null)
                idSubj = dc.test_subject_approveds.Where(ww => ww.id == idApprove).FirstOrDefault().idSubject;

            dc.Test_Results_Txts.DeleteAllOnSubmit(dc.Test_Results_Txts.Where(rt => rt.subject_id == idSubj));
            dc.Test_Results.DeleteAllOnSubmit(dc.Test_Results.Where(rt => rt.Subject_ID == idSubj));

            foreach (Player.AnswerInfo ai in TextAnswerList)
            {
                dc.Test_Results_Txts.InsertOnSubmit (new Test_Results_Txt()
                {
                    subject_id = (int)idSubj,        
                    item_id = ai.ItemID,
                    text = ai.Text
                });
            }
            foreach (Player.AnswerInfo ai in ItemAnsList)
            {
                dc.Test_Results.InsertOnSubmit(new Test_Result()
                {
                    Subject_ID = (int)idSubj,
                    item_id = ai.ItemID,
                    SubScale_ID = ai.AnsID,
                    SelectedValue = ai.SelectedValue
                });
            }

            Test_Subject tsbj = dc.Test_Subjects.Where(ts => ts.id == idSubj).FirstOrDefault();
            if (tsbj != null)
            {
                tsbj.Test_Date = DateTime.UtcNow;
                tsbj.actual = true;
                subject_group sgroup = tsbj.subject_group;
                if (sgroup != null)
                {
                    if (sgroup.isAutoSubjAdd)
                    {// автоматическое добавление субъекта при прохождении теста
                        sgroup.Test_Subjects.Add(
                            new Test_Subject()
                            {
                                Age = tsbj.Age,
                                fio = tsbj.fio,
                                Gender = tsbj.Gender,
                                idUser = tsbj.idUser,
                                Nick_Name = tsbj.Nick_Name,
                                Test_Id = tsbj.Test_Id
                            }
                        );
                    }

                    if (sgroup.isAnonymous && rblAnon.SelectedValue == "1")
                    {// реализация анонимности
                        tsbj.fio = "Аноним";
                        tsbj.Nick_Name = "Аноним";
                        tsbj.idUser = null;
                    }
                    else
                    {// смена состояния актуальности старых записей
                        dc.Test_Subjects.Where(q => q.Test_Id == tsbj.Test_Id && q.idUser == tsbj.idUser && q.actual == true
                            && q.Test_Date != null && q.Test_Date < tsbj.Test_Date).ToList().ForEach(x => x.actual = false);
                    }
                }
            }

            if (idApprove!= null)
            {
                test_subject_approved tsapp = dc.test_subject_approveds.Where(tsa => tsa.id == (int)idApprove).FirstOrDefault();
                if (tsapp != null)
                {
                    tsapp.isApproved = true;
                    tsapp.commentary = tboxApproveComment.Text;
                    tsapp.ApprovedDate = DateTime.UtcNow;
                }
            }

            dc.CalcSubjectTestValues(idSubj);
            dc.SubmitChanges();
        }
    }

    private void CreateControls()
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            if (idApprove != null)
                idSubj = dc.test_subject_approveds.Where(ww => ww.id == idApprove).FirstOrDefault().idSubject;

            if (idSubj != null)
            {
                Test_Subject subj = dc.Test_Subjects.Where(ts => ts.id == idSubj).FirstOrDefault();
                if (subj != null)
                {
                    divInfo.Controls.Add(new LiteralControl(string.Format(
                        "Исследование: '{0}'<br/>Респондент: {1}<br/>Дата прохождения: {2}<br/><br/>",
                        subj.subject_group.name, subj.fio, subj.Test_Date)));

                    this.Master.TestName = subj.subject_group.name;

                    if (subj.subject_group != null)
                    {
                        Title = subj.subject_group.name;
                        rblAnon.Visible = subj.subject_group.isAnonymous;
                        divAnonHeader.Visible = rblAnon.Visible;
                    }
                    divInstruction.InnerText = subj.test.instruction;

                    Test_SubjectGroup_Link tsg_lnk = subj.subject_group.Test_SubjectGroup_Links.FirstOrDefault();
                    int MM = tsg_lnk == null ? 0 : tsg_lnk.id; // не совсем разобрался. что-то про объекты оценки

                    foreach (Test_Question tq in subj.test.Test_Questions.OrderBy(ord=> ord.number))
                    {
                        foreach (item itm in tq.items.OrderBy(o=>o.number))
                        {
                            if (itm.SubScaleDimension != null)
                            {
                                //divContent.Controls.Add(new LiteralControl(string.Format ( "<h4>{0}</h4>", itm.text))); // all headers are redifined in style.css !?!
                                divContent.Controls.Add(new Label() { Text = itm.text, CssClass = "itemText" });
                                divContent.Controls.Add(new LiteralControl("<br/><br/>"));

                                if (itm.description != null || itm.description != "")
                                {
                                    divContent.Controls.Add(new Label() { Text = itm.description, CssClass="itemDescript" });
                                    divContent.Controls.Add(new LiteralControl("<br/><br/>"));
                                }

                                switch ((DimensionType)itm.SubScaleDimension.dimension_type)
                                {
                                    case DimensionType.dtOpenAnswer:
                                        Test_Results_Txt txt_answer = subj.Test_Results_Txts.Where(trt => trt.item_id == itm.id).FirstOrDefault();

                                        divContent.Controls.Add(new TextBox()
                                        {
                                            TextMode = TextBoxMode.MultiLine,
                                            Width = Unit.Percentage(60.0),
                                            Text = txt_answer == null ? null : txt_answer.text,
                                            ID = string.Format("txt_{0}__{1}", itm.id, MM)
                                        });
                                        break;
                                    case DimensionType.dtSingleChoise:
                                        CreateSingleChoiseControl(subj, MM, itm);
                                        break;
                                    case DimensionType.dtDate:
                                        Test_Results_Txt dt_answer = subj.Test_Results_Txts.Where(trt => trt.item_id == itm.id).FirstOrDefault();
                                        TextBox tbox = new TextBox()
                                        {
                                            ID = string.Format("dt_{0}__{1}", itm.id, MM),
                                            Text = dt_answer == null ? null : dt_answer.text,
                                            //, Enabled = false
                                            CssClass = "datePickText"
                                        };
                                        Button btn = new Button() { Text = "...", ID = "btn" + itm.id.ToString() };
                                        AjaxControlToolkit.CalendarExtender ce = new AjaxControlToolkit.CalendarExtender()
                                        {
                                            TargetControlID = tbox.ID,
                                            PopupButtonID = btn.ID,
                                            Format = "dd.MM.yyyy",
                                            FirstDayOfWeek = FirstDayOfWeek.Monday
                                        };
                                        divContent.Controls.Add(tbox);
                                        divContent.Controls.Add(btn);
                                        divContent.Controls.Add(ce);

                                        break;
                                    case DimensionType.dtCompetence:
                                        //CreateSingleChoiseControl(subj, MM, itm);
                                        CreateCompetenceControl(subj, MM, itm, dc);
                                        
                                        string ScriptText = "var nn = 1";
                                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DataScript", "<script language=javascript>" + ScriptText + "</script>");
                                        break;
                                    case DimensionType.dtBook:
                                        CommonData.GenerateAnswerWithBook(itm.SubScaleDimension, subj.subject_group.Company);
                                        dc.SubmitChanges();

                                        CreateSingleChoiseControl(subj, MM, itm);

                                        break;
                                    case DimensionType.dtEMP:
                                        CommonData.GenerateAnswerWithEMP(itm.SubScaleDimension, subj.subject_group.Company);
                                        dc.SubmitChanges();

                                        string[] emp_fired = (
                                            from ua in dc.user_accounts
                                            where (ua.idCompany == subj.subject_group.idCompany && (ua.idState == 1 || ua.idState == 3 || ua.idState == 4 || ua.idState == null))
                                            select ua.fio).ToArray();

                                        //DimensionType.dtCompetence

                                        
                                        List<Test_Result> ltr = subj.Test_Results.Where(trr => trr.item_id == itm.id).ToList();
                                        CheckBoxList cbl = new CheckBoxList()
                                        {
                                            ID = string.Format("spCtr_{0}__{1}", itm.id, MM),
                                            //CssClass = "invisible"
                                        };
                                        cbl.Attributes["style"] = "display: none";
                                        
                                        string SelectedNames = "";
                                        foreach (SubScale ssc in itm.SubScaleDimension.SubScales.OrderBy(q=> q.name))
                                        {

                                            //li.Attributes["class"] = "emp_fired";
                                            if (!emp_fired.Contains(ssc.name))
                                            {
                                                ListItem li = new ListItem(ssc.name, ssc.id.ToString())
                                                {
                                                    Selected = ltr.Where(bb => bb.SubScale_ID == ssc.id).FirstOrDefault() != null
                                                };

                                                cbl.Items.Add(li);
                                                if (li.Selected)
                                                {
                                                    SelectedNames = SelectedNames + ", " + ssc.name;
                                                }

                                            }
                                            
                                        }
                                        cbl.RepeatColumns = 3;
                                        divContent.Controls.Add(new LiteralControl(string.Format (@"<button type='button' onclick='empClick(""ContentPlaceHolder1_{0}"")'>Сотрудники...</button>", cbl.ID)));
                                        
                                        if (SelectedNames != "")
                                        {
                                            divContent.Controls.Add(new LiteralControl(@"<span style=""margin-left: 25px""></span>"));
                                            SelectedNames = "Выбраны:" + SelectedNames.Remove(0, 1);
                                            divContent.Controls.Add(new Literal() { Text = SelectedNames });
                                        }
                                        divContent.Controls.Add(cbl);
                                        break;
                                    case DimensionType.dtMultiSelect:
                                        List<Test_Result> ltr_ms = subj.Test_Results.Where(trr => trr.item_id == itm.id).ToList();
                                        CheckBoxList cbl_ms = new CheckBoxList()
                                        {
                                            ID = string.Format("spCtr_{0}__{1}", itm.id, MM),
                                            Visible = false
                                        };
                                        foreach (SubScale ssc in itm.SubScaleDimension.SubScales)
                                        {
                                            cbl_ms.Items.Add(new ListItem(ssc.name, ssc.id.ToString())
                                            {
                                                Selected = ltr_ms.Where(bb => bb.SubScale_ID == ssc.id).FirstOrDefault() != null
                                            });
                                        }
                                        //cbl_ms.RepeatColumns = 3;
                                        divContent.Controls.Add(cbl_ms);
                                        break;
                                } // case

                                divContent.Controls.Add(new LiteralControl("<br/><hr/><br/>"));
                            }
                        } //foreach
                    }//foreach

                    //tboxApproveComment.Visible = idApprove != null;
                    divApprove.Visible = idApprove != null;
                    divApproveTitle.Visible = idApprove != null;
                } // if subj!= null
                else
                    throw new Exception("субъект не найден");
            }
            else
                throw new Exception("субъект не определен");
        }
    }

    private void CreateSingleChoiseControl(Test_Subject subj, int MM, item itm)
    {
        ListControl lct;
        if (itm.SubScaleDimension.dimension_mode == 3) // dropdownlist
        {
            lct = new DropDownList() { CssClass = "AnsAllDropList" };
        }
        else // radiobuttons
        {
            lct = new RadioButtonList();
        }
        lct.ID = string.Format("spCtr_{0}__{1}", itm.id, MM);
        foreach (SubScale ssc in itm.SubScaleDimension.SubScales.OrderBy(q => q.OrderNumber))
        {
            lct.Items.Add(new ListItem(
                ssc.name,
                ssc.id.ToString()
                //string.Format("spCtr_{0}_{1}_{2}", itm.id, ssc.id, MM)
                ));
        }
        Test_Result tr = subj.Test_Results.Where(trr => trr.item_id == itm.id).FirstOrDefault();
        if (tr != null && tr.SubScale_ID != null)
            lct.SelectedValue = tr.SubScale_ID.ToString();
        divContent.Controls.Add(lct);

        RequiredFieldValidator vld = new RequiredFieldValidator();
        vld.ControlToValidate = lct.ID;
        vld.ErrorMessage = "Не выбран ответ";
        vld.ForeColor = System.Drawing.Color.Red;
        vld.SetFocusOnError = true;
        //vld.ValidationGroup = "ValidGroup";
        divContent.Controls.Add(vld);
    }

    private void CreateCompetenceControl(Test_Subject subj, int MM, item itm, TesterDataClassesDataContext p_dc)
    {
        ListControl lct;
        if (itm.SubScaleDimension.dimension_mode == 3) // dropdownlist
        {
            lct = new DropDownList() { CssClass = "AnsAllDropList" };
        }
        else // radiobuttons
        {
            lct = new RadioButtonList();
        }
        lct.ID = string.Format("spCtr_{0}__{1}", itm.id, MM);
        foreach (SubScale ssc in itm.SubScaleDimension.SubScales.OrderBy(q => q.OrderNumber))
        {
            string Partners = "";

            foreach (string s in (
                from ts in subj.subject_group.Test_Subjects
                join trr in p_dc.Test_Results on ts.id equals trr.Subject_ID
                select new {ts.fio, trr.SubScale} ).Where (e=> 
                    e.SubScale.SubScaleDimension.dimension_type == (byte)DimensionType.dtCompetence && 
                    e.SubScale.name == ssc.name).Select (s=> s.fio)
                )
            {
                Partners += "," + s;
            }

            if (Partners.Length > 0)
            {
                
                Partners = string.Format("<i> (выбрали {0})</i>", Partners.Remove(0, 1));
            }
            
            lct.Items.Add(new ListItem(
                ssc.name + Partners,
                ssc.id.ToString()
                ));
        }
        Test_Result tr = subj.Test_Results.Where(trr => trr.item_id == itm.id).FirstOrDefault();
        if (tr != null && tr.SubScale_ID != null)
            lct.SelectedValue = tr.SubScale_ID.ToString();
        divContent.Controls.Add(lct);

        RequiredFieldValidator vld = new RequiredFieldValidator();
        vld.ControlToValidate = lct.ID;
        vld.ErrorMessage = "Не выбран ответ";
        vld.ForeColor = System.Drawing.Color.Red;
        vld.SetFocusOnError = true;
        divContent.Controls.Add(vld);
    }
}