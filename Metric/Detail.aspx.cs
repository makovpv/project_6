using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Metric_Detail : System.Web.UI.Page
{

    int idMetric { get { return Convert.ToInt32(Request.QueryString["id"]); } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["id"] != null)
            {
                using (TesterDataClassesDataContext dc = new TesterDataClassesDataContext())
                {
                    lblMetric.InnerText = string.Format("Список респондентов для метрики '{0}'", dc.metrics.Where(p => p.idMetric == idMetric).FirstOrDefault().name);
                    sqlDetail.SelectParameters["idmetric"].DefaultValue = idMetric.ToString();
                }
            }
            else
                throw new Exception("Метрика не определена");
        }

    }
}