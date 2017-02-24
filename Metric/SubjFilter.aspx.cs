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

}