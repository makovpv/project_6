using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


//public partial class test2 : INotifyPropertyChanging, INotifyPropertyChanged
//{
//    public string UUU
//    {
//        get
//        { 
//            return this.
//        }
//    }
//}

public partial class Designer_Projects : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        test tst = new test() { name = "Новый тест"};
        tst.ins_date = DateTime.Now;
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        tst.Scales.Add(new Scale() { name = "Шкала-1", abreviature = "Ш1", ScoreCalcType=2 });
        dc.tests.InsertOnSubmit(tst);
        dc.SubmitChanges();

        dc.Test_Questions.InsertOnSubmit(new Test_Question() { test_id = tst.id, number = 1, text = "первый блок." });
        dc.SubmitChanges();
        Response.Redirect("~/Designer/EditTest.aspx?TestID="+tst.id.ToString());
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName.Equals("ShowResult"))
        //    Response.Redirect("~/Analyse/TestSubjectResult.aspx?TestID="+);
    }
    
    protected void GridView2_DataBound(object sender, EventArgs e)
    {
        //(sender as GridView).d
    }
    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.DataItem as test).isPublished)
            {
                (e.Row.Cells[0].Controls[1] as Image).ImageUrl = "~\\Images\\brick_edit.png";
                //e.Row.Cells[0].Text = "~/Iamges/brick_edit.png";
            }
            else
            {
             //   e.Row.Cells[0].Text = "~/Iamges/asterisk_yellow.png";
                (e.Row.Cells[0].Controls[1] as Image).ImageUrl = "~\\Images\\asterisk_yellow.png";
            }
        }
    }
}