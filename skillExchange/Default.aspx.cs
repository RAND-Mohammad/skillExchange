using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // يتاكدد أن المستخدم مسجل دخول
            if (Session["userId"] == null)
            {
                Response.Redirect("Login.aspx");
            }


            // يعرض اسم المستخدم مرة واحدة عند فتح الصفحة
            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome " + Session["fullName"].ToString();
                LoadLatestSkills();
            }

        }

        private void LoadLatestSkills()
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                const string query = @"SELECT TOP 6 s.skillName, s.description, s.skillLevels, s.availableDays, c.categoryName, u.fullName AS ownerName FROM skills s INNER JOIN categories c ON s.categoryId = c.categoryId INNER JOIN users u ON s.userId = u.userId WHERE s.isAvailable = 1 ORDER BY s.createdDate DESC";
                DataTable table = new DataTable();
                new SqlDataAdapter(query, con).Fill(table);
                rptLatestSkills.DataSource = table;
                rptLatestSkills.DataBind();
                pnlNoSkills.Visible = table.Rows.Count == 0;
            }
        }
    }
}
