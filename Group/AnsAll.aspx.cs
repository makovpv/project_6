﻿using System;
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

    public enum DimensionType {dtSingleChoise = 1, dtMultiSelect = 2, dtOpenAnswer = 3, 
        dtGender = 8, dtBirthYear = 9, dtRange = 10, dtDate = 11, dtNumber = 12,
        dtEMP = 13 }
//select 7, 'ранжирование',	1, 0 union all
//select 5, 'попарное ранжирование',	1, 0 union all
//select 6, 'шкалирование',	1, 0 union all
    
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

                    foreach (Test_Question tq in subj.test.Test_Questions)
                    {
                        foreach (item itm in tq.items)
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
                                        
                                        ListControl lct;
                                        if (itm.SubScaleDimension.dimension_mode == 3) // dropdownlist
                                        {
                                            lct = new DropDownList() { CssClass = "AnsAllDropList"};
                                        }
                                        else // radiobuttons
                                        {
                                            lct = new RadioButtonList();
                                        }
                                        lct.ID = string.Format("spCtr_{0}__{1}", itm.id, MM);
                                        foreach (SubScale ssc in itm.SubScaleDimension.SubScales)
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
                                        vld.ControlToValidate =  lct.ID;
                                        vld.ErrorMessage = "Не выбран ответ";
                                        vld.ForeColor = System.Drawing.Color.Red;
                                        vld.SetFocusOnError = true;
                                        //vld.ValidationGroup = "ValidGroup";
                                        divContent.Controls.Add(vld);

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
                                    case DimensionType.dtEMP:
                                        CommonData.GenerateAnswerWithEMP(itm.SubScaleDimension, subj.subject_group.Company);
                                        dc.SubmitChanges();

                                        
                                        List<Test_Result> ltr = subj.Test_Results.Where(trr => trr.item_id == itm.id).ToList();
                                        CheckBoxList cbl = new CheckBoxList()
                                        {
                                            ID = string.Format("spCtr_{0}__{1}", itm.id, MM),
                                            //CssClass = "invisible"
                                        };
                                        cbl.Attributes["style"] = "display: none";
                                        
                                        string SelectedNames = "";
                                        foreach (SubScale ssc in itm.SubScaleDimension.SubScales)
                                        {
                                            ListItem li = new ListItem(ssc.name, ssc.id.ToString()) {
                                                Selected = ltr.Where(bb => bb.SubScale_ID == ssc.id).FirstOrDefault() != null
                                            };
                                            cbl.Items.Add(li);
                                            if (li.Selected) {
                                                SelectedNames= SelectedNames +", " +ssc.name;
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
}