using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class Designer_EditDimension : System.Web.UI.Page
{
    int ItemID { 
        get {return Convert.ToInt16(Request.QueryString["ItemID"]);}
    }
    int TestID { 
        get {
            return Convert.ToInt16(ViewState["TestID"]); 
        } 
        set {ViewState["TestID"] = value; } 
    }
    decimal? StepValue
    {
        get { 
            return StepValueTextBox.Text=="" ? 0: Convert.ToDecimal(StepValueTextBox.Text);
            }
        set {
            StepValueTextBox.Text = Convert.ToString (value);
        }
    }
    int? MaxValue
    {
        get
        {
            return MaxValueTextBox.Text == "" ? 0 : Convert.ToInt16 (MaxValueTextBox.Text);
        }
        set
        {
            MaxValueTextBox.Text = Convert.ToString(value);
        }
    }
    int? MinValue
    {
        get
        {
            return MinValueTextBox.Text == "" ? 0 : Convert.ToInt16(MinValueTextBox.Text);
        }
        set
        {
            MinValueTextBox.Text = Convert.ToString(value);
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            item CurrItem = dc.items.Where(p => p.id == ItemID).FirstOrDefault();
            if (CurrItem != null)
            {
                lblCurrentItem.Text = string.Format("{0}. {1}", CurrItem.number, CurrItem.text);
                TestID = CurrItem.Test_Question.test_id;
                hlGotoTest.NavigateUrl = string.Format("~/Designer/EditTest.aspx?TestID={0}", TestID);
            }

            if (Request.QueryString["DimID"] == "")
            {
                //Button3_Click(sender, e);
            }
            else
            {
                //string DimID = Request.QueryString["DimID"];
                //DimensionVarsDropDownList.SelectedValue = DimID;

                //DimensionVarsDropDownList_SelectedIndexChanged(sender, e);
            }
        }

        string ImageExtension = CommonData.GetQuestImageExtension(Server.MapPath("~\\Images\\"+TestID.ToString()), ItemID);
        if (ImageExtension != null)
        {
            Image1.Height = 250;
            Image1.ImageUrl = string.Format("~\\Images\\{0}\\{1}{2}", TestID, ItemID, ImageExtension);
        }
        else Image1.Height = 0;
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        string[] pss = tbBuffer.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            if (CommonData.ExistTestResults(TestID, dc))
                vldExistResult.IsValid = false;
            else
            {
                SubScaleDimension dd = new SubScaleDimension()
                {
                    name = "Новая группа ответов",
                    test_id = TestID,
                    isUniqueSelect = true,
                    dimension_mode = Convert.ToByte(ddlDimensionMode.SelectedValue),
                    dimension_type = Convert.ToByte(ddlDimensionType.SelectedValue),
                    GradationCount = GradationCountTextBox.Text == "" ? (byte?)0 : Convert.ToByte(GradationCountTextBox.Text),
                    min_value = MinValue,
                    max_value = MaxValue,
                    step_value = StepValue
                };
                dc.SubScaleDimensions.InsertOnSubmit(dd);
                dc.SubmitChanges();

                byte i = 1;
                foreach (string s in pss)
                {
                    dc.SubScales.InsertOnSubmit(new SubScale() { name = s, Dimension_ID = dd.id, OrderNumber = i });
                    i++;
                }
                //dc.SubmitChanges();

                byte SetMode = 1;
                if (CurrentItemRadioButton.Checked) SetMode = 1;
                else if (CurrentBlockRadioButton.Checked) SetMode = 2;
                else if (AllTestItemRadioButton.Checked) SetMode = 3;
                dc.SetItemDimension(ItemID, dd.id, SetMode,
                    Convert.ToByte(ddlDimensionType.SelectedValue),
                    Convert.ToByte(ddlDimensionMode.SelectedValue),
                    GradationCountTextBox.Text == "" ? (byte?)0 : Convert.ToByte(GradationCountTextBox.Text),
                    MaxValue,
                    MinValue,
                    StepValue
                    );

                dc.SubmitChanges();

                btnNextItem_Click(sender, e);
            }
        }
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        SubScaleDimension dd = new SubScaleDimension()
        {
            name = "Новая группа ответов",
            test_id = TestID,
            isUniqueSelect = true,
            dimension_mode = 1,
            dimension_type = 1,
            GradationCount = 0
        };
        dc.SubScaleDimensions.InsertOnSubmit(dd);
        dc.SubmitChanges();

        DimensionVarsDropDownList.DataBind();
        DimensionVarsDropDownList.SelectedIndex = DimensionVarsDropDownList.Items.Count - 1;
        //Response.Redirect(Request.RawUrl);
    }
    protected void btnApply_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            if (!CommonData.ExistTestResults(TestID, dc))
            {
                byte SetMode = 1;
                if (CurrentItemRadioButton.Checked) SetMode = 1;
                else if (CurrentBlockRadioButton.Checked) SetMode = 2;
                else if (AllTestItemRadioButton.Checked) SetMode = 3;

                dc.SetItemDimension(ItemID,
                    Convert.ToInt16(DimensionVarsDropDownList.SelectedValue),
                    SetMode,
                    Convert.ToByte(ddlDimensionType.SelectedValue),
                    Convert.ToByte(ddlDimensionMode.SelectedValue),
                    Convert.ToByte(GradationCountTextBox.Text),
                    MinValue,
                    MaxValue,
                    StepValue
                    );

                if (FileUpload1.HasFile)
                {
                    if (!Directory.Exists(Server.MapPath(string.Format(@"~/Images/{0}", TestID))))
                    {
                        Directory.CreateDirectory(Server.MapPath(string.Format(@"~/Images/{0}", TestID)));
                    }
                    FileUpload1.SaveAs(Server.MapPath(string.Format(@"~/Images/{0}/{1}{2}", TestID, ItemID, Path.GetExtension(FileUpload1.FileName))));
                }

                Response.Redirect("~/Designer/EditTest.aspx?TestID=" + TestID.ToString());
            }
            else
            {
                vldExistResult.IsValid = false;
            }
        }
    }
    protected void DropDownList3_DataBound(object sender, EventArgs e)
    {
        //for (int i = 0; i < DimensionVarsDropDownList.Items.Count; i++)
        //{
        //    if (DimensionVarsDropDownList.Items[i].Value == "100")
        //    { }
        //}

        //object DimID =Request.QueryString["DimID"];

        //if (DimID.ToString()!="")
        //    DimensionVarsDropDownList.SelectedValue = DimID.ToString();
    }
    protected void DimensionVarsDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //string s = DimensionVarsDropDownList.SelectedValue;

        if (DimensionVarsDropDownList.SelectedValue != "")
        {
            TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
            SubScaleDimension ssd = dc.SubScaleDimensions.Where(p => p.id == Convert.ToInt16(DimensionVarsDropDownList.SelectedValue)).FirstOrDefault();
            if (ssd != null)
            {
                ddlDimensionType.SelectedValue = ssd.dimension_type.ToString();
                ddlDimensionMode.SelectedValue = ssd.dimension_mode.ToString();
                GradationCountTextBox.Text = ssd.GradationCount.ToString(); // сделать через datacontext у всей формы ? (datacontext = ssd)
                MinValue = ssd.min_value;
                MaxValue = ssd.max_value;
                StepValue = ssd.step_value;
            }
        }
    }
    protected void DimensionVarsDropDownList_DataBound(object sender, EventArgs e)
    {
        object DimID = Request.QueryString["DimID"];

        if (DimID.ToString() != "")
        {
            DimensionVarsDropDownList.SelectedValue = DimID.ToString();
            DimensionVarsDropDownList_SelectedIndexChanged(sender, e);
        }
    }
    protected void btnNextItem_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            item itm = dc.items.Where(p => p.id == ItemID).FirstOrDefault();
            if (itm != null)
            {
                item NextItm = dc.items.Where(p => p.number > itm.number && p.Test_Question.id == itm.Test_Question.id).OrderBy(p => p.number).FirstOrDefault();
                if (NextItm != null)
                    Response.Redirect(string.Format("~/Designer/EditDimension.aspx?ItemID={0}&DimID={1}", NextItm.id, NextItm.dimension_id));
            }
        }
    }


    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (CommonData.ExistTestResults(TestID, null))
        {
            vldExistResult.IsValid = false;
            e.Cancel = true;
        }
    }
    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (CommonData.ExistTestResults(TestID, null))
        {
            vldExistResult.IsValid = false;
            e.Cancel = true;
        }
    }
}