using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class Feedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadSkillsDropdown();
                LoadFeedback();
            }
        }

        private void LoadSkillsDropdown()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT
                            skills.skillId,
                            skills.skillName + ' - by ' + users.fullName AS skillDisplay
                         FROM skills
                         INNER JOIN users ON skills.userId = users.userId";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                ddlSkillFeedback.DataSource = dataTable;
                ddlSkillFeedback.DataTextField = "skillDisplay";
                ddlSkillFeedback.DataValueField = "skillId";
                ddlSkillFeedback.DataBind();
                ddlSkillFeedback.Items.Insert(0, new ListItem("-- Select a Skill --", "0"));
            }
        }

        private void LoadFeedback()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT
                            f.feedbackId, s.skillName, u.fullName AS reviewerName,
                            f.rating, f.comment, f.createdDate
                         FROM feedbacks f
                         INNER JOIN skills s ON f.skillId = s.skillId
                         INNER JOIN users u ON f.userId = u.userId
                         ORDER BY f.createdDate DESC";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                gvFeedback.DataSource = dataTable;
                gvFeedback.DataBind();
            }
        }

        protected void btnSubmitFeedback_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlSkillFeedback.SelectedValue == "0")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please choose a skill.";
                return;
            }
            if (rblRating.SelectedItem == null)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please choose a rating.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtComment.Text))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please write a comment.";
                return;
            }

            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string insertQuery = @"INSERT INTO feedbacks
                        (userId, skillId, rating, comment, createdDate)
                        VALUES
                        (@userId, @skillId, @rating, @comment, GETDATE())";

                SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userId", Session["userId"]);
                sqlCommand.Parameters.AddWithValue("@skillId", ddlSkillFeedback.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@rating", rblRating.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@comment", txtComment.Text.Trim());

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Thank you! Your feedback was submitted.";

                ddlSkillFeedback.SelectedIndex = 0;
                rblRating.ClearSelection();
                txtComment.Text = "";

                LoadFeedback();
            }
        }
    }
}