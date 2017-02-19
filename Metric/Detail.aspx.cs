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
                    metric mm = dc.metrics.Where(p => p.idMetric == idMetric).FirstOrDefault();
                    if (mm != null)
                    {
                        lblMetric.InnerText = string.Format("Список респондентов для метрики '{0}'", mm.name);
                        lDescription.Text = mm.description; 
                        lCalcDescription.Text = mm.calc_description;
                        lEliminate_Schema.Text = mm.eliminate_scheme;
                        
                        
                        sqlDetail.SelectParameters["idmetric"].DefaultValue = idMetric.ToString();
                    }
                }
            }
            else
                throw new Exception("Метрика не определена");
        }

    }
}