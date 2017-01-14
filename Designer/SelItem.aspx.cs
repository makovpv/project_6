using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Designer_SelItem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //при выборе нужно показывать номер вопроса еще!
    

    protected void Button1_Click(object sender, EventArgs e)
    {
        int ScaleID = Convert.ToInt16(Session["ScaleID"]);
        TesterDataClassesDataContext dc = new TesterDataClassesDataContext();
        foreach (ListItem li in ItemsCheckBoxList.Items)
        {
            if (li.Selected)
            {
                dc.Add_ItemScaleLink (Convert.ToInt16 ( li.Value),  ScaleID);
                
                
                //ItemScale_Link isl = new ItemScale_Link();
                //isl.item_id = Convert.ToInt16 ( li.Value);
                //isl.scale_id = Convert.ToInt16(BlockDropDownList.SelectedValue);
                

                
                //isl.subscale_id
                //dc.ItemScale_Links.InsertOnSubmit(isl);
            }
        }
        //dc.SubmitChanges();

        Response.Redirect("~/Designer/EditTest.aspx?TestID=" + Session["TestID"]);
        
    }

}