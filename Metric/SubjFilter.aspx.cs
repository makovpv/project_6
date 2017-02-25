using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Metric_SubjFilter : System.Web.UI.Page
{

    int idMetric { get { return Convert.ToInt32(Request.QueryString["id"]); } }
    
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                metric m = dc.metrics.Single(q => q.idMetric == idMetric);
                ddlTest.SelectedValue = m.idTest.ToString();
            }
        }
    }

    protected void btnAddStateFilterClick(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            metric mm = dc.metrics.Single(q => q.idMetric == idMetric);
            foreach (user_state st in dc.user_states)
            {
                mm.metric_subj_filters.Add(new metric_subj_filter() { idState = st.id });

            }
            dc.SubmitChanges();
        }
        gv_State.DataBind();
    }

    protected void btnAddJobFilterClick(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            metric mm = dc.metrics.Single(q => q.idMetric == idMetric);
            foreach (Job j in dc.Jobs)
            {
                mm.metric_subj_filters.Add(new metric_subj_filter() { idJob = j.id });

            }
            dc.SubmitChanges();
        }        
        gv_job.DataBind();
    }

    protected void btnApplyClick(object sender, EventArgs e)
    {
        using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
        {
            metric m = dc.metrics.Single(q => q.idMetric == idMetric);
            m.idTest = ddlTest.SelectedValue != "" ? Convert.ToInt32(ddlTest.SelectedValue) : (int?)null;
            
            if (ddlScale.SelectedValue != "")
                m.idScale = Convert.ToInt32(ddlScale.SelectedValue);

            dc.SubmitChanges();
            Response.Redirect("~\\metric\\list.aspx");
        }
    }

    protected void ddlScale_DataBound(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
            {
                metric m = dc.metrics.Single(q => q.idMetric == idMetric);
                ddlScale.SelectedValue = m.idScale.ToString();
            }
        }

    }
}