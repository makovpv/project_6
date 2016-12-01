using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Player
/// </summary>
public class Player
{
	public Player()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public struct AnswerInfo // необходимо этот тип заменить на тип из DBML
    {
        public int ItemID;
        public int? AnsID;
        public short? SelectedValue;
        public string Text;
        public int? linkID; // ссылка на тест_в_исследовании
    };

    /// <summary>
    /// обработка ответов для одностраничного режима
    /// </summary>
    public static List<AnswerInfo> GetUserAnswers_JS_SinglePage(string[] aKeys, System.Collections.Specialized.NameValueCollection aForm)
    {
        List<AnswerInfo> ItemAnsList = new List<AnswerInfo>();
        foreach (string key in aKeys)
        {
            string partialKey = null;
            foreach (string s in key.Split('$'))
            {
                if (s.StartsWith("spCtr_"))
                {
                    partialKey = s;
                    break;
                }
            }
            
            int idx1 = partialKey.IndexOf('_');
            int idx2 = partialKey.IndexOf('_', idx1 + 1);
            int idx3 = partialKey.IndexOf('_', idx2 + 1);
            int UItemID = Convert.ToInt32(partialKey.Substring(idx1 + 1, idx2 - idx1 - 1));
            int? UAnsID = null;
            short? val = null;
            int? SubjLinkID = null;

            string val_str = aForm.GetValues(key)[0];
            if (val_str == "")
            {
            }
            else
            {
                UAnsID = Convert.ToInt32(val_str); //UAnsID = Convert.ToInt32(key.Substring(idx2 + 1, idx3 - idx2 - 1));
                SubjLinkID = Convert.ToInt32(partialKey.Substring(idx3 + 1, partialKey.Length - idx3 - 1));
                val = 1; //Convert.ToInt16(val_str);
            }
            ItemAnsList.Add(new AnswerInfo() { AnsID = UAnsID, ItemID = UItemID, SelectedValue = val, linkID = SubjLinkID }); //почему то SelectedValue всегда равно 1 для тестового теста (1206, по группе 1)
        }
        return ItemAnsList;
    }

    public static List<AnswerInfo> GetUserAnswers_JS(string[] aKeys, System.Collections.Specialized.NameValueCollection aForm)
    {
        List<AnswerInfo> ItemAnsList = new List<AnswerInfo>();
        //ItemAnsList = (List<AnswerInfo>)Session["AnsList"]; //ItemAnsList = (List<AnswerInfo>)ViewState["AnsList"];
        foreach (string key in aKeys)
        {
            int idx1 = key.IndexOf('_');
            int idx2 = key.IndexOf('_', idx1 + 1);
            int idx3 = key.IndexOf('_', idx2 + 1);
            int UItemID = Convert.ToInt32(key.Substring(idx1 + 1, idx2 - idx1 - 1));
            int? UAnsID = null;
            short? val = null;
            int? SubjLinkID = null;

            string val_str = aForm.GetValues(key)[0];
            if (val_str == "")
            {
            }
            else
            {
                UAnsID = Convert.ToInt32(key.Substring(idx2 + 1, idx3 - idx2 - 1));
                SubjLinkID = Convert.ToInt32(key.Substring(idx3 + 1, key.Length - idx3 - 1));
                val = Convert.ToInt16(val_str);
            }
            ItemAnsList.Add(new AnswerInfo() { AnsID = UAnsID, ItemID = UItemID, SelectedValue = val, linkID = SubjLinkID }); //почему то SelectedValue всегда равно 1 для тестового теста (1206, по группе 1)
        }
        return ItemAnsList;
    }

    public static List<AnswerInfo> GetUserText_JS(string[] aKeys, System.Collections.Specialized.NameValueCollection aForm)
    {
        List<AnswerInfo> TextAnsList = new List<AnswerInfo>();
        foreach (string key in aKeys)
        {
            int idx1 = key.IndexOf('_');
            int idx2 = key.IndexOf('_', idx1 + 1);
            int idx3 = key.IndexOf('_', idx2 + 1);

            int UItemID = Convert.ToInt32(key.Substring(idx1 + 1, idx2 - idx1 - 1));
            int? SubjLinkID = Convert.ToInt32(key.Substring(idx3 + 1, key.Length - idx3 - 1));   // как будет при прохождении НЕ из группы?
            string ans_text = aForm.GetValues(key)[0];

            TextAnsList.Add(new AnswerInfo() { ItemID = UItemID, linkID = SubjLinkID, Text = ans_text });
        }
        return TextAnsList;
    }

    public static List<AnswerInfo> GetUserParams_JS(string[] aKeys, System.Collections.Specialized.NameValueCollection aForm)
    {
        List<AnswerInfo> ParamAnsList = new List<AnswerInfo>();
        int UItemID = 0;
        foreach (string key in aKeys)
        {
            if (!key.Contains("_txt_"))
            {
                short val = Convert.ToInt16(aForm.GetValues(key)[0]);
                string ShortKey = key.Replace("prm_ContentPlaceHolder1_ddl_Prm_", "");
                if (ShortKey.Contains("Gender"))
                {
                    UItemID = Convert.ToInt16(ShortKey.Substring(7, ShortKey.Length - 8));
                    ParamAnsList.Add(new AnswerInfo() { ItemID = UItemID, AnsID = Convert.ToInt16(val) });
                }
                else // Age_27_
                    if (ShortKey.Contains("Age"))
                    {
                        // для возраста не сохраняем значение параметра. сохраняется в субъекте
                        UItemID = Convert.ToInt16(ShortKey.Substring(4, ShortKey.Length - 5));
                        int? AgeRangeID = CommonData.GetAgeRangeID(UItemID, val);
                        ParamAnsList.Add(new AnswerInfo() { ItemID = UItemID, AnsID = AgeRangeID });
                    }
                    else
                    {
                        UItemID = Convert.ToInt16(ShortKey.Substring(0, ShortKey.Length - 1));
                        ParamAnsList.Add(new AnswerInfo() { ItemID = UItemID, AnsID = Convert.ToInt16(val) });
                    }
            }
            else // текстовый параметр
            {
                string txt_val = aForm.GetValues(key)[0];
                string ShortKey = key.Replace("prm_ContentPlaceHolder1_txt_Prm_", "");
                UItemID = Convert.ToInt16(ShortKey.Substring(0, ShortKey.Length - 1));
                ParamAnsList.Add(new AnswerInfo() { ItemID = UItemID, AnsID = null, Text = txt_val });
            }

        }
        return ParamAnsList;
    }

    public static string RemovePlaceHolderPrefix(string aString)
    {
        return aString.Replace("ctl00$ContentPlaceHolder1$", "");
    }
}