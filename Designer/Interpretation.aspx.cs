using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Designer_Interpretation : System.Web.UI.Page
{
    int InterID {get{return Convert.ToInt32(Request.QueryString["InterID"]); }}

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                interpretation itrp = dc.interpretations.Where(p => p.id == InterID).FirstOrDefault();
                hlGotoTest.NavigateUrl = string.Format("~/Designer/EditTest.aspx?TestID={0}", itrp.test_id);
                hlGotoBlocks.NavigateUrl = string.Format("~/Designer/TestBlocks.aspx?TestID={0}", itrp.test_id);

                //cboxActiveThreat.Checked = itrp.idInterKind == 1;
                ddlInterKind.SelectedValue = itrp.idInterKind != null ? itrp.idInterKind.ToString() : "0";
                tbText.Text = Server.HtmlDecode( itrp.text); //dc.ExecuteQuery<string>("select text from interpretation where id={0}", new object[] { InterID }).First();
            }
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            int TestID = dc.interpretations.Where(p => p.id == InterID).FirstOrDefault().test_id;
            System.Data.Linq.EntitySet<Scale> Scales = dc.tests.Where(p => p.id == TestID).First().Scales;
            if (Scales.Count == 0)
                throw new Exception("Для теста не заведены шкалы");
            else
            {
                dc.interpretations.Where(p => p.id == InterID).First().inter_conditions.Add(
                    new inter_condition() { inter_id = InterID, scale_id = Scales[0].id, range_type = 1 });

                dc.SubmitChanges();
                gridConditions.DataBind();
            }
        }
    }
    protected void btnOk_Click(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            interpretation intrpret = dc.interpretations.Where(p => p.id == InterID).FirstOrDefault();
            intrpret.text = Server.HtmlEncode (tbText.Text); // см. секцию в web.config (<pages validateRequest="false"/><httpRuntime requestValidationMode="2.0"/>)
            //intrpret.idInterKind = cboxActiveThreat.Checked == true ? (byte)1 : (byte)0;
            intrpret.idInterKind = Convert.ToByte ( ddlInterKind.SelectedValue);
            int TestID = intrpret.test_id;
            dc.SubmitChanges();
            Response.Redirect(string.Format("EditTest.aspx?TestID={0}", TestID));
        }
    }
    protected void ajaxFileUpload_OnUploadComplete(object sender, AjaxControlToolkit.AjaxFileUploadEventArgs e)
    {
        if (e.ContentType.Contains("jpg") || e.ContentType.Contains("gif")
            || e.ContentType.Contains("png") || e.ContentType.Contains("jpeg"))
        {
            Session["fileContentType_" + e.FileId] = e.ContentType;
            Session["fileContents_" + e.FileId] = e.GetContents();
        }

        // Set PostedUrl to preview the uploaded file.         
        e.PostedUrl = string.Format("?preview=1&fileId={0}", e.FileId);
    }
}